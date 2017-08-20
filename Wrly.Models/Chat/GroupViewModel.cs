using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wrly.Models.Chat
{
    public class GroupViewModel
    {
        public long ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public List<AuthorViewModel> Participants { get; set; }
    }
}
