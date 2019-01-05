using NetCore.Model.Model;
using NetCore.Spider.WebApi.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.WebApi.Business
{
    public class SecurityService : ISecurity
    {
        public Task<LoginResult> Login(LoginModel model)
        {
            throw new NotImplementedException();
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
