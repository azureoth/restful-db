using System.Collections.Generic;

namespace Azureoth.Router
{
    public interface ISqlManager
    {
        IEnumerable<object> Get(string appId, string table);

        object Get(string appId, string table, int id);

        decimal Insert(string appId, string table, IDictionary<string, object> data);

        void Update(string appId, string table, int id, IDictionary<string, object> data);

        void Delete(string appId, string table, int id);
    }
}
