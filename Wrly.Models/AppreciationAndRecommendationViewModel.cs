using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class MasterAppreciationAndRecommendationViewModel : BaseViewModel
    {
        public long ReferenceID { get; set; }
        public long EntityID { get; set; }
        public string Title { get; set; }
        [Required]
        [StringLength(700, ErrorMessage = "Cannot be more than 700 characters.")]
        public string Description { get; set; }
        public byte? Status { get; set; }
        public byte? Type { get; set; }

        public long? AwardID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public int? SkillID { get; set; }
        public string SkillName { get; set; }

        public int EntityType { get; set; }

        public long? CareerHistoryID { get; set; }
        public string JobTitleName { get; set; }
        public string OrganizationName { get; set; }

        public int For { get; set; }


        public string AuthorName
        {
            get
            {
                if (EntityType == (int)Enums.EntityTypes.Person)
                {
                    return FormatedName;
                }
                return Name;
            }
        }
        public string AuthorHeading
        {
            get
            {
                if (EntityType == (int)Enums.EntityTypes.Person)
                {
                    return ProfileHeading;
                }
                return Category;
            }
        }

        public string AuthorImage
        {
            get
            {
                if (EntityType == (int)Enums.EntityTypes.Person)
                {
                    return ProfilePath;
                }
                return LogoPath;
            }
        }


        public string AuthorIcon
        {
            get
            {
                if (!string.IsNullOrEmpty(AuthorImage))
                {
                    return AuthorImage.ImagePath(AuthorImage, 50);
                }
                return null;
            }
        }

        public string AuthorProfilePath
        {
            get
            {
                if (EntityType == (int)Enums.EntityTypes.Person)
                {
                    return ProfileName;
                }
                return string.Format("fou/{0}", Url);
            }
        }

        // Person
        [ScriptIgnore]
        public string FormatedName { get; set; }
        [ScriptIgnore]
        public string ProfileHeading { get; set; }
        [ScriptIgnore]
        public string ProfileName { get; set; }
        [ScriptIgnore]
        public string ProfilePath { get; set; }
        [ScriptIgnore]
        public int CommonSkillCount { get; set; }

        // Organization
        [ScriptIgnore]
        public string Name { get; set; }
        [ScriptIgnore]
        public string Category { get; set; }
        [ScriptIgnore]
        public string LogoPath { get; set; }
        [ScriptIgnore]
        public string Url { get; set; }

    }
    public class AppreciationAndRecommendationViewModel : MasterAppreciationAndRecommendationViewModel
    {
        public System.Web.Mvc.SelectList Project { get; set; }
        public SelectList CareerHistoryList { get; set; }
        public SelectList Skills { get; set; }

        public List<AwardViewModel> ServicesAndProducts { get; set; }
        public long? ParentID { get; set; }
        public string RecomedationRelation { get; set; }
    }

    public class AskRecommendationViewModel : AppreciationAndRecommendationViewModel
    {
        public string[] Providers { get; set; }
        public string RecommedationType { get; set; }
    }

    public class PublicAppreciationAndRecommendationViewModel : MasterAppreciationAndRecommendationViewModel
    {

    }

    public class AccompishmentAppriciation : AppreciationAndRecommendationViewModel
    {
        public AwardViewModel Accomplishment { get; set; }
    }

    public class CareerHistoryReferenceViewModel : AppreciationAndRecommendationViewModel
    {
        public CareerHistoryViewModel CareerHistory { get; set; }
    }
    public class SkillReferenceViewModel : AppreciationAndRecommendationViewModel
    {
        public SkillViewModel Skill { get; set; }
    }

    public class ReferenceViewModel
    {
        public List<PublicAppreciationAndRecommendationViewModel> References { get; set; }

        public int TotalForSkill { get { return SkillReferences != null ? SkillReferences.Count : 0; } }
        public int TotalForRole { get { return RoleReferences != null ? RoleReferences.Count : 0; } }
        public int TotalGeneral { get { return GeneralReferences != null ? GeneralReferences.Count : 0; } }
        public int TotalForAccomplishments { get { return AccomplishmentReferences != null ? AccomplishmentReferences.Count : 0; } }

        public List<PublicAppreciationAndRecommendationViewModel> SkillReferences
        {
            get
            {
                return References != null ? References.Where(c => c.SkillID > 0).ToList() : null;
            }
        }
        public List<PublicAppreciationAndRecommendationViewModel> RoleReferences
        {
            get
            {
                return References != null ? References.Where(c => c.CareerHistoryID > 0).ToList() : null;
            }
        }
        public List<PublicAppreciationAndRecommendationViewModel> GeneralReferences
        {
            get
            {
                return References != null ? References.Where(c => (c.CareerHistoryID == null || c.CareerHistoryID <= 0) && (c.SkillID == null || c.SkillID <= 0)).ToList() : null;
            }
        }

        public List<PublicAppreciationAndRecommendationViewModel> AccomplishmentReferences
        {
            get
            {
                return References != null ? References.Where(c => c.AwardID > 0).ToList() : null;
            }
        }
    }

    public class ReferenceRequestViewModel : NewsAndReplyAuthorInfo
    {
        public long ReferenceID { get; set; }
        public string Title { get; set; }
        [Required]
        [StringLength(700, ErrorMessage = "Cannot be more than 700 characters.")]
        public string Description { get; set; }
        public byte? Status { get; set; }
        public byte? Type { get; set; }

        public long? AwardID { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescription { get; set; }

        public int? SkillID { get; set; }
        public string SkillName { get; set; }

        public long? CareerHistoryID { get; set; }
        public string JobTitleName { get; set; }
        public string OrganizationName { get; set; }

        public int CommonSkillCount { get; set; }

    }
}