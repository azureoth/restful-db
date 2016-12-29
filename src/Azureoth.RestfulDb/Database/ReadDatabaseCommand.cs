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

            if (data.NavigationTable != null)
            {
                return this.ReadJoinedRecords(connection, data.AppId, data.Table, data.NavigationTable, data.Key);
            }
            else if (data.Key != null)
            {
                return this.ReadRecord(connection, table, data.Key);
            }
            else
            {
                return this.ReadRecords(connection, table);
            }
        }

        public object ReadJoinedRecords(SqlConnection connection, string appId, string table, string navTable, object key)
        {
            var columnName = this.GetForeignKeyColumn(connection, appId, table, navTable);

            var query = $"SELECT * FROM [{appId}].[{navTable}] WHERE [{columnName}] = @ID ORDER BY [Id];";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ID", key);
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

        public object ReadRecord(SqlConnection connection, string table, object id)
        {
            var query = $"SELECT * FROM {table} WHERE [Id] = @ID ORDER BY [Id];";

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

        private string GetForeignKeyColumn(SqlConnection connection, string appId, string table, string navTable)
        {
            string fkName1 = $"fk_{table}_{navTable}";
            string fkName2 = $"fk_{navTable}_{table}";

            var query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE WHERE TABLE_SCHEMA = '{appId}' AND TABLE_NAME = '{navTable}' AND (CONSTRAINT_NAME = @FK1 OR CONSTRAINT_NAME = @FK2) ORDER BY TABLE_NAME, ORDINAL_POSITION;";

            using (var command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FK1", fkName1);
                command.Parameters.AddWithValue("@FK2", fkName2);
                return command.ExecuteScalar().ToString();
            }
        }
    }
}
