using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class SkillViewModel : BaseViewModel
    {
        public System.Web.Mvc.SelectList MonthList { get; set; }

        public System.Web.Mvc.SelectList YearList { get; set; }

        public bool IsCurrent { get; set; }

        public long EntitySkillID { get; set; }

        public bool AllowEdit { get; set; }

        public int? StartFromMonth { get; set; }

        public int? EndFromMonth { get; set; }

        public string StartFromMonthName { get; set; }

        public string EndFromMonthName { get; set; }

        public string Name { get; set; }

        public long SkillID { get; set; }

        public byte? ExpertiseLevel { get; set; }

        public string ExpertiseLevelText
        {
            get
            {
                if (ExpertiseLevel > 0)
                {
                    return ((Enums.ExpertiseLevel)ExpertiseLevel).GetDescription();
                }
                return null;
            }
        }

        public int TotalEndorsements { get; set; }
        public int TotalRecommendations { get; set; }

        public SelectList ExpetiseLevels { get; set; }

        public long EntityID { get; set; }
    }

    public class AwardSkillViewModel : SkillViewModel
    {
        public long AwardSkillID { get; set; }
        public int TotalCount { get; set; }
    }

    public class CareerHistorySkillViewModel : SkillViewModel
    {
        public long ID { get; set; }
        public int TotalCount { get; set; }
    }

    public class AwardParticipantViewModel : PersonFacehead
    {
        public long ID { get; set; }

        public string Hash { get; set; }

        public bool AllowEdit { get; set; }

        public string Role { get; set; }

        public int Status { get; set; }

        public bool ShowApproval
        {
            get;
            set;
        }

        public long RefrenceEntity { get; set; }
    }

    public class PublicSkillViewModel : SkillViewModel
    {
        public bool Endorsed { get; set; }
        public bool Recommended { get; set; }

    
    }

    public class ListSkillViewModel : BaseViewModel
    {
        public string[] Skills { get; set; }
        public long EntityID { get; set; }
    }
}