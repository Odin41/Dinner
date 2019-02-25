using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using Dinner.Models;

namespace Dinner
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }


        //protected void Application_Error(object sender, EventArgs e)
        //{

        //    Exception exception = Server.GetLastError();
        //    //Log your exception here
        //    Response.Clear();
        //    string action = "Error";
        //    // clear error on server
        //    Server.ClearError();

        //    Response.Redirect(String.Format("~/Error/{0}?message={1}", action, exception.Message));

        //}
    }
}
