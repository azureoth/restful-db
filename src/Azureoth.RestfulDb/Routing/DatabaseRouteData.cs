using System;
using System.Collections.Generic;

namespace Azureoth.RestfulDb.Routing
{
    class DatabaseRouteData
    {
        public string Method { get; set; }

        public string AppId { get; set; }

        public string Table { get; set; }

        public object Key { get; set; }

        public IDictionary<string, object> Data { get; set; }
    }
}
