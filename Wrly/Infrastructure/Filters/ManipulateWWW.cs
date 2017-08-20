using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wrly.Infrastuctures.Filters
{
    public class ManipulateWWW : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            if (!actionContext.HttpContext.Request.IsLocal)
            {
                //Request URL with of current executing context.
                string strUrl = actionContext.RequestContext.HttpContext.Request.Url.ToString();
                string strActionName = actionContext.ActionDescriptor.ActionName;
                // Checks URL does not contain www prefix and action is not profile.
                if (!actionContext.RequestContext.HttpContext.Request.Url.IsWWWRequest())
                {
                    //Assigning result by adding www into current execution context so that result can be executed by base action handler.
                    actionContext.Result = new RedirectResult(HttpContext.Current.Request.Url.AddWWW(), true);
                    //Calling base action handler.
                    base.OnActionExecuting(actionContext);
                    return;
                }
            }
        }
    }
}