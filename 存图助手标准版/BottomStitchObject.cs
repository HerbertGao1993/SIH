using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace 存图助手标准版
{
    
    class BottomStitchObject
    {
        public string StationName;
        public DateTime TimeStamp;
        public List<FileInfo> BT01 = new List<FileInfo>(4);
        public List<FileInfo> BT02 = new List<FileInfo>(4);
        public List<FileInfo> BT03 = new List<FileInfo>(4);
        public List<FileInfo> BT04 = new List<FileInfo>(4);
        public List<FileInfo> BT05 = new List<FileInfo>(4);
        public List<FileInfo> BT06 = new List<FileInfo>(4);
        //public List<FileInfo> Images = new List<FileInfo>(6);
        public string Description = "";
        public string N0 = "-1";
        public string localDir = "";
        public string localFileName = "";
        public string ftpDir = "";
        //public string ftpFullName = "";
        public string ftpFileNameHeader = "";
        public string ftpFileName = "";
        public string Result = "GR";
        
        public string BoxID = "";
        public string extDescription = "";
        public bool handled = false;
        //public int ImagesNum = 0;
        public BottomStitchObject(string N0, string N12, string result, string description, DateTime timestamp, string localDir, string localfilename, string ftpDir, string ftpFileNameHeader, string ftpFileName, string boxid)
        {
            this.N0 = N0;
            this.Result = result;
            this.StationName = N12;
            this.Description = description;
            this.TimeStamp = timestamp;
            this.localDir = localDir;
            this.localFileName = localfilename;
            this.ftpDir = ftpDir;
            this.ftpFileNameHeader = ftpFileNameHeader;
            this.ftpFileName = ftpFileName;
            this.BoxID = boxid;
            this.BT01.Add(null);
            this.BT01.Add(null);
            this.BT01.Add(null);
            this.BT01.Add(null);
            this.BT02.Add(null);
            this.BT02.Add(null);
            this.BT02.Add(null);
            this.BT02.Add(null);
            this.BT03.Add(null);
            this.BT03.Add(null);
            this.BT03.Add(null);
            this.BT03.Add(null);
            this.BT04.Add(null);
            this.BT04.Add(null);
            this.BT04.Add(null);
            this.BT04.Add(null);
            this.BT05.Add(null);
            this.BT05.Add(null);
            this.BT05.Add(null);
            this.BT05.Add(null);
            this.BT06.Add(null);
            this.BT06.Add(null);
            this.BT06.Add(null);
            this.BT06.Add(null);
            //this.Images.Add(null);
            //this.Images.Add(null);
            //this.Images.Add(null);
            //this.Images.Add(null);
            //this.Images.Add(null);
            //this.Images.Add(null);
            this.handled = false;
            //this.ImagesNum = 0;
        }
    }
}
