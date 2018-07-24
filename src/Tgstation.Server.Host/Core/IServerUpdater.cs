﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Tgstation.Server.Host.IO;

namespace Tgstation.Server.Host.Core
{
	/// <summary>
	/// Represents a service that may take an updated <see cref="Host"/> assembly and run it, stopping the current assembly in the process
	/// </summary>
    interface IServerUpdater
    {
		/// <summary>
		/// Run a new <see cref="Host"/> assembly and stop the current one. This will likely trigger all active <see cref="System.Threading.CancellationToken"/>s
		/// </summary>
		/// <param name="updateZipData">The <see cref="byte"/>s of the .zip file that contains the new <see cref="Host"/> assembly</param>
		/// <param name="cancellationToken">The <see cref="CancellationToken"/> for the operation</param>
		/// <param name="ioManager">The <see cref="IIOManager"/> for the operation</param>
		/// <returns>A <see cref="Task"/> representing the running operation</returns>
		Task ApplyUpdate(byte[] updateZipData, IIOManager ioManager, CancellationToken cancellationToken);

		/// <summary>
		/// Register a given <paramref name="action"/> to run before stopping the server for updates
		/// </summary>
		/// <param name="action">The <see cref="Action"/> to run</param>
		void RegisterForUpdate(Action action);
    }
}
