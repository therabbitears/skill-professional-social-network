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
    
    public partial class Widget
    {
        public Widget()
        {
            this.EntityWidgets = new HashSet<EntityWidget>();
        }
    
        public long WidgetID { get; set; }
        public string WidgetName { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<System.DateTime> UpdateDate { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> CategoryID { get; set; }
        public string Icon { get; set; }
        public string Help { get; set; }
        public string Description { get; set; }
        public Nullable<bool> ReadOnly { get; set; }
        public Nullable<bool> Active { get; set; }
        public Nullable<bool> IsPrimium { get; set; }
        public Nullable<bool> ForIndividual { get; set; }
        public Nullable<bool> Organization { get; set; }
    
        public virtual ICollection<EntityWidget> EntityWidgets { get; set; }
    }
}