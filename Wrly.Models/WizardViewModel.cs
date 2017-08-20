using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wrly.Models
{

    public class WizardViewModel
    {
        public long EntityID { get; set; }
        public long PersonID { get; set; }
        public bool IsWizard { get; set; }
        public int StepCreatedFor { get; set; }
        public DateTime Stamp { get; set; }
        public string DbID { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }
    }

    public class WizardResultViewModel
    {
        public string RedirectUrl { get; set; }
        public bool IsSuccess { get; set; }
    }

    public class WizardHashViewModel
    {
        public long EntityID { get; set; }
        public long PersonID { get; set; }
        public bool IsWizard { get; set; }
        public DateTime Stamp { get; set; }
        public int StepCreatedFor { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddress { get; set; }
        public string Id { get; set; }
    }
}