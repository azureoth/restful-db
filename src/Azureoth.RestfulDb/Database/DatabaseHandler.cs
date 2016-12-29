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
                var response = command.Execute(connection, data);
                connection.Close();
                return response;
            }
        }
    }
}
