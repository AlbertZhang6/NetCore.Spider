using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.ControlCenter.Models;
using NetCore.Spider.Repository;

namespace NetCore.ControlCenter.Controllers
{
    public class HomeController : Controller
    {
        _1024xpRepository _1024XpRepository = null;

        public HomeController(_1024xpRepository  xpRepository)
        {
            this._1024XpRepository = xpRepository;
        }


        public IActionResult Index()
        {
            //_1024XpRepository.AddCrawHistory(null);
            var craw =  _1024XpRepository.GetCrawHistory();
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
