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
    
    public partial class Master_EmailType
    {
        public Master_EmailType()
        {
            this.Emails = new HashSet<Email>();
        }
    
        public int EmailType { get; set; }
        public string Name { get; set; }
        public string Decription { get; set; }
        public bool Active { get; set; }
    
        public virtual ICollection<Email> Emails { get; set; }
    }
}