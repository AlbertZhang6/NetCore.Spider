using AutoMapper;
using NetCore.Model.Model;
using NetCore.Spider.WebApi.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.WebApi
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<LoginResult, LoginReply>();
        }
    }
}
