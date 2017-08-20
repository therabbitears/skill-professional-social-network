using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class ForgotPasswordViewModel
    {
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        [Required(ErrorMessage="Email address cannot be left blank.")]
        public string EmailAddress { get; set; }
    }
}