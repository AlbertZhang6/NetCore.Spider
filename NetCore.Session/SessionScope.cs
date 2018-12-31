using Microsoft.AspNetCore.Http;

namespace NetCore.Session
{
    internal class SessionScope
    {
        private bool establish = true;

        public ISession Session
        {
            get;
            set;
        }

        public string Key
        {
            get;
            set;
        }

        public bool TryEstablish()
        {
            return establish;
        }

        public void Complete()
        {
            establish = false;
        }
    }
}
