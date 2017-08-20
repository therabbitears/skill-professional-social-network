using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class EntityProfileViewModel : BaseViewModel
    {
        public long EntityID { get; set; }
        public int EntityType { get; set; }
        public string FormatedName { get; set; }
        public string ProfilePath { get; set; }
        public string ProfileName { get; set; }
        public string ProfileHeading { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }
        public string LogoPath { get; set; }
        public int CommonSkillCount { get; set; }
        public int CommonPeopleCount { get; set; }
        public string PreviousJobTitle { get; set; }
        public string EmailAddress { get; set; }
    }

    public class ProfileViewModel : BaseViewModel
    {
        public bool IsConnected { get; set; }
        public int TotalTags { get; set; }
        public long ID { get; set; }
        public string UserID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        string _FullName;
        public string FullName
        {
            get
            {
                if (!string.IsNullOrEmpty(LastName))
                {
                    _FullName = FirstName + " " + LastName;
                }
                else
                {
                    _FullName = FirstName;
                }
                return _FullName;
            }
            set { _FullName = value; }
        }
        public DateTime DateOfBirth { get; set; }
        public int Gender { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }
        public string Mobile { get; set; }
        public string ProfilePic { get; set; }
        public long TotalConnections { get; set; }
        public long TotalQuestions { get; set; }
        public long TotalAnswers { get; set; }
        public int TotalFollowers { get; set; }
        public string ProfileName { get; set; }
        public int TotalBadges { get; set; }
        public List<AssociateProfileViewModel> Followers { get; set; }
        public string UserName { get; set; }
        public bool AllowEdit { get; set; }
        public int NameFormat { get; set; }
        public string FormatedName { get; set; }
        public string FormatedAddress { get; set; }
        public string FormatedStudyTitle { get; set; }
        public string FormatedJobTitle { get; set; }
        public string PreviousJobTitle { get; set; }
        public long ProfileIndustry { get; set; }
        public int ProfileLevel { get; set; }
        public string ProfileImagePath { get; set; }
        public string ProfileImageIconPath
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfileImagePath))
                {
                    return ProfileImagePath.ImagePath(ProfileImagePath, 50);
                }
                return null;
            }
        }

        public DateTime LastVarificationSent { get; set; }
        public bool EmailVarified { get; set; }
        public int WizardStep { get; set; }

        public string ProfileSummary { get; set; }

        public string FormatedProfileSummary
        {
            get
            {
                if (!string.IsNullOrEmpty(ProfileSummary))
                {
                    if (ProfileSummary.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length > 1)
                    {
                        return string.Format("<ul class='career-history-details'>{0}</ul>", string.Join(Environment.NewLine, ProfileSummary.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select(x => string.Format("<li>{0}</li>", x)).ToList()));
                    }
                }
                return ProfileSummary;
            }
        }

        public string ProfileCoverPath { get; set; }



        public string ProfileHash { get; set; }

        public long EntityID { get; set; }

        public long PersonID { get; set; }

        public string SkillHead { get; set; }

        public string SkillHeadWithTwoSkills
        {
            get
            {
                if (!string.IsNullOrEmpty(SkillHead))
                {
                    if (SkillHead.Split(',').Length > 2)
                    {
                        return string.Format("Knows {0} and {1} more skills", string.Join(",", SkillHead.Split(',').Take(2)), SkillHead.Split(',').Length - 2);
                    }
                    return string.Format("Knows {0}", string.Join(",", SkillHead.Split(',').Take(2)));
                }
                return string.Empty;
            }
        }

        public Feeds.HomeFeedViewModel Feed { get; set; }

        public string NetworkHash { get; set; }

        public List<WidgetSettingViewModel> Widgets { get; set; }

        public List<AssociateProfileViewModel> Connections { get; set; }

        public PersonStatisticsViewModel Statistics { get; set; }

        public Types.Enums.ProfileMode ProfileMode { get; set; }

        public decimal ProfileScore { get; set; }
    }


    public class AuthorViewModel : IAuthor
    {

        #region Author
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string Category { get; set; }
        public string Url { get; set; }

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
                    if (!string.IsNullOrEmpty(ProfilePath) || !string.IsNullOrEmpty(ProfileImagePath))
                    {
                        return ProfilePath ?? ProfileImagePath;
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


        public string ProfileHeading
        {
            get;
            set;
        }

        public string FormatedName { get; set; }

        public string ProfilePath { get; set; }

        public string ProfileImagePath { get; set; }

        public string ProfileName { get; set; }

        public string UserID { get; set; }
        public string UserName { get; set; }
        public long EntityID { get; set; }
    }

    public class PersonStatisticsViewModel
    {
        public int CareerHistoryCount { get; set; }
        public int EducationCount { get; set; }
        public int CertificationCount { get; set; }
        public int SkillCount { get; set; }
        public int AppriciationCount { get; set; }
        public int RecommendationCount { get; set; }
        public int PublicationsCount { get; set; }
        public int CompositionsCount { get; set; }
        public int ProjectsCount { get; set; }
        public int ResearchCount { get; set; }
        public int FindingsCount { get; set; }
        public int AwardCount { get; set; }
    }

    public class BusinessStatisticsViewModel
    {
        public int ServicesCount { get; set; }
        public int ProductsCount { get; set; }
        public int AffiliationCount { get; set; }
        public int AppriciationCount { get; set; }
        public int RecommendationCount { get; set; }
        public int AwardCount { get; set; }
    }
}