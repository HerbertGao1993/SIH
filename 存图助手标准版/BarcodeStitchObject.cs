using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace 存图助手标准版
{
    class BarcodeStitchObject
    {
        public double parcelHeight;
        public string StationName;
        public string Description = "";
        public string N0 = "-1";
        public string N1 = "UP01";
        public string N2 = "xx";
        public string N3 = "xx";
        public string N4 = "xx";
        public string N5 = "xx";
        public string N6 = "xx";
        public string N7 = "xx";
        public string N8 = "xx";
        public string N9 = "xx";
        public string N10 = "xx";
        public string N11 = "xx";
        public string localDir = "";
        public string localFileName = "";
        public string ftpDir = "";
        public string ftpFileNameHeader = "";
        public string ftpFileName = "";
        public string barcodePos = "";
        public string extDescription = "";
        public bool handled = false;
        public DateTime TimeStamp;//对象生成时间
        public DateTime TriggerTimestamp;//包裹触发时间
        public string pos = "";
        public string BoxID = "";
        public FileInfo UPImage = null;
        public FileInfo BarcodeImage = null;
        public List<FileInfo> NRImage = new List<FileInfo>(100);
        public readonly object NRLock = new object();
        public BarcodeStitchObject(string N0, string N2, string N3, string N4, string N5, string N6, string N7, string N8, string N9, string N10, string N11, string N12, string description, DateTime timestamp, string localDir, string localFileName, string ftpDir, string ftpFileNameHeader,string ftpFileName, string pos, string barcodePos, DateTime TriggerTimestamp,string boxid,string parcelHeight)
        {
            this.N0 = N0;
            this.N2 = N2;
            this.N3 = N3;
            this.N4 = N4;
            this.N5 = N5;
            this.N6 = N6;
            this.N7 = N7;
            this.N8 = N8;
            this.N9 = N9;
            this.N10 = N10;
            this.N11 = N11;
            this.StationName = N12;
            this.BoxID = boxid;
            this.Description = description;
            this.TimeStamp = timestamp;
            this.TriggerTimestamp = TriggerTimestamp;
            this.localDir = localDir;
            this.localFileName = localFileName;
            this.ftpDir = ftpDir;
            this.ftpFileNameHeader = ftpFileNameHeader;
            this.ftpFileName = ftpFileName;
            this.pos = pos;
            this.barcodePos = barcodePos;
            this.handled = false;
            try
            {
                this.parcelHeight = Convert.ToDouble(parcelHeight);
            }
            catch 
            { 
                this.parcelHeight = -1; 
            }
        }
    }
}
