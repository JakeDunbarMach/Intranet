using WebUI.Controllers.Custom;
using System.Web.Mvc;
using Syncfusion.Pdf;

using Syncfusion.HtmlConverter;
using System.IO;
using System.Threading.Tasks;
using System.Configuration;
using System;

namespace WebUI.Controllers
{
    public class HomeController : SystemControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Pdf()
        {

            //Initialize the HTML to PDF converter 
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

            WebKitConverterSettings settings = new WebKitConverterSettings();

            //Set WebKit path
            settings.WebKitPath = Server.MapPath("~/bin/QtBinariesWindows");

            //Set additional delay; units in milliseconds;
            settings.AdditionalDelay = 100;

            //Assign WebKit settings to HTML converter
            htmlConverter.ConverterSettings = settings;

            //Convert URL to PDF
            PdfDocument document = htmlConverter.Convert("https://www.google.com");

            //Saving the PDF to the MemoryStream
            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            //Close the document.
            document.Close(true);

            //Download the PDF document in the browser
            return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "Output.pdf");
        }

        /// <summary>
        /// Creates a PDF by Getting an image from a storage accounts then converting it to a Base64 string and putting it into 
        /// the image SRC.
        /// You will need to add file(s) to the storage container and reference them below
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> HtmlImageToPdf()
        {
            string storageConnectionString = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_CONNECTION];
            //Create shared access policy for container to allow access. Needed to append onto querystring of document path

            string containerName = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_FOLDER];

            string blobImagelogo = "https://webtemplatestoragedev.blob.core.windows.net/documents/10_Jellyfish.jpg";
            string imageBase64 = await ConvertBlobToBase64Image(storageConnectionString, containerName, "14_Tulips.jpg");


            //Initialize HTML to PDF converter 
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

            //Initialize PDF Settings
            WebKitConverterSettings settings = new WebKitConverterSettings();

            //HTML string and Base URL 
            string htmlText = "<html><body>" +
                "<p>Static Image (wwwroot)</p>" +
                "<img src='19_Sample.jpg' alt=\"19_Sample.jpg\" width=\"360\" height=\"80\">" +
                "<p>Blob Image (storage account)</p>" +
                "<img src=\"" + imageBase64 + "\" alt=\"14_Tulips.jpg\">" +
                "<p><a href=\"" + blobImagelogo + "\">" + blobImagelogo + "</a></p>" +
                "</body></html>";

            //Set the baseUrl this can point to the storage account or to the wwwroot folder
            string baseUrl = "https://webtemplatestoragedev.blob.core.windows.net/documents"; //_env.WebRootPath;

            //Set WebKit path
            settings.WebKitPath = Server.MapPath("~/bin/QtBinariesWindows");

            //Assign WebKit settings to HTML converter
            htmlConverter.ConverterSettings = settings;

            //Convert HTML string to PDF
            PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);


            //Save the document into stream.
            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            //Close the document.
            document.Close(true);

            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";

            //Define the file name.
            string fileName = " Output.pdf";

            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);
        }

        /// <summary>
        /// Creates a PDF with image in it.  
        /// You will need to add files to the storage container and reference them below
        /// </summary>
        /// <returns></returns>
        public ActionResult HtmlToPDF()
        {
            //Initialize HTML to PDF converter 
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter(HtmlRenderingEngine.WebKit);

            //Initialize PDF Settings
            WebKitConverterSettings settings = new WebKitConverterSettings();

            //HTML string and Base URL 
            string htmlText = "<html><body>" +
                 "<p>This image is from web templates websie, using baseURL</p>" +
                "<img src=\"14_Tulips.jpg\" alt=\"Tulips.jpg\"><br />" +
                "<p>This image is from this website, using full URL</p>" +
                "<img src=\"https://webtemplatestoragedev.blob.core.windows.net/documents/10_Jellyfish.jpg\" alt=\"10_Jellyfish.jpg\"><br />" +
                "</body></html>";

            //Set the baseUrl this can point to the storage account or to the wwwroot folder
            string baseUrl = "https://webtemplatestoragedev.blob.core.windows.net/documents"; //_env.WebRootPath;

            //Set WebKit path to the library files
            settings.WebKitPath = Server.MapPath("~/bin/QtBinariesWindows");
            //Assign WebKit settings to HTML converter
            htmlConverter.ConverterSettings = settings;

            //Convert HTML string to PDF
            PdfDocument document = htmlConverter.Convert(htmlText, baseUrl);

            //Save the document into stream.
            MemoryStream stream = new MemoryStream();

            document.Save(stream);

            stream.Position = 0;

            //Close the document.
            document.Close(true);

            //Defining the ContentType for pdf file.
            string contentType = "application/pdf";

            //Define the file name.
            string fileName = " Output.pdf";

            //Creates a FileContentResult object by using the file contents, content type, and file name.
            return File(stream, contentType, fileName);
        }

        /// <summary>
        /// Creates a Zip package
        /// You will need to add files to the storage container and reference them below
        /// 
        /// Once you have excuted refresh the storage container and you will see the test.zip file in a new folder called
        /// archive
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> CreateZipPackageAsync()
        {
            string storageConnectionString = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_CONNECTION];
            //Create shared access policy for container to allow access. Needed to append onto querystring of document path

            string containerName = ConfigurationManager.AppSettings[AppData.Constants.Configuration.STORAGE_FOLDER];

            //string ED_SAS = BlobStorageService.CreateSharedAccessPolicy(storageConnectionString, containerName);
            string zipFilename = "archive/test.zip";
            var filesToZip = new string[] { "0_Tulips.jpg", "10_Sample.jpg" };
            await ZipFilesAsync(storageConnectionString, containerName, zipFilename, filesToZip);

            return View("Index");
        }

    }
}