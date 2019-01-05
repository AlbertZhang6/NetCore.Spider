using System;
using System.Collections.Generic;
using System.Text;

namespace NetCore.Model.Model
{
    public class LoginResult
    {
        public string UserName { get; set; }

        public LoginStatus CurrentLoginStatus { get; set; }
    }
}
