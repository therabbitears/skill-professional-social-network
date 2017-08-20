using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Wrly
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");


            routes.MapRoute(
                name: "CurrentStatistics",
                url: "entity-statistics",
                defaults: new { controller = "Account", action = "userstatistics" }
            );

            routes.MapRoute(
                    name: "Conversations",
                    url: "conversations",
                    defaults: new { controller = "ChatGroup", action = "StartChat" }
            );
            routes.MapRoute(
                   name: "ConversationsUser",
                   url: "conversations/{id}",
                   defaults: new { controller = "ChatGroup", action = "StartChat" }
           );
            routes.MapRoute(
                    name: "Configurations",
                    url: "configurations",
                    defaults: new { controller = "Account", action = "Configuration" }
            );
            routes.MapRoute(
                    name: "notifications",
                    url: "notifications",
                    defaults: new { controller = "Notification", action = "All" }
            );
            routes.MapRoute(
                    name: "Explore",
                    url: "explore",
                    defaults: new { controller = "Association", action = "Explore" }
            );

            routes.MapRoute(
                   name: "Interests",
                   url: "interests",
                   defaults: new { controller = "Association", action = "Interests" }
           );
            routes.MapRoute(
                   name: "Followings",
                   url: "followings",
                   defaults: new { controller = "Association", action = "Followings" }
           );
            routes.MapRoute(
               name: "followers",
               url: "followers",
               defaults: new { controller = "Association", action = "followers" }
       );
            routes.MapRoute(
                  name: "Connections",
                  url: "connections",
                  defaults: new { controller = "Association", action = "Connections" }
          );

            routes.MapRoute(
                name: "Suggestions",
                url: "suggestions",
                defaults: new { controller = "Association", action = "SuggestionList" }
        );

            routes.MapRoute(
                  name: "ConnectionSent",
                  url: "requests",
                  defaults: new { controller = "Association", action = "ConnectionRequests" }
          );

            routes.MapRoute(
                name: "Activities",
                url: "network/activities",
                defaults: new { controller = "Association", action = "HappeningsAll" }
        );


            routes.MapRoute(
          name: "MyActivities",
          url: "network/my-happenings",
          defaults: new { controller = "Association", action = "MyHappenings" }
  );


            routes.MapRoute(
                 name: "NewOrganization",
                 url: "new-organization",
                 defaults: new { controller = "Account", action = "RegisterOrganization" }
             );

            routes.MapRoute(
   name: "NewGroup",
   url: "groups/new",
   defaults: new { controller = "Groups", action = "New", id = UrlParameter.Optional }
);

            routes.MapRoute(
               name: "MembersGroup",
               url: "groups/members",
               defaults: new { controller = "Groups", action = "Members", id = UrlParameter.Optional }
            );

            routes.MapRoute(
               name: "MemmberList",
               url: "groups/memmberlist",
               defaults: new { controller = "Groups", action = "MemmberList", id = UrlParameter.Optional }
            );



            routes.MapRoute(
                 name: "Groups",
                 url: "groups/{id}/{tab}",
                 defaults: new { controller = "Groups", action = "Index", id = UrlParameter.Optional, tab = UrlParameter.Optional }
            );



            routes.MapRoute(
                 name: "Home",
                 url: "home",
                 defaults: new { controller = "Account", action = "Home" }
            );

            routes.MapRoute(
                name: "Profile",
                url: "profile",
                defaults: new { controller = "Account", action = "Profile" }
        );


            routes.MapRoute(
            name: "ProfileItems",
            url: "profile/{id}",
            defaults: new { controller = "Account", action = "Profile" }
    );


            routes.MapRoute(
                      name: "Search",
                      url: "results",
                      defaults: new { controller = "Search", action = "Results" }
                  );


            routes.MapRoute(
                name: "ManageProfileItems",
                url: "{profilename}/profile-items/{itemtype}",
                defaults: new { controller = "Account", action = "EntityProfile" }
            );

            routes.MapRoute(
                              "Stream", // Route name
                              "cdn/imagestore/{id}/{ext}/width_{w}", // URL with parameters
                              new { controller = "Images", action = "RenderStatic" } // Parameter defaults
                            );
            routes.MapRoute(
                             "WiteBig", // Route name
                             "publishing/write", // URL with parameters
                             new { controller = "Press", action = "WriteBig" } // Parameter defaults
                           );


            routes.MapRoute(
                             "Blogs", // Route name
                             "publishing", // URL with parameters
                             new { controller = "Press", action = "Publishing" } // Parameter defaults
                           );


            routes.MapRoute(
              name: "help",
              url: "help-and-knowledge",
              defaults: new { controller = "Knowledge", action = "Index" }
         );

            routes.MapRoute(
             name: "helpCategory",
             url: "help-and-knowledge/{category}",
             defaults: new { controller = "Knowledge", action = "Index", category = UrlParameter.Optional }
        );

            routes.MapRoute(
             name: "helpCategoryTopic",
             url: "help-and-knowledge/{category}/{topicId}",
             defaults: new { controller = "Knowledge", action = "Index", category = UrlParameter.Optional, topicId = UrlParameter.Optional }
        );


            routes.MapRoute(
           name: "Feed",
           url: "feed",
           defaults: new { controller = "Account", action = "Home" }
       );

            routes.MapRoute(
           name: "About",
           url: "about-sklative",
           defaults: new { controller = "Home", action = "About" }
       );

            

            routes.MapRoute(
           name: "PublicProfile",
           url: "{profilename}",
           defaults: new { controller = "Account", action = "EntityProfile" }
       );


            routes.MapRoute(
           name: "OrganizationProfile",
           url: "fou/{profilename}",
           defaults: new { controller = "Business", action = "Profile" }
       );

            routes.MapRoute(
             name: "BusinessProfileItems",
             url: "fou/{profilename}/profile-items/{itemtype}",
             defaults: new { controller = "Business", action = "Profile" }
         );

            routes.MapRoute(
                 name: "LuceneEngine",
                 url: "l/search-results",
                 defaults: new { controller = "Lucene", action = "Index" }
             );


            routes.MapRoute(
                name: "Privacy",
                url: "policy/privacy",
                defaults: new { controller = "Common", action = "Privacy" }
            );

            routes.MapRoute(
                 name: "Cookie",
                 url: "policy/cookie",
                 defaults: new { controller = "Common", action = "Cookie" }
             );

            routes.MapRoute(
              name: "Terms",
              url: "legal/terms",
              defaults: new { controller = "Common", action = "Terms" }
          );

            routes.MapRoute(
                  name: "Abuse",
                  url: "report/abuse",
                  defaults: new { controller = "Common", action = "Abuse" }
           );

            routes.MapRoute(
                   name: "Feedback",
                   url: "interaction/feedback",
                   defaults: new { controller = "Common", action = "Feedback" }
            );

            routes.MapRoute(
                    name: "Opportunities",
                    url: "opportunities/{id}",
                    defaults: new { controller = "Press", action = "Posts", id = UrlParameter.Optional }
             );

            routes.MapRoute
           (
                 name: "Insights",
                 url: "insights/{id}",
                 defaults: new { controller = "Account", action = "Insights", id = UrlParameter.Optional }
            );
            routes.MapRoute(
                 name: "unsubscribe",
                 url: "emails/unsubscribe",
                 defaults: new { controller = "Account", action = "EmailPreferencesUnsubscribe" }
            );


            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Home", id = UrlParameter.Optional }
            );
        }
    }
}
