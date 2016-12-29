using Azureoth.RestfulDb.Routing;
using System.Data.SqlClient;

namespace Azureoth.RestfulDb.Database
{
    interface IDatabaseCommand
    {
        object Execute(SqlConnection connection, DatabaseRouteData data);
    }

    static class IDatabaseCommandExtensions
    {
        public static string GetTableName(this IDatabaseCommand command, DatabaseRouteData data)
        {
            return $"[{data.AppId}].[{data.Table}]";
        }
    }

}
