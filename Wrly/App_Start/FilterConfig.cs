using System.Web;
using System.Web.Mvc;
using Wrly.Infrastructure.Filters;

namespace Wrly
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new RequireSecureConnection());
            
        }
    }
}
