using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;
using Wrly.Infrastructure.Extended;

namespace Wrly.Models.Listing
{
    public class CareerHistoryViewModel : BaseViewModel
    {
        public long OrganizationID { get; set; }
        public long CareerHistoryID { get; set; }
        public virtual int? StartFromDay { get; set; }

        [CannotGreaterMonthAndYear("StartFromYear", "EndFromMonth", "EndFromYear", true, true, false, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [RequeiredIfSelected("StartFromYear", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Month needs to be selected in case year is selected")]
        public virtual int? StartFromMonth { get; set; }

        [CannotGreaterMonthAndYear("StartFromMonth", "EndFromMonth", "EndFromYear", true, false, true, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [RequeiredIfSelected("StartFromMonth", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public virtual int? StartFromYear { get; set; }


        public virtual int? EndFromDay { get; set; }

        [CannotGreaterMonthAndYear("EndFromYear", "StartFromMonth", "StartFromYear", false, true, false, ErrorMessage = "Year and month combination must be greater than what selected as starting combination.")]
        [RequeiredIfSelected("EndFromYear", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Month needs to be selected in case year is selected")]
        public virtual int? EndFromMonth { get; set; }

        [CannotGreaterMonthAndYear("EndFromMonth", "StartFromMonth", "StartFromYear", false, false, true, ErrorMessage = "Year must be greater than what selected as starting year.")]
        [RequeiredIfSelected("EndFromMonth", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public virtual int? EndFromYear { get; set; }
        public string About { get; set; }


        public string FormatedDescription
        {
            get
            {
                if (!string.IsNullOrEmpty(About))
                {
                    if (About.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Length > 1)
                    {
                        return string.Format("<ul class='career-history-details'>{0}</ul>", string.Join(Environment.NewLine, About.Split(new string[] { Environment.NewLine }, StringSplitOptions.None).Select(x => string.Format("<li>{0}</li>", x)).ToList()));
                    }
                }
                return About;
            }
        }



        [Required]
        public virtual string OrganizationName { get; set; }
        public string ProfileName { get; set; }

        public long JobTitleID { get; set; }
        [Required]
        public virtual string JobTitleName { get; set; }

        public bool IsCurrent { get; set; }
        public SelectList MonthList { get; set; }
        public SelectList YearList { get; set; }

        public bool AllowEdit { get; set; }

        public string StartFromMonthName { get; set; }

        public string EndFromMonthName { get; set; }

        public short Type { get; set; }

        public string[] Skills { get; set; }

        public List<CareerHistorySkillViewModel> SkillIncluded { get; set; }


        public long EntityID { get; set; }

        [JsonIgnore]
        public bool HasEducationStartSpecified { get { return StartFromMonth != null && StartFromYear != null && StartFromYear > 0 && StartFromMonth > 0; } }

        public string SubType { get; set; }

        public bool Confidential { get; set; }

        public bool HasValidEndDate
        {
            get { return EndFromMonth != null && EndFromMonth > 0 && EndFromYear != null && EndFromYear > 0; }
        }

        public SelectList ComingYearList { get; set; }

        public SelectList DayList { get; set; }

        public SelectList EndDayList { get; set; }

        public bool IsPeriodMode
        {
            get
            {
                return StartFromMonth > 0 && EndFromMonth > 0;
            }
        }
    }

    public class IntelligenceCareerHistoryViewModel : CareerHistoryViewModel
    {
        public string DisplayText { get; set; }
    }
}