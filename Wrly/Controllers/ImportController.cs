using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Models.Import;

namespace Wrly.Controllers
{
    //[Authorize]
    public class ImportController : BaseController
    {
        private IContactProcessor _ContactProcessor;

        public IContactProcessor ContactProcessor
        {
            get
            {
                if (_ContactProcessor == null)
                {
                    _ContactProcessor = new ContactProcessor();
                }
                return _ContactProcessor;

            }
        }

        public async Task<ActionResult> Import(string id)
        {
            var result = await ContactProcessor.GetByImportID(id);
            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Invite(List<Wrly.Models.Import.ContactViewModel> data, long inviteID)
        {
            var result = await ContactProcessor.Invite(data.Where(c => c.Send), inviteID);
            return PartialView("_ActionResultMessage", result);
        }

        [HttpPost]
        public async Task<ActionResult> Update(long? id, string value)
        {
            var result = await ContactProcessor.Update(id, value);
            return WJson(result);
        }


        public async Task<ActionResult> InviteManual(string src)
        {
            var model = new ContactImportViewModel() { Message = string.Format("Hey ***Name**, I am using sklative a social media allows you to connect and find people having similar skills and allow you to put your skill in 360 Degree showcase, Join me here.") };
            return PartialView("_Invite");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> InviteManual(ContactImportViewModel model)
        {
            var result = await ContactProcessor.Invite(model);
            return WJson(result);
        }
    }
}