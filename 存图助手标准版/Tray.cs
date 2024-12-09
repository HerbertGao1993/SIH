using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace 存图助手标准版
{
    class Tray
    {
        //构造函数初始化后不能更改
        public readonly int TrayID = 9999;
        public readonly object TrayInductorLock = new object();
        //底扫
        public DateTime BottomTimestamp = DateTime.MinValue;
        public int InductID = 0;//底扫上包台区域
        public string BottomCodes = "";
        public string BottomResult = "";
        public string BoxID = "000000";
        //主线
        public DateTime SorterTimestamp = DateTime.MinValue;
        public int CurrentImageInductID = 0;//当前主线图片的区域
        public string SorterCodes = "";
        public string SorterResult = "";
        public string N0 = "-1";
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
        public string N12 = "-1";
        //共有
        public FileInfo[] Images = new FileInfo[6] { null, null, null, null, null, null };//UP LF LB RF RB DN
        public bool[] Uploaded = new bool[6] { false, false, false, false, false, false };//UP LF LB RF RB DN
        public Tray(int trayid)
        {
            this.TrayID = trayid;       
        }
    }
}
