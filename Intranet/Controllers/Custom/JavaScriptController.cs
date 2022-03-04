using System;
using System.Web;
using System.Web.Mvc;
using System.Text;
using System.Configuration;

namespace WebUI.Controllers.Custom
{
    /// <summary>
    /// Controller to return JavaScript variables.
    /// </summary>
    public class JavaScriptController : Controller
    {
        [HttpGet]
        public JavaScriptResult JavaScriptConfiguration()
        {
            //Set the JsonErrorMsg to the JSON_ERROR returned from CustomHandleExceptionAttribute
            string JsonErrorMsg = CustomHandleErrorAttribute.JSON_ERROR;
            string JsonErrorMsgVar = string.Format("var JSON_ERROR = '{0}';", JsonErrorMsg);

            // Set a string to AppDomainAppVirtualPath so can get the beginning part of the URL
            string virtualPath = HttpRuntime.AppDomainAppVirtualPath;
            string rootPath = ConfigurationManager.AppSettings["RootPath"];

            // Strip out erroneous / if at the root
            if (virtualPath == "/")
            {
                virtualPath = String.Empty;
            }
            //Setup variable to access in jQuery i.e. Complaint.js $('#btnComplaintSave').click
            string virtualPathVar = string.Format("var VIRTUAL_PATH = '{0}';", virtualPath);
            string rootPathVar = string.Format("var ROOT_PATH = '{0}';", rootPath);

            //Build up string of variables for virtual path and error message
            StringBuilder configScript = new StringBuilder();
            configScript.AppendLine(virtualPathVar);
            configScript.AppendLine(rootPathVar);
            configScript.AppendLine(JsonErrorMsgVar);
            return JavaScript(configScript.ToString());
        }
    }
}
