using System.Web;
using System.Web.Optimization;

namespace Wrly
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            // Bundeles New Mobile Names Layout
            bundles.Add(new StyleBundle("~/cdn/_LayoutStyles").Include
                (
                "~/Content/css/shared/main.css",
                "~/Content/css/shared/theme_base.css"
                ));

            // Bundeles New Mobile Names Layout
            bundles.Add(new StyleBundle("~/cdn/_MobileLayoutStyles").Include
                (
                "~/Content/css/shared/main.css",
                "~/Content/css/shared/theme_base.css",
                "~/Content/Views/shared.mobile.css"
                ));


            // Bundeles New Mobile Names Layout
            bundles.Add(new StyleBundle("~/cdn/_AuthLayoutStyles").Include
                (
                    "~/Content/extensions/featherlight/popupwindow.css",
                    "~/Content/assets/hovercard-master/hovercard-master/jquery-ui.min.css"
                ));

            // Bundeles New Mobile Names Layout
            bundles.Add(new StyleBundle("~/cdn/_MobileAuthLayoutStyles").Include
                (
                    "~/Content/extensions/featherlight/popupwindow.css",
                    "~/Content/assets/hovercard-master/hovercard-master/jquery-ui.min.css",
                    "~/Content/css/shared/mobile.css"
                ));

            // Bundeles New Mobile Names Layout
            bundles.Add(new StyleBundle("~/cdn/_WizardLayoutStyles").Include
                (
                    "~/Content/Views/wizard/_shared.css",
                    "~/Content/assets/hovercard-master/hovercard-master/jquery-ui.min.css"
                ));

            bundles.Add
                (
                new ScriptBundle("~/cdn/_AuthLayoutScripts").Include
                (
                    "~/scripts/common.js",
                    "~/Scripts/jquery-jtemplates_uncompressed.js",
                    "~/Content/extensions/featherlight/popupwindow.js"
                ));

            bundles.Add
               (
               new ScriptBundle("~/cdn/_JTemplates").Include
               (
                   "~/Scripts/jquery-jtemplates_uncompressed.js"
              ));

            // Page scripts
            bundles.Add
             (
             new ScriptBundle("~/cdn/_ProfilePublic").Include
             (
                 "~/Content/ViewResources/Scripts/Profile/common.js",
                 "~/Content/ViewResources/Scripts/Profile/public.js"
             ));

            bundles.Add
             (
             new ScriptBundle("~/cdn/_ProfileManage").Include
             (
                 "~/Content/ViewResources/Scripts/Profile/common.js",
                 "~/Content/ViewResources/Scripts/Profile/manage.js"
             ));


            bundles.Add
            (
            new ScriptBundle("~/cdn/_BusinessProfileManage").Include
            (
                "~/Content/Views/Profile/manage_business.js"
            ));


            bundles.Add
           (
           new ScriptBundle("~/cdn/_AuthHomeScripts").Include
           (
               "~/Content/Views/Home/auth_home.js",
               "~/Content/ViewResources/Scripts/News/news-feeds.js"
           ));


            bundles.Add(new StyleBundle("~/cdn/_AuthHomeStyles").Include
           (
                "~/Content/Views/Home/auth_home.css"
           ));


            bundles.Add(new StyleBundle("~/cdn/_AuthHomeMobileStyles").Include
         (
              "~/Content/Views/Home/auth_home.css",
              "~/Content/Views/Home/auth_home_mobile.css"
         ));



            bundles.Add
            (
            new ScriptBundle("~/cdn/_ProfileCommon").Include
            (
                "~/Content/ViewResources/Scripts/Profile/common.js"
            ));


            bundles.Add
        (
        new ScriptBundle("~/cdn/_NewsFeeds").Include
        (
            "~/Content/ViewResources/Scripts/News/news-feeds.js"
        ));




            bundles.Add
               (
               new ScriptBundle("~/cdn/_Profile").Include
               (
                   "~/Content/ViewResources/Scripts/Profile/general.js"
               ));



            bundles.Add(new ScriptBundle("~/cdn/_MobileLayoutScripts").Include
                (
                "~/scripts/slideout.js"
                ));

            bundles.Add(new ScriptBundle("~/cdn/grpcht").Include
                (
                "~/Scripts/jquery-1.10.2.min.js",
                "~/Scripts/jquery.signalR-2.2.0.min.js",
                "~/Content/Views/scripts/chatgroup/general.js"
                ));


            // Page styles

            bundles.Add(new StyleBundle("~/cdn/_ProfileSharedStyles").Include
            (
                "~/Content/Views/profile/shared.css",
                "~/Content/Views/css/profile/profile.css"
            ));

            bundles.Add(new StyleBundle("~/cdn/_ProfilePublicStyles").Include
              (
                  "~/Content/Views/profile/public.css"
              ));

            bundles.Add(new StyleBundle("~/cdn/_ProfileManageStyles").Include
              (
                  "~/Content/Views/profile/manage.css"
              ));


            /// Pages
            /// Configuration
            bundles.Add(new StyleBundle("~/cdn/_ConfigurationStyles").Include
            (
                "~/Content/Views/Account/configuration.css"
            ));

            bundles.Add(new ScriptBundle("~/cdn/_ConfigurationScripts").Include
                (
                    "~/Content/Views/Account/configuration.js"
                ));

            /// End Configuration

            /// Activity
            bundles.Add(new ScriptBundle("~/cdn/_ActivitiesScripts").Include
            (
                "~/Content/Views/network/activities.js"
            ));

            /// End Activity

            /// Network
            bundles.Add(new StyleBundle("~/cdn/_NetworkSharedStyles").Include
           (
               "~/Content/Views/network/_shared.css"
           ));

            bundles.Add(new ScriptBundle("~/cdn/_NetworkSharedScripts").Include
            (
                "~/Content/Views/network/shared.js"
            ));

            /// End Network

            /// Import
            bundles.Add(new StyleBundle("~/cdn/_ImportStyles").Include
           (
               "~/Content/Views/import/import.css"
           ));

            bundles.Add(new ScriptBundle("~/cdn/_ImportScripts").Include
            (
                "~/Content/Views/import/import.js"
            ));

            /// End Import

            /// Wizard Add Career
            bundles.Add(new ScriptBundle("~/cdn/_WizardAddCareerScripts").Include
         (
             "~/Content/Views/wizard/careerHistory.js",
             "~/Scripts/jquery-ui.min.js"
         ));

            /// End Wizard Add Career

            /// Wizard Add Skill
            bundles.Add(new ScriptBundle("~/cdn/_WizardAddSkillsScripts").Include
         (
             "~/Scripts/jquery-ui.min.js",
             "~/Content/Views/wizard/skills.js"
         ));

            /// Wizard Add Skill
            bundles.Add(new ScriptBundle("~/cdn/_WizardAddSkillsScript").Include
         (
             "~/Content/Views/wizard/skills.js"
         ));

            /// End Wizard Add Career

            /// Account
            bundles.Add(new StyleBundle("~/cdn/_AccountSharedStyles").Include
           (
               "~/Content/Views/Account/shared.css"
           ));

            /// Account End

            /// Home
            bundles.Add(new StyleBundle("~/cdn/_HomeSharedStyles").Include
           (
               "~/Content/Views/Home/_shared.css"
           ));


            bundles.Add(new StyleBundle("~/cdn/_HomeMobileSharedStyles").Include
           (
               "~/Content/Views/Home/mobile.css"
           ));
            /// Home End

            // Wizard mobile
            bundles.Add(new StyleBundle("~/cdn/_WizardMobileStyles").Include
       (
           "~/Content/Views/wizard/wizard.mobile.css"
       ));

            // Feeds
            bundles.Add(new StyleBundle("~/cdn/_NewsFeedStyles").Include
       (
           "~/Content/Views/Press/_Feeds.css"
       ));

        }

    }
}
