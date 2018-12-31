using System.Collections;
using System.Collections.Generic;

namespace NetCore.Session
{
    public interface ITypedSession : IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        string Id
        {
            get;
        }

        void Set(string key, object value);

        void Set<T>(string key, T value);

        object Get(string key);

        T Get<T>(string key);

        bool TryGet(string key, out object value);

        bool TryGet<T>(string key, out T value);

        bool HasKey(string key);

        void Remove(string key);

        void Clear();
    }
}
