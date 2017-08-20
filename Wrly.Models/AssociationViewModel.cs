using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Models
{
    public class AssociationViewModel
    {
        public long AssociationID { get; set; }
        public Nullable<long> EntityID { get; set; }
        public Nullable<long> EntityID2 { get; set; }
        public Nullable<byte> AssociationType { get; set; }
        public Nullable<System.DateTime> CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public Nullable<byte> ObjectStatus { get; set; }
        public string IpAddress { get; set; }
        public Nullable<long> OppositeRowID { get; set; }
        public string EditedBy { get; set; }
        public Nullable<System.DateTime> EditedOn { get; set; }
        public Nullable<int> Score { get; set; }
    }
}
