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
    
    public partial class EntitySkillState
    {
        public long ID { get; set; }
        public long EntitySkillID { get; set; }
        public long EntityID { get; set; }
        public string IpAddress { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public int Type { get; set; }
        public int SubType { get; set; }
        public Nullable<byte> Status { get; set; }
    
        public virtual Entity Entity { get; set; }
        public virtual EntitySkill EntitySkill { get; set; }
    }
}