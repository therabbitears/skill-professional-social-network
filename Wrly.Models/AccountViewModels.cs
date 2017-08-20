using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Wrly.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [Required(ErrorMessage = "Email address cannot be left blank.")]
        [Display(Name = "Email address")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "First name cannot be left blank.")]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name cannot be left blank.")]
        [Display(Name = "Last name")]
        public string LastName { get; set; }
    }


    public class ResetPasswordViewModel : BaseViewModel
    {
        [Required(ErrorMessage = "New password cannot be left blank.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class ManageUserViewModel
    {
        [Required(ErrorMessage = "Old password cannot be left blank.")]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required(ErrorMessage = "New password cannot be left blank.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginViewModel
    {
        [Required(ErrorMessage="User name cannot be left blank.")]
        [Display(Name = "User name:")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password cannot be left blank.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Display(Name = "User name:")]
        public string UserName { get; set; }

        [Required(ErrorMessage="Password cannot be left blank.")]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password:")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name cannot be left blank.")]
        [DisplayName("First name:")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last name cannot be left blank.")]
        [DisplayName("Last name(surname):")]
        public string LastName { get; set; }
        [DisplayName("Your name:")]
        public string FullName { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [Required(ErrorMessage = "Email address cannot be left blank.")]
        [DisplayName("Email address:")]
        public string EmailAddress { get; set; }
        public int Status { get; set; }

        public NewOrganizationViewModel OrganiationInfo { get; set; }
    }

    public class ConnectAccountViewModel : BaseViewModel
    {
        public RegisterViewModel RegisterInfo { get; set; }
        [Required(ErrorMessage = "Your job role at this organization cannot be left blank.")]
        public string JobRollName { get; set; }
        public int JobRoleID { get; set; }

        public long OrganizationID { get; set; }
        public string Name { get; set; }
    }
}
