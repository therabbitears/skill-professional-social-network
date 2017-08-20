using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class Enums
    {
        public enum SharingType
        {
            Share = 1,
            Reshare = 2,
            Commented = 3,
            Liked = 4,
            Trended = 5
        };
        public enum ShareType
        {
            News = 1,
            Post = 2
        }

        public enum EmployeeStrengths
        {
            [Description("1 - 10")]
            OneToTen,
            [Description("10 - 20")]
            TenToTwenty,
            [Description("20 - 50")]
            TwentyToFifty,
            [Description("50 - 100")]
            FiftyToHundred,
            [Description("100 - 1000")]
            HundredToThousand,
            [Description("1000 - 5000")]
            ThousandToFiveThousand,
            [Description("More than 5000")]
            MoreThanFiveThousand,
        }
        public enum UserAccountStatus
        {
            Registered

        }

        public enum EntityTypes
        {
            Person = 1,
            Organization = 2,
            Topic = 3,
            Group = 4
        }

        public enum TopicType
        {
            Topic = 1,
            Tag = 2
        }

        public enum EmailType
        {
            Primary = 1,

        }

        public enum PhoneType
        {
            Primary = 1,

        }

        public enum OrganizationSaveStatus
        {
            Success,
            AlreadyExist,
            Error
        }

        public enum MediaType
        {
            Image
        }

        public enum MediaStatus
        {
            Active
        }

        public enum Months
        {
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            Saptember = 9,
            October = 10,
            November = 11,
            December = 12
        }

        public enum CareerHistoryMode
        {
            Profession = 1,
            Education = 2,
            Affiliation = 3,
        }

        public enum AwardAndAssignmentMode
        {
            Award = 1,
            Assignment = 2,
            Publication = 3,
            Composition = 4,
            Research = 5,
            Finding = 6,
            Services = 7,
            Products = 8
        }

        public enum ReferenceMode
        {
            Appreciation = 1,
            Recommendation = 2
        }

        public enum AssociationRequestDirection
        {
            Received = 1,
            Sent = 2
        }

        public enum AssociationRequestStatus
        {
            Pending = 1,
            Approve = 2,
            Rejected = 3,
            Removed = 4,
            Skiped = 5,
            Canceled = 6
        }



        public enum AssociationType
        {
            NetworkConnection = 1,
            Follow = 2,
            TopicFollow = 3,
            Block = 4,
            GroupOwner
        }

        public enum ExpertiseLevel
        {
            Beginner = 1,
            [Description("Intermediate - Lower")]
            IntermediateLower = 2,
            [Description("Intermediate - Upper")]
            IntermidiateUppper = 3,
            [Description("Advanced - Lower")]
            AdvancedLower = 4,
            [Description("Advanced - Upper")]
            AdvancedUpper = 5
        }

        public enum InteligenceType
        {
            RequireAddingSkills = 1,
            RequireAddingWorkInfo = 2,
            RequireEducation = 3,
            RequireConnections = 4,
            RequireAssignments = 5,
            RequirePhoto = 6,
            RequireAbout = 7,
            RequireAddingCareerLineCompany = 8,
            RequireAddingTimeToCareerLine = 9,
            RequireAddingAwardAndAchievement = 10,
            RequireTeamIntoAward = 11,
            RequireAddingCertification = 12,
            RequireTeamIntoAssignment = 13,
            ContactImport = 14
        }

        public enum TableNameIndex
        {
            EntityProfile = 1,

        }

        public enum ColumnNameIndex
        {
            ProfileName = 1
        }

        public enum SettingType
        {
            JobSearch = 1,
            General = 2,
            Network = 3,
            Privacy = 4,
            Widget = 5,
            All
        }

        public enum OppurtunityLevel
        {
            [Description("Better opportunity(Not actively looking)")]
            Medium = 1,
            [Description("Actively looking")]
            Active = 2
        }

        public enum NetworkCoverageLevel
        {
            [Description("Allow similar skill")]
            ToSkill = 1,
            [Description("Allow similar job title")]
            ToJobFunction = 2,
            [Description("Allow specific industry")]
            ToIndustry = 3,
            [Description("Best matching (default)")]
            BestMatch = 4
        }

        public enum BusinessNetworkCoverageLevel
        {
            [Description("Allow specific industry")]
            ToIndustry = 3,
            [Description("Best matching (default)")]
            BestMatch = 4
        }

        public enum RequestSenderCapabilities
        {
            [Description("Allow similar skill profiles")]
            ToSkill = 1,
            [Description("Allow similar job title")]
            ToJbFunction = 2,
            [Description("Allow similar industry or equaivalent")]
            ToIndustry = 3,
            [Description("Limit to mutual profiles")]
            MutualAny = 4,
            [Description("Allow Anyone")]
            Anyone = 5,
            DisallowRequest = 6,
        }

        public enum PublicationType
        {
            Novel,
            Book,
            Blog,
            Article
        }

        public enum CompositionType
        {
            Video,
            Song,
            Tune,
            Lyrics,
            Movie
        }

        public enum ResearchType
        {
            Science,
            Technology,
            Market,
            Social,
            Phsycological,
            Philosophical,
            Other
        }

        public enum FindingType
        {
            Science = 1,
            Technology = 2,
            Market = 3,
            Arts = 4,
            Other = 5
        }

        public enum EducationType
        {
            Course = 1,
            Certification = 2
        }

        public enum InteligenceAction
        {
            Skip = 1
        }

        public enum AccomplishmentParticipantStatus
        {
            [Description("Waiting approval")]
            PendingForApproval = 1,
            Active = 2,
            [Description("Removed by user")]
            RemovedBySelf = 3,
            [Description("Removed by owner")]
            RemoveByOwner = 4
        }

        public enum AccomplishmentStatus
        {
            [Description("Waiting approval")]
            PendingForApproval,
            Active,
            [Description("Removed by user")]
            RemovedBySelf,
            [Description("Removed by owner")]
            RemoveByOwner
        }

        public enum AccomplishmentStateType
        {
            Report = 1,
            Congratulate = 2,
        }

        public enum AccomplishmentStateSubType
        {
            General = 1
        }

        public enum AccomplishmentStateStatus
        {
            PendingForReview = 0,
            Active = 1
        }

        public enum ResultType
        {
            Error = 1,
            Success = 2,
            Warning = 3
        }

        public enum SkillStateType
        {
            Endorcement = 1
        }

        public enum SkillStateSubType
        {
            General = 1
        }

        public enum SkillStateStatus
        {
            Active = 1,
            Reverted = 2
        }

        public enum PostTypes
        {
            PressRelease = 1,
            Blog = 2,
            Questions = 3,
            UserPost = 4,
            Page = 5,
            AskOpportunity = 6,
            ShareOpportunity = 7,
            Discussion = 8
        }



        public enum PostInteractionType
        {
            Like = 1,
            Unlike = 2,
            Save = 3,
            RemoveFavorite = 4,
            Report = 5,
            Remove = 6,
            UnRemove = 7,
            Hide = 8,
            Unhide = 9,
            Apply = 10,
            Refer = 11,
            ReferForOpportunity = 12,
            View = 13
        }

        public enum ReplyType
        {
            CommentOnPost = 1,
            ResponseOnOpportunity = 2
        }

        public enum FileType
        {
            Image = 1,
            Document = 1,
            Pdf = 3
        }

        public enum PersonNameFormat
        {
            [Description("[first] [last]")]
            FirstNameLastName = 1,
            [Description("[last] [first]")]
            LastNameFirstName = 2
        }

        public enum WizardStep
        {
            AddCareerHistory = 1,
            AddSkills = 2,
            VarifyEmail = 3,
            AddConnections = 4,
            Invalid = 5,
            Feedback = 6,
            ForgottenPassword = 7
        }
        public enum CareerStage
        {
            Employement = 1,
            Student = 2,
            PostEmployement = 3,
            Retired = 4,
            None
        }

        public enum EmployementEndedStage
        {
            OnABreak = 1,
            Retired = 2,
            LookingOpportunity = 3,
        }

        public enum OrganizationType
        {
            Company = 1,
            University = 2,
            Group = 3
        }

        public enum JobtTitleType
        {
            Professional = 1,
            Educational = 2
        }

        public enum StaticCategories
        {
            EducationAndCertificationProvider = 580,
            Media = 389
        }

        public enum EmailFilePaths
        {
            [Description("~/Content/emailhelper/XmlData/Verification.xml")]
            Varification,
            [Description("~/Content/emailhelper/XmlData/Welcome.xml")]
            WelcomeEmail,
            [Description("~/Content/emailhelper/XmlData/Invite.xml")]
            InviteEmail,
            [Description("~/Content/emailhelper/XmlData/Feedback.xml")]
            Feedback,
            [Description("~/Content/emailhelper/XmlData/FeedbackThank.xml")]
            FeedbackThank,
            [Description("~/Content/emailhelper/XmlData/Forgot_Password.xml")]
            ForgottenPassword,
            [Description("~/Content/emailhelper/XmlData/Abuse.xml")]
            Abuse,
            [Description("~/Content/emailhelper/XmlData/AbuseFeedback.xml")]
            AbuseAknowledgement,
            [Description("~/Content/emailhelper/XmlData/AskOpportunity.xml")]
            AskingAnOpportunity,
            [Description("~/Content/emailhelper/XmlData/AppliedAnOpportunity.xml")]
            AppliedAnOpportunity,
            [Description("~/Content/emailhelper/XmlData/ReferedOnThierOpportunity.xml")]
            ReferedOnThierOpportunity,
            [Description("~/Content/emailhelper/XmlData/GroupInvitation.xml")]
            GroupInvitation
        }

        public enum EmailTypes
        {
            [Description("Email address verification")]
            Varification = 1,
            [Description("Welcome email")]
            WelcomeEmail = 2,
            [Description("Invite email")]
            InviteEmail = 3,
            [Description("Feedback")]
            Feedback = 4,
            [Description("Feedback thank")]
            FeedbackThank = 5,
            [Description("Forgotten password")]
            ForgottenPassword = 6,
            [Description("Abuse")]
            Abuse = 7,
            [Description("Abuse acknowledgement")]
            AbuseAknowledgement = 8,
            [Description("Asking an opportunity")]
            AskingAnOpportunity = 9,
            [Description("Applied for an opportunity")]
            AppliedAnOpportunity = 10,
            [Description("Referred on an opportunity")]
            ReferedOnThierOpportunity = 11
        }

        public enum VarificationResult
        {
            Varified,
            AlreadyVarified,
            Error
        }

        public enum VarificationStatus
        {
            AlreadyVarified,
            InvalidLink
        }

        public enum AppriciationAndRecommedationStatus
        {
            /// <summary>
            /// Request been sent from the professional to another person.
            /// </summary>
            Requested = 1,
            /// <summary>
            /// Request been sent and the another employee is waiting for writting.
            /// </summary>
            PendingToAction = 2,
            /// <summary>
            /// Approved by the user to show on his profile.
            /// </summary>
            Approved = 3,
            /// <summary>
            /// Rejected by user to show on screen.
            /// </summary>
            Rejected = 4,
            /// <summary>
            /// Deleted after approving the reference.
            /// </summary>
            Removed = 5,

            /// <summary>
            /// Canceled by sender
            /// </summary>
            Cancel = 6


        }

        /// <summary>
        /// Enum represent the notification type
        /// </summary>
        public enum NotificationType
        {
            PostLike = 1,
            PostComment = 2,
            Reshare = 3,
            UserAsksForReccomend = 4,
            // PostTaggedInLike = 5,
            // PostTaggedInComment = 6,
            UserRecommendsMe = 7,
            UserAppriciatesMe = 8,
            Endoreced = 9,
            AwardCongratulated = 10,
            WelcomedOnJoiningCompany = 11,
            LikedAnActivity = 12,
            WellWishOnJoiningCompany = 13,
            CongratulateOnAnniversary = 14,
            OpportunityResponse = 15,
            OpportunityApplied = 16,
            OpportunityReference = 17,
            ReferenceForAnOpportunity = 18,
            ReferedForAnOpportunity = 19,
            ReferenceOnAnOpportunity = 20,
            ProfileVisited = 21,
            GroupInvitation = 22
        }

        public enum NotificationStatus
        {
            Pending = 1,

        }

        public enum MessageType
        {
            Text = 1,
            Html = 2
        }
        public enum MessageReadingStatus
        {
            Pending = 1,
            ReadByReceiver
        }

        public enum MessageCallbackFor
        {
            ReceivedToServer = 1
        }

        public enum ImageObject
        {
            News = 1,
            UserProfileImage = 2,
            UserCoverImage = 3,
            BusinessProfileImage = 4,
            BusinessCoverImage = 5,
            GroupImage = 6
        }

        public enum PostStatus
        {
            Drafted = 1,
            Published = 2,
            Removed = 3,
            Hidden = 4
        }

        //public enum AssociationAlgorithm
        //{

        //    ScoreBased = 1,
        //    SimilarRole = 2,
        //    BestMatches = 3
        //}

        public enum AddressType
        {
            Primary = 1
        }

        public enum NetworkActivityType
        {
            JoinedMyCompany = 1,
            JoinedCompany = 2,
            AddedAwardWithMe = 3,
            AddedAward = 4,
            WorkAnneversary = 5,
            AddedSkill = 6
        }


        public enum NetworkActivityAction
        {
            Welcomed = 1,
            Liked = 2,
            SentGoodluck = 3,
            Skipped = 4,
            Congratulated = 5,
            Endorsed = 6
        }

        public enum RecommedationFor
        {
            Skill = 1,
            Role = 2,
            None = 3
        }

        public enum BasicWidgets
        {
            Publications = 1,
            Compositions = 2,
            Projects = 3,
            CareerHistory = 4,
            Skill = 5,
            Research = 6,
            Education = 7,
            Awards = 8,
            Findings = 9,
            Volunteership = 10,
            Participations = 11,
            Nominations = 12
        }

        public enum ImportType
        {
            Google = 1,
            Manual = 2
        }


        public enum InviteType
        {
            Email = 1,
            Sms = 2
        }

        public enum ProfileMode
        {
            Default = 0,
            Connections,
            Feeds,
            Followers,
            Following
        }

        public enum FeedType : int
        {
            Default = 0,
            MyUpdates = 1,
            Saved = 2
        }

        public enum StorageProvider
        {
            Local = 1,
            Amazon = 2
        }
        public enum SocialSharingContentType
        {
            Opportunity = 1

        }

        public enum EntityStateType
        {
            ProfileView = 1,
            InviteForGroup
        }

        public enum GroupType
        {
            [Description("Job & opportunities")]
            JobAndOpportunities = 1,
            [Description("Startups & entrepreneurs")]
            StartupAndEntrepreneurs = 2,
            [Description("Discussions")]
            Discussions = 3,
            [Description("Professionals")]
            Professionals = 4
        }

        public enum GroupMode
        {
            Requests,
            Members,
            General
        }

        public enum Status
        {
            Active = 1,
            InActive = 0
        }
    }
}
