using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wrly.Models
{
    public class OrganizationSignupViewModel : BaseViewModel
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "Organization name cannot be left blank.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address cannot be left blank.")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [DisplayName("Email address")]
        public string EmailAddress { get; set; }

        public string Website { get; set; }

        [DisplayName("Category")]
        public int IndustryID { get; set; }

        [Required(ErrorMessage="Password cannot be left blank.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password:")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password does not match the confirm password.")]
        public string ConfirmPassword { get; set; }

        public SelectList Industries { get; set; }

        public string UserID { get; set; }
    }

    public class NewOrganizationViewModel : BaseViewModel
    {
        public long OrganizationID { get; set; }
        [DisplayName("Name")]
        [Required(ErrorMessage = "Organization name cannot be left blank")]
        public string Name { get; set; }

        public string EmployeeStrength { get; set; }

        public byte Status { get; set; }

        [Required(ErrorMessage = "Please enter about services you provide")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please enter email address")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }

        public string Website { get; set; }

        public string Phone1 { get; set; }

        public string Phone2 { get; set; }

        [DisplayName("Year founded")]
        public string EstablishedYear { get; set; }

        [Required(ErrorMessage = "Address cannot be left blank")]
        public string AddressLine1 { get; set; }

        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }

        [Required(ErrorMessage = "City name cannot be left blank")]
        public string CityName { get; set; }

        [Required(ErrorMessage = "Zip/Pin/ code cannot be left blank")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "State name cannot be left blank")]
        public string StateName { get; set; }
        public SelectList Cities { get; set; }
        public HttpPostedFileBase Logo { get; set; }
        public SelectList Countries { get; set; }
        public string AddressLine4 { get; set; }


        [DisplayName("Category")]
        public int IndustryID { get; set; }
        public SelectList Industries { get; set; }

        public SelectList EmployeeStrengths { get; set; }

        public string County { get; set; }

        public string Slogan { get; set; }

        public string Country { get; set; }
    }

    public class ExtendedInfoViewModel : BaseViewModel
    {
        public SelectList EmployeeStrengths { get; set; }
        public string EmployeeStrength { get; set; }

        public string Phone1 { get; set; }

        [Required(ErrorMessage = "Tell something your organization or services you offer.")]
        public string Description { get; set; }

        public string EstablishedYear { get; set; }
    }

    public class AddressViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "Address cannot be left blank")]
        public string AddressLine1 { get; set; }
        public int AddressType { get; set; }

        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }

        [Required(ErrorMessage = "City name cannot be left blank")]
        public string City { get; set; }
        [Required(ErrorMessage = "State name cannot be left blank")]
        public string State { get; set; }
        [Required(ErrorMessage = "Country name cannot be left blank")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Zip/Pin/ code cannot be left blank")]
        public string ZipCode { get; set; }

        public SelectList Cities { get; set; }
        public SelectList Countries { get; set; }
        public long AddressId { get; set; }
    }

    public class PhoneViewModel:BaseViewModel
    {
        public string Phone { get; set; }
        public int PhoneType { get; set; }
        public long PhoneID { get; set; }
    }
}