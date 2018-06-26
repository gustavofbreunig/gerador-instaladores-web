using log4net;
using System;
using System.Web;
using System.Web.Mvc;

namespace GeradorInstaladores
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Log4NetExceptionFilter());
        }
    }

    public class Log4NetExceptionFilter : IExceptionFilter
    {
        private static readonly ILog Logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public void OnException(ExceptionContext context)
        {
            Exception ex = context.Exception;

            Logger.Error(ex.Message);
        }
    }
}
