using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace CloudSalon.API
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(HttpFilterCollection filters)
        {
            //filters.Add(new HandleErrorAttribute());
            filters.Add(new AuthenticationFilter());
            filters.Add(new ApiExceptionFilter());
            filters.Add(new ApiActionFilter());
        }
    }
}