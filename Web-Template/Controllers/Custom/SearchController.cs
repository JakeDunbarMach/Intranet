using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebUI.Controllers.Custom
{
    public class SearchController : SystemControllerBase
    {
        /// <summary>
        /// Return the permissions for a section
        /// </summary>
        /// <param name="sectionID">ID of the section selected</param>
        /// <returns>Json</returns>
        public JsonResult FillPermissions(int? sectionID)
        {
            AppData.SystemPermission permissionData = new AppData.SystemPermission();
            return Json(CreateSelectList(permissionData.GetPermissionList(sectionID), x => x.PermissionID, x => x.PermissionName), JsonRequestBehavior.AllowGet);
        }

    }
}