using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wrly.Models
{
    public class ApplyViewModel:SharedBaseViewModel
    {
        public string Text { get; set; }
        public bool NotifyNetwork { get; set; }
    }
}
