using Azureoth.RestfulDb.Routing;

namespace Azureoth.RestfulDb.Database
{
    interface IDatabaseHandler
    {
        object Execute(DatabaseRouteData data);
    }
}