using NetCore.Model.Model;
using NetCore.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.WebApi.ViewModel
{
    public class LoginReply : ApiToken
    {
        public LoginStatus CurrentLoginStatus { get; set; }
    }
}
