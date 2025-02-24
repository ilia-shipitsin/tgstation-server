﻿using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.Logging;

using Tgstation.Server.Host.IO;

namespace Tgstation.Server.Host.Components.Byond
{
	/// <inheritdoc />
	abstract class ByondInstallerBase : IByondInstaller
	{
		/// <summary>
		/// The name of BYOND's cache directory.
		/// </summary>
		const string CacheDirectoryName = "cache";

		/// <inheritdoc />
		public abstract string DreamMakerName { get; }

		/// <inheritdoc />
		public abstract string PathToUserByondFolder { get; }

		/// <summary>
		/// Gets the URL formatter string for downloading a byond version of {0:Major} {1:Minor}.
		/// </summary>
		protected abstract string ByondRevisionsUrlTemplate { get; }

		/// <summary>
		/// Gets the <see cref="IIOManager"/> for the <see cref="ByondInstallerBase"/>.
		/// </summary>
		protected IIOManager IOManager { get; }

		/// <summary>
		/// Gets the <see cref="ILogger"/> for the <see cref="ByondInstallerBase"/>.
		/// </summary>
		protected ILogger<ByondInstallerBase> Logger { get; }

		/// <summary>
		/// The <see cref="IFileDownloader"/> for the <see cref="ByondInstallerBase"/>.
		/// </summary>
		readonly IFileDownloader fileDownloader;

		/// <summary>
		/// Initializes a new instance of the <see cref="ByondInstallerBase"/> class.
		/// </summary>
		/// <param name="ioManager">The value of <see cref="IOManager"/>.</param>
		/// <param name="fileDownloader">The value of <see cref="fileDownloader"/>.</param>
		/// <param name="logger">The value of <see cref="Logger"/>.</param>
		protected ByondInstallerBase(IIOManager ioManager, IFileDownloader fileDownloader, ILogger<ByondInstallerBase> logger)
		{
			IOManager = ioManager ?? throw new ArgumentNullException(nameof(ioManager));
			this.fileDownloader = fileDownloader ?? throw new ArgumentNullException(nameof(fileDownloader));
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		public abstract string GetDreamDaemonName(Version version, out bool supportsCli);

		/// <inheritdoc />
		public async Task CleanCache(CancellationToken cancellationToken)
		{
			try
			{
				Logger.LogDebug("Cleaning BYOND cache...");
				await IOManager.DeleteDirectory(
					IOManager.ConcatPath(
						PathToUserByondFolder,
						CacheDirectoryName),
					cancellationToken)
					;
			}
			catch (OperationCanceledException)
			{
				throw;
			}
			catch (Exception e)
			{
				Logger.LogWarning(e, "Error deleting BYOND cache!");
			}
		}

		/// <inheritdoc />
		public abstract Task InstallByond(Version version, string path, CancellationToken cancellationToken);

		/// <inheritdoc />
		public abstract Task UpgradeInstallation(Version version, string path, CancellationToken cancellationToken);

		/// <inheritdoc />
		public Task<MemoryStream> DownloadVersion(Version version, CancellationToken cancellationToken)
		{
			if (version == null)
				throw new ArgumentNullException(nameof(version));

			Logger.LogTrace("Downloading BYOND version {major}.{minor}...", version.Major, version.Minor);
			var url = String.Format(CultureInfo.InvariantCulture, ByondRevisionsUrlTemplate, version.Major, version.Minor);
			return fileDownloader.DownloadFile(new Uri(url), null, cancellationToken);
		}
	}
}
