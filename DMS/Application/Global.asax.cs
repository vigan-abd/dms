using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Application
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
#if DEBUG
            string dataDir =  AppDomain.CurrentDomain.BaseDirectory;
            dataDir = dataDir.Substring(0, dataDir.LastIndexOf("\\"));
            dataDir = dataDir.Substring(0, dataDir.LastIndexOf("\\"))+"\\Model";
            AppDomain.CurrentDomain.SetData("DataDirectory", dataDir);
#endif
        }
    }
}
