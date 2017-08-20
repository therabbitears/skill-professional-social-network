using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Utils;
using Wrly.Models;

namespace Wrly.Controllers
{
    public class SearchController : BaseController
    {
        ISearchProcessor _Processor;
        public ISearchProcessor Processor
        {
            get
            {
                if (_Processor == null)
                {
                    _Processor = new SearchProcessor();
                }
                return _Processor;
            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(EntitySearchViewModel model)
        {
            var recordSearch = await Processor.RecordSearch(model);
            if (model.EntityID > 0)
            {
                if (model.EntityType == (int)Enums.EntityTypes.Person)
                {
                    return RedirectToRoute("PublicProfile", new { profilename = model.Url });
                }
                if (model.EntityType == (int)Enums.EntityTypes.Organization)
                {
                    return RedirectToRoute("OrganizationProfile", new { profilename = model.Url });
                }
                if (model.EntityType == (int)Enums.EntityTypes.Group)
                {
                    return RedirectToRoute("Groups", new { id = model.Url });
                }
            }
            return RedirectToRoute("Search", new { q = model.Keyword });
        }


        [HttpGet]
        public async Task<ActionResult> Results(string q, string type = "general")
        {
            var results = await Processor.GetResults(q, type);
            return View(results);
        }


        [HttpGet]
        public async Task<JsonResult> SearchList()
        {
            var data = UserCacheManager.Searches;
            return WJson(data);
        }

        [HttpPost]
        public async Task<JsonResult> Execute(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
            {
                var data = await Processor.Execute(keyword);
                return WJson(data);
            }

            var dataSearches = UserCacheManager.Searches;
            return WJson(dataSearches);
        }


        [HttpGet]
        public async Task<PartialViewResult> SearchView()
        {
            var data = UserCacheManager.Searches;
            return PartialView("_Searches", data);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<RedirectToRouteResult> HomeSearch(string searchTerm)
        {
            return RedirectToRoute("LuceneEngine", new { searchTerm = searchTerm });
        }
    }
}