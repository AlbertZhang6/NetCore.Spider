using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NetCore.Model.Entity;
using NetCore.Spider.Repository;

namespace NetCore.ControlCenter.Controllers
{
    public class SourceClassifyController : Controller
    {

        SourceClassifyRepository _sourceClassifyRepository = null;

        public SourceClassifyController(SourceClassifyRepository sourceClassifyRepository)
        {
            this._sourceClassifyRepository = sourceClassifyRepository;
        }

        public IActionResult Index(int? id)
        {
            IList<SourceClassify> model = _sourceClassifyRepository.GetSourceClassifyListByParentId(id);
            return View(model);
        }

        public IActionResult SourceClassifyAdd(int? id)
        {
            SourceClassify sourceClassify = new SourceClassify();
            return View(sourceClassify);
        }

        public IActionResult AddSourceClassify(SourceClassify sourceClassify)
        {
            //SourceClassify sourceClassify = new SourceClassify();
            return View(sourceClassify);
        }
    }
}