using System.Web.Optimization;

namespace ApiAggregator.Web
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            AddStyles(bundles);

            AddScripts(bundles);
        }
        
        public static void AddStyles(BundleCollection bundles)
        {
            // Styles that are application wide
            bundles.Add(new StyleBundle("~/Content/Styles/global")
                .Include("~/Content/application.css")
            );

            // Prettify theme for the json view on the test page
            bundles.Add(new StyleBundle("~/Content/Styles/test")
                .Include("~/Content/Prettify/Themes/desert.css")
            );
        }

        public static void AddScripts(BundleCollection bundles)
        {
            // Scripts required throughout the entire app
            bundles.Add(new ScriptBundle("~/Content/Scripts/global")
                .Include("~/Content/Scripts/jquery-1.9.1.js")
                .Include("~/Content/Scripts/bootstrap.js")
                .Include("~/Content/Scripts/theme.js")
            );

            // Scripts for filtering & deleting from tables
            bundles.Add(new ScriptBundle("~/Content/Scripts/table")
                .Include("~/Content/Scripts/handlebars.js")
                .Include("~/Content/Scripts/prompt.js")
                .Include("~/Content/Scripts/filter.js")
            );

            // Configuration page
            bundles.Add(new ScriptBundle("~/Content/Scripts/configuration")
                .Include("~/Content/Scripts/radioHighlight.js")
                .Include("~/Content/Scripts/configuration.js")
            );

            // Mapping form
            bundles.Add(new ScriptBundle("~/Content/Scripts/mapping")
            .Include("~/Content/Scripts/radioHighlight.js")
                .Include("~/Content/Scripts/mapping.js")
            );

            // Service form
            bundles.Add(new ScriptBundle("~/Content/Scripts/service")
                .Include("~/Content/Scripts/handlebars.js")
                .Include("~/Content/Scripts/radioHighlight.js")
                .Include("~/Content/Scripts/service.js")
            );

            // Testing page
            bundles.Add(new ScriptBundle("~/Content/Scripts/test")
                .Include("~/Content/Scripts/Prettify/prettify.js")
                .Include("~/Content/Scripts/testing.js")
            );
        }
    }
}