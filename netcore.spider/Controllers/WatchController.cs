﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace NetCore.Spider.Controllers
{
    public class WatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}