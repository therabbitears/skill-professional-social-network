
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class AbuseViewModel
    {
        [Required(ErrorMessage="Name cannot be left blank.")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address cannot be left blank.")]
        [StringLength(120, ErrorMessage = "Email address cannot be longer than 120 characters")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Invalid email address formate, please email address in [aaaaa@aaa.aaa] formate.")]
        public string EmailAddress { get; set; }

        [Required(ErrorMessage = "Value cannot be left blank.")]
        public string Particular { get; set; }

        public string Description { get; set; }

        public string Type { get; set; }
    }
}