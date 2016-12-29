using Azureoth.RestfulDb.Routing;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Azureoth.RestfulDb.Database
{
    class DatabaseHandler : IDatabaseHandler
    {
        private readonly static IDictionary<string, IDatabaseCommand> Commands = new Dictionary<string, IDatabaseCommand>(StringComparer.OrdinalIgnoreCase)
        {
            { "GET", new ReadDatabaseCommand() },
            { "POST", new CreateDatabaseCommand() },
            { "PUT", new UpdateDatabaseCommand() },
            { "DELETE", new DeleteDatabaseCommand() }
        };

        private readonly string connectionString;

        public DatabaseHandler(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public object Execute(DatabaseRouteData data)
        {
            IDatabaseCommand command;

            if (!Commands.TryGetValue(data.Method, out command))
            {
                throw new NotSupportedException("The specified HTTP method is not supported by the Restful Database Layer.");
            }

            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();

                EnsureTableExists(connection, data.AppId, data.Table);

                if (data.NavigationTable != null)
                {
                    EnsureTableExists(connection, data.AppId, data.NavigationTable);
                }

                var response = command.Execute(connection, data);
                connection.Close();
                return response;
            }
        }

        private void EnsureTableExists(SqlConnection connection, string appId, string table)
        {
            var query = $"SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{appId}' AND TABLE_NAME = '{table}';";
            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
                   if (!reader.Read())
                    {
                        throw new TableNotFoundException("The specified table was not found");
                    }
                }
            }
        }
    }
}
