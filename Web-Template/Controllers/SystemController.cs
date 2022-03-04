using System;
using System.Web.Mvc;

namespace WebUI.Controllers
{
    public class SystemController : Controller
    {
        /// <summary>
        /// Called by HandleUnauthorizedRequest in AuthorizeActionAttribute to show permission error view
        /// </summary>
        /// <returns>View PermissionError</returns>
        public ActionResult PermissionError(string userName)
        {
            ViewBag.UserName = userName;
            return View("PermissionError");
        }

        /// <summary>
        /// Return the view SystemIssue
        /// </summary>
        /// <returns>View SystemIssues</returns>
        public ActionResult SystemIssue()
        {
            return View("SystemIssue");
        }


        /// <summary>
        /// See Ping.PingSetup() function in Application.js
        /// </summary>
        /// <returns>Json Result</returns>
        public JsonResult Ping()
        {
            DateTime pingTime = DateTime.Now;
            //Only returning pingTime so not returning null
            return Json(pingTime, JsonRequestBehavior.AllowGet);
        }

    }
}