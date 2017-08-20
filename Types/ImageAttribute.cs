using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public class ImageAttribute : FileAttribute
    {
        public bool IsValidImage { get; set; }
        public int WidthInPixels { get; set; }
        public int HeightInPixels { get; set; }
        public string Message { get; set; }
    }
}
