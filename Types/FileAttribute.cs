using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Types
{
    public abstract class FileAttribute
    {
        public string FileName { get; set; }
        public Enums.FileType Type { get; set; }
        public int Size { get; set; }
        public int SizeInMb { get { return (Size / 1024) / 1024; } }
        public int SizeInKb { get { return (Size / 1024); } }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public string Location { get; set; }
    }
}
