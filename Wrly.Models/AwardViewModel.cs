using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Types;
using Wrly.Infrastructure.Extended;

namespace Wrly.Models
{
    public class IntelligenceAwardViewModel : AwardViewModel
    {
        public string DisplayText { get; set; }
    }

    public class ParentAwardViewModel : BaseViewModel
    {
        public long AwardID { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }
        public long? CareerHistoryID { get; set; }
        public long? JobTitleID { get; set; }
        public string JobTitle { get; set; }
        public byte? JobTitleType { get; set; }
        public string OrganizationName { get; set; }
        public string DisplayJobTitleText { get; set; }
        public byte? Type { get; set; }
        public List<AwardSkillViewModel> SkillIncluded { get; set; }
        public List<AwardParticipantViewModel> ParticipantIncluded { get; set; }
        [Newtonsoft.Json.JsonIgnore]
        public long EntityID { get; set; }
        [Required]
        public string Role { get; set; }
        public int? StartFromDay { get; set; }

        [CannotGreaterMonthAndYear("StartFromYear", "EndFromMonth", "EndFromYear", true, true, false, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [RequeiredIfSelected("StartFromYear", Mode = Types.Enums.CareerStage.None, ErrorMessage = "Month needs to be selected in case year is selected")]
        public int? StartFromMonth { get; set; }

        [CannotGreaterMonthAndYear("StartFromMonth", "EndFromMonth", "EndFromYear", true, false, true, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [RequeiredIfSelected("StartFromMonth", Mode = Types.Enums.CareerStage.None, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public int? StartFromYear { get; set; }
        public int TotalCongrates { get; set; }
        
        public int? EndFromDay { get; set; }
        [CannotGreaterMonthAndYear("EndFromYear", "StartFromMonth", "StartFromYear", false, true, false, ErrorMessage = "Year and month combination must be greater than what selected as starting combination.")]
        [RequeiredIfSelected("EndFromYear", Mode = Types.Enums.CareerStage.None, ErrorMessage = "Month needs to be selected in case year is selected")]
        public int? EndFromMonth { get; set; }
        [CannotGreaterMonthAndYear("EndFromMonth", "StartFromMonth", "StartFromYear", false, false, true, ErrorMessage = "Year must be greater than what selected as starting year.")]
        [RequeiredIfSelected("EndFromMonth", Mode = Types.Enums.CareerStage.None, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public int? EndFromYear { get; set; }
        public long[] Participants { get; set; }
        public int TotalAppriciations { get; set; }
        [Url(ErrorMessage="Invalid url")]
        public string Url { get; set; }
        public string SubType { get; set; }

        #region getters
        public bool HasAwardTimeSpecified
        {
            get
            {
                return !((StartFromMonth == null || StartFromMonth == -1) && (StartFromYear == null || StartFromYear == -1));
            }
        }

        public string StartFromMonthName
        {
            get
            {
                if (StartFromMonth > 0)
                {
                    return ((Enums.Months)StartFromMonth).GetDescription();
                }
                return string.Empty;
            }
        }
        public string EndFromMonthName
        {
            get
            {
                if (EndFromMonth > 0)
                {
                    return ((Enums.Months)EndFromMonth).GetDescription();
                }
                return string.Empty;
            }
        }

        public bool AnySkill { get { return SkillIncluded != null && SkillIncluded.Count > 0; } }

        public bool AnyTeamMember { get { return ParticipantIncluded != null && ParticipantIncluded.Count > 0; } } 
        #endregion

        public long ParentID { get; set; }
    }

    public class AwardViewModel : ParentAwardViewModel
    {
        public SelectList CareerHistoryList { get; set; }
        public long[] Skills { get; set; }
        public SelectList SkillList { get; set; }
        public bool AllowEdit { get; set; }
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }
        public bool IsPeriodMode
        {
            get
            {
                return StartFromMonth > 0 && EndFromMonth > 0;
            }
        }
    }

    public class PublicAwardViewModel : ParentAwardViewModel
    {
        public bool Congratulated { get; set; }
    }
}