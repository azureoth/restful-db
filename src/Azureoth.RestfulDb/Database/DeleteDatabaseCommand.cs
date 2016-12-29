using System.Data.SqlClient;
using Azureoth.RestfulDb.Routing;

namespace Azureoth.RestfulDb.Database
{
    class DeleteDatabaseCommand : IDatabaseCommand
    {
        public object Execute(SqlConnection connection, DatabaseRouteData data)
        {
            var table = this.GetTableName(data);

            var query = $"DELETE FROM {table} WHERE [Id] = @ID";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", data.Key);
                command.ExecuteNonQuery();
            }

            return null;
        }
    }
}
