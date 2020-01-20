using System.Web;
using System.Web.Optimization;

namespace GBHO_Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {

            /* Public */
             
            bundles.Add(new ScriptBundle("~/bundles/public/js")
                .Include("~/App_Themes/Public/js/jquery.min.js")
                .Include("~/App_Themes/Public/js/jquery.easing.1.3.js")
                .Include("~/App_Themes/Public/js/bootstrap.min.js")
                .Include("~/App_Themes/Public/js/jquery.waypoints.min.js")
                .Include("~/App_Themes/Public/js/jquery.flexslider-min.js")
                .Include("~/App_Themes/Public/js/jquery.magnific-popup.min.js")
                .Include("~/App_Themes/Public/js/magnific-popup-options.js")
                .Include("~/App_Themes/Public/js/main.js")
                );

            bundles.Add(new StyleBundle("~/bundles/public/css")
                .Include("~/App_Themes/Public/css/animate.css")
                .Include("~/App_Themes/Public/css/flexslider.css")
                .Include("~/App_Themes/Public/css/magnific-popup.css")
                .Include("~/App_Themes/Public/css/bootstrap.css")
                .Include("~/App_Themes/Public/css/style.css")
                );

            bundles.Add(new ScriptBundle("~/bundles/public_v2/js")
                .Include("~/App_Themes/Public/v2/js/jquery.js")
                .Include("~/App_Themes/Public/v2/js/jquery.easing.1.3.js")
                .Include("~/App_Themes/Public/v2/js/bootstrap.js")
                .Include("~/App_Themes/Public/v2/js/jcarousel/jquery.jcarousel.min.js")
                .Include("~/App_Themes/Public/v2/js/jquery.fancybox.pack.js")
                .Include("~/App_Themes/Public/v2/js/jquery.fancybox-media.js")
                .Include("~/App_Themes/Public/v2/js/google-code-prettify/prettify.js")
                .Include("~/App_Themes/Public/v2/js/portfolio/jquery.quicksand.js")
                .Include("~/App_Themes/Public/v2/js/portfolio/setting.js")
                .Include("~/App_Themes/Public/v2/js/jquery.flexslider.js")
                .Include("~/App_Themes/Public/v2/js/jquery.nivo.slider.js")
                .Include("~/App_Themes/Public/v2/js/modernizr.custom.js")
                .Include("~/App_Themes/Public/v2/js/jquery.ba-cond.min.js")
                .Include("~/App_Themes/Public/v2/js/jquery.slitslider.js")
                .Include("~/App_Themes/Public/v2/js/animate.js")
                .Include("~/App_Themes/Public/v2/js/custom.js")
                );

            bundles.Add(new StyleBundle("~/bundles/public_v2/css")
                .Include("~/App_Themes/Public/v2/css/bootstrap.css")
                .Include("~/App_Themes/Public/v2/css/bootstrap-responsive.css")
                .Include("~/App_Themes/Public/v2/css/fancybox/jquery.fancybox.css")
                .Include("~/App_Themes/Public/v2/css/jcarousel.css")
                .Include("~/App_Themes/Public/v2/css/flexslider.css")
                );
            

            /* Login */
            
            bundles.Add(new ScriptBundle("~/bundles/login/js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/jquery-2.1.4.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/popper.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/plugins.js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/main.js")
                );

            bundles.Add(new StyleBundle("~/bundles/login/css")
                .Include("~/App_Themes/MyAccount/assets/css/normalize.css")
                .Include("~/App_Themes/MyAccount/assets/css/bootstrap.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/font-awesome.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/themify-icons.css")
                .Include("~/App_Themes/MyAccount/assets/css/flag-icon.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/cs-skin-elastic.css")
                .Include("~/App_Themes/MyAccount/assets/scss/style.css")
                );


            /* MyAccount */
            bundles.Add(new ScriptBundle("~/bundles/myaccount/js")
                .Include("~/App_Themes/MyAccount/assets/js/popper.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/plugins.js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/moment.js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/bootstrap-datetimepicker.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/vendor/jquery.maskedinput.min.js")                
                .Include("~/App_Themes/MyAccount/assets/js/main.js")
                .Include("~/App_Themes/MyAccount/assets/js/custom.js")
                );

            bundles.Add(new StyleBundle("~/bundles/myaccount/css")
                .Include("~/App_Themes/MyAccount/assets/css/normalize.css")
                .Include("~/App_Themes/MyAccount/assets/css/bootstrap.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/font-awesome.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/bootstrap-datetimepicker.css")           
                .Include("~/App_Themes/MyAccount/assets/css/themify-icons.css")
                .Include("~/App_Themes/MyAccount/assets/css/flag-icon.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/cs-skin-elastic.css")
                .Include("~/App_Themes/MyAccount/assets/css/lib/datatable/dataTables.bootstrap.min.css")
                .Include("~/App_Themes/MyAccount/assets/scss/style.css")
                .Include("~/App_Themes/MyAccount/assets/css/lib/vector-map/jqvmap.min.css")
                .Include("~/App_Themes/MyAccount/assets/css/datepicker-icons.css")                
                );

            bundles.Add(new ScriptBundle("~/bundles/myaccount/data-table/js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/datatables.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/dataTables.bootstrap.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/dataTables.buttons.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/buttons.bootstrap.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/jszip.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/pdfmake.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/vfs_fonts.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/buttons.html5.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/buttons.print.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/buttons.colVis.min.js")
                .Include("~/App_Themes/MyAccount/assets/js/lib/data-table/datatables-init.js")
                );

            bundles.IgnoreList.Clear();
            BundleTable.EnableOptimizations = true;
        }
    }
}