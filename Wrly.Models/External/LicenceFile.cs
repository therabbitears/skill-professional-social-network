using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Models.External
{
    public class LicenceFile
    {
        [Required(ErrorMessage="Company name is required.")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Total users is required.")]
        public string TotalUsers { get; set; }
        [Required(ErrorMessage = "Date of generation is required.")]
        public string Stamp { get; set; }
        [Required(ErrorMessage = "Licence category is required.")]
        public string LicenceCategory { get; set; }
        [Required(ErrorMessage = "Validity is required.")]
        public string Validity { get; set; }
        
        public string LicenceString { get; set; }
        public string SaltHash { get; set; }
        
        public DateTime? DateOfActivation { get; set; }
        public DateTime? DateOfExpiry { get; set; }
        public bool Result { get; set; }
        public string Message { get; set; }
    }
}
