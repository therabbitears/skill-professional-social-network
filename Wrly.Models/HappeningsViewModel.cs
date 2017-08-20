using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class HappeningsViewModel : BaseViewModel, IAuthor
    {
        public long ID { get; set; }
        public int Type
        {
            get;
            set;
        }
        public virtual string Text
        {
            get
            {
                string text = string.Empty;
                if (Type == (int)Enums.NetworkActivityType.JoinedMyCompany)
                {
                    text = string.Format("has started working at {0} as {1} send a welcome.", CompanyName, JobTitle);
                }
                if (Type == (int)Enums.NetworkActivityType.JoinedCompany)
                {
                    text = string.Format("has started working at {0} as {1} wish a good luck.", CompanyName, JobTitle);
                }
                if (Type == (int)Enums.NetworkActivityType.AddedAwardWithMe)
                {
                    text = string.Format("has added an award with you '{0}' share the joy.", AwardName);
                }
                if (Type == (int)Enums.NetworkActivityType.AddedAward)
                {
                    text = string.Format("has added an award '{0}' greet a congratulate.", AwardName);
                }
                if (Type == (int)Enums.NetworkActivityType.WorkAnneversary)
                {
                    text = string.Format("celebrating {0} year(s) at {1} congratulate them", Years, CompanyName);
                }
                if (Type == (int)Enums.NetworkActivityType.AddedSkill)
                {
                    text = string.Format("added new skill {0} endorce them.", SkillName);
                }
                return text;
            }
        }
        public long? AwardID { get; set; }
        public long? CareerHistoryID { get; set; }
        public long? SkillID { get; set; }
        public long EntityID { get; set; }
        public string ProfileName { get; set; }
        public string Url { get; set; }
        public string FormatedName { get; set; }
        public string Name { get; set; }
        public string ProfilePath { get; set; }
        public string LogoPath { get; set; }
        public string Category { get; set; }
        public long? ForEntityID { get; set; }
        public string ProfileHeading { get; set; }
        public long TotalRows { get; set; }

        #region Author
        public int EntityType
        {
            get;
            set;
        }
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

        public string Token { get; set; }

        #region Abstract
        [ScriptIgnore]
        public string AwardName { get; set; }
        [ScriptIgnore]
        public string CompanyName { get; set; }
        [ScriptIgnore]
        public string JobTitle { get; set; }
        [ScriptIgnore]
        public string SkillName { get; set; }
        [ScriptIgnore]
        public long? Years { get; set; }
        #endregion
    }

    public class MyHappeningsViewModel : HappeningsViewModel
    {
        public override string Text
        {
            get
            {
                string text = string.Empty;
                if (Type == (int)Enums.NetworkActivityType.JoinedMyCompany)
                {
                    text = string.Format("has started working at {0} as {1}.", CompanyName, JobTitle);
                }
                if (Type == (int)Enums.NetworkActivityType.JoinedCompany)
                {
                    text = string.Format("has started working at {0} as {1}.", CompanyName, JobTitle);
                }
                if (Type == (int)Enums.NetworkActivityType.AddedAwardWithMe)
                {
                    text = string.Format("has added an award with you '{0}'.", AwardName);
                }
                if (Type == (int)Enums.NetworkActivityType.AddedAward)
                {
                    text = string.Format("has added an award '{0}'.", AwardName);
                }
                if (Type == (int)Enums.NetworkActivityType.WorkAnneversary)
                {
                    text = string.Format("celebrating {0} year(s) at {1}.", Years, CompanyName);
                }
                if (Type == (int)Enums.NetworkActivityType.AddedSkill)
                {
                    text = string.Format("added new skill {0}.", SkillName);
                }
                return text;
            }
        }
    }
}