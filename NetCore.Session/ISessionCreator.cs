using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Session
{
    public interface ISessionCreator
    {
        SessionPair Create();
    }
}
