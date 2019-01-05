using Microsoft.AspNetCore.Identity.UI.Pages.Account.Internal;
using NetCore.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoginModel = NetCore.Model.Model.LoginModel;

namespace NetCore.Spider.WebApi.Interfaces
{
    public interface ISecurity
    {
        Task<LoginResult> Login(LoginModel model);

        Task Logout();
    }
}
