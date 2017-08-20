using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Wrly.Controllers
{
    //[Authorize]
    public class LookupController : BaseController
    {
        //
        // GET: /Lookup/
        public async Task<ActionResult> Skills()
        {
            return View();
        }

        public async Task<ActionResult> IndustryList(string keyWord)
        {
            return WJson(await IndustryLookupList(keyWord));
        }
    }
}