using System.Collections.Generic;

namespace Azureoth.RestfulDb
{
    public interface IDatabaseService
    {
        IEnumerable<object> Get(string appId, string table);

        object Get<T>(string appId, string table, T id);

        T Insert<T>(string appId, string table, IDictionary<string, object> data);

        void Update<T>(string appId, string table, T id, IDictionary<string, object> data);

        void Delete<T>(string appId, string table, T id);
    }
}
