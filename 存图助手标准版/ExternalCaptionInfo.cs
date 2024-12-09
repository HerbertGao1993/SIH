using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace 存图助手标准版
{
    class ExternalCaptionInfo
    {
        public string BoxID = "";
        public string extDescription = "";
        public DateTime Timestamp = DateTime.MaxValue;
        public ExternalCaptionInfo(string BoxID, string extDescription)
        {
            this.BoxID = BoxID;
            this.extDescription = extDescription;
            this.Timestamp = DateTime.Now;
        }
    }
}
