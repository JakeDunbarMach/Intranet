using WebUI.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Data.Entity;
using System.Web.Helpers;
using System.Security.Claims;
using System.Configuration;

namespace WebUI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            //Added after move to Azure otherwise get ClaimsIdentity error
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimTypes.NameIdentifier;

            /* Code below is used to display which system the user is accessing */
            string connStr = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            string currentSystem = "";

            if (connStr.IndexOf("-dev;") > 0)
            {
                currentSystem = " (DEV)";
            }
            else if (connStr.IndexOf("-uat;") > 0)
            {
                currentSystem = " (UAT)";
            }

            Application.Add("SystemEnvironment", currentSystem);

            //need this for the PDF - OpenSSL libaries
            string SSLPath = Server.MapPath("~/bin/QtBinariesWindows");
            Environment.SetEnvironmentVariable("Path", SSLPath);

        }

        void Application_Error(Object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();

            // Clear the error
            Server.ClearError();
            Response.Clear();

            RouteData routeData = new RouteData();
            routeData.Values.Add("controller", "System");
            routeData.Values.Add("action", "SystemIssue");

            //// Call target Controller and pass the routeData.
            //IController errorController = new SystemController();
            //errorController.Execute(new RequestContext(
            //        new HttpContextWrapper(Context), routeData));

        }

    }
}
