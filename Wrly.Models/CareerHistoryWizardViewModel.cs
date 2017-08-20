using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Wrly.Infrastructure.Extended;
using Wrly.Models.Listing;

namespace Wrly.Models
{
    public class CareerHistoryWizardViewModel : CareerHistoryViewModel
    {
        public int CareerStage { get; set; }
        public int EmployementEndedStage { get; set; }

        public int DreamJobTitleID { get; set; }
        public string DreamJobTitle { get; set; }

        [Required(ErrorMessage = "Organization cannot be left blank")]
        public override string OrganizationName { get; set; }

        [Required(ErrorMessage = "Job title cannot be left blank")]
        public override string JobTitleName { get; set; }

        public int UniversityID { get; set; }
        [Required(ErrorMessage = "Univertsity/college cannot be left blank")]
        public string UniversityName { get; set; }

        public int CourseID { get; set; }
        [Required(ErrorMessage = "Course name cannot be left blank")]
        public string CourseName { get; set; }


        public override int? StartFromDay { get; set; }

        [CannotGreaterMonthAndYear("StartFromYear", "EndFromMonth", "EndFromYear", true, true, false, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [RequeiredIfSelected("StartFromYear", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Month needs to be selected in case year is selected")]
        public virtual int? StartFromMonth { get; set; }

        [CannotGreaterMonthAndYear("StartFromMonth", "EndFromMonth", "EndFromYear", true, false, true, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [RequeiredIfSelected("StartFromMonth", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public virtual int? StartFromYear { get; set; }

        public override int? EndFromDay { get; set; }

        
        [CannotGreaterMonthAndYear("EndFromYear", "StartFromMonth", "StartFromYear", false, true, false, ErrorMessage = "Year and month combination must be greater than what selected as starting combination.")]
        [RequeiredIfSelected("EndFromYear", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Month needs to be selected in case year is selected")]
        [IfCheckedNeedToSelect("Working", ErrorMessage = "In case if you are not working here, the month and year you end here must be defined")]
        public virtual int? EndFromMonth { get; set; }

        [CannotGreaterMonthAndYear("EndFromMonth", "StartFromMonth", "StartFromYear", false, false, true, ErrorMessage = "Year must be greater than what selected as starting year.")]
        [RequeiredIfSelected("EndFromMonth", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Year needs to be selected in case the month is selected")]
        [IfCheckedNeedToSelect("Working", ErrorMessage = "In case if you are not working here, the month and year you end here must be defined")]
        public virtual int? EndFromYear { get; set; }


        //[CannotGreaterMonthAndYear("EndFromYear", "StartFromMonth", "StartFromYear", false, true, false, ErrorMessage = "Year and month combination must be greater than what selected as starting combination.")]
        //[IfCheckedNeedToSelect("Working", ErrorMessage = "In case if you are not working here, the month and year you end here must be defined")]
        //[RequeiredIfSelected("EndFromYear", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Month needs to be selected in case year is selected")]
        //public override int? EndFromMonth { get; set; }

        //[CannotGreaterMonthAndYear("EndFromMonth", "StartFromMonth", "StartFromYear", false, false, true, ErrorMessage = "Year must be greater than what selected as starting year.")]
        //[IfCheckedNeedToSelect("Working", ErrorMessage = "In case if you are not working here, the month and year you end here must be defined")]
        //[RequeiredIfSelected("EndFromMonth", Mode = Types.Enums.CareerStage.Employement, ErrorMessage = "Year needs to be selected in case the month is selected")]
        //public override int? EndFromYear { get; set; }

        [CannotGreaterMonthAndYear("EducationStartFromYear", "EducationEndFromMonth", "EducationEndFromYear", true, true, false, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [Required(ErrorMessage = "Your start cannot be left blank")]
        [RequeiredIfSelected("EducationStartFromYear", Mode = Types.Enums.CareerStage.Student, ErrorMessage = "Month needs to be selected in case year is selected")]
        public int? EducationStartFromMonth { get; set; }

        
        [CannotGreaterMonthAndYear("EducationStartFromMonth", "EducationEndFromMonth", "EducationEndFromYear", true, false, true, ErrorMessage = "Year and month combination must be smaller than what selected as ending combination.")]
        [Required(ErrorMessage = "Your start year cannot be left blank")]
        [RequeiredIfSelected("EducationStartFromMonth", Mode = Types.Enums.CareerStage.Student, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public int? EducationStartFromYear { get; set; }

        [CannotGreaterMonthAndYear("EducationEndFromYear", "EducationStartFromMonth", "EducationStartFromYear", false, true, false, ErrorMessage = "Year and month combination must be greater than what selected as starting combination.")]
        [Required(ErrorMessage = "End  of month as course(Or expected) cannot be left blank")]
        [RequeiredIfSelected("EducationEndFromYear", Mode = Types.Enums.CareerStage.Student, ErrorMessage = "Month needs to be selected in case year is selected")]
        public int? EducationEndFromMonth { get; set; }

        [CannotGreaterMonthAndYear("EducationEndFromMonth", "EducationStartFromMonth", "EducationStartFromYear", false, false, true, ErrorMessage = "Year must be greater than what selected as starting year.")]
        [Required(ErrorMessage = "End  of year as course(Or expected) cannot be left blank")]
        [RequeiredIfSelected("EducationEndFromMonth", Mode = Types.Enums.CareerStage.Student, ErrorMessage = "Year needs to be selected in case the month is selected")]
        public int? EducationEndFromYear { get; set; }

        public bool Working { get; set; }
        
        public int Industry { get; set; }
        
        public string IndustryName { get; set; }
    }
}