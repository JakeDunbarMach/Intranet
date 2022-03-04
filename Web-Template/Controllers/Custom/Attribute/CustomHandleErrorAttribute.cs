using System;
using System.Configuration;
using System.Diagnostics;
using System.Web.Mvc;
using System.Web.Routing;
using AppData.Constants;
using Microsoft.ApplicationInsights;

namespace WebUI.Controllers.Custom
{

    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {

        public const string JSON_ERROR = "Sorry an error occurred";

        /// <summary>
        /// Override the method OnException to return different views and log error
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            Exception exception = filterContext.Exception;

            /*
             *  We need to save the following exceptions to the ErrorLog database table:  
             *  ArgumentException, ArgumentNullException, , NullReferenceException, ArgumentOutOfRangeException and InvalidOperationException 
            */

            if (
                exception.GetType() == typeof(ArgumentException) ||
                exception.GetType() == typeof(ArgumentNullException) ||
                exception.GetType() == typeof(NullReferenceException) ||
                exception.GetType() == typeof(ArgumentOutOfRangeException) ||
                exception.GetType() == typeof(InvalidOperationException))
            {
                WriteToDatabaseLog(filterContext);
            }
            else
            {
                //Write exception information to the event log on the webserver for the source ComplaintsWebsite
                WriteToEventLog(filterContext);
            }

            ActionResult redirectActionResult = null;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                redirectActionResult = new JsonResult();
                ((JsonResult)redirectActionResult).Data = JSON_ERROR;
                ((JsonResult)redirectActionResult).JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            }
            //Check if came from a child i.e. partial view
            else if (filterContext.IsChildAction)
            {
                redirectActionResult = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "System" }, { "action", "SystemIssuePartial" } });
            }
            else
            {
                //Using RedirectToRouteResult as unsuccessful with RedirectToAction and ViewResult
                redirectActionResult = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "System" }, { "action", "SystemIssue" } });
            }

            filterContext.Result = redirectActionResult;

            // Set ExceptionHandled to true so Exception marked as handled
            filterContext.ExceptionHandled = true;

        }


        /// <summary>
        /// Write entry to table in database
        /// </summary>
        /// <param name="err"></param>
        private void WriteToDatabaseLog(ExceptionContext err)
        {
            string guid = Guid.NewGuid().ToString();

            AppData.ErrorLog errorLog = new AppData.ErrorLog();

            int id = errorLog.InsertError(guid, err.Exception.ToString());

            if (errorLog.InsertError(guid, err.Exception.ToString()) == 0)
            {
                //failed to insert so log to events
                WriteToEventLog(err);
            }
        }

        /// <summary>
        /// Write entry to the event view application event log
        /// </summary>
        /// <param name="err"></param>
        private void WriteToEventLog(ExceptionContext err)
        {
            //For Azure write to ApplicationInsights
            var ai = new TelemetryClient();
            ai.TrackException(err.Exception);
        }

    }


}