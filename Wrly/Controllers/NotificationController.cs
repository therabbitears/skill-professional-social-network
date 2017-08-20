using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using WrlyInfrastucture.Filters;

namespace Wrly.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        private IPushNotificationProcessor _processor;

        public IPushNotificationProcessor Processor
        {
            get
            {
                if (_processor==null)
                {
                    _processor = new PushNotificationProcessor();
                }
                return _processor;
            }
        }
        

        //
        // GET: /Notification/
        public async Task<ActionResult> Index()
        {
            var notifications = await Processor.Get(0, 10);
            return new JsonResult() { ContentType = "application/json", Data = notifications, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [CompressFilter]
        public async Task<ActionResult> All()
        {
            var notifications = await Processor.Get(0, 100);
            return View(notifications);
        }
    }
}