﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using Newtonsoft.Json;

using Tgstation.Server.Common;
using Tgstation.Server.Host.Configuration;
using Tgstation.Server.Host.Controllers;

namespace Tgstation.Server.Host.Swarm.Tests
{
	sealed class SwarmRpcMapper : IDisposable
	{
		public bool AsyncRequests { get; set; }

		readonly ILogger logger;

		readonly Func<SwarmService, SwarmController> createSwarmController;

		List<(SwarmConfiguration, TestableSwarmNode)> configToNodes;

		int serverErrorCount;

		public SwarmRpcMapper(Func<SwarmService, SwarmController> createSwarmController, Mock<IHttpClient> clientMock, ILogger logger)
		{
			this.createSwarmController = createSwarmController;
			clientMock
				.Setup(x => x.SendAsync(It.IsNotNull<HttpRequestMessage>(), It.IsAny<CancellationToken>()))
				.Returns(MapRequest);
			this.logger = logger;
			AsyncRequests = true;
		}

		public void Dispose()
		{
			Assert.AreEqual(0, serverErrorCount);
		}

		public void Register(List<(SwarmConfiguration, TestableSwarmNode)> configToNodes)
		{
			this.configToNodes = configToNodes;
		}

		async Task<HttpResponseMessage> MapRequest(
			HttpRequestMessage request,
			CancellationToken cancellationToken)
		{
			var (config, node) = configToNodes.FirstOrDefault(
				pair => pair.Item1.Address.IsBaseOf(request.RequestUri));

			if (config == default)
				Assert.Fail($"Invalid node address: {request.RequestUri}");

			if (!node.WebServerOpen)
			{
				throw new HttpRequestException("Can't connect to uninitialized node!");
			}

			if (node.Shutdown)
			{
				throw new HttpRequestException("Can't connect to shutdown node!");
			}

			var controller = createSwarmController(node.Service);

			Type targetAttribute = null;
			bool isDataRequest = false;
			switch (request.Method.Method.ToUpperInvariant())
			{
				case "GET":
					targetAttribute = typeof(HttpGetAttribute);
					break;
				case "POST":
					targetAttribute = typeof(HttpPostAttribute);
					isDataRequest = true;
					break;
				case "PUT":
					targetAttribute = typeof(HttpPutAttribute);
					isDataRequest = true;
					break;
				case "DELETE":
					targetAttribute = typeof(HttpDeleteAttribute);
					break;
				case "PATCH":
					targetAttribute = typeof(HttpPatchAttribute);
					isDataRequest = true;
					break;
				default:
					Assert.Fail($"Unknown request method: {request.Method.Method}");
					break;
			}

			var stringUrl = request.RequestUri.ToString();
			var rootIndex = stringUrl.IndexOf(SwarmConstants.ControllerRoute);
			if (rootIndex == -1)
				Assert.Fail($"Invalid Swarm route: {stringUrl}");

			var route = stringUrl[(rootIndex + SwarmConstants.ControllerRoute.Length)..].TrimStart('/');

			var controllerMethod = controller
				.GetType()
				.GetMethods()
				.Select(method => (method, (HttpMethodAttribute)method.GetCustomAttribute(targetAttribute)))
				.Where(pair => pair.Item2 != null
					&& pair.Item2.HttpMethods.Count() == 1
					&& pair.Item2.HttpMethods.All(supportedMethod => supportedMethod.Equals(request.Method.Method))
					&& (pair.Item2.Template ?? String.Empty) == route)
				.Select(pair => pair.method)
				.SingleOrDefault();

			if (controllerMethod == default)
				Assert.Fail($"SwarmController has no method with attribute {targetAttribute}!");

			IActionResult result;
			var hasRegistrationHeader = request.Headers.TryGetValues(SwarmConstants.RegistrationIdHeader, out var values)
				&& values.Count() == 1;
			var response = new HttpResponseMessage();
			try
			{
				// We're not testing OnActionExecutingAsync, that's covered by integration.
				if (hasRegistrationHeader)
				{
					var mockRequest = new Mock<HttpRequest>();
					mockRequest.SetupGet(x => x.Headers).Returns(new HeaderDictionary
					{
						{
							SwarmConstants.RegistrationIdHeader,
							new StringValues(values.First())
						},
					});
					var mockHttpContext = new Mock<HttpContext>();
					mockHttpContext.SetupGet(x => x.Request).Returns(mockRequest.Object);

					controller
						.ControllerContext
						.HttpContext = mockHttpContext.Object;

					var args = new List<object>();
					if (isDataRequest && request.Content != null)
					{
						var dataType = controllerMethod.GetParameters().First().ParameterType;
						var json = await request.Content.ReadAsStringAsync(cancellationToken);
						var parameter = JsonConvert.DeserializeObject(json, dataType, SwarmService.SerializerSettings);
						args.Add(parameter);
					}

					if (AsyncRequests)
						await Task.Yield();

					if (controllerMethod.ReturnType != typeof(IActionResult))
					{
						Assert.AreEqual(typeof(Task<IActionResult>), controllerMethod.ReturnType);
						args.Add(cancellationToken);
						var invocationTask = (Task<IActionResult>)controllerMethod.Invoke(controller, args.ToArray());
						result = await invocationTask;
					}
					else
					{
						result = (IActionResult)controllerMethod.Invoke(controller, args.ToArray());

						// simulate worst case, request completed but was aborted before server replied
						cancellationToken.ThrowIfCancellationRequested();
					}
				}
				else
				{
					result = controller.BadRequest();
				}

				// manually checked all controller response types
				// Fobid, NoContent, Conflict, StatusCode
				if (result is ForbidResult forbidResult)
					response.StatusCode = HttpStatusCode.Forbidden;
				else if (result is IStatusCodeActionResult statusCodeResult)
					response.StatusCode = (HttpStatusCode)statusCodeResult.StatusCode;
				else
				{
					response.Dispose();
					Assert.Fail($"Unrecognized result type: {result.GetType()}");
				}

				return response;
			}
			catch (Exception ex)
			{
				if (ex is not OperationCanceledException)
				{
					logger.LogCritical(ex, "Error in request to {nodeId}!", config.Identifier);
					++serverErrorCount;
				}

				response.Dispose();
				throw;
			}
		}
	}
}
