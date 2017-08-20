using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Knowledge;

namespace Wrly.Controllers
{
    public class KnowledgeController : Controller
    {
        IKnowledgeProcessor _press;
        IKnowledgeProcessor Processor
        {
            get
            {
                if (_press == null)
                {
                    _press = new KnowledgeProcessor();
                }
                return _press;
            }
        }


        public async Task<ActionResult> Index(string category, long? topicId = null)
        {
            var model = await Processor.Categories(category, topicId);
            return View(model);
        }


        [Authorize(Users = "banshi003@gmail.com")]
        public async Task<ActionResult> New(string category, long? topicId = null)
        {
            var model = new HelpViewModel()
            {
                Categories = await Processor.GetCategories()
            };
            return View(model);
        }

        [ValidateInput(false)]
        [HttpPost]
        [Authorize(Users = "banshi003@gmail.com")]
        public async Task<ActionResult> New(HelpViewModel model)
        {
            await Processor.Save(model);
            return View(model);
        }

        [Authorize(Users = "banshi003@gmail.com")]
        public async Task<JsonResult> Topics(long categoryID)
        {
            var topics = await Processor.GetTopics(categoryID);
            return new JsonResult() { Data = topics, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}