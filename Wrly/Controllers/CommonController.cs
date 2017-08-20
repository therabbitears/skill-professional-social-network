using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    public class CommonController : Controller
    {
        IWizardProcessor _Processor;
        public IWizardProcessor Processor
        {
            get
            {
                if (_Processor == null)
                {
                    _Processor = new WizardProcessor();
                }
                return _Processor;
            }
        }

        public ActionResult Privacy()
        {
            return View();
        }

        public ActionResult Cookie()
        {
            return View();
        }

        public ActionResult Terms()
        {
            return View();
        }

        public ActionResult Abuse()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Abuse(AbuseViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.SendAbuse(model);
                ViewBag.ShowMessage = true;
                ViewBag.ModelResult = result;
            }
            return View(model);
        }

        //[Authorize]
        public ActionResult Feedback()
        {
            return View();
        }

        //[Authorize]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Feedback(FeedbackViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Processor.Feeback(model);
                ViewBag.ShowMessage = true;
                ViewBag.ModelResult = result;
            }
            return View(model);
        }

        [CompressFilter]
        [Authorize]
        public async Task<ActionResult> Cropped()
        {
            return PartialView("_CropAndUpload");
        }
    }
}