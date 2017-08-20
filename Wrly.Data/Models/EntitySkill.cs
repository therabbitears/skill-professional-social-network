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
    
    public partial class EntitySkill
    {
        public EntitySkill()
        {
            this.AppreciationAndRecommendationSkills = new HashSet<AppreciationAndRecommendationSkill>();
            this.AwardSkills = new HashSet<AwardSkill>();
            this.CareerHistorySkills = new HashSet<CareerHistorySkill>();
            this.EntitySkillStates = new HashSet<EntitySkillState>();
            this.NetworkActivities = new HashSet<NetworkActivity>();
            this.Notificactions = new HashSet<Notificaction>();
        }
    
        public long EntitySkillID { get; set; }
        public int SkillID { get; set; }
        public long EntityID { get; set; }
        public Nullable<int> FromYear { get; set; }
        public Nullable<int> FromMonth { get; set; }
        public string IpAddress { get; set; }
        public System.DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<byte> ExpertiseLevel { get; set; }
        public Nullable<long> Score { get; set; }
        public Nullable<bool> Active { get; set; }
    
        public virtual ICollection<AppreciationAndRecommendationSkill> AppreciationAndRecommendationSkills { get; set; }
        public virtual ICollection<AwardSkill> AwardSkills { get; set; }
        public virtual ICollection<CareerHistorySkill> CareerHistorySkills { get; set; }
        public virtual Entity Entity { get; set; }
        public virtual Skill Skill { get; set; }
        public virtual ICollection<EntitySkillState> EntitySkillStates { get; set; }
        public virtual ICollection<NetworkActivity> NetworkActivities { get; set; }
        public virtual ICollection<Notificaction> Notificactions { get; set; }
    }
}