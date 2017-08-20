using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;

namespace Wrly.Controllers
{
    //[Authorize]
    public class MessagesController : Controller
    {
        private IMessageProcessor _processor;

        public IMessageProcessor Processor
        {
            get
            {
                if (_processor == null)
                {
                    _processor = new MessageProcessor();
                }
                return _processor;
            }
        }

        public async Task<ActionResult> Index()
        {
            var messages = await Processor.GetAll();
            return new JsonResult() { ContentType = "application/json", Data = messages, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}