using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Azureoth.RestfulDb.Routing
{
    class RouteResolver : IRouteResolver
    {
        private readonly string apiPrefix;

        public RouteResolver(string apiPrefix)
        {
            this.apiPrefix = apiPrefix?.ToLowerInvariant() ?? "/api/db";
        }

        public bool ShouldResolve(HttpRequest request)
        {
            return request.Path.Value.ToLowerInvariant().StartsWith(apiPrefix);
        }

        public DatabaseRouteData Resolve(HttpRequest request)
        {
            var path = request.Path.Value.Substring(this.apiPrefix.Length);

            var parts = path.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 2)
            {
                throw new FormatException();
            }

            var data = new DatabaseRouteData
            {
                AppId = parts[0],
                Table = parts[1]
            };

            if (parts.Length > 2)
            {
                data.Key = parts[2]; 
            }

            data.Method = request.Method;

            if (request.Body != null && (StringComparer.OrdinalIgnoreCase.Equals(request.Method, "POST") || StringComparer.OrdinalIgnoreCase.Equals(request.Method, "PUT")))
            {
                if (!StringComparer.OrdinalIgnoreCase.Equals(request.ContentType, "application/json"))
                {
                    throw new FormatException("Restful Database only support application/json content type");
                }

                using (var memstream = new MemoryStream())
                {
                    request.Body.CopyTo(memstream);
                    memstream.Position = 0;
                    using (var reader = new StreamReader(memstream))
                    {
                        string text = reader.ReadToEnd();
                        data.Data = JsonConvert.DeserializeObject<Dictionary<string, object>>(text);
                    }
                }
            }

            return data;
        }
    }
}
