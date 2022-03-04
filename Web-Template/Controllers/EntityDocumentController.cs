using AppModel;
using AppModel.Constants;
using WebUI.Controllers.Custom;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;

namespace WebUI.Controllers
{
    [CustomAuthorize(SectionID = (int)SectionEnum.Sections.ENTITY, SystemID = 9999)]
    public class EntityDocumentController : SystemControllerBase
    {
        readonly string containerName = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_FOLDER].ToLower();

        #region Add

        /// <summary>
        /// Load partial view with blank model for adding new record
        /// </summary>
        /// <param name="ID">ID to associate documents to</param>
        /// <returns>Partial View _Document</returns>
        public ActionResult AddDocument(int EntityID)
        {
            AppModel.EntityDocument documentModel = new EntityDocument
            {
                EntityID = EntityID
            };
            ViewBag.noOfDocuments = 0;
            return PartialView("_AddDocument", documentModel);
        }

        #endregion

        #region Saves

        /// <summary>
        /// Save file selected on view, check a file of the same name doesn't already exist,
        /// save to the network and insert record into SQL database
        /// </summary>
        /// <param name="file">File object from document view</param>
        /// <param name="ID">ID to associate document with</param>
        /// <returns>string outcome</returns>

        [HttpPost]
        [CustomValidateAjax]
        [ValidateAntiForgeryToken]
        public ContentResult UploadFile([Bind(Include = "EntityID")] int EntityID, List<HttpPostedFileBase> fileupload)
        {
            string outcome = "false";
            string fileName = "";
            string displayName = "";
            int contentLength = 0;
            string contentType = "";
            Boolean status = false;

            var returnFileResult = new List<UploadFilesResult>();

            //Check enough permission to save
            if (@ViewBag.PermissionID == (int)PermissionEnum.Permissions.Update || @ViewBag.PermissionID == (int)PermissionEnum.Permissions.Admin)
            {

                foreach (HttpPostedFileBase file in fileupload)
                {
                    displayName = Path.GetFileName(file.FileName);
                    if ((file != null) && (file.ContentLength > 0))
                    {
                        contentLength = Convert.ToInt32(file.ContentLength / 1024);
                        contentType = file.ContentType;
                        var fileExtension = "";
                        fileExtension = Path.GetExtension(file.FileName).ToLower();
                        string pathAndFileName = "";

                        //Check not uploading an exe as dangerous
                        if (fileExtension.Length > 0 && !fileExtension.Equals(".exe"))
                        {
                            //Save file to Azure
                            fileName = EntityID.ToString() + "_" + FormatFilename(displayName);
                            bool blobExists = CheckBlobExists(fileName, containerName);

                            if (blobExists)
                            {
                                outcome = "File already exists";
                            }
                            else
                            {
                                try
                                {
                                    //Save file to Azure
                                    pathAndFileName = SaveFileToBlob(fileName, file.InputStream, containerName);
                                    if (pathAndFileName != "")
                                    {
                                        AppData.EntityDocument documentData = new AppData.EntityDocument();
                                        EntityDocument documentModel = new EntityDocument
                                        {
                                            EntityID = EntityID,
                                            DocumentName = displayName,
                                            DocumentPath = pathAndFileName
                                        };

                                        if (documentData.InsertDocument(documentModel, Convert.ToInt32(HttpContext.Session["UserID"])) > 0)
                                        {   //SQL returned an ID for the record inserted so insert successful
                                            outcome = "Document uploaded.";
                                            status = true;
                                            ViewBag.noOfDocuments = ViewBag.noOfDocuments + 1;
                                        }
                                        else
                                        {
                                            outcome = "Unable to upload document.";
                                        }
                                    }
                                    else
                                    {
                                        outcome = "Unable to upload document.";
                                    }

                                }
                                catch
                                {
                                    outcome = "Unable to upload document.";
                                }
                            }
                        }
                    }
                    else
                    {
                        if ((file == null))
                        {
                            outcome = "Invalid file.";
                        }
                        else
                        {
                            outcome = "File has no content.";
                        }
                    }
                }
            }
            else
            {
                outcome = "false";
            }

            returnFileResult.Add(new UploadFilesResult()
            {
                Name = FormatFilename(displayName),
                Length = contentLength,
                Type = contentType,
                Status = status,
                StatusMessage = outcome
            });

            // Returns json
            return Content("{\"status\":\"" + returnFileResult[0].Status + "\",\"message\":\"" + returnFileResult[0].StatusMessage + "\",\"name\":\"" + returnFileResult[0].Name + "\",\"type\":\"" + returnFileResult[0].Type + "\",\"size\":\"" + string.Format("{0} KB", returnFileResult[0].Length.ToString("n2")) + "\"}", "application/json");

        }

        #endregion

        #region Deletes

        /// <summary>
        /// Delete document record
        /// </summary>
        /// <param name="ID">ID of document to delete</param>
        /// <returns>Json</returns>

        [HttpPost]
        [CustomValidateAjax]
        [ValidateAntiForgeryToken]
        public JsonResult DeleteDocument(int ID)
        {
            Boolean saveState = false;
            string message = "";
            int userID = Convert.ToInt32(HttpContext.Session["UserID"]);
            int resultStatus = 0;

            //Check enough permission to delete
            if (@ViewBag.PermissionID == (int)PermissionEnum.Permissions.Admin)
            {
                AppData.EntityDocument EntityDocumentData = new AppData.EntityDocument();
                EntityDocument EntityDocumentModel = new EntityDocument();
                EntityDocumentModel = EntityDocumentData.GetDocumentItem(ID); //Needed to get info for moving file below
                resultStatus = EntityDocumentData.DeleteDocument(ID, userID);

                try
                {
                    //Delete file reading file name form path as DocumentName is a friendly name for user
                    if (EntityDocumentModel.DocumentPath != null)
                    {
                        Task.Factory.StartNew(() => DeleteFileFromBlob(Path.GetFileName(EntityDocumentModel.DocumentPath), containerName), TaskCreationOptions.LongRunning);
                    }
                }
                catch { }

                if (resultStatus > 0)
                {
                    saveState = true;
                    message = "Document deleted";
                }
                else
                {
                    saveState = false;
                    message = "Error deleting document. Please try again.";
                }

            }
            else
            {
                message = "This document cannot be deleted due to permission issues. Please try again.";
            }

            return Json(new { Saved = saveState, Title = "Delete Document", Message = message }, JsonRequestBehavior.AllowGet);
        }

        #endregion

    }
}