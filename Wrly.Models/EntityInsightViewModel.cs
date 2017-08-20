using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class EntityInsightViewModel : BaseViewModel, IAuthor
    {
        #region Author
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string FormatedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }
        public int EntityType { get; set; }

        public string AuthorName
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    return FormatedName;
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return Name;
                }
                return null;
            }
        }
        public string Heading
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    return ProfileHeading;
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return Category;
                }
                return null;
            }
        }
        public string ProfilePhotoUrl
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    if (!string.IsNullOrEmpty(ProfilePath))
                    {
                        return ProfilePath;
                    }
                    return "/content/images/no-image.png";
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    if (!string.IsNullOrEmpty(LogoPath))
                    {
                        return LogoPath;
                    }
                    return "/content/images/o/no-image.png";
                }
                return null;
            }
        }
        public string ProfilePhotoIconUrl
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfilePhotoUrl))
                {
                    return ProfilePhotoUrl.ImagePath(ProfilePhotoUrl, 50);
                }
                return null;
            }
        }
        public string ProfileUrl
        {
            get
            {
                if (EntityType == (byte)Enums.EntityTypes.Person)
                {
                    return ProfileName.ToLower();
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return string.Format("fou/{0}", Url);
                }
                return null;
            }
        }
        #endregion

        public int Type { get; set; }
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
