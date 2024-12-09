using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace 存图助手标准版
{
    public class ExtCaptionBox
    {
        public readonly object BoxLock = new object();
        public readonly DateTime CreateTime;
        public string BoxID = "";
        public string N0 = "";
        public string N12 = "";
        public List<FileInfo> Images = new List<FileInfo>(10);
        public string extDescription = "";
        public bool handled = false;
        public ExtCaptionBox(string N0, string N12, string BoxID)
        {
            this.N0 = N0;
            this.N12 = N12;
            this.BoxID = BoxID;
            this.CreateTime = DateTime.Now;
        }
    }
}
