using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace NetCore.Session
{
    public class SessionPair
    {
        [JsonIgnore]
        public ISession Session
        {
            get;
            set;
        }

        [JsonProperty("session_key")]
        public string Key
        {
            get;
            set;
        }
    }

}
