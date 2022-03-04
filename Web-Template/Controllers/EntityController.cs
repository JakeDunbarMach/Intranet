using AppModel;
using AppModel.Constants;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebUI.Controllers.Custom;

namespace WebUI.Controllers
{
    [CustomAuthorize(SectionID = (int)SectionEnum.Sections.ENTITY, SystemID = 9999)]
    public class EntityController : SystemControllerBase
    {
        #region Loads

        /// <summary>
        /// Return Edit view (if ID supplied then load record otherwise new record)
        /// </summary>
        /// <param name="ID">ID to view</param>
        /// <returns>Edit view</returns>
        public ActionResult Load(int? ID)
        {
            int userID = Convert.ToInt32(HttpContext.Session["UserID"]);
            AppModel.Entity entityModel = new Entity();
            AppData.Entity entityData = new AppData.Entity();

            int entityID = ID ?? 0;

            if (entityID > 0)
            {
                entityModel = entityData.GetEntity(entityID, userID);
            }

            if (entityModel == null)
            {
                return RedirectToAction("SystemIssue", "System");
            }

            FillLookupLists();

            return View("Edit", entityModel);
        }

        #endregion

        #region Saves

        /// <summary>
        /// Return Json for save result with ID and error messages
        /// </summary>
        /// <param name="model">model to save</param>
        /// <returns>Json</returns>
        [HttpPost]
        [CustomValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Save(Entity entity)
        {
            Boolean saveState = false;
            string message = "";
            int resultStatus = 0;

            //Check enough permission to save
            if (@ViewBag.PermissionID == (int)PermissionEnum.Permissions.Update || @ViewBag.PermissionID == (int)PermissionEnum.Permissions.Admin)
            {
                //Check the model is valid
                if (ModelState.IsValid == true)
                {
                    AppData.Entity entityData = new AppData.Entity();

                    if (entity.EntityID > 0)
                    {
                        resultStatus = entityData.UpdateEntity(entity, Convert.ToInt32(HttpContext.Session["UserID"]));
                        if (resultStatus > 0)
                        {
                            saveState = true;
                            message = "Record saved successfully";
                        }
                        else
                        {
                            saveState = false;
                            message = "The record cannot be saved. Please try again.";
                        }
                    }
                    else
                    {
                        resultStatus = entityData.InsertEntity(entity, Convert.ToInt32(HttpContext.Session["UserID"]));
                        //if successful ID will be > 0
                        if (resultStatus > 0)
                        {
                            saveState = true;
                            message = "Record saved successfully";
                        }
                        else
                        {
                            saveState = false;
                            message = "The record cannot be saved. Please try again.";
                        }
                    }
                }
                else
                {
                    saveState = false;
                    message = "The record cannot be saved due to errors. Please correct the errors and try again.";
                }
            }
            else
            {
                saveState = false;
                message = "The record cannot be saved due to permission issues. Please try again.";
            }

            return Json(new { Saved = saveState, Title = "Save Record", Message = message, ID = resultStatus });
        }

        #endregion

        #region ActionLists

        /// <summary>
        /// Returns list of documents by id
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public ActionResult ListDocuments(int ID)
        {
            AppData.EntityDocument documentData = new AppData.EntityDocument();
            List<AppModel.EntityDocument> entityDocumentList = documentData.GetDocumentList(ID);

            //Create shared access policy for container to allow access. Needed to append onto querystring of document path
            string containerName = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_FOLDER].ToLower();
            ViewBag.ED_SAS = CreateSharedAccessPolicy(containerName);

            return PartialView("_DocumentList", entityDocumentList);
        }

        #endregion

        #region Deletes

        /// <summary>
        /// Return Json for delete outcome
        /// </summary>
        /// <param name="ID">ID of record to delete</param>
        /// <returns>Json</returns>
        [HttpPost]
        [CustomValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult Delete(int ID)
        {
            Boolean saveState = false;
            string message = "";
            int resultStatus = 0;
            int userID = Convert.ToInt32(HttpContext.Session["UserID"]);

            //Check enough permission to delete
            if (@ViewBag.PermissionID == (int)PermissionEnum.Permissions.Admin)
            {
                AppData.Entity entityData = new AppData.Entity();
                resultStatus = entityData.DeleteEntity(ID, userID);

                if (resultStatus > 0)
                {
                    saveState = true;
                    message = "Record deleted";
                }
                else
                {
                    saveState = false;
                    message = "Error deleting record. Please try again.";
                }

            }
            else
            {
                saveState = false;
                message = "Error deleting record. Please try again.";
            }

            return Json(new { Saved = saveState, Title = "Delete Record", Message = message });
        }

        #endregion

        #region LookupLists

        /// <summary>
        /// Populate the lookup lists in the ViewBag using values from the model provided
        /// </summary>
        private void FillLookupLists()
        {
            AppData.GenericList genericListData = new AppData.GenericList();
            ViewBag.EntityTypeList = CreateSelectList(genericListData.GetEntityTypeList(), x => x.ID, x => x.Name);
        }

        #endregion

    }
}
