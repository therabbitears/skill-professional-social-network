#region ' ---- Includes ---- '

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Processors.Implementations;
using Wrly.Infrastructure.Processors.Structures;
using Wrly.Infrastructure.Utils;
using Wrly.infrastuctures.Utils;
using Wrly.Infrastuctures.Filters;
using Wrly.Infrastuctures.Utils;
using Wrly.Models;
using Wrly.Models.Listing;

#endregion
namespace Wrly.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    [ManipulateWWW()]
    public class BaseController : Controller
    {

        private ICommonDataProcessor _commonProcessor;

        public ICommonDataProcessor CommonProcessor
        {
            get
            {
                if (_commonProcessor == null)
                {
                    _commonProcessor = new CommonProcessor();
                }
                return _commonProcessor;
            }
        }

        private ILookupProcessor _lookupProcessor;

        public ILookupProcessor LookupProcessor
        {
            get
            {
                if (_lookupProcessor == null)
                {
                    _lookupProcessor = new LookupProcessor();
                }
                return _lookupProcessor;
            }
        }


        #region ' ---- Methods ---- '
        protected override void OnException(ExceptionContext filterContext)
        {
            if (AppConfig.OverrideExceptionScreen)
            {
                var exception = filterContext.Exception;
                if (exception.GetType() == typeof(HttpAntiForgeryException))
                {
                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var result = new PartialViewResult() { ViewName = "_PageExpired" };
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                        base.OnException(filterContext);
                        return;
                    }
                    else
                    {
                        var result = new ViewResult() { ViewName = "PageExpired" };
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                        base.OnException(filterContext);
                        return;
                    }
                }

                if (exception.GetType() == typeof(HttpRequestValidationException))
                {
                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var result = new PartialViewResult() { ViewName = "_InvalidInput" };
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                        base.OnException(filterContext);
                        return;
                    }
                    else
                    {
                        var result = new ViewResult() { ViewName = "InvalidInput" };
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                        base.OnException(filterContext);
                        return;
                    }
                }
                else
                {
                    if (filterContext.RequestContext.HttpContext.Request.IsAjaxRequest())
                    {
                        var result = new PartialViewResult() { ViewName = "_Error" };
                        filterContext.Result = result;
                        filterContext.ExceptionHandled = true;
                        base.OnException(filterContext);
                        return;
                    }
                    else
                    {
                        ViewResult objViewResult = new ViewResult();
                        objViewResult.ViewName = "Error";
                        filterContext.Result = objViewResult;
                        filterContext.ExceptionHandled = true;
                        base.OnException(filterContext);
                    }
                }
            }
            filterContext.Exception.HandleUILayerException("Wrly.Controllers", this.GetType().FullName, string.Concat("Controller:", filterContext.RouteData.Values["controller"].ToString(), "| Action:", filterContext.RouteData.Values["action"].ToString()));
        }

        public Dictionary<string, string> Contries
        {
            get
            {
                return CommonProcessor.Countries();
            }
        }

        public Dictionary<int, string> Industries
        {
            get
            {
                return CommonProcessor.Industries().Result;
            }
        }


        public async Task<List<KeyValue>> IndustryLookupList(string keyWord)
        {
            return await LookupProcessor.Industries(keyWord);
        }

        public Dictionary<string, string> EmployeeStrengths
        {
            get
            {
                return CommonData.EmployeeStrengths();
            }
        }

        public Dictionary<int?, string> Month
        {
            get
            {
                return CommonData.Months();
            }
        }

        public Dictionary<int?, string> Year
        {
            get
            {
                return CommonData.Years();
            }
        }


        public Dictionary<int?, string> Coming30Years
        {
            get
            {
                return CommonData.Coming30Years();
            }
        }

        public Dictionary<string, string> Projects()
        {
            return LookupProcessor.Projects(User.Identity.Name, true);
        }

        public async Task<List<KeyValue>> MySkills(long? entityID = null)
        {
            entityID = entityID ?? UserHashObject.EntityID;
            return await LookupProcessor.Skills(Convert.ToInt64(entityID));
        }

        public async Task<Dictionary<string, string>> CareerHistoryList(long? entityID = null)
        {
            entityID = entityID ?? UserHashObject.EntityID;
            return await LookupProcessor.CareerHistoryList(Convert.ToInt64(entityID));
        }


        public async Task<List<KeyValue>> MySkills(string keyword)
        {
            return await LookupProcessor.MySkills(keyword);
        }

        public async Task<List<KeyValue>> AllSkills(string keyword)
        {
            return await LookupProcessor.AllSkills(keyword);
        }

        public async Task<List<KeyValue>> JobTitles(string keyword)
        {
            return await LookupProcessor.JobTitles(keyword);
        }

        public async Task<List<KeyValue>> SearchCourses(string keyword)
        {
            return await LookupProcessor.Courses(keyword);
        }

        public async Task<List<KeyValue>> SearchOrganizations(string key)
        {
            return await LookupProcessor.Organizations(key);
        }

        public async Task<List<KeyValue>> SearchUniversities(string key)
        {
            return await LookupProcessor.Universities(key);
        }

        public async Task<List<KeyValue>> MYConnections()
        {
            return await LookupProcessor.Connections(UserHashObject.EntityID);
        }

        public async Task<List<PersonFacehead>> MyConnections(string keyWord)
        {
            return await LookupProcessor.Connections(UserHashObject.EntityID, keyWord);
        }


        public JsonResult WJson(dynamic data)
        {
            return new JsonResult() { ContentType = "application/json", Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public JsonResult WJsonError(string data)
        {
            return new JsonResult() { ContentType = "application/json", Data = new Result() { Type = Types.Enums.ResultType.Error, Description = data }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }


        public UserHash UserHashObject { get { return SessionInfo.UserHash; } }

        public int EntityType { get { return SessionInfo.UserHash.EntityType; } }


        #endregion
    }

}

