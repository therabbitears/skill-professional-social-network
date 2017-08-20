using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;

namespace Wrly.Infrastructure.Extended
{
    public class AuthorizeUser : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            //if (filterContext.RequestContext.HttpContext.Request.IsAuthenticated)
            //{
            //    using (var acountRepository = new AccountRepository())
            //    {
            //        var entityID = SessionInfo.UserHash.EntityID;
            //        var entityType = SessionInfo.UserHash.EntityType;
            //        using (var validationData = acountRepository.GetValidationContext(entityID))
            //        {
            //            if (entityType == ((int)Enums.EntityTypes.Person))
            //            {
            //                var validatableObject = validationData.Tables[0].FromDataTable<PersonEntityScopData>(null).FirstOrDefault();
            //                if (validatableObject.WizardStep == null && filterContext.RouteData.Values["action"].ToString() != "SetCareerOption")
            //                {
            //                    filterContext.Result = new RedirectResult("/wizard/setcareeroption?setup=cs&mode=email&stamp=" + DateTime.UtcNow.Ticks.ToString());
            //                }
            //            }
            //        }
            //    }
            //}
            base.OnActionExecuting(filterContext);
        }
    }
}