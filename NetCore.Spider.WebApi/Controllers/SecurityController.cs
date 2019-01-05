using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetCore.Jwt;
using NetCore.Model.Model;
using NetCore.Session;
using NetCore.Spider.WebApi.Interfaces;
using NetCore.Spider.WebApi.Shared;
using NetCore.Spider.WebApi.ViewModel;
using NetCore.WebApi;

namespace NetCore.Spider.WebApi.Controllers
{
   
    public class SecurityController : ApiController
    {
        private IJwtTokenGenerator tokenGenerator;
        private ISecurity security;
        private IMapper mapper;

        public SecurityController(IJwtTokenGenerator tokenGenerator, ISecurity security, IMapper mapper)
        {
            this.tokenGenerator = tokenGenerator;
            this.security = security;
            this.mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<LoginReply> Login([FromBody]LoginInfo info, [FromServices]ISessionCreator sessionCreator)
        {
            LoginModel model = mapper.Map<LoginModel>(info);
            LoginResult result = await security.Login(model);
            LoginReply reply = mapper.Map<LoginReply>(result);

            if (result != null && (result.CurrentLoginStatus == LoginStatus.Success ||
                result.CurrentLoginStatus == LoginStatus.PasswordExpiring))
            {
                UserIdentity identity = new UserIdentity();

                var principal = identity.Build();
                var jwt = tokenGenerator.Generate(principal);
                var session = sessionCreator.Create();

                reply.Jwt = jwt;
                reply.Session = session;
            }
            reply.CurrentLoginStatus = result.CurrentLoginStatus;
            return reply;
        }
    }
}