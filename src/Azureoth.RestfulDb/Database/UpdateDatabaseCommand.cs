using System.Data.SqlClient;
using Azureoth.RestfulDb.Routing;
using System.Collections.Generic;
using System.Linq;

namespace Azureoth.RestfulDb.Database
{
    class UpdateDatabaseCommand : IDatabaseCommand
    {
        public object Execute(SqlConnection connection, DatabaseRouteData data)
        {
            var table = this.GetTableName(data);

            var parameters = string.Join(",", data.Data.Select(i => $"{i.Key}=@{i.Key}"));
            var query = $"UPDATE {table} SET {parameters}  WHERE [Id] = @ID;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", data.Key);
                foreach (var item in data.Data)
                {
                    command.Parameters.AddWithValue("@" + item.Key, item.Value);
                }

                return command.ExecuteScalar();
            }
        }
    }
}
