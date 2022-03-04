using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace WebUI.Controllers.Custom
{
    /// <summary>
    /// Based on this example http://stackoverflow.com/questions/14005773/use-asp-net-mvc-validation-with-jquery-ajax
    /// </summary>
    public class CustomValidateAjaxAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// Override the method OnActionExecuting to validate the model and return errors in an array
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.HttpContext.Request.IsAjaxRequest())
                return;

            var modelState = filterContext.Controller.ViewData.ModelState;
            if (!modelState.IsValid)
            {   //If model is not valid then build up array of errors and return in Json
                var errorModel =
                        from x in modelState.Keys
                        where modelState[x].Errors.Count > 0
                        select new
                        {
                            key = x,
                            errors = modelState[x].Errors.
                                                   Select(y => y.ErrorMessage).
                                                   ToArray()
                        };
                filterContext.Result = new JsonResult()
                {
                    Data = errorModel
                };
                //Return back request which triggers error function on ajax calls
                filterContext.HttpContext.Response.StatusCode =
                                                      (int)HttpStatusCode.BadRequest;
            }
        }
    }
}