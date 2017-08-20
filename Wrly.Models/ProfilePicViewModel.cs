using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wrly.Models
{
    public class ProfilePicViewModel
    {
        public HttpPostedFileBase Image { get; set; }
        public decimal ImgX1 { get; set; }
        public decimal ImgY1 { get; set; }
        public decimal ImgWidth { get; set; }
        public decimal ImgHeight { get; set; }
        public string FileName { get; set; }
    }
}