//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Wrly.Data.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class CategoryMaster
    {
        public CategoryMaster()
        {
            this.Organizations = new HashSet<Organization>();
        }
    
        public int Categoryid { get; set; }
        public string CategoryName { get; set; }
        public string SubCategoryName { get; set; }
    
        public virtual ICollection<Organization> Organizations { get; set; }
    }
}