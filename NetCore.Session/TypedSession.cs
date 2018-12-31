using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NetCore.Session
{
    internal class TypedSession : ITypedSession, IEnumerable<KeyValuePair<string, object>>, IEnumerable
    {
        private ISession session;
        private Dictionary<string, object> cache;

        public TypedSession(ISession session)
        {
            this.session = session;
            this.cache = new Dictionary<string, object>();
        }

        public string Id
        {
            get { return session.Id; }
        }

        public void Set(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(nameof(key));
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            using (MemoryStream stream = new MemoryStream())
            using (StreamWriter sw = new StreamWriter(stream, Encoding.UTF8))
            using (JsonTextWriter jw = new JsonTextWriter(sw))
            {
                JsonSerializerSettings settings = UseSettings();
                JsonSerializer serializer = JsonSerializer.Create(settings);
                serializer.Serialize(jw, value);
                jw.Flush();

                byte[] data = stream.ToArray();
                session.Set(key, data);
                //add or update cache
                cache[key] = value;
            }
        }

        public void Set<T>(string key, T value)
        {
            Set(key, (object)value);
        }

        public object Get(string key)
        {
            if (TryGet(key, out object value))
                return value;
            else
                return null;
        }

        public T Get<T>(string key)
        {
            if (TryGet<T>(key, out T value))
                return value;
            else
                return default(T);
        }

        public bool TryGet(string key, out object value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(nameof(key));

            if (cache.TryGetValue(key, out value))
                return true;
            bool exists = session.TryGetValue(key, out byte[] data);
            if (!exists)
            {
                value = null;
            }
            else
            {
                using (MemoryStream stream = new MemoryStream(data))
                using (StreamReader sr = new StreamReader(stream, Encoding.UTF8))
                using (JsonTextReader jr = new JsonTextReader(sr))
                {
                    JsonSerializerSettings settings = UseSettings();
                    JsonSerializer serializer = JsonSerializer.Create(settings);
                    value = serializer.Deserialize(jr);
                    cache.Add(key, value);
                }
            }
            return exists;
        }

        public bool TryGet<T>(string key, out T value)
        {
            bool exists = TryGet(key, out object itemValue);
            if (exists)
                value = (T)itemValue;
            else
                value = default(T);
            return exists;
        }

        public bool HasKey(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException(nameof(key));
            return session.Keys.Any(k => k == key);
        }

        public void Remove(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }
            session.Remove(key);
            cache.Remove(key);
        }

        public void Clear()
        {
            session.Clear();
            cache.Clear();
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            foreach (string key in session.Keys)
            {
                object value = Get(key);
                yield return new KeyValuePair<string, object>(key, value);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private JsonSerializerSettings UseSettings()
        {
            return new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.All,
                PreserveReferencesHandling = PreserveReferencesHandling.All
            };
        }
    }

}
