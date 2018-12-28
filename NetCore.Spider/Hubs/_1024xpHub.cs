using Microsoft.AspNetCore.SignalR;
using NetCore.Model.Model;
using NetCore.Spider.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCore.Spider.Hubs
{
    public class _1024xpHub :  Hub, IDisposable
    {
        public static async Task SendContentToClient(IHubContext<_1024xpHub> hubContext, _1024xCrawModel crawModel)
        {
            await hubContext.Clients.All.SendAsync("Show", crawModel);
        }
    }
}
