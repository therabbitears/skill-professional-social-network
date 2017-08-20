using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;
using Types;

namespace Wrly.Models.Listing
{
    public class AssociateProfileActionViewModel
    {
        public int ConnectionStatus { get; set; }
        public int BlockStatus { get; set; }
        public int FollowStatus { get; set; }
        public bool Blocked { get; set; }
        public bool AllowUnblock { get; set; }
        public bool AllowUnFollow { get; set; }
        public bool AllowDisconnect { get; set; }
        public bool AllowApproveRequest { get; set; }
        public bool AllowCancelRequest { get; set; }

        public object NetworkHash { get; set; }

        public bool AllowToNewRequest
        {
            get
            {
                if (ConnectionStatus == 0 && !AllowUnblock)
                {
                    return true;
                }
                return false;
            }
        }
    }

    public class AssociateProfileViewModel : BaseViewModel, IAuthor
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FormatedJobTitle { get; set; }
        public string ProfileHeading { get; set; }
        public string FormatedStudy { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string JobTitle { get; set; }
        public string FormatedName { get; set; }
        public string OrganizationName { get; set; }
        public string OrganizationProfileName { get; set; }
        public string CommonSkill { get; set; }
        public int CommonSkillCount { get; set; }
        public long EntityID { get; set; }
        public long EntityID2 { get; set; }
        public string Hash { get; set; }
        public string Token { get; set; }
        public long AssociationID { get; set; }
        public bool IsConnected { get; set; }
        public bool IsFollowed { get; set; }
        public string CommonSkillText
        {
            get
            {
                if (!string.IsNullOrEmpty(CommonSkill) && CommonSkillCount >= 1)
                {
                    return string.Format("{0} and {1} skill(s) in common", CommonSkill, CommonSkillCount);
                }
                if (!string.IsNullOrEmpty(CommonSkill))
                {
                    return string.Format("{0} is in common", CommonSkill);
                }
                return string.Empty;
            }
        }

        public string Name { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string LogoPath { get; set; }

        public bool RequirePermission { get; set; }

        #region IAuthor
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

        public string ProfileHeadline
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

        public string ProfilePhotoIcon
        {

            get
            {
                return ProfilePhotoUrl.ImagePath(ProfilePhotoUrl, 50);
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
                    return string.Format("{0}", ProfileName.ToLower());
                }
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                {
                    return string.Format("fou/{0}", Url);
                }
                return null;
            }
        }
        #endregion
    }

    public class ActionAssociateProfileViewModel : AssociateProfileViewModel
    {
        [JsonIgnore]
        public long AssociationID { get; set; }

        [JsonIgnore]
        public string Action { get; set; }

        public DateTime SentOn { get; set; }

        public string Mode { get; set; }
    }

    public class InviteAssociateProfileViewModel : AssociateProfileViewModel
    {
        public bool Invited { get; set; }
    }


    public interface IAuthor
    {
        int EntityType { get; set; }
        string AuthorName { get; }
        string ProfileHeading { get; }
        string ProfilePhotoUrl { get; }
        string ProfileUrl { get; }
    }
}