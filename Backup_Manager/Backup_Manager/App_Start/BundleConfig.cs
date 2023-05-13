using System.Web;
using System.Web.Optimization;

namespace Backup_Manager
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                         //"~/Scripts/jquery-1.6.4.min.js",
                         "~/Scripts/jquery-{version}.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));
            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Content/AdminLTE-3.0.0-alpha.2/plugins/jquery/jquery.js",
                      "~/Content/AdminLTE-3.0.0-alpha.2/plugins/bootstrap/js/bootstrap.bundle.min.js",
                      "~/Content/AdminLTE-3.0.0-alpha.2/plugins/datatables/jquery.dataTables.min.js"));
            bundles.Add(new ScriptBundle("~/bundles/global-js").Include(
                      "~/Content/AdminLTE-3.0.0-alpha.2/swal/sweetalert2@9.js",
                      "~/Scripts/Helper.js",
                      "~/Scripts/JSGlobal.js"));
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/AdminLTE-3.0.0-alpha.2/dist/css/adminlte.min.css",
                      "~/Content/AdminLTE-3.0.0-alpha.2/dist/css/ionicons.css",
                      "~/Content/AdminLTE-3.0.0-alpha.2/plugins/font-awesome/css/font-awesome.min.css",
                      "~/Content/AdminLTE-3.0.0-alpha.2/plugins/font-awesome/css/font-awesome-fonts.css",
                      "~/Content/AdminLTE-3.0.0-alpha.2/plugins/datatables/jquery.dataTables.min.css",
                      "~/Content/AdminLTE-3.0.0-alpha.2/dist/css/fonts.css",
                      "~/Content/site.css"));
        }
    }
}
