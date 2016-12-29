using System.Data.SqlClient;
using Azureoth.RestfulDb.Routing;
using System.Collections.Generic;

namespace Azureoth.RestfulDb.Database
{
    class ReadDatabaseCommand : IDatabaseCommand
    {
        public object Execute(SqlConnection connection, DatabaseRouteData data)
        {
            var table = this.GetTableName(data);

            if (data.Key != null)
            {
                return this.ReadRecord(connection, table, data.Key);
            }
            else
            {
                return this.ReadRecords(connection, table);
            }
        }

        public object ReadRecord(SqlConnection connection, string table, object id)
        {
            var query = $"SELECT * FROM {table} WHERE [Id] = @ID;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", id);
                using (var reader = command.ExecuteReader())
                {
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

                    return null;
                }
            }
        }

        public object ReadRecords(SqlConnection connection, string table)
        {
            var query = $"SELECT * FROM {table};";

            using (var command = new SqlCommand(query, connection))
            {
                using (var reader = command.ExecuteReader())
                {
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

                    return records;
                }
            }
        }
    }
}
