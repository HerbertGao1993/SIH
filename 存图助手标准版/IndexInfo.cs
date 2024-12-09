using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace 存图助手标准版
{
    class IndexInfo
    {
        //构造函数初始化后不能更改
        public readonly int ID = -1;
        public readonly int InductID = 0;
        public readonly object InductInfoLock = new object();
        //底扫
        public DateTime BottomTimestamp = DateTime.MinValue;
        public string BottomCodes = "";
        public string BottomResult = "";
        public FileInfo BottomImage = null;
        public int TrayID = 9999;
        public string BoxID = "000000";
        public IndexInfo(int id,int inductid)
        {
            this.ID = id;
            this.InductID = inductid;
        }
    }
}
