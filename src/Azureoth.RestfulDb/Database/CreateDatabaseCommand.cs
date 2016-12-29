using System.Data.SqlClient;
using Azureoth.RestfulDb.Routing;
using System.Collections.Generic;
using System.Linq;

namespace Azureoth.RestfulDb.Database
{
    class CreateDatabaseCommand : IDatabaseCommand
    {
        public object Execute(SqlConnection connection, DatabaseRouteData data)
        {
            var table = this.GetTableName(data);

            var keys = string.Join(",", data.Data.Select(i => $"\"{i.Key}\""));
            var parameters = string.Join(",", data.Data.Select(i => "@" + i.Key));
            var query = $"INSERT INTO {table} ({keys}) VALUES ({parameters}); SELECT SCOPE_IDENTITY();";

            using (var command = new SqlCommand(query, connection))
            {
                foreach (var item in data.Data)
                {
                    command.Parameters.AddWithValue("@" + item.Key, item.Value);
                }

                return command.ExecuteScalar();
            }
        }
    }
}
