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
        public async Task<LoginResult> Login(LoginModel model)
        {
            return new LoginResult() {  CurrentLoginStatus = LoginStatus.Success};
        }

        public Task Logout()
        {
            throw new NotImplementedException();
        }
    }
}
