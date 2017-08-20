using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Wrly.Models.Import
{
    public class ContactViewModel
    {
        public string Name { get; set; }
        public List<string> EmailList { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string EmailAddresses { get; set; }
        public bool Send { get; set; }
        public long ID { get; set; }
        /// <summary>
        /// ImportID
        /// </summary>
        public long EntityImportID { get; set; }
    }

    public class ContactImportViewModel
    {
        [Required(ErrorMessage="Invitee name cannot be left blank")]
        public string Name { get; set; }
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [Required(ErrorMessage = "Invite email address cannot be left blank")]
        public string EmailAddress { get; set; }
        public string Message { get; set; }
    }

}