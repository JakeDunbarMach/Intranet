using ICSharpCode.SharpZipLib.Zip;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WebUI.Controllers.Custom
{
    /// <summary>
    /// Base controller that other controllers all inherit from
    /// Set ViewBag.UserName so not manually setting in all controller actions
    /// By default caching is 0, use the custom authorisation and custom error handling
    /// </summary>
    [OutputCache(Duration = 0, NoStore = true)]
    [CustomAuthorizeAttribute]
    [CustomHandleError]
    public class SystemControllerBase : Controller
    {

        /// <summary>
        /// Override the method OnActionExecuting to set the ViewBag UserName to be the session variable 
        /// as needed for the SystemController PermissionError method and view
        /// </summary>
        /// <param name="filterContext"></param>
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.UserName = HttpContext.Session["UserName"];
            ViewBag.UserID = HttpContext.Session["UserID"];
            ViewBag.PermissionID = HttpContext.Session["PermissionID"];
            ViewBag.ManagedByID = HttpContext.Session["ManagedByID"];
            ViewBag.UserEmployeeTypeID = HttpContext.Session["UserEmployeeTypeID"];
            ViewBag.JavaScriptVersion = HttpContext.Session["JavaScriptVersion"];
            ViewBag.SystemEnvironment = HttpContext.Application["SystemEnvironment"];

            base.OnActionExecuting(filterContext);
        }

        #region File Saving

        /// <summary>
        /// Remove spaces, etc when saving filename to Azure as stops them being viewed on the device
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>string of formatted fileName</returns>
        public string FormatFilename(string fileName)
        {
            try
            {
                string fileExtension = Path.GetExtension(fileName).ToLower();
                string name = Path.GetFileNameWithoutExtension(fileName);
                return name.Replace(" ", "").Replace(".", "_") + fileExtension;
            }
            catch
            {
                return fileName;
            }
        }

        /// <summary>
        /// Check if a blob already exists
        /// </summary>
        /// <param name="filename">name of file to check exists</param>
        /// <param name="containerName">container to check in</param>
        /// <returns></returns>
        public bool CheckBlobExists(string filename, string containerName)
        {
            string storageConnectionString = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_CONNECTION];
            var storageClient = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageClient.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);
            var blockBlob = blobContainer.GetBlockBlobReference(filename);
            return blockBlob.Exists();

        }

        /// <summary>
        /// Code provided by Rarely Impossible for saving to Azure
        /// </summary>
        /// <param name="filename">name of file uploaded</param>
        /// <param name="stream">stream of file contents</param>
        /// <param name="containerName">name of container to save file in</param>
        /// <returns></returns>
        public string SaveFileToBlob(string filename, Stream stream, string containerName)
        {
            string storageConnectionString = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_CONNECTION];
            var storageClient = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageClient.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);

            blobContainer.CreateIfNotExists();
            BlobContainerPermissions permissions = blobContainer.GetPermissions();
            permissions.PublicAccess = BlobContainerPublicAccessType.Off;
            blobContainer.SetPermissions(permissions);

            var blockBlob = blobContainer.GetBlockBlobReference(filename);
            blockBlob.UploadFromStream(stream);
            var baseUri = new Uri(blobContainer.Uri.AbsoluteUri + '/');
            return new Uri(baseUri, filename).AbsoluteUri;
        }


        /// <summary>
        /// Delete Azure blob from specified container
        /// </summary>
        /// <param name="filename">name of file to delete</param>
        /// <param name="containerName">name of container file is in</param>
        public static async void DeleteFileFromBlob(string filename, string containerName)
        {
            string storageConnectionString = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_CONNECTION];
            var storageClient = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageClient.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);

            string newFileName = "Del_" + DateTime.Now.ToString("yyyyMMddhhmmss") + "_" + filename;
            await blobContainer.CreateIfNotExistsAsync();

            CloudBlockBlob blobCopy = blobContainer.GetBlockBlobReference(newFileName);

            if (!await blobCopy.ExistsAsync())
            {
                CloudBlockBlob blob = blobContainer.GetBlockBlobReference(filename);

                if (await blob.ExistsAsync())
                {
                    await blobCopy.StartCopyAsync(blob);
                    await blob.DeleteIfExistsAsync();
                }
            }
        }


        public static async Task<string> ConvertBlobToBase64Image(string storageConnectionString, string containerName, string fileName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(storageConnectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(containerName);
                CloudBlockBlob blob = container.GetBlockBlobReference(fileName);

                Stream blobStream = await blob.OpenReadAsync();

                using (var memoryStream = new MemoryStream())
                {
                    blobStream.CopyTo(memoryStream);
                    return "data:image/png;base64," + Convert.ToBase64String(memoryStream.ToArray());
                }
            }
            catch (Exception e)
            {
                string guid = Guid.NewGuid().ToString();

                AppData.ErrorLog errorLog = new AppData.ErrorLog();

                errorLog.InsertError(guid, e.Message.ToString());

                throw;
            }
        }


        public static async Task<string> ZipFilesAsync(string storageConnectionString, string containerName, string zipFilename, string[] files)
        {
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnectionString);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(containerName);

            var outputMemStream = new MemoryStream();
            var zipOutputStream = new ZipOutputStream(outputMemStream);
            foreach (var fileName in files)
            {
                zipOutputStream.SetLevel(5);
                var blob = cloudBlobContainer.GetBlockBlobReference(fileName);
                var entry = new ZipEntry(fileName);
                zipOutputStream.PutNextEntry(entry);
                await blob.DownloadToStreamAsync(zipOutputStream);
            }

            zipOutputStream.Finish();
            zipOutputStream.IsStreamOwner = false;
            outputMemStream.Position = 0;
            var blobU = cloudBlobContainer.GetBlockBlobReference(zipFilename);
            await blobU.UploadFromStreamAsync(outputMemStream);

            //// Returning the URI of the freshly created resource
            return blobU.Uri.ToString();

        }


        /// <summary>
        /// Create a sas token for the container for allowing access to the container
        /// </summary>
        /// <param name="containerName">name of container to set up sas on</param>
        /// <returns></returns>
        internal string CreateSharedAccessPolicy(string containerName)
        {
            string storageConnectionString = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_CONNECTION];
            var storageClient = CloudStorageAccount.Parse(storageConnectionString);
            var blobClient = storageClient.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference(containerName);

            var sasToken = blobContainer.GetSharedAccessSignature(new SharedAccessBlobPolicy()
            {
                Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.List | SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddHours(-1), //Set start date back because of time zone/UTC issues
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(20),
            });
            return string.Format(CultureInfo.InvariantCulture, sasToken);
        }

        #endregion                

        #region Fill Lookup Lists


        /// <summary>
        /// Generic method that take a List of T and returns a List of SelectListItem for use with an HTML DropDownList 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entities"></param>
        /// <param name="funcToGetValue"></param>
        /// <param name="funcToGetText"></param>
        /// <returns></returns>
        public List<SelectListItem> CreateSelectList<T>(IList<T> entities, Func<T, object> funcToGetValue, Func<T, object> funcToGetText)
        {
            if (entities != null)
            {
                return entities
                    .Select(x => new SelectListItem
                    {
                        Value = funcToGetValue(x).ToString(),
                        Text = funcToGetText(x).ToString()
                    }).ToList();
            }
            else
            {
                return new List<SelectListItem>(); //Prevents the DropDownListFor crashing
            }
        }

        public List<SelectListItem> CreateSelectList<T>(IList<T> entities, Func<T, object> funcToGetValue, Func<T, object> funcToGetText, int selectedOptionValue)
        {
            if (entities != null)
            {
                return entities
                    .Select(x => new SelectListItem
                    {
                        Value = funcToGetValue(x).ToString(),
                        Text = funcToGetText(x).ToString(),
                        Selected = (int.Parse(funcToGetValue(x).ToString()) == selectedOptionValue ? true : false)
                    }).ToList();
            }
            else
            {
                return new List<SelectListItem>(); //Prevents the DropDownListFor crashing
            }
        }

        /// <summary>
        /// This fills a list of SelectListItems with the hours 0 - 23
        /// </summary>
        /// <param name="selected">selected - value of hours from model</param>
        /// <returns>List of hours</returns>
        public static List<SelectListItem> FillHours(string modelHour)
        {
            List<SelectListItem> hours = new List<SelectListItem>();
            for (int i = 0; i < 24; i++)
            {
                hours.Add(new SelectListItem() { Text = i.ToString("00"), Value = i.ToString() });
            }

            //Loop through and set selected item to string passed in. Will default to 0 as first item in list
            if (modelHour != "0")
            {
                foreach (var item in hours)
                {
                    if (item.Value == modelHour)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            return hours;
        }

        /// <summary>
        /// This fills a list of SelectListItems with the minutes 0 - 59
        /// </summary>
        /// <param name="selected">selected - value of minutes from model</param>
        /// <returns>List of minutes</returns>
        public static List<SelectListItem> FillMinutes(string modelMinute)
        {
            List<SelectListItem> minutes = new List<SelectListItem>();
            for (int i = 0; i < 60; i++)
            {
                minutes.Add(new SelectListItem() { Text = i.ToString("00"), Value = i.ToString() });
            }

            //Loop through and set selected item to string passed in. Will default to 0 as first item in list
            if (modelMinute != "0")
            {
                foreach (var item in minutes)
                {
                    if (item.Value == modelMinute)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }
            return minutes;
        }

        #endregion Fill Lookup Lists

        #region Data Cleansing

        /// <summary>
        /// Escape special characters for when exporting to csv
        /// </summary>
        /// <param name="fieldValue">value of field to test</param>
        /// <returns>string of formatted fieldValue</returns>
        public string CSVFieldFormat(string fieldValue)
        {
            if (fieldValue != null && fieldValue.Contains("="))
            {
                return "'" + fieldValue + "'";
            }
            else
            {
                return fieldValue;
            }
        }

        #endregion

        #region Email Validation
        public static class EmailValidator
        {

            static Regex ValidEmailRegex = CreateValidEmailRegex();
            private static Regex CreateValidEmailRegex()
            {
                string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                    + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                    + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

                return new Regex(validEmailPattern, RegexOptions.IgnoreCase);
            }

            internal static bool EmailIsValid(string emailAddress)
            {
                String[] addresses = emailAddress.Replace(" ", string.Empty).Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (String address in addresses)
                {
                    if (!ValidEmailRegex.IsMatch(address))
                        return false;
                }
                return true;
            }
        }
        #endregion Email Validation

    }
}