using System.Web;
using System.Web.Optimization;

namespace WebUI
{
    public class BundleConfig
    {

        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery.hoverIntent.min.js"
             ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                  "~/Scripts/jquery.validate*",
                  "~/Scripts/jquery.unobtrusive-ajax.js"
              ));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                 "~/Scripts/bootstrap.js",
                "~/Scripts/bootstrap-multiselect.js",
                "~/Scripts/bootstrap.bundle.min.js",
                "~/Scripts/respond.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jquery-UI").Include(
               "~/Scripts/jquery-ui.min.js",
                "~/Scripts/jquery.dialogOptions.js",
                "~/Scripts/jquery.ui.widget.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/datatables").Include(
                "~/Scripts/AddOn-DataTables/datatables.min.js",
                "~/Scripts/AddOn-DataTables/dataTables.keyTable.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/jqueryfileupload").Include(
                "~/Scripts/jquery.fileupload.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/application").Include(
                "~/Scripts/select2.full.min.js",
                "~/Scripts/Application/Constants.js",
                "~/Scripts/Application/Application.js",
                "~/Scripts/Application/jquery.form.min.js"
            ));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/jquery-ui.min.css",
                "~/Content/jquery-ui.structure.min.css",
                "~/Content/jquery-ui.theme.min.css",
                "~/Content/bootstrap.css",
                "~/Content/PagedList.css",
                "~/Content/bootstrap-multiselect.css",
                 "~/Content/select2.min.css",
                "~/Content/AddOn-DataTables/datatables.min.css",
                "~/Content/AddOn-DataTables/keyTable.dataTables.min.css",
                "~/Content/Site.min.css"
            ));

            BundleTable.EnableOptimizations = false;
        }
    }
}
