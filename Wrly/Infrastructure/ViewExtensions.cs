using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Wrly.Data.Repositories.Implementors;
using Wrly.Infrastructure.Utils;
using Wrly.Models;
using System.Web.Mvc;
namespace Wrly
{
    public static class ViewExtensions
    {
        public static HtmlString UserName(this HtmlHelper helper)
        {
            if (UserCacheManager.Face == null)
            {
                return  new HtmlString("anonymous");
            }
            return new HtmlString(UserCacheManager.Face.AuthorName);
        }

        public static int EntityType(this HtmlHelper helper)
        {
            if (UserCacheManager.Face == null)
            {
                return 0;
            }
            return UserCacheManager.Face.EntityType;
        }

        public static long EntityID(this HtmlHelper helper)
        {
            if (UserCacheManager.Face==null)
            {
                return 0;
            }
            return UserCacheManager.Face.EntityID;
        }

        public static HtmlString UserNamePhoto(this HtmlHelper helper, int w = 50)
        {
            if (UserCacheManager.Face!=null)
            {
                return new HtmlString(ImagePath(helper, UserCacheManager.Face.AuthorPhoto, 50));    
            }
            return new HtmlString("/content/images/no-image.png"); 
        }

        public static HtmlString UserProfileHeading(this HtmlHelper helper)
        {
            return new HtmlString(UserCacheManager.Face.Heading);
        }

        public static async Task<ClaimsIdentity> GenerateUserIdentityAsync(this UserManager<ApplicationUser> manager, ApplicationUser user)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            using (AccountRepository repository = new AccountRepository())
            {
                using (var objectDataSet = repository.GetUserHashData(user.EmailAddress))
                {
                    var userHash = objectDataSet.Tables[0].FromDataTable<UserHash>()[0];
                    // Add custom user claims here => this.OrganizationId is a value stored in database against the user
                    userIdentity.AddClaim(new Claim("EntityID", userHash.EntityID.ToString()));
                    userIdentity.AddClaim(new Claim("PersonID", userHash.PersonID.ToString()));
                    userIdentity.AddClaim(new Claim("OrganizationID", userHash.OrganizationID.ToString()));
                    userIdentity.AddClaim(new Claim("EntityType", userHash.EntityType.ToString()));
                }
            }
            // Add custom user claims here
            return userIdentity;
        }

        private static string ImagePath(this HtmlHelper helper, string path, int w = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (w == int.MaxValue)
                {
                    return string.Format(path, "full");
                }
                return string.Format(path, w);
            }
            return null;
        }

        private static string ImagePath(this string helper, string path, int w = int.MaxValue)
        {
            if (!string.IsNullOrEmpty(path))
            {
                if (w == int.MaxValue)
                {
                    return string.Format(path, "full");
                }
                return string.Format(path, w);
            }
            return null;
        }
    }
}