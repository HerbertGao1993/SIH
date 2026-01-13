using System;
using System.Collections.Generic;
using System.IO;

namespace 存图助手标准版
{
    class MasterImage
    {
        public bool barcodestitchenabled = false;
        public bool bottomenabled = false;
        public int time = 0;
        public bool BarcodeImagePresent = false;
        public string N0 = "-1";
        //public string N1 = "xx";
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
        public DateTime TimeStamp;//主机图片生成时间
        public DateTime TriggerTimeStamp;//主机图片触发时间
        public List<FileInfo> Images = new List<FileInfo>();//Http图片
        public List<DateTime> ImageTime = new List<DateTime>();//Http图片时间戳
        public List<string> BcrName = new List<string>();//Http图片读码器名
        public int retry = 5;
        public DateTime LastTry = DateTime.MinValue;
        public string token = "";
        public string md5 = "";
        public int AllowUpload = -1;//-1:未定义；0：不允许；1：允许
        public bool uploaded = false;
        public bool upload2 = false;
        public List<string> JDAIFullFileName = new List<string>(20);
        public readonly object JD_AI_INFO_LOCK = new object();
        public bool JDAIUploaded = false;
        volatile public int boxid = -1;
        public MasterImage(string N0, string N2, string N3, string N4, string N5, string N6, string N7, string N8, string N9, string N10, string N11, DateTime timestamp, DateTime triggertimestamp)
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
            this.TimeStamp = timestamp;
            this.TriggerTimeStamp = triggertimestamp;
            this.time = 0;
            this.upload2 = false;
            if (N2 != "None")
            {
                this.BarcodeImagePresent = false;
            }
            else
            {
                this.BarcodeImagePresent = false;
                //this.BarcodeImagePresent = true;
            }
            this.bottomenabled = true;
            this.barcodestitchenabled = true;
        }
        public MasterImage()
        { 
            this.bottomenabled = false;
            this.barcodestitchenabled = false;
        }
    }
}
