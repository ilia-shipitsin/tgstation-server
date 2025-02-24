﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Tgstation.Server.Host.Configuration;
using Tgstation.Server.Host.Core;

namespace Tgstation.Server.Host
{
	/// <inheritdoc />
	sealed class Server : IServer, IServerControl
	{
		/// <inheritdoc />
		public bool RestartRequested { get; private set; }

		/// <inheritdoc />
		public bool UpdateInProgress { get; private set; }

		/// <inheritdoc />
		public bool WatchdogPresent =>
#if WATCHDOG_FREE_RESTART
			true;
#else
			updatePath != null;
#endif

		/// <summary>
		/// The <see cref="IHost"/> of the running server.
		/// </summary>
		internal IHost Host { get; private set; }

		/// <summary>
		/// The <see cref="IHostBuilder"/> for the <see cref="Server"/>.
		/// </summary>
		readonly IHostBuilder hostBuilder;

		/// <summary>
		/// The <see cref="IRestartHandler"/>s to run when the <see cref="Server"/> restarts.
		/// </summary>
		readonly List<IRestartHandler> restartHandlers;

		/// <summary>
		/// The absolute path to install updates to.
		/// </summary>
		readonly string updatePath;

		/// <summary>
		/// <see langword="lock"/> <see cref="object"/> for certain restart related operations.
		/// </summary>
		readonly object restartLock;

		/// <summary>
		/// The <see cref="ILogger"/> for the <see cref="Server"/>.
		/// </summary>
		ILogger<Server> logger;

		/// <summary>
		/// The <see cref="GeneralConfiguration"/> for the <see cref="Server"/>.
		/// </summary>
		GeneralConfiguration generalConfiguration;

		/// <summary>
		/// The <see cref="cancellationTokenSource"/> for the <see cref="Server"/>.
		/// </summary>
		CancellationTokenSource cancellationTokenSource;

		/// <summary>
		/// The <see cref="Exception"/> to propagate when the server terminates.
		/// </summary>
		Exception propagatedException;

		/// <summary>
		/// The <see cref="Task"/> that is used for asynchronously updating the server.
		/// </summary>
		Task updateTask;

		/// <summary>
		/// If the server is being shut down or restarted.
		/// </summary>
		bool shutdownInProgress;

		/// <summary>
		/// If there is an update in progress and this flag is set, it should stop the server immediately if it fails.
		/// </summary>
		bool terminateIfUpdateFails;

		/// <summary>
		/// Initializes a new instance of the <see cref="Server"/> class.
		/// </summary>
		/// <param name="hostBuilder">The value of <see cref="hostBuilder"/>.</param>
		/// <param name="updatePath">The value of <see cref="updatePath"/>.</param>
		public Server(IHostBuilder hostBuilder, string updatePath)
		{
			this.hostBuilder = hostBuilder ?? throw new ArgumentNullException(nameof(hostBuilder));
			this.updatePath = updatePath;

			hostBuilder.ConfigureServices(serviceCollection => serviceCollection.AddSingleton<IServerControl>(this));

			restartHandlers = new List<IRestartHandler>();
			restartLock = new object();
		}

		/// <inheritdoc />
		public async Task Run(CancellationToken cancellationToken)
		{
			using (cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken))
			using (var fsWatcher = updatePath != null ? new FileSystemWatcher(Path.GetDirectoryName(updatePath)) : null)
			{
				if (fsWatcher != null)
				{
					fsWatcher.Created += WatchForShutdownFileCreation;
					fsWatcher.EnableRaisingEvents = true;
				}

				try
				{
					using (Host = hostBuilder.Build())
					{
						try
						{
							logger = Host.Services.GetRequiredService<ILogger<Server>>();
							using (cancellationToken.Register(() => logger.LogInformation("Server termination requested!")))
							{
								var generalConfigurationOptions = Host.Services.GetRequiredService<IOptions<GeneralConfiguration>>();
								generalConfiguration = generalConfigurationOptions.Value;
								await Host.RunAsync(cancellationTokenSource.Token);
							}

							if (updateTask != null)
								await updateTask;
						}
						catch (OperationCanceledException ex)
						{
							logger.LogDebug(ex, "Server run cancelled!");
						}
						catch (Exception ex)
						{
							CheckExceptionPropagation(ex);
							throw;
						}
						finally
						{
							logger = null;
						}
					}
				}
				finally
				{
					Host = null;
				}
			}

			CheckExceptionPropagation(null);
		}

		/// <inheritdoc />
		public bool TryStartUpdate(IServerUpdateExecutor updateExecutor, Version newVersion)
		{
			if (updateExecutor == null)
				throw new ArgumentNullException(nameof(updateExecutor));
			if (newVersion == null)
				throw new ArgumentNullException(nameof(newVersion));

			CheckSanity(true);

			logger.LogTrace("Begin ApplyUpdate...");

			CancellationToken criticalCancellationToken;
			lock (restartLock)
			{
				if (UpdateInProgress || shutdownInProgress)
				{
					logger.LogDebug("Aborted update due to concurrency conflict!");
					return false;
				}

				if (cancellationTokenSource == null)
					throw new InvalidOperationException("Tried to update a non-running Server!");

				criticalCancellationToken = cancellationTokenSource.Token;
				UpdateInProgress = true;
			}

			async Task RunUpdate()
			{
				if (await updateExecutor.ExecuteUpdate(updatePath, criticalCancellationToken, criticalCancellationToken))
				{
					logger.LogTrace("Update complete!");
					await Restart(newVersion, null, true);
				}
				else if (terminateIfUpdateFails)
				{
					logger.LogTrace("Stopping host due to termination request...");
					cancellationTokenSource.Cancel();
				}
				else
				{
					logger.LogTrace("Update failed!");
					UpdateInProgress = false;
				}
			}

			updateTask = RunUpdate();
			return true;
		}

