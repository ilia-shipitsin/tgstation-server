﻿using Tgstation.Server.Api.Models;
using Tgstation.Server.Client.Rights;

namespace Tgstation.Server.Client
{
	/// <summary>
	/// <see cref="IClient{TRights}"/> for server instances
	/// </summary>
	public interface IInstanceClient : IClient<InstanceRights>
	{
		/// <summary>
		/// The <see cref="Instance"/> of the <see cref="IInstanceClient"/>
		/// </summary>
		Instance Metadata { get; }
	}
}