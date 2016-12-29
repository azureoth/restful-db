using Microsoft.AspNetCore.Http;

namespace Azureoth.RestfulDb.Routing
{
    interface IRouteResolver
    {
        bool ShouldResolve(HttpRequest request);

        DatabaseRouteData Resolve(HttpRequest request);
    }
}
