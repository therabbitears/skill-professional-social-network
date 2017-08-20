using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastuctures.Utils;

namespace Wrly.Infrastructure.Filters
{
    public class RequireSecureConnection: FilterAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsSecureConnection && !filterContext.HttpContext.Request.IsLocal)
            {
                if (AppConfig.ForceSecure)
                {
                    var url = filterContext.HttpContext.Request.Url.ToString().Replace("http:", "https:");
                    filterContext.Result = new RedirectResult(url, true);
                    //Calling base action handler.
                    return;
                }
            }
        }
    }
}