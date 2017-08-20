using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Wrly.Models.Share;

namespace Wrly.Models.Feeds
{
    public class HomeFeedViewModel
    {
        public List<FeedDetailViewModel> Feeds { get; set; }
        public long LoadedOn { get; set; }
    }
}