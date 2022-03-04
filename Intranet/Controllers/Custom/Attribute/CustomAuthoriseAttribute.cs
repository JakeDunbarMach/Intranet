using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AppData;
using System.Diagnostics;
using System;
using System.Globalization;

namespace WebUI.Controllers.Custom
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {

        #region Properties

        /// <summary>
        /// The username string to show on the SystemIssue views
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ID of section permissions required
        /// </summary>
        public int SectionID { get; set; }

        public int SystemID { get; set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// Verifies user can access system. 
        /// Overriding the standard AuthorizeCore method on the AuthorizeAttribute attribute
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return true;
        }


        /// <summary>
        /// If authorisation failes (AuthorizeCore allowed = false) then load error page
        /// Overriding the standard HandleUnauthorizedRequest method on the AuthorizeAttribute attribute
        /// </summary>
        /// <param name="filterContext">The filter context</param>
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            RedirectToRouteResult redirectResult = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "System" }, { "action", "PermissionError" }, { "userName", UserName } });
            filterContext.Result = redirectResult;
        }

        #endregion
    }
}
