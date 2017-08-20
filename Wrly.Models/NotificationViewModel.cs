using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Types;

namespace Wrly.Models
{
    public class NotificationViewModel
    {
        public long NotificationID { get; set; }
        public long ReferenceID { get; set; }
        public long EntitySkillID { get; set; }
        [ScriptIgnore]
        public DateTime Stamp { get; set; }
        public long Type { get; set; }
        public byte? Status { get; set; }
        public long EntityType { get; set; }
        [ScriptIgnore]
        public string AuthorPhoto
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfileImage))
                {
                    return ProfileImage;
                }
                if (EntityType == (byte)Enums.EntityTypes.Person)
                    return "/content/images/no-image.png";
                if (EntityType == (byte)Enums.EntityTypes.Organization)
                    return "/content/images/o/no-image.png";
                return "/content/images/no-image.png";
            }
        }
        public string AdditionalText { get; set; }
        public long Total { get; set; }

        public string Text
        {
            get
            {
                return ToText();
            }
        }

        public string CreatedOnText
        {
            get
            {
                return Stamp.ToChatTime().ToString();
            }
        }

        private string ToText()
        {
            switch ((Enums.NotificationType)Type)
            {
                case Enums.NotificationType.PostLike:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others liked your post.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} liked your post.", Name);
                    }
                case Enums.NotificationType.PostComment:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others commented on your post.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} commented on your post.", Name);
                    }
                case Enums.NotificationType.GroupInvitation:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others invited you to a group.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} invited you to a group.", Name);
                    }
                case Enums.NotificationType.OpportunityResponse:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others responded on an opportunity you shared.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} responded on an opportunity you shared.", Name);
                    }
                case Enums.NotificationType.OpportunityApplied:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others applied on an opportunity you shared.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} applied on an opportunity you shared.", Name);
                    }
                case Enums.NotificationType.OpportunityReference:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others referenced their connections for an opportunity you shared.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} referenced their connections for an opportunity you shared.", Name);
                    }
                case Enums.NotificationType.ReferenceForAnOpportunity:
                    return string.Format("{0} asked for an opportunity for their connection.", Name);
                case Enums.NotificationType.ReferedForAnOpportunity:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others referred you to a recruiter for an opportunity you posted.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} referred you to a recruiter for an opportunity you posted.", Name);
                    }
                case Enums.NotificationType.ReferenceOnAnOpportunity:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others referred you to their connection for an opportunity you asked.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} referred you to their connection for an opportunity you asked.", Name);
                    }
                case Enums.NotificationType.Reshare:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others shared your post.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} shared your post.", Name);
                    }
                case Enums.NotificationType.UserAsksForReccomend:
                    if (EntitySkillID > 0)
                        return string.Format("{0} asked for a recommendation for skill \"{1}\".", Name, AdditionalText);
                    else if (CareerHistoryID > 0)
                        return string.Format("{0} asked for a recommendation for role \"{1}\".", Name, AdditionalText);
                    else
                    {
                        return string.Format("{0} asked for a recommendation.", Name);
                    }
                case Enums.NotificationType.UserRecommendsMe:
                    if (EntitySkillID > 0)
                    {
                        return string.Format("{0} recommended you for skill \"{1}\".", Name, AdditionalText);
                    }
                    else if (CareerHistoryID > 0)
                    {
                        return string.Format("{0} recommended you for role \"{1}\".", Name, AdditionalText);
                    }
                    else
                    {
                        return string.Format("{0} recommended you.", Name);
                    }
                case Enums.NotificationType.UserAppriciatesMe:
                    if (AwardID > 0)
                    {
                        return string.Format("{0} appreciated you for \"{1}\".", Name, AdditionalText);
                    }
                    else
                    {
                        return string.Format("{0} appreciated you.", AuthorName);
                    }
                case Enums.NotificationType.Endoreced:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others endorsed you for skill \"{2}\".", Name, Total - 1, AdditionalText);
                    }
                    else
                    {
                        return string.Format("{0} endorsed you for skill \"{1}\".", Name, AdditionalText);
                    }
                case Enums.NotificationType.ProfileVisited:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others viewed your profile.", Name, Total - 1);
                    }
                    else
                    {
                        return string.Format("{0} viewed your profile.", Name);
                    }
                case Enums.NotificationType.AwardCongratulated:
                    if (Total > 1)
                    {
                        return string.Format("{0} and {1} others congratulated you for the award \"{2}\".", Name, Total - 1, AdditionalText);
                    }
                    else
                    {
                        return string.Format("{0} congratulated you for the award \"{1}\".", Name, AdditionalText);
                    }
                case Enums.NotificationType.WelcomedOnJoiningCompany:
                    return string.Format("{0} welcomed you for joining {1} as {2}.", Name, AdditionalText);
                case Enums.NotificationType.LikedAnActivity:
                    return string.Format("Your {0} connections liked an activity in your career.", Total);
                case Enums.NotificationType.WellWishOnJoiningCompany:
                    return string.Format("Your {0} connections sent you well wishes for your new job {1} at {2}.", Total, AdditionalText);
                case Enums.NotificationType.CongratulateOnAnniversary:
                    return string.Format("Your {0} connections congratulated on your work anniversary at {1}.", Total, AdditionalText);
                default:
                    return "Failed to display text, might notification been deleted.";
            }
        }


        public string AuthorName
        {
            get
            {
                return Name;
            }
        }

        public string Name { get; set; }

        [ScriptIgnore]
        public string ProfileImage { get; set; }
        public string ProfileIcon
        {
            get
            {
                return AuthorPhoto.ImagePath(AuthorPhoto, 50);
            }
        }
        public string ProfilePath { get; set; }
        public string Heading { get; set; }

        public long CareerHistoryID { get; set; }
        public long AwardID { get; set; }

        public string Url { get; set; }
    }
}