		/// <inheritdoc />
		public IRestartRegistration RegisterForRestart(IRestartHandler handler)
		{
			if (handler == null)
				throw new ArgumentNullException(nameof(handler));

			CheckSanity(false);

			lock (restartLock)
				if (!shutdownInProgress)
				{
					logger.LogTrace("Registering restart handler {handlerImplementationName}...", handler);
					restartHandlers.Add(handler);
					return new RestartRegistration(() =>
					{
						lock (restartLock)
							if (!shutdownInProgress)
								restartHandlers.Remove(handler);
					});
				}

			logger.LogWarning("Restart handler {handlerImplementationName} register after a shutdown had begun!", handler);
			return new RestartRegistration(null);
		}

		/// <inheritdoc />
		public Task Restart() => Restart(null, null, true);

		/// <inheritdoc />
		public Task GracefulShutdown() => Restart(null, null, false);

		/// <inheritdoc />
		public Task Die(Exception exception) => Restart(null, exception, false);

		/// <summary>
		/// Throws an <see cref="InvalidOperationException"/> if the <see cref="IServerControl"/> cannot be used.
		/// </summary>
		/// <param name="checkWatchdog">If <see cref="WatchdogPresent"/> should be checked.</param>
		void CheckSanity(bool checkWatchdog)
		{
			if (checkWatchdog && !WatchdogPresent && propagatedException == null)
				throw new InvalidOperationException("Server restarts are not supported");

			if (cancellationTokenSource == null || logger == null)
				throw new InvalidOperationException("Tried to control a non-running Server!");
		}

		/// <summary>
		/// Re-throw <see cref="propagatedException"/> if it exists.
		/// </summary>
		/// <param name="otherException">An existing <see cref="Exception"/> that should be thrown as well, but not by itself.</param>
		void CheckExceptionPropagation(Exception otherException)
		{
			if (propagatedException == null)
				return;

			if (otherException != null)
				throw new AggregateException(propagatedException, otherException);

			throw propagatedException;
		}

		/// <summary>
		/// Implements <see cref="Restart()"/>.
		/// </summary>
		/// <param name="newVersion">The <see cref="Version"/> of any potential updates being applied.</param>
		/// <param name="exception">The potential value of <see cref="propagatedException"/>.</param>
		/// <param name="requireWatchdog">If the host watchdog is required for this "restart".</param>
		/// <returns>A <see cref="Task"/> representing the running operation.</returns>
		async Task Restart(Version newVersion, Exception exception, bool requireWatchdog)
		{
			CheckSanity(requireWatchdog);

			// if the watchdog isn't required and there's no issue, this is just a graceful shutdown
			bool isGracefulShutdown = !requireWatchdog && exception == null;
			logger.LogTrace(
				"Begin {restartType}...",
				isGracefulShutdown
					? "graceful shutdown"
					: "restart");

			lock (restartLock)
			{
				if ((UpdateInProgress && newVersion == null) || shutdownInProgress)
				{
					logger.LogTrace("Aborted restart due to concurrency conflict!");
					return;
				}

				RestartRequested = !isGracefulShutdown;
				propagatedException ??= exception;
			}

			if (exception == null)
			{
				logger.LogInformation("Stopping server...");
				using var cts = new CancellationTokenSource(
					TimeSpan.FromMinutes(
						isGracefulShutdown
							? generalConfiguration.ShutdownTimeoutMinutes
							: generalConfiguration.RestartTimeoutMinutes));
				var cancellationToken = cts.Token;
				try
				{
					var eventsTask = Task.WhenAll(
						restartHandlers.Select(
							x => x.HandleRestart(newVersion, isGracefulShutdown, cancellationToken))
						.ToList());

					logger.LogTrace("Joining restart handlers...");
					await eventsTask;
				}
				catch (OperationCanceledException ex)
				{
					if (isGracefulShutdown)
						logger.LogWarning(ex, "Graceful shutdown timeout hit! Existing DreamDaemon processes will be terminated!");
					else
						logger.LogError(
							ex,
							"Restart timeout hit! Existing DreamDaemon processes will be lost and must be killed manually before being restarted with TGS!");
				}
				catch (Exception e)
				{
					logger.LogError(e, "Restart handlers error!");
				}
			}

			StopServerImmediate();
		}

		/// <summary>
		/// Event handler for the <see cref="updatePath"/>'s <see cref="FileSystemWatcher"/>. Triggers shutdown if requested by host watchdog.
		/// </summary>
		/// <param name="sender">The <see cref="object"/> that sent the event.</param>
		/// <param name="eventArgs">The <see cref="FileSystemEventArgs"/>.</param>
		void WatchForShutdownFileCreation(object sender, FileSystemEventArgs eventArgs)
		{
			logger?.LogTrace("FileSystemWatcher triggered.");

			// TODO: Refactor this to not use System.IO function here.
			if (eventArgs.FullPath == Path.GetFullPath(updatePath) && File.Exists(eventArgs.FullPath))
			{
				logger?.LogInformation("Host watchdog appears to be requesting server termination!");
				lock (restartLock)
				{
					if (!UpdateInProgress)
					{
						StopServerImmediate();
						return;
					}

					terminateIfUpdateFails = true;
				}

				logger?.LogInformation("An update is in progress, we will wait for that to complete...");
			}
		}

		/// <summary>
		/// Fires off the <see cref="cancellationTokenSource"/> without any checks, shutting down everything.
		/// </summary>
		void StopServerImmediate()
		{
			shutdownInProgress = true;
			logger.LogTrace("Stopping host...");
			cancellationTokenSource.Cancel();
		}
	}
}
