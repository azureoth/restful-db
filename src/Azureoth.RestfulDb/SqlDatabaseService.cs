using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;

namespace Azureoth.RestfulDb
{
    public sealed class SqlDatabaseService : IDatabaseService
    {
        private readonly static string ConnectionString = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Azureoth;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private readonly static Regex NameValidator = new Regex("^[a-zA-Z0-9_]*$", RegexOptions.Compiled);

        public object Get<T>(string appId, string table, T id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = $"SELECT * FROM [{appId}].[{table}] WHERE [Id] = @ID";

                var command = new SqlCommand(query, connection);
                command.Parameters.Add("@ID", SqlDbType.Int);
                command.Parameters["@ID"].Value = id;

                connection.Open();

                var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    var properties = new Dictionary<string, object>();

                    int fields = reader.FieldCount;

                    for (int i = 0; i < fields; i++)
                    {
                        properties[reader.GetName(i)] = reader[i];
                    }

                    return properties;
                }

                connection.Close();
                return null;
            }
        }

        public IEnumerable<object> Get(string appId, string table)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = $"SELECT * FROM [{appId}].[{table}]";

                var command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                var records = new List<Dictionary<string, object>>();

                while (reader.Read())
                {
                    var properties = new Dictionary<string, object>();

                    int fields = reader.FieldCount;

                    for (int i = 0; i < fields; i++)
                    {
                        properties[reader.GetName(i)] = reader[i];
                    }

                    records.Add(properties);
                }

                connection.Close();
                return records;
            }
        }

        public T Insert<T>(string appId, string table, IDictionary<string, object> data)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var keys = string.Join(",", data.Select(i => i.Key));
                var parameters = string.Join(",", data.Select(i => "@" + i.Key));
                var query = $"INSERT INTO [{appId}].[{table}] ({keys}) VALUES ({parameters}); SELECT SCOPE_IDENTITY();";

                var command = new SqlCommand(query, connection);

                foreach (var item in data)
                {
                    command.Parameters.AddWithValue("@" + item.Key, item.Value);
                }

                connection.Open();
                var id = (T)command.ExecuteScalar();
                connection.Close();

                return id;
            }
        }

        public void Update<T>(string appId, string table, T id, IDictionary<string, object> data)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var parameters = string.Join(",", data.Select(i => $"{i.Key}=@{i.Key}"));
                var query = $"UPDATE [{appId}].[{table}] SET {parameters}  WHERE [Id] = @ID;";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);

                foreach (var item in data)
                {
                    command.Parameters.AddWithValue($"@{item.Key}", item.Value);
                }

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        public void Delete<T>(string appId, string table, T id)
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = $"DELETE FROM [{appId}].[{table}] WHERE [Id] = @ID";

                var command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@ID", id);

                connection.Open();
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
    }
}
