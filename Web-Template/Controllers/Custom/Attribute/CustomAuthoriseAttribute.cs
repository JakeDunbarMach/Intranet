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
            bool allowed = false;

            AppModel.EmployeeUser userModel = new AppModel.EmployeeUser();
            EmployeeUser userData = new EmployeeUser();
            string fullUserName = httpContext.User.Identity.Name;

            //Strip off domain from fullUserName - find position of \ and check if not null
            int stop = fullUserName.IndexOf("\\");
            UserName = (stop > -1) ? fullUserName.Substring(stop + 1, fullUserName.Length - stop - 1) : fullUserName;

            if (HttpContext.Current.Request.IsLocal)
            {
                UserName = "Jake.Dunbar@Mach.co.uk";
            }

            userModel = userData.GetUserData(UserName,4);

            if (userModel != null && httpContext.Session != null)
            {
                httpContext.Session.Add("UserID", userModel.EmployeeID);
                httpContext.Session.Add("UserName", userModel.EmployeeFullName);
                httpContext.Session.Add("UserEmployeeTypeID", userModel.UserEmployeeTypeID);
                httpContext.Session.Add("ManagedByID", userModel.ManagedByID);

                /*
                * Get the application assembly version and set it to the in-line javascript in views. 
                * This means if we update some code it will always get the latest version and not cache
                */
                string version = "";

                /*If running locally*/
                if (HttpContext.Current.Request.IsLocal)
                {
                    version = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture);
                    version = version.Replace(" ", "");
                }
                else
                {
                    System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                    FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
                    version = fvi.FileVersion;
                }

                httpContext.Session.Add("JavaScriptVersion", version);

                allowed = true;

                //Check if passed in a section and return permission
                if (SectionID > 0)
                {
                    AppModel.EmployeePermission employeePermissionModel = new AppModel.EmployeePermission();
                    SystemEmployeePermission employeePermissionData = new SystemEmployeePermission();
                    employeePermissionModel = employeePermissionData.GetEmployeeSectionPermission(userModel.EmployeeID, SectionID, SystemID);

                    if (employeePermissionModel != null && employeePermissionModel.PermissionID > 0)
                    {
                        httpContext.Session.Add("PermissionID", employeePermissionModel.PermissionID);
                        allowed = true;
                    }
                    else
                    {
                        allowed = false;
                    }
                }
            }
            else
            {
                allowed = false;
            }
            return allowed;
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
