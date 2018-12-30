using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using NetCore.Spider.Common;
using NetCore.Spider.Hubs;
using NetCore.Spider.Models;
using NetCore.Spider.Repository;
using Microsoft.Extensions.DependencyInjection;
namespace NetCore.Spider.Controllers
{
    public class HomeController : Controller
    {
        private _1024xpRepository _xpRepository = null;
        private IHubContext<_1024xpHub> hubContext;
        public HomeController(_1024xpRepository _1024XpRepository, IServiceProvider service)
        {
            _xpRepository = _1024XpRepository;
            hubContext = service.GetService<IHubContext<_1024xpHub>>();
        }

        public async Task<IActionResult> Index()
        {
            //Crawler crawler = new Crawler(_xpRepository, hubContext);
            //string indexURL = "http://1024.chxdoa.pw/pw/";
            //await crawler.CrawlerCoreAsync(indexURL);
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";
            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
