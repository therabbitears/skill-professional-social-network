using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using Microsoft.Owin.Security.Google;
using Microsoft.Owin.Security.Facebook;

namespace Wrly
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login")
            });
            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");


             //Live
            var options = new FacebookAuthenticationOptions()
            {
                AppId = "567239779977614",
                AppSecret = "d855b299f85b0df08034b4ee54da58fb"
            };
            ////options.Scope.Add("public_profile");
            app.UseFacebookAuthentication(options);

            //app.UseFacebookAuthentication(
            //   appId: "1785821611668809",
            //   appSecret: "49f6ceeee5c5637d2146f7f44ed6885c");
            

            var googleOAuth2AuthenticationOptions = new GoogleOAuth2AuthenticationOptions
            {
                ClientId = "855860101438-fe0pluk9p25rqerbv9lb5t7nubrbutej.apps.googleusercontent.com",
                ClientSecret = "UcnFlbvlRSSTsVmuOg1flbw-"
            };

            app.UseGoogleAuthentication(googleOAuth2AuthenticationOptions);
        }
    }
}