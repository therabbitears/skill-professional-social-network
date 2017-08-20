using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Wrly.Infrastructure.Filters
{
    public class AntiForgeryValidate : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string cookieToken = "";
            string formToken = "";

            string tokenHeaders = filterContext.RequestContext.HttpContext.Request.Params["__RequestVerificationToken"];
            string[] tokens = tokenHeaders.Split(':');
            if (tokens.Length == 2)
            {
                cookieToken = tokens[0].Trim();
                formToken = tokens[1].Trim();
            }
            System.Web.Helpers.AntiForgery.Validate(cookieToken, formToken);

            base.OnActionExecuting(filterContext);
        }
    }
}