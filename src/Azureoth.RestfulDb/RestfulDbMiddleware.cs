using System;
using System.Threading.Tasks;
using Azureoth.RestfulDb.Database;
using Azureoth.RestfulDb.Routing;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Azureoth.RestfulDb
{
    internal class RestfulDbMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IRouteResolver routeResolver;
        private readonly IDatabaseHandler dbHandler;

        public RestfulDbMiddleware(RequestDelegate next, IRouteResolver routeResolver, IDatabaseHandler dbHandler)
        {
            this.next = next;
            this.routeResolver = routeResolver;
            this.dbHandler = dbHandler;
        }

        public async Task Invoke(HttpContext context)
        {
            if (this.routeResolver.ShouldResolve(context.Request))
            {
                var data = this.routeResolver.Resolve(context.Request);

                try
                {
                    var response = this.dbHandler.Execute(data);

                    context.Response.StatusCode = 200;

                    if (response != null)
                    {
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
                    }
                }
                catch(TableNotFoundException)
                {
                    context.Response.StatusCode = 404;
                }
                catch
                {
                    context.Response.StatusCode = 500;
                }
            }
            else
            {
                await this.next.Invoke(context);
            }
        }
    }

    public static class RestfulDbMiddlewareExtensions
    {
        public static IApplicationBuilder UseRestfulDb(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RestfulDbMiddleware>();
        }

        public static IServiceCollection ConfigureRestfulDb(this IServiceCollection services, string connectionString, string apiPrefix)
        {
            return services
                .AddTransient<IRouteResolver>(s => new RouteResolver(apiPrefix))
                .AddTransient<IDatabaseHandler>(s => new DatabaseHandler(connectionString));
        }
    }
}
