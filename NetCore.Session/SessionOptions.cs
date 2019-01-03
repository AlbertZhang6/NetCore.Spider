using System;

namespace NetCore.Session
{
    public class SessionOptions
    {
        public SessionOptions()
        {
            this.IdleTimeout = TimeSpan.FromMinutes(30d);
            this.IOTimeout = TimeSpan.FromSeconds(30d);
            this.HeaderName = "Api-Session";
        }

        public TimeSpan IdleTimeout { get; set; }

        public TimeSpan IOTimeout { get; set; }

        public string HeaderName { get; set; }
    }
}
