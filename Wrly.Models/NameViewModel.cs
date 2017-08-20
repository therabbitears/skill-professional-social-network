using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Wrly.Models
{
    public class NameViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public SelectList Formats { get; set; }
        [Required]
        public int NameFormat { get; set; }
    }

    public class OrgNameViewModel
    {
        [Required]
        public string Name { get; set; }
    }

    public class OrgCategoryViewModel
    {
        [Required]
        [Range(1,int.MaxValue)]
        public int CategoryID { get; set; }
        public string Category { get; set; }
        public SelectList CategoryList { get; set; }
    }

    public class HeadingViewModel
    {
        [Required]
        public string Heading { get; set; }
    }
}