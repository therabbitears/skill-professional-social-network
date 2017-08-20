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
        public string CommentText { get; set; }
        public string LikeText { get; set; }
        public long NotificationID { get; set; }
        public long AwardID { get; set; }
        public string PostTitle { get; set; }
        public long PostReplyCount { get; set; }
        public long PostLikesCount { get; set; }
        public long SelfLiked { get; set; }
        public string AwardName { get; set; }
        public long AppriciationCompletionID { get; set; }
        public string AppriciationCompletionName { get; set; }
        public long PostID { get; set; }
        public long ReferenceID { get; set; }
        public long EntitySkillID { get; set; }
        public string SkillName { get; set; }
        public long ActivityID { get; set; }
        public long CareerHistoryID { get; set; }
        public long JobTitleID { get; set; }
        public string JobTitleName { get; set; }
        public long OrganizationID { get; set; }
        public string OrganizationName { get; set; }
        public long ActivityCareerHistoryID { get; set; }
        public long ActivityJobTitleID { get; set; }
        public string ActivityJobTitleName { get; set; }
        public long ActivityOrganizationID { get; set; }
        public string ActivityOrganizationName { get; set; }
        public long Years { get; set; }
        [ScriptIgnore]
        public DateTime CreatedOn { get; set; }
        public long Type { get; set; }
        public byte? Status { get; set; }

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
                return CreatedOn.ToChatTime().ToString();
            }
        }

        private string ToText()
        {
            switch ((Enums.NotificationType)Type)
            {
                case Enums.NotificationType.PostLike:
                    if (SelfLiked > 0)
                    {
                        if (PostLikesCount > 3)
                        {
                            return string.Format("You {0} and {1} others liked your post.", LikeText, PostLikesCount - 3);
                        }
                        else if (PostLikesCount > 1)
                        {
                            return string.Format("You and {0} other liked your post.", PostLikesCount - 1);
                        }
                        else
                        {
                            return string.Format("You liked your post.", PostLikesCount - 1);
                        }
                    }
                    else
                    {
                        if (PostLikesCount > 2)
                        {
                            return string.Format("{0} and {1} others liked your post.", LikeText, PostLikesCount - 2);
                        }
                        else
                        {
                            return string.Format("{0} liked your post.", LikeText);
                        }
                    }
                case Enums.NotificationType.PostComment:
                    if (PostReplyCount > 1)
                    {
                        return string.Format("{0} and {1} others commented on your post.", CommentText, PostReplyCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} commented on your post.", CommentText, PostReplyCount - 1);
                    }
                case Enums.NotificationType.GroupInvitation:
                    if (TotalGroupInviteCounts > 1)
                    {
                        return string.Format("{0} and {1} others invited you to a group.", GroupInviteText, TotalGroupInviteCounts - 1);
                    }
                    else
                    {
                        return string.Format("{0} invited you to a group.", GroupInviteText, TotalGroupInviteCounts - 1);
                    }
                case Enums.NotificationType.OpportunityResponse:
                    if (PostResponseCount > 1)
                    {
                        return string.Format("{0} and {1} others responded on an opportunity you shared.", CommentText, PostReplyCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} responded on an opportunity you shared.", CommentText);
                    }
                case Enums.NotificationType.OpportunityApplied:
                    if (PostAppliedCount > 1)
                    {
                        return string.Format("{0} and {1} others applied on an opportunity you shared.", ApplyText, PostAppliedCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} applied on an opportunity you shared.", ApplyText);
                    }
                case Enums.NotificationType.OpportunityReference:
                    if (PostReferenceCount > 1)
                    {
                        return string.Format("{0} and {1} others referenced their connections for an opportunity you shared.", ReferText, PostReferenceCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} referenced their connections for an opportunity you shared.", ReferText);
                    }
                case Enums.NotificationType.ReferenceForAnOpportunity:
                    return string.Format("{0} asked for an opportunity for their connection.", ReferToReviewText);
                case Enums.NotificationType.ReferedForAnOpportunity:
                    if (MyOpportunityReferenceCount > 1)
                    {
                        return string.Format("{0} and {1} others referred you to a recruiter for an oppurtunity you posted.", ReferToReviewText, MyOpportunityReferenceCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} referred you to a recruiter for an oppurtunity you posted.", ReferToReviewText);
                    }
                case Enums.NotificationType.ReferenceOnAnOpportunity:
                    if (MyOpportunityReferenceCount > 1)
                    {
                        return string.Format("{0} and {1} others referred you to their connection for an opportunity you asked.", ReferToReviewText, MyOpportunityReferenceCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} referred you to their connection for an opportunity you asked.", ReferToReviewText);
                    }
                case Enums.NotificationType.Reshare:
                    if (PostReshareCount > 1)
                    {
                        return string.Format("{0} and {1} others shared your post.", ReshareText, PostReshareCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} shared your post.", ReshareText);
                    }
                case Enums.NotificationType.UserAsksForReccomend:
                    if (EntitySkillID > 0)
                    {
                        return string.Format("{0} asked for a recommendation for skill {1}", AuthorName, SkillName);
                    }
                    else if (CareerHistoryID > 0)
                    {
                        return string.Format("{0} asked for a recommendation for role {1} at {2}", AuthorName, JobTitleName, OrganizationName);
                    }
                    else
                    {
                        return string.Format("{0} asked for a recommendation.", AuthorName);
                    }
                case Enums.NotificationType.UserRecommendsMe:
                    if (EntitySkillID > 0)
                    {
                        return string.Format("{0} recommended you for skill {1}", AuthorName, SkillName);
                    }
                    else if (CareerHistoryID > 0)
                    {
                        return string.Format("{0} recommended you for role {1} at {2}", AuthorName, JobTitleName, OrganizationName);
                    }
                    else
                    {
                        return string.Format("{0} recommended you.", AuthorName);
                    }
                case Enums.NotificationType.UserAppriciatesMe:
                    if (AwardID > 0)
                    {
                        return string.Format("{0} appreciated you for {1}", AuthorName, AwardName);
                    }
                    else
                    {
                        return string.Format("{0} appreciated you.", AuthorName);
                    }
                case Enums.NotificationType.Endoreced:
                    if (SkillEndoresementCount > 3)
                    {
                        return string.Format("{0} and {1} others endorsed you for skill.", EndoresedText, SkillEndoresementCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} endorsed you for skill {1}.", EndoresedText, SkillName);
                    }
                case Enums.NotificationType.ProfileVisited:
                    if (TotalProfileVisitsCount > 1)
                    {
                        return string.Format("{0} and {1} others viewed your profile.", ProfileVisitText, TotalProfileVisitsCount - 1);
                    }
                    else
                    {
                        return string.Format("{0} viewed your profile.", ProfileVisitText);
                    }
                case Enums.NotificationType.AwardCongratulated:
                    if (AwardCongratulatedCount >= 3)
                    {
                        return string.Format("{0} and {1} others congratulated you for the award {2}.", AwardCongratulatedText, AwardCongratulatedCount - 2, AwardName);
                    }
                    else if (AwardCongratulatedCount == 2)
                    {
                        return string.Format("{0} and {1} congratulated you for the award {2}.", AwardCongratulatedText.Split(',')[0], AwardCongratulatedText.Split(',')[1], AwardName);
                    }
                    else
                    {
                        return string.Format("{0} congratulated you for the award {1}.", AwardCongratulatedText, AwardName);
                    }
                case Enums.NotificationType.WelcomedOnJoiningCompany:
                    return string.Format("{0} welcomed you for joining {1} as {2}.", AuthorName, ActivityOrganizationName, ActivityJobTitleName);
                case Enums.NotificationType.LikedAnActivity:
                    return string.Format("Your {0} connections liked an activity in your career.", ActivityLikedCount);
                case Enums.NotificationType.WellWishOnJoiningCompany:
                    return string.Format("Your {0} connections sent you well wishes for your new job {1} at {2}.", WellWishesCount, ActivityJobTitleName, ActivityOrganizationName);
                case Enums.NotificationType.CongratulateOnAnniversary:
                    return string.Format("Your {0} connections congratulated on your work anniversary at {1}.", AnniversaryCongratulatedCount, ActivityOrganizationName);
                default:
                    return "Failed to display text, might notification been deleted.";
            }
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

        [ScriptIgnore]
        public string AuthorPhoto
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

        public long EntityType { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string OrganizationUrl { get; set; }
        public string LogoPath { get; set; }

        // Author
        public string FormatedName { get; set; }
        [ScriptIgnore]
        public string ProfilePath { get; set; }
        public string ProfileIcon
        {
            get
            {
                return AuthorPhoto.ImagePath(AuthorPhoto, 50);
            }
        }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }


        public string EndoresedText { get; set; }
        public long SkillEndoresementCount { get; set; }

        public long AwardCongratulatedCount { get; set; }
        public string AwardCongratulatedText { get; set; }

        public string JobJoinedText { get; set; }

        public long ActivityLikedCount { get; set; }

        public long WellWishesCount { get; set; }

        public long AnniversaryCongratulatedCount { get; set; }

        public string Url { get; set; }

        public long PostReshareCount { get; set; }

        public string ReshareText { get; set; }

        public int PostResponseCount { get; set; }

        public int PostAppliedCount { get; set; }

        public int PostReferenceCount { get; set; }

        public string ApplyText { get; set; }

        public string ReferText { get; set; }

        public string ReferToReviewText { get; set; }

        public int MyOpportunityReferenceCount { get; set; }


        public int TotalProfileVisitsCount { get; set; }

        public string ProfileVisitText { get; set; }

        public int TotalGroupInviteCounts { get; set; }

        public string GroupInviteText { get; set; }
    }
}