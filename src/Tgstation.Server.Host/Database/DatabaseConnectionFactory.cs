﻿using System;
using System.Data.Common;
using System.Data.SqlClient;

using Microsoft.Data.Sqlite;

using MySqlConnector;

using Npgsql;

using Tgstation.Server.Host.Configuration;

namespace Tgstation.Server.Host.Database
{
	/// <inheritdoc />
	sealed class DatabaseConnectionFactory : IDatabaseConnectionFactory
	{
		/// <inheritdoc />
		public DbConnection CreateConnection(string connectionString, DatabaseType databaseType)
		{
			if (connectionString == null)
				throw new ArgumentNullException(nameof(connectionString));

			return databaseType switch
			{
				DatabaseType.MariaDB or DatabaseType.MySql => new MySqlConnection(connectionString),
				DatabaseType.SqlServer => new SqlConnection(connectionString),
				DatabaseType.Sqlite => new SqliteConnection(connectionString),
				DatabaseType.PostgresSql => new NpgsqlConnection(connectionString),
				_ => throw new ArgumentOutOfRangeException(nameof(databaseType), databaseType, "Invalid DatabaseType!"),
			};
		}
	}
}
