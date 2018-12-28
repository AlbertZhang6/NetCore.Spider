using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetCore.Spider.Hubs;
using NetCore.Spider.Repository;
using Microsoft.Extensions.DependencyInjection;
using NetCore.Spider.Common;
using Microsoft.AspNetCore.Cors;

namespace NetCore.Spider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("SignalRCors")]
    public class CrawllerController : Controller
    {

        private _1024xpRepository _xpRepository = null;
        private IHubContext<_1024xpHub> hubContext;
        public CrawllerController(_1024xpRepository _1024XpRepository, IServiceProvider service)
        {
            _xpRepository = _1024XpRepository;
            hubContext = service.GetService<IHubContext<_1024xpHub>>();
        }

        [HttpGet]
        public async Task<IActionResult> RunCraw(string url)
        {
            Crawler crawler = new Crawler(_xpRepository, hubContext);
            await crawler.CrawlerCoreAsync(url);
            return Ok();
        }

        // GET: api/Crawller
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/Crawller/5
        [HttpGet("{link}", Name = "Get")]
        public async Task Get(string link)
        {
            var url = "http://1024.chxdoa.pw/pw/";
            await RunCraw(url);
        }

        // POST: api/Crawller
        [HttpPost]
        public void Post([FromBody] string link)
        {
            var url = "http://1024.chxdoa.pw/pw/";
            RunCraw(url);
        }

        // PUT: api/Crawller/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
