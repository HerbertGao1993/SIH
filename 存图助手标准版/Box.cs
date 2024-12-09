using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace 存图助手标准版
{
    class Box//供包台绑定用的Box对象
    {
        //构造函数初始化后不能更改
        public readonly object BoxLock = new object();
        public readonly DateTime CreateTime;
        //底扫
        public DateTime BottomTimestamp = DateTime.MinValue;
        public int InductID = 0;//底扫上包台区域
        public string BottomCodes = "";
        public string BottomResult = "";
        
        public string BottomN0 = "-1";
        public string BottomN1 = "xx";
        /*
        public string BottomN3 = "xx";
        public string BottomN4 = "xx";
        public string BottomN5 = "xx";
        public string BottomN6 = "xx";
        public string BottomN7 = "xx";
        public string BottomN8 = "xx";
        public string BottomN9 = "xx";
        public string BottomN10 = "xx";
        public string BottomN11 = "xx";
        */
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
        public string BoxID = "000000";
        public string extDescription = "";
        public int TrayID = 9999;
        public FileInfo[] Images = new FileInfo[6] { null, null, null, null, null, null };//UP LF LB RF RB DN
        public bool[] Uploaded = new bool[6] { false, false, false, false, false, false };//UP LF LB RF RB DN
        public Box(string boxid,int inductid)
        {
            this.BoxID = boxid;
            this.InductID = inductid;
            this.CreateTime = DateTime.Now;
        }

    }
}
