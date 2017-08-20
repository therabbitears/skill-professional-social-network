using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wrly.App_Start
{
    public class MyCustomViewEngine : RazorViewEngine
    {
        /// <summary>
        /// Partial view locations.
        /// </summary>
        private static string[] PartialViewEngineFormats = new[] 
            {
                "~/Views/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/Account/{0}.cshtml",
                "~/Views/Press/Common/{0}.cshtml",
                "~/Views/Business/{0}.cshtml",
                "~/Views/CareerHistory/Profile/{0}.cshtml",
                "~/Views/Home/{0}.cshtml",
                "~/Views/ChatGroup/{0}.cshtml"
            };


        /// <summary>
        /// View Locations 
        /// </summary>
        private static string[] ViewEngineFormats = new[] 
            {
                "~/Views/Account/{0}.cshtml",
                "~/Views/Business/{0}.cshtml",
                "~/Views/Association/{0}.cshtml",
                "~/Views/CareerHistory/{0}.cshtml"
            };


        ///// <summary>
        ///// File Extensions 
        ///// </summary>
        public MyCustomViewEngine()
        {
            base.PartialViewLocationFormats = base.PartialViewLocationFormats.Union(PartialViewEngineFormats).ToArray();
            base.ViewLocationFormats = base.ViewLocationFormats.Union(ViewEngineFormats).ToArray();
        }

    }

    public class ViewConfigurator
    {
        public static void RegisterLocations()
        {
            ViewEngines.Engines.Add(new MyCustomViewEngine());
        }
    }
}