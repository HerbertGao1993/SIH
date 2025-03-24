using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenCvSharp;
using SocketLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
#if FEATURE_VPRO
using Cognex.VisionPro;
using Cognex.VisionPro.ToolBlock;
using Cognex.VisionPro.ImageFile;
#endif

namespace 存图助手标准版
{
    public partial class MainForm : Form
    {
        private GridSetting gs = new GridSetting();
        internal static string DefaultPixelSize = "3.45";
        internal static string DefaultFocusLength = "16";
        internal static string DefaultReadAngle = "25";
        internal static string DefaultMountingHeight = "1500";
        internal static string DefaultMaxParcelHeight = "700";
        internal static double PixelSize = 3.45;//um
        internal static double FocusLength = 16;//mm
        internal static double ReadAngle = 25;//deg
        internal static double MountingHeight = 1500;//mm
        internal static double MaxParcelHeight = 700;//mm
        static readonly object L = new object();
        private bool GridLineEnable = false;
        private bool imgEncryptEnable = false;
        static string imgLockPSW = "";
        static string imgLockIV = "";
        int mImageIndex = 0;
        //static string mImageFolderPath = Application.StartupPath + "\\images";
        public static bool DWSInterfaceEnabled = false;
        private static IntPtr hWnd = IntPtr.Zero;
        private List<string> JDRegexList = new List<string>();
        private List<string> BroadJDRegexList = new List<string>();
        private bool LastJDRegexSuccessful = false;
        private DateTime LastRegexCompleteTime = DateTime.MinValue;
        private int JDRegexRetry = 5;
        private static DateTime PowerOnTime = DateTime.Now;
        private string KeyPath = Application.StartupPath + "\\keys\\privatekey.txt";
        private string PassPhrase = "cognex";
        private SFTPHelper sftp1;
        private SFTPHelper sftp2;
        private SFTPHelper sftp3;
        private SFTPHelper sftp4;
        private SFTPHelper sftp5;
        private static InductorSetting ind = new InductorSetting();
        private static IndexInfo[] IndexObjects11 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects12 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects13 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects14 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects15 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects21 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects22 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects23 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects24 = new IndexInfo[32];
        private static IndexInfo[] IndexObjects25 = new IndexInfo[32];
        private static MonitorClientThread Monitor11 = new MonitorClientThread("Bottom11Handler");
        private static MonitorClientThread Monitor12 = new MonitorClientThread("Bottom12Handler");
        private static MonitorClientThread Monitor13 = new MonitorClientThread("Bottom13Handler");
        private static MonitorClientThread Monitor14 = new MonitorClientThread("Bottom14Handler");
        private static MonitorClientThread Monitor15 = new MonitorClientThread("Bottom15Handler");
        private static MonitorClientThread Monitor21 = new MonitorClientThread("Bottom21Handler");
        private static MonitorClientThread Monitor22 = new MonitorClientThread("Bottom22Handler");
        private static MonitorClientThread Monitor23 = new MonitorClientThread("Bottom23Handler");
        private static MonitorClientThread Monitor24 = new MonitorClientThread("Bottom24Handler");
        private static MonitorClientThread Monitor25 = new MonitorClientThread("Bottom25Handler");
        private static MonitorClientThread MonitorDWS1 = new MonitorClientThread("DWS1Handler");
        private static Thread threadclient11 = new Thread(Monitor11.Monitor);
        private static Thread threadclient12 = new Thread(Monitor12.Monitor);
        private static Thread threadclient13 = new Thread(Monitor13.Monitor);
        private static Thread threadclient14 = new Thread(Monitor14.Monitor);
        private static Thread threadclient15 = new Thread(Monitor15.Monitor);
        private static Thread threadclient21 = new Thread(Monitor21.Monitor);
        private static Thread threadclient22 = new Thread(Monitor22.Monitor);
        private static Thread threadclient23 = new Thread(Monitor23.Monitor);
        private static Thread threadclient24 = new Thread(Monitor24.Monitor);
        private static Thread threadclient25 = new Thread(Monitor25.Monitor);
        private static Thread threadclientDWS1 = new Thread(MonitorDWS1.Monitor);
        private static Thread threadserver = new Thread(MonitorServer);
        private static Thread threadresidual1 = new Thread(new ParameterizedThreadStart(HandleResidualExtCaptionImage));
        private static Thread threadresidual2 = new Thread(new ParameterizedThreadStart(HandleResidualExtCaptionImage));
        private static Thread threadresidual3 = new Thread(new ParameterizedThreadStart(HandleResidualExtCaptionImage));
        private static Thread threadresidual4 = new Thread(new ParameterizedThreadStart(HandleResidualExtCaptionImage));
        private static Thread threadDWSProcess = new Thread(MonitorDWSProcess);
        //private Thread CaptionThread1 = null;
        //private Thread CaptionThread2 = null;
        //private Thread CaptionThread3 = null;
        //private Thread CaptionThread4 = null;
        //private Thread CaptionThread5 = null;
        internal static SocketLib.TcpClientBase DWS1Handler;
        internal static SocketLib.TcpClientBase Bottom11Handler;
        internal static SocketLib.TcpClientBase Bottom12Handler;
        internal static SocketLib.TcpClientBase Bottom13Handler;
        internal static SocketLib.TcpClientBase Bottom14Handler;
        internal static SocketLib.TcpClientBase Bottom15Handler;
        internal static SocketLib.TcpClientBase Bottom21Handler;
        internal static SocketLib.TcpClientBase Bottom22Handler;
        internal static SocketLib.TcpClientBase Bottom23Handler;
        internal static SocketLib.TcpClientBase Bottom24Handler;
        internal static SocketLib.TcpClientBase Bottom25Handler;
        internal static SocketLib.TcpServerBase LocalServerHandler;
        public static bool LocalServerEnable = true;
        public static bool Induct11Enable = true;
        public static bool Induct12Enable = true;
        public static bool Induct13Enable = true;
        public static bool Induct14Enable = true;
        public static bool Induct15Enable = true;
        public static bool Induct21Enable = true;
        public static bool Induct22Enable = true;
        public static bool Induct23Enable = true;
        public static bool Induct24Enable = true;
        public static bool Induct25Enable = true;
        public static string Bottom11IP = "192.168.0.1";
        public static string Bottom12IP = "192.168.0.2";
        public static string Bottom13IP = "192.168.0.3";
        public static string Bottom14IP = "192.168.0.4";
        public static string Bottom15IP = "192.168.0.5";
        public static string Bottom21IP = "192.168.0.6";
        public static string Bottom22IP = "192.168.0.7";
        public static string Bottom23IP = "192.168.0.8";
        public static string Bottom24IP = "192.168.0.9";
        public static string Bottom25IP = "192.168.0.10";
        public static string Bottom11Port = "2112";
        public static string Bottom12Port = "2112";
        public static string Bottom13Port = "2112";
        public static string Bottom14Port = "2112";
        public static string Bottom15Port = "2112";
        public static string Bottom21Port = "2112";
        public static string Bottom22Port = "2112";
        public static string Bottom23Port = "2112";
        public static string Bottom24Port = "2112";
        public static string Bottom25Port = "2112";
        public static string ExtCaptionIP = "192.168.1.11";
        public static string DWS1IP = "192.168.1.11";
        public static string DWS1Port = "2112";
        public static string Plc11IP = "192.168.1.1";
        public static string Plc12IP = "192.168.1.2";
        public static string Plc13IP = "192.168.1.3";
        public static string Plc14IP = "192.168.1.4";
        public static string Plc15IP = "192.168.1.5";
        public static string Plc21IP = "192.168.1.6";
        public static string Plc22IP = "192.168.1.7";
        public static string Plc23IP = "192.168.1.8";
        public static string Plc24IP = "192.168.1.9";
        public static string Plc25IP = "192.168.1.10";
        public static string LocalIP = "192.168.1.11";
        public static string LocalPort = "2112";
        public static bool running = true;
        //private static Tray[] TrayObjects = new Tray[1000];
#if FEATURE_VPRO
        private CogImageFileTool mImageFileTool1 = new CogImageFileTool();
        private CogImageFileTool mImageFileTool2 = new CogImageFileTool();
        private CogImageFileTool mImageFileTool3 = new CogImageFileTool();
        private CogImageFileTool mImageFileTool4 = new CogImageFileTool();
        private CogImageFileTool mImageFileTool5 = new CogImageFileTool();
        private CogToolBlock mTB = null;
#endif
        private readonly static Object TBLock = new object();
        private readonly static Object hWndLock = new object();
        private readonly static Object ExtCaptionBoxListLock1 = new object();
        private readonly static Object ExtCaptionBoxListLock2 = new object();
        private readonly static Object ExtCaptionBoxListLock3 = new object();
        private readonly static Object ExtCaptionBoxListLock4 = new object();
        private static List<ExtCaptionBox> ExtCaptionBoxList1 = new List<ExtCaptionBox>(10000);//外部水印绑定用的BoxList
        private static List<ExtCaptionBox> ExtCaptionBoxList2 = new List<ExtCaptionBox>(10000);
        private static List<ExtCaptionBox> ExtCaptionBoxList3 = new List<ExtCaptionBox>(10000);
        private static List<ExtCaptionBox> ExtCaptionBoxList4 = new List<ExtCaptionBox>(10000);
        private static List<Box> BoxList = new List<Box>(10000);//供包台绑定用的BoxList
        private readonly static Object BoxListLock = new object();
        private static List<ExternalCaptionInfo> ExternalCaptionInfoList = new List<ExternalCaptionInfo>(10000);//外部水印信息
        private readonly static Object ExternalCaptionInfoListLock = new object();
        private int[] ol01 = { 0, 0, 0, 0, 0, 0 };
        private int[] ol02 = { 0, 0, 0, 0, 0, 0 };
        private int[] ol03 = { 0, 0, 0, 0, 0, 0 };
        private int[] ol04 = { 0, 0, 0, 0, 0, 0 };
        EncoderParameters ep = new EncoderParameters(1);
        EncoderParameters epbottom = new EncoderParameters(1);
        private Calib cal = new Calib();
        private static double[] defaulthomo = { 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0 };
        //private static Mat defaultMat = new Mat(3, 3, MatType.CV_64FC1, defaulthomo);
        private static Mat defaultMat = Mat.FromPixelData(3, 3, MatType.CV_64FC1, defaulthomo);
        private Mat[] HomographySta1 = new Mat[5] { defaultMat, defaultMat, defaultMat, defaultMat, defaultMat };
        private Mat[] HomographySta2 = new Mat[5] { defaultMat, defaultMat, defaultMat, defaultMat, defaultMat };
        private Mat[] HomographySta3 = new Mat[5] { defaultMat, defaultMat, defaultMat, defaultMat, defaultMat };
        private Mat[] HomographySta4 = new Mat[5] { defaultMat, defaultMat, defaultMat, defaultMat, defaultMat };
        private bool SCADAflag = true;
        private bool allowChinese = false;
        internal static string AdminUser;
        internal static string AdminPassword;
        internal static calObject cal1_1 = new calObject();
        internal static calObject cal2_1 = new calObject();
        internal static calObject cal3_1 = new calObject();
        internal static calObject cal4_1 = new calObject();
        internal static calObject cal1_2 = new calObject();
        internal static calObject cal2_2 = new calObject();
        internal static calObject cal3_2 = new calObject();
        internal static calObject cal4_2 = new calObject();
        //internal static double r11 = 0, r12 = 0, r13 = 0, r14 = 0, r21 = 0, r22 = 0, r23 = 0, r24 = 0, r31 = 0, r32 = 0, r33 = 0, r34 = 1;//矩阵待求参数
        private Object FtpLogWriteLock = new Object();
        private Object WarningLock1 = new Object();
        private Object WarningLock2 = new Object();
        private Object WarningLock3 = new Object();
        internal static readonly Object OpenConfigLock = new Object();
        private readonly Object SystemLogWriteLockSta1 = new Object();
        private readonly Object SystemLogWriteLockSta2 = new Object();
        private readonly Object SystemLogWriteLockSta3 = new Object();
        private readonly Object SystemLogWriteLockSta4 = new Object();
        private readonly Object SystemLogWriteLockInductor = new Object();
        private readonly Object SystemLogWriteLockComm = new Object();
        private readonly Object SystemLogWriteLockFTP1 = new Object();
        private readonly Object SystemLogWriteLockFTP2 = new Object();
        private readonly Object SystemLogWriteLockFTP3 = new Object();
        private readonly Object SystemLogWriteLockFTP4 = new Object();
        private readonly Object SystemLogWriteLockFTP5 = new Object();
        private readonly Object SystemLogWriteLockFTP6 = new Object();
        private static readonly Object LogContentArrayLock1 = new Object();
        private static readonly Object LogContentArrayLock2 = new Object();
        private static readonly Object LogContentArrayLock3 = new Object();
        private static readonly Object LogContentArrayLock4 = new Object();
        private static readonly Object LogContentArrayLockInductor = new Object();
        private static readonly Object LogContentArrayLockComm = new Object();
        private static readonly Object LogContentArrayLockFTP1 = new Object();
        private static readonly Object LogContentArrayLockFTP2 = new Object();
        private static readonly Object LogContentArrayLockFTP3 = new Object();
        private static readonly Object LogContentArrayLockFTP4 = new Object();
        private static readonly Object LogContentArrayLockFTP5 = new Object();
        private static readonly Object LogContentArrayLockFTP6 = new Object();
        private readonly Object BottomStitchArrayLock1 = new Object();
        private readonly Object BottomStitchArrayLock2 = new Object();
        private readonly Object BottomStitchArrayLock3 = new Object();
        private readonly Object BottomStitchArrayLock4 = new Object();
        private readonly Object BottomStitchArrayLock5 = new Object();
        private readonly Object BarcodeStitchArrayLock1 = new Object();
        private readonly Object BarcodeStitchArrayLock2 = new Object();
        private readonly Object BarcodeStitchArrayLock3 = new Object();
        private readonly Object BarcodeStitchArrayLock4 = new Object();
        private readonly Object BarcodeStitchArrayLock5 = new Object();
        private readonly Object MasterArraySta1Lock = new Object();
        private readonly Object MasterArraySta2Lock = new Object();
        private readonly Object MasterArraySta3Lock = new Object();
        private readonly Object MasterArraySta4Lock = new Object();
        private readonly Object HTTPArraySta1_1Lock = new Object();
        private readonly Object HTTPArraySta1_2Lock = new Object();
        private readonly Object HTTPArraySta2_1Lock = new Object();
        private readonly Object HTTPArraySta2_2Lock = new Object();
        private readonly Object HTTPArraySta3_1Lock = new Object();
        private readonly Object HTTPArraySta3_2Lock = new Object();
        private readonly Object HTTPArraySta4_1Lock = new Object();
        private readonly Object HTTPArraySta4_2Lock = new Object();
        private readonly Object HTTPInfoSta1_1Lock = new Object();
        private readonly Object HTTPInfoSta1_2Lock = new Object();
        private readonly Object HTTPInfoSta2_1Lock = new Object();
        private readonly Object HTTPInfoSta2_2Lock = new Object();
        private readonly Object HTTPInfoSta3_1Lock = new Object();
        private readonly Object HTTPInfoSta3_2Lock = new Object();
        private readonly Object HTTPInfoSta4_1Lock = new Object();
        private readonly Object HTTPInfoSta4_2Lock = new Object();
        internal static string DoublePattern = @"^\d+(.\d+)?$";
        internal static string DaysPattern = @"^\d+$";
        private string NonSpacePattern = @"^\S+$";
        private string UrlPattern = @"^[C-Z]:(\\[^\\]+)*$";
        private string IPPattern = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";
        private string PortPattern = @"^\d{1,5}$";
        long HandleWaitTime = 1200;//ms
        internal static long InductorBindingTimeout = 200;//s
        internal static long BottomOnlyBindingTimeout = 30000;//s
        long deleteRawImage = 25000;//ftp根目录下图片的生命周期ms
        long deleteRawImageInductor = 10000;//ftp根目录下图片的生命周期ms
        long MstTimeOut = 20000;//MasterArray中MasterImage对象的生命周期ms
        long ExtCaptionBoxTimeOut1 = 5000;//ExtCaptionBoxList中对象过5秒后无论图片是否齐全均直接处理
        long ExtCaptionBoxTimeOut2 = 5000;//ExtCaptionBoxList中对象过5秒后无论图片是否齐全均直接处理
        long ExtCaptionBoxTimeOut3 = 5000;//ExtCaptionBoxList中对象过5秒后无论图片是否齐全均直接处理
        long ExtCaptionBoxTimeOut4 = 5000;//ExtCaptionBoxList中对象过5秒后无论图片是否齐全均直接处理
        long BottomStitchTimeOut = 5000;//BottomStitchArray中对象过5秒后无论图片是否齐全均直接处理
        long BarcodeStitchTimeOut = 5000;//BarcodeStitchArray中对象过5秒后无论图片是否齐全均直接处理
        long ExtCaptionBoxImgTimeOut = 5000;//ExtCaptionBoxList中对象过5秒后无论图片是否齐全均直接处理
        long ExternalCaptionInfoTimeout = 60000;//ExternalCaptionInfoList中对象60s后
        private bool allowPackageBorder = false;
        private static bool logEnable = true;
        private static bool BarcodeImgEnable = true;
        private static bool TopOnlyOnRead1 = false;
        private static bool TopOnlyOnRead2 = false;
        private static bool TopOnlyOnRead3 = false;
        private static bool TopOnlyOnRead4 = false;
        private bool uploadStatus = false;
        private bool upload = false;
        private bool upload2 = false;
        private bool httpUpload = false;
        private bool regexSta1Enable = false;
        private bool regexSta2Enable = false;
        private bool regexSta3Enable = false;
        private bool regexSta4Enable = false;
        private bool Sta1On = true;
        private bool Sta2On = false;
        private bool Sta3On = false;
        private bool Sta4On = false;
        private bool NoReadAnalysisEnable = false;
        private bool captionEnable = false;
        internal static bool extCaptionEnable = false;
        DateTime LastDeleteFileTimeStamp = DateTime.MinValue;
        private string DimAndWeightFormat;
        private string PosFormat;
        private string BarcodePosFormat;
        private string FtpKeepAlive = "false";
        private string CaptionFormat;
        private string FileNameFormat;
        private string FtpFileNameFormat;
        private string FtpFileNameFormat2;
        private string HttpFileNameFormat;
        private string DefaultFileNameFormat = "#N0_#N1_#N2_#N3_#N4_#N5_#N6_#N7_#N8_#N9_#N10_#N11";
        private string DefaultSubFolder = "#None";
        private static string inductor_sorter_temp_url;
        private string barcode_stitch_url1;
        private string barcode_stitch_url2;
        private string barcode_stitch_url3;
        private string barcode_stitch_url4;
        private string barcode_stitch_url5;
        private string stitch_url1;
        private string stitch_url2;
        private string stitch_url3;
        private string stitch_url4;
        private string stitch_url5;
        private static string ext_caption_url1;
        private static string ext_caption_url2;
        private static string ext_caption_url3;
        private static string ext_caption_url4;
        private string caption_url1;
        private string caption_url2;
        private string caption_url3;
        private string caption_url4;
        private string caption_url5;
        private string ftp_temp_url1;
        private string ftp_temp_url2;
        private string ftp_temp_url3;
        private string ftp_temp_url4;
        private string ftp_temp_url5;
        private string ftp_temp_url6;
        private string http_temp_url1;
        private string http_temp_url2;
        private string http_temp_url3;
        private string systemLog_dir = Application.StartupPath + "\\SystemLog";
        private string systemLog_Station1 = "Station1";
        private string systemLog_Station2 = "Station2";
        private string systemLog_Station3 = "Station3";
        private string systemLog_Station4 = "Station4";
        private string systemLog_Inductor = "Inductor";
        private string systemLog_Comm = "Communication";
        private string systemLog_FTP1 = "FTP1";
        private string systemLog_FTP2 = "FTP2";
        private string systemLog_FTP3 = "FTP3";
        private string systemLog_FTP4 = "FTP4";
        private string systemLog_FTP5 = "FTP5";
        private string systemLog_FTP6 = "FTP6";
        private string HttpCheckUrl;
        private string HttpUploadUrl;
        private string HttpRegexUrl;
        private string machineCodeFormat;
        private string regexMachineCode;
        private string disk;
        private string ftp_log_dir = Application.StartupPath + "\\FtpLog";
        internal static string cal_dir = Application.StartupPath + "\\calib";
        internal static string bottom_dir = Application.StartupPath + "\\bottomscan calibration";
        internal static string bottomcal_path_1 = bottom_dir + "\\bottom1.txt";
        internal static string bottomcal_path_2 = bottom_dir + "\\bottom2.txt";
        internal static string bottomcal_path_3 = bottom_dir + "\\bottom3.txt";
        internal static string bottomcal_path_4 = bottom_dir + "\\bottom4.txt";
        internal static string cal_path1_1 = cal_dir + "\\calib1-0.txt";
        internal static string cal_path2_1 = cal_dir + "\\calib2-0.txt";
        internal static string cal_path3_1 = cal_dir + "\\calib3-0.txt";
        internal static string cal_path4_1 = cal_dir + "\\calib4-0.txt";
        internal static string cal_path1_2 = cal_dir + "\\calib1-1.txt";
        internal static string cal_path2_2 = cal_dir + "\\calib2-1.txt";
        internal static string cal_path3_2 = cal_dir + "\\calib3-1.txt";
        internal static string cal_path4_2 = cal_dir + "\\calib4-1.txt";
        private string UploadMode = "ftp";
        private string InductorStation1No = "";
        private string InductorStation2No = "";
        private string[] MultiCodeDelimiter = new string[1];
        private string delimiter = "+_+_+";
        private int PicStorDays = 7;
        internal static string DefaultBindingTimeoutText = "200";
        private string DefaultPicStorDaysText = "7";
        private int LogFailureNum1 = 0;
        private int LogFailureNum2 = 0;
        private static ArrayList LogContentArray1 = new ArrayList();
        private static ArrayList LogContentArray2 = new ArrayList();
        private static ArrayList LogContentArray3 = new ArrayList();
        private static ArrayList LogContentArray4 = new ArrayList();
        private static ArrayList LogContentArrayInductor = new ArrayList();
        public static ArrayList LogContentArrayComm = new ArrayList();
        private static ArrayList LogContentArrayFTP1 = new ArrayList();
        private static ArrayList LogContentArrayFTP2 = new ArrayList();
        private static ArrayList LogContentArrayFTP3 = new ArrayList();
        private static ArrayList LogContentArrayFTP4 = new ArrayList();
        private static ArrayList LogContentArrayFTP5 = new ArrayList();
        private static ArrayList LogContentArrayFTP6 = new ArrayList();
        private ArrayList MasterArraySta1 = new ArrayList();
        private ArrayList MasterArraySta2 = new ArrayList();
        private ArrayList MasterArraySta3 = new ArrayList();
        private ArrayList MasterArraySta4 = new ArrayList();
        private List<BottomStitchObject> BottomStitchArray1 = new List<BottomStitchObject>();
        private List<BottomStitchObject> BottomStitchArray2 = new List<BottomStitchObject>();
        private List<BottomStitchObject> BottomStitchArray3 = new List<BottomStitchObject>();
        private List<BottomStitchObject> BottomStitchArray4 = new List<BottomStitchObject>();
        private List<BottomStitchObject> BottomStitchArray5 = new List<BottomStitchObject>();
        private List<BarcodeStitchObject> BarcodeStitchArray1 = new List<BarcodeStitchObject>();
        private List<BarcodeStitchObject> BarcodeStitchArray2 = new List<BarcodeStitchObject>();
        private List<BarcodeStitchObject> BarcodeStitchArray3 = new List<BarcodeStitchObject>();
        private List<BarcodeStitchObject> BarcodeStitchArray4 = new List<BarcodeStitchObject>();
        private List<BarcodeStitchObject> BarcodeStitchArray5 = new List<BarcodeStitchObject>();
        private ArrayList HTTPArraySta1_1 = new ArrayList();
        private ArrayList HTTPArraySta1_2 = new ArrayList();
        private ArrayList HTTPArraySta2_1 = new ArrayList();
        private ArrayList HTTPArraySta2_2 = new ArrayList();
        private ArrayList HTTPArraySta3_1 = new ArrayList();
        private ArrayList HTTPArraySta3_2 = new ArrayList();
        private ArrayList HTTPArraySta4_1 = new ArrayList();
        private ArrayList HTTPArraySta4_2 = new ArrayList();
        System.Timers.Timer Timer1 = new System.Timers.Timer();
        System.Timers.Timer Timer2 = new System.Timers.Timer();
        System.Timers.Timer Timer3 = new System.Timers.Timer();
        System.Timers.Timer Timer4 = new System.Timers.Timer();
        System.Timers.Timer Timer5 = new System.Timers.Timer();
        System.Timers.Timer Timer6 = new System.Timers.Timer();
        System.Timers.Timer Timer7 = new System.Timers.Timer();
        System.Timers.Timer Timer8 = new System.Timers.Timer();
        System.Timers.Timer Timer9 = new System.Timers.Timer();
        System.Timers.Timer Timer10 = new System.Timers.Timer();
        System.Timers.Timer Timer11 = new System.Timers.Timer();
        System.Timers.Timer Timer12 = new System.Timers.Timer();
        System.Timers.Timer Timer13 = new System.Timers.Timer();
        System.Timers.Timer Timer14 = new System.Timers.Timer();
        System.Timers.Timer Timer15 = new System.Timers.Timer();
        System.Timers.Timer Timer16 = new System.Timers.Timer();
        System.Timers.Timer Timer17 = new System.Timers.Timer();
        System.Timers.Timer Timer18 = new System.Timers.Timer();
        System.Timers.Timer Timer19 = new System.Timers.Timer();
        System.Timers.Timer Timer20 = new System.Timers.Timer();
        System.Timers.Timer Timer21 = new System.Timers.Timer();
        System.Timers.Timer Timer22 = new System.Timers.Timer();
        System.Timers.Timer Timer23 = new System.Timers.Timer();
        System.Timers.Timer Timer24 = new System.Timers.Timer();
        System.Timers.Timer Timer25 = new System.Timers.Timer();
        System.Timers.Timer Timer26 = new System.Timers.Timer();
        System.Timers.Timer Timer27 = new System.Timers.Timer();
        System.Timers.Timer Timer28 = new System.Timers.Timer();
        System.Timers.Timer Timer29 = new System.Timers.Timer();
        System.Timers.Timer Timer30 = new System.Timers.Timer();
        System.Timers.Timer Timer31 = new System.Timers.Timer();
        System.Timers.Timer Timer32 = new System.Timers.Timer();
        System.Timers.Timer Timer33 = new System.Timers.Timer();
        System.Timers.Timer Timer34 = new System.Timers.Timer();
        System.Timers.Timer Timer35 = new System.Timers.Timer();
        System.Timers.Timer Timer36 = new System.Timers.Timer();
        System.Timers.Timer Timer37 = new System.Timers.Timer();
        System.Timers.Timer Timer38 = new System.Timers.Timer();
        System.Timers.Timer Timer39 = new System.Timers.Timer();
        System.Timers.Timer Timer40 = new System.Timers.Timer();
        System.Timers.Timer Timer41 = new System.Timers.Timer();
        System.Timers.Timer Timer42 = new System.Timers.Timer();
        System.Timers.Timer Timer43 = new System.Timers.Timer();
        System.Timers.Timer Timer44 = new System.Timers.Timer();
        System.Timers.Timer Timer45 = new System.Timers.Timer();
        System.Timers.Timer Timer46 = new System.Timers.Timer();
        System.Timers.Timer Timer47 = new System.Timers.Timer();
        System.Timers.Timer Timer48 = new System.Timers.Timer();
        System.Timers.Timer Timer49 = new System.Timers.Timer();
        bool Timer1Running = false;
        bool Timer1Enabled = false;
        bool Timer1Finished = true;
        bool Timer2Running = false;
        bool Timer2Enabled = false;
        bool Timer2Finished = true;
        bool Timer3Running = false;
        bool Timer3Enabled = false;
        bool Timer3Finished = true;
        bool Timer4Running = false;
        bool Timer4Enabled = false;
        bool Timer4Finished = true;
        bool Timer5Running = false;
        bool Timer5Enabled = false;
        bool Timer5Finished = true;
        bool Timer6Running = false;
        bool Timer6Enabled = false;
        bool Timer6Finished = true;
        bool Timer7Running = false;
        bool Timer7Enabled = false;
        bool Timer7Finished = true;
        bool Timer8Running = false;
        bool Timer8Enabled = false;
        bool Timer8Finished = true;
        bool Timer9Running = false;
        bool Timer9Enabled = false;
        bool Timer9Finished = true;
        bool Timer10Running = false;
        bool Timer10Enabled = false;
        bool Timer10Finished = true;
        bool Timer11Running = false;
        bool Timer11Enabled = false;
        bool Timer11Finished = true;
        bool Timer12Running = false;
        bool Timer12Enabled = false;
        bool Timer12Finished = true;
        bool Timer13Running = false;
        bool Timer13Enabled = false;
        bool Timer13Finished = true;
        bool Timer14Running = false;
        bool Timer14Enabled = false;
        bool Timer14Finished = true;
        bool Timer15Running = false;
        bool Timer15Enabled = false;
        bool Timer15Finished = true;
        bool Timer16Running = false;
        bool Timer16Enabled = false;
        bool Timer16Finished = true;
        bool Timer17Running = false;
        bool Timer17Enabled = false;
        bool Timer17Finished = true;
        bool Timer18Running = false;
        bool Timer18Enabled = false;
        bool Timer18Finished = true;
        bool Timer19Running = false;
        bool Timer19Enabled = false;
        bool Timer19Finished = true;
        bool Timer20Running = false;
        bool Timer20Enabled = false;
        bool Timer20Finished = true;
        bool Timer21Running = false;
        bool Timer21Enabled = false;
        bool Timer21Finished = true;
        bool Timer22Running = false;
        bool Timer22Enabled = false;
        bool Timer22Finished = true;
        bool Timer23Running = false;
        bool Timer23Enabled = false;
        bool Timer23Finished = true;
        bool Timer24Running = false;
        bool Timer24Enabled = false;
        bool Timer24Finished = true;
        bool Timer25Running = false;
        bool Timer25Enabled = false;
        bool Timer25Finished = true;
        bool Timer26Running = false;
        bool Timer26Enabled = false;
        bool Timer26Finished = true;
        bool Timer27Running = false;
        bool Timer27Enabled = false;
        bool Timer27Finished = true;
        bool Timer28Running = false;
        bool Timer28Enabled = false;
        bool Timer28Finished = true;
        bool Timer29Running = false;
        bool Timer29Enabled = false;
        bool Timer29Finished = true;
        bool Timer30Running = false;
        bool Timer30Enabled = false;
        bool Timer30Finished = true;
        bool Timer31Running = false;
        bool Timer31Enabled = false;
        bool Timer31Finished = true;
        bool Timer32Running = false;
        bool Timer32Enabled = false;
        bool Timer32Finished = true;
        bool Timer33Running = false;
        bool Timer33Enabled = false;
        bool Timer33Finished = true;
        bool Timer34Running = false;
        bool Timer34Enabled = false;
        bool Timer34Finished = true;
        bool Timer35Running = false;
        bool Timer35Enabled = false;
        bool Timer35Finished = true;
        bool Timer36Running = false;
        bool Timer36Enabled = false;
        bool Timer36Finished = true;
        bool Timer37Running = false;
        bool Timer37Enabled = false;
        bool Timer37Finished = true;
        bool Timer38Running = false;
        bool Timer38Enabled = false;
        bool Timer38Finished = true;
        bool Timer39Running = false;
        bool Timer39Enabled = false;
        bool Timer39Finished = true;
        bool Timer40Running = false;
        bool Timer40Enabled = false;
        bool Timer40Finished = true;
        bool Timer41Running = false;
        bool Timer41Enabled = false;
        bool Timer41Finished = true;
        bool Timer42Running = false;
        bool Timer42Enabled = false;
        bool Timer42Finished = true;
        bool Timer43Running = false;
        bool Timer43Enabled = false;
        bool Timer43Finished = true;
        bool Timer44Running = false;
        bool Timer44Enabled = false;
        bool Timer44Finished = true;
        bool Timer45Running = false;
        bool Timer45Enabled = false;
        bool Timer45Finished = true;
        bool Timer46Running = false;
        bool Timer46Enabled = false;
        bool Timer46Finished = true;
        bool Timer47Running = false;
        bool Timer47Enabled = false;
        bool Timer47Finished = true;
        bool Timer48Running = false;
        bool Timer48Enabled = false;
        bool Timer48Finished = true;
        bool Timer49Running = false;
        bool Timer49Enabled = false;
        bool Timer49Finished = true;
        private bool warningflag1 = false;
        private bool warningflag2 = false;
        private bool UpLoadOK = true;
        private NetworkCredential Credential;
        private NetworkCredential Credential2;
        private int Timer1TaskRunning = 0;
        private int Timer2TaskRunning = 0;
        private int Timer3TaskRunning = 0;
        private int Timer4TaskRunning = 0;
        private int Timer5TaskRunning = 0;
        private int Timer6TaskRunning = 0;
        private int Timer7TaskRunning = 0;
        private int Timer8TaskRunning = 0;
        private int Timer9TaskRunning = 0;
        private int Timer10TaskRunning = 0;
        private int Timer11TaskRunning = 0;
        private int Timer12TaskRunning = 0;
        private int Timer13TaskRunning = 0;
        private int Timer14TaskRunning = 0;
        private int Timer15TaskRunning = 0;
        private int Timer16TaskRunning = 0;
        private int Timer17TaskRunning = 0;
        private int Timer18TaskRunning = 0;
        private int Timer19TaskRunning = 0;
        private int Timer20TaskRunning = 0;
        private int Timer21TaskRunning = 0;
        private int Timer22TaskRunning = 0;
        private int Timer23TaskRunning = 0;
        private int Timer24TaskRunning = 0;
        private int Timer25TaskRunning = 0;
        private int Timer26TaskRunning = 0;
        private int Timer27TaskRunning = 0;
        private int Timer28TaskRunning = 0;
        private int Timer29TaskRunning = 0;
        private int Timer30TaskRunning = 0;
        private int Timer31TaskRunning = 0;
        private int Timer32TaskRunning = 0;
        private int Timer33TaskRunning = 0;
        private int Timer34TaskRunning = 0;
        private int Timer35TaskRunning = 0;
        private int Timer36TaskRunning = 0;
        private int Timer37TaskRunning = 0;
        private int Timer38TaskRunning = 0;
        private int Timer39TaskRunning = 0;
        private int Timer40TaskRunning = 0;
        private int Timer41TaskRunning = 0;
        private int Timer42TaskRunning = 0;
        private int Timer43TaskRunning = 0;
        private int Timer44TaskRunning = 0;
        private int Timer45TaskRunning = 0;
        private int Timer46TaskRunning = 0;
        private int Timer47TaskRunning = 0;
        private int Timer48TaskRunning = 0;
        private int Timer49TaskRunning = 0;
        private string SubFolder1;
        private string SubFolder2;
        private string SubFolder3;
        private string SubFolder4;
        private string SubFolder5;
        private string FtpSubFolder1;
        private string FtpSubFolder2;
        private string FtpSubFolder3;
        private string FtpSubFolder4;
        private string FtpSubFolder5;
        private string[] SubFolders = new string[5];
        private string[] FtpRoots = new string[4];
        private string[] StorageRoots = new string[4];
        private string User;
        private string Password;
        private string regexMasterIP1;
        private string regexMasterPort1;
        private string regexMasterIP2;
        private string regexMasterPort2;
        private string regexMasterIP3;
        private string regexMasterPort3;
        private string regexMasterIP4;
        private string regexMasterPort4;
        private string ServerIP;
        private string FTPPort;
        private string User2;
        private string Password2;
        private string ServerIP2;
        private string FTPPort2;
        private string SCADAIP;
        private string SCADAPort;
        private string FTPOKMes = "FTP OK";
        private string FTPNOKMes = "FTP NOK";
        private string HTTPOKMes = "HTTP OK";
        private string HTTPNOKMes = "HTTP NOK";
        private int CamCount1 = 5;
        private int CamCount2 = 5;
        private int CamCount3 = 5;
        private int CamCount4 = 5;
        private int BottomCamCount1 = 0;
        private int BottomCamCount2 = 0;
        private int BottomCamCount3 = 0;
        private int BottomCamCount4 = 0;
        private int TotalCamCount1 = 5;
        private int TotalCamCount2 = 5;
        private int TotalCamCount3 = 5;
        private int TotalCamCount4 = 5;
        private string StorageRoot1;
        private string StorageRoot2;
        private string StorageRoot3;
        private string StorageRoot4;
        private string InductFtpRoot;//供包台存图地址
        private string FtpRoot1;//相机ftp存图地址，需要所有相机都存在这个地址
        private string FtpRoot2;
        private string FtpRoot3;
        private string FtpRoot4;
        private string BarcodeImgMode = "不拼接";
        private string InductID1 = "禁用融合";//对应的供包台区域
        private string InductID2 = "禁用融合";
        private string InductID3 = "禁用融合";
        private string InductID4 = "禁用融合";
        private string SorterStationNoFormat = "#N2";
        private string SorterResultFormat = "#N3";
        private string SorterBarcodeFormat = "#N4";
        private string SorterTrayIDFormat = "#N5";
        private string SorterTimeStampFormat = "#N6";
        private static string SorterBoxIDFormat = "#N7";
        private string startup_dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
        private string startup_path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup), "存图助手标准版.lnk");
        private string target_dir = Application.StartupPath;
        private string target_path = Application.StartupPath + @"\存图助手标准版.exe";
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;  // 若发送较小数据可用此字段传输
            public int cbData;     // 发送数据的大小（以字节为单位）
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;  // 发送数据
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll", EntryPoint = "GetWindowThreadProcessId")]
        public static extern int GetWindowThreadProcessID(IntPtr hwnd, out int processid);
        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        public static extern int SendMessage(IntPtr hWnd, int msg, int wParam, ref COPYDATASTRUCT lParam);
        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "IsWindow")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindow(IntPtr hWnd);
        private static Process findDWSInterface(Process[] p)
        {
            foreach (Process pro in p)
            {
                try
                {
                    if (pro.ProcessName == "DWSInterface")
                    {
                        return pro;
                    }
                }
                catch { }
            }
            return null;
        }
        private static void MonitorDWSProcess()
        {
            while (true)
            {
                
                //IntPtr ppp = FindWindow(null, "Yaki");
                
                Process[] processes = Process.GetProcessesByName("DWSInterface");
                if (DWSInterfaceEnabled)
                {
                    Process process = null;
                    if (processes.Length > 0)
                    {
                        process = processes[0];
                    }
                    if (process == null)
                    {
                        process = new Process();
                        try
                        {
                            process.StartInfo.FileName = Application.StartupPath + "\\DWSInterface.exe";
                            //process.StartInfo.FileName = "../../../../DWSInterface/bin/Debug/DWSInterface.exe";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = false;
                            process.Start();
                            DateTime starttime = DateTime.Now;
                            while (FindWindow(null, "DWSInterfaceHerbert") == IntPtr.Zero && (DateTime.Now - starttime).TotalMilliseconds <= 5000)
                            {

                            }
                            if (FindWindow(null, "DWSInterfaceHerbert") != IntPtr.Zero)
                            {
                                lock (hWndLock)
                                {
                                    hWnd = FindWindow(null, "DWSInterfaceHerbert");
                                }
                            }
                            else
                            {
                                lock (hWndLock)
                                {
                                    hWnd = IntPtr.Zero;
                                }
                                process.Kill();
                            }
                        }
                        catch (Exception ex) { }
                        finally
                        {
                            if (process != null)
                            {
                                process.Dispose();
                            }
                        }
                    }
                    else
                    {
                        DateTime starttime = DateTime.Now;
                        try
                        {
                            while (FindWindow(null, "DWSInterfaceHerbert") == IntPtr.Zero && (DateTime.Now - starttime).TotalMilliseconds <= 5000)
                            {

                            }
                            if (FindWindow(null, "DWSInterfaceHerbert") != IntPtr.Zero)
                            {
                                lock (hWndLock)
                                {
                                    if (hWnd != FindWindow(null, "DWSInterfaceHerbert"))
                                    {
                                        hWnd = FindWindow(null, "DWSInterfaceHerbert");
                                    }
                                }
                            }
                            else
                            {
                                lock (hWndLock)
                                {
                                    hWnd = IntPtr.Zero;
                                }
                                process.Kill();
                            }
                        }
                        catch { }
                    }
                    foreach (Process pro in processes)
                    {
                        if (pro != null)
                        {
                            pro.Dispose();
                        }
                    }
                    Thread.Sleep(2900);
                }
                else
                {
                    foreach(Process p in processes)
                    {
                        if (p != null)
                        {
                            p.Kill();
                            p.Dispose();
                        }
                    }
                }
                Thread.Sleep(100);
                FileStream fs = null;
                StreamWriter sr = null;
                try
                {
                    //fs = new FileStream("D:\\log3.txt", FileMode.Append, FileAccess.Write);
                    //sr = new StreamWriter(fs);
                    //sr.WriteLine("Winform:" + hWnd.ToString());
                }
                catch { }
                finally
                {
                    if (sr != null)
                    {
                        sr.Dispose();
                    }
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }
        }
        private static void MonitorServer()
        {
            int cnt = 0;
            int mesid = 0;
            while (true)
            {
                cnt++;
                if (cnt == 5)
                {
                    cnt = 0;
                }
                try
                {
                    if (LocalServerHandler != null)
                    {
                        if (!LocalServerHandler.Disposed && !LocalServerHandler.Listening)
                        {
                            LocalServerHandler.Start();
                        }
                        else if(!LocalServerHandler.Disposed)
                        {
                            if (cnt == 0)
                            {
                                mesid++;
                                if (mesid == 10000)
                                {
                                    mesid = 0;
                                }
                                string mesidtext = "0000" + mesid;
                                mesidtext = mesidtext.Substring(mesidtext.Length - 4);
                                string hbtext = "\x02" + mesidtext + "\x3B\x48\x3B\x03";
                                LocalServerHandler.Send(hbtext, ExtCaptionIP);
                            }
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception ex) { }
            }
        }
        public void ReadBottomCalib(Mat[] Homography, int[] ol, string path)
        {
            if (Homography.Length == 5)
            {
                StreamReader sr = null;
                try
                {
                    if (!Directory.Exists(bottom_dir))
                    {
                        Directory.CreateDirectory(bottom_dir);
                    }
                    if (System.IO.File.Exists(path))
                    {

                        sr = new StreamReader(path);
                        string line = sr.ReadLine();
                        sr.Close();
                        if (line != null)
                        {
                            JObject Jo = (JObject)JsonConvert.DeserializeObject(line);
                            if (Jo != null)
                            {
                                if (Jo.Property("H12") != null)
                                {
                                    double[] H12array = ((JArray)Jo["H12"]).ToObject<double[]>();
                                    //Homography[0] = new Mat(3, 3, MatType.CV_64FC1, H12array);
                                    Homography[0] = Mat.FromPixelData(3, 3, MatType.CV_64FC1, H12array);
                                }
                                if (Jo.Property("H23") != null)
                                {
                                    double[] H23array = ((JArray)Jo["H23"]).ToObject<double[]>();
                                    //Homography[1] = new Mat(3, 3, MatType.CV_64FC1, H23array);
                                    Homography[1] = Mat.FromPixelData(3, 3, MatType.CV_64FC1, H23array);
                                }
                                if (Jo.Property("H34") != null)
                                {
                                    double[] H34array = ((JArray)Jo["H34"]).ToObject<double[]>();
                                    //Homography[2] = new Mat(3, 3, MatType.CV_64FC1, H34array);
                                    Homography[2] = Mat.FromPixelData(3, 3, MatType.CV_64FC1, H34array);
                                }
                                if (Jo.Property("H45") != null)
                                {
                                    double[] H45array = ((JArray)Jo["H45"]).ToObject<double[]>();
                                    //Homography[3] = new Mat(3, 3, MatType.CV_64FC1, H45array);
                                    Homography[3] = Mat.FromPixelData(3, 3, MatType.CV_64FC1, H45array);
                                }
                                if (Jo.Property("H56") != null)
                                {
                                    double[] H56array = ((JArray)Jo["H56"]).ToObject<double[]>();
                                    //Homography[4] = new Mat(3, 3, MatType.CV_64FC1, H56array);
                                    Homography[4] = Mat.FromPixelData(3, 3, MatType.CV_64FC1, H56array);
                                }
                                if (Jo.Property("overlap") != null)
                                { 
                                    int[] overlap = ((JArray)Jo["overlap"]).ToObject<int[]>();
                                    if (overlap.Length == ol.Length)
                                    {
                                        for (int i = 0; i < ol.Length; i++)
                                        {
                                            ol[i] = overlap[i];
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                finally
                {
                    if (sr != null)
                    {
                        sr.Dispose();
                    }
                }
            }
        }

        public void ReadCalib(calObject calobj, string path)
        {
            bool calReadOK = false;
            StreamReader sr = null;
            try
            {
                if (!Directory.Exists(cal_dir))
                {
                    Directory.CreateDirectory(cal_dir);
                }
                if (System.IO.File.Exists(path))
                {

                    sr = new StreamReader(path);
                    string line = sr.ReadLine();
                    sr.Close();
                    if (line != null)
                    {
                        JObject Jo = (JObject)JsonConvert.DeserializeObject(line);
                        if (Jo != null)
                        {
                            if (Jo.Property("r11") != null && Jo.Property("r12") != null && Jo.Property("r13") != null && Jo.Property("r14") != null && Jo.Property("r21") != null && Jo.Property("r22") != null && Jo.Property("r23") != null && Jo.Property("r24") != null && Jo.Property("r31") != null && Jo.Property("r32") != null && Jo.Property("r33") != null)
                            {
                                calobj.r11 = Jo.Value<double>("r11");
                                calobj.r12 = Jo.Value<double>("r12");
                                calobj.r13 = Jo.Value<double>("r13");
                                calobj.r14 = Jo.Value<double>("r14");
                                calobj.r21 = Jo.Value<double>("r21");
                                calobj.r22 = Jo.Value<double>("r22");
                                calobj.r23 = Jo.Value<double>("r23");
                                calobj.r24 = Jo.Value<double>("r24");
                                calobj.r31 = Jo.Value<double>("r31");
                                calobj.r32 = Jo.Value<double>("r32");
                                calobj.r33 = Jo.Value<double>("r33");
                                calobj.calOK = calobj.r11 != 0 || calobj.r12 != 0 || calobj.r13 != 0 || calobj.r14 != 0 || calobj.r21 != 0 || calobj.r22 != 0 || calobj.r23 != 0 || calobj.r24 != 0 || calobj.r31 != 0 || calobj.r32 != 0 || calobj.r33 != 0;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            { 
            
            }
            finally
            {
                if (sr != null)
                {
                    sr.Dispose();
                }
            }

        }
        private void InitializeIndexObjects(IndexInfo[] IndexObjects, int InductID)
        {
            for (int i = 0; i < IndexObjects.Length; i++)
            {
                IndexObjects[i] = new IndexInfo(i + 1, InductID);
            }
        }
        internal static void InitializeTray(Tray tray, bool deleteflag)
        {
            if (tray != null)
            {
                if (deleteflag)
                {
                    for (int i = 0; i < tray.Images.Length; i++)
                    {
                        if (tray.Images[i] != null)
                        {
                            tray.Images[i].Refresh();
                            if (tray.Images[i].Exists && tray.Images[i].DirectoryName == inductor_sorter_temp_url)
                            {
                                try
                                {
                                    tray.Images[i].Delete();
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                }
                tray.BottomTimestamp = DateTime.MinValue;//一个新的货物上车，该小车信息重置
                tray.InductID = 0;
                tray.BottomCodes = "";
                tray.BottomResult = "";
                tray.BoxID = "000000";
                tray.SorterTimestamp = DateTime.MinValue;
                tray.CurrentImageInductID = 0;
                tray.SorterCodes = "";
                tray.SorterResult = "";
                tray.N0 = "-1";
                tray.N2 = "xx";
                tray.N3 = "xx";
                tray.N4 = "xx";
                tray.N5 = "xx";
                tray.N6 = "xx";
                tray.N7 = "xx";
                tray.N8 = "xx";
                tray.N9 = "xx";
                tray.N10 = "xx";
                tray.N11 = "xx";
                tray.N12 = "-1";
                tray.Images[0] = null;
                tray.Images[1] = null;
                tray.Images[2] = null;
                tray.Images[3] = null;
                tray.Images[4] = null;
                tray.Images[5] = null;
                tray.Uploaded[0] = false;
                tray.Uploaded[1] = false;
                tray.Uploaded[2] = false;
                tray.Uploaded[3] = false;
                tray.Uploaded[4] = false;
                tray.Uploaded[5] = false;
            }
        }
        /*
        public void InitializeTrayObjects()
        {
            for (int i = 0; i < TrayObjects.Length; i++)
            {
                TrayObjects[i] = new Tray(i + 1);
            }
        }*/
        internal static void OnConnect(TcpClientBase client)
        {
            if (client.Equals(MainForm.DWS1Handler))
            {
                client.HandlerName = "DWS1Handler: ";
            }
            else if (client.Equals(MainForm.Bottom11Handler))
            {
                client.HandlerName = "Bottom11Server: ";
            }
            else if (client.Equals(MainForm.Bottom12Handler))
            {
                client.HandlerName = "Bottom12Server: ";
            }
            else if (client.Equals(MainForm.Bottom13Handler))
            {
                client.HandlerName = "Bottom13Server: ";
            }
            else if (client.Equals(MainForm.Bottom14Handler))
            {
                client.HandlerName = "Bottom14Server: ";
            }
            else if (client.Equals(MainForm.Bottom15Handler))
            {
                client.HandlerName = "Bottom15Server: ";
            }
            else if (client.Equals(MainForm.Bottom21Handler))
            {
                client.HandlerName = "Bottom21Server: ";
            }
            else if (client.Equals(MainForm.Bottom22Handler))
            {
                client.HandlerName = "Bottom22Server: ";
            }
            else if (client.Equals(MainForm.Bottom23Handler))
            {
                client.HandlerName = "Bottom23Server: ";
            }
            else if (client.Equals(MainForm.Bottom24Handler))
            {
                client.HandlerName = "Bottom24Server: ";
            }
            else if (client.Equals(MainForm.Bottom25Handler))
            {
                client.HandlerName = "Bottom25Server: ";
            }
            else
            {
                client.HandlerName = "UnexpectedServer: ";
            }
            client.ExpectFramed("\x02", "\x03", 1024);
        }
        internal static void logmsg(TcpClientBase client, string message)
        {
            { 
                AddLog(message.Replace("\x02","<STX>").Replace("\x03","<ETX>").Replace("\r","\\r").Replace("\n","\\n"), LogContentArrayComm);
            }
        }
        internal static void OnExpectedData(TcpClientBase client, string currentmessage)//和底扫通讯，软件是客户端
        {
            try
            {
                string plcip = "";
                IndexInfo[] IndexObjects = null;
                if (client == null)
                {
                    return;
                }
                else if (!client.Equals(DWS1Handler))
                {
                    if (client.Equals(Bottom11Handler))
                    {
                        plcip = Plc11IP;
                        IndexObjects = IndexObjects11;
                    }
                    else if (client.Equals(Bottom12Handler))
                    {
                        plcip = Plc12IP;
                        IndexObjects = IndexObjects12;
                    }
                    else if (client.Equals(Bottom13Handler))
                    {
                        plcip = Plc13IP;
                        IndexObjects = IndexObjects13;
                    }
                    else if (client.Equals(Bottom14Handler))
                    {
                        plcip = Plc14IP;
                        IndexObjects = IndexObjects14;
                    }
                    else if (client.Equals(Bottom15Handler))
                    {
                        plcip = Plc15IP;
                        IndexObjects = IndexObjects15;
                    }
                    else if (client.Equals(Bottom21Handler))
                    {
                        plcip = Plc21IP;
                        IndexObjects = IndexObjects21;
                    }
                    else if (client.Equals(Bottom22Handler))
                    {
                        plcip = Plc22IP;
                        IndexObjects = IndexObjects22;
                    }
                    else if (client.Equals(Bottom23Handler))
                    {
                        plcip = Plc23IP;
                        IndexObjects = IndexObjects23;
                    }
                    else if (client.Equals(Bottom24Handler))
                    {
                        plcip = Plc24IP;
                        IndexObjects = IndexObjects24;
                    }
                    else if (client.Equals(Bottom25Handler))
                    {
                        plcip = Plc25IP;
                        IndexObjects = IndexObjects25;
                    }
                    else
                    {
                        plcip = "";
                        IndexObjects = null;
                    }
                    /*
                    if (plcip != "" && LocalServerHandler != null)
                    {
                        if (!LocalServerHandler.Disposed)
                        {
                            try
                            {
                                LocalServerHandler.Send("\x02" + currentmessage + "\x03", plcip);
                            }
                            catch (Exception ex) { }
                        }
                    }
                    */
                    string[] mes = currentmessage.Split('#');
                    if (mes.Length == 3 && IndexObjects != null)//id+code+timestamp
                    {
                        if (plcip != "" && LocalServerHandler != null)
                        {
                            if (!LocalServerHandler.Disposed)
                            {
                                try
                                {
                                    //LocalServerHandler.Send("\x02" + currentmessage + "\x03", plcip);
                                    LocalServerHandler.Send("\x02" + mes[0] + "#" + mes[1] + "\x03", plcip);
                                }
                                catch (Exception ex) { }
                            }
                        }
                        int id = Convert.ToInt32(mes[0]) - 1;
                        string boxid = "000000";
                        DateTime bottomts = DateTime.MinValue;
                        string bottomcodes = "";
                        string bottomresult = "";
                        if (id >= 0 && id < IndexObjects.Length)
                        {
                            lock (IndexObjects[id].InductInfoLock)
                            {
                                if (IndexObjects[id].BottomTimestamp == DateTime.MinValue)
                                {
                                    boxid = IndexObjects[id].BoxID;
                                    bottomcodes = mes[1].Replace(@"&", @"+");
                                    IndexObjects[id].BottomCodes = mes[1].Replace(@"&", @"+");
                                    var format = "yyyyMMddHHmmssfff";
                                    IndexObjects[id].BottomTimestamp = DateTime.ParseExact(mes[2], format, CultureInfo.InvariantCulture);
                                    bottomts = DateTime.ParseExact(mes[2], format, CultureInfo.InvariantCulture);
                                    if (mes[1].ToLower() != "noread" && mes[1] != "")
                                    {
                                        if (mes[1].IndexOf("&") >= 0)
                                        {
                                            bottomresult = "MR";
                                            IndexObjects[id].BottomResult = "MR";
                                        }
                                        else
                                        {
                                            bottomresult = "GR";
                                            IndexObjects[id].BottomResult = "GR";
                                        }
                                    }
                                    else
                                    {
                                        bottomresult = "NR";
                                        IndexObjects[id].BottomResult = "NR";
                                    }
                                }
                            }
                            if (boxid != "000000")
                            {
                                int index = -1;
                                Box box = null;
                                lock (BoxListLock)
                                {
                                    index = FindBoxListIndex(boxid);
                                    if (index > -1)
                                    {
                                        box = BoxList[index];
                                    }
                                }
                                if (box != null)
                                {
                                    lock (box.BoxLock)
                                    {
                                        if (box.BottomTimestamp == DateTime.MinValue)
                                        {
                                            box.BottomTimestamp = bottomts;
                                        }
                                        if (box.BottomCodes == "")
                                        {
                                            box.BottomCodes = bottomcodes;
                                        }
                                        if (box.BottomResult == "")
                                        {
                                            box.BottomResult = bottomresult;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    /*                  
                        * DWS消息：
                        * {
                        *     "Barcode" : "SF123",
                        *     "Length"  : 100,
                        *     "Width"   : 100,
                        *     "Height"  : 100,
                        *     "Weight"  : 100,
                        *     "Volume"  : 100000,
                        *     "ImageFullName" : @"D:\2024\09\01\12\UP01.jpg",
                        *     "ReadStatus"   : true,
                        *     "InfoIntact"   : true,
                        *     "ParcelID" : "00001"，
                        *     "TriggerOnTimeStamp": "yyyyMMddHHmmssfff"
                        * }
                    */
                    COPYDATASTRUCT cds;
                    cds.dwData = (IntPtr)0;
                    cds.cbData = System.Text.Encoding.Default.GetBytes(currentmessage).Length + 1;
                    cds.lpData = currentmessage;
                    lock (hWndLock)
                    {
                        if (hWnd != IntPtr.Zero)
                        {
                            if (IsWindow(hWnd))
                            {
                                //SetForegroundWindow(hWnd);
                                //Thread.Sleep(5);
                                SendMessage(hWnd, 0x004A, 0, ref cds);
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }
        internal static void OnConnect(ClientConnection connection)
        {
            if (connection != null)
            {
                if (connection.RemoteIP == MainForm.Plc11IP)
                {
                    connection.HandlerName = "PLC11Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc12IP)
                {
                    connection.HandlerName = "PLC12Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc13IP)
                {
                    connection.HandlerName = "PLC13Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc14IP)
                {
                    connection.HandlerName = "PLC14Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc15IP)
                {
                    connection.HandlerName = "PLC15Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc21IP)
                {
                    connection.HandlerName = "PLC21Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc22IP)
                {
                    connection.HandlerName = "PLC22Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc23IP)
                {
                    connection.HandlerName = "PLC23Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc24IP)
                {
                    connection.HandlerName = "PLC24Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.Plc25IP)
                {
                    connection.HandlerName = "PLC25Client: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                else if (connection.RemoteIP == MainForm.ExtCaptionIP)
                {
                    connection.HandlerName = "ExtCaptionClient: ";
                    connection.ExpectFramed("\x02", "\x03", 1024);
                }
                /*else if (connection.RemoteIP == MainForm.DWS1IP)
                {
                    connection.HandlerName = "DWS1Client: ";
                    connection.ExpectFramed("\x02", "\x03", 1024);
                }*/
                else
                {
                    connection.HandlerName = "UnexpectedClient: ";
                    connection.ExpectFramed("\x02", "\r\n", 1024);
                }
                //connection.ExpectFramed("\x02", "\r\n", 1024);
            }
        }
        internal static void logmsg(string message, ClientConnection connection)
        {
            AddLog(message.Replace("\x02", "<STX>").Replace("\x03", "<ETX>").Replace("\r", "\\r").Replace("\n", "\\n"), LogContentArrayComm);
        }
        internal static void OnExpectedData(string currentmessage, ClientConnection connection)//和PLC通讯，软件是服务器
        {
            try
            {
                bool extcaptionflag = false;
                bool dwsflag = false;
                SocketLib.TcpClientBase client = null;
                IndexInfo[] IndexObjects = null;
                if (connection == null)
                {
                    return;
                }
                if (connection.RemoteIP == Plc11IP)
                {
                    client = Bottom11Handler;
                    IndexObjects = IndexObjects11;
                }
                else if (connection.RemoteIP == Plc12IP)
                {
                    client = Bottom12Handler;
                    IndexObjects = IndexObjects12;
                }
                else if (connection.RemoteIP == Plc13IP)
                {
                    client = Bottom13Handler;
                    IndexObjects = IndexObjects13;
                }
                else if (connection.RemoteIP == Plc14IP)
                {
                    client = Bottom14Handler;
                    IndexObjects = IndexObjects14;
                }
                else if (connection.RemoteIP == Plc15IP)
                {
                    client = Bottom15Handler;
                    IndexObjects = IndexObjects15;
                }
                else if (connection.RemoteIP == Plc21IP)
                {
                    client = Bottom21Handler;
                    IndexObjects = IndexObjects21;
                }
                else if (connection.RemoteIP == Plc22IP)
                {
                    client = Bottom22Handler;
                    IndexObjects = IndexObjects22;
                }
                else if (connection.RemoteIP == Plc23IP)
                {
                    client = Bottom23Handler;
                    IndexObjects = IndexObjects23;
                }
                else if (connection.RemoteIP == Plc24IP)
                {
                    client = Bottom24Handler;
                    IndexObjects = IndexObjects24;
                }
                else if (connection.RemoteIP == Plc25IP)
                {
                    client = Bottom25Handler;
                    IndexObjects = IndexObjects25;
                }
                else
                {
                    if (connection.RemoteIP == ExtCaptionIP)
                    {
                        extcaptionflag = true;
                    }
                    /*else if (connection.RemoteIP == DWS1IP)
                    {
                        dwsflag = true;
                    }*/
                    client = null;
                    IndexObjects = null;
                }
                if (client != null && IndexObjects != null)
                {
                    if (!client.Disposed)
                    {
                        if (!currentmessage.StartsWith("\x54") && currentmessage.Length == 11)//id+BoxID
                        {
                            int id = Convert.ToInt32(currentmessage.Substring(0, 4)) - 1;
                            string boxid = currentmessage.Substring(4, 6);
                            if (id >= 0 && id < IndexObjects.Length)//重置
                            {
                                lock (IndexObjects[id].InductInfoLock)
                                {
                                    IndexObjects[id].BottomCodes = "";
                                    IndexObjects[id].BottomTimestamp = DateTime.MinValue;
                                    IndexObjects[id].BottomResult = "";
                                    IndexObjects[id].BottomImage = null;
                                    IndexObjects[id].TrayID = 9999;
                                    IndexObjects[id].BoxID = boxid;
                                }
                                if (boxid != "000000")
                                {
                                    int index = -1;
                                    lock (BoxListLock)
                                    {
                                        index = FindBoxListIndex(boxid);
                                        if (index > -1)
                                        {
                                            BoxList.RemoveAt(index);
                                        }
                                    }
                                    if (index < 0)
                                    {
                                        Box box = new Box(boxid, IndexObjects[id].InductID);
                                        lock (BoxListLock)
                                        {
                                            BoxList.Add(box);
                                        }
                                    }
                                }
                            }
                            try
                            {
                                client.Send("\x02" + currentmessage + "\r\n");
                                //client.Send("\x02" + Convert.ToInt32(currentmessage.Substring(0, 4)) + "\x03\r\n");
                            }
                            catch (Exception ex) { }

                        }
                        else if (currentmessage.StartsWith("\x54"))//id+Trayid
                        {
                            int id = Convert.ToInt32(currentmessage.Substring(1, 4)) - 1;
                            int trayid = Convert.ToInt32(currentmessage.Substring(5, 4));
                            string boxid = "000000";
                            if (id >= 0 && id < IndexObjects.Length)
                            {
                                lock (IndexObjects[id].InductInfoLock)
                                {
                                    if (IndexObjects[id].TrayID == 9999 && ((DateTime.Now - IndexObjects[id].BottomTimestamp).TotalSeconds < 10 || IndexObjects[id].BottomTimestamp == DateTime.MinValue))
                                    {
                                        boxid = IndexObjects[id].BoxID;
                                        IndexObjects[id].TrayID = trayid;
                                    }
                                }
                                if (boxid != "000000")
                                {
                                    int index = -1;
                                    Box box = null;
                                    lock (BoxListLock)
                                    {
                                        index = FindBoxListIndex(boxid);
                                        if (index > -1)
                                        {
                                            box = BoxList[index];
                                        }
                                    }
                                    if (box != null)
                                    {
                                        lock (box.BoxLock)
                                        {
                                            if (box.TrayID == 9999)
                                            {
                                                box.TrayID = trayid;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else if (extcaptionflag)
                {
                    string[] items = currentmessage.Split('\x3B');
                    if (items.Length >= 9)
                    {
                        if (items[1] == "\x57")
                        {
                            string boxid = items[2].Replace("\x00", "\x30");
                            string weight = items[5];
                            string length = items[6];
                            string width = items[7];
                            string height = items[8];
                            ExternalCaptionInfo info = new ExternalCaptionInfo(boxid, weight + ";" + length + ";" + width + ";" + height);
                            lock (ExternalCaptionInfoListLock)
                            {
                                ExternalCaptionInfoList.Add(info);
                            }
                        }
                    }
                }
                /*                  
                    * DWS消息：
                    * {
                    *     "Barcode" : "SF123",
                    *     "Length"  : 100,
                    *     "Width"   : 100,
                    *     "Height"  : 100,
                    *     "Weight"  : 100,
                    *     "Volume"  : 100000,
                    *     "ImageFullName" : @"D:\2024\09\01\12\UP01.jpg",
                    *     "ReadStatus"   : true,
                    *     "InfoIntact"   : true
                    * }
                */
                /*else if (dwsflag)//dws消息
                {
                    if (DWSInterfaceEnabled)
                    {
                        
                        COPYDATASTRUCT cds;
                        cds.dwData = (IntPtr)0;
                        cds.cbData = System.Text.Encoding.Default.GetBytes(currentmessage).Length + 1;
                        cds.lpData = currentmessage;
                        lock (hWndLock)
                        {
                            if (hWnd != IntPtr.Zero)
                            {
                                if (IsWindow(hWnd))
                                {
                                    SendMessage(hWnd, 0x004A, 0, ref cds);
                                }
                            }
                        }
                    }
                }*/
            }
            catch (Exception ex) { }
        }
        public MainForm()
        {
            try
            {
                InitializeComponent();
                imgLockPSW = readPSW();
                imgLockIV = readIV();
                PowerOnTime = DateTime.Now;
                running = true;
                //ind.Show();
                gs.Hide();
                ind.Hide();
                //bool falgg = ind.IsHandleCreated;
                cal.Hide();
                ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L);
                epbottom.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 90L);
                bool f = File.Exists(Application.StartupPath + "\\ToolBlock.vpp");
                //MessageBox.Show(Application.StartupPath + "\\ToolBlock.vpp", f.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error);
#if FEATURE_VPRO
                if (File.Exists(Application.StartupPath + "\\ToolBlock.vpp"))
                {
                    string mes = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Licenses:\r\n";
                    bool generic_flag = false;
                    bool elclassify_flag = false;
                    bool elsegment_flag = false;
                    try
                    {
                        
                        CogStringCollection csc = CogLicense.GetLicensedFeatures(false, false);
                        
                        foreach (string s in csc)
                        {
                            mes += "\t" + s + "\r\n";
                            if (s == "VProX.ViDiELSegment")
                            {
                                elsegment_flag = true;
                            }
                            else if (s == "VisionPro.Generic")
                            {
                                generic_flag = true;
                            }
                            else if (s == "VProX.ViDiELClassify")
                            {
                                elclassify_flag = true;
                            }
                            if (elsegment_flag && generic_flag && elclassify_flag)
                            {
                                mTB = CogSerializer.LoadObjectFromFile(Application.StartupPath + "\\ToolBlock.vpp") as CogToolBlock;
                                break;
                            }
                        }
                    }
                    catch { }
                    mes += "\tVProX.ViDiELSegment: " + elsegment_flag.ToString() +"\r\n";
                    mes += "\tVisionPro.Generic " + generic_flag.ToString() + "\r\n";
                    mes += "\tVProX.ViDiELClassify: " + elclassify_flag.ToString() + "\r\n";
                    AddLog(mes, LogContentArray1);
                }
#endif
                ReadBottomCalib(HomographySta1, ol01, bottomcal_path_1);
                ReadBottomCalib(HomographySta2, ol02, bottomcal_path_2);
                ReadBottomCalib(HomographySta3, ol03, bottomcal_path_3);
                ReadBottomCalib(HomographySta4, ol04, bottomcal_path_4);
                ReadCalib(cal1_1, cal_path1_1);
                ReadCalib(cal2_1, cal_path2_1);
                ReadCalib(cal3_1, cal_path3_1);
                ReadCalib(cal4_1, cal_path4_1);
                ReadCalib(cal1_2, cal_path1_2);
                ReadCalib(cal2_2, cal_path2_2);
                ReadCalib(cal3_2, cal_path3_2);
                ReadCalib(cal4_2, cal_path4_2);
                //InitializeTrayObjects();
                InitializeIndexObjects(IndexObjects11, 1);
                InitializeIndexObjects(IndexObjects12, 1);
                InitializeIndexObjects(IndexObjects13, 1);
                InitializeIndexObjects(IndexObjects14, 1);
                InitializeIndexObjects(IndexObjects15, 1);
                InitializeIndexObjects(IndexObjects21, 2);
                InitializeIndexObjects(IndexObjects22, 2);
                InitializeIndexObjects(IndexObjects23, 2);
                InitializeIndexObjects(IndexObjects24, 2);
                InitializeIndexObjects(IndexObjects25, 2);
                try
                {
                    if (ConfigurationManager.AppSettings["LocalServerEnable"] == "true")
                    {
                        ind.LocalServerOnBox.Checked = true;
                        LocalServerEnable = true;
                        ind.LocalIPBox.Text = ConfigurationManager.AppSettings["LocalIP"];//每次开机访问配置文件，获取与供包台plc通讯的本地服务器IP地址
                        ind.LocalPortBox.Text = ConfigurationManager.AppSettings["LocalPort"];//每次开机访问配置文件，获取与供包台plc通讯的本地服务器端口
                    }
                    else if (ConfigurationManager.AppSettings["LocalServerEnable"] == "false")
                    {
                        ind.LocalServerOnBox.Checked = false;
                        LocalServerEnable = false;
                        ind.LocalIPBox.Text = "";
                        ind.LocalPortBox.Text = "";
                        lock (MainForm.OpenConfigLock)
                        {
                            try
                            {
                                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                config.AppSettings.Settings["LocalIP"].Value = "";
                                config.AppSettings.Settings["LocalPort"].Value = "";
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");
                            }
                            catch { }
                        }
                    }
                    ind.ExtCaptionIPBox.Text = ConfigurationManager.AppSettings["ExtCaptionIP"];//每次开机访问配置文件，获取ExtCaptionIP
                    if (ConfigurationManager.AppSettings["Induct11Enable"] == "true")
                    {
                        ind.Ind11OnBox.Checked = true;
                        Induct11Enable = true;
                        ind.Plc11IPBox.Text = ConfigurationManager.AppSettings["Plc11IP"];//每次开机访问配置文件，获取供包台plcIP
                        ind.Bot11PortBox.Text = ConfigurationManager.AppSettings["Bottom11Port"];//每次开机访问配置文件，获取供包台底扫端口
                        ind.Bot11IPBox.Text = ConfigurationManager.AppSettings["Bottom11IP"];//每次开机访问配置文件，获取供包台底扫IP地址
                    }
                    else if (ConfigurationManager.AppSettings["Induct11Enable"] == "false")
                    {
                        ind.Ind11OnBox.Checked = false;
                        Induct11Enable = false;
                        ind.Plc11IPBox.Text = "";
                        ind.Bot11PortBox.Text = "";
                        ind.Bot11IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc11IP"].Value = "";
                            config.AppSettings.Settings["Bottom11Port"].Value = "";
                            config.AppSettings.Settings["Bottom11IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct12Enable"] == "true")
                    {
                        ind.Ind12OnBox.Checked = true;
                        Induct12Enable = true;
                        ind.Plc12IPBox.Text = ConfigurationManager.AppSettings["Plc12IP"];
                        ind.Bot12PortBox.Text = ConfigurationManager.AppSettings["Bottom12Port"];
                        ind.Bot12IPBox.Text = ConfigurationManager.AppSettings["Bottom12IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct12Enable"] == "false")
                    {
                        ind.Ind12OnBox.Checked = false;
                        Induct12Enable = false;
                        ind.Plc12IPBox.Text = "";
                        ind.Bot12PortBox.Text = "";
                        ind.Bot12IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc12IP"].Value = "";
                            config.AppSettings.Settings["Bottom12Port"].Value = "";
                            config.AppSettings.Settings["Bottom12IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct13Enable"] == "true")
                    {
                        ind.Ind13OnBox.Checked = true;
                        Induct13Enable = true;
                        ind.Plc13IPBox.Text = ConfigurationManager.AppSettings["Plc13IP"];
                        ind.Bot13PortBox.Text = ConfigurationManager.AppSettings["Bottom13Port"];
                        ind.Bot13IPBox.Text = ConfigurationManager.AppSettings["Bottom13IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct13Enable"] == "false")
                    {
                        ind.Ind13OnBox.Checked = false;
                        Induct13Enable = false;
                        ind.Plc13IPBox.Text = "";
                        ind.Bot13PortBox.Text = "";
                        ind.Bot13IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc13IP"].Value = "";
                            config.AppSettings.Settings["Bottom13Port"].Value = "";
                            config.AppSettings.Settings["Bottom13IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct14Enable"] == "true")
                    {
                        ind.Ind14OnBox.Checked = true;
                        Induct14Enable = true;
                        ind.Plc14IPBox.Text = ConfigurationManager.AppSettings["Plc14IP"];
                        ind.Bot14PortBox.Text = ConfigurationManager.AppSettings["Bottom14Port"];
                        ind.Bot14IPBox.Text = ConfigurationManager.AppSettings["Bottom14IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct14Enable"] == "false")
                    {
                        ind.Ind14OnBox.Checked = false;
                        Induct14Enable = false;
                        ind.Plc14IPBox.Text = "";
                        ind.Bot14PortBox.Text = "";
                        ind.Bot14IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc14IP"].Value = "";
                            config.AppSettings.Settings["Bottom14Port"].Value = "";
                            config.AppSettings.Settings["Bottom14IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct15Enable"] == "true")
                    {
                        ind.Ind15OnBox.Checked = true;
                        Induct15Enable = true;
                        ind.Plc15IPBox.Text = ConfigurationManager.AppSettings["Plc15IP"];
                        ind.Bot15PortBox.Text = ConfigurationManager.AppSettings["Bottom15Port"];
                        ind.Bot15IPBox.Text = ConfigurationManager.AppSettings["Bottom15IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct15Enable"] == "false")
                    {
                        ind.Ind15OnBox.Checked = false;
                        Induct15Enable = false;
                        ind.Plc15IPBox.Text = "";
                        ind.Bot15PortBox.Text = "";
                        ind.Bot15IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc15IP"].Value = "";
                            config.AppSettings.Settings["Bottom15Port"].Value = "";
                            config.AppSettings.Settings["Bottom15IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct21Enable"] == "true")
                    {
                        ind.Ind21OnBox.Checked = true;
                        Induct21Enable = true;
                        ind.Plc21IPBox.Text = ConfigurationManager.AppSettings["Plc21IP"];
                        ind.Bot21PortBox.Text = ConfigurationManager.AppSettings["Bottom21Port"];
                        ind.Bot21IPBox.Text = ConfigurationManager.AppSettings["Bottom21IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct21Enable"] == "false")
                    {
                        ind.Ind21OnBox.Checked = false;
                        Induct21Enable = false;
                        ind.Plc21IPBox.Text = "";
                        ind.Bot21PortBox.Text = "";
                        ind.Bot21IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc21IP"].Value = "";
                            config.AppSettings.Settings["Bottom21Port"].Value = "";
                            config.AppSettings.Settings["Bottom21IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct22Enable"] == "true")
                    {
                        ind.Ind22OnBox.Checked = true;
                        Induct22Enable = true;
                        ind.Plc22IPBox.Text = ConfigurationManager.AppSettings["Plc22IP"];
                        ind.Bot22PortBox.Text = ConfigurationManager.AppSettings["Bottom22Port"];
                        ind.Bot22IPBox.Text = ConfigurationManager.AppSettings["Bottom22IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct22Enable"] == "false")
                    {
                        ind.Ind22OnBox.Checked = false;
                        Induct22Enable = false;
                        ind.Plc22IPBox.Text = "";
                        ind.Bot22PortBox.Text = "";
                        ind.Bot22IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc22IP"].Value = "";
                            config.AppSettings.Settings["Bottom22Port"].Value = "";
                            config.AppSettings.Settings["Bottom22IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct23Enable"] == "true")
                    {
                        ind.Ind23OnBox.Checked = true;
                        Induct23Enable = true;
                        ind.Plc23IPBox.Text = ConfigurationManager.AppSettings["Plc23IP"];
                        ind.Bot23PortBox.Text = ConfigurationManager.AppSettings["Bottom23Port"];
                        ind.Bot23IPBox.Text = ConfigurationManager.AppSettings["Bottom23IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct23Enable"] == "false")
                    {
                        ind.Ind23OnBox.Checked = false;
                        Induct23Enable = false;
                        ind.Plc23IPBox.Text = "";
                        ind.Bot23PortBox.Text = "";
                        ind.Bot23IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc23IP"].Value = "";
                            config.AppSettings.Settings["Bottom23Port"].Value = "";
                            config.AppSettings.Settings["Bottom23IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct24Enable"] == "true")
                    {
                        ind.Ind24OnBox.Checked = true;
                        Induct24Enable = true;
                        ind.Plc24IPBox.Text = ConfigurationManager.AppSettings["Plc24IP"];
                        ind.Bot24PortBox.Text = ConfigurationManager.AppSettings["Bottom24Port"];
                        ind.Bot24IPBox.Text = ConfigurationManager.AppSettings["Bottom24IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct24Enable"] == "false")
                    {
                        ind.Ind24OnBox.Checked = false;
                        Induct24Enable = false;
                        ind.Plc24IPBox.Text = "";
                        ind.Bot24PortBox.Text = "";
                        ind.Bot24IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc24IP"].Value = "";
                            config.AppSettings.Settings["Bottom24Port"].Value = "";
                            config.AppSettings.Settings["Bottom24IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["Induct25Enable"] == "true")
                    {
                        ind.Ind25OnBox.Checked = true;
                        Induct25Enable = true;
                        ind.Plc25IPBox.Text = ConfigurationManager.AppSettings["Plc25IP"];
                        ind.Bot25PortBox.Text = ConfigurationManager.AppSettings["Bottom25Port"];
                        ind.Bot25IPBox.Text = ConfigurationManager.AppSettings["Bottom25IP"];
                    }
                    else if (ConfigurationManager.AppSettings["Induct25Enable"] == "false")
                    {
                        ind.Ind25OnBox.Checked = false;
                        Induct25Enable = false;
                        ind.Plc25IPBox.Text = "";
                        ind.Bot25PortBox.Text = "";
                        ind.Bot25IPBox.Text = "";
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["Plc25IP"].Value = "";
                            config.AppSettings.Settings["Bottom25Port"].Value = "";
                            config.AppSettings.Settings["Bottom25IP"].Value = "";
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");
                        }
                        catch { }
                    }
                    if (ConfigurationManager.AppSettings["ExtCaptionEnable"] == "true")
                    {
                        ExtCaptionEnableCheck.Checked = true;
                        extCaptionEnable = true;
                    }
                    else if (ConfigurationManager.AppSettings["ExtCaptionEnable"] == "false")
                    {
                        ExtCaptionEnableCheck.Checked = false;
                        extCaptionEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["CaptionEnable"] == "true")
                    {
                        CaptionEnableCheck.Checked = true;
                        captionEnable = true;
                    }
                    else if (ConfigurationManager.AppSettings["CaptionEnable"] == "false")
                    {
                        CaptionEnableCheck.Checked = false;
                        captionEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["NRAnalysisEnable"] == "true")
                    {
                        NRAnalysisEnableCheck.Checked = true;
                        NoReadAnalysisEnable = true;
                    }
                    else if (ConfigurationManager.AppSettings["NRAnalysisEnable"] == "false")
                    {
                        NRAnalysisEnableCheck.Checked = false;
                        NoReadAnalysisEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["Upload"] == "true")
                    {
                        UploadCheck.Checked = true;
                        upload = true;
                    }
                    else if (ConfigurationManager.AppSettings["Upload"] == "false")
                    {
                        UploadCheck.Checked = false;
                        upload = false;
                    }
                    if (ConfigurationManager.AppSettings["Upload2"] == "true")
                    {
                        UploadCheck2.Checked = true;
                        upload2 = true;
                    }
                    else if (ConfigurationManager.AppSettings["Upload2"] == "false")
                    {
                        UploadCheck2.Checked = false;
                        upload2 = false;
                    }
                    if (ConfigurationManager.AppSettings["httpUpload"] == "true")
                    {
                        HTTPUploadCheck.Checked = true;
                        httpUpload = true;
                    }
                    else if (ConfigurationManager.AppSettings["httpUpload"] == "false")
                    {
                        HTTPUploadCheck.Checked = false;
                        httpUpload = false;
                    }
                    if (ConfigurationManager.AppSettings["regexSta1Enable"] == "true")
                    {
                        regexSta1Check.Checked = true;
                        regexSta1Enable = true;
                    }
                    else if (ConfigurationManager.AppSettings["regexSta1Enable"] == "false")
                    {
                        regexSta1Check.Checked = false;
                        regexSta1Enable = false;
                    }
                    if (ConfigurationManager.AppSettings["regexSta2Enable"] == "true")
                    {
                        regexSta2Check.Checked = true;
                        regexSta2Enable = true;
                    }
                    else if (ConfigurationManager.AppSettings["regexSta2Enable"] == "false")
                    {
                        regexSta2Check.Checked = false;
                        regexSta2Enable = false;
                    }
                    if (ConfigurationManager.AppSettings["regexSta3Enable"] == "true")
                    {
                        regexSta3Check.Checked = true;
                        regexSta3Enable = true;
                    }
                    else if (ConfigurationManager.AppSettings["regexSta3Enable"] == "false")
                    {
                        regexSta3Check.Checked = false;
                        regexSta3Enable = false;
                    }
                    if (ConfigurationManager.AppSettings["regexSta4Enable"] == "true")
                    {
                        regexSta4Check.Checked = true;
                        regexSta4Enable = true;
                    }
                    else if (ConfigurationManager.AppSettings["regexSta4Enable"] == "false")
                    {
                        regexSta4Check.Checked = false;
                        regexSta4Enable = false;
                    }
                    if (ConfigurationManager.AppSettings["UploadStatus"] == "true")
                    {
                        UploadStatusCheck.Checked = true;
                        uploadStatus = true;
                    }
                    else if (ConfigurationManager.AppSettings["UploadStatus"] == "false")
                    {
                        UploadStatusCheck.Checked = false;
                        uploadStatus = false;
                    }
                    
                    if (ConfigurationManager.AppSettings["DWSEnable"] == "true")
                    {
                        DWSEnableCheck.Checked = true;
                        DWSInterfaceEnabled = true;
                        ind.DWS1PortBox.Text = ConfigurationManager.AppSettings["DWS1Port"];//每次开机访问配置文件，获取DWS1端口
                        ind.DWS1IPBox.Text = ConfigurationManager.AppSettings["DWS1IP"];//每次开机访问配置文件，获取DWS1地址
                    }
                    else if (ConfigurationManager.AppSettings["DWSEnable"] == "false")
                    {
                        DWSEnableCheck.Checked = false;
                        DWSInterfaceEnabled = false;
                        ind.DWS1PortBox.Text = ConfigurationManager.AppSettings["DWS1Port"];//每次开机访问配置文件，获取DWS1端口
                        ind.DWS1IPBox.Text = ConfigurationManager.AppSettings["DWS1IP"];//每次开机访问配置文件，获取DWS1地址
                    }
                    if (ConfigurationManager.AppSettings["EncryptionEnable"] == "true")
                    {
                        EncryptionEnableCheck.Checked = true;
                        imgEncryptEnable = true; 
                    }
                    else if (ConfigurationManager.AppSettings["EncryptionEnable"] == "false")
                    {
                        EncryptionEnableCheck.Checked = false;
                        imgEncryptEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["GridLineEnable"] == "true")
                    {
                        GridLineEnableCheck.Checked = true;
                        GridLineEnable = true;
                    }
                    else if (ConfigurationManager.AppSettings["GridLineEnable"] == "false")
                    {
                        GridLineEnableCheck.Checked = false;
                        GridLineEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["LogEnable"] == "true")
                    {
                        LogEnableCheck.Checked = true;
                        logEnable = true;
                    }
                    else if (ConfigurationManager.AppSettings["LogEnable"] == "false")
                    {
                        LogEnableCheck.Checked = false;
                        logEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["BarcodeImgEnable"] == "true")
                    {
                        BarcodeImgEnableCheck.Checked = true;
                        BarcodeImgEnable = true;
                    }
                    else if (ConfigurationManager.AppSettings["BarcodeImgEnable"] == "false")
                    {
                        BarcodeImgEnableCheck.Checked = false;
                        BarcodeImgEnable = false;
                    }
                    if (ConfigurationManager.AppSettings["TopOnlyOnRead1"] == "true")
                    {
                        TopOnlyCheck1.Checked = true;
                        TopOnlyOnRead1 = true;
                    }
                    else if (ConfigurationManager.AppSettings["TopOnlyOnRead1"] == "false")
                    {
                        TopOnlyCheck1.Checked = false;
                        TopOnlyOnRead1 = false;
                    }
                    if (ConfigurationManager.AppSettings["TopOnlyOnRead2"] == "true")
                    {
                        TopOnlyCheck2.Checked = true;
                        TopOnlyOnRead2 = true;
                    }
                    else if (ConfigurationManager.AppSettings["TopOnlyOnRead2"] == "false")
                    {
                        TopOnlyCheck2.Checked = false;
                        TopOnlyOnRead2 = false;
                    }
                    if (ConfigurationManager.AppSettings["TopOnlyOnRead3"] == "true")
                    {
                        TopOnlyCheck3.Checked = true;
                        TopOnlyOnRead3 = true;
                    }
                    else if (ConfigurationManager.AppSettings["TopOnlyOnRead3"] == "false")
                    {
                        TopOnlyCheck3.Checked = false;
                        TopOnlyOnRead3 = false;
                    }
                    if (ConfigurationManager.AppSettings["TopOnlyOnRead4"] == "true")
                    {
                        TopOnlyCheck4.Checked = true;
                        TopOnlyOnRead4 = true;
                    }
                    else if (ConfigurationManager.AppSettings["TopOnlyOnRead4"] == "false")
                    {
                        TopOnlyCheck4.Checked = false;
                        TopOnlyOnRead4 = false;
                    }
                    if (ConfigurationManager.AppSettings["Sta1On"] == "true")
                    {
                        Sta1OnBox.Checked = true;
                        Sta1On = true;
                    }
                    else if (ConfigurationManager.AppSettings["Sta1On"] == "false")
                    {
                        Sta1OnBox.Checked = false;
                        Sta1On = false;
                    }
                    if (ConfigurationManager.AppSettings["Sta2On"] == "true")
                    {
                        Sta2OnBox.Checked = true;
                        Sta2On = true;
                    }
                    else if (ConfigurationManager.AppSettings["Sta2On"] == "false")
                    {
                        Sta2OnBox.Checked = false;
                        Sta2On = false;
                    }
                    if (ConfigurationManager.AppSettings["Sta3On"] == "true")
                    {
                        Sta3OnBox.Checked = true;
                        Sta3On = true;
                    }
                    else if (ConfigurationManager.AppSettings["Sta3On"] == "false")
                    {
                        Sta3OnBox.Checked = false;
                        Sta3On = false;
                    }
                    if (ConfigurationManager.AppSettings["Sta4On"] == "true")
                    {
                        Sta4OnBox.Checked = true;
                        Sta4On = true;
                    }
                    else if (ConfigurationManager.AppSettings["Sta4On"] == "false")
                    {
                        Sta4OnBox.Checked = false;
                        Sta4On = false;
                    }
                    if (ConfigurationManager.AppSettings["LastDeleteFileTimeStamp"] == "")
                    {
                        lock (OpenConfigLock)
                        {
                            try
                            {
                                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                config.AppSettings.Settings["LastDeleteFileTimeStamp"].Value = DateTime.MinValue.ToString("yyyy/MM/dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("-", "/");
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");//写入上次处理的图片的时间戳
                            }
                            catch (Exception ex) { }
                        }
                    }
                   
                    if (ConfigurationManager.AppSettings["AllowPackageBorder"] == "true")
                    {
                        PackageBorderCheck.Checked = true;
                        allowPackageBorder = true;
                    }
                    else if (ConfigurationManager.AppSettings["AllowPackageBorder"] == "false")
                    {
                        PackageBorderCheck.Checked = false;
                        allowPackageBorder = false;
                    }
                    var format = "yyyy/MM/dd HH:mm:ss.fff";
                    //string LastHandleImageTimeStampstr = ConfigurationManager.AppSettings["LastHandleImageTimeStamp"];
                    // LastHandleImageTimeStamp = DateTime.ParseExact(LastHandleImageTimeStampstr, format, CultureInfo.InvariantCulture);
                    string LastDeleteFileTimeStampstr = ConfigurationManager.AppSettings["LastDeleteFileTimeStamp"];
                    LastDeleteFileTimeStamp = DateTime.ParseExact(LastDeleteFileTimeStampstr.Replace("-", "/"), format, CultureInfo.InvariantCulture);
                    ConnectStatusLabel.BackColor = Color.White;
                    ConnectStatusLabel2.BackColor = Color.White;
                    HttpConnectStatusLabel.BackColor = Color.White;
                    FtpKeepAlive = ConfigurationManager.AppSettings["FtpKeepAlive"];//每次开机访问配置文件，获取是否keepalive
                    AdminUser = ConfigurationManager.AppSettings["AdminUser"];//每次开机访问配置文件，获取Admin用户名
                    AdminPassword = ConfigurationManager.AppSettings["AdminPassword"];//每次开机访问配置文件，获取Admin密码
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["PicStorDays"], DaysPattern))
                    {
                        PicStorDaysText.Text = ConfigurationManager.AppSettings["PicStorDays"];
                    }
                    else
                    {
                        PicStorDaysText.Text = DefaultPicStorDaysText;
                    }
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["BindingTimeout"], DaysPattern))
                    {
                        ind.BindingTimeoutBox.Text = ConfigurationManager.AppSettings["BindingTimeout"];
                    }
                    else
                    {
                        ind.BindingTimeoutBox.Text = DefaultBindingTimeoutText;
                    }
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["PixelSize"], DoublePattern))
                    {
                        gs.PixelSizeText.Text = ConfigurationManager.AppSettings["PixelSize"];
                    }
                    else
                    {
                        gs.PixelSizeText.Text = DefaultPixelSize;
                    }
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["FocusLength"], DoublePattern))
                    {
                        gs.FocusLengthText.Text = ConfigurationManager.AppSettings["FocusLength"];
                    }
                    else
                    {
                        gs.FocusLengthText.Text = DefaultFocusLength;
                    }
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["ReadAngle"], DoublePattern))
                    {
                        gs.ReadAngleText.Text = ConfigurationManager.AppSettings["ReadAngle"];
                    }
                    else
                    {
                        gs.ReadAngleText.Text = DefaultReadAngle;
                    }
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["MountingHeight"], DoublePattern))
                    {
                        gs.MountingHeightText.Text = ConfigurationManager.AppSettings["MountingHeight"];
                    }
                    else
                    {
                        gs.MountingHeightText.Text = DefaultMountingHeight;
                    }
                    if (Regex.IsMatch(ConfigurationManager.AppSettings["MaxParcelHeight"], DoublePattern))
                    {
                        gs.MaxParcelHeightText.Text = ConfigurationManager.AppSettings["MaxParcelHeight"];
                    }
                    else
                    {
                        gs.MaxParcelHeightText.Text = DefaultMaxParcelHeight;
                    }
                    delimiterBox.Text = ConfigurationManager.AppSettings["MultiCodeDelimiter"];//每次开机访问配置文件，获取多条码分隔符
                    FTPOKBox.Text = ConfigurationManager.AppSettings["FTPOKMes"];//每次开机访问配置文件，获取FTPOK报文
                    FTPNOKBox.Text = ConfigurationManager.AppSettings["FTPNOKMes"];//每次开机访问配置文件，获取FTPNOK报文
                    HTTPOKBox.Text = ConfigurationManager.AppSettings["HTTPOKMes"];//每次开机访问配置文件，获取HTTPOK报文
                    HTTPNOKBox.Text = ConfigurationManager.AppSettings["HTTPNOKMes"];//每次开机访问配置文件，获取HTTPNOK报文
                    regexMachineCodeBox.Text = ConfigurationManager.AppSettings["regexMachineCode"];//每次开机访问配置文件，获取regexMachineCode
                    machineCodeBox.Text = ConfigurationManager.AppSettings["machineCode"];//每次开机访问配置文件，获取machineCode
                    HttpCheckUrlBox.Text = ConfigurationManager.AppSettings["HttpCheckUrl"];//每次开机访问配置文件，获取HttpCheckUrl
                    HttpUploadUrlBox.Text = ConfigurationManager.AppSettings["HttpUploadUrl"];//每次开机访问配置文件，获取HttpUploadUrl
                    HttpRegexUrlBox.Text = ConfigurationManager.AppSettings["HttpRegexUrl"];//每次开机访问配置文件，获取HttpRegexUrl
                    InductorStation1NoBox.Text = ConfigurationManager.AppSettings["InductorStation1No"];//每次开机访问配置文件，获取供包台一区工站号
                    InductorStation2NoBox.Text = ConfigurationManager.AppSettings["InductorStation2No"];//每次开机访问配置文件，获取供包台一区工站号
                    UploadModeList.Text = ConfigurationManager.AppSettings["UploadMode"];//每次开机访问配置文件，获取图片上传模式
                    UserBox.Text = ConfigurationManager.AppSettings["User"];//每次开机访问配置文件，获取ftp用户名
                    PasswordBox.Text = ConfigurationManager.AppSettings["Password"];//每次开机访问配置文件，获取ftp密码
                    UserBox2.Text = ConfigurationManager.AppSettings["User2"];//每次开机访问配置文件，获取多路输出ftp用户名
                    PasswordBox2.Text = ConfigurationManager.AppSettings["Password2"];//每次开机访问配置文件，获取多路输出ftp密码
                    SCADAIPBox.Text = ConfigurationManager.AppSettings["SCADAIP"];//每次开机访问配置文件，获取服务器ip
                    SCADAPortBox.Text = ConfigurationManager.AppSettings["SCADAPort"];//每次开机访问配置文件，获取服务器端口
                    regexMasterIPBox1.Text = ConfigurationManager.AppSettings["RegexMasterIP1"];//每次开机访问配置文件，获取regex主机ip
                    regexMasterPortBox1.Text = ConfigurationManager.AppSettings["RegexMasterPort1"];//每次开机访问配置文件，获取regex主机端口
                    regexMasterIPBox2.Text = ConfigurationManager.AppSettings["RegexMasterIP2"];//每次开机访问配置文件，获取regex主机ip
                    regexMasterPortBox2.Text = ConfigurationManager.AppSettings["RegexMasterPort2"];//每次开机访问配置文件，获取regex主机端口
                    regexMasterIPBox3.Text = ConfigurationManager.AppSettings["RegexMasterIP3"];//每次开机访问配置文件，获取regex主机ip
                    regexMasterPortBox3.Text = ConfigurationManager.AppSettings["RegexMasterPort3"];//每次开机访问配置文件，获取regex主机端口
                    regexMasterIPBox4.Text = ConfigurationManager.AppSettings["RegexMasterIP4"];//每次开机访问配置文件，获取regex主机ip
                    regexMasterPortBox4.Text = ConfigurationManager.AppSettings["RegexMasterPort4"];//每次开机访问配置文件，获取regex主机端口
                    ServerIPBox.Text = ConfigurationManager.AppSettings["ServerIP"];//每次开机访问配置文件，获取服务器ip
                    PortBox.Text = ConfigurationManager.AppSettings["FTPPort"];//每次开机访问配置文件，获取服务器端口
                    ServerIPBox2.Text = ConfigurationManager.AppSettings["ServerIP2"];//每次开机访问配置文件，获取服务器ip
                    PortBox2.Text = ConfigurationManager.AppSettings["FTPPort2"];//每次开机访问配置文件，获取服务器端口
                    InductFtpRootBox.Text = ConfigurationManager.AppSettings["InductFtpRoot"];//每次开机访问配置文件，获取供包台ftp存图地址
                    FtpRootBox1.Text = ConfigurationManager.AppSettings["ftp_image_dir1"];//每次开机访问配置文件，获取相机ftp存图地址
                    FtpRootBox2.Text = ConfigurationManager.AppSettings["ftp_image_dir2"];//每次开机访问配置文件，获取相机ftp存图地址
                    FtpRootBox3.Text = ConfigurationManager.AppSettings["ftp_image_dir3"];//每次开机访问配置文件，获取相机ftp存图地址
                    FtpRootBox4.Text = ConfigurationManager.AppSettings["ftp_image_dir4"];//每次开机访问配置文件，获取相机ftp存图地址
                    ExtDelayCombo1.Text = ConfigurationManager.AppSettings["ExtCaptionDelay1"];//每次开机访问配置文件，获取外部水印等待时间
                    ExtDelayCombo2.Text = ConfigurationManager.AppSettings["ExtCaptionDelay2"];//每次开机访问配置文件，获取外部水印等待时间
                    ExtDelayCombo3.Text = ConfigurationManager.AppSettings["ExtCaptionDelay3"];//每次开机访问配置文件，获取外部水印等待时间
                    ExtDelayCombo4.Text = ConfigurationManager.AppSettings["ExtCaptionDelay4"];//每次开机访问配置文件，获取外部水印等待时间
                    BottomCamNoCombo1.Text = ConfigurationManager.AppSettings["BottomCamNo1"];//每次开机访问配置文件，获取底扫存图相机数量
                    BottomCamNoCombo2.Text = ConfigurationManager.AppSettings["BottomCamNo2"];//每次开机访问配置文件，获取底扫存图相机数量
                    BottomCamNoCombo3.Text = ConfigurationManager.AppSettings["BottomCamNo3"];//每次开机访问配置文件，获取底扫存图相机数量
                    BottomCamNoCombo4.Text = ConfigurationManager.AppSettings["BottomCamNo4"];//每次开机访问配置文件，获取底扫存图相机数量
                    LangCombo.Text = ConfigurationManager.AppSettings["CurLang"];
                    if (LangCombo.Text == "Chinese")
                    {
                        captionOptionList.Items[0] = "包含中文字符";
                        captionOptionList.Items[1] = "仅英文字符";
                        if (ConfigurationManager.AppSettings["AllowChineseCaption"] == "false")
                        {
                            allowChinese = false;
                            captionOptionList.Text = "仅英文字符";
                        }
                        else
                        {
                            allowChinese = true;
                            captionOptionList.Text = "包含中文字符";
                        }
                        BarcodeImgModeCombo.Items[0] = "拼接";
                        BarcodeImgModeCombo.Items[1] = "不拼接";
                        BarcodeImgModeCombo.Text = (ConfigurationManager.AppSettings["BarcodeImgMode"]== "拼接" || ConfigurationManager.AppSettings["BarcodeImgMode"] == "stitched")? "拼接": "不拼接";//每次开机访问配置文件，获取局部图片处理模式
                    }
                    else
                    {
                        captionOptionList.Items[0] = "AllowChinese";
                        captionOptionList.Items[1] = "EnglishOnly";
                        if (ConfigurationManager.AppSettings["AllowChineseCaption"] == "false")
                        {
                            allowChinese = false;
                            captionOptionList.Text = "EnglishOnly";
                        }
                        else
                        {
                            allowChinese = true;
                            captionOptionList.Text = "AllowChinese";
                        }
                        BarcodeImgModeCombo.Items[0] = "stitched";
                        BarcodeImgModeCombo.Items[1] = "normal";
                        BarcodeImgModeCombo.Text = (ConfigurationManager.AppSettings["BarcodeImgMode"] == "拼接" || ConfigurationManager.AppSettings["BarcodeImgMode"] == "stitched") ? "stitched" : "normal";
                    }
                    
                    InductIDCombo1.Text = ConfigurationManager.AppSettings["InductID1"];//每次开机访问配置文件，获取工位一对应的供包台区域
                    InductIDCombo2.Text = ConfigurationManager.AppSettings["InductID2"];//每次开机访问配置文件，获取工位二对应的供包台区域
                    InductIDCombo3.Text = ConfigurationManager.AppSettings["InductID3"];//每次开机访问配置文件，获取工位三对应的供包台区域
                    InductIDCombo4.Text = ConfigurationManager.AppSettings["InductID4"];//每次开机访问配置文件，获取工位四对应的供包台区域
                    SorterTimeStampCombo.Text = ConfigurationManager.AppSettings["SorterTimeStamp"];//每次开机访问配置文件，获取主线时间戳字段名称
                    TrayIDCombo.Text = ConfigurationManager.AppSettings["SorterTrayID"];//每次开机访问配置文件，获取主线小车号字段名称
                    ResultCombo.Text = ConfigurationManager.AppSettings["SorterResult"];//每次开机访问配置文件，获取主线结果字段名称
                    BarcodeCombo.Text = ConfigurationManager.AppSettings["SorterBarcode"];//每次开机访问配置文件，获取主线条码字段名称
                    BoxIDCombo.Text = ConfigurationManager.AppSettings["SorterBoxID"];//每次开机访问配置文件，获取主线索引号字段名称
                    SorterStationNoCombo.Text = ConfigurationManager.AppSettings["SorterStationNo"];//每次开机访问配置文件，获取主线工位号字段名称
                    CamNoCombo1.Text = ConfigurationManager.AppSettings["CamNo1"];//每次开机访问配置文件，获取存图相机数量
                    CamNoCombo2.Text = ConfigurationManager.AppSettings["CamNo2"];//每次开机访问配置文件，获取存图相机数量
                    CamNoCombo3.Text = ConfigurationManager.AppSettings["CamNo3"];//每次开机访问配置文件，获取存图相机数量
                    CamNoCombo4.Text = ConfigurationManager.AppSettings["CamNo4"];//每次开机访问配置文件，获取存图相机数量
                    ImageRootBox1.Text = ConfigurationManager.AppSettings["StorageRoot1"];//每次开机访问配置文件，获取图片存放位置
                    ImageRootBox2.Text = ConfigurationManager.AppSettings["StorageRoot2"];//每次开机访问配置文件，获取图片存放位置
                    ImageRootBox3.Text = ConfigurationManager.AppSettings["StorageRoot3"];//每次开机访问配置文件，获取图片存放位置
                    ImageRootBox4.Text = ConfigurationManager.AppSettings["StorageRoot4"];//每次开机访问配置文件，获取图片存放位置
                    Folder1Combo.Text = ConfigurationManager.AppSettings["SubFolder1"];//每次开机访问配置文件，获取第一层文件夹命名
                    Folder2Combo.Text = ConfigurationManager.AppSettings["SubFolder2"];//每次开机访问配置文件，获取第二层文件夹命名
                    Folder3Combo.Text = ConfigurationManager.AppSettings["SubFolder3"];//每次开机访问配置文件，获取第三层文件夹命名
                    Folder4Combo.Text = ConfigurationManager.AppSettings["SubFolder4"];//每次开机访问配置文件，获取第四层文件夹命名
                    Folder5Combo.Text = ConfigurationManager.AppSettings["SubFolder5"];//每次开机访问配置文件，获取第五层文件夹命名
                    FTPFolder1Combo.Text = ConfigurationManager.AppSettings["FtpSubFolder1"];//每次开机访问配置文件，获取FTP第一层文件夹命名
                    FTPFolder2Combo.Text = ConfigurationManager.AppSettings["FtpSubFolder2"];//每次开机访问配置文件，获取FTP第二层文件夹命名
                    FTPFolder3Combo.Text = ConfigurationManager.AppSettings["FtpSubFolder3"];//每次开机访问配置文件，获取FTP第三层文件夹命名
                    FTPFolder4Combo.Text = ConfigurationManager.AppSettings["FtpSubFolder4"];//每次开机访问配置文件，获取FTP第四层文件夹命名
                    FTPFolder5Combo.Text = ConfigurationManager.AppSettings["FtpSubFolder5"];//每次开机访问配置文件，获取FTP第五层文件夹命名
                    FtpFileNameBox.Text = ConfigurationManager.AppSettings["FtpFileNameFormat"];//每次开机访问配置文件，获取FTP图片名格式
                    FtpFileNameBox.Text = Regex.IsMatch(FtpFileNameBox.Text, NonSpacePattern) ? FtpFileNameBox.Text : DefaultFileNameFormat;
                    FtpFileNameBox.Text = FtpFileNameBox.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    FtpFileNameFormat = FtpFileNameBox.Text;
                    FtpFileNameBox2.Text = ConfigurationManager.AppSettings["FtpFileNameFormat2"];//每次开机访问配置文件，获取FTP图片名格式
                    FtpFileNameBox2.Text = Regex.IsMatch(FtpFileNameBox2.Text, NonSpacePattern) ? FtpFileNameBox2.Text : DefaultFileNameFormat;
                    FtpFileNameBox2.Text = FtpFileNameBox2.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    FtpFileNameFormat2 = FtpFileNameBox2.Text;
                    FileNameBox.Text = ConfigurationManager.AppSettings["FileNameFormat"];//每次开机访问配置文件，获取图片名格式
                    FileNameBox.Text = Regex.IsMatch(FileNameBox.Text, NonSpacePattern) ? FileNameBox.Text : DefaultFileNameFormat;
                    FileNameBox.Text = FileNameBox.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    FileNameFormat = FileNameBox.Text;
                    HttpFileNameBox.Text = ConfigurationManager.AppSettings["HttpFileNameFormat"];//每次开机访问配置文件，获取HTTP图片名格式
                    HttpFileNameBox.Text = Regex.IsMatch(HttpFileNameBox.Text, NonSpacePattern) ? HttpFileNameBox.Text : DefaultFileNameFormat;
                    HttpFileNameBox.Text = HttpFileNameBox.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    HttpFileNameFormat = HttpFileNameBox.Text;
                    CaptionFormatBox.Text = ConfigurationManager.AppSettings["CaptionFormat"];//每次开机访问配置文件，获取水印格式
                    CaptionFormat = CaptionFormatBox.Text;
                    PosCombo.Text = ConfigurationManager.AppSettings["3DPosFormat"];//每次开机访问配置文件，获取三维位置格式
                    PosFormat = PosCombo.Text;
                    BarcodePosCombo.Text = ConfigurationManager.AppSettings["BarocdePosFormat"];//每次开机访问配置文件，获取三维位置格式
                    BarcodePosFormat = BarcodePosCombo.Text;
                    DimWgtCombo.Text = ConfigurationManager.AppSettings["DimAndWeightFormat"];//每次开机访问配置文件，获取体积重量格式
                    DimAndWeightFormat = DimWgtCombo.Text;
                    InductorBindingTimeout = Convert.ToInt32(ind.BindingTimeoutBox.Text);
                    
                    //shiftLang();
                    LocalIP = ind.LocalIPBox.Text;
                    LocalPort = ind.LocalPortBox.Text;
                    ExtCaptionIP = ind.ExtCaptionIPBox.Text;
                    Plc11IP = ind.Plc11IPBox.Text;
                    Plc12IP = ind.Plc12IPBox.Text;
                    Plc13IP = ind.Plc13IPBox.Text;
                    Plc14IP = ind.Plc14IPBox.Text;
                    Plc15IP = ind.Plc15IPBox.Text;
                    Plc21IP = ind.Plc21IPBox.Text;
                    Plc22IP = ind.Plc22IPBox.Text;
                    Plc23IP = ind.Plc23IPBox.Text;
                    Plc24IP = ind.Plc24IPBox.Text;
                    Plc25IP = ind.Plc25IPBox.Text;
                    DWS1IP = ind.DWS1IPBox.Text;
                    DWS1Port = ind.DWS1PortBox.Text;
                    Bottom11Port = ind.Bot11PortBox.Text;
                    Bottom12Port = ind.Bot12PortBox.Text;
                    Bottom13Port = ind.Bot13PortBox.Text;
                    Bottom14Port = ind.Bot14PortBox.Text;
                    Bottom15Port = ind.Bot15PortBox.Text;
                    Bottom21Port = ind.Bot21PortBox.Text;
                    Bottom22Port = ind.Bot22PortBox.Text;
                    Bottom23Port = ind.Bot23PortBox.Text;
                    Bottom24Port = ind.Bot24PortBox.Text;
                    Bottom25Port = ind.Bot25PortBox.Text;
                    Bottom11IP = ind.Bot11IPBox.Text;
                    Bottom12IP = ind.Bot12IPBox.Text;
                    Bottom13IP = ind.Bot13IPBox.Text;
                    Bottom14IP = ind.Bot14IPBox.Text;
                    Bottom15IP = ind.Bot15IPBox.Text;
                    Bottom21IP = ind.Bot21IPBox.Text;
                    Bottom22IP = ind.Bot22IPBox.Text;
                    Bottom23IP = ind.Bot23IPBox.Text;
                    Bottom24IP = ind.Bot24IPBox.Text;
                    Bottom25IP = ind.Bot25IPBox.Text;
                    MultiCodeDelimiter[0] = delimiterBox.Text;
                    FTPOKMes = FTPOKBox.Text;
                    FTPNOKMes = FTPNOKBox.Text;
                    HTTPOKMes = HTTPOKBox.Text;
                    HTTPNOKMes = HTTPNOKBox.Text;
                    //siteCode = siteCodeBox.Text;
                    regexMachineCode = regexMachineCodeBox.Text;
                    machineCodeFormat = machineCodeBox.Text;
                    HttpCheckUrl = HttpCheckUrlBox.Text;
                    HttpUploadUrl = HttpUploadUrlBox.Text;
                    HttpRegexUrl = HttpRegexUrlBox.Text;
                    InductorStation1No = InductorStation1NoBox.Text;
                    InductorStation2No = InductorStation2NoBox.Text;
                    UploadMode = UploadModeList.Text;
                    User = UserBox.Text;
                    Password = PasswordBox.Text;
                    Credential = new NetworkCredential(User, Password);
                    User2 = UserBox2.Text;
                    Password2 = PasswordBox2.Text;
                    Credential2 = new NetworkCredential(User2, Password2);
                    PicStorDays = Convert.ToInt32(PicStorDaysText.Text);
                    PixelSize = Convert.ToDouble(gs.PixelSizeText.Text);
                    FocusLength = Convert.ToDouble(gs.FocusLengthText.Text);
                    ReadAngle = Convert.ToDouble(gs.ReadAngleText.Text);
                    MountingHeight = Convert.ToDouble(gs.MountingHeightText.Text);
                    MaxParcelHeight = Convert.ToDouble(gs.MaxParcelHeightText.Text);
                    regexMasterIP1 = regexMasterIPBox1.Text;
                    regexMasterPort1 = regexMasterPortBox1.Text;
                    regexMasterIP2 = regexMasterIPBox2.Text;
                    regexMasterPort2 = regexMasterPortBox2.Text;
                    regexMasterIP3 = regexMasterIPBox3.Text;
                    regexMasterPort3 = regexMasterPortBox3.Text;
                    regexMasterIP4 = regexMasterIPBox4.Text;
                    regexMasterPort4 = regexMasterPortBox4.Text;
                    ServerIP = ServerIPBox.Text;
                    FTPPort = PortBox.Text;
                    ServerIP2 = ServerIPBox2.Text;
                    FTPPort2 = PortBox2.Text;
                    SCADAIP = SCADAIPBox.Text;
                    SCADAPort = SCADAPortBox.Text;
                    InductFtpRoot = InductFtpRootBox.Text;
                    FtpRoot1 = FtpRootBox1.Text;//ftp存图根目录
                    FtpRoot2 = FtpRootBox2.Text;
                    FtpRoot3 = FtpRootBox3.Text;
                    FtpRoot4 = FtpRootBox4.Text;
                    FtpRoots[0] = FtpRoot1;
                    FtpRoots[1] = FtpRoot2;
                    FtpRoots[2] = FtpRoot3;
                    FtpRoots[3] = FtpRoot4;
                    StorageRoot1 = ImageRootBox1.Text;//本地图片存放位置根目录
                    StorageRoot2 = ImageRootBox2.Text;
                    StorageRoot3 = ImageRootBox3.Text;
                    StorageRoot4 = ImageRootBox4.Text;
                    StorageRoots[0] = StorageRoot1;
                    StorageRoots[1] = StorageRoot2;
                    StorageRoots[2] = StorageRoot3;
                    StorageRoots[3] = StorageRoot4;
                    disk = FtpRoot1.Substring(0, 2);
                    //disk = StorageRoot1.Substring(0, 2);
                    barcode_stitch_url1 = disk + "\\SIH\\TopStitch1";
                    barcode_stitch_url2 = disk + "\\SIH\\TopStitch2";
                    barcode_stitch_url3 = disk + "\\SIH\\TopStitch3";
                    barcode_stitch_url4 = disk + "\\SIH\\TopStitch4";
                    barcode_stitch_url5 = disk + "\\SIH\\TopStitch5";
                    inductor_sorter_temp_url = disk + "\\SIH\\InductorSorterCombination";
                    stitch_url1 = disk + "\\SIH\\Bottom\\Stitch1";
                    stitch_url2 = disk + "\\SIH\\Bottom\\Stitch2";
                    stitch_url3 = disk + "\\SIH\\Bottom\\Stitch3";
                    stitch_url4 = disk + "\\SIH\\Bottom\\Stitch4";
                    stitch_url5 = disk + "\\SIH\\Bottom\\Stitch5";
                    ext_caption_url1 = disk + "\\SIH\\ExtCaption\\Station1";
                    ext_caption_url2 = disk + "\\SIH\\ExtCaption\\Station2";
                    ext_caption_url3 = disk + "\\SIH\\ExtCaption\\Station3";
                    ext_caption_url4 = disk + "\\SIH\\ExtCaption\\Station4";
                    caption_url1 = disk + "\\SIH\\Caption\\Temp1";//Caption临时文件夹
                    caption_url2 = disk + "\\SIH\\Caption\\Temp2";
                    caption_url3 = disk + "\\SIH\\Caption\\Temp3";
                    caption_url4 = disk + "\\SIH\\Caption\\Temp4";
                    caption_url5 = disk + "\\SIH\\Caption\\Temp5";
                    ftp_temp_url1 = disk + "\\SIH\\FtpUpload\\Temp1";//ftp上传临时文件夹
                    ftp_temp_url2 = disk + "\\SIH\\FtpUpload\\Temp2";
                    ftp_temp_url3 = disk + "\\SIH\\FtpUpload\\Temp3";
                    ftp_temp_url4 = disk + "\\SIH\\FtpUpload\\Temp4";
                    ftp_temp_url5 = disk + "\\SIH\\FtpUpload\\Temp5";
                    ftp_temp_url6 = disk + "\\SIH\\FtpUpload\\Temp6";
                    http_temp_url1 = disk + "\\SIH\\HttpUpload\\Temp1";
                    http_temp_url2 = disk + "\\SIH\\HttpUpload\\Temp2";
                    http_temp_url3 = disk + "\\SIH\\HttpUpload\\Temp3";
                    //ftp_log_dir = disk + "\\log";
                    Folder1Combo.Text = Regex.IsMatch(Folder1Combo.Text, NonSpacePattern) ? Folder1Combo.Text : DefaultSubFolder;
                    Folder2Combo.Text = Regex.IsMatch(Folder2Combo.Text, NonSpacePattern) ? Folder2Combo.Text : DefaultSubFolder;
                    Folder3Combo.Text = Regex.IsMatch(Folder3Combo.Text, NonSpacePattern) ? Folder3Combo.Text : DefaultSubFolder;
                    Folder4Combo.Text = Regex.IsMatch(Folder4Combo.Text, NonSpacePattern) ? Folder4Combo.Text : DefaultSubFolder;
                    Folder5Combo.Text = Regex.IsMatch(Folder5Combo.Text, NonSpacePattern) ? Folder5Combo.Text : DefaultSubFolder;
                    Folder1Combo.Text = Folder1Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    Folder2Combo.Text = Folder2Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    Folder3Combo.Text = Folder3Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    Folder4Combo.Text = Folder4Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    Folder5Combo.Text = Folder5Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    FTPFolder1Combo.Text = Regex.IsMatch(FTPFolder1Combo.Text, NonSpacePattern) ? FTPFolder1Combo.Text : DefaultSubFolder;
                    FTPFolder2Combo.Text = Regex.IsMatch(FTPFolder2Combo.Text, NonSpacePattern) ? FTPFolder2Combo.Text : DefaultSubFolder;
                    FTPFolder3Combo.Text = Regex.IsMatch(FTPFolder3Combo.Text, NonSpacePattern) ? FTPFolder3Combo.Text : DefaultSubFolder;
                    FTPFolder4Combo.Text = Regex.IsMatch(FTPFolder4Combo.Text, NonSpacePattern) ? FTPFolder4Combo.Text : DefaultSubFolder;
                    FTPFolder5Combo.Text = Regex.IsMatch(FTPFolder5Combo.Text, NonSpacePattern) ? FTPFolder5Combo.Text : DefaultSubFolder;
                    FTPFolder1Combo.Text = FTPFolder1Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
                    FTPFolder2Combo.Text = FTPFolder2Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
                    FTPFolder3Combo.Text = FTPFolder3Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
                    FTPFolder4Combo.Text = FTPFolder4Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
                    FTPFolder5Combo.Text = FTPFolder5Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
                    SubFolder1 = Folder1Combo.Text;//本地文件夹名称
                    SubFolder2 = Folder2Combo.Text;
                    SubFolder3 = Folder3Combo.Text;
                    SubFolder4 = Folder4Combo.Text;
                    SubFolder5 = Folder5Combo.Text;
                    FtpSubFolder1 = FTPFolder1Combo.Text;
                    FtpSubFolder2 = FTPFolder2Combo.Text;
                    FtpSubFolder3 = FTPFolder3Combo.Text;
                    FtpSubFolder4 = FTPFolder4Combo.Text;
                    FtpSubFolder5 = FTPFolder5Combo.Text;
                    SubFolders[0] = SubFolder1;
                    SubFolders[1] = SubFolder2;
                    SubFolders[2] = SubFolder3;
                    SubFolders[3] = SubFolder4;
                    SubFolders[4] = SubFolder5;
                    ExtCaptionBoxTimeOut1 = Convert.ToInt16(ExtDelayCombo1.Text) * 1000;
                    ExtCaptionBoxTimeOut2 = Convert.ToInt16(ExtDelayCombo2.Text) * 1000;
                    ExtCaptionBoxTimeOut3 = Convert.ToInt16(ExtDelayCombo3.Text) * 1000;
                    ExtCaptionBoxTimeOut4 = Convert.ToInt16(ExtDelayCombo4.Text) * 1000;
                    BottomCamCount1 = Convert.ToInt16(BottomCamNoCombo1.Text);
                    BottomCamCount2 = Convert.ToInt16(BottomCamNoCombo2.Text);
                    BottomCamCount3 = Convert.ToInt16(BottomCamNoCombo3.Text);
                    BottomCamCount4 = Convert.ToInt16(BottomCamNoCombo4.Text);
                    BarcodeImgMode = BarcodeImgModeCombo.Text;
                    InductID1 = InductIDCombo1.Text;
                    InductID2 = InductIDCombo2.Text;
                    InductID3 = InductIDCombo3.Text;
                    InductID4 = InductIDCombo4.Text;
                    SorterTimeStampFormat = SorterTimeStampCombo.Text;
                    SorterTrayIDFormat = TrayIDCombo.Text;
                    SorterResultFormat = ResultCombo.Text;
                    SorterBarcodeFormat = BarcodeCombo.Text;
                    SorterBoxIDFormat = BoxIDCombo.Text;
                    SorterStationNoFormat = SorterStationNoCombo.Text;
                    CamCount1 = Convert.ToInt16(CamNoCombo1.Text);
                    CamCount2 = Convert.ToInt16(CamNoCombo2.Text);
                    CamCount3 = Convert.ToInt16(CamNoCombo3.Text);
                    CamCount4 = Convert.ToInt16(CamNoCombo4.Text);
                    TotalCamCount1 = CamCount1 + 4 * BottomCamCount1;
                    TotalCamCount2 = CamCount2 + 4 * BottomCamCount2;
                    TotalCamCount3 = CamCount3 + 4 * BottomCamCount3;
                    TotalCamCount4 = CamCount4 + 4 * BottomCamCount4;
                    sftp1 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    sftp2 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    sftp3 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    sftp4 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    sftp5 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    if (DWSInterfaceEnabled)
                    {
                        try
                        {
                            if (DWS1IP != "" && DWS1Port != "")
                            {
                                DWS1Handler = new SocketLib.TcpClientBase(DWS1IP, Convert.ToInt32(DWS1Port));
                                DWS1Handler.OnExpectedData += OnExpectedData;
                                DWS1Handler.logmsg += logmsg;
                                DWS1Handler.OnConnect += OnConnect;
                                //DWS1Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct11Enable)
                    {
                        try
                        {
                            if (Bottom11IP != "" && Bottom11Port != "")
                            {
                                Bottom11Handler = new SocketLib.TcpClientBase(Bottom11IP, Convert.ToInt32(Bottom11Port));
                                Bottom11Handler.OnExpectedData += OnExpectedData;
                                Bottom11Handler.logmsg += logmsg;
                                Bottom11Handler.OnConnect += OnConnect;
                                //Bottom11Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct12Enable)
                    {
                        try
                        {
                            if (Bottom12IP != "" && Bottom12Port != "")
                            {
                                Bottom12Handler = new SocketLib.TcpClientBase(Bottom12IP, Convert.ToInt32(Bottom12Port));
                                Bottom12Handler.OnExpectedData += OnExpectedData;
                                Bottom12Handler.logmsg += logmsg;
                                Bottom12Handler.OnConnect += OnConnect;
                                //Bottom12Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct13Enable)
                    {
                        try
                        {
                            if (Bottom13IP != "" && Bottom13Port != "")
                            {
                                Bottom13Handler = new SocketLib.TcpClientBase(Bottom13IP, Convert.ToInt32(Bottom13Port));
                                Bottom13Handler.OnExpectedData += OnExpectedData;
                                Bottom13Handler.logmsg += logmsg;
                                Bottom13Handler.OnConnect += OnConnect;
                                //Bottom13Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct14Enable)
                    {
                        try
                        {
                            if (Bottom14IP != "" && Bottom14Port != "")
                            {
                                Bottom14Handler = new SocketLib.TcpClientBase(Bottom14IP, Convert.ToInt32(Bottom14Port));
                                Bottom14Handler.OnExpectedData += OnExpectedData;
                                Bottom14Handler.logmsg += logmsg;
                                Bottom14Handler.OnConnect += OnConnect;
                                //Bottom14Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct15Enable)
                    {
                        try
                        {
                            if (Bottom15IP != "" && Bottom15Port != "")
                            {
                                Bottom15Handler = new SocketLib.TcpClientBase(Bottom15IP, Convert.ToInt32(Bottom15Port));
                                Bottom15Handler.OnExpectedData += OnExpectedData;
                                Bottom15Handler.logmsg += logmsg;
                                Bottom15Handler.OnConnect += OnConnect;
                                //Bottom15Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct21Enable)
                    {
                        try
                        {
                            if (Bottom21IP != "" && Bottom21Port != "")
                            {
                                Bottom21Handler = new SocketLib.TcpClientBase(Bottom21IP, Convert.ToInt32(Bottom21Port));
                                Bottom21Handler.OnExpectedData += OnExpectedData;
                                Bottom21Handler.logmsg += logmsg;
                                Bottom21Handler.OnConnect += OnConnect;
                                //Bottom21Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct22Enable)
                    {
                        try
                        {
                            if (Bottom22IP != "" && Bottom22Port != "")
                            {
                                Bottom22Handler = new SocketLib.TcpClientBase(Bottom22IP, Convert.ToInt32(Bottom22Port));
                                Bottom22Handler.OnExpectedData += OnExpectedData;
                                Bottom22Handler.logmsg += logmsg;
                                Bottom22Handler.OnConnect += OnConnect;
                                //Bottom22Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct23Enable)
                    {
                        try
                        {
                            if (Bottom23IP != "" && Bottom23Port != "")
                            {
                                Bottom23Handler = new SocketLib.TcpClientBase(Bottom23IP, Convert.ToInt32(Bottom23Port));
                                Bottom23Handler.OnExpectedData += OnExpectedData;
                                Bottom23Handler.logmsg += logmsg;
                                Bottom23Handler.OnConnect += OnConnect;
                                //Bottom23Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct24Enable)
                    {
                        try
                        {
                            if (Bottom24IP != "" && Bottom24Port != "")
                            {
                                Bottom24Handler = new SocketLib.TcpClientBase(Bottom24IP, Convert.ToInt32(Bottom24Port));
                                Bottom24Handler.OnExpectedData += OnExpectedData;
                                Bottom24Handler.logmsg += logmsg;
                                Bottom24Handler.OnConnect += OnConnect;
                                //Bottom24Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (Induct25Enable)
                    {
                        try
                        {
                            if (Bottom25IP != "" && Bottom25Port != "")
                            {
                                Bottom25Handler = new SocketLib.TcpClientBase(Bottom25IP, Convert.ToInt32(Bottom25Port));
                                Bottom25Handler.OnExpectedData += OnExpectedData;
                                Bottom25Handler.logmsg += logmsg;
                                Bottom25Handler.OnConnect += OnConnect;
                                //Bottom25Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                    if (LocalServerEnable)
                    {
                        try
                        {
                            if (LocalIP != "" && LocalPort != "")
                            {
                                LocalServerHandler = new SocketLib.TcpServerBase(LocalIP, Convert.ToInt32(LocalPort));
                                LocalServerHandler.OnExpectedData += OnExpectedData;
                                LocalServerHandler.logmsg += logmsg;
                                LocalServerHandler.OnConnect += OnConnect;
                                //Bottom25Handler.Start();
                            }
                        }
                        catch
                        { }
                    }
                }
                catch (Exception ex) { }
                MonitorTimer.Enabled = true;
                Timer1.Interval = 300;
                Timer1.AutoReset = true;
                Timer1.Elapsed += TimeEvent1;//计时器事件
                Timer2.Interval = 300;
                Timer2.AutoReset = true;
                Timer2.Elapsed += TimeEvent2;//计时器事件
                Timer3.Interval = 300;
                Timer3.AutoReset = true;
                Timer3.Elapsed += TimeEvent3;//计时器事件
                Timer4.Interval = 300;
                Timer4.AutoReset = true;
                Timer4.Elapsed += TimeEvent4;//计时器事件
                Timer5.Interval = 1000;
                Timer5.AutoReset = true;
                Timer5.Elapsed += TimeEvent5;//计时器事件
                Timer6.Interval = 1000;
                Timer6.AutoReset = true;
                Timer6.Elapsed += TimeEvent6;//计时器事件
                Timer7.Interval = 1000;
                Timer7.AutoReset = true;
                Timer7.Elapsed += TimeEvent7;//计时器事件
                Timer8.Interval = 1000;
                Timer8.AutoReset = true;
                Timer8.Elapsed += TimeEvent8;//计时器事件
                Timer9.Interval = 5000;
                Timer9.AutoReset = true;
                Timer9.Elapsed += TimeEvent9;//计时器事件
                Timer10.Interval = 5000;
                Timer10.AutoReset = true;
                Timer10.Elapsed += TimeEvent10;//计时器事件
                Timer11.Interval = 5000;
                Timer11.AutoReset = true;
                Timer11.Elapsed += TimeEvent11;//计时器事件
                Timer12.Interval = 5000;
                Timer12.AutoReset = true;
                Timer12.Elapsed += TimeEvent12;//计时器事件
                Timer13.Interval = 1000;
                Timer13.AutoReset = true;
                Timer13.Elapsed += TimeEvent13;//计时器事件
                Timer14.Interval = 1000;
                Timer14.AutoReset = true;
                Timer14.Elapsed += TimeEvent14;//计时器事件
                Timer15.Interval = 1000;
                Timer15.AutoReset = true;
                Timer15.Elapsed += TimeEvent15;//计时器事件
                Timer16.Interval = 1000;
                Timer16.AutoReset = true;
                Timer16.Elapsed += TimeEvent16;//计时器事件
                Timer17.Interval = 1000;
                Timer17.AutoReset = true;
                Timer17.Elapsed += TimeEvent17;//计时器事件
                Timer18.Interval = 1000;
                Timer18.AutoReset = true;
                Timer18.Elapsed += TimeEvent18;//计时器事件
                Timer19.Interval = 1000;
                Timer19.AutoReset = true;
                Timer19.Elapsed += TimeEvent19;//计时器事件
                Timer20.Interval = 1000;
                Timer20.AutoReset = true;
                Timer20.Elapsed += TimeEvent20;//计时器事件
                Timer21.Interval = 5000;
                Timer21.AutoReset = true;
                Timer21.Elapsed += TimeEvent21;//计时器事件
                Timer22.Interval = 1000;
                Timer22.AutoReset = true;
                Timer22.Elapsed += TimeEvent22;//计时器事件
                Timer23.Interval = 1000;
                Timer23.AutoReset = true;
                Timer23.Elapsed += TimeEvent23;//计时器事件
                Timer24.Interval = 1000;
                Timer24.AutoReset = true;
                Timer24.Elapsed += TimeEvent24;//计时器事件
                Timer25.Interval = 1000;
                Timer25.AutoReset = true;
                Timer25.Elapsed += TimeEvent25;//计时器事件
                Timer26.Interval = 1000;
                Timer26.AutoReset = true;
                Timer26.Elapsed += TimeEvent26;//计时器事件
                Timer27.Interval = 1000;
                Timer27.AutoReset = true;
                Timer27.Elapsed += TimeEvent27;//计时器事件
                Timer28.Interval = 1000;
                Timer28.AutoReset = true;
                Timer28.Elapsed += TimeEvent28;//计时器事件
                Timer29.Interval = 1000;
                Timer29.AutoReset = true;
                Timer29.Elapsed += TimeEvent29;//计时器事件
                Timer30.Interval = 1000;
                Timer30.AutoReset = true;
                Timer30.Elapsed += TimeEvent30;//计时器事件
                Timer31.Interval = 1000;
                Timer31.AutoReset = true;
                Timer31.Elapsed += TimeEvent31;//计时器事件
                Timer32.Interval = 5000;
                Timer32.AutoReset = true;
                Timer32.Elapsed += TimeEvent32;//计时器事件
                Timer33.Interval = 300;
                Timer33.AutoReset = true;
                Timer33.Elapsed += TimeEvent33;//计时器事件
                Timer34.Interval = 1000;
                Timer34.AutoReset = true;
                Timer34.Elapsed += TimeEvent34;//计时器事件
                Timer35.Interval = 5000;
                Timer35.AutoReset = true;
                Timer35.Elapsed += TimeEvent35;//计时器事件
                Timer36.Interval = 1000;
                Timer36.AutoReset = true;
                Timer36.Elapsed += TimeEvent36;//计时器事件
                Timer37.Interval = 1000;
                Timer37.AutoReset = true;
                Timer37.Elapsed += TimeEvent37;//计时器事件
                Timer38.Interval = 1000;
                Timer38.AutoReset = true;
                Timer38.Elapsed += TimeEvent38;//计时器事件
                Timer39.Interval = 1000;
                Timer39.AutoReset = true;
                Timer39.Elapsed += TimeEvent39;//计时器事件
                Timer40.Interval = 1000;
                Timer40.AutoReset = true;
                Timer40.Elapsed += TimeEvent40;//计时器事件
                Timer41.Interval = 1000;
                Timer41.AutoReset = true;
                Timer41.Elapsed += TimeEvent41;//计时器事件
                Timer42.Interval = 1000;
                Timer42.AutoReset = true;
                Timer42.Elapsed += TimeEvent42;//计时器事件
                Timer43.Interval = 1000;
                Timer43.AutoReset = true;
                Timer43.Elapsed += TimeEvent43;//计时器事件
                Timer44.Interval = 1000;
                Timer44.AutoReset = true;
                Timer44.Elapsed += TimeEvent44;//计时器事件
                Timer45.Interval = 1000;
                Timer45.AutoReset = true;
                Timer45.Elapsed += TimeEvent45;//计时器事件
                Timer46.Interval = 1000;
                Timer46.AutoReset = true;
                Timer46.Elapsed += TimeEvent46;//计时器事件
                Timer47.Interval = 1000;
                Timer47.AutoReset = true;
                Timer47.Elapsed += TimeEvent47;//计时器事件
                Timer48.Interval = 1000;
                Timer48.AutoReset = true;
                Timer48.Elapsed += TimeEvent48;//计时器事件
                Timer49.Interval = 1000;
                Timer49.AutoReset = true;
                Timer49.Elapsed += TimeEvent49;//计时器事件
                Start.Enabled = false;
                Stop.Enabled = true;
                delimiterBox.Enabled = false;
                calibrateBtn.Enabled = false;
                captionOptionList.Enabled = false;
                FTPOKBox.Enabled = false;
                FTPNOKBox.Enabled = false;
                HTTPOKBox.Enabled = false;
                HTTPNOKBox.Enabled = false;
                HttpRegexUrlBox.Enabled = false;
                regexMachineCodeBox.Enabled = false;
                machineCodeBox.Enabled = false;
                HttpCheckUrlBox.Enabled = false;
                HttpUploadUrlBox.Enabled = false;
                SCADAIPBox.Enabled = false;
                SCADAPortBox.Enabled = false;
                regexMasterIPBox1.Enabled = false;
                regexMasterPortBox1.Enabled = false;
                regexMasterIPBox2.Enabled = false;
                regexMasterPortBox2.Enabled = false;
                regexMasterIPBox3.Enabled = false;
                regexMasterPortBox3.Enabled = false;
                regexMasterIPBox4.Enabled = false;
                regexMasterPortBox4.Enabled = false;
                ServerIPBox.Enabled = false;
                PortBox.Enabled = false;
                InductorStation1NoBox.Enabled = false;
                InductorStation2NoBox.Enabled = false;
                UploadModeList.Enabled = false;
                UserBox.Enabled = false;
                PasswordBox.Enabled = false;
                ServerIPBox2.Enabled = false;
                PortBox2.Enabled = false;
                UserBox2.Enabled = false;
                PasswordBox2.Enabled = false;
                InductFtpRootBox.Enabled = false;
                FtpRootBox1.Enabled = false;
                FtpRootBox2.Enabled = false;
                FtpRootBox3.Enabled = false;
                FtpRootBox4.Enabled = false;
                ImageRootBox1.Enabled = false;
                ImageRootBox2.Enabled = false;
                ImageRootBox3.Enabled = false;
                ImageRootBox4.Enabled = false;
                Folder1Combo.Enabled = false;
                Folder2Combo.Enabled = false;
                Folder3Combo.Enabled = false;
                Folder4Combo.Enabled = false;
                Folder5Combo.Enabled = false;
                FTPFolder1Combo.Enabled = false;
                FTPFolder2Combo.Enabled = false;
                FTPFolder3Combo.Enabled = false;
                FTPFolder4Combo.Enabled = false;
                FTPFolder5Combo.Enabled = false;
                ExtDelayCombo1.Enabled = false;
                ExtDelayCombo2.Enabled = false;
                ExtDelayCombo3.Enabled = false;
                ExtDelayCombo4.Enabled = false;
                BottomCamNoCombo1.Enabled = false;
                BottomCamNoCombo2.Enabled = false;
                BottomCamNoCombo3.Enabled = false;
                BottomCamNoCombo4.Enabled = false;
                BarcodeImgModeCombo.Enabled = false;
                InductIDCombo1.Enabled = false;
                InductIDCombo2.Enabled = false;
                InductIDCombo3.Enabled = false;
                InductIDCombo4.Enabled = false;
                SorterTimeStampCombo.Enabled = false;
                TrayIDCombo.Enabled = false;
                BoxIDCombo.Enabled = false;
                ResultCombo.Enabled = false;
                BarcodeCombo.Enabled = false;
                SorterStationNoCombo.Enabled = false;
                CamNoCombo1.Enabled = false;
                CamNoCombo2.Enabled = false;
                CamNoCombo3.Enabled = false;
                CamNoCombo4.Enabled = false;
                InductFtpRootButton.Enabled = false;
                SelectFtpRootButton1.Enabled = false;
                SelectFtpRootButton2.Enabled = false;
                SelectFtpRootButton3.Enabled = false;
                SelectFtpRootButton4.Enabled = false;
                SelectImageRootButton1.Enabled = false;
                SelectImageRootButton2.Enabled = false;
                SelectImageRootButton3.Enabled = false;
                SelectImageRootButton4.Enabled = false;
                PackageBorderCheck.Enabled = false;
                CaptionEnableCheck.Enabled = false;
                EncryptionEnableCheck.Enabled = false;
                GridLineEnableCheck.Enabled = false;
                NRAnalysisEnableCheck.Enabled = false;
                ExtCaptionEnableCheck.Enabled = false;
                Sta1OnBox.Enabled = false;
                Sta2OnBox.Enabled = false;
                Sta3OnBox.Enabled = false;
                Sta4OnBox.Enabled = false;
                FtpFileNameBox.Enabled = false;
                FtpFileNameBox2.Enabled = false;
                FileNameBox.Enabled = false;
                HttpFileNameBox.Enabled = false;
                DimWgtCombo.Enabled = false;
                PosCombo.Enabled = false;
                BarcodePosCombo.Enabled = false;
                CaptionFormatBox.Enabled = false;
                PicStorDaysText.Enabled = false;
                gs.PixelSizeText.Enabled = false;
                gs.FocusLengthText.Enabled = false;
                gs.ReadAngleText.Enabled = false;
                gs.MountingHeightText.Enabled = false;
                gs.MaxParcelHeightText.Enabled = false;
                gs.BtnSave.Enabled = false;
                if (!CheckDuplication(FtpRoots) && !CheckDuplication(StorageRoots))
                {
                    if (Regex.IsMatch(FtpRoot1, UrlPattern) && Regex.IsMatch(StorageRoot1, UrlPattern) && Sta1On)
                    {
                        Timer1Enabled = true;
                        Timer1.Start();
                    }
                    else if (Sta1On)
                    {
                        MessageBox.Show("工位一路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Regex.IsMatch(FtpRoot2, UrlPattern) && Regex.IsMatch(StorageRoot2, UrlPattern) && Sta2On)
                    {
                        Timer2Enabled = true;
                        Timer2.Start();
                    }
                    else if (Sta2On)
                    {
                        MessageBox.Show("工位二路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Regex.IsMatch(FtpRoot3, UrlPattern) && Regex.IsMatch(StorageRoot3, UrlPattern) && Sta3On)
                    {
                        Timer3Enabled = true;
                        Timer3.Start();
                    }
                    else if (Sta3On)
                    {
                        MessageBox.Show("工位三路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Regex.IsMatch(FtpRoot4, UrlPattern) && Regex.IsMatch(StorageRoot4, UrlPattern) && Sta4On)
                    {
                        Timer4Enabled = true;
                        Timer4.Start();
                    }
                    else if (Sta4On)
                    {
                        MessageBox.Show("工位四路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Timer5.Start();
                    Timer6.Start();
                    Timer7.Start();
                    Timer8.Start();
                    Timer9.Start();
                    Timer10.Start();
                    Timer11.Start();
                    Timer12.Start();
                    Timer5Enabled = true;
                    Timer6Enabled = true;
                    Timer7Enabled = true;
                    Timer8Enabled = true;
                    Timer9Enabled = true;
                    Timer10Enabled = true;
                    Timer11Enabled = true;
                    Timer12Enabled = true;
                    if (Sta1On)
                    {
                        Timer13Enabled = true;
                        Timer14Enabled = true;
                        Timer13.Start();
                        Timer14.Start();
                    }
                    if (Sta2On)
                    {
                        Timer15Enabled = true;
                        Timer16Enabled = true;
                        Timer15.Start();
                        Timer16.Start();
                    }
                    if (Sta3On)
                    {
                        Timer17Enabled = true;
                        Timer18Enabled = true;
                        Timer17.Start();
                        Timer18.Start();
                    }
                    if (Sta4On)
                    {
                        Timer19Enabled = true;
                        Timer20Enabled = true;
                        Timer19.Start();
                        Timer20.Start();
                    }
                    Timer21.Start();
                    Timer22.Start();
                    Timer23.Start();
                    Timer24.Start();
                    Timer25.Start();
                    Timer26.Start();
                    Timer27.Start();
                    Timer28.Start();
                    Timer29.Start();
                    Timer30.Start();
                    Timer31.Start();
                    Timer32.Start();
                    Timer33.Start();
                    Timer34.Start();
                    Timer35.Start();
                    Timer36.Start();
                    Timer37.Start();
                    Timer38.Start();
                    Timer39.Start();
                    Timer40.Start();
                    Timer41.Start();
                    Timer42.Start();
                    Timer43.Start();
                    Timer44.Start();
                    Timer45.Start();
                    Timer46.Start();
                    Timer47.Start();
                    Timer48.Start();
                    Timer49.Start();
                    Timer21Enabled = true;
                    Timer22Enabled = true;
                    Timer23Enabled = true;
                    Timer24Enabled = true;
                    Timer25Enabled = true;
                    Timer26Enabled = true;
                    Timer27Enabled = true;
                    Timer28Enabled = true;
                    Timer29Enabled = true;
                    Timer30Enabled = true;
                    Timer31Enabled = true;
                    Timer32Enabled = true;
                    Timer33Enabled = true;
                    Timer34Enabled = true;
                    Timer35Enabled = true;
                    Timer36Enabled = true;
                    Timer37Enabled = true;
                    Timer38Enabled = true;
                    Timer39Enabled = true;
                    Timer40Enabled = true;
                    Timer41Enabled = true;
                    Timer42Enabled = true;
                    Timer43Enabled = true;
                    Timer44Enabled = true;
                    Timer45Enabled = true;
                    Timer46Enabled = true;
                    Timer47Enabled = true;
                    Timer48Enabled = true;
                    Timer49Enabled = true;
                    threadclient11.Start();
                    threadclient12.Start();
                    threadclient13.Start();
                    threadclient14.Start();
                    threadclient15.Start();
                    threadclient21.Start();
                    threadclient22.Start();
                    threadclient23.Start();
                    threadclient24.Start();
                    threadclient25.Start();
                    threadclientDWS1.Start();
                    threadserver.Start();
                    threadDWSProcess.Start();
                    threadresidual1.Start(ext_caption_url1);
                    threadresidual2.Start(ext_caption_url2);
                    threadresidual3.Start(ext_caption_url3);
                    threadresidual4.Start(ext_caption_url4);
                /*
                    CaptionThread1 = new Thread(new ParameterizedThreadStart(AddCaptionInThreadMethod));
                    CaptionThread2 = new Thread(new ParameterizedThreadStart(AddCaptionInThreadMethod));
                    CaptionThread3 = new Thread(new ParameterizedThreadStart(AddCaptionInThreadMethod));
                    CaptionThread4 = new Thread(new ParameterizedThreadStart(AddCaptionInThreadMethod));
                    CaptionThread5 = new Thread(new ParameterizedThreadStart(AddCaptionInThreadMethod));
                    CaptionThread1.Start(caption_url1);
                    CaptionThread2.Start(caption_url2);
                    CaptionThread3.Start(caption_url3);
                    CaptionThread4.Start(caption_url4);
                    CaptionThread5.Start(caption_url5);
                */
                }
                else
                {
                    MessageBox.Show("不同工位的FTP根目录或存图根目录相同", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message+"\r\n"+ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }
        private void Stop_Click(object sender, EventArgs e)
        {
            try
            {
                Login login = new Login();
                login.ShowDialog();
                while (login.dialogOK == false && login.dialogNOK == false && !login.IsDisposed)
                {
                    if (!login.Visible)
                    {
                        break;
                    }
                }
                if (login.dialogOK)
                {
                    Start.Enabled = true;
                    Stop.Enabled = false;
                    running = false;
                    delimiterBox.Enabled = true;
                    calibrateBtn.Enabled = true;
                    captionOptionList.Enabled = true;
                    FTPOKBox.Enabled = true;
                    FTPNOKBox.Enabled = true;
                    HTTPOKBox.Enabled = true;
                    HTTPNOKBox.Enabled = true;
                    HttpRegexUrlBox.Enabled = true;
                    regexMachineCodeBox.Enabled = true;
                    machineCodeBox.Enabled = true;
                    HttpCheckUrlBox.Enabled = true;
                    HttpUploadUrlBox.Enabled = true;
                    regexMasterIPBox1.Enabled = true;
                    regexMasterPortBox1.Enabled = true;
                    regexMasterIPBox2.Enabled = true;
                    regexMasterPortBox2.Enabled = true;
                    regexMasterIPBox3.Enabled = true;
                    regexMasterPortBox3.Enabled = true;
                    regexMasterIPBox4.Enabled = true;
                    regexMasterPortBox4.Enabled = true;
                    ServerIPBox.Enabled = true;
                    PortBox.Enabled = true;
                    ServerIPBox2.Enabled = true;
                    PortBox2.Enabled = true;
                    SCADAIPBox.Enabled = true;
                    SCADAPortBox.Enabled = true;
                    InductorStation1NoBox.Enabled = true;
                    InductorStation2NoBox.Enabled = true;
                    UploadModeList.Enabled = true;
                    UserBox.Enabled = true;
                    PasswordBox.Enabled = true;
                    UserBox2.Enabled = true;
                    PasswordBox2.Enabled = true;
                    InductFtpRootButton.Enabled = true;
                    SelectFtpRootButton1.Enabled = true;
                    SelectFtpRootButton2.Enabled = true;
                    SelectFtpRootButton3.Enabled = true;
                    SelectFtpRootButton4.Enabled = true;
                    SelectImageRootButton1.Enabled = true;
                    SelectImageRootButton2.Enabled = true;
                    SelectImageRootButton3.Enabled = true;
                    SelectImageRootButton4.Enabled = true;
                    Folder1Combo.Enabled = true;
                    Folder2Combo.Enabled = true;
                    Folder3Combo.Enabled = true;
                    Folder4Combo.Enabled = true;
                    Folder5Combo.Enabled = true;
                    FTPFolder1Combo.Enabled = true;
                    FTPFolder2Combo.Enabled = true;
                    FTPFolder3Combo.Enabled = true;
                    FTPFolder4Combo.Enabled = true;
                    FTPFolder5Combo.Enabled = true;
                    ExtDelayCombo1.Enabled = true;
                    ExtDelayCombo2.Enabled = true;
                    ExtDelayCombo3.Enabled = true;
                    ExtDelayCombo4.Enabled = true;
                    BottomCamNoCombo1.Enabled = true;
                    BottomCamNoCombo2.Enabled = true;
                    BottomCamNoCombo3.Enabled = true;
                    BottomCamNoCombo4.Enabled = true;
                    BarcodeImgModeCombo.Enabled = true;
                    InductIDCombo1.Enabled = true;
                    InductIDCombo2.Enabled = true;
                    InductIDCombo3.Enabled = true;
                    InductIDCombo4.Enabled = true;
                    SorterTimeStampCombo.Enabled = true;
                    BoxIDCombo.Enabled = true;
                    TrayIDCombo.Enabled = true;
                    ResultCombo.Enabled = true;
                    BarcodeCombo.Enabled = true;
                    SorterStationNoCombo.Enabled = true;
                    CamNoCombo1.Enabled = true;
                    CamNoCombo2.Enabled = true;
                    CamNoCombo3.Enabled = true;
                    CamNoCombo4.Enabled = true;
                    PackageBorderCheck.Enabled = true;
                    CaptionEnableCheck.Enabled = true;
                    EncryptionEnableCheck.Enabled = true;
                    GridLineEnableCheck.Enabled = true;
                    NRAnalysisEnableCheck.Enabled = true;
                    ExtCaptionEnableCheck.Enabled = true;
                    Sta1OnBox.Enabled = true;
                    Sta2OnBox.Enabled = true;
                    Sta3OnBox.Enabled = true;
                    Sta4OnBox.Enabled = true;
                    FtpFileNameBox.Enabled = true;
                    FtpFileNameBox2.Enabled = true;
                    FileNameBox.Enabled = true;
                    HttpFileNameBox.Enabled = true;
                    DimWgtCombo.Enabled = true;
                    PosCombo.Enabled = true;
                    BarcodePosCombo.Enabled = true;
                    CaptionFormatBox.Enabled = true;
                    PicStorDaysText.Enabled = true;
                    gs.PixelSizeText.Enabled = true;
                    gs.FocusLengthText.Enabled = true;
                    gs.ReadAngleText.Enabled = true;
                    gs.MountingHeightText.Enabled = true;
                    gs.MaxParcelHeightText.Enabled = true;
                    gs.BtnSave.Enabled = true;
                    MonitorTimer.Enabled = false;
                    Timer1.Stop();
                    Timer2.Stop();
                    Timer3.Stop();
                    Timer4.Stop();
                    Timer5.Stop();
                    Timer6.Stop();
                    Timer7.Stop();
                    Timer8.Stop();
                    Timer9.Stop();
                    Timer10.Stop();
                    Timer11.Stop();
                    Timer12.Stop();
                    Timer13.Stop();
                    Timer14.Stop();
                    Timer15.Stop();
                    Timer16.Stop();
                    Timer17.Stop();
                    Timer18.Stop();
                    Timer19.Stop();
                    Timer20.Stop();
                    Timer21.Stop();
                    Timer22.Stop();
                    Timer23.Stop();
                    Timer24.Stop();
                    Timer25.Stop();
                    Timer26.Stop();
                    Timer27.Stop();
                    Timer28.Stop();
                    Timer29.Stop();
                    Timer30.Stop();
                    Timer31.Stop();
                    Timer32.Stop();
                    Timer33.Stop();
                    Timer34.Stop();
                    Timer35.Stop();
                    Timer36.Stop();
                    Timer37.Stop();
                    Timer38.Stop();
                    Timer39.Stop();
                    Timer40.Stop();
                    Timer41.Stop();
                    Timer42.Stop();
                    Timer43.Stop();
                    Timer44.Stop();
                    Timer45.Stop();
                    Timer46.Stop();
                    Timer47.Stop();
                    Timer48.Stop();
                    Timer49.Stop();
                    Timer1Enabled = false;
                    Timer2Enabled = false;
                    Timer3Enabled = false;
                    Timer4Enabled = false;
                    Timer5Enabled = false;
                    Timer6Enabled = false;
                    Timer7Enabled = false;
                    Timer8Enabled = false;
                    Timer9Enabled = false;
                    Timer10Enabled = false;
                    Timer11Enabled = false;
                    Timer12Enabled = false;
                    Timer13Enabled = false;
                    Timer14Enabled = false;
                    Timer15Enabled = false;
                    Timer16Enabled = false;
                    Timer17Enabled = false;
                    Timer18Enabled = false;
                    Timer19Enabled = false;
                    Timer20Enabled = false;
                    Timer21Enabled = false;
                    Timer22Enabled = false;
                    Timer23Enabled = false;
                    Timer24Enabled = false;
                    Timer25Enabled = false;
                    Timer26Enabled = false;
                    Timer27Enabled = false;
                    Timer28Enabled = false;
                    Timer29Enabled = false;
                    Timer30Enabled = false;
                    Timer31Enabled = false;
                    Timer32Enabled = false;
                    Timer33Enabled = false;
                    Timer34Enabled = false;
                    Timer35Enabled = false;
                    Timer36Enabled = false;
                    Timer37Enabled = false;
                    Timer38Enabled = false;
                    Timer39Enabled = false;
                    Timer40Enabled = false;
                    Timer41Enabled = false;
                    Timer42Enabled = false;
                    Timer43Enabled = false;
                    Timer44Enabled = false;
                    Timer45Enabled = false;
                    Timer46Enabled = false;
                    Timer47Enabled = false;
                    Timer48Enabled = false;
                    Timer49Enabled = false;
                }
                login.Dispose();
                
            }
            catch (Exception ex) { }
        }
        private void MainFrm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Login login = new Login();
                login.ShowDialog();
                while (login.dialogOK == false && login.dialogNOK == false && !login.IsDisposed)
                {
                    if (!login.Visible)
                    {
                        break;
                    }
                }
                if (login.dialogOK)
                {
                    this.FormClosing -= new FormClosingEventHandler(this.MainFrm_FormClosing);
                    Process[] processes = Process.GetProcessesByName("DWSInterface");
                    foreach(Process p in processes)
                    {
                        try
                        {
                            if (p != null)
                            {
                                p.Kill();
                                p.Dispose();
                            }
                        }
                        catch { }
                    }
                    Application.Exit();
                    //System.Environment.Exit(0);
                    Process.GetCurrentProcess().Kill();
                    
                }
                else
                {
                    e.Cancel = true;
                }
                login.Dispose();
            }
            catch (Exception ex) { }
        }
        private void Start_Click(object sender, EventArgs e)
        {
            
            PicStorDaysText.Text = Regex.IsMatch(PicStorDaysText.Text, DaysPattern) ? PicStorDaysText.Text : DefaultPicStorDaysText;
            Folder1Combo.Text = Regex.IsMatch(Folder1Combo.Text, NonSpacePattern) ? Folder1Combo.Text : DefaultSubFolder;
            Folder2Combo.Text = Regex.IsMatch(Folder2Combo.Text, NonSpacePattern) ? Folder2Combo.Text : DefaultSubFolder;
            Folder3Combo.Text = Regex.IsMatch(Folder3Combo.Text, NonSpacePattern) ? Folder3Combo.Text : DefaultSubFolder;
            Folder4Combo.Text = Regex.IsMatch(Folder4Combo.Text, NonSpacePattern) ? Folder4Combo.Text : DefaultSubFolder;
            Folder5Combo.Text = Regex.IsMatch(Folder5Combo.Text, NonSpacePattern) ? Folder5Combo.Text : DefaultSubFolder;
            Folder1Combo.Text = Folder1Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            Folder2Combo.Text = Folder2Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            Folder3Combo.Text = Folder3Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            Folder4Combo.Text = Folder4Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            Folder5Combo.Text = Folder5Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            FTPFolder1Combo.Text = Regex.IsMatch(FTPFolder1Combo.Text, NonSpacePattern) ? FTPFolder1Combo.Text : DefaultSubFolder;
            FTPFolder2Combo.Text = Regex.IsMatch(FTPFolder2Combo.Text, NonSpacePattern) ? FTPFolder2Combo.Text : DefaultSubFolder;
            FTPFolder3Combo.Text = Regex.IsMatch(FTPFolder3Combo.Text, NonSpacePattern) ? FTPFolder3Combo.Text : DefaultSubFolder;
            FTPFolder4Combo.Text = Regex.IsMatch(FTPFolder4Combo.Text, NonSpacePattern) ? FTPFolder4Combo.Text : DefaultSubFolder;
            FTPFolder5Combo.Text = Regex.IsMatch(FTPFolder5Combo.Text, NonSpacePattern) ? FTPFolder5Combo.Text : DefaultSubFolder;
            FTPFolder1Combo.Text = FTPFolder1Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
            FTPFolder2Combo.Text = FTPFolder2Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
            FTPFolder3Combo.Text = FTPFolder3Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
            FTPFolder4Combo.Text = FTPFolder4Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
            FTPFolder5Combo.Text = FTPFolder5Combo.Text.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "").Replace(@"~", "");
            FtpFileNameBox.Text = Regex.IsMatch(FtpFileNameBox.Text, NonSpacePattern) ? FtpFileNameBox.Text : DefaultFileNameFormat;
            FtpFileNameBox.Text = FtpFileNameBox.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            FtpFileNameBox2.Text = Regex.IsMatch(FtpFileNameBox2.Text, NonSpacePattern) ? FtpFileNameBox2.Text : DefaultFileNameFormat;
            FtpFileNameBox2.Text = FtpFileNameBox2.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            FileNameBox.Text = Regex.IsMatch(FileNameBox.Text, NonSpacePattern) ? FileNameBox.Text : DefaultFileNameFormat;
            FileNameBox.Text = FileNameBox.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            HttpFileNameBox.Text = Regex.IsMatch(HttpFileNameBox.Text, NonSpacePattern) ? HttpFileNameBox.Text : DefaultFileNameFormat;
            HttpFileNameBox.Text = HttpFileNameBox.Text.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
            InductFtpRoot = InductFtpRootBox.Text;
            FtpRoot1 = FtpRootBox1.Text;
            FtpRoot2 = FtpRootBox2.Text;
            FtpRoot3 = FtpRootBox3.Text;
            FtpRoot4 = FtpRootBox4.Text;
            FtpRoots[0] = FtpRoot1;
            FtpRoots[1] = FtpRoot2;
            FtpRoots[2] = FtpRoot3;
            FtpRoots[3] = FtpRoot4;
            StorageRoot1 = ImageRootBox1.Text;
            StorageRoot2 = ImageRootBox2.Text;
            StorageRoot3 = ImageRootBox3.Text;
            StorageRoot4 = ImageRootBox4.Text;
            StorageRoots[0] = StorageRoot1;
            StorageRoots[1] = StorageRoot2;
            StorageRoots[2] = StorageRoot3;
            StorageRoots[3] = StorageRoot4;
            bool flag1 = Regex.IsMatch(regexMasterIPBox1.Text, IPPattern) && Regex.IsMatch(regexMasterPortBox1.Text, PortPattern) && Regex.IsMatch(regexMasterIPBox2.Text, IPPattern) && Regex.IsMatch(regexMasterPortBox2.Text, PortPattern) && Regex.IsMatch(regexMasterIPBox3.Text, IPPattern) && Regex.IsMatch(regexMasterPortBox3.Text, PortPattern) && Regex.IsMatch(regexMasterIPBox4.Text, IPPattern) && Regex.IsMatch(regexMasterPortBox4.Text, PortPattern) && Regex.IsMatch(ServerIPBox.Text, IPPattern) && Regex.IsMatch(PortBox.Text, PortPattern) && Regex.IsMatch(ServerIPBox2.Text, IPPattern) && Regex.IsMatch(PortBox2.Text, PortPattern) && Regex.IsMatch(SCADAIPBox.Text, IPPattern) && Regex.IsMatch(SCADAPortBox.Text, PortPattern);
            bool flag2 = IsValidFileName(FileNameBox.Text) && IsValidFileName(HttpFileNameBox.Text) && IsValidFileName(FtpFileNameBox.Text) && IsValidFileName(FtpFileNameBox2.Text);
            bool flag3 = Regex.IsMatch(FtpRootBox1.Text, UrlPattern) || !Sta1On;
            bool flag4 = Regex.IsMatch(FtpRootBox2.Text, UrlPattern) || !Sta2On;
            bool flag5 = Regex.IsMatch(FtpRootBox3.Text, UrlPattern) || !Sta3On;
            bool flag6 = Regex.IsMatch(FtpRootBox4.Text, UrlPattern) || !Sta4On;
            bool flag7 = Regex.IsMatch(ImageRootBox1.Text, UrlPattern) || !Sta1On;
            bool flag8 = Regex.IsMatch(ImageRootBox2.Text, UrlPattern) || !Sta2On;
            bool flag9 = Regex.IsMatch(ImageRootBox3.Text, UrlPattern) || !Sta3On;
            bool flag10 = Regex.IsMatch(ImageRootBox4.Text, UrlPattern) || !Sta4On;
            bool flag11 = !CheckDuplication(FtpRoots);
            bool flag12 = !CheckDuplication(StorageRoots);
            bool flag13 = HttpUploadUrlBox.Text.StartsWith(@"http://") && HttpCheckUrlBox.Text.StartsWith(@"http://") && HttpRegexUrlBox.Text.StartsWith(@"http://");
            bool flag14 = Regex.IsMatch(InductFtpRootBox.Text, UrlPattern);
            bool allflag = flag1 && flag2 && flag3 && flag4 && flag5 && flag6 && flag7 && flag8 && flag9 && flag10 && flag11 && flag12 && flag13 && flag14;
            try
            {
                if (allflag)
                {
                    LogFailureNum1 = 0;
                    LogFailureNum2 = 0;
                    warningflag1 = false;
                    warningflag2 = false;
                    lock(OpenConfigLock)
                    {
                        try
                        {
                            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                            config.AppSettings.Settings["AllowChineseCaption"].Value = (captionOptionList.Text == "包含中文字符"|| captionOptionList.Text == "AllowChinese") ? "true" : "false";
                            config.AppSettings.Settings["FTPOKMes"].Value = FTPOKBox.Text;
                            config.AppSettings.Settings["FTPNOKMes"].Value = FTPNOKBox.Text;
                            config.AppSettings.Settings["HTTPOKMes"].Value = HTTPOKBox.Text;
                            config.AppSettings.Settings["HTTPNOKMes"].Value = HTTPNOKBox.Text;
                            config.AppSettings.Settings["HttpRegexUrl"].Value = HttpRegexUrlBox.Text;
                            config.AppSettings.Settings["regexMachineCode"].Value = regexMachineCodeBox.Text;
                            config.AppSettings.Settings["machineCode"].Value = machineCodeBox.Text;
                            config.AppSettings.Settings["HttpCheckUrl"].Value = HttpCheckUrlBox.Text;
                            config.AppSettings.Settings["HttpUploadUrl"].Value = HttpUploadUrlBox.Text;
                            config.AppSettings.Settings["PicStorDays"].Value = PicStorDaysText.Text;
                            
                            config.AppSettings.Settings["InductFtpRoot"].Value = InductFtpRootBox.Text;
                            config.AppSettings.Settings["ftp_image_dir1"].Value = FtpRootBox1.Text;
                            config.AppSettings.Settings["ftp_image_dir2"].Value = FtpRootBox2.Text;
                            config.AppSettings.Settings["ftp_image_dir3"].Value = FtpRootBox3.Text;
                            config.AppSettings.Settings["ftp_image_dir4"].Value = FtpRootBox4.Text;
                            config.AppSettings.Settings["FtpSubFolder1"].Value = FTPFolder1Combo.Text;
                            config.AppSettings.Settings["FtpSubFolder2"].Value = FTPFolder2Combo.Text;
                            config.AppSettings.Settings["FtpSubFolder3"].Value = FTPFolder3Combo.Text;
                            config.AppSettings.Settings["FtpSubFolder4"].Value = FTPFolder4Combo.Text;
                            config.AppSettings.Settings["FtpSubFolder5"].Value = FTPFolder5Combo.Text;
                            config.AppSettings.Settings["SubFolder1"].Value = Folder1Combo.Text;
                            config.AppSettings.Settings["SubFolder2"].Value = Folder2Combo.Text;
                            config.AppSettings.Settings["SubFolder3"].Value = Folder3Combo.Text;
                            config.AppSettings.Settings["SubFolder4"].Value = Folder4Combo.Text;
                            config.AppSettings.Settings["SubFolder5"].Value = Folder5Combo.Text;
                            config.AppSettings.Settings["ExtCaptionDelay1"].Value = ExtDelayCombo1.Text;
                            config.AppSettings.Settings["ExtCaptionDelay2"].Value = ExtDelayCombo2.Text;
                            config.AppSettings.Settings["ExtCaptionDelay3"].Value = ExtDelayCombo3.Text;
                            config.AppSettings.Settings["ExtCaptionDelay4"].Value = ExtDelayCombo4.Text;
                            config.AppSettings.Settings["BottomCamNo1"].Value = BottomCamNoCombo1.Text;
                            config.AppSettings.Settings["BottomCamNo2"].Value = BottomCamNoCombo2.Text;
                            config.AppSettings.Settings["BottomCamNo3"].Value = BottomCamNoCombo3.Text;
                            config.AppSettings.Settings["BottomCamNo4"].Value = BottomCamNoCombo4.Text;
                            config.AppSettings.Settings["BarcodeImgMode"].Value = BarcodeImgModeCombo.Text;
                            config.AppSettings.Settings["InductID1"].Value = InductIDCombo1.Text;
                            config.AppSettings.Settings["InductID2"].Value = InductIDCombo2.Text;
                            config.AppSettings.Settings["InductID3"].Value = InductIDCombo3.Text;
                            config.AppSettings.Settings["InductID4"].Value = InductIDCombo4.Text;
                            config.AppSettings.Settings["SorterTimeStamp"].Value = SorterTimeStampCombo.Text;
                            config.AppSettings.Settings["SorterBoxID"].Value = BoxIDCombo.Text;
                            config.AppSettings.Settings["SorterTrayID"].Value = TrayIDCombo.Text;
                            config.AppSettings.Settings["SorterResult"].Value = ResultCombo.Text;
                            config.AppSettings.Settings["SorterBarcode"].Value = BarcodeCombo.Text;
                            config.AppSettings.Settings["SorterStationNo"].Value = SorterStationNoCombo.Text;
                            config.AppSettings.Settings["CamNo1"].Value = CamNoCombo1.Text;
                            config.AppSettings.Settings["CamNo2"].Value = CamNoCombo2.Text;
                            config.AppSettings.Settings["CamNo3"].Value = CamNoCombo3.Text;
                            config.AppSettings.Settings["CamNo4"].Value = CamNoCombo4.Text;
                            config.AppSettings.Settings["SCADAIP"].Value = SCADAIPBox.Text;
                            config.AppSettings.Settings["SCADAPort"].Value = SCADAPortBox.Text;
                            config.AppSettings.Settings["RegexMasterIP1"].Value = regexMasterIPBox1.Text;
                            config.AppSettings.Settings["RegexMasterPort1"].Value = regexMasterPortBox1.Text;
                            config.AppSettings.Settings["RegexMasterIP2"].Value = regexMasterIPBox2.Text;
                            config.AppSettings.Settings["RegexMasterPort2"].Value = regexMasterPortBox2.Text;
                            config.AppSettings.Settings["RegexMasterIP3"].Value = regexMasterIPBox3.Text;
                            config.AppSettings.Settings["RegexMasterPort3"].Value = regexMasterPortBox3.Text;
                            config.AppSettings.Settings["RegexMasterIP4"].Value = regexMasterIPBox4.Text;
                            config.AppSettings.Settings["RegexMasterPort4"].Value = regexMasterPortBox4.Text;
                            config.AppSettings.Settings["ServerIP"].Value = ServerIPBox.Text;
                            config.AppSettings.Settings["FTPPort"].Value = PortBox.Text;
                            config.AppSettings.Settings["InductorStation1No"].Value = InductorStation1NoBox.Text;
                            config.AppSettings.Settings["InductorStation2No"].Value = InductorStation2NoBox.Text;
                            config.AppSettings.Settings["UploadMode"].Value = UploadModeList.Text;
                            config.AppSettings.Settings["User"].Value = UserBox.Text;
                            config.AppSettings.Settings["Password"].Value = PasswordBox.Text;
                            config.AppSettings.Settings["ServerIP2"].Value = ServerIPBox2.Text;
                            config.AppSettings.Settings["FTPPort2"].Value = PortBox2.Text;
                            config.AppSettings.Settings["User2"].Value = UserBox2.Text;
                            config.AppSettings.Settings["Password2"].Value = PasswordBox2.Text;
                            config.AppSettings.Settings["StorageRoot1"].Value = ImageRootBox1.Text;
                            config.AppSettings.Settings["StorageRoot2"].Value = ImageRootBox2.Text;
                            config.AppSettings.Settings["StorageRoot3"].Value = ImageRootBox3.Text;
                            config.AppSettings.Settings["StorageRoot4"].Value = ImageRootBox4.Text;
                            config.AppSettings.Settings["FtpFileNameFormat"].Value = FtpFileNameBox.Text;
                            config.AppSettings.Settings["FtpFileNameFormat2"].Value = FtpFileNameBox2.Text;
                            config.AppSettings.Settings["FileNameFormat"].Value = FileNameBox.Text;
                            config.AppSettings.Settings["HttpFileNameFormat"].Value = HttpFileNameBox.Text;
                            config.AppSettings.Settings["CaptionFormat"].Value = CaptionFormatBox.Text;
                            config.AppSettings.Settings["3DPosFormat"].Value = PosCombo.Text;
                            config.AppSettings.Settings["BarocdePosFormat"].Value = BarcodePosCombo.Text;
                            config.AppSettings.Settings["DimAndWeightFormat"].Value = DimWgtCombo.Text;
                            config.AppSettings.Settings["MultiCodeDelimiter"].Value = delimiterBox.Text;
                            config.Save(ConfigurationSaveMode.Modified);
                            ConfigurationManager.RefreshSection("appSettings");//每次开始工作后将目前的相机存图地址保存到配置文件
                        }
                        catch (Exception ex) { }
                    }
                    allowChinese = (captionOptionList.Text == "包含中文字符" || captionOptionList.Text == "AllowChinese") ? true : false;
                    PicStorDays = Convert.ToInt32(PicStorDaysText.Text);
                    MultiCodeDelimiter[0] = delimiterBox.Text;
                    FTPOKMes = FTPOKBox.Text;
                    FTPNOKMes = FTPNOKBox.Text;
                    HTTPOKMes = HTTPOKBox.Text;
                    HTTPNOKMes = HTTPNOKBox.Text;
                    //siteCode = siteCodeBox.Text;
                    regexMachineCode = regexMachineCodeBox.Text;
                    machineCodeFormat = machineCodeBox.Text;
                    HttpCheckUrl = HttpCheckUrlBox.Text;
                    HttpUploadUrl = HttpUploadUrlBox.Text;
                    FtpFileNameFormat = FtpFileNameBox.Text;
                    FtpFileNameFormat2 = FtpFileNameBox2.Text;
                    FileNameFormat = FileNameBox.Text;
                    HttpFileNameFormat = HttpFileNameBox.Text;
                    CaptionFormat = CaptionFormatBox.Text;
                    DimAndWeightFormat = DimWgtCombo.Text;
                    PosFormat = PosCombo.Text;
                    BarcodePosFormat = BarcodePosCombo.Text;
                    InductorStation1No = InductorStation1NoBox.Text;
                    InductorStation2No = InductorStation2NoBox.Text;
                    UploadMode = UploadModeList.Text;
                    User = UserBox.Text;
                    Password = PasswordBox.Text;
                    Credential = new NetworkCredential(User, Password);
                    User2 = UserBox2.Text;
                    Password2 = PasswordBox2.Text;
                    Credential2 = new NetworkCredential(User2, Password2);
                    SCADAIP = SCADAIPBox.Text;
                    SCADAPort = SCADAPortBox.Text;
                    regexMasterIP1 = regexMasterIPBox1.Text;
                    regexMasterPort1 = regexMasterPortBox1.Text;
                    regexMasterIP2 = regexMasterIPBox2.Text;
                    regexMasterPort2 = regexMasterPortBox2.Text;
                    regexMasterIP3 = regexMasterIPBox3.Text;
                    regexMasterPort3 = regexMasterPortBox3.Text;
                    regexMasterIP4 = regexMasterIPBox4.Text;
                    regexMasterPort4 = regexMasterPortBox4.Text;
                    ServerIP = ServerIPBox.Text;
                    FTPPort = PortBox.Text;
                    ServerIP2 = ServerIPBox2.Text;
                    FTPPort2 = PortBox2.Text;
                    InductFtpRoot = InductFtpRootBox.Text;
                    FtpRoot1 = FtpRootBox1.Text;
                    FtpRoot2 = FtpRootBox2.Text;
                    FtpRoot3 = FtpRootBox3.Text;
                    FtpRoot4 = FtpRootBox4.Text;
                    FtpRoots[0] = FtpRoot1;
                    FtpRoots[1] = FtpRoot2;
                    FtpRoots[2] = FtpRoot3;
                    FtpRoots[3] = FtpRoot4;
                    StorageRoot1 = ImageRootBox1.Text;
                    StorageRoot2 = ImageRootBox2.Text;
                    StorageRoot3 = ImageRootBox3.Text;
                    StorageRoot4 = ImageRootBox4.Text;
                    StorageRoots[0] = StorageRoot1;
                    StorageRoots[1] = StorageRoot2;
                    StorageRoots[2] = StorageRoot3;
                    StorageRoots[3] = StorageRoot4;
                    disk = FtpRoot1.Substring(0, 2);
                    //disk = StorageRoot1.Substring(0, 2);
                    barcode_stitch_url1 = disk + "\\SIH\\TopStitch1";
                    barcode_stitch_url2 = disk + "\\SIH\\TopStitch2";
                    barcode_stitch_url3 = disk + "\\SIH\\TopStitch3";
                    barcode_stitch_url4 = disk + "\\SIH\\TopStitch4";
                    barcode_stitch_url5 = disk + "\\SIH\\TopStitch5";
                    inductor_sorter_temp_url = disk + "\\SIH\\InductorSorterCombination";
                    stitch_url1 = disk + "\\SIH\\Bottom\\Stitch1";
                    stitch_url2 = disk + "\\SIH\\Bottom\\Stitch2";
                    stitch_url3 = disk + "\\SIH\\Bottom\\Stitch3";
                    stitch_url4 = disk + "\\SIH\\Bottom\\Stitch4";
                    stitch_url5 = disk + "\\SIH\\Bottom\\Stitch5";
                    ext_caption_url1 = disk + "\\SIH\\ExtCaption\\Station1";
                    ext_caption_url2 = disk + "\\SIH\\ExtCaption\\Station2";
                    ext_caption_url3 = disk + "\\SIH\\ExtCaption\\Station3";
                    ext_caption_url4 = disk + "\\SIH\\ExtCaption\\Station4";
                    caption_url1 = disk + "\\SIH\\Caption\\Temp1";//Caption临时文件夹
                    caption_url2 = disk + "\\SIH\\Caption\\Temp2";
                    caption_url3 = disk + "\\SIH\\Caption\\Temp3";
                    caption_url4 = disk + "\\SIH\\Caption\\Temp4";
                    caption_url5 = disk + "\\SIH\\Caption\\Temp5";
                    ftp_temp_url1 = disk + "\\SIH\\FtpUpload\\Temp1";
                    ftp_temp_url2 = disk + "\\SIH\\FtpUpload\\Temp2";
                    ftp_temp_url3 = disk + "\\SIH\\FtpUpload\\Temp3";
                    ftp_temp_url4 = disk + "\\SIH\\FtpUpload\\Temp4";
                    ftp_temp_url5 = disk + "\\SIH\\FtpUpload\\Temp5";
                    ftp_temp_url6 = disk + "\\SIH\\FtpUpload\\Temp6";
                    //ftp_log_dir = disk + "\\log";
                    http_temp_url1 = disk + "\\SIH\\HttpUpload\\Temp1";
                    http_temp_url2 = disk + "\\SIH\\HttpUpload\\Temp2";
                    http_temp_url3 = disk + "\\SIH\\HttpUpload\\Temp3";
                    FtpSubFolder1 = FTPFolder1Combo.Text;
                    FtpSubFolder2 = FTPFolder2Combo.Text;
                    FtpSubFolder3 = FTPFolder3Combo.Text;
                    FtpSubFolder4 = FTPFolder4Combo.Text;
                    FtpSubFolder5 = FTPFolder5Combo.Text;
                    SubFolder1 = Folder1Combo.Text;
                    SubFolder2 = Folder2Combo.Text;
                    SubFolder3 = Folder3Combo.Text;
                    SubFolder4 = Folder4Combo.Text;
                    SubFolder5 = Folder5Combo.Text;
                    SubFolders[0] = SubFolder1;
                    SubFolders[1] = SubFolder2;
                    SubFolders[2] = SubFolder3;
                    SubFolders[3] = SubFolder4;
                    SubFolders[4] = SubFolder5;
                    ExtCaptionBoxTimeOut1 = Convert.ToInt16(ExtDelayCombo1.Text) * 1000;
                    ExtCaptionBoxTimeOut2 = Convert.ToInt16(ExtDelayCombo2.Text) * 1000;
                    ExtCaptionBoxTimeOut3 = Convert.ToInt16(ExtDelayCombo3.Text) * 1000;
                    ExtCaptionBoxTimeOut4 = Convert.ToInt16(ExtDelayCombo4.Text) * 1000;
                    BottomCamCount1 = Convert.ToInt16(BottomCamNoCombo1.Text);
                    BottomCamCount2 = Convert.ToInt16(BottomCamNoCombo2.Text);
                    BottomCamCount3 = Convert.ToInt16(BottomCamNoCombo3.Text);
                    BottomCamCount4 = Convert.ToInt16(BottomCamNoCombo4.Text);
                    BarcodeImgMode = BarcodeImgModeCombo.Text;
                    InductID1 = InductIDCombo1.Text;
                    InductID2 = InductIDCombo2.Text;
                    InductID3 = InductIDCombo3.Text;
                    InductID4 = InductIDCombo4.Text;
                    SorterTimeStampFormat = SorterTimeStampCombo.Text;
                    SorterTrayIDFormat = TrayIDCombo.Text;
                    SorterBoxIDFormat = BoxIDCombo.Text;
                    SorterResultFormat = ResultCombo.Text;
                    SorterBarcodeFormat = BarcodeCombo.Text;
                    SorterStationNoFormat = SorterStationNoCombo.Text;
                    CamCount1 = Convert.ToInt16(CamNoCombo1.Text);
                    CamCount2 = Convert.ToInt16(CamNoCombo2.Text);
                    CamCount3 = Convert.ToInt16(CamNoCombo3.Text);
                    CamCount4 = Convert.ToInt16(CamNoCombo4.Text);
                    TotalCamCount1 = CamCount1 + 4 * BottomCamCount1;
                    TotalCamCount2 = CamCount2 + 4 * BottomCamCount2;
                    TotalCamCount3 = CamCount3 + 4 * BottomCamCount3;
                    TotalCamCount4 = CamCount4 + 4 * BottomCamCount4;
                    if (sftp1 != null)
                    {
                        //sftp1.Disconnect();
                        sftp1.Dispose();
                    }
                    sftp1 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    if (sftp2 != null)
                    {
                        //sftp2.Disconnect();
                        sftp2.Dispose();
                    }
                    sftp2 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    if (sftp3 != null)
                    {
                        //sftp3.Disconnect();
                        sftp3.Dispose();
                    }
                    sftp3 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    if (sftp4 != null)
                    {
                        //sftp4.Disconnect();
                        sftp4.Dispose();
                    }
                    sftp4 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    if (sftp5 != null)
                    {
                        //sftp5.Disconnect();
                        sftp5.Dispose();
                    }
                    sftp5 = new SFTPHelper(ServerIP, FTPPort, User, Password, KeyPath, PassPhrase);
                    delimiterBox.Enabled = false;
                    calibrateBtn.Enabled = false;
                    captionOptionList.Enabled = false;
                    FTPOKBox.Enabled = false;
                    FTPNOKBox.Enabled = false;
                    HTTPOKBox.Enabled = false;
                    HTTPNOKBox.Enabled = false;
                    HttpRegexUrlBox.Enabled = false;
                    regexMachineCodeBox.Enabled = false;
                    machineCodeBox.Enabled = false;
                    HttpCheckUrlBox.Enabled = false;
                    HttpUploadUrlBox.Enabled = false;
                    FtpFileNameBox.Enabled = false;
                    FtpFileNameBox2.Enabled = false;
                    FileNameBox.Enabled = false;
                    HttpFileNameBox.Enabled = false;
                    DimWgtCombo.Enabled = false;
                    PosCombo.Enabled = false;
                    BarcodePosCombo.Enabled = false;
                    CaptionFormatBox.Enabled = false;
                    SCADAIPBox.Enabled = false;
                    SCADAPortBox.Enabled = false;
                    regexMasterIPBox1.Enabled = false;
                    regexMasterPortBox1.Enabled = false;
                    regexMasterIPBox2.Enabled = false;
                    regexMasterPortBox2.Enabled = false;
                    regexMasterIPBox3.Enabled = false;
                    regexMasterPortBox3.Enabled = false;
                    regexMasterIPBox4.Enabled = false;
                    regexMasterPortBox4.Enabled = false;
                    ServerIPBox.Enabled = false;
                    PortBox.Enabled = false;
                    InductorStation1NoBox.Enabled = false;
                    InductorStation2NoBox.Enabled = false;
                    UploadModeList.Enabled = false;
                    UserBox.Enabled = false;
                    PasswordBox.Enabled = false;
                    ServerIPBox2.Enabled = false;
                    PortBox2.Enabled = false;
                    UserBox2.Enabled = false;
                    PasswordBox2.Enabled = false;
                    Folder1Combo.Enabled = false;
                    Folder2Combo.Enabled = false;
                    Folder3Combo.Enabled = false;
                    Folder4Combo.Enabled = false;
                    Folder5Combo.Enabled = false;
                    FTPFolder1Combo.Enabled = false;
                    FTPFolder2Combo.Enabled = false;
                    FTPFolder3Combo.Enabled = false;
                    FTPFolder4Combo.Enabled = false;
                    FTPFolder5Combo.Enabled = false;
                    ExtDelayCombo1.Enabled = false;
                    ExtDelayCombo2.Enabled = false;
                    ExtDelayCombo3.Enabled = false;
                    ExtDelayCombo4.Enabled = false;
                    BottomCamNoCombo1.Enabled = false;
                    BottomCamNoCombo2.Enabled = false;
                    BottomCamNoCombo3.Enabled = false;
                    BottomCamNoCombo4.Enabled = false;
                    BarcodeImgModeCombo.Enabled = false;
                    InductIDCombo1.Enabled = false;
                    InductIDCombo2.Enabled = false;
                    InductIDCombo3.Enabled = false;
                    InductIDCombo4.Enabled = false;
                    SorterTimeStampCombo.Enabled = false;
                    BoxIDCombo.Enabled = false;
                    TrayIDCombo.Enabled = false;
                    ResultCombo.Enabled = false;
                    BarcodeCombo.Enabled = false;
                    SorterStationNoCombo.Enabled = false;
                    CamNoCombo1.Enabled = false;
                    CamNoCombo2.Enabled = false;
                    CamNoCombo3.Enabled = false;
                    CamNoCombo4.Enabled = false;
                    InductFtpRootButton.Enabled = false;
                    SelectFtpRootButton1.Enabled = false;
                    SelectFtpRootButton2.Enabled = false;
                    SelectFtpRootButton3.Enabled = false;
                    SelectFtpRootButton4.Enabled = false;
                    SelectImageRootButton1.Enabled = false;
                    SelectImageRootButton2.Enabled = false;
                    SelectImageRootButton3.Enabled = false;
                    SelectImageRootButton4.Enabled = false;
                    PackageBorderCheck.Enabled = false;
                    CaptionEnableCheck.Enabled = false;
                    EncryptionEnableCheck.Enabled = false;
                    GridLineEnableCheck.Enabled = false;
                    NRAnalysisEnableCheck.Enabled = false;
                    ExtCaptionEnableCheck.Enabled = false;
                    Sta1OnBox.Enabled = false;
                    Sta2OnBox.Enabled = false;
                    Sta3OnBox.Enabled = false;
                    Sta4OnBox.Enabled = false;
                    PicStorDaysText.Enabled = false;
                    gs.PixelSizeText.Enabled = false;
                    gs.FocusLengthText.Enabled = false;
                    gs.ReadAngleText.Enabled = false;
                    gs.MountingHeightText.Enabled = false;
                    gs.MaxParcelHeightText.Enabled = false;
                    gs.BtnSave.Enabled = false;
                    MonitorTimer.Enabled = true;
                    if (Regex.IsMatch(FtpRoot1, UrlPattern) && Regex.IsMatch(StorageRoot1, UrlPattern) && Sta1On)
                    {
                        Timer1Enabled = true;
                        Timer1.Start();
                    }
                    else if (Sta1On)
                    {
                        MessageBox.Show("工位一路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Regex.IsMatch(FtpRoot2, UrlPattern) && Regex.IsMatch(StorageRoot2, UrlPattern) && Sta2On)
                    {
                        Timer2Enabled = true;
                        Timer2.Start();
                    }
                    else if (Sta2On)
                    {
                        MessageBox.Show("工位二路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Regex.IsMatch(FtpRoot3, UrlPattern) && Regex.IsMatch(StorageRoot3, UrlPattern) && Sta3On)
                    {
                        Timer3Enabled = true;
                        Timer3.Start();
                    }
                    else if (Sta3On)
                    {
                        MessageBox.Show("工位三路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (Regex.IsMatch(FtpRoot4, UrlPattern) && Regex.IsMatch(StorageRoot4, UrlPattern) && Sta4On)
                    {
                        Timer4Enabled = true;
                        Timer4.Start();
                    }
                    else if (Sta4On)
                    {
                        MessageBox.Show("工位四路径格式有误，请重新输入", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    Timer5.Start();
                    Timer6.Start();
                    Timer7.Start();
                    Timer8.Start();
                    Timer9.Start();
                    Timer10.Start();
                    Timer11.Start();
                    Timer12.Start();
                    Timer5Enabled = true;
                    Timer6Enabled = true;
                    Timer7Enabled = true;
                    Timer8Enabled = true;
                    Timer9Enabled = true;
                    Timer10Enabled = true;
                    Timer11Enabled = true;
                    Timer12Enabled = true;
                    if (Sta1On)
                    {
                        Timer13.Start();
                        Timer14.Start();
                        Timer13Enabled = true;
                        Timer14Enabled = true;
                    }
                    if (Sta2On)
                    {
                        Timer15.Start();
                        Timer16.Start();
                        Timer15Enabled = true;
                        Timer16Enabled = true;
                    }
                    if (Sta3On)
                    {
                        Timer17.Start();
                        Timer18.Start();
                        Timer17Enabled = true;
                        Timer18Enabled = true;
                    }
                    if (Sta4On)
                    {
                        Timer19.Start();
                        Timer20.Start();
                        Timer19Enabled = true;
                        Timer20Enabled = true;
                    }
                    Timer21.Start();
                    Timer22.Start();
                    Timer23.Start();
                    Timer24.Start();
                    Timer25.Start();
                    Timer26.Start();
                    Timer27.Start();
                    Timer28.Start();
                    Timer29.Start();
                    Timer30.Start();
                    Timer31.Start();
                    Timer32.Start();
                    Timer33.Start();
                    Timer34.Start();
                    Timer35.Start();
                    Timer36.Start();
                    Timer37.Start();
                    Timer38.Start();
                    Timer39.Start();
                    Timer40.Start();
                    Timer41.Start();
                    Timer42.Start();
                    Timer43.Start();
                    Timer44.Start();
                    Timer45.Start();
                    Timer46.Start();
                    Timer47.Start();
                    Timer48.Start();
                    Timer49.Start();
                    Timer21Enabled = true;
                    Timer22Enabled = true;
                    Timer23Enabled = true;
                    Timer24Enabled = true;
                    Timer25Enabled = true;
                    Timer26Enabled = true;
                    Timer27Enabled = true;
                    Timer28Enabled = true;
                    Timer29Enabled = true;
                    Timer30Enabled = true;
                    Timer31Enabled = true;
                    Timer32Enabled = true;
                    Timer33Enabled = true;
                    Timer34Enabled = true;
                    Timer35Enabled = true;
                    Timer36Enabled = true;
                    Timer37Enabled = true;
                    Timer38Enabled = true;
                    Timer39Enabled = true;
                    Timer40Enabled = true;
                    Timer41Enabled = true;
                    Timer42Enabled = true;
                    Timer43Enabled = true;
                    Timer44Enabled = true;
                    Timer45Enabled = true;
                    Timer46Enabled = true;
                    Timer47Enabled = true;
                    Timer48Enabled = true;
                    Timer49Enabled = true;
                    Start.Enabled = false;
                    Stop.Enabled = true;
                    running = true;
                }
                else
                {
                    string ErrMsg = "";
                    if (!flag1)
                    {
                        ErrMsg += "——输入FTP服务器或SCADA服务器IP端口有误\r\n";
                    }
                    if (!flag2)
                    {
                        ErrMsg += "——文件名格式有误\r\n";
                    }
                    if (!flag3)
                    {
                        ErrMsg += "——工站一FTP根目录有误\r\n";
                    }
                    if (!flag4)
                    {
                        ErrMsg += "——工站二FTP根目录有误\r\n";
                    }
                    if (!flag5)
                    {
                        ErrMsg += "——工站三FTP根目录有误\r\n";
                    }
                    if (!flag6)
                    {
                        ErrMsg += "——工站四FTP根目录有误\r\n";
                    }
                    if (!flag7)
                    {
                        ErrMsg += "——工站一图片存放根目录有误\r\n";
                    }
                    if (!flag8)
                    {
                        ErrMsg += "——工站二图片存放根目录有误\r\n";
                    }
                    if (!flag9)
                    {
                        ErrMsg += "——工站三图片存放根目录有误\r\n";
                    }
                    if (!flag10)
                    {
                        ErrMsg += "——工站四图片存放根目录有误\r\n";
                    }
                    if (!flag11)
                    {
                        ErrMsg += "——不同工位使用相同的FTP根目录\r\n";
                    }
                    if (!flag12)
                    {
                        ErrMsg += "——不同工位使用相同的存图位置根目录\r\n";
                    }
                    if (!flag13)
                    {
                        ErrMsg += "——输入HTTP服务器URL有误\r\n";
                    }
                    if (!flag14)
                    {
                        ErrMsg += "——供包台图片存放根目录有误\r\n";
                    }
                    MessageBox.Show(ErrMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { }
        }
        private bool CheckDuplication(string[] Roots)
        {
            Hashtable ht = new Hashtable();
            for(int i=0;i<Roots.Length;i++)
            {
                if (ht.Contains(Roots[i]))
                {
                    if (Roots[i] != "")
                    {
                        return true;
                    }
                }
                else
                {
                    ht.Add(Roots[i], Roots[i]);
                }
            }
            return false;
        }
        private bool IsValidFileName(string filename)
        {
            char[] chh = Path.GetInvalidFileNameChars();
            if (Path.GetInvalidFileNameChars().Any(c => filename.Contains(c)))//Any返回是否集合中有任意一个元素符合某个条件
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void TimeEvent1(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer1Running = true;
                if (Interlocked.Exchange(ref Timer1TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer1Finished = false;
                    Timer1Running = true;
                    if (Sta1On)
                    {
                        MovePic(FtpRoot1, StorageRoot1, MasterArraySta1, MasterArraySta1Lock, TotalCamCount1, systemLog_Station1, LogContentArray1,InductID1);
                    }
                    Timer1Finished = true;
                    Interlocked.Exchange(ref Timer1TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer1TaskRunning, 0);
                Timer1Finished = true;
            }
        }
        public void TimeEvent2(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer2Running = true;
                if (Interlocked.Exchange(ref Timer2TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer2Finished = false;
                    Timer2Running = true;
                    if (Sta2On)
                    {
                        MovePic(FtpRoot2, StorageRoot2, MasterArraySta2, MasterArraySta2Lock, TotalCamCount2, systemLog_Station2, LogContentArray2, InductID2);
                    }
                    Timer2Finished = true;
                    Interlocked.Exchange(ref Timer2TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer2TaskRunning, 0);
                Timer2Finished = true;
            }
        }
        public void TimeEvent3(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer3Running = true;
                if (Interlocked.Exchange(ref Timer3TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer3Finished = false;
                    Timer3Running = true;
                    if (Sta3On)
                    {
                        MovePic(FtpRoot3, StorageRoot3, MasterArraySta3, MasterArraySta3Lock, TotalCamCount3, systemLog_Station3, LogContentArray3, InductID3);
                    }
                    Timer3Finished = true;
                    Interlocked.Exchange(ref Timer3TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer3TaskRunning, 0);
                Timer3Finished = true;
            }
        }
        public void TimeEvent4(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer4Running = true;
                if (Interlocked.Exchange(ref Timer4TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer4Finished = false;
                    Timer4Running = true;
                    if (Sta4On)
                    {
                        MovePic(FtpRoot4, StorageRoot4, MasterArraySta4, MasterArraySta4Lock, TotalCamCount4, systemLog_Station4, LogContentArray4, InductID4);
                    }
                    Timer4Finished = true;
                    Interlocked.Exchange(ref Timer4TaskRunning, 0);
                }
            }
            catch (Exception ex)
            { 
                Interlocked.Exchange(ref Timer4TaskRunning, 0);
                Timer4Finished = true;
            }
        }
        public void TimeEvent5(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer5Running = true;
                if (Interlocked.Exchange(ref Timer5TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer5Finished = false;
                    Timer5Running = true;
                    autorun();
                    HandleMasterArray(MasterArraySta1,LogContentArray1, MasterArraySta1Lock, TotalCamCount1, systemLog_Station1);
                    HandleMasterArray(MasterArraySta2, LogContentArray2, MasterArraySta2Lock, TotalCamCount2, systemLog_Station2);
                    HandleMasterArray(MasterArraySta3, LogContentArray3, MasterArraySta3Lock, TotalCamCount3, systemLog_Station3);
                    HandleMasterArray(MasterArraySta4, LogContentArray4, MasterArraySta4Lock, TotalCamCount4, systemLog_Station4);
                    Timer5Finished = true;
                    Interlocked.Exchange(ref Timer5TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            {
                Interlocked.Exchange(ref Timer5TaskRunning, 0);
                Timer5Finished = true;
            }
        }
        public void TimeEvent6(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer6Running = true;
                if (Interlocked.Exchange(ref Timer6TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer6Finished = false;
                    Timer6Running = true;
                    if ((DateTime.Now - LastDeleteFileTimeStamp).TotalHours > 24)
                    {
                        deletefiles(inductor_sorter_temp_url, 2);
                        deletefiles(ftp_log_dir, 2);
                        deletefiles(systemLog_dir, PicStorDays);
                        deletefiles(ftp_temp_url1, 2);
                        deletefiles(ftp_temp_url2, 2);
                        deletefiles(ftp_temp_url3, 2);
                        deletefiles(ftp_temp_url4, 2);
                        deletefiles(ftp_temp_url5, 2);
                        deletefiles(ftp_temp_url6, 2);
                        deletefiles(barcode_stitch_url1, 2);
                        deletefiles(barcode_stitch_url2, 2);
                        deletefiles(barcode_stitch_url3, 2);
                        deletefiles(barcode_stitch_url4, 2);
                        deletefiles(barcode_stitch_url5, 2);
                        deletefiles(stitch_url1, 2);
                        deletefiles(stitch_url2, 2);
                        deletefiles(stitch_url3, 2);
                        deletefiles(stitch_url4, 2);
                        deletefiles(stitch_url5, 2);
                        deletefiles(ext_caption_url1, 2);
                        deletefiles(ext_caption_url2, 2);
                        deletefiles(ext_caption_url3, 2);
                        deletefiles(ext_caption_url4, 2);
                        deletefiles(caption_url1, 2);
                        deletefiles(caption_url2, 2);
                        deletefiles(caption_url3, 2);
                        deletefiles(caption_url4, 2);
                        deletefiles(caption_url5, 2);
                        //deletefiles(http_temp_url1, 3);
                        //deletefiles(http_temp_url2, 3);
                        //deletefiles(ftp_temp_url3, 3);
                        deleteImageFolderFiles(StorageRoot1, PicStorDays);
                        deleteImageFolderFiles(StorageRoot2, PicStorDays);
                        deleteImageFolderFiles(StorageRoot3, PicStorDays);
                        deleteImageFolderFiles(StorageRoot4, PicStorDays);
                        LastDeleteFileTimeStamp = DateTime.Now;
                        lock (OpenConfigLock)
                        {
                            try
                            {
                                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                                string test = DateTime.MinValue.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                config.AppSettings.Settings["LastDeleteFileTimeStamp"].Value = LastDeleteFileTimeStamp.ToString("yyyy/MM/dd HH:mm:ss.fff", System.Globalization.DateTimeFormatInfo.InvariantInfo).Replace("-","/");
                                config.Save(ConfigurationSaveMode.Modified);
                                ConfigurationManager.RefreshSection("appSettings");//写入上次处理的图片的时间戳
                            }
                            catch (Exception ex)
                            { }
                        }
                    }
                    Timer6Finished = true;
                    Interlocked.Exchange(ref Timer6TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer6TaskRunning, 0);
                Timer6Finished = true;
            }
        }
        public void TimeEvent7(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer7Running = true;
                if (Interlocked.Exchange(ref Timer7TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer7Finished = false;
                    Timer7Running = true;
                    if (upload)
                    {
                        if (UploadMode == "ftp")
                        {
                            UploadFile(ftp_temp_url2, ServerIP, FTPPort, Credential, ref LogFailureNum1, LogContentArrayFTP2);
                        }
                        else if(UploadMode == "sftp")
                        {
                            sftpUploadFile(ftp_temp_url2, LogContentArrayFTP2);
                        }
                    }
                    Timer7Finished = true;
                    Interlocked.Exchange(ref Timer7TaskRunning, 0);
                }
            }
            catch (Exception ex)
            { 
                Interlocked.Exchange(ref Timer7TaskRunning, 0);
                Timer7Finished = true;
            }
        }
        public void TimeEvent8(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer8Running = true;
                if (Interlocked.Exchange(ref Timer8TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer8Finished = false;
                    Timer8Running = true;
                    if (upload)
                    {
                        if (UploadMode == "ftp")
                        {
                            UploadFile(ftp_temp_url1, ServerIP, FTPPort, Credential, ref LogFailureNum1, LogContentArrayFTP1);
                        }
                        else if (UploadMode == "sftp")
                        {
                            sftpUploadFile(ftp_temp_url1, LogContentArrayFTP1);
                        }
                    }
                    Timer8Finished = true;
                    Interlocked.Exchange(ref Timer8TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer8TaskRunning, 0);
                Timer8Finished = true;
            }
        }
        public void TimeEvent9(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer9Running = true;
                if (Interlocked.Exchange(ref Timer9TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer9Finished = false;
                    Timer9Running = true;
                    SystemLogv2(LogContentArray1);
                    SystemLogv2(LogContentArrayFTP1);
                    Timer9Finished = true;
                    Interlocked.Exchange(ref Timer9TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer9TaskRunning, 0);
                Timer9Finished = true;
            }
        }
        public void TimeEvent10(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer10Running = true;
                if (Interlocked.Exchange(ref Timer10TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer10Finished = false;
                    Timer10Running = true;
                    SystemLogv2(LogContentArray2);
                    SystemLogv2(LogContentArrayFTP2);
                    Timer10Finished = true;
                    Interlocked.Exchange(ref Timer10TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer10TaskRunning, 0);
                Timer10Finished = true;
            }
        }
        public void TimeEvent11(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer11Running = true;
                if (Interlocked.Exchange(ref Timer11TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer11Finished = false;
                    Timer11Running = true;
                    SystemLogv2(LogContentArray3);
                    SystemLogv2(LogContentArrayFTP3);
                    Timer11Finished = true;
                    Interlocked.Exchange(ref Timer11TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer11TaskRunning, 0);
                Timer11Finished = true;
            }
        }
        public void TimeEvent12(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer12Running = true;
                if (Interlocked.Exchange(ref Timer12TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer12Finished = false;
                    Timer12Running = true;
                    SystemLogv2(LogContentArray4);
                    SystemLogv2(LogContentArrayFTP4);
                    Timer12Finished = true;
                    Interlocked.Exchange(ref Timer12TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer12TaskRunning, 0);
                Timer12Finished = true;
            }
        }
        public void TimeEvent13(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer13Running = true;
                if (Interlocked.Exchange(ref Timer13TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer13Finished = false;
                    Timer13Running = true;
                    if (httpUpload)
                    {
                        HttpRegexCheck(LogContentArray1);
                        HandleHttpArray(HTTPArraySta1_1, LogContentArray1, HTTPArraySta1_1Lock, TotalCamCount1, systemLog_Station1);
                    }
                    Timer13Finished = true;
                    Interlocked.Exchange(ref Timer13TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer13TaskRunning, 0);
                Timer13Finished = true;
            }
        }
        public void TimeEvent14(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer14Running = true;
                if (Interlocked.Exchange(ref Timer14TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer14Finished = false;
                    Timer14Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta1_2, LogContentArray1, HTTPArraySta1_2Lock, TotalCamCount1, systemLog_Station1);
                    }
                    Timer14Finished = true;
                    Interlocked.Exchange(ref Timer14TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer14TaskRunning, 0);
                Timer14Finished = true;
            }
        }
        public void TimeEvent15(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer15Running = true;
                if (Interlocked.Exchange(ref Timer15TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer15Finished = false;
                    Timer15Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta2_1, LogContentArray2, HTTPArraySta2_1Lock, TotalCamCount2, systemLog_Station2);
                    }
                    Timer15Finished = true;
                    Interlocked.Exchange(ref Timer15TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer15TaskRunning, 0);
                Timer15Finished = true;
            }
        }
        public void TimeEvent16(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer16Running = true;
                if (Interlocked.Exchange(ref Timer16TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer16Finished = false;
                    Timer16Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta2_2, LogContentArray2, HTTPArraySta2_2Lock, TotalCamCount2, systemLog_Station2);
                    }
                    Timer16Finished = true;
                    Interlocked.Exchange(ref Timer16TaskRunning, 0);
                }
            }
            catch (Exception ex)
            { 
                Interlocked.Exchange(ref Timer16TaskRunning, 0);
                Timer16Finished = true;
            }
        }
        public void TimeEvent17(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer17Running = true;
                if (Interlocked.Exchange(ref Timer17TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer17Finished = false;
                    Timer17Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta3_1, LogContentArray3, HTTPArraySta3_1Lock, TotalCamCount3, systemLog_Station3);
                    }
                    Timer17Finished = true;
                    Interlocked.Exchange(ref Timer17TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer17TaskRunning, 0);
                Timer17Finished = true;
            }
            
        }
        public void TimeEvent18(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer18Running = true;
                if (Interlocked.Exchange(ref Timer18TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer18Finished = false;
                    Timer18Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta3_2, LogContentArray3, HTTPArraySta3_2Lock, TotalCamCount3, systemLog_Station3);
                    }
                    Timer18Finished = true;
                    Interlocked.Exchange(ref Timer18TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer18TaskRunning, 0);
                Timer18Finished = true;
            }
        }
        public void TimeEvent19(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer19Running = true;
                if (Interlocked.Exchange(ref Timer19TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer19Finished = false;
                    Timer19Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta4_1, LogContentArray4, HTTPArraySta4_1Lock, TotalCamCount4, systemLog_Station4);
                    }
                    Timer19Finished = true;
                    Interlocked.Exchange(ref Timer19TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer19TaskRunning, 0);
                Timer19Finished = true;
            }
        }
        public void TimeEvent20(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer20Running = true;
                if (Interlocked.Exchange(ref Timer20TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer20Finished = false;
                    Timer20Running = true;
                    if (httpUpload)
                    {
                        HandleHttpArray(HTTPArraySta4_2, LogContentArray4, HTTPArraySta4_2Lock, TotalCamCount4, systemLog_Station4);
                    }
                    Timer20Finished = true;
                    Interlocked.Exchange(ref Timer20TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            {
                Interlocked.Exchange(ref Timer20TaskRunning, 0);
                Timer20Finished = true;
            }
        }
        public void TimeEvent21(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer21Running = true;
                if (Interlocked.Exchange(ref Timer21TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer21Finished = false;
                    Timer21Running = true;
                    if (uploadStatus)
                    {
                        UploadScada();
                    }
                    if (httpUpload)
                    {
                        if (regexSta1Enable)
                        {
                            UploadRegex(regexMasterIP1, regexMasterPort1);
                        }
                        if (regexSta2Enable)
                        {
                            UploadRegex(regexMasterIP2, regexMasterPort2);
                        }
                        if (regexSta3Enable)
                        {
                            UploadRegex(regexMasterIP3, regexMasterPort3);
                        }
                        if (regexSta4Enable)
                        {
                            UploadRegex(regexMasterIP4, regexMasterPort4);
                        }
                    }
                    Timer21Finished = true;
                    Interlocked.Exchange(ref Timer21TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer21TaskRunning, 0);
                Timer21Finished = true;
            }
        }
        public void TimeEvent22(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer22Running = true;
                if (Interlocked.Exchange(ref Timer22TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer22Finished = false;
                    Timer22Running = true;
                    AddCaption(caption_url1);
                    Timer22Finished = true;
                    Interlocked.Exchange(ref Timer22TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer22TaskRunning, 0);
                Timer22Finished = true;
            }
        }
        public void TimeEvent23(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer23Running = true;
                if (Interlocked.Exchange(ref Timer23TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer23Finished = false;
                    Timer23Running = true;
                    AddCaption(caption_url2);
                    Timer23Finished = true;
                    Interlocked.Exchange(ref Timer23TaskRunning, 0);
                }
            }
            catch (Exception ex)
            { 
                Interlocked.Exchange(ref Timer23TaskRunning, 0);
                Timer23Finished = true;
            }
        }
        public void TimeEvent24(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer24Running = true;
                if (Interlocked.Exchange(ref Timer24TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer24Finished = false;
                    Timer24Running = true;
                    AddCaption(caption_url3);
                    Timer24Finished = true;
                    Interlocked.Exchange(ref Timer24TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer24TaskRunning, 0);
                Timer24Finished = true;
            }
        }
        public void TimeEvent25(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer25Running = true;
                if (Interlocked.Exchange(ref Timer25TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer25Finished = false;
                    Timer25Running = true;
                    AddCaption(caption_url4);
                    Timer25Finished = true;
                    Interlocked.Exchange(ref Timer25TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer25TaskRunning, 0);
                Timer25Finished = true;
            }
        }
        public void TimeEvent26(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer26Running = true;
                if (Interlocked.Exchange(ref Timer26TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer26Finished = false;
                    Timer26Running = true;
                    AddCaption(caption_url5);
                    Timer26Finished = true;
                    Interlocked.Exchange(ref Timer26TaskRunning, 0);
                }
            }
            catch (Exception ex) 
            { 
                Interlocked.Exchange(ref Timer26TaskRunning, 0);
                Timer26Finished = true;
            }
        }
        public void TimeEvent27(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer27Running = true;
                if (Interlocked.Exchange(ref Timer27TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer27Finished = false;
                    Timer27Running = true;
                    HandleBottomStitchArray(BottomStitchArray1);
                    Timer27Finished = true;
                    Interlocked.Exchange(ref Timer27TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer27TaskRunning, 0);
                Timer27Finished = true;
            }
        }
        public void TimeEvent28(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer28Running = true;
                if (Interlocked.Exchange(ref Timer28TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer28Finished = false;
                    Timer28Running = true;
                    HandleBottomStitchArray(BottomStitchArray2);
                    Timer28Finished = true;
                    Interlocked.Exchange(ref Timer28TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer28TaskRunning, 0);
                Timer28Finished = true;
            }
        }
        public void TimeEvent29(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer29Running = true;
                if (Interlocked.Exchange(ref Timer29TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer29Finished = false;
                    Timer29Running = true;
                    HandleBottomStitchArray(BottomStitchArray3);
                    Timer29Finished = true;
                    Interlocked.Exchange(ref Timer29TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer29TaskRunning, 0);
                Timer29Finished = true;
            }
        }
        public void TimeEvent30(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer30Running = true;
                if (Interlocked.Exchange(ref Timer30TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer30Finished = false;
                    Timer30Running = true;
                    HandleBottomStitchArray(BottomStitchArray4);
                    Timer30Finished = true;
                    Interlocked.Exchange(ref Timer30TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer30TaskRunning, 0);
                Timer30Finished = true;
            }
        }
        public void TimeEvent31(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer31Running = true;
                if (Interlocked.Exchange(ref Timer31TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer31Finished = false;
                    Timer31Running = true;
                    HandleBottomStitchArray(BottomStitchArray5);
                    Timer31Finished = true;
                    Interlocked.Exchange(ref Timer31TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer31TaskRunning, 0);
                Timer31Finished = true;
            }
        }
        public void TimeEvent32(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer32Running = true;
                if (Interlocked.Exchange(ref Timer32TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer32Finished = false;
                    Timer32Running = true;
                    SystemLogv2(LogContentArrayInductor);
                    SystemLogv2(LogContentArrayFTP5);
                    Timer32Finished = true;
                    Interlocked.Exchange(ref Timer32TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer32TaskRunning, 0);
                Timer32Finished = true;
            }
        }
        public void TimeEvent33(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer33Running = true;
                if (Interlocked.Exchange(ref Timer33TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer33Finished = false;
                    Timer33Running = true;
                    MoveInductorPic(InductFtpRoot,LogContentArrayInductor);
                    Timer33Finished = true;
                    Interlocked.Exchange(ref Timer33TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer33TaskRunning, 0);
                Timer33Finished = true;
            }
        }
        public void TimeEvent34(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer34Running = true;
                if (Interlocked.Exchange(ref Timer34TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer34Finished = false;
                    Timer34Running = true;
                    HandleBoxList();
                    //HandleTrayArray();
                    /*
                    if (InductID1 == "一区" || InductID1 == "二区" || InductID2 == "一区" || InductID2 == "二区" || InductID3 == "一区" || InductID3 == "二区" || InductID4 == "一区" || InductID4 == "二区")
                    {
                        HandleTrayArray();
                    }
                    */
                    Timer34Finished = true;
                    Interlocked.Exchange(ref Timer34TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer34TaskRunning, 0);
                Timer34Finished = true;
            }
        }
        public void TimeEvent35(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer35Running = true;
                if (Interlocked.Exchange(ref Timer35TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer35Finished = false;
                    Timer35Running = true;
                    SystemLogv2(LogContentArrayComm);
                    SystemLogv2(LogContentArrayFTP6);
                    Timer35Finished = true;
                    Interlocked.Exchange(ref Timer35TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer35TaskRunning, 0);
                Timer35Finished = true;
            }
        }
        public void TimeEvent36(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer36Running = true;
                if (Interlocked.Exchange(ref Timer36TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer36Finished = false;
                    Timer36Running = true;
                    if (upload2)
                    {
                        UploadFile(ftp_temp_url6, ServerIP2, FTPPort2, Credential2, ref LogFailureNum2, LogContentArrayFTP6);
                    }
                    Timer36Finished = true;
                    Interlocked.Exchange(ref Timer36TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer36TaskRunning, 0);
                Timer36Finished = true;
            }
        }
        public void TimeEvent37(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer37Running = true;
                if (Interlocked.Exchange(ref Timer37TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer37Finished = false;
                    Timer37Running = true;
                    HandleBarcodeStitchArray(BarcodeStitchArray1);
                    Timer37Finished = true;
                    Interlocked.Exchange(ref Timer37TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer37TaskRunning, 0);
                Timer37Finished = true;
            }
        }
        public void TimeEvent38(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer38Running = true;
                if (Interlocked.Exchange(ref Timer38TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer38Finished = false;
                    Timer38Running = true;
                    HandleBarcodeStitchArray(BarcodeStitchArray2);
                    Timer38Finished = true;
                    Interlocked.Exchange(ref Timer38TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer38TaskRunning, 0);
                Timer38Finished = true;
            }
        }
        public void TimeEvent39(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer39Running = true;
                if (Interlocked.Exchange(ref Timer39TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer39Finished = false;
                    Timer39Running = true;
                    HandleBarcodeStitchArray(BarcodeStitchArray3);
                    Timer39Finished = true;
                    Interlocked.Exchange(ref Timer39TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer39TaskRunning, 0);
                Timer39Finished = true;
            }
        }
        public void TimeEvent40(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer40Running = true;
                if (Interlocked.Exchange(ref Timer40TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer40Finished = false;
                    Timer40Running = true;
                    HandleBarcodeStitchArray(BarcodeStitchArray4);
                    Timer40Finished = true;
                    Interlocked.Exchange(ref Timer40TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer40TaskRunning, 0);
                Timer40Finished = true;
            }
        }
        public void TimeEvent41(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer41Running = true;
                if (Interlocked.Exchange(ref Timer41TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer41Finished = false;
                    Timer41Running = true;
                    HandleBarcodeStitchArray(BarcodeStitchArray5);
                    Timer41Finished = true;
                    Interlocked.Exchange(ref Timer41TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer41TaskRunning, 0);
                Timer41Finished = true;
            }
        }
        public void TimeEvent42(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer42Running = true;
                if (Interlocked.Exchange(ref Timer42TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer42Finished = false;
                    Timer42Running = true;
                    if (upload)
                    {
                        if (UploadMode == "ftp")
                        {
                            UploadFile(ftp_temp_url3, ServerIP, FTPPort, Credential, ref LogFailureNum1, LogContentArrayFTP3);
                        }
                        else if (UploadMode == "sftp")
                        {
                            sftpUploadFile(ftp_temp_url3, LogContentArrayFTP3);
                        }
                    }
                    Timer42Finished = true;
                    Interlocked.Exchange(ref Timer42TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer42TaskRunning, 0);
                Timer42Finished = true;
            }
        }
        public void TimeEvent43(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer43Running = true;
                if (Interlocked.Exchange(ref Timer43TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer43Finished = false;
                    Timer43Running = true;
                    if (upload)
                    {
                        if (UploadMode == "ftp")
                        {
                            UploadFile(ftp_temp_url4, ServerIP, FTPPort, Credential, ref LogFailureNum1, LogContentArrayFTP4);
                        }
                        else if (UploadMode == "sftp")
                        {
                            sftpUploadFile(ftp_temp_url4, LogContentArrayFTP4);
                        }
                    }
                    Timer43Finished = true;
                    Interlocked.Exchange(ref Timer43TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer43TaskRunning, 0);
                Timer43Finished = true;
            }
        }
        public void TimeEvent44(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer44Running = true;
                if (Interlocked.Exchange(ref Timer44TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer44Finished = false;
                    Timer44Running = true;
                    if (upload)
                    {
                        if (UploadMode == "ftp")
                        {
                            UploadFile(ftp_temp_url5, ServerIP, FTPPort, Credential, ref LogFailureNum1, LogContentArrayFTP5);
                        }
                        else if (UploadMode == "sftp")
                        {
                            sftpUploadFile(ftp_temp_url5, LogContentArrayFTP5);
                        }
                    }
                    Timer44Finished = true;
                    Interlocked.Exchange(ref Timer44TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer44TaskRunning, 0);
                Timer44Finished = true;
            }
        }
        public void TimeEvent45(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer45Running = true;
                if (Interlocked.Exchange(ref Timer45TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer45Finished = false;
                    Timer45Running = true;
                    HandleExtCaptionBoxList(ExtCaptionBoxList1, LogContentArray1, ExtCaptionBoxListLock1, ExtCaptionBoxTimeOut1);
                    Timer45Finished = true;
                    Interlocked.Exchange(ref Timer45TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer45TaskRunning, 0);
                Timer45Finished = true;
            }
        }
        public void TimeEvent46(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer46Running = true;
                if (Interlocked.Exchange(ref Timer46TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer46Finished = false;
                    Timer46Running = true;
                    HandleExtCaptionBoxList(ExtCaptionBoxList2, LogContentArray2, ExtCaptionBoxListLock2, ExtCaptionBoxTimeOut2);
                    Timer46Finished = true;
                    Interlocked.Exchange(ref Timer46TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer46TaskRunning, 0);
                Timer46Finished = true;
            }
        }
        public void TimeEvent47(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer47Running = true;
                if (Interlocked.Exchange(ref Timer47TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer47Finished = false;
                    Timer47Running = true;
                    HandleExtCaptionBoxList(ExtCaptionBoxList3, LogContentArray3, ExtCaptionBoxListLock3, ExtCaptionBoxTimeOut3);
                    Timer47Finished = true;
                    Interlocked.Exchange(ref Timer47TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer47TaskRunning, 0);
                Timer47Finished = true;
            }
        }
        public void TimeEvent48(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer48Running = true;
                if (Interlocked.Exchange(ref Timer48TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer48Finished = false;
                    Timer48Running = true;
                    HandleExtCaptionBoxList(ExtCaptionBoxList4, LogContentArray4, ExtCaptionBoxListLock4, ExtCaptionBoxTimeOut4);
                    Timer48Finished = true;
                    Interlocked.Exchange(ref Timer48TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer48TaskRunning, 0);
                Timer48Finished = true;
            }
        }
        public void TimeEvent49(object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                Timer49Running = true;
                if (Interlocked.Exchange(ref Timer49TaskRunning, 1) == 0)//若定时器任务尚未完成，下一个定时器任务为空
                {
                    Timer49Finished = false;
                    Timer49Running = true;
                    HandleExternalCaptionInfoList();
                    Timer49Finished = true;
                    Interlocked.Exchange(ref Timer49TaskRunning, 0);
                }
            }
            catch (Exception ex)
            {
                Interlocked.Exchange(ref Timer49TaskRunning, 0);
                Timer49Finished = true;
            }
        }
        public delegate void SetLabelDelegate();
        public delegate void SetLabelDelegate2();
        public delegate void HttpSetLabelDelegate();
        public delegate void MoveFile();
        public delegate void DoNothing();
        public void DoNothingFunction()
        { }
        public void setLabelGray2()
        {
            ConnectStatusLabel2.BackColor = Color.Gray;
        }
        public void setLabelWhite2()
        {
            ConnectStatusLabel2.BackColor = Color.White;
        }
        public void setLabelGreen2()
        {
            ConnectStatusLabel2.BackColor = Color.Green;
        }
        public void setLabelGray()
        {
            ConnectStatusLabel.BackColor = Color.Gray;
        }
        public void setLabelWhite()
        {
            ConnectStatusLabel.BackColor = Color.White;
        }
        public void setLabelGreen()
        {
            ConnectStatusLabel.BackColor = Color.Green;
        }
        public void HttpSetLabelGray()
        {
            HttpConnectStatusLabel.BackColor = Color.Gray;
        }
        public void HttpSetLabelWhite()
        {
            HttpConnectStatusLabel.BackColor = Color.White;
        }
        public void HttpSetLabelGreen()
        {
            HttpConnectStatusLabel.BackColor = Color.Green;
        }
        public static void AddLog(string logcontent, ArrayList LogContentArray)
        {
            if (logEnable)
            {
                object LogContentArrayLock = LogContentArrayLock1;
                if (LogContentArray == LogContentArray1)
                {
                    LogContentArrayLock = LogContentArrayLock1;
                }
                else if (LogContentArray == LogContentArray2)
                {
                    LogContentArrayLock = LogContentArrayLock2;
                }
                else if (LogContentArray == LogContentArray3)
                {
                    LogContentArrayLock = LogContentArrayLock3;
                }
                else if (LogContentArray == LogContentArray4)
                {
                    LogContentArrayLock = LogContentArrayLock4;
                }
                else if (LogContentArray == LogContentArrayInductor)
                {
                    LogContentArrayLock = LogContentArrayLockInductor;
                }
                else if (LogContentArray == LogContentArrayComm)
                {
                    LogContentArrayLock = LogContentArrayLockComm;
                }
                else if (LogContentArray == LogContentArrayFTP1)
                {
                    LogContentArrayLock = LogContentArrayLockFTP1;
                }
                else if (LogContentArray == LogContentArrayFTP2)
                {
                    LogContentArrayLock = LogContentArrayLockFTP2;
                }
                else if (LogContentArray == LogContentArrayFTP3)
                {
                    LogContentArrayLock = LogContentArrayLockFTP3;
                }
                else if (LogContentArray == LogContentArrayFTP4)
                {
                    LogContentArrayLock = LogContentArrayLockFTP4;
                }
                else if (LogContentArray == LogContentArrayFTP5)
                {
                    LogContentArrayLock = LogContentArrayLockFTP5;
                }
                else if (LogContentArray == LogContentArrayFTP6)
                {
                    LogContentArrayLock = LogContentArrayLockFTP6;
                }
                lock (LogContentArrayLock)
                {
                    LogContentArray.Add(new LogContentClass(DateTime.Now.ToString("yyyyMMdd"), logcontent));
                    if (LogContentArray.Count > 1000)
                    {
                        LogContentArray.RemoveAt(0);
                    }
                }
            }
        }
        
        public void SystemLogv2(ArrayList LogContentArray)
        {
            string logpath = "";
            string nowstr = "";
            string log = "";
            if (logEnable)
            {
                string StationLogDir = systemLog_dir + "\\" + systemLog_Station1;
                object SystemLogWriteLock = SystemLogWriteLockSta1;
                object LogContentArrayLock = LogContentArrayLock1;
                if (LogContentArray == LogContentArray1)
                {
                    SystemLogWriteLock = SystemLogWriteLockSta1;
                    LogContentArrayLock = LogContentArrayLock1;
                    StationLogDir = systemLog_dir + "\\" + systemLog_Station1;
                }
                else if (LogContentArray == LogContentArray2)
                {
                    SystemLogWriteLock = SystemLogWriteLockSta2;
                    LogContentArrayLock = LogContentArrayLock2;
                    StationLogDir = systemLog_dir + "\\" + systemLog_Station2;
                }
                else if (LogContentArray == LogContentArray3)
                {
                    SystemLogWriteLock = SystemLogWriteLockSta3;
                    LogContentArrayLock = LogContentArrayLock3;
                    StationLogDir = systemLog_dir + "\\" + systemLog_Station3;
                }
                else if (LogContentArray == LogContentArray4)
                {
                    SystemLogWriteLock = SystemLogWriteLockSta4;
                    LogContentArrayLock = LogContentArrayLock4;
                    StationLogDir = systemLog_dir + "\\" + systemLog_Station4;
                }
                else if (LogContentArray == LogContentArrayInductor)
                {
                    SystemLogWriteLock = SystemLogWriteLockInductor;
                    LogContentArrayLock = LogContentArrayLockInductor;
                    StationLogDir = systemLog_dir + "\\" + systemLog_Inductor;
                }
                else if (LogContentArray == LogContentArrayComm)
                {
                    SystemLogWriteLock = SystemLogWriteLockComm;
                    LogContentArrayLock = LogContentArrayLockComm;
                    StationLogDir = systemLog_dir + "\\" + systemLog_Comm;
                }
                else if (LogContentArray == LogContentArrayFTP1)
                {
                    SystemLogWriteLock = SystemLogWriteLockFTP1;
                    LogContentArrayLock = LogContentArrayLockFTP1;
                    StationLogDir = systemLog_dir + "\\" + systemLog_FTP1;
                }
                else if (LogContentArray == LogContentArrayFTP2)
                {
                    SystemLogWriteLock = SystemLogWriteLockFTP2;
                    LogContentArrayLock = LogContentArrayLockFTP2;
                    StationLogDir = systemLog_dir + "\\" + systemLog_FTP2;
                }
                else if (LogContentArray == LogContentArrayFTP3)
                {
                    SystemLogWriteLock = SystemLogWriteLockFTP3;
                    LogContentArrayLock = LogContentArrayLockFTP3;
                    StationLogDir = systemLog_dir + "\\" + systemLog_FTP3;
                }
                else if (LogContentArray == LogContentArrayFTP4)
                {
                    SystemLogWriteLock = SystemLogWriteLockFTP4;
                    LogContentArrayLock = LogContentArrayLockFTP4;
                    StationLogDir = systemLog_dir + "\\" + systemLog_FTP4;
                }
                else if (LogContentArray == LogContentArrayFTP5)
                {
                    SystemLogWriteLock = SystemLogWriteLockFTP5;
                    LogContentArrayLock = LogContentArrayLockFTP5;
                    StationLogDir = systemLog_dir + "\\" + systemLog_FTP5;
                }
                else if (LogContentArray == LogContentArrayFTP6)
                {
                    SystemLogWriteLock = SystemLogWriteLockFTP6;
                    LogContentArrayLock = LogContentArrayLockFTP6;
                    StationLogDir = systemLog_dir + "\\" + systemLog_FTP6;
                }
                int count = LogContentArray.Count;
                if (count > 0)
                {
                    if (LogContentArray[0] is LogContentClass)
                    {
                        LogContentClass FirstLogContentObj = LogContentArray[0] as LogContentClass;
                        nowstr = FirstLogContentObj.nowstr;
                    }
                    for (int i = 0; i < count; i++)
                    {
                        if (LogContentArray[i] is LogContentClass)
                        {
                            LogContentClass LogContentObj = LogContentArray[i] as LogContentClass;
                            if (nowstr == LogContentObj.nowstr)
                            {
                                log += LogContentObj.logcontent + "\r\n";
                            }
                            else
                            {
                                logpath = StationLogDir + "\\log " + nowstr + ".txt";
                                if (logpath != "" && log != "")
                                {
                                    string logdir = logpath.Substring(0, logpath.LastIndexOf("\\"));
                                    if (!Directory.Exists(logdir))
                                    {
                                        Directory.CreateDirectory(logdir);
                                    }
                                    FileStream fs = null;
                                    StreamWriter sr = null;
                                    lock (SystemLogWriteLock)
                                    {
                                        try
                                        {
                                            fs = new FileStream(logpath, FileMode.Append, FileAccess.Write);
                                            sr = new StreamWriter(fs);
                                            sr.WriteLine(log.Substring(0, log.Length - 2));
                                            sr.Close();
                                            fs.Close();
                                        }
                                        catch (Exception ex) { }
                                        finally
                                        {
                                            if (sr != null)
                                            {
                                                sr.Dispose();
                                            }
                                            if (fs != null)
                                            {
                                                fs.Dispose();
                                            }
                                            
                                        }
                                    }
                                    log = "";
                                    log += LogContentObj.logcontent + "\r\n";
                                    nowstr = LogContentObj.nowstr;
                                }
                            }
                        }
                    }
                    lock (LogContentArrayLock)
                    {
                        LogContentArray.RemoveRange(0, count);
                    }
                    logpath = StationLogDir + "\\log " + nowstr + ".txt";
                    if (logpath != "" && log != "")
                    {
                        string logdir = logpath.Substring(0, logpath.LastIndexOf("\\"));
                        if (!Directory.Exists(logdir))
                        {
                            Directory.CreateDirectory(logdir);
                        }
                        FileStream fs1 = null;
                        StreamWriter sr1 = null;
                        lock (SystemLogWriteLock)
                        {
                            try
                            {
                                fs1 = new FileStream(logpath, FileMode.Append, FileAccess.Write);
                                sr1 = new StreamWriter(fs1);
                                sr1.WriteLine(log.Substring(0, log.Length - 2));
                                sr1.Close();
                                fs1.Close();
                            }
                            catch (Exception ex) { }
                            finally
                            {
                                if (sr1 != null)
                                {
                                    sr1.Dispose();
                                }
                                if (fs1 != null)
                                {
                                    fs1.Dispose();
                                }
                                
                            }
                        }
                    }

                }
            }
        }
        private void UploadRegex(string IP, string Port)
        {
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(IP), Convert.ToInt32(Port)));
                var obj = new { regxArray = JDRegexList.ToArray(), broadRegxArray = BroadJDRegexList.ToArray() };
                socket.Send(Encoding.UTF8.GetBytes("\x02" + JsonConvert.SerializeObject(obj) + "\x03"));
            }
            catch
            {

            }
            finally
            {
                if (socket != null)
                {
                    socket.Dispose();
                }
            }
        }
        private void UploadScada()
        {
            Socket socket = null;
            try
            {
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(new IPEndPoint(IPAddress.Parse(SCADAIP), Convert.ToInt32(SCADAPort)));
                if (SCADAflag)//FTP 状态
                {
                    if (true)
                    {
                        if (ConnectStatusLabel.BackColor == Color.Green || ConnectStatusLabel.BackColor == Color.White)
                        {
                            socket.Send(Encoding.UTF8.GetBytes(FTPOKMes));
                        }
                        else if (ConnectStatusLabel.BackColor == Color.Gray)
                        {
                            socket.Send(Encoding.UTF8.GetBytes(FTPNOKMes));
                        }
                    }
                }
                else//HTTP 状态
                {
                    if (true)
                    {
                        if (HttpConnectStatusLabel.BackColor == Color.Green || HttpConnectStatusLabel.BackColor == Color.White)
                        {
                            socket.Send(Encoding.UTF8.GetBytes(HTTPOKMes));
                        }
                        else if (HttpConnectStatusLabel.BackColor == Color.Gray)
                        {
                            socket.Send(Encoding.UTF8.GetBytes(HTTPNOKMes));
                        }
                    }
                }
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
            }
            catch (Exception ex) { }
            finally
            {
                if (socket != null)
                {
                    socket.Dispose();
                }
            }
            if (SCADAflag)
            {
                SCADAflag = false;
            }
            else
            {
                SCADAflag = true;
            }
        }
        public int ReadFtpLog(string path)//FTP上传文件的LOG
        {
            int sum = 0;
            try
            {
                StreamReader sr = new StreamReader(path);
                try
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        sum++;
                    }
                }
                catch (Exception ex) { sum = -1; }
                sr.Close();
                sr.Dispose();
            }
            catch (Exception ex) { sum = -1; }
            return sum;
        }
        public void FtpRecordLog(string FileName)//FTP上传文件的LOG
        {
            try
            {
                DateTime Timestamp = DateTime.Now;
                string Timestampstr = Timestamp.ToString("yyyyMMdd");
                string path = ftp_log_dir + "\\" + Timestampstr + ".txt";
                if (!Directory.Exists(ftp_log_dir))
                {
                    Directory.CreateDirectory(ftp_log_dir);
                }
                FileStream fs = null;
                StreamWriter sr = null;
                try
                {
                    fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    sr = new StreamWriter(fs);
                    sr.WriteLine(FileName);

                }
                catch (Exception ex) { }
                finally
                {
                    if (sr != null)
                    {
                        sr.Close();
                        sr.Dispose();
                    }
                    if (fs != null)
                    {
                        fs.Close();
                        fs.Dispose();
                    }
                }
            }
            catch (Exception ex) { };
        }
        //上传文件
        public int Upload(FileInfo File, Uri uri, NetworkCredential Credential, ref int LogFailureNum, ArrayList LogContentArray)
        {
            string logcontent = "";
            //0:非2的原因上传不成功；1：上传成功；2：目录不全导致的上传不成功
            FtpWebRequest Uploadreq = (FtpWebRequest)WebRequest.Create(uri);
            File.Refresh();
            if (File.Exists && LogFailureNum < 1)
            {
                FileStream fs = File.OpenRead();
                try
                {
                    long length = fs.Length;
                    Uploadreq.ServicePoint.ConnectionLimit = 10;
                    Uploadreq.Timeout = 3 * 1000;
                    Uploadreq.UseBinary = true;
                    Uploadreq.KeepAlive = (FtpKeepAlive == "true");
                    Uploadreq.UsePassive = true;
                    Uploadreq.Credentials = Credential;
                    //Uploadreq.Credentials = new NetworkCredential(User, Password);
                    Uploadreq.Method = WebRequestMethods.Ftp.UploadFile;
                    Uploadreq.ContentLength = length;
                    Stream stream = Uploadreq.GetRequestStream();
                    try
                    {
                        int BufferLength = 4096; //4K     
                        byte[] b = new byte[BufferLength];
                        int i;
                        while ((i = fs.Read(b, 0, BufferLength)) > 0)
                        {
                            stream.Write(b, 0, i);
                        }

                    }
                    catch (Exception ex)
                    {
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + File.Name + " Read File Error!";
                        AddLog(logcontent, LogContentArray);
                        fs.Close();
                        stream.Close();
                        stream.Dispose();
                        Uploadreq.Abort();
                        return 0;
                    }
                    fs.Close();
                    stream.Close();
                    stream.Dispose();
                    if ((FtpKeepAlive != "true"))
                    {
                        Uploadreq.Abort();
                    }
                    return 1;
                }
                catch (Exception ex)
                {
                    if (ex.Message.IndexOf("530") != -1)//登录失败
                    {
                        /*
                        lock (WarningLock2)
                        {
                            //LogFailureNum++;
                        }
                        */
                        fs.Close();
                        Uploadreq.Abort();
                        return 0;
                    }
                    else if (ex.Message.IndexOf("550") != -1 || ex.Message.IndexOf("551") != -1 || ex.Message.IndexOf("552") != -1 || ex.Message.IndexOf("553") != -1)
                    {
                        fs.Close();
                        Uploadreq.Abort();
                        return 2;
                    }
                    else
                    {
                        fs.Close();
                        Uploadreq.Abort();
                        return 0;
                    }
                }
            }
            else
            {
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + File.Name + " does not exist!";
                AddLog(logcontent, LogContentArray);
                Uploadreq.Abort();
                return 0;
            }
        }
        public bool FtpUpload(FileInfo File,string ServerIP,string FTPPort, NetworkCredential Credential, ref int LogFailureNum, ArrayList LogContentArray)
        {
            string logcontent = "";
            Regex MultiSlash = new Regex("/{2,}");
            //string ftpPath = @"/app/image/app/image/app/";
            string ftpPath = @"";
            string uripathstr = @"";
            Uri uri;
            if (File.Name.IndexOf(delimiter) < 0)
            {
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + File.Name + "File Name Invalid!";
                AddLog(logcontent, LogContentArray);
                return false;
            }
            else if (File.Name.IndexOf(delimiter) > -1 && File.Name.IndexOf(delimiter) + 5 < File.Name.Length)
            {
                ftpPath = @"/" + File.Name.Substring(0, File.Name.IndexOf(delimiter)).Replace(@"~", @"/") + @"/";
                uripathstr = "ftp://" + ServerIP + @":" + FTPPort;
                //string s = uripathstr.TrimEnd('/');
                //Uri u = new Uri(uripathstr.TrimEnd('/') + "/");
                //Uri uu = new Uri(new Uri(uripathstr.TrimEnd('/') + "/"), HttpUtility.UrlEncode(MultiSlash.Replace(ftpPath, @"/").TrimStart('/').TrimEnd('/') + "/").Replace("+", "%20"));
                uri = new Uri(new Uri(uripathstr.TrimEnd('/') + "/"), HttpUtility.UrlEncode(MultiSlash.Replace(ftpPath, @"/").TrimStart('/').TrimEnd('/')+"/"+ File.Name.Substring(File.Name.IndexOf(delimiter) + 5)).Replace("+", "%20"));
                //string uristr = uri.ToString();
            }
            else
            {
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + File.Name + "File Name Invalid!";
                AddLog(logcontent, LogContentArray);
                return false;
            }
            //Uploadreq = (FtpWebRequest)WebRequest.Create(uri);
            //检查目录是否存在，不存在创建 
            int uploadWithoutMkDir = Upload(File, uri, Credential,ref LogFailureNum, LogContentArray);
            if (uploadWithoutMkDir == 1)//上传成功
            {
                return true;
            }
            else if (uploadWithoutMkDir == 2)//目录不全导致上传不成功
            {
                FtpCheckDirectoryExist(ftpPath, ServerIP, FTPPort, Credential,ref LogFailureNum);
                if (Upload(File, uri, Credential,ref LogFailureNum, LogContentArray) == 1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else//其他原因导致上传不成功
            {
                return false;
            }
        }

        //判断文件的目录是否存在,不存在则创建  
        public void FtpCheckDirectoryExist(string destFilePath, string ServerIP, string FTPPort, NetworkCredential Credential, ref int LogFailureNum)
        {
            try
            {
                string fullDir = FtpParseDirectory(destFilePath);
                string[] dirs = fullDir.Split('/');
                string curDir = "/";
                for (int i = 0; i < dirs.Length; i++)
                {
                    string dir = dirs[i];
                    //如果是以/开始的路径,第一个为空    
                    if (dir != null && dir.Length > 0)
                    {
                        try
                        {
                            curDir += dir + "/";
                            FtpMakeDir(curDir, ServerIP, FTPPort, Credential, ref LogFailureNum);
                        }
                        catch (Exception ex)
                        { string s = ex.Message; }
                    }
                }
            }
            catch (Exception ex) { }
        }

        public string FtpParseDirectory(string destFilePath)
        {
            return destFilePath.Substring(0, destFilePath.LastIndexOf("/"));
        }

        //创建目录  
        public bool FtpMakeDir(string ftpPath, string ServerIP, string FTPPort, NetworkCredential Credential,ref int LogFailureNum)
        {
            string uripathstr = "ftp://" + ServerIP + @":" + FTPPort;
            Uri uri = new Uri(new Uri(uripathstr.TrimEnd('/') + "/"), HttpUtility.UrlEncode(ftpPath.TrimStart('/')).Replace("+","%20"));
            FtpWebRequest MakeDirreq = (FtpWebRequest)WebRequest.Create(uri);
            //FtpWebRequest MakeDirreq = (FtpWebRequest)WebRequest.Create("ftp://" + ServerIP + @":" + FTPPort + ftpPath);
            //MakeDirreq = (FtpWebRequest)WebRequest.Create("ftp://" + ServerIP + @":" + FTPPort + ftpPath);
            MakeDirreq.Timeout = 3000;
            MakeDirreq.ServicePoint.ConnectionLimit = 10;
            MakeDirreq.KeepAlive = true;
            MakeDirreq.UsePassive = true;
            MakeDirreq.Credentials = Credential;
            //MakeDirreq.Credentials = new NetworkCredential(User, Password);
            MakeDirreq.Method = WebRequestMethods.Ftp.MakeDirectory;
            try
            {
                if (LogFailureNum < 1)
                {
                    FtpWebResponse response = (FtpWebResponse)MakeDirreq.GetResponse();
                    response.Close();
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.IndexOf("530") != -1)
                {
                    /*
                    lock (WarningLock2)
                    {
                        //LogFailureNum++;
                    }
                    */
                }
                MakeDirreq.Abort();
                return false;
            }
            //MakeDirreq.Abort();  
            return true;
        }
        private void UploadFile(string tempfolder, string ServerIP, string FTPPort, NetworkCredential Credential, ref int LogFailureNum, ArrayList LogContentArray)
        {
            string logcontent = "";
            if (!Directory.Exists(tempfolder))
            {
                Directory.CreateDirectory(tempfolder);
            }
            DirectoryInfo directoryinfo = new DirectoryInfo(tempfolder);
            FileInfo[] Files = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + Files.Length + " Files Found!";
            AddLog(logcontent, LogContentArray);
            /*
            Stopwatch sw = new Stopwatch();
            sw.Start();
              */
            foreach (FileInfo file in Files)
            {
                if (UploadMode == "sftp" && tempfolder != ftp_temp_url6)
                {
                    break;
                }
                if ((DateTime.Now - file.CreationTime).TotalMilliseconds > 800)
                {
                    if ((DateTime.Now - file.CreationTime).TotalSeconds < 300 || tempfolder != ftp_temp_url6)
                    {
                        /*
                        if (tempfolder != ftp_temp_url6 && LogFailureNum1 > 0)
                        {
                            //Timer1.Stop();
                            //Timer2.Stop();
                            //Timer3.Stop();
                            //Timer4.Stop();
                            Timer7.Stop();
                            Timer8.Stop();
                            //Timer9.Stop();
                            //Timer9.Stop();
                            //Timer10.Stop();
                            //Timer11.Stop();
                            lock (WarningLock1)
                            {
                                if (!warningflag1)
                                {
                                    MessageBox.Show("ftp用户名或密码有误，请修改", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    warningflag1 = true;
                                }
                            }
                            break;
                        }
                        */
                        /*
                        if (tempfolder == ftp_temp_url6 && LogFailureNum2 > 0)
                        {
                            //Timer1.Stop();
                            //Timer2.Stop();
                            //Timer3.Stop();
                            //Timer4.Stop();
                            //Timer7.Stop();
                            //Timer8.Stop();
                            Timer36.Stop();
                            //Timer9.Stop();
                            //Timer10.Stop();
                            //Timer11.Stop();
                            lock (WarningLock3)
                            {
                                if (!warningflag2)
                                {
                                    MessageBox.Show("多路输出ftp用户名或密码有误，请修改", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    warningflag2 = true;
                                }
                            }
                            break;
                        }
                        */
                        try
                        {
                            if (FtpUpload(file, ServerIP, FTPPort, Credential, ref LogFailureNum,LogContentArray))
                            {
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " ftp uploaded successfully!";
                                AddLog(logcontent, LogContentArray);
                                lock (FtpLogWriteLock)
                                {
                                    if (file.Name.IndexOf(delimiter) > -1 && file.Name.IndexOf(delimiter) + 5 < file.Name.Length && tempfolder != ftp_temp_url6)
                                    {
                                        FtpRecordLog("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name.Substring(file.Name.IndexOf(delimiter) + 5));
                                    }
                                }
                                file.Delete();
                                if (tempfolder != ftp_temp_url6)
                                {
                                    if (ConnectStatusLabel.InvokeRequired)
                                    {
                                        if (!ConnectStatusLabel.IsDisposed)
                                        {
                                            ConnectStatusLabel.Invoke(new SetLabelDelegate(setLabelGreen));
                                        }
                                    }
                                }
                                else
                                {
                                    if (ConnectStatusLabel2.InvokeRequired)
                                    {
                                        if (!ConnectStatusLabel2.IsDisposed)
                                        {
                                            ConnectStatusLabel2.Invoke(new SetLabelDelegate(setLabelGreen2));
                                        }
                                    }
                                }
                                UpLoadOK = true;
                            }
                            else
                            {
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " ftp uploaded failed!";
                                AddLog(logcontent, LogContentArray);
                                if (tempfolder != ftp_temp_url6)
                                {
                                    if (ConnectStatusLabel.InvokeRequired)
                                    {
                                        if (!ConnectStatusLabel.IsDisposed)
                                        {
                                            ConnectStatusLabel.Invoke(new SetLabelDelegate(setLabelGray));
                                        }
                                    }
                                }
                                else
                                {
                                    if (ConnectStatusLabel2.InvokeRequired)
                                    {
                                        if (!ConnectStatusLabel2.IsDisposed)
                                        {
                                            ConnectStatusLabel2.Invoke(new SetLabelDelegate(setLabelGray2));
                                        }
                                    }
                                    try
                                    {
                                        file.Delete();
                                    }
                                    catch (Exception ex) { }
                                }
                                UpLoadOK = false;
                            }
                        }
                        catch (Exception ex) { }
                    }
                    else
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex) { }
                    }
                }
            }
            /*
            sw.Stop();
            if (sw.ElapsedMilliseconds >= 10)
            {
                MessageBox.Show(sw.ElapsedMilliseconds + "ms");
            }
            */
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Files Clear!";
            AddLog(logcontent, LogContentArray);

        }
        private bool sftpPut(string tempfolder, FileInfo file, ArrayList LogContentArray)
        {
            string logcontent = "";
            SFTPHelper sftp = sftp1;
            Regex MultiSlash = new Regex("/{2,}");
            if (tempfolder == ftp_temp_url1)
            {
                sftp = sftp1;
            }
            else if (tempfolder == ftp_temp_url2)
            {
                sftp = sftp2;
            }
            else if (tempfolder == ftp_temp_url3)
            {
                sftp = sftp3;
            }
            else if (tempfolder == ftp_temp_url4)
            {
                sftp = sftp4;
            }
            else if (tempfolder == ftp_temp_url5)
            {
                sftp = sftp5;
            }
            if (file != null && sftp != null)
            {
                if (file.Name.IndexOf(delimiter) < 0)
                {
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " File Name Invalid!";
                    AddLog(logcontent, LogContentArray);
                    return false;
                }
                else if (file.Name.IndexOf(delimiter) > -1 && file.Name.IndexOf(delimiter) + 5 < file.Name.Length)
                {
                    string RemotePath = @"/" + file.Name.Substring(0, file.Name.IndexOf(delimiter)).Replace(@"~", @"/") + @"/";
                    RemotePath = MultiSlash.Replace(RemotePath, @"/");
                    string FileName = file.Name.Substring(file.Name.IndexOf(delimiter) + 5);
                    if (sftp.Put(file.FullName, RemotePath, FileName))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    //uri = new Uri(new Uri(uripathstr.TrimEnd('/') + "/"), HttpUtility.UrlEncode(MultiSlash.Replace(ftpPath, @"/").TrimStart('/').TrimEnd('/') + "/" + File.Name.Substring(File.Name.IndexOf(delimiter) + 5)).Replace("+", "%20"));
                    //string uristr = uri.ToString();
                }
                else
                {
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " File Name Invalid!";
                    AddLog(logcontent, LogContentArray);
                    return false;
                }
            }
            else
            {
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " File/SftpClient does not exist!";
                AddLog(logcontent, LogContentArray);
                return false;
            }
        }
        private void sftpUploadFile(string tempfolder, ArrayList LogContentArray)
        {
            string logcontent = "";
            if (!Directory.Exists(tempfolder))
            {
                Directory.CreateDirectory(tempfolder);
            }
            DirectoryInfo directoryinfo = new DirectoryInfo(tempfolder);
            FileInfo[] Files = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + Files.Length + " Files Found!";
            AddLog(logcontent, LogContentArray);
            foreach (FileInfo file in Files)
            {
                if (UploadMode == "ftp")
                {
                    break;
                }
                if ((DateTime.Now - file.CreationTime).TotalMilliseconds > 800)
                {
                    try
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        if (sftpPut(tempfolder, file, LogContentArray))
                        {
                            sw.Stop();
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " sftp uploaded successfully!";
                            AddLog(logcontent, LogContentArray);
                            lock (FtpLogWriteLock)
                            {
                                if (file.Name.IndexOf(delimiter) > -1 && file.Name.IndexOf(delimiter) + 5 < file.Name.Length && tempfolder != ftp_temp_url6)
                                {
                                    FtpRecordLog("[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name.Substring(file.Name.IndexOf(delimiter) + 5) + " encrypt & transfer time: " + sw.ElapsedMilliseconds + "ms.");
                                }
                            }
                            file.Delete();
                            if (tempfolder != ftp_temp_url6)
                            {
                                if (ConnectStatusLabel.InvokeRequired)
                                {
                                    if (!ConnectStatusLabel.IsDisposed)
                                    {
                                        ConnectStatusLabel.Invoke(new SetLabelDelegate(setLabelGreen));
                                    }
                                }
                            }
                            else
                            {
                                if (ConnectStatusLabel2.InvokeRequired)
                                {
                                    if (!ConnectStatusLabel2.IsDisposed)
                                    {
                                        ConnectStatusLabel2.Invoke(new SetLabelDelegate(setLabelGreen2));
                                    }
                                }
                            }
                        }
                        else
                        {
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " sftp uploaded failed!";
                            AddLog(logcontent, LogContentArray);
                            if (tempfolder != ftp_temp_url6)
                            {
                                if (ConnectStatusLabel.InvokeRequired)
                                {
                                    if (!ConnectStatusLabel.IsDisposed)
                                    {
                                        ConnectStatusLabel.Invoke(new SetLabelDelegate(setLabelGray));
                                    }
                                }
                            }
                            else
                            {
                                if (ConnectStatusLabel2.InvokeRequired)
                                {
                                    if (!ConnectStatusLabel2.IsDisposed)
                                    {
                                        ConnectStatusLabel2.Invoke(new SetLabelDelegate(setLabelGray2));
                                    }
                                }
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception ex) { }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Files Clear!";
            AddLog(logcontent, LogContentArray);
        }
        /*private void deletefiles(string srcfolder, int days)//定时删除文件夹中的文件
        {
            if (srcfolder != "")
            {
                if (!Directory.Exists(srcfolder))
                {
                    Directory.CreateDirectory(srcfolder);
                }
                DirectoryInfo directoryinfo = new DirectoryInfo(srcfolder);
                FileInfo[] Files = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
                foreach (FileInfo file in Files)
                {
                    if ((DateTime.Now - file.CreationTime).Days > days)
                    {
                        try
                        {
                            file.Delete();
                        }
                        catch (Exception ex)
                        { }
                    }

                }
                foreach (DirectoryInfo Folder in directoryinfo.GetDirectories("*", SearchOption.AllDirectories))
                {
                    try
                    {
                        if (Folder.GetFiles("*", SearchOption.AllDirectories).Length == 0 && (DateTime.Now - Folder.CreationTime).TotalDays > 2)
                        {

                            Folder.Delete(true);

                        }

                    }
                    catch (Exception ex) { }
                }
            }
        }*/
        private void deleteImageFolderFiles(string srcfolder, int days)
        {
            try
            {
                if (srcfolder != "")
                {
                    if (!Directory.Exists(srcfolder))
                    {
                        Directory.CreateDirectory(srcfolder);
                    }
                    DirectoryInfo directoryinfo = new DirectoryInfo(srcfolder);
                    //FileInfo[] Files = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
                    FileInfo[] Files = directoryinfo.GetFiles();
                    if (Files.Length > 0)
                    {
                        foreach (FileInfo file in Files)
                        {
                            if ((DateTime.Now - file.CreationTime).Days > days)
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    DirectoryInfo[] Folders = directoryinfo.GetDirectories("*", SearchOption.AllDirectories);
                    for (int i = Folders.Length - 1; i >= 0; i--)
                    {
                        /*Files = Folder.GetFiles();
                        if (Files.Length > 0)
                        {
                            foreach (FileInfo file in Files)
                            {
                                if ((DateTime.Now - file.CreationTime).Days > days)
                                {
                                    try
                                    {
                                        file.Delete();
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                            }
                        }*/
                        try
                        {
                            if (Folders[i].GetDirectories().Length == 0 && (DateTime.Now - Folders[i].CreationTime).TotalDays > days)
                            {

                                Folders[i].Delete(true);
                                //Thread.Sleep(10);
                            }
                        }
                        catch (Exception ex) { }
                    }
                    /*
                    for (int i = Folders.Length - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (Folders[i].GetFiles().Length == 0 && Folders[i].GetDirectories().Length == 0 && (DateTime.Now - Folders[i].CreationTime).TotalDays > 2 && Folders[i].Exists)
                            {

                                Folders[i].Delete(true);
                                //Thread.Sleep(10);
                            }

                        }
                        catch (Exception ex) { }
                    }
                    */
                }
            }
            catch (Exception ex) { }
        }
        private void deletefiles(string srcfolder, int days)
        {
            try
            {
                if (srcfolder != "")
                {

                    if (!Directory.Exists(srcfolder))
                    {
                        Directory.CreateDirectory(srcfolder);
                    }
                    DirectoryInfo directoryinfo = new DirectoryInfo(srcfolder);
                    //FileInfo[] Files = directoryinfo.GetFiles("*", SearchOption.AllDirectories);
                    FileInfo[] Files = directoryinfo.GetFiles();
                    if (Files.Length > 0)
                    {
                        foreach (FileInfo file in Files)
                        {
                            if ((DateTime.Now - file.CreationTime).Days > days)
                            {
                                try
                                {
                                    file.Delete();
                                }
                                catch (Exception ex)
                                { }
                            }
                        }
                    }
                    DirectoryInfo[] Folders = directoryinfo.GetDirectories("*", SearchOption.AllDirectories);
                    foreach (DirectoryInfo Folder in Folders)
                    {
                        Files = Folder.GetFiles();
                        if (Files.Length > 0)
                        {
                            foreach (FileInfo file in Files)
                            {
                                if ((DateTime.Now - file.CreationTime).Days > days)
                                {
                                    try
                                    {
                                        file.Delete();
                                    }
                                    catch (Exception ex)
                                    { }
                                }
                            }
                        }
                    }
                    for (int i = Folders.Length - 1; i >= 0; i--)
                    {
                        try
                        {
                            if (Folders[i].GetFiles().Length == 0 && Folders[i].GetDirectories().Length == 0 && (DateTime.Now - Folders[i].CreationTime).TotalDays > 2)
                            {

                                Folders[i].Delete(true);
                                //Thread.Sleep(10);
                            }

                        }
                        catch (Exception ex) { }
                    }
                }
            }
            catch (Exception ex) { }
        }
        private bool HandlePic(BottomStitchObject obj, int btncamcount, ArrayList LogContentArray, string StationName, string GroupID, int[] ol, long Timeout)
        {
            DateTime Now = DateTime.Now;
            bool flag = false;
            Mat BT01mat = null;
            Mat BT02mat = null;
            Mat BT03mat = null;
            Mat BT04mat = null;
            Mat BT05mat = null;
            Mat BT06mat = null;
            Mat lastmat = null;
            Mat CurrentWarp = null;
            Mat CurrentWarpRGB = null;
            int lastid = 0;
            int time = 0;
            Image image = null;
            Graphics g = null;
            Font font = null;
            SolidBrush sb = null;
            MemoryStream ms = null;
            Mat[] H = HomographySta1;
            string logcontent = "";
            string CurrentImageName = "";
            if (obj.StationName == "1")
            {
                H = HomographySta1;
            }
            else if (obj.StationName == "2")
            {
                H = HomographySta2;
            }
            else if (obj.StationName == "3")
            {
                H = HomographySta3;
            }
            else if (obj.StationName == "4")
            {
                H = HomographySta4;
            }
            else
            {
                H = HomographySta1;
            }
            try
            {
                //BT01
                Mat[] BT01raw = { null, null, null, null };
                int BT01col = 0;
                int BT01row = ol[0];
                int endid01 = -1;
                int currow01 = 0;
                try
                {
                    for (int i1 = 0; i1 < obj.BT01.Count; i1++)
                    {
                        if (obj.BT01[i1] != null)
                        {
                            BT01raw[i1] = Cv2.ImRead(obj.BT01[i1].FullName, ImreadModes.Grayscale);
                            if (BT01raw[i1] != null)
                            {
                                if (!BT01raw[i1].Empty())
                                {
                                    BT01row += BT01raw[i1].Rows;
                                    BT01row -= ol[0];
                                    BT01col = Math.Max(BT01col, BT01raw[i1].Cols);
                                    endid01 = i1;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (endid01 > -1)
                    {
                        if (BT01raw[0] != null)
                        {
                            BT01mat = new Mat(BT01row, BT01col, MatType.CV_8UC1, new Scalar(0));
                            BT01mat[0, BT01raw[0].Rows, 0, BT01raw[0].Cols] = BT01raw[0];
                            currow01 = BT01raw[0].Rows;
                            for (int ii1 = 1; ii1 <= endid01; ii1++)
                            {
                                if (BT01raw[ii1] != null)
                                {
                                    BT01mat[currow01, currow01 + BT01raw[ii1].Rows - ol[0], 0, BT01raw[ii1].Cols] = BT01raw[ii1][ol[0], BT01raw[ii1].Rows, 0, BT01raw[ii1].Cols];
                                    currow01 += BT01raw[ii1].Rows - ol[0];
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }   
                }
                catch (Exception ex) { }
                finally
                {
                    for (int iii1 = 0; iii1 < BT01raw.Length; iii1++)
                    {
                        if (BT01raw[iii1] != null)
                        {
                            BT01raw[iii1].Dispose();
                        }
                    }
                }
                //BT02
                Mat[] BT02raw = { null, null, null, null };
                int BT02col = 0;
                int BT02row = ol[1];
                int endid02 = -1;
                int currow02 = 0;
                try
                {
                    for (int i2 = 0; i2 < obj.BT02.Count; i2++)
                    {
                        if (obj.BT02[i2] != null)
                        {
                            BT02raw[i2] = Cv2.ImRead(obj.BT02[i2].FullName, ImreadModes.Grayscale);
                            if (BT02raw[i2] != null)
                            {
                                if (!BT02raw[i2].Empty())
                                {
                                    BT02row += BT02raw[i2].Rows;
                                    BT02row -= ol[1];
                                    BT02col = Math.Max(BT02col, BT02raw[i2].Cols);
                                    endid02 = i2;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (endid02 > -1)
                    {
                        if (BT02raw[0] != null)
                        {
                            BT02mat = new Mat(BT02row, BT02col, MatType.CV_8UC1, new Scalar(0));
                            BT02mat[0, BT02raw[0].Rows, 0, BT02raw[0].Cols] = BT02raw[0];
                            currow02 = BT02raw[0].Rows;
                            for (int ii2 = 1; ii2 <= endid02; ii2++)
                            {
                                if (BT02raw[ii2] != null)
                                {
                                    BT02mat[currow02, currow02 + BT02raw[ii2].Rows - ol[1], 0, BT02raw[ii2].Cols] = BT02raw[ii2][ol[1], BT02raw[ii2].Rows, 0, BT02raw[ii2].Cols];
                                    currow02 += BT02raw[ii2].Rows - ol[1];
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                catch (Exception ex) { }
                finally
                {
                    for (int iii2 = 0; iii2 < BT02raw.Length; iii2++)
                    {
                        if (BT02raw[iii2] != null)
                        {
                            BT02raw[iii2].Dispose();
                        }
                    }
                }
                //BT03
                Mat[] BT03raw = { null, null, null, null };
                int BT03col = 0;
                int BT03row = ol[2];
                int endid03 = -1;
                int currow03 = 0;
                try
                {
                    for (int i3 = 0; i3 < obj.BT03.Count; i3++)
                    {
                        if (obj.BT03[i3] != null)
                        {
                            BT03raw[i3] = Cv2.ImRead(obj.BT03[i3].FullName, ImreadModes.Grayscale);
                            if (BT03raw[i3] != null)
                            {
                                if (!BT03raw[i3].Empty())
                                {
                                    BT03row += BT03raw[i3].Rows;
                                    BT03row -= ol[2];
                                    BT03col = Math.Max(BT03col, BT03raw[i3].Cols);
                                    endid03 = i3;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (endid03 > -1)
                    {
                        if (BT03raw[0] != null)
                        {
                            BT03mat = new Mat(BT03row, BT03col, MatType.CV_8UC1, new Scalar(0));
                            BT03mat[0, BT03raw[0].Rows, 0, BT03raw[0].Cols] = BT03raw[0];
                            currow03 = BT03raw[0].Rows;
                            for (int ii3 = 1; ii3 <= endid03; ii3++)
                            {
                                if (BT03raw[ii3] != null)
                                {
                                    BT03mat[currow03, currow03 + BT03raw[ii3].Rows - ol[2], 0, BT03raw[ii3].Cols] = BT03raw[ii3][ol[2], BT03raw[ii3].Rows, 0, BT03raw[ii3].Cols];
                                    currow03 += BT03raw[ii3].Rows - ol[2];
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                catch (Exception ex) { }
                finally
                {
                    for (int iii3 = 0; iii3 < BT03raw.Length; iii3++)
                    {
                        if (BT03raw[iii3] != null)
                        {
                            BT03raw[iii3].Dispose();
                        }
                    }
                }
                //BT04
                Mat[] BT04raw = { null, null, null, null };
                int BT04col = 0;
                int BT04row = ol[3];
                int endid04 = -1;
                int currow04 = 0;
                try
                {
                    for (int i4 = 0; i4 < obj.BT04.Count; i4++)
                    {
                        if (obj.BT04[i4] != null)
                        {
                            BT04raw[i4] = Cv2.ImRead(obj.BT04[i4].FullName, ImreadModes.Grayscale);
                            if (BT04raw[i4] != null)
                            {
                                if (!BT04raw[i4].Empty())
                                {
                                    BT04row += BT04raw[i4].Rows;
                                    BT04row -= ol[3];
                                    BT04col = Math.Max(BT04col, BT04raw[i4].Cols);
                                    endid04 = i4;
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (endid04 > -1)
                    {
                        if (BT04raw[0] != null)
                        {
                            BT04mat = new Mat(BT04row, BT04col, MatType.CV_8UC1, new Scalar(0));
                            BT04mat[0, BT04raw[0].Rows, 0, BT04raw[0].Cols] = BT04raw[0];
                            currow04 = BT04raw[0].Rows;
                            for (int ii4 = 1; ii4 <= endid04; ii4++)
                            {
                                if (BT04raw[ii4] != null)
                                {
                                    BT04mat[currow04, currow04 + BT04raw[ii4].Rows - ol[3], 0, BT04raw[ii4].Cols] = BT04raw[ii4][ol[3], BT04raw[ii4].Rows, 0, BT04raw[ii4].Cols];
                                    currow04 += BT04raw[ii4].Rows - ol[3];
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                    
                }
                catch (Exception ex) { }
                finally
                {
                    for (int iii4 = 0; iii4 < BT04raw.Length; iii4++)
                    {
                        if (BT04raw[iii4] != null)
                        {
                            BT04raw[iii4].Dispose();
                        }
                    }
                }
                //BT05
                if (btncamcount == 5 || btncamcount == 6)
                {
                    Mat[] BT05raw = { null, null, null, null };
                    int BT05col = 0;
                    int BT05row = ol[4];
                    int endid05 = -1;
                    int currow05 = 0;
                    try
                    {
                        for (int i5 = 0; i5 < obj.BT05.Count; i5++)
                        {
                            if (obj.BT05[i5] != null)
                            {
                                BT05raw[i5] = Cv2.ImRead(obj.BT05[i5].FullName, ImreadModes.Grayscale);
                                if (BT05raw[i5] != null)
                                {
                                    if (!BT05raw[i5].Empty())
                                    {
                                        BT05row += BT05raw[i5].Rows;
                                        BT05row -= ol[4];
                                        BT05col = Math.Max(BT05col, BT05raw[i5].Cols);
                                        endid05 = i5;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (endid05 > -1)
                        {
                            if (BT05raw[0] != null)
                            {
                                BT05mat = new Mat(BT05row, BT05col, MatType.CV_8UC1, new Scalar(0));
                                BT05mat[0, BT05raw[0].Rows, 0, BT05raw[0].Cols] = BT05raw[0];
                                currow05 = BT05raw[0].Rows;
                                for (int ii5 = 1; ii5 <= endid05; ii5++)
                                {
                                    if (BT05raw[ii5] != null)
                                    {
                                        BT05mat[currow05, currow05 + BT05raw[ii5].Rows - ol[4], 0, BT05raw[ii5].Cols] = BT05raw[ii5][ol[4], BT05raw[ii5].Rows, 0, BT05raw[ii5].Cols];
                                        currow05 += BT05raw[ii5].Rows - ol[4];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        for (int iii5 = 0; iii5 < BT05raw.Length; iii5++)
                        {
                            if (BT05raw[iii5] != null)
                            {
                                BT05raw[iii5].Dispose();
                            }
                        }
                    }
                }
                //BT06
                if (btncamcount == 6)
                {
                    Mat[] BT06raw = { null, null, null, null };
                    int BT06col = 0;
                    int BT06row = ol[5];
                    int endid06 = -1;
                    int currow06 = 0;
                    try
                    {
                        for (int i6 = 0; i6 < obj.BT06.Count; i6++)
                        {
                            if (obj.BT06[i6] != null)
                            {
                                BT06raw[i6] = Cv2.ImRead(obj.BT06[i6].FullName, ImreadModes.Grayscale);
                                if (BT06raw[i6] != null)
                                {
                                    if (!BT06raw[i6].Empty())
                                    {
                                        BT06row += BT06raw[i6].Rows;
                                        BT06row -= ol[5];
                                        BT06col = Math.Max(BT06col, BT06raw[i6].Cols);
                                        endid06 = i6;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    break;
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        if (endid06 > -1)
                        {
                            if (BT06raw[0] != null)
                            {
                                BT06mat = new Mat(BT06row, BT06col, MatType.CV_8UC1, new Scalar(0));
                                BT06mat[0, BT06raw[0].Rows, 0, BT06raw[0].Cols] = BT06raw[0];
                                currow06 = BT06raw[0].Rows;
                                for (int ii6 = 1; ii6 <= endid06; ii6++)
                                {
                                    if (BT06raw[ii6] != null)
                                    {
                                        BT06mat[currow06, currow06 + BT06raw[ii6].Rows - ol[5], 0, BT06raw[ii6].Cols] = BT06raw[ii6][ol[5], BT06raw[ii6].Rows, 0, BT06raw[ii6].Cols];
                                        currow06 += BT06raw[ii6].Rows - ol[5];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        
                    }
                    catch (Exception ex) { }
                    finally
                    {
                        for (int iii6 = 0; iii6 < BT06raw.Length; iii6++)
                        {
                            if (BT06raw[iii6] != null)
                            {
                                BT06raw[iii6].Dispose();
                            }
                        }
                    }
                }
                Mat[] BTmat = { BT01mat, BT02mat, BT03mat, BT04mat, BT05mat, BT06mat };
                /*
                Cv2.ImWrite("D:\\test\\BT01.jpg", BT01mat);
                Cv2.ImWrite("D:\\test\\BT02.jpg", BT02mat);
                Cv2.ImWrite("D:\\test\\BT03.jpg", BT03mat); 
                Cv2.ImWrite("D:\\test\\BT04.jpg", BT04mat);
                Cv2.ImWrite("D:\\test\\BT05.jpg", BT05mat);
                Cv2.ImWrite("D:\\test\\BT06.jpg", BT06mat);
                */
                for (int i = BTmat.Length - 1; i > -1; i--)
                {
                    if (BTmat[i] != null && lastmat == null)
                    { 
                        if(!BTmat[i].Empty())
                        {
                            lastid = i;
                            lastmat = BTmat[i];
                            break;
                        }
                    }
                }
                /*
                for (int i = obj.Images.Count - 1; i > -1; i--)
                {
                    if (obj.Images[i] != null && lastmat == null)
                    {

                        lastmat = Cv2.ImRead(obj.Images[i].FullName,ImreadModes.Grayscale);
                        if (!lastmat.Empty())
                        {
                            lastid = i;
                            //CurrentImageName = obj.Images[i].FullName;
                            break;
                        }
                        else
                        {
                            lastmat.Dispose();
                            lastmat = null;
                        }
                    }
                }
                */
                if (lastmat != null)
                {
                    CurrentWarp = new Mat(lastmat.Rows, (int)((0.6 * btncamcount + 0.4) * lastmat.Cols), MatType.CV_8UC1,new Scalar(0));
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + "当前图片:" + CurrentWarp.ToString();
                    AddLog(logcontent, LogContentArray);
                    CurrentWarp[0, lastmat.Rows, 0, lastmat.Cols] = lastmat;
                    time++;
                    for (int j = lastid; j > 0; j--)
                    {
                        Cv2.WarpPerspective(CurrentWarp, CurrentWarp, H[j - 1], CurrentWarp.Size());
                        //NextMat = Cv2.ImRead(obj.Images[j - 1].FullName,ImreadModes.Grayscale);
                        if (BTmat[j - 1] != null)
                        {
                            if (!BTmat[j - 1].Empty())
                            {
                                for (int m = 0; m < Math.Min(CurrentWarp.Rows, BTmat[j - 1].Rows); m++)
                                {
                                    for (int n = BTmat[j - 1].Cols - 100; n < BTmat[j - 1].Cols; n++)
                                    {
                                        if (CurrentWarp.At<byte>(m, n) > 10)
                                        {
                                            CurrentWarp.At<byte>(m, n) = (byte)((n - (BTmat[j - 1].Cols - 100)) * 0.01 * CurrentWarp.At<byte>(m, n) + (1 - (n - (BTmat[j - 1].Cols - 100)) * 0.01) * BTmat[j - 1].At<byte>(m, n));
                                        }
                                        else
                                        {
                                            CurrentWarp.At<byte>(m, n) = BTmat[j - 1].At<byte>(m, n);
                                        }

                                    }
                                }
                                CurrentWarp[0, Math.Min(CurrentWarp.Rows, BTmat[j - 1].Rows), 0, BTmat[j - 1].Cols - 100] = BTmat[j - 1][0, Math.Min(CurrentWarp.Rows, BTmat[j - 1].Rows), 0, BTmat[j - 1].Cols - 100];
                                //CurrentWarp[0, Math.Min(CurrentWarp.Rows,NextMat.Rows), 0, NextMat.Cols] = NextMat[0, Math.Min(CurrentWarp.Rows, NextMat.Rows), 0, NextMat.Cols];
                                time++;
                                //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + "当前图片:" + CurrentImageName;
                                //AddLog(logcontent, LogContentArray);
                            }
                        }
                        
                    }
                    if ((long)(Now - obj.TimeStamp).TotalMilliseconds > Timeout)
                    {
                        /*if (!Directory.Exists(obj.extCaptionDir))
                        {
                            Directory.CreateDirectory(obj.extCaptionDir);
                        }*/
                        Regex MultiSlash = new Regex("\\\\{2,}");
                        Regex Multibar = new Regex("~{2,}");
                        Regex Startbar = new Regex("^~");
                        Regex Endbar = new Regex("~$");
                        string local_fullname = "";//本地文件全名含路径
                        string ftp_fullname = "";
                        if (captionEnable)
                        {
                            string Des = obj.Description;
                            string ext0 = "";
                            string ext1 = "";
                            string ext2 = "";
                            string ext3 = "";
                            string ext4 = "";
                            string ext5 = "";
                            string ext6 = "";
                            string ext7 = "";
                            string ext8 = "";
                            string ext9 = "";
                            if (extCaptionEnable)
                            {
                                if (obj.extDescription != null)
                                {
                                    string[] items = obj.extDescription.Split(';');
                                    if (items.Length >= 1)
                                    {
                                        ext0 = items[0];
                                    }
                                    if (items.Length >= 2)
                                    {
                                        ext1 = items[1];
                                    }
                                    if (items.Length >= 3)
                                    {
                                        ext2 = items[2];
                                    }
                                    if (items.Length >= 4)
                                    {
                                        ext3 = items[3];
                                    }
                                    if (items.Length >= 5)
                                    {
                                        ext4 = items[4];
                                    }
                                    if (items.Length >= 6)
                                    {
                                        ext5 = items[5];
                                    }
                                    if (items.Length >= 7)
                                    {
                                        ext6 = items[6];
                                    }
                                    if (items.Length >= 8)
                                    {
                                        ext7 = items[7];
                                    }
                                    if (items.Length >= 9)
                                    {
                                        ext8 = items[8];
                                    }
                                    if (items.Length >= 10)
                                    {
                                        ext9 = items[9];
                                    }
                                }
                            }
                            Des = Des.Replace("#EXT0", ext0).Replace("#EXT1", ext1).Replace("#EXT2", ext2).Replace("#EXT3", ext3);
                            Des = Des.Replace("#EXT4", ext4).Replace("#EXT5", ext5).Replace("#EXT6", ext6).Replace("#EXT7", ext7);
                            Des = Des.Replace("#EXT8", ext8).Replace("#EXT9", ext9);
                            if (NoReadAnalysisEnable)
                            {

                                string NoReadReason = "N/A";
                                if (obj.Result == "NR")
                                {
                                    //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] "  + "StationName: " + StationName + "GroupID: " + GroupID + " Starting Bottom No Read Analysis!";
                                    //AddLog(logcontent, LogContentArray);
#if FEATURE_VPRO
                                    NoReadReason = NoReadAnalyzeFromMat(CurrentWarp, LogContentArray, StationName, GroupID);
#endif
                                    //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + " Finished Bottom No Read Analysis!";
                                    //AddLog(logcontent, LogContentArray);
                                }
                                Des = Des.Replace("#ANA", NoReadReason);
                                string NoReadReasonPath = NoReadReason.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                if (NoReadReason != "N/A")
                                {
                                    obj.localFileName = obj.localFileName.Replace("#ANA", NoReadReasonPath);
                                    obj.ftpFileNameHeader = obj.ftpFileNameHeader.Replace("#ANA", NoReadReasonPath);
                                    obj.ftpFileName = obj.ftpFileName.Replace("#ANA", NoReadReasonPath);
                                    obj.localDir = obj.localDir.Replace("#ANA", NoReadReasonPath);
                                }
                                else
                                {
                                    obj.localFileName = obj.localFileName.Replace("#ANA", "NA");
                                    obj.ftpFileNameHeader = obj.ftpFileNameHeader.Replace("#ANA", "");
                                    obj.ftpFileName = obj.ftpFileName.Replace("#ANA", "NA");
                                    obj.localDir = obj.localDir.Replace("#ANA", "");
                                }

                            }
                            else
                            {
                                Des = Des.Replace("#ANA", "N/A");
                                obj.localFileName = obj.localFileName.Replace("#ANA", "NA");
                                obj.ftpFileNameHeader = obj.ftpFileNameHeader.Replace("#ANA", "");
                                obj.ftpFileName = obj.ftpFileName.Replace("#ANA", "NA");
                                obj.localDir = obj.localDir.Replace("#ANA", "");
                            }
                            obj.ftpFileNameHeader = Multibar.Replace(obj.ftpFileNameHeader, "~");
                            obj.ftpFileNameHeader = Startbar.Replace(obj.ftpFileNameHeader, "");
                            obj.ftpFileNameHeader = Endbar.Replace(obj.ftpFileNameHeader, "");
                            obj.localDir = obj.localDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                            obj.localDir = MultiSlash.Replace(obj.localDir, "\\");//本地路径

                            local_fullname = obj.localDir + obj.ftpFileName;//本地文件全名含路径
                            ftp_fullname = obj.ftpDir + "\\" + obj.ftpFileNameHeader + delimiter + obj.ftpFileName;
                            if (!Directory.Exists(obj.localDir))
                            {
                                Directory.CreateDirectory(obj.localDir);
                            }
                            if (!Directory.Exists(obj.ftpDir))
                            {
                                Directory.CreateDirectory(obj.ftpDir);
                            }
                            if (allowChinese)
                            {
                                ms = CurrentWarp.ToMemoryStream();
                                image = Image.FromStream(ms);
                                g = Graphics.FromImage(image);
                                PointF p = new PointF(image.Width / 80, image.Height / 60);
                                font = new Font("宋体", image.Width / 50);
                                sb = new SolidBrush(Color.FromArgb(255, 0, 255, 0));
                                   
                                if (Des != "")
                                {
                                    g.DrawString(Des, font, sb, p);
                                }
                                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                                if (!imgEncryptEnable)
                                {
                                    image.Save(local_fullname, ImageCodecInfo.GetImageEncoders().FirstOrDefault(j => j.MimeType == "image/jpeg"), epbottom);
                                }
                                else
                                {
                                    EncryptImage(image, local_fullname, ImageCodecInfo.GetImageEncoders().FirstOrDefault(j => j.MimeType == "image/jpeg"), epbottom);
                                }
                            }
                            else
                            {
                                string[] des = Regex.Split(Des, "\r\n");
                                int x = (int)(CurrentWarp.Cols / 80);
                                int y = (int)(CurrentWarp.Rows / 15);
                                CurrentWarpRGB = new Mat();
                                //CurrentWarpRGB.Dispose();
                                Cv2.CvtColor(CurrentWarp, CurrentWarpRGB, ColorConversionCodes.GRAY2RGB);
                                for (int i = 0; i < des.Length; i++)
                                {
                                    if (des[i] != "")
                                    {
                                        Cv2.PutText(CurrentWarpRGB, des[i], new OpenCvSharp.Point(x, y), HersheyFonts.HersheySimplex, Math.Max(1.0*CurrentWarp.Rows / 750.0, 1), new Scalar(0, 255, 0), (int)(Math.Max(1.0*CurrentWarp.Rows / 500.0, 2)));
                                    }
                                    y += (int)(40.0 * CurrentWarp.Rows / 750.0);
                                }
                                if (!imgEncryptEnable)
                                {
                                    Cv2.ImWrite(local_fullname, CurrentWarpRGB, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                                }
                                else
                                {
                                    //Stopwatch s = new Stopwatch();
                                    //s.Start();
                                    EncryptMat(CurrentWarpRGB, local_fullname, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                                    //s.Stop();
                                    //long ss = s.ElapsedMilliseconds;
                                }
                            }
                        }
                        else
                        {
                            obj.localFileName = obj.localFileName.Replace("#ANA", "NA");
                            obj.ftpFileNameHeader = obj.ftpFileNameHeader.Replace("#ANA", "");
                            obj.ftpFileName = obj.ftpFileName.Replace("#ANA", "NA");
                            obj.localDir = obj.localDir.Replace("#ANA", "");
                            obj.ftpFileNameHeader = Multibar.Replace(obj.ftpFileNameHeader, "~");
                            obj.ftpFileNameHeader = Startbar.Replace(obj.ftpFileNameHeader, "");
                            obj.ftpFileNameHeader = Endbar.Replace(obj.ftpFileNameHeader, "");
                            obj.localDir = obj.localDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                            obj.localDir = MultiSlash.Replace(obj.localDir, "\\");//本地路径
                            local_fullname = obj.localDir + obj.ftpFileName;//本地文件全名含路径
                            ftp_fullname = obj.ftpDir + "\\" + obj.ftpFileNameHeader + delimiter + obj.ftpFileName;
                            if (!imgEncryptEnable)
                            {
                                Cv2.ImWrite(local_fullname, CurrentWarp, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            }
                            else
                            {
                                EncryptMat(CurrentWarpRGB, local_fullname, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            }
                        }
                        if (System.IO.File.Exists(local_fullname))
                        {
                            if (upload)
                            {
                                FileInfo fileInfo = new FileInfo(local_fullname);
                                fileInfo.CopyTo(ftp_fullname, false);
                            }
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                        }
                    }
                    else
                    {
                        flag = false;
                    }
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex) { flag = false; }
            finally
            {
                if (CurrentWarp != null)
                {
                    CurrentWarp.Dispose();
                }
                if (CurrentWarpRGB != null)
                {
                    CurrentWarpRGB.Dispose();
                }
                if (ms != null)
                {
                    ms.Dispose();
                }
                if (lastmat != null)
                {
                    lastmat.Dispose();
                }
                if (BT01mat != null)
                {
                    BT01mat.Dispose();
                }
                if (BT02mat != null)
                {
                    BT02mat.Dispose();
                }
                if (BT03mat != null)
                {
                    BT03mat.Dispose();
                }
                if (BT04mat != null)
                {
                    BT04mat.Dispose();
                }
                if (BT05mat != null)
                {
                    BT05mat.Dispose();
                }
                if (BT06mat != null)
                {
                    BT06mat.Dispose();
                }
                if (image != null)
                {
                    image.Dispose();
                }
                if (g != null)
                {
                    g.Dispose();
                }
                if (font != null)
                {
                    font.Dispose();
                }
                if (sb != null)
                {
                    sb.Dispose();
                }
            }
            return flag;
        }
        private void HandleBottomStitchArray(List<BottomStitchObject> BottomStitchArray)
        {
            string logcontent = "";
            string GroupID = "-1";
            string StationName = "0";
            string ArrayName = "null";
            int[] overlap = ol01;
            long Timeout = 5000;
            long ExtCaptionTimeout = 5000;
            Object BottomStitchLock = BottomStitchArrayLock1;
            ArrayList LogContentArray = LogContentArray1;
            if (BottomStitchArray == BottomStitchArray1)
            {
                BottomStitchLock = BottomStitchArrayLock1;
                ArrayName = "BottomStitchArray1";
            }
            else if (BottomStitchArray == BottomStitchArray2)
            {
                BottomStitchLock = BottomStitchArrayLock2;
                ArrayName = "BottomStitchArray2";
            }
            else if (BottomStitchArray == BottomStitchArray3)
            {
                BottomStitchLock = BottomStitchArrayLock3;
                ArrayName = "BottomStitchArray3";
            }
            else if (BottomStitchArray == BottomStitchArray4)
            {
                BottomStitchLock = BottomStitchArrayLock4;
                ArrayName = "BottomStitchArray4";
            }
            else if (BottomStitchArray == BottomStitchArray5)
            {
                BottomStitchLock = BottomStitchArrayLock5;
                ArrayName = "BottomStitchArray5";
            }
            else
            {
                BottomStitchLock = BottomStitchArrayLock1;
                ArrayName = "null";
            }
            for (int i = 0; i < BottomStitchArray.Count; i++)
            {
                if (BottomStitchArray[i] != null)
                {
                    GroupID = BottomStitchArray[i].N0;
                    StationName = BottomStitchArray[i].StationName;
                    int BottomCamCount = 0;
                    if (BottomStitchArray[i].StationName == "1")
                    {
                        BottomCamCount = BottomCamCount1;
                        LogContentArray = LogContentArray1;
                        overlap = ol01;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut1;
                    }
                    else if (BottomStitchArray[i].StationName == "2")
                    {
                        BottomCamCount = BottomCamCount2;
                        LogContentArray = LogContentArray2;
                        overlap = ol02;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut2;
                    }
                    else if (BottomStitchArray[i].StationName == "3")
                    {
                        BottomCamCount = BottomCamCount3;
                        LogContentArray = LogContentArray3;
                        overlap = ol03;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut3;
                    }
                    else if (BottomStitchArray[i].StationName == "4")
                    {
                        BottomCamCount = BottomCamCount4;
                        LogContentArray = LogContentArray4;
                        overlap = ol04;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut4;
                    }
                    else
                    {
                        BottomCamCount = 0;
                        LogContentArray = LogContentArray1;
                        overlap = ol01;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut1;
                    }
                    DateTime Now = DateTime.Now;
                    if (captionEnable && extCaptionEnable)
                    {
                        Timeout = ExtCaptionTimeout;
                    }
                    else
                    {
                        Timeout = BottomStitchTimeOut;
                    }
                    if (BottomCamCount > 0)
                    {
                        //if (((long)(Now - BottomStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) && !BottomStitchArray[i].handled)
                        if ((((long)(Now - BottomStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) || ((long)(Now - BottomStitchArray[i].TimeStamp).TotalMilliseconds > BottomStitchTimeOut)) && !BottomStitchArray[i].handled && captionEnable && extCaptionEnable)
                        {
                            
                            int index = -1;
                            ExternalCaptionInfo Info = null;
                            lock (ExternalCaptionInfoListLock)
                            {
                                index = FindExternalCaptionInfoListIndex(BottomStitchArray[i].BoxID);
                                if (index > -1)
                                {
                                    Info = ExternalCaptionInfoList[index];
                                }
                            }
                            if (Info != null)
                            {
                                BottomStitchArray[i].extDescription = Info.extDescription;
                            }
                            
                            
                        }
                        if ((((long)(Now - BottomStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) || ((long)(Now - BottomStitchArray[i].TimeStamp).TotalMilliseconds > BottomStitchTimeOut && (!extCaptionEnable || BottomStitchArray[i].extDescription != ""))) && !BottomStitchArray[i].handled)
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + "开始底面拼图！";
                            AddLog(logcontent, LogContentArray);
                            bool HandleOK = HandlePic(BottomStitchArray[i], BottomCamCount, LogContentArray, StationName, GroupID, overlap, BottomStitchTimeOut);
                            sw.Stop();
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + "底面拼图耗时:" + sw.ElapsedMilliseconds + "ms!";
                            AddLog(logcontent, LogContentArray);
                            if (HandleOK)
                            {
                                BottomStitchArray[i].handled = true;
                            }
                        }
                    }
                    else
                    {
                        BottomStitchArray[i].handled = true;
                    }
                    if (((long)(Now - BottomStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) || BottomStitchArray[i].handled)
                    {
                        for (int j = 0; j < BottomStitchArray[i].BT01.Count; j++)
                        {
                            if (BottomStitchArray[i].BT01[j] != null)
                            {
                                FileInfo File = BottomStitchArray[i].BT01[j];
                                try
                                {
                                    //mst.Image[j] = "deleted";
                                    File.Refresh();
                                    if (File.Exists)
                                    {
                                        File.Delete();
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        for (int j = 0; j < BottomStitchArray[i].BT02.Count; j++)
                        {
                            if (BottomStitchArray[i].BT02[j] != null)
                            {
                                FileInfo File = BottomStitchArray[i].BT02[j];
                                try
                                {
                                    //mst.Image[j] = "deleted";
                                    File.Refresh();
                                    if (File.Exists)
                                    {
                                        File.Delete();
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        for (int j = 0; j < BottomStitchArray[i].BT03.Count; j++)
                        {
                            if (BottomStitchArray[i].BT03[j] != null)
                            {
                                FileInfo File = BottomStitchArray[i].BT03[j];
                                try
                                {
                                    //mst.Image[j] = "deleted";
                                    File.Refresh();
                                    if (File.Exists)
                                    {
                                        File.Delete();
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        for (int j = 0; j < BottomStitchArray[i].BT04.Count; j++)
                        {
                            if (BottomStitchArray[i].BT04[j] != null)
                            {
                                FileInfo File = BottomStitchArray[i].BT04[j];
                                try
                                {
                                    //mst.Image[j] = "deleted";
                                    File.Refresh();
                                    if (File.Exists)
                                    {
                                        File.Delete();
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        for (int j = 0; j < BottomStitchArray[i].BT05.Count; j++)
                        {
                            if (BottomStitchArray[i].BT05[j] != null)
                            {
                                FileInfo File = BottomStitchArray[i].BT05[j];
                                try
                                {
                                    //mst.Image[j] = "deleted";
                                    File.Refresh();
                                    if (File.Exists)
                                    {
                                        File.Delete();
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        for (int j = 0; j < BottomStitchArray[i].BT06.Count; j++)
                        {
                            if (BottomStitchArray[i].BT06[j] != null)
                            {
                                FileInfo File = BottomStitchArray[i].BT06[j];
                                try
                                {
                                    //mst.Image[j] = "deleted";
                                    File.Refresh();
                                    if (File.Exists)
                                    {
                                        File.Delete();
                                    }
                                }
                                catch (Exception ex) { }
                            }
                        }
                        string reason = "";
                        if (BottomStitchArray[i].handled)
                        {
                            reason = "handled";
                        }
                        else
                        {
                            reason = "timeout";
                        }
                        lock (BottomStitchLock)
                        {
                            BottomStitchArray.RemoveAt(i);
                            i--;
                        }
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName +  "GroupID: " + GroupID + " has been deleted from " + ArrayName + " for reason: " + reason + "!";
                        AddLog(logcontent, LogContentArray);
                    }
                }
                else
                {
                    lock (BottomStitchLock)
                    {
                        BottomStitchArray.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        private bool HandleBarcodeStitchPic(BarcodeStitchObject obj)
        {
            bool flag = false;
            string logcontent = "";
            Mat fullmat = null;
            Mat fullmatRGB = null;
            Mat UPmat = null;
            Mat Barcodemat = null;
            string[] pos = obj.barcodePos.Split(';');
            int xx1 = 0, yy1 = 0, xx2 = 0, yy2 = 0, xx3 = 0, yy3 = 0, xx4 = 0, yy4 = 0;
            try
            {
                if (pos.Length == 8 && obj.UPImage != null)
                {
                    xx1 = Convert.ToInt32(pos[0]);
                    yy1 = Convert.ToInt32(pos[1]);
                    xx2 = Convert.ToInt32(pos[2]);
                    yy2 = Convert.ToInt32(pos[3]);
                    xx3 = Convert.ToInt32(pos[4]);
                    yy3 = Convert.ToInt32(pos[5]);
                    xx4 = Convert.ToInt32(pos[6]);
                    yy4 = Convert.ToInt32(pos[7]);
                    int minx = Math.Min(Math.Min(Math.Min(xx1, xx2), xx3), xx4);
                    int miny = Math.Min(Math.Min(Math.Min(yy1, yy2), yy3), yy4);
                    int maxx = Math.Max(Math.Max(Math.Max(xx1, xx2), xx3), xx4);
                    int maxy = Math.Max(Math.Max(Math.Max(yy1, yy2), yy3), yy4);
                    UPmat = Cv2.ImRead(obj.UPImage.FullName, ImreadModes.Color);
                    if (obj.BarcodeImage != null)
                    {
                        Barcodemat = Cv2.ImRead(obj.BarcodeImage.FullName, ImreadModes.Color);
                        Cv2.Line(Barcodemat, new OpenCvSharp.Point(xx1, yy1), new OpenCvSharp.Point(xx2, yy2), new Scalar(0, 255, 0), (int)(1 + Barcodemat.Cols / 500));
                        Cv2.Line(Barcodemat, new OpenCvSharp.Point(xx2, yy2), new OpenCvSharp.Point(xx3, yy3), new Scalar(0, 255, 0), (int)(1 + Barcodemat.Cols / 500));
                        Cv2.Line(Barcodemat, new OpenCvSharp.Point(xx3, yy3), new OpenCvSharp.Point(xx4, yy4), new Scalar(0, 255, 0), (int)(1 + Barcodemat.Cols / 500));
                        Cv2.Line(Barcodemat, new OpenCvSharp.Point(xx4, yy4), new OpenCvSharp.Point(xx1, yy1), new Scalar(0, 255, 0), (int)(1 + Barcodemat.Cols / 500));
                    }
                    if (UPmat != null)
                    {
                        if (Barcodemat != null)
                        {
                            //if ((0 <= minx) && (minx <= maxx) && (maxx <= Barcodemat.Cols) && (0 <= miny) && (miny <= maxy) && (maxy <= Barcodemat.Rows))
                            if ((minx <= maxx) && (miny <= maxy) )
                            {
                                int startx = Math.Max(0, minx - 200);
                                int starty = Math.Max(0, miny - 200);
                                int endx = Math.Min(Barcodemat.Cols, maxx + 200);
                                int endy = Math.Min(Barcodemat.Rows, maxy + 200);
                                fullmat = new Mat(Math.Max(endy - starty, UPmat.Rows), UPmat.Cols + endx - startx, MatType.CV_8UC3, new Scalar(0, 0, 0));
                                if (!UPmat.Empty())
                                {
                                    fullmat[0, UPmat.Rows, 0, UPmat.Cols] = UPmat;
                                }
                                if (!Barcodemat.Empty())
                                {
                                    if (endy - starty <= fullmat.Rows)
                                    {
                                        fullmat[0, endy - starty, UPmat.Cols, UPmat.Cols + endx - startx] = Barcodemat[starty, endy, startx, endx];
                                    }
                                }
                            }
                            else
                            {
                                fullmat = UPmat;
                            }
                        }
                        else
                        {
                            fullmat = UPmat;
                        }
                        if (GridLineEnable && obj.parcelHeight > 0 && obj.parcelHeight < MountingHeight)
                        {
                            
                            GridLine(fullmat, UPmat.Cols, fullmat.Rows, obj.parcelHeight, 20);
                           
                        }
                        int dimNo = 1;
                        string[] posstring = obj.pos.Split(';');
                        if (posstring.Length == 13)
                        {
                            if (posstring[0] == "0")
                            {
                                dimNo = 1;
                            }
                            else if (posstring[0] == "1")
                            {
                                dimNo = 2;
                            }
                        }
                        MasterImage master = null;
                        ArrayList LogContentArray = null;
                        ArrayList HttpArray = HTTPArraySta1_1; ;
                        Object HttpArrayLock = HTTPArraySta1_1Lock;
                        Object HttpInfoLock = HTTPInfoSta1_1Lock;
                        calObject calobj = cal1_1;
                        string InductID = InductID1;
                        FileInfo fileInfo = null;//拼接完的图片
                        switch (obj.StationName)
                        {
                            case "1":
                                //StorageRoot = StorageRoot1;
                                LogContentArray = LogContentArray1;
                                InductID = InductID1;
                                if (dimNo == 1)
                                {
                                    calobj = cal1_1;
                                }
                                else
                                {
                                    calobj = cal1_2;
                                }
                                if (Convert.ToInt64(obj.N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta1_1;
                                    HttpArrayLock = HTTPArraySta1_1Lock;
                                    HttpInfoLock = HTTPInfoSta1_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta1_2;
                                    HttpArrayLock = HTTPArraySta1_2Lock;
                                    HttpInfoLock = HTTPInfoSta1_2Lock;
                                }
                                break;
                            case "2":
                                //StorageRoot = StorageRoot2;
                                LogContentArray = LogContentArray2;
                                InductID = InductID2;
                                if (dimNo == 1)
                                {
                                    calobj = cal2_1;
                                }
                                else
                                {
                                    calobj = cal2_2;
                                }
                                if (Convert.ToInt64(obj.N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta2_1;
                                    HttpArrayLock = HTTPArraySta2_1Lock;
                                    HttpInfoLock = HTTPInfoSta2_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta2_2;
                                    HttpArrayLock = HTTPArraySta2_2Lock;
                                    HttpInfoLock = HTTPInfoSta2_2Lock;
                                }
                                break;
                            case "3":
                                //StorageRoot = StorageRoot3;
                                LogContentArray = LogContentArray3;
                                InductID = InductID3;
                                if (dimNo == 1)
                                {
                                    calobj = cal3_1;
                                }
                                else
                                {
                                    calobj = cal3_2;
                                }
                                if (Convert.ToInt64(obj.N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta3_1;
                                    HttpArrayLock = HTTPArraySta3_1Lock;
                                    HttpInfoLock = HTTPInfoSta3_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta3_2;
                                    HttpArrayLock = HTTPArraySta3_2Lock;
                                    HttpInfoLock = HTTPInfoSta3_2Lock;
                                }
                                break;
                            case "4":
                                //StorageRoot = StorageRoot4;
                                LogContentArray = LogContentArray4;
                                InductID = InductID4;
                                if (dimNo == 1)
                                {
                                    calobj = cal4_1;
                                }
                                else
                                {
                                    calobj = cal4_2;
                                }
                                if (Convert.ToInt64(obj.N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta4_1;
                                    HttpArrayLock = HTTPArraySta4_1Lock;
                                    HttpInfoLock = HTTPInfoSta4_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta4_2;
                                    HttpArrayLock = HTTPArraySta4_2Lock;
                                    HttpInfoLock = HTTPInfoSta4_2Lock;
                                }
                                break;
                            default:
                                //StorageRoot = StorageRoot1;
                                LogContentArray = LogContentArray1;
                                InductID = InductID1;
                                if (dimNo == 1)
                                {
                                    calobj = cal1_1;
                                }
                                else
                                {
                                    calobj = cal1_2;
                                }
                                HttpArray = HTTPArraySta1_1;
                                HttpArrayLock = HTTPArraySta1_1Lock;
                                HttpInfoLock = HTTPInfoSta1_1Lock;
                                break;
                        }
                        if (!Directory.Exists(obj.ftpDir))
                        {
                            Directory.CreateDirectory(obj.ftpDir);
                        }
                        if (!Directory.Exists(obj.localDir))
                        {
                            Directory.CreateDirectory(obj.localDir);
                        }
                        
                        if (InductID != "一区" && InductID != "二区")//供包台禁用，直接移动到本地存储区域
                        {
                            if (captionEnable)
                            {
                                string Des = obj.Description;
                                string ext0 = "";
                                string ext1 = "";
                                string ext2 = "";
                                string ext3 = "";
                                string ext4 = "";
                                string ext5 = "";
                                string ext6 = "";
                                string ext7 = "";
                                string ext8 = "";
                                string ext9 = "";
                                if (extCaptionEnable)
                                {
                                    if (obj.extDescription != null)
                                    {
                                        string[] items = obj.extDescription.Split(';');
                                        if (items.Length >= 1)
                                        {
                                            ext0 = items[0];
                                        }
                                        if (items.Length >= 2)
                                        {
                                            ext1 = items[1];
                                        }
                                        if (items.Length >= 3)
                                        {
                                            ext2 = items[2];
                                        }
                                        if (items.Length >= 4)
                                        {
                                            ext3 = items[3];
                                        }
                                        if (items.Length >= 5)
                                        {
                                            ext4 = items[4];
                                        }
                                        if (items.Length >= 6)
                                        {
                                            ext5 = items[5];
                                        }
                                        if (items.Length >= 7)
                                        {
                                            ext6 = items[6];
                                        }
                                        if (items.Length >= 8)
                                        {
                                            ext7 = items[7];
                                        }
                                        if (items.Length >= 9)
                                        {
                                            ext8 = items[8];
                                        }
                                        if (items.Length >= 10)
                                        {
                                            ext9 = items[9];
                                        }
                                    }
                                }
                                Des = Des.Replace("#EXT0", ext0).Replace("#EXT1", ext1).Replace("#EXT2", ext2).Replace("#EXT3", ext3);
                                Des = Des.Replace("#EXT4", ext4).Replace("#EXT5", ext5).Replace("#EXT6", ext6).Replace("#EXT7", ext7);
                                Des = Des.Replace("#EXT8", ext8).Replace("#EXT9", ext9);
                                //fullmatRGB = new Mat();
                                //Cv2.CvtColor(fullmat, fullmatRGB, ColorConversionCodes.GRAY2RGB);
                                if (allowChinese)
                                {
                                    DrawCaptionGDI(fullmat, Des, obj.localFullName, obj.pos, true, calobj);
                                }
                                else
                                {
                                    DrawCaptionOpencv(fullmat, Des, obj.localFullName, obj.pos, true, calobj);
                                }
                                
                                /*else//移动到extcaption文件夹
                                {
                                    Cv2.ImWrite(obj.extCaptionFullName, fullmat, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                                }*/
                            }
                            else
                            {
                                if (!imgEncryptEnable)
                                {
                                    Cv2.ImWrite(obj.localFullName, fullmat, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                                }
                                else
                                {
                                    EncryptMat(fullmat, obj.localFullName, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                                }
                            }
                            if (System.IO.File.Exists(obj.localFullName))
                            {
                                fileInfo = new FileInfo(obj.localFullName);
                                if (upload)
                                {
                                    fileInfo.CopyTo(obj.ftpFullName, false);
                                }
                                if (httpUpload)
                                {
                                    if (fileInfo != null)
                                    {
                                        int MstIndex = -1;
                                        lock (HttpArrayLock)
                                        {
                                            MstIndex = FindMstIndex(obj.N0, HttpArray);
                                            if (MstIndex > -1)
                                            {
                                                master = HttpArray[MstIndex] as MasterImage;
                                            }
                                        }
                                        if (MstIndex < 0)
                                        {
                                            master = new MasterImage(obj.N0, obj.N2, obj.N3, obj.N4, obj.N5, obj.N6, obj.N7, obj.N8, obj.N9, obj.N10, obj.N11, fileInfo.CreationTime, obj.TriggerTimestamp);
                                            lock (HttpArrayLock)
                                            {
                                                HttpArray.Add(master);
                                            }
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " added to httpArray! " + fileInfo.Name + " is the current file!";
                                            AddLog(logcontent, LogContentArray);
                                        }
                                        if (master != null && fileInfo != null && master.Images.Count == 0)
                                        {
                                            lock (HttpInfoLock)
                                            {
                                                master.Images.Add(fileInfo);
                                                master.ImageTime.Add(obj.TriggerTimestamp);
                                                master.BcrName.Add(obj.N1);
                                            }
                                        }
                                    }
                                }
                                flag = true;
                            }
                            /*else if ()//到了extcaption里
                            {
                                flag = true;
                            }*/
                            else
                            {
                                flag = false;
                            }
                        }
                    }
                }
                else
                {
                    flag = false;
                }
            }
            catch (Exception ex) { flag = false; }
            finally
            {
                if (fullmat != null)
                {
                    fullmat.Dispose();
                }
                if (fullmatRGB != null)
                {
                    fullmatRGB.Dispose();
                }
                if (UPmat != null)
                {
                    UPmat.Dispose();
                }
                if (Barcodemat != null)
                {
                    Barcodemat.Dispose();
                }
            }
            return flag;
        }
        private void HandleBarcodeStitchArray(List<BarcodeStitchObject> BarcodeStitchArray)
        {
            string logcontent = "";
            string GroupID = "-1";
            string StationName = "0";
            string ArrayName = "null";
            long Timeout = 5000;
            long ExtCaptionTimeout = 5000;
            Object BarcodeStitchLock = BarcodeStitchArrayLock1;
            ArrayList LogContentArray = LogContentArray1;
            if (BarcodeStitchArray == BarcodeStitchArray1)
            {
                BarcodeStitchLock = BarcodeStitchArrayLock1;
                ArrayName = "BarcodeStitchArrayLock1";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray2)
            {
                BarcodeStitchLock = BarcodeStitchArrayLock2;
                ArrayName = "BarcodeStitchArrayLock2";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray3)
            {
                BarcodeStitchLock = BarcodeStitchArrayLock3;
                ArrayName = "BarcodeStitchArrayLock3";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray4)
            {
                BarcodeStitchLock = BarcodeStitchArrayLock4;
                ArrayName = "BarcodeStitchArrayLock4";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray5)
            {
                BarcodeStitchLock = BarcodeStitchArrayLock5;
                ArrayName = "BarcodeStitchArrayLock5";
            }
            else
            {
                BarcodeStitchLock = BarcodeStitchArrayLock1;
                ArrayName = "null";
            }
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Count of items in BarcodeStitchArray " + ArrayName + ": " + BarcodeStitchArray.Count;
            AddLog(logcontent, LogContentArray);
            for (int i = 0; i < BarcodeStitchArray.Count; i++)
            {
                if (BarcodeStitchArray[i] != null)
                {
                    GroupID = BarcodeStitchArray[i].N0;
                    StationName = BarcodeStitchArray[i].StationName;
                    if (BarcodeStitchArray[i].StationName == "1")
                    {
                        LogContentArray = LogContentArray1;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut1;
                    }
                    else if (BarcodeStitchArray[i].StationName == "2")
                    {
                        LogContentArray = LogContentArray2;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut2;
                    }
                    else if (BarcodeStitchArray[i].StationName == "3")
                    {
                        LogContentArray = LogContentArray3;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut3;
                    }
                    else if (BarcodeStitchArray[i].StationName == "4")
                    {
                        LogContentArray = LogContentArray4;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut4;
                    }
                    else
                    {
                        LogContentArray = LogContentArray1;
                        ExtCaptionTimeout = ExtCaptionBoxTimeOut1;
                    }
                    DateTime Now = DateTime.Now;
                    if (captionEnable && extCaptionEnable)
                    {
                        Timeout = ExtCaptionTimeout;
                    }
                    else
                    {
                        Timeout = BarcodeStitchTimeOut;
                    }
                    if ((((long)(Now - BarcodeStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) || (BarcodeStitchArray[i].UPImage != null && BarcodeStitchArray[i].BarcodeImage != null)) && !BarcodeStitchArray[i].handled && captionEnable && extCaptionEnable)
                    {
                        int index = -1;
                        ExternalCaptionInfo Info = null;
                        lock (ExternalCaptionInfoListLock)
                        {
                            index = FindExternalCaptionInfoListIndex(BarcodeStitchArray[i].BoxID);
                            if (index > -1)
                            {
                                Info = ExternalCaptionInfoList[index];
                            }
                        }
                        if (Info != null)
                        {
                            BarcodeStitchArray[i].extDescription = Info.extDescription;
                        }
                    }
                    if ((((long)(Now - BarcodeStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) || (BarcodeStitchArray[i].UPImage != null && BarcodeStitchArray[i].BarcodeImage != null && (!extCaptionEnable || BarcodeStitchArray[i].extDescription != "")))  && !BarcodeStitchArray[i].handled)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + "开始局部拼图！";
                        AddLog(logcontent, LogContentArray);
                        bool HandleOK = HandleBarcodeStitchPic(BarcodeStitchArray[i]);
                        sw.Stop();
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + "局部拼图耗时:" + sw.ElapsedMilliseconds + "ms!";
                        AddLog(logcontent, LogContentArray);
                        if (HandleOK)
                        {
                            BarcodeStitchArray[i].handled = true;
                        }
                    }
                    if (((long)(Now - BarcodeStitchArray[i].TimeStamp).TotalMilliseconds > Timeout) || BarcodeStitchArray[i].handled)
                    {
                        FileInfo UPImage = BarcodeStitchArray[i].UPImage;
                        FileInfo BarcodeImage = BarcodeStitchArray[i].BarcodeImage;
                        if (UPImage != null)
                        {
                            try
                            {
                                UPImage.Refresh();
                                if (UPImage.Exists)
                                {
                                    UPImage.Delete();
                                }
                            }
                            catch (Exception ex) { }
                        }
                        if (BarcodeImage != null)
                        {
                            try
                            {
                                BarcodeImage.Refresh();
                                if (BarcodeImage.Exists)
                                {
                                    BarcodeImage.Delete();
                                }
                            }
                            catch (Exception ex) { }
                        }
                        string reason = "";
                        if (BarcodeStitchArray[i].handled)
                        {
                            reason = "handled";
                        }
                        else
                        {
                            reason = "timeout";
                        }
                        lock (BarcodeStitchLock)
                        {
                            BarcodeStitchArray.RemoveAt(i);
                            i--;
                        }
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + " has been deleted from " + ArrayName + " for reason: " + reason + "!";
                        AddLog(logcontent, LogContentArray);
                    }
                }
                else
                {
                    lock (BarcodeStitchLock)
                    {
                        BarcodeStitchArray.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void HandleMasterArray(ArrayList MasterArray, ArrayList LogContentArray, Object MasterArrayLock, int CamCount, string StationLogDir)
        {
            string logpath = "";
            string logcontent = "";
            string SystemLogDirectory = systemLog_dir + "\\" + StationLogDir;
            string GroupID = "-1";
            for (int i = 0; i < MasterArray.Count; i++)
            {
                if (MasterArray[i] is MasterImage)
                {
                    MasterImage mst = MasterArray[i] as MasterImage;
                    int maxtime = CamCount;
                    if ((long)(DateTime.Now - mst.TimeStamp).TotalMilliseconds > MstTimeOut || (mst.time >= maxtime))
                    {
                        int time = mst.time;
                        lock (MasterArrayLock)
                        {
                            GroupID = mst.N0;
                            MasterArray.RemoveAt(i);
                            mst = null;
                            i--;
                        }
                        //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " has been deleted from Master for reason: timeout/maxtime reached! time=" + time;
                        AddLog(logcontent, LogContentArray);
                        //SystemLog(logpath, logcontent, MasterArray);
                    }
                }
            }
        }
        public static string GetMD5HashFromFile(string fileName)
        {
            FileStream file = new FileStream(fileName, FileMode.Open);
            try
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();
                file.Dispose();
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                file.Close();
                file.Dispose();
                throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
            }
        }
        private void HttpRegexCheck(ArrayList LogContentArray)
        {
            string logcontent = "";
            string response = "";
            string errMsg = "";
            bool flag1 = (DateTime.Now - LastRegexCompleteTime).TotalHours >= 24;
            bool flag2 = (DateTime.Now - LastRegexCompleteTime).TotalMinutes >= 10;
            if (flag1)
            {
                JDRegexRetry = 5;
                LastJDRegexSuccessful = false;
                RegexCheckGet(out response, out errMsg);
            }
            else if (flag2 && JDRegexRetry > 0 && !LastJDRegexSuccessful)
            {
                RegexCheck(out response, out errMsg);
            }
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + " Regex Check Complete! Remaining Retry = " + JDRegexRetry + "Response: " + response.Substring(0, Math.Min(response.Length, 100)) + " ErrMsg: " + errMsg;
            AddLog(logcontent, LogContentArray);
        }
        private void RegexCheckGet(out string response, out string errMsg)
        {
            LastRegexCompleteTime = DateTime.Now;
            errMsg = "";
            response = "";
            HttpWebRequest httpReq = null;
            Stream postStream = null;
            HttpWebResponse webRespon = null;
            Stream Responsestream = null;
            StreamReader reader = null;
            Uri uri = new Uri(HttpRegexUrl+ "?deviceNo=" + HttpUtility.UrlEncode( regexMachineCode).Replace("+", "%20"));
            try
            {
                httpReq = (HttpWebRequest)WebRequest.Create(uri);
                string urlraw = HttpRegexUrl.Substring(HttpRegexUrl.IndexOf("http://") + 7);
                string host = "";
                if (urlraw.IndexOf(@"/") > -1)
                {
                    host = urlraw.Substring(0, urlraw.IndexOf(@"/"));
                }
                else
                {
                    host = urlraw;
                }
                httpReq.Host = host;
                httpReq.ServicePoint.Expect100Continue = false;
                httpReq.ServicePoint.ConnectionLimit = 512;
                httpReq.AllowAutoRedirect = true;
                httpReq.KeepAlive = false;
                httpReq.Method = "Get";
                httpReq.AllowWriteStreamBuffering = false;
                httpReq.Timeout = 3000;
                //httpReq.ContentType = "application/x-www-form-urlencoded";
                //string content = "machineCode=" + HttpUtility.UrlEncode(regexMachineCode).Replace("+", "%20");
                //byte[] bytes = Encoding.UTF8.GetBytes(content.ToString());
                //httpReq.ContentLength = bytes.Length;
                //postStream = httpReq.GetRequestStream();
                //postStream.Write(bytes, 0, bytes.Length);
                //postStream.Close();
                webRespon = httpReq.GetResponse() as HttpWebResponse;
                string encoding = webRespon.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                Responsestream = webRespon.GetResponseStream();
                reader = new StreamReader(Responsestream, Encoding.GetEncoding(encoding));
                try
                {
                    response = reader.ReadToEnd();
                    int start = response.IndexOf("{");
                    int end = response.LastIndexOf("}");
                    if (start > -1 && end > -1 && end > start)
                    {
                        response = response.Substring(start, end - start + 1);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    errMsg += "Regex Checking Error when reading response streamreader: " + ex.Message + "!!";
                    JDRegexRetry--;
                }
                //reader.Dispose();
                Responsestream.Close();
                if (webRespon.ContentType.StartsWith("application/json") && response.StartsWith("{") && response.EndsWith("}"))
                {
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(response);
                    JArray JregxArray = (JArray)jobj["regxArray"];
                    List<string> regxArray = new List<string>();
                    if (JregxArray is JArray)
                    {
                        regxArray = JregxArray.Select(jo => jo.ToObject<string>()).ToList();
                    }
                    JArray broadJregxArray = (JArray)jobj["broadRegxArray"];
                    List<string> broadRegxArray = new List<string>();
                    if (broadJregxArray is JArray)
                    {
                        broadRegxArray = broadJregxArray.Select(jo => jo.ToObject<string>()).ToList();
                    }
                    response = webRespon.ContentType + "," + jobj.SelectToken("code").Value<int>() + "," + jobj.SelectToken("message").Value<string>() + ", regxArray: " + regxArray + ", broadRegxArray: " + broadRegxArray;
                    if (jobj.SelectToken("valid").Value<bool>() == true)
                    {
                        if (regxArray.Count > 0 || broadRegxArray.Count > 0)
                        {
                            JDRegexList = regxArray;
                            BroadJDRegexList = broadRegxArray;
                            LastJDRegexSuccessful = true;
                        }
                        else
                        {
                            JDRegexRetry--;
                        }
                    }
                    else
                    {
                        JDRegexRetry--;
                    }
                }
                else
                {
                    JDRegexRetry = 0;
                }
            }
            catch (Exception ex)
            {
                errMsg += "Regex Checking Error: " + ex.Message + "!!";
                JDRegexRetry--;
            }
            finally
            {
                if (httpReq != null)
                {
                    httpReq.Abort();
                }
                if (postStream != null)
                {
                    postStream.Dispose();
                }
                if (webRespon != null)
                {
                    webRespon.Dispose();
                }
                if (Responsestream != null)
                {
                    Responsestream.Dispose();
                }
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

        }
        private void RegexCheck(out string response, out string errMsg)
        {
            LastRegexCompleteTime = DateTime.Now;
            errMsg = "";
            response = "";
            HttpWebRequest httpReq = null;
            Stream postStream = null;
            HttpWebResponse webRespon = null;
            Stream Responsestream = null;
            StreamReader reader = null;
            Uri uri = new Uri(HttpRegexUrl);
            try
            {
                httpReq = (HttpWebRequest)WebRequest.Create(uri);
                string urlraw = HttpRegexUrl.Substring(HttpRegexUrl.IndexOf("http://") + 7);
                string host = "";
                if (urlraw.IndexOf(@"/") > -1)
                {
                    host = urlraw.Substring(0, urlraw.IndexOf(@"/"));
                }
                else
                {
                    host = urlraw;
                }
                httpReq.Host = host;
                httpReq.ServicePoint.Expect100Continue = false;
                httpReq.ServicePoint.ConnectionLimit = 512;
                httpReq.AllowAutoRedirect = true;
                httpReq.KeepAlive = false;
                httpReq.Method = "POST";
                httpReq.AllowWriteStreamBuffering = false;
                httpReq.Timeout = 3000;
                httpReq.ContentType = "application/x-www-form-urlencoded";
                string content = "machineCode=" + HttpUtility.UrlEncode(regexMachineCode).Replace("+", "%20");
                byte[] bytes = Encoding.UTF8.GetBytes(content.ToString());
                httpReq.ContentLength = bytes.Length;
                postStream = httpReq.GetRequestStream();
                postStream.Write(bytes, 0, bytes.Length);
                postStream.Close();
                webRespon = httpReq.GetResponse() as HttpWebResponse;
                string encoding = webRespon.ContentEncoding;
                if (encoding == null || encoding.Length < 1)
                {
                    encoding = "UTF-8"; //默认编码  
                }
                Responsestream = webRespon.GetResponseStream();
                reader = new StreamReader(Responsestream, Encoding.GetEncoding(encoding));
                try
                {
                    response = reader.ReadToEnd();
                    int start = response.IndexOf("{");
                    int end = response.LastIndexOf("}");
                    if (start > -1 && end > -1 && end > start)
                    {
                        response = response.Substring(start, end - start + 1);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    errMsg += "Regex Checking Error when reading response streamreader: " + ex.Message + "!!";
                    JDRegexRetry--;
                }
                //reader.Dispose();
                Responsestream.Close();
                if (webRespon.ContentType.StartsWith("application/json") && response.StartsWith("{") && response.EndsWith("}"))
                {
                    JObject jobj = JsonConvert.DeserializeObject<JObject>(response);
                    JArray JregxArray = (JArray)jobj["regxArray"];
                    List<string> regxArray = new List<string>();
                    if (JregxArray is JArray)
                    {
                        regxArray = JregxArray.Select(jo => jo.ToObject<string>()).ToList();
                    }
                    response = webRespon.ContentType + "," + jobj.SelectToken("code").Value<int>() + "," + jobj.SelectToken("message").Value<string>() + ", regxArray: " + regxArray;
                    if (jobj.SelectToken("code").Value<int>() == 200)
                    {
                        if (regxArray.Count > 0)
                        {
                            JDRegexList = regxArray;
                            LastJDRegexSuccessful = true;
                        }
                        else
                        {
                            JDRegexRetry--;
                        }
                    }
                    else if (jobj.SelectToken("code").Value<int>() == 500)
                    {
                        JDRegexRetry--;
                    }
                    else
                    {
                        JDRegexRetry = 0;
                    }
                }
                else
                {
                    JDRegexRetry = 0;
                }
            }
            catch (Exception ex)
            {
                errMsg += "Regex Checking Error: " + ex.Message + "!!";
                JDRegexRetry--;
            }
            finally
            {
                if (httpReq != null)
                {
                    httpReq.Abort();
                }
                if (postStream != null)
                {
                    postStream.Dispose();
                }
                if (webRespon != null)
                {
                    webRespon.Dispose();
                }
                if (Responsestream != null)
                {
                    Responsestream.Dispose();
                }
                if (reader != null)
                {
                    reader.Dispose();
                }
            }

        }
        private bool HttpCheck(MasterImage mst, out string response, out string errMsg, out long uploaded_size)//true:返回正常；false：连接、返回异常
        {
            uploaded_size = 0;
            errMsg = "";
            response = "";
            bool CheckOK = false;
            int count = Math.Min(Math.Min(mst.Images.Count, mst.ImageTime.Count), mst.BcrName.Count);
            if (count > 0)
            {
                HttpWebRequest httpReq = null;
                Stream postStream = null;
                HttpWebResponse webRespon = null;
                Stream Responsestream = null;
                StreamReader reader = null;
                try
                {
                    if (mst.Images[0] is FileInfo)
                    {
                        if (mst.md5 == "")
                        {
                            mst.md5 = GetMD5HashFromFile(mst.Images[0].FullName);
                        }
                    }
                    string Barcode = HttpFileNameFormat;
                    Barcode = Barcode.Replace("#N7", mst.N7).Replace("#N8", mst.N8).Replace("#N9", mst.N9).Replace("#N10", mst.N10).Replace("#N11", mst.N11);
                    Barcode = Barcode.Replace("#N0", mst.N0).Replace("#N1", mst.BcrName[0].Replace("Master","").Replace("master","")).Replace("#N2", mst.N2).Replace("#N3", mst.N3).Replace("#N4", mst.N4).Replace("#N5", mst.N5).Replace("#N6", mst.N6);
                    Barcode = Barcode.Replace("#yyyy", mst.ImageTime[0].ToString("yyyy")).Replace("#MM", mst.ImageTime[0].ToString("MM")).Replace("#dd", mst.ImageTime[0].ToString("dd")).Replace("#HH", mst.ImageTime[0].ToString("HH")).Replace("#mm", mst.ImageTime[0].ToString("mm")).Replace("#ss", mst.ImageTime[0].ToString("ss")).Replace("#fff", mst.ImageTime[0].ToString("fff"));
                    Barcode = Barcode.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    string HttpFileName = Barcode + ".jpg";
                    //string[] BarcodesArray = Regex.Split(Barcode, MultiCodeDelimiter.Replace);
                    List<string> BarcodeList = Barcode.Split(MultiCodeDelimiter, StringSplitOptions.RemoveEmptyEntries).ToList<string>();
                    if (JDRegexList.Count > 0 || BroadJDRegexList.Count > 0)
                    {
                        for (int i = 0; i < BarcodeList.Count; i++)
                        {
                            bool match = false;
                            for (int j = 0; j < JDRegexList.Count; j++)
                            {
                                if (Regex.IsMatch(BarcodeList[i], JDRegexList[j]))
                                {
                                    match = true;
                                    break;
                                }
                            }
                            if (!match)
                            {
                                for (int k = 0; k < BroadJDRegexList.Count; k++)
                                {
                                    if (Regex.IsMatch(BarcodeList[i], BroadJDRegexList[k]))
                                    {
                                        match = true;
                                        break;
                                    }
                                }
                            }
                            if (!match)
                            {
                                BarcodeList.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    //string[] BarcodesArray = Barcode.Split(MultiCodeDelimiter,StringSplitOptions.RemoveEmptyEntries);
                    string[] BarcodesArray = BarcodeList.ToArray();
                    bool barcodeNumFlag = false;
                    if (BarcodesArray.Length == 0)
                    {
                        barcodeNumFlag = false;
                    }
                    else if (BarcodesArray.Length == 1)
                    {
                        if (BarcodesArray[0].ToLower() != "noread" && BarcodesArray[0] != "")
                        {
                            barcodeNumFlag = true;
                            HttpFileName = BarcodesArray[0] + ".jpg";
                        }
                        else
                        {
                            barcodeNumFlag = false;
                        }
                    }
                    else
                    {
                        barcodeNumFlag = false;
                    }
                    if (barcodeNumFlag)
                    {
                        double Length = 0;
                        double Width = 0;
                        double Height = 0;
                        double Weight = 0;
                        string DimWeight = DimAndWeightFormat;
                        DimWeight = DimWeight.Replace("#N7", mst.N7).Replace("#N8", mst.N8).Replace("#N9", mst.N9).Replace("#N10", mst.N10).Replace("#N11", mst.N11);
                        DimWeight = DimWeight.Replace("#N0", mst.N0).Replace("#N1", mst.BcrName[0].Replace("Master", "").Replace("master", "")).Replace("#N2", mst.N2).Replace("#N3", mst.N3).Replace("#N4", mst.N4).Replace("#N5", mst.N5).Replace("#N6", mst.N6);
                        DimWeight = DimWeight.Replace("#None", "0;0;0;0");
                        string[] dimweight = DimWeight.Split(';');
                        string machineCode = machineCodeFormat;
                        machineCode = machineCode.Replace("#N7", mst.N7).Replace("#N8", mst.N8).Replace("#N9", mst.N9).Replace("#N10", mst.N10).Replace("#N11", mst.N11);
                        machineCode = machineCode.Replace("#N0", mst.N0).Replace("#N1", mst.BcrName[0].Replace("Master", "").Replace("master", "")).Replace("#N2", mst.N2).Replace("#N3", mst.N3).Replace("#N4", mst.N4).Replace("#N5", mst.N5).Replace("#N6", mst.N6);
                        machineCode = machineCode.Replace("#None", "");
                        if (dimweight.Length == 4)
                        {
                            try
                            {
                                Length = Convert.ToDouble(dimweight[0]);
                                Width = Convert.ToDouble(dimweight[1]);
                                Height = Convert.ToDouble(dimweight[2]);
                                Weight = Convert.ToDouble(dimweight[3]);
                            }
                            catch (Exception ex)
                            {
                                Length = 0;
                                Width = 0;
                                Height = 0;
                                Weight = 0;
                            }
                        }
                        Uri uri = new Uri(HttpCheckUrl);
                        httpReq = (HttpWebRequest)WebRequest.Create(uri);
                        string urlraw = HttpCheckUrl.Substring(HttpCheckUrl.IndexOf("http://") + 7);
                        string host = "";
                        if (urlraw.IndexOf(@"/") > -1)
                        {
                            host = urlraw.Substring(0, urlraw.IndexOf(@"/"));
                        }
                        else
                        {
                            host = urlraw;
                        }
                        httpReq.Host = host;
                        long length = 0;
                        //string strBoundary = DateTime.Now.Ticks.ToString("x");           
                        httpReq.ServicePoint.Expect100Continue = false;
                        httpReq.ServicePoint.ConnectionLimit = 512;
                        httpReq.AllowAutoRedirect = true;
                        httpReq.KeepAlive = false;
                        httpReq.Method = "POST";
                        httpReq.AllowWriteStreamBuffering = false;
                        //httpReq.SendChunked = true;
                        httpReq.Timeout = 3000;
                        httpReq.ContentType = "application/json";
                        var jsonObject = new { barcode = BarcodesArray, machineCode = machineCode, operateTime = (long)(mst.ImageTime[0] - DateTime.ParseExact("1970-01-01 +00:00", "yyyy-MM-dd K", CultureInfo.InvariantCulture)).TotalMilliseconds, weight = Weight, length = Length, width = Width, height = Height, lengthUnit = "cm", weightUnit = "kg", fileName = HttpFileName, fileSize = mst.Images[0].Length, fileMd5sum = mst.md5 };
                        StringBuilder jsonstr = new StringBuilder(JsonConvert.SerializeObject(jsonObject));
                        byte[] jsonstrbyte = Encoding.UTF8.GetBytes(jsonstr.ToString());
                        httpReq.ContentLength = jsonstrbyte.Length;
                        postStream = httpReq.GetRequestStream();
                        try
                        {
                            postStream.Write(jsonstrbyte, 0, jsonstrbyte.Length);
                            postStream.Close();
                            uploaded_size += jsonstrbyte.Length;
                        }
                        catch (Exception ex)
                        {
                            mst.retry = 0;
                            CheckOK = false;
                            httpReq.Abort();
                            errMsg += "HTTP Checking Error when writing request stream: " + ex.Message + "!!";
                        }
                        postStream.Dispose();
                        webRespon = httpReq.GetResponse() as HttpWebResponse;
                        string encoding = webRespon.ContentEncoding;
                        if (encoding == null || encoding.Length < 1)
                        {
                            encoding = "UTF-8"; //默认编码  
                        }
                        Responsestream = webRespon.GetResponseStream();
                        try
                        {
                            reader = new StreamReader(Responsestream, Encoding.GetEncoding(encoding));
                            try
                            {
                                response = reader.ReadToEnd();
                                int start = response.IndexOf("{");
                                int end = response.LastIndexOf("}");
                                if (start > -1 && end > -1 && end > start)
                                {
                                    response = response.Substring(start, end - start + 1);
                                }
                                reader.Close();
                            }
                            catch (Exception ex)
                            {
                                mst.retry = 0;
                                CheckOK = false;
                                errMsg += "HTTP Checking Error when reading response streamreader: " + ex.Message + "!!";
                            }
                            reader.Dispose();
                            Responsestream.Close();
                        }
                        catch (Exception ex)
                        {
                            mst.retry = 0;
                            CheckOK = false;
                            errMsg += "HTTP Checking Error when opening response streamreader or closing response stream: " + ex.Message + "!!";
                        }
                        Responsestream.Dispose();
                        if (webRespon.ContentType.StartsWith("application/json") && response.StartsWith("{") && response.EndsWith("}"))
                        {
                            JObject jobj = JsonConvert.DeserializeObject<JObject>(response);
                            response = webRespon.ContentType + "," + jobj.SelectToken("code").Value<int>() + "," + jobj.SelectToken("allowUpload").Value<bool>() + "," + jobj.SelectToken("token").Value<string>(); 
                            if (jobj.SelectToken("code").Value<int>() == 200)
                            {
                                if (jobj.SelectToken("allowUpload").Value<bool>())//需要上传
                                {
                                    if (jobj.SelectToken("token").Value<string>() is string)
                                    {
                                        mst.token = jobj.SelectToken("token").Value<string>();
                                        mst.AllowUpload = 1;
                                    }
                                    else
                                    {
                                        mst.AllowUpload = 0;
                                        mst.retry = 0;
                                    }
                                }
                                else//不上传
                                {
                                    mst.AllowUpload = 0;
                                }
                                CheckOK = true;
                            }
                            else if (jobj.SelectToken("code").Value<int>() == 500)
                            {
                                mst.LastTry = DateTime.Now;
                                CheckOK = false;
                                errMsg += "HTTP Checking Error status 500";
                            }
                            else
                            {
                                mst.retry = 0;
                                CheckOK = false;
                                errMsg += "HTTP Checking Error strange status";
                            }
                        }
                        else//不是application/json
                        {
                            mst.retry = 0;
                            //mst.AllowUpload = 1;
                            //mst.token = "123456789abcd";
                            CheckOK = false;
                            errMsg += "HTTP Checking Error not json";
                        }
                        httpReq.Abort();
                    }
                    else
                    {
                        mst.retry = 0;
                        CheckOK = false;
                        errMsg += "HTTP Checking Error Barcode Num Error";
                        mst.AllowUpload = 0;
                    }
                }
                catch (Exception ex)
                {
                    mst.retry = 0;
                    CheckOK = false;
                    errMsg += "HTTP Checking general error: " + ex.Message + "!!";
                }
                finally
                {
                    if (httpReq != null)
                    {
                        httpReq.Abort();
                    }
                    if (postStream != null)
                    {
                        postStream.Dispose();
                    }
                    if (webRespon != null)
                    {
                        webRespon.Dispose();
                    }
                    if (Responsestream != null)
                    {
                        Responsestream.Dispose();
                    }
                    if (reader != null)
                    {
                        reader.Dispose();
                    }
                }
                if (!CheckOK)
                {
                    mst.retry--;
                }
            }
            else//count = 0
            {
                mst.retry = 0;
            }
            /*
            mst.AllowUpload = 1;
            mst.token = "123456789abcd";
            return true;*/
            return CheckOK;
        }
        private bool HttpUploadMasterImage(MasterImage mst, out long uploaded_size, out string errMsg, out string response)
        {
            response = "";
            uploaded_size = 0;
            errMsg = "";
            long file_size = 0;
            bool UploadOK = false;
            int count = Math.Min(Math.Min(mst.Images.Count, mst.ImageTime.Count), mst.BcrName.Count);
            if (count > 0)
            {
                if (mst.Images[0] is FileInfo)
                {
                    HttpWebRequest httpReq = null;
                    Stream postStream = null;
                    FileStream fs = null;
                    HttpWebResponse webRespon = null;
                    Stream Responsestream = null;
                    StreamReader reader = null;
                    try
                    {
                        string Barcode = HttpFileNameFormat;
                        Barcode = Barcode.Replace("#N7", mst.N7).Replace("#N8", mst.N8).Replace("#N9", mst.N9).Replace("#N10", mst.N10).Replace("#N11", mst.N11);
                        Barcode = Barcode.Replace("#N0", mst.N0).Replace("#N1", mst.BcrName[0].Replace("Master", "").Replace("master", "")).Replace("#N2", mst.N2).Replace("#N3", mst.N3).Replace("#N4", mst.N4).Replace("#N5", mst.N5).Replace("#N6", mst.N6);
                        Barcode = Barcode.Replace("#yyyy", mst.ImageTime[0].ToString("yyyy")).Replace("#MM", mst.ImageTime[0].ToString("MM")).Replace("#dd", mst.ImageTime[0].ToString("dd")).Replace("#HH", mst.ImageTime[0].ToString("HH")).Replace("#mm", mst.ImageTime[0].ToString("mm")).Replace("#ss", mst.ImageTime[0].ToString("ss")).Replace("#fff", mst.ImageTime[0].ToString("fff"));
                        Barcode = Barcode.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                        string HttpFileName = Barcode + ".jpg";
                        Uri uri = new Uri(HttpUploadUrl + "?token=" + mst.token);
                        httpReq = (HttpWebRequest)WebRequest.Create(uri);
                        string urlraw = HttpUploadUrl.Substring(HttpUploadUrl.IndexOf("http://") + 7);
                        string host = "";
                        if (urlraw.IndexOf(@"/") > -1)
                        {
                            host = urlraw.Substring(0, urlraw.IndexOf(@"/"));
                        }
                        else
                        {
                            host = urlraw;
                        }
                        httpReq.Host = host;
                        //WebProxy proxy = new WebProxy("202.108.22.5");
                        //httpReq.Proxy = proxy;
                        /*httpReq.ServicePoint.BindIPEndPointDelegate = delegate
                        {
                            return new IPEndPoint(IPAddress.Any, 1000);
                        };*/
                        long length = 0;
                        //string strBoundary = DateTime.Now.Ticks.ToString("x");
                        string strBoundary = "----WebKitFormBoundaryxHF2G5VOVrMnXBlo";
                        StringBuilder FileHeadersb = new StringBuilder(string.Format("Content-Disposition: form-data; name=\"file\"; filename=\"{0}\"\r\nContent-Type: image/jpeg\r\n\r\n", HttpFileName));
                        byte[] FileHeaderByte = Encoding.UTF8.GetBytes(FileHeadersb.ToString());
                        byte[] firstItemBoundaryBytes = Encoding.UTF8.GetBytes("--" + strBoundary + "\r\n");
                        byte[] itemBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + strBoundary + "\r\n");
                        byte[] endBoundaryBytes = Encoding.UTF8.GetBytes("\r\n--" + strBoundary + "--\r\n");
                        httpReq.ServicePoint.Expect100Continue = false;
                        httpReq.ServicePoint.ConnectionLimit = 512;
                        httpReq.AllowAutoRedirect = true;
                        httpReq.KeepAlive = false;
                        httpReq.Method = "POST";
                        httpReq.AllowWriteStreamBuffering = false;
                        //httpReq.SendChunked = true;
                        httpReq.Timeout = 5000;
                        httpReq.ContentType = "multipart/form-data; boundary=" + strBoundary;
                        length += firstItemBoundaryBytes.Length + FileHeaderByte.Length + mst.Images[0].Length;
                        //length += itemBoundaryBytes.Length + OperationTimeHeaderByteList[i].Length;
                        length += endBoundaryBytes.Length;
                        httpReq.ContentLength = length;
                        postStream = httpReq.GetRequestStream();
                        try
                        {
                            postStream.Write(firstItemBoundaryBytes, 0, firstItemBoundaryBytes.Length);
                            uploaded_size += firstItemBoundaryBytes.Length;
                            postStream.Write(FileHeaderByte, 0, FileHeaderByte.Length);
                            uploaded_size += FileHeaderByte.Length;
                            mst.Images[0].Refresh();
                            fs = mst.Images[0].OpenRead();
                            try
                            {
                                int BufferLength = 4096; //4K     
                                byte[] b = new byte[BufferLength];
                                int size;
                                while ((size = fs.Read(b, 0, BufferLength)) > 0)
                                {
                                    postStream.Write(b, 0, size);
                                    uploaded_size += size;
                                    file_size += size;
                                }
                                if (file_size != mst.Images[0].Length)
                                {
                                    errMsg += "file No." + 0 + "length not equal to stream read";
                                    UploadOK = false;
                                }
                            }
                            catch (Exception ex)
                            {
                                errMsg += "HTTP Uploading Error when reading file: " + ex.Message + "!!";
                                UploadOK = false;
                            }
                            fs.Close();
                            fs.Dispose();
                            postStream.Write(endBoundaryBytes, 0, endBoundaryBytes.Length);
                            uploaded_size += endBoundaryBytes.Length;
                            postStream.Close();
                        }
                        catch (Exception ex)
                        {
                            errMsg += "HTTP Uploading Error when writing request stream: " + ex.Message + "!!";
                            UploadOK = false;
                            httpReq.Abort();
                        }
                        postStream.Dispose();
                        webRespon = httpReq.GetResponse() as HttpWebResponse;
                        string encoding = webRespon.ContentEncoding;
                        if (encoding == null || encoding.Length < 1)
                        {
                            encoding = "UTF-8"; //默认编码  
                        }
                        Responsestream = webRespon.GetResponseStream();
                        try
                        {
                            reader = new StreamReader(Responsestream, Encoding.GetEncoding(encoding));
                            try
                            {
                                response = reader.ReadToEnd();
                                reader.Close();
                            }
                            catch (Exception ex)
                            {
                                UploadOK = false;
                                errMsg += "HTTP Uploading Error when reading response streamreader: " + ex.Message + "!!";
                            }
                            reader.Dispose();
                            Responsestream.Close();
                        }
                        catch (Exception ex)
                        {
                            UploadOK = false;
                            errMsg += "HTTP Uploading Error when opening response streamreader or closing response stream: " + ex.Message + "!!";
                        }
                        Responsestream.Dispose();
                        if (webRespon.ContentType.StartsWith("application/json") && response.StartsWith("{") && response.EndsWith("}"))
                        {
                            JObject jobj = JsonConvert.DeserializeObject<JObject>(response);
                            if (jobj.SelectToken("code").Value<int>() == 200)
                            {
                                UploadOK = true;
                                mst.uploaded = true;
                            }
                            else if (jobj.SelectToken("code").Value<int>() == 500)
                            {
                                mst.LastTry = DateTime.Now;
                                UploadOK = false;
                                errMsg += "HTTP Uploading Error 500 Status Code!";
                            }
                            else
                            {
                                mst.retry = 0;
                                UploadOK = false;
                                errMsg += "HTTP Uploading Error strange Status Code!";
                            }
                        }
                        else//不是application/json
                        {
                            mst.retry = 0;
                            UploadOK = false;
                            errMsg += "HTTP Uploading Error not json!";
                        }
                        httpReq.Abort();
                    }
                    catch (Exception ex)
                    {
                        errMsg += "HTTP Uploading general error: " + ex.Message + "!!";
                        UploadOK = false;
                    }
                    finally
                    {
                        if (httpReq != null)
                        {
                            httpReq.Abort();
                        }
                        if (postStream != null)
                        {
                            postStream.Dispose();
                        }
                        if (fs != null)
                        {
                            fs.Dispose();
                        }
                        if (webRespon != null)
                        {
                            webRespon.Dispose();
                        }
                        if (Responsestream != null)
                        {
                            Responsestream.Dispose();
                        }
                        if (reader != null)
                        {
                            reader.Dispose();
                        }
                    }
                }
                if (!UploadOK)
                {
                    mst.retry--;
                }
            }
            else//count = 0
            {
                mst.retry = 0;
            }
            return UploadOK;
        }
        public void HandleHttpArray(ArrayList HttpArray, ArrayList LogContentArray, Object HttpArrayLock, int CamCount, string StationLogDir)
        {
            string Response = "";
            long uploaded_size = 0;
            string errMsg = "";
            string logpath = "";
            string logcontent = "";
            string SystemLogDirectory = systemLog_dir + "\\" + StationLogDir;
            string GroupID = "-1";
            string HttpArrayName = "HTTPArraySta1_1";
            if (HttpArray == HTTPArraySta1_1)
            {
                HttpArrayName = "HTTPArraySta1_1";
            }
            else if (HttpArray == HTTPArraySta1_2)
            {
                HttpArrayName = "HTTPArraySta1_2";
            }
            else if (HttpArray == HTTPArraySta2_1)
            {
                HttpArrayName = "HTTPArraySta2_1";
            }
            else if (HttpArray == HTTPArraySta2_2)
            {
                HttpArrayName = "HTTPArraySta2_2";
            }
            else if (HttpArray == HTTPArraySta3_1)
            {
                HttpArrayName = "HTTPArraySta3_1";
            }
            else if (HttpArray == HTTPArraySta3_2)
            {
                HttpArrayName = "HTTPArraySta3_2";
            }
            else if (HttpArray == HTTPArraySta4_1)
            {
                HttpArrayName = "HTTPArraySta4_1";
            }
            else if (HttpArray == HTTPArraySta4_2)
            {
                HttpArrayName = "HTTPArraySta4_2";
            }
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Count of items in HttpArray " + HttpArrayName + ": " + HttpArray.Count;
            AddLog(logcontent, LogContentArray);
            while (HttpArray.Count > 1000)
            {
                string ID = "";
                if (HttpArray[0] is MasterImage)
                {
                    MasterImage delmst = HttpArray[0] as MasterImage;
                    ID = delmst.N0;
                }
                lock (HttpArrayLock)
                {
                    HttpArray.RemoveAt(0);
                }
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Too many items in HTTPArray so the earliest item has been deleted.GroupID = " + ID;
                AddLog(logcontent, LogContentArray);
            }
            for (int i = 0; i < HttpArray.Count; i++)
            {
                if (HttpArray[i] is MasterImage)
                {
                    MasterImage mst = HttpArray[i] as MasterImage;
                    GroupID = mst.N0;
                    if (((long)(DateTime.Now - mst.TimeStamp).TotalMilliseconds > MstTimeOut || mst.Images.Count >= 1) && mst.retry > 0 && !mst.uploaded && (DateTime.Now - mst.LastTry).TotalSeconds > 600 && mst.AllowUpload != 0)
                    {
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " HTTP checking...";
                        AddLog(logcontent, LogContentArray);
                        if (HttpCheck(mst, out Response, out errMsg, out uploaded_size))//返回正常值
                        {
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " HTTP checking successfully! " + uploaded_size + "bytes transmitted! Response: " + Response;
                            AddLog(logcontent, LogContentArray);
                            if (HttpConnectStatusLabel.InvokeRequired)
                            {
                                if (!HttpConnectStatusLabel.IsDisposed)
                                {
                                    HttpConnectStatusLabel.Invoke(new HttpSetLabelDelegate(HttpSetLabelGreen));
                                }
                            }
                            if (mst.AllowUpload == 1 && mst.token != "")//允许上传
                            {
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " HTTP uploading...";
                                AddLog(logcontent, LogContentArray);
                                if (HttpUploadMasterImage(mst, out uploaded_size, out errMsg, out Response))//上传成功，回复正常
                                {
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " HTTP uploaded successfully! " + uploaded_size + "bytes transmitted! Response: " + Response;
                                    AddLog(logcontent, LogContentArray);
                                    if (HttpConnectStatusLabel.InvokeRequired)
                                    {
                                        if (!HttpConnectStatusLabel.IsDisposed)
                                        {
                                            HttpConnectStatusLabel.Invoke(new HttpSetLabelDelegate(HttpSetLabelGreen));
                                        }
                                    }
                                }
                                else
                                {
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " HTTP uploaded unsuccessfully! Remaining Retry = " + mst.retry + "Response: " + Response.Substring(0, Math.Min(Response.Length, 100)) + " ErrMsg: " + errMsg;
                                    AddLog(logcontent, LogContentArray);
                                    if (HttpConnectStatusLabel.InvokeRequired)
                                    {
                                        if (!HttpConnectStatusLabel.IsDisposed)
                                        {
                                            HttpConnectStatusLabel.Invoke(new HttpSetLabelDelegate(HttpSetLabelGray));
                                        }
                                    }
                                }
                            }
                            else
                            {
                                mst.retry = 0;
                            }
                        }
                        else
                        {
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " HTTP check unsuccessfully! Remaining Retry = " + mst.retry + "Response: " + Response.Substring(0, Math.Min(Response.Length,100)) + " ErrMsg: " + errMsg;
                            int ii = 0;
                            AddLog(logcontent, LogContentArray);
                            if (HttpConnectStatusLabel.InvokeRequired)
                            {
                                if (!HttpConnectStatusLabel.IsDisposed)
                                {
                                    HttpConnectStatusLabel.Invoke(new HttpSetLabelDelegate(HttpSetLabelGray));
                                }
                            }
                        }
                        
                    }
                    if (mst.retry <= 0 || mst.uploaded || mst.AllowUpload == 0)
                    {
                        string reason = "";
                        lock (HttpArrayLock)
                        {
                            HttpArray.Remove(mst);
                            i--;
                        }
                        if (mst.retry <= 0)
                        {
                            reason = "retry<=0";
                        }
                        else if (mst.uploaded)
                        {
                            reason = "image has been uploaded";
                        }
                        else if (mst.AllowUpload == 0)
                        {
                            reason = "not allowed to upload";
                        }
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + GroupID + " has been deleted from HTTPArray for reason: " + reason;
                        AddLog(logcontent, LogContentArray);
                    }
                }
            }
        }
        public static void HandleResidualExtCaptionImage(object Folder)
        {
            try
            {
                List<ExtCaptionBox> ExtCaptionBoxList = ExtCaptionBoxList1;
                Object ExtCaptionBoxListLock = ExtCaptionBoxListLock1;
                string folder = Folder.ToString();
                if (folder == ext_caption_url1)
                {
                    ExtCaptionBoxList = ExtCaptionBoxList1;
                    ExtCaptionBoxListLock = ExtCaptionBoxListLock1;
                }
                else if (folder == ext_caption_url2)
                {
                    ExtCaptionBoxList = ExtCaptionBoxList2;
                    ExtCaptionBoxListLock = ExtCaptionBoxListLock2;
                }
                else if (folder == ext_caption_url3)
                {
                    ExtCaptionBoxList = ExtCaptionBoxList3;
                    ExtCaptionBoxListLock = ExtCaptionBoxListLock3;
                }
                else if (folder == ext_caption_url4)
                {
                    ExtCaptionBoxList = ExtCaptionBoxList4;
                    ExtCaptionBoxListLock = ExtCaptionBoxListLock4;
                }
                else
                {
                    ExtCaptionBoxList = ExtCaptionBoxList1;
                    ExtCaptionBoxListLock = ExtCaptionBoxListLock1;
                }
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                DirectoryInfo directoryinfo = new DirectoryInfo(folder);
                FileInfo[] Files = directoryinfo.GetFiles();
                //int cnt = 0;
                foreach (FileInfo file in Files)
                {
                    if (file.CreationTime < PowerOnTime)
                    {
                        //cnt++;
                        try
                        {
                            string N0 = "0";
                            string N1 = "xx";
                            string N2 = "xx";
                            string N3 = "xx";
                            string N4 = "xx";
                            string N5 = "xx";
                            string N6 = "xx";
                            string N7 = "xx";
                            string N8 = "xx";
                            string N9 = "xx";
                            string N10 = "xx";
                            string N11 = "xx";
                            string N12 = "1";
                            string N13 = "";//外部水印信息
                            string extension = file.Extension;
                            int N0pos;
                            int N1pos;
                            int N2pos;
                            int N3pos;
                            int N4pos;
                            int N5pos;
                            int N6pos;
                            int N7pos;
                            int N8pos;
                            int N9pos;
                            int N10pos;
                            int N11pos;
                            int N12pos;//StationNo.
                            int N13pos;
                            int extpos;
                            ArrayList name = new ArrayList();
                            if ((N0pos = file.Name.IndexOf("_0,")) > -1) { name.Add(N0pos); };
                            if ((N1pos = file.Name.IndexOf("_1,")) > -1) { name.Add(N1pos); };
                            if ((N2pos = file.Name.IndexOf("_2,")) > -1) { name.Add(N2pos); };
                            if ((N3pos = file.Name.IndexOf("_3,")) > -1) { name.Add(N3pos); };
                            if ((N4pos = file.Name.IndexOf("_4,")) > -1) { name.Add(N4pos); };
                            if ((N5pos = file.Name.IndexOf("_5,")) > -1) { name.Add(N5pos); };
                            if ((N6pos = file.Name.IndexOf("_6,")) > -1) { name.Add(N6pos); };
                            if ((N7pos = file.Name.IndexOf("_7,")) > -1) { name.Add(N7pos); };
                            if ((N8pos = file.Name.IndexOf("_8,")) > -1) { name.Add(N8pos); };
                            if ((N9pos = file.Name.IndexOf("_9,")) > -1) { name.Add(N9pos); };
                            if ((N10pos = file.Name.IndexOf("_10,")) > -1) { name.Add(N10pos); };
                            if ((N11pos = file.Name.IndexOf("_11,")) > -1) { name.Add(N11pos); };
                            if ((N12pos = file.Name.IndexOf("_12,")) > -1) { name.Add(N12pos); };
                            if ((N13pos = file.Name.IndexOf("_13,")) > -1) { name.Add(N13pos); };
                            if ((extpos = file.Name.LastIndexOf(file.Extension)) > -1) { name.Add(extpos); };
                            name.Sort();
                            if (name.IndexOf(N0pos) < name.Count - 1 && (name[name.IndexOf(N0pos) + 1] is int) && name.IndexOf(N0pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N0pos) + 1];
                                N0 = file.Name.Substring(N0pos + 3, next - N0pos - 3);
                            }
                            if (name.IndexOf(N1pos) < name.Count - 1 && (name[name.IndexOf(N1pos) + 1] is int) && name.IndexOf(N1pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N1pos) + 1];
                                N1 = file.Name.Substring(N1pos + 3, next - N1pos - 3);
                            }
                            if (name.IndexOf(N2pos) < name.Count - 1 && (name[name.IndexOf(N2pos) + 1] is int) && name.IndexOf(N2pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N2pos) + 1];
                                N2 = file.Name.Substring(N2pos + 3, next - N2pos - 3);
                            }
                            if (name.IndexOf(N3pos) < name.Count - 1 && (name[name.IndexOf(N3pos) + 1] is int) && name.IndexOf(N3pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N3pos) + 1];
                                N3 = file.Name.Substring(N3pos + 3, next - N3pos - 3);
                            }
                            if (name.IndexOf(N4pos) < name.Count - 1 && (name[name.IndexOf(N4pos) + 1] is int) && name.IndexOf(N4pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N4pos) + 1];
                                N4 = file.Name.Substring(N4pos + 3, next - N4pos - 3);
                            }
                            if (name.IndexOf(N5pos) < name.Count - 1 && (name[name.IndexOf(N5pos) + 1] is int) && name.IndexOf(N5pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N5pos) + 1];
                                N5 = file.Name.Substring(N5pos + 3, next - N5pos - 3);
                            }
                            if (name.IndexOf(N6pos) < name.Count - 1 && (name[name.IndexOf(N6pos) + 1] is int) && name.IndexOf(N6pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N6pos) + 1];
                                N6 = file.Name.Substring(N6pos + 3, next - N6pos - 3);
                            }
                            if (name.IndexOf(N7pos) < name.Count - 1 && (name[name.IndexOf(N7pos) + 1] is int) && name.IndexOf(N7pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N7pos) + 1];
                                N7 = file.Name.Substring(N7pos + 3, next - N7pos - 3);
                            }
                            if (name.IndexOf(N8pos) < name.Count - 1 && (name[name.IndexOf(N8pos) + 1] is int) && name.IndexOf(N8pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N8pos) + 1];
                                N8 = file.Name.Substring(N8pos + 3, next - N8pos - 3);
                            }
                            if (name.IndexOf(N9pos) < name.Count - 1 && (name[name.IndexOf(N9pos) + 1] is int) && name.IndexOf(N9pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N9pos) + 1];
                                N9 = file.Name.Substring(N9pos + 3, next - N9pos - 3);
                            }
                            if (name.IndexOf(N10pos) < name.Count - 1 && (name[name.IndexOf(N10pos) + 1] is int) && name.IndexOf(N10pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N10pos) + 1];
                                N10 = file.Name.Substring(N10pos + 4, next - N10pos - 4);
                            }
                            if (name.IndexOf(N11pos) < name.Count - 1 && (name[name.IndexOf(N11pos) + 1] is int) && name.IndexOf(N11pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N11pos) + 1];
                                N11 = file.Name.Substring(N11pos + 4, next - N11pos - 4);
                            }
                            if (name.IndexOf(N12pos) < name.Count - 1 && (name[name.IndexOf(N12pos) + 1] is int) && name.IndexOf(N12pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N12pos) + 1];
                                N12 = file.Name.Substring(N12pos + 4, next - N12pos - 4);
                            }
                            if (name.IndexOf(N13pos) < name.Count - 1 && (name[name.IndexOf(N13pos) + 1] is int) && name.IndexOf(N13pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N13pos) + 1];
                                N13 = file.Name.Substring(N13pos + 4, next - N13pos - 4);
                            }
                            string boxidtext = SorterBoxIDFormat;
                            boxidtext = boxidtext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            boxidtext = boxidtext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            boxidtext = boxidtext.Replace("#None", "");
                            int index = -1;
                            ExtCaptionBox currentBox = null;
                            lock (ExtCaptionBoxListLock)
                            {
                                index = FindExtCaptionBoxListIndex(N0, N12, boxidtext, ExtCaptionBoxList);
                                if (index > -1)
                                {
                                    currentBox = ExtCaptionBoxList[index];
                                }
                                else
                                {
                                    currentBox = new ExtCaptionBox(N0, N12, boxidtext);
                                    ExtCaptionBoxList.Add(currentBox);
                                }
                            }
                            lock (currentBox.BoxLock)
                            {
                                currentBox.Images.Add(file);
                            }
                        }
                        catch (Exception ex) { }
                    }
                }
                //cnt=cnt;
            }
            catch { }
            
        }
        public void HandleExternalCaptionInfoList()
        {
            for (int i = 0; i < ExternalCaptionInfoList.Count; i++)
            {
                DateTime Now = DateTime.Now;
                if ((Now - ExternalCaptionInfoList[i].Timestamp).TotalMilliseconds > ExternalCaptionInfoTimeout || ExternalCaptionInfoList[i].Timestamp == DateTime.MaxValue)
                {
                    lock (ExternalCaptionInfoListLock)
                    {
                        ExternalCaptionInfoList.RemoveAt(i);
                        i--;
                    }
                }
            }
        }
        public void TransferExtCaptionBox(ExtCaptionBox currentBox, ArrayList LogContentArray)
        {
            string logcontent = "";
            string caption_temp_url = caption_url1;
            try
            {
                if (Convert.ToInt64(currentBox.N0) % 5 == 0)
                {
                    caption_temp_url = caption_url1;
                }
                else if (Convert.ToInt64(currentBox.N0) % 5 == 1)
                {
                    caption_temp_url = caption_url2;
                }
                else if (Convert.ToInt64(currentBox.N0) % 5 == 2)
                {
                    caption_temp_url = caption_url3;
                }
                else if (Convert.ToInt64(currentBox.N0) % 5 == 3)
                {
                    caption_temp_url = caption_url4;
                }
                else if (Convert.ToInt64(currentBox.N0) % 5 == 4)
                {
                    caption_temp_url = caption_url5;
                }
                else
                {
                    caption_temp_url = caption_url1;
                }
            }
            catch { }
            for (int i = 0; i < currentBox.Images.Count; i++)
            {
                if (currentBox.Images[i] != null)
                {
                    string captionFileName = currentBox.Images[i].Name;
                    string extension = currentBox.Images[i].Extension;
                    int pos = captionFileName.LastIndexOf(extension);
                    if (pos > 0)
                    {
                        captionFileName = captionFileName.Substring(0, pos) + currentBox.extDescription + extension;
                    }
                    string CaptionFullName = caption_temp_url + "\\" + captionFileName;
                    try
                    {
                        if (!Directory.Exists(caption_temp_url))
                        {
                            Directory.CreateDirectory(caption_temp_url);
                        }
                        if (currentBox.Images[i].Exists && !System.IO.File.Exists(CaptionFullName))
                        {
                            //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Start to transfer ext caption pic: " + captionFileName;
                            //AddLog(logcontent, LogContentArray);
                            currentBox.Images[i].MoveTo(CaptionFullName);
                            //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Successfully transfered ext caption pic:" + captionFileName;
                            //AddLog(logcontent, LogContentArray);
                        }
                    }
                    catch (Exception ex)
                    {
                        //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Error transfering ext caption pic:" + captionFileName;
                        //AddLog(logcontent, LogContentArray);
                    }
                }
            }
        }
        public void HandleExtCaptionBoxList(List<ExtCaptionBox> ExtCaptionBoxList, ArrayList LogContentArray, Object ExtCaptionBoxListLock,long ExtCaptionBoxTimeOut)
        {
            string logcontent = "";
            string ArrayName = "ExtCaptionBoxList1";
            if (ExtCaptionBoxList == ExtCaptionBoxList1)
            {
                ArrayName = "ExtCaptionBoxList1";
            }
            else if (ExtCaptionBoxList == ExtCaptionBoxList2)
            {
                ArrayName = "ExtCaptionBoxList2";
            }
            else if (ExtCaptionBoxList == ExtCaptionBoxList3)
            {
                ArrayName = "ExtCaptionBoxList3";
            }
            else if (ExtCaptionBoxList == ExtCaptionBoxList4)
            {
                ArrayName = "ExtCaptionBoxList4";
            }
            else
            {
                ArrayName = "ExtCaptionBoxList1";
            }
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Count of items in" + ArrayName + " : " + ExtCaptionBoxList.Count;
            AddLog(logcontent, LogContentArray);
            while (ExtCaptionBoxList.Count > 1000)
            {
                string ID = "";
                ExtCaptionBox firstbox = ExtCaptionBoxList[0];
                if (firstbox is ExtCaptionBox)
                {
                    ID = firstbox.BoxID;
                }
                lock (ExtCaptionBoxListLock)
                {
                    ExtCaptionBoxList.RemoveAt(0);
                }
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Too many items in ExtCaptionBoxList so the earliest item has been deleted.BoxID = " + ID;
                AddLog(logcontent, LogContentArray);
            }
            for(int i = 0; i< ExtCaptionBoxList.Count; i++ )
            {
                ExtCaptionBox currentBox = ExtCaptionBoxList[i];
                if (currentBox != null)
                {
                    if (currentBox.Images != null)
                    {
                        DateTime Now = DateTime.Now;
                        //if (((long)(Now - currentBox.CreateTime).TotalMilliseconds > ExtCaptionBoxTimeOut) && !currentBox.handled)
                        if ((((long)(Now - currentBox.CreateTime).TotalMilliseconds > ExtCaptionBoxTimeOut) || ((long)(Now - currentBox.CreateTime).TotalMilliseconds > ExtCaptionBoxImgTimeOut)) && !currentBox.handled)
                        {
                            int index = -1;
                            ExternalCaptionInfo Info = null;
                            lock (ExternalCaptionInfoListLock)
                            {
                                index = FindExternalCaptionInfoListIndex(currentBox.BoxID);
                                if (index > -1)
                                {
                                    Info = ExternalCaptionInfoList[index];
                                }
                            }
                            if (Info != null)
                            {
                                currentBox.extDescription = Info.extDescription;
                            }
                            
                        }
                        if ((((long)(Now - currentBox.CreateTime).TotalMilliseconds > ExtCaptionBoxTimeOut) || ((long)(Now - currentBox.CreateTime).TotalMilliseconds > ExtCaptionBoxImgTimeOut && currentBox.extDescription != "")) && !currentBox.handled)
                        {
                            TransferExtCaptionBox(currentBox, LogContentArray);
                            currentBox.handled = true;
                        }
                        if (((long)(Now - currentBox.CreateTime).TotalMilliseconds > ExtCaptionBoxTimeOut) || currentBox.handled)
                        {
                            for (int j = 0; j < currentBox.Images.Count; j++)
                            {
                                FileInfo File = currentBox.Images[j];
                                if (File != null)
                                {
                                    try
                                    {
                                        File.Refresh();
                                        if (File.Exists && (File.DirectoryName == ext_caption_url1 || File.DirectoryName == ext_caption_url2 || File.DirectoryName == ext_caption_url3 || File.DirectoryName == ext_caption_url4))
                                        {
                                            File.Delete();
                                        }
                                    }
                                    catch { }
                                }
                            }
                            string reason = "";
                            string BoxID = currentBox.BoxID;
                            if (currentBox.handled)
                            {
                                reason = "handled";
                            }
                            else
                            {
                                reason = "timeout";
                            }
                            lock (ExtCaptionBoxListLock)
                            {
                                ExtCaptionBoxList.RemoveAt(i);
                                i--;
                            }
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "BoxID: " + BoxID + " has been deleted from " + ArrayName + " for reason: " + reason + "!";
                            AddLog(logcontent, LogContentArray);
                        }
                    }
                    else
                    {
                        lock (ExtCaptionBoxListLock)
                        {
                            ExtCaptionBoxList.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }
        public void HandleBoxList()
        {
            string logcontent = "";
            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Count of items in BoxList: " + BoxList.Count;
            AddLog(logcontent, LogContentArray1);
            while (BoxList.Count > 1000)
            {
                string ID = "";
                Box firstbox = BoxList[0];
                if (firstbox is Box)
                {
                    ID = firstbox.BoxID;
                }
                lock (BoxListLock)
                {
                    BoxList.RemoveAt(0);
                }
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "Too many items in BoxList so the earliest item has been deleted.BoxID = " + ID;
                AddLog(logcontent, LogContentArray1);
            }
            for (int i = 0; i < BoxList.Count; i++)
            {
                Box currentBox = BoxList[i];
                if (currentBox != null)
                {
                    bool flag1 = Sta1On && (InductID1 == "一区" || InductID1 == "二区");
                    bool flag2 = Sta2On && (InductID2 == "一区" || InductID2 == "二区");
                    bool flag3 = Sta3On && (InductID3 == "一区" || InductID3 == "二区");
                    bool flag4 = Sta4On && (InductID4 == "一区" || InductID4 == "二区");
                    DateTime now = DateTime.Now;
                    if (flag1 || flag2 || flag3 || flag4)//至少有一个站融合启用
                    {

                    }
                    else//融合禁用
                    {
                        bool checkflag = false;
                        bool handleflag = false;
                        bool deleteflag = false;
                        string curBoxID = "000000";
                        lock (currentBox.BoxLock)
                        {
                            checkflag = ((now - currentBox.CreateTime).TotalMilliseconds > BottomOnlyBindingTimeout || ( currentBox.TrayID != 9999 && currentBox.Images[5] != null && currentBox.BottomTimestamp != DateTime.MinValue)) && !currentBox.Uploaded[5];
                        }
                        if (captionEnable && extCaptionEnable && checkflag)
                        {
                            int index = -1;
                            ExternalCaptionInfo Info = null;
                            lock (ExternalCaptionInfoListLock)
                            {
                                index = FindExternalCaptionInfoListIndex(currentBox.BoxID);
                                if (index > -1)
                                {
                                    Info = ExternalCaptionInfoList[index];
                                }
                            }
                            if (Info != null)
                            {
                                lock (currentBox.BoxLock)
                                {
                                    currentBox.extDescription = Info.extDescription;
                                }
                            }
                        }
                        
                        lock (currentBox.BoxLock)
                        {
                            /*handleflag1 = (now - currentBox.CreateTime).TotalSeconds > BottomOnlyBindingTimeout;
                            handleflag2 = ((!extCaptionEnable || currentBox.extDescription != "") && currentBox.TrayID != 9999 && currentBox.Images[5] != null && currentBox.BottomTimestamp != DateTime.MinValue);
                            handdleflag3 = !currentBox.Uploaded[5];
                            handleflag = (handleflag1 || handleflag2) && handdleflag3;*/
                            handleflag = ((now - currentBox.CreateTime).TotalMilliseconds > BottomOnlyBindingTimeout || ((!extCaptionEnable || currentBox.extDescription != "") && currentBox.TrayID != 9999 && currentBox.Images[5] != null && currentBox.BottomTimestamp != DateTime.MinValue)) && !currentBox.Uploaded[5];
                        }
                        if (handleflag)
                        {
                            UploadBottom(currentBox);
                        }
                        lock (currentBox.BoxLock)
                        {
                            deleteflag = (now - currentBox.CreateTime).TotalMilliseconds > BottomOnlyBindingTimeout || currentBox.Uploaded[5];
                            curBoxID = currentBox.BoxID;
                        }
                        if (deleteflag)
                        {
                            string reason = "";
                            lock (BoxListLock)
                            {
                                BoxList.Remove(currentBox);
                                i--;
                            }
                            if (currentBox.Uploaded[5])
                            {
                                reason = "Uploaded";
                            }
                            else
                            {
                                reason = "Timeout";
                            }
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "BoxID: " + curBoxID + " has been deleted from BoxList for reason: " + reason;
                            AddLog(logcontent, LogContentArray1);
                        }
                    }
                }
            }
        }
        internal void UploadBottom(Box currentBox)
        {
            try
            {
                if (currentBox != null)
                {
                    FileInfo file = null;
                    DateTime Timestamp;
                    string N0 = "-1";//index
                    string N1 = "xx";//读码器名称，DN1-5下层SC01，DN6-10下层SC02，DN11-15上层SC01，DN16-20上层SC02
                    string N2 = "xx";
                    string N3 = "xx";
                    string N4 = "xx";
                    string N5 = "xx";
                    string N6 = "xx";
                    string N7 = "xx";
                    string N8 = "xx";
                    string N9 = "xx";
                    string N10 = "xx";
                    string N11 = "xx";
                    string N12 = "xx";
                    lock (currentBox.BoxLock)
                    {
                        N0 = currentBox.BottomN0;
                        N1 = currentBox.BottomN1;
                        file = currentBox.Images[5];
                        Timestamp = currentBox.BottomTimestamp;
                    }
                    N12 = "1";
                    if (file != null)
                    {
                        string Originalname = file.FullName;
                        if (file.Exists && file.DirectoryName == inductor_sorter_temp_url)
                        {
                            string trayid = "0000" + currentBox.TrayID.ToString();
                            trayid = trayid.Substring(trayid.Length - 4, 4);
                            if (SorterTrayIDFormat == "#N2")
                            {
                                N2 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N3")
                            {
                                N3 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N4")
                            {
                                N4 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N5")
                            {
                                N5 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N6")
                            {
                                N6 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N7")
                            {
                                N7 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N8")
                            {
                                N8 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N9")
                            {
                                N9 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N10")
                            {
                                N10 = trayid;
                            }
                            else if (SorterTrayIDFormat == "#N11")
                            {
                                N11 = trayid;
                            }
                            if (SorterBoxIDFormat == "#N2")
                            {
                                N2 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N3")
                            {
                                N3 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N4")
                            {
                                N4 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N5")
                            {
                                N5 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N6")
                            {
                                N6 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N7")
                            {
                                N7 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N8")
                            {
                                N8 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N9")
                            {
                                N9 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N10")
                            {
                                N10 = currentBox.BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N11")
                            {
                                N11 = currentBox.BoxID;
                            }
                            if (SorterBarcodeFormat == "#N2")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N2 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N2 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N3")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N3 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N3 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N4")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N4 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N4 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N5")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N5 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N5 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N6")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N6 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N6 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N7")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N7 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N7 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N8")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N8 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N8 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N9")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N9 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N9 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N10")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N10 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N10 = currentBox.BottomCodes;
                                }
                            }
                            else if (SorterBarcodeFormat == "#N11")
                            {
                                if (currentBox.BottomCodes.ToLower() == "noread")
                                {
                                    N11 = "EEEEEEEEEEEEE";
                                }
                                else
                                {
                                    N11 = currentBox.BottomCodes;
                                }
                            }
                            if (SorterResultFormat == "#N2")
                            {
                                N2 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N3")
                            {
                                N3 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N4")
                            {
                                N4 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N5")
                            {
                                N5 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N6")
                            {
                                N6 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N7")
                            {
                                N7 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N8")
                            {
                                N8 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N9")
                            {
                                N9 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N10")
                            {
                                N10 = currentBox.BottomResult;
                            }
                            else if (SorterResultFormat == "#N11")
                            {
                                N11 = currentBox.BottomResult;
                            }
                            if (SorterTimeStampFormat == "#N2")
                            {
                                N2 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N3")
                            {
                                N3 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N4")
                            {
                                N4 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N5")
                            {
                                N5 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N6")
                            {
                                N6 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N7")
                            {
                                N7 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N8")
                            {
                                N8 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N9")
                            {
                                N9 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N10")
                            {
                                N10 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            else if (SorterTimeStampFormat == "#N11")
                            {
                                N11 = currentBox.BottomTimestamp.ToString("yyyyMMddHHmmssfff");
                            }
                            if (SorterStationNoFormat == "#N2")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N2 = InductorStation1No;
                                }
                                else
                                {
                                    N2 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N3")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N3 = InductorStation1No;
                                }
                                else
                                {
                                    N3 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N4")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N4 = InductorStation1No;
                                }
                                else
                                {
                                    N4 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N5")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N5 = InductorStation1No;
                                }
                                else
                                {
                                    N5 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N6")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N6 = InductorStation1No;
                                }
                                else
                                {
                                    N6 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N7")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N7 = InductorStation1No;
                                }
                                else
                                {
                                    N7 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N8")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N8 = InductorStation1No;
                                }
                                else
                                {
                                    N8 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N9")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N9 = InductorStation1No;
                                }
                                else
                                {
                                    N9 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N10")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N10 = InductorStation1No;
                                }
                                else
                                {
                                    N10 = InductorStation2No;
                                }
                            }
                            else if (SorterStationNoFormat == "#N11")
                            {
                                if (currentBox.InductID == 1)
                                {
                                    N11 = InductorStation1No;
                                }
                                else
                                {
                                    N11 = InductorStation2No;
                                }
                            }
                            if (captionEnable)//移动到水印文件夹
                            {
                                
                                string caption_temp_url = caption_url1;
                                if (Convert.ToInt64(N0) % 5 == 0)
                                {
                                    caption_temp_url = caption_url1;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 1)
                                {
                                    caption_temp_url = caption_url2;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 2)
                                {
                                    caption_temp_url = caption_url3;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 3)
                                {
                                    caption_temp_url = caption_url4;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 4)
                                {
                                    caption_temp_url = caption_url5;
                                }
                                else
                                {
                                    caption_temp_url = caption_url1;
                                }
                                string captionFileName = "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + "_13," + currentBox.extDescription + ".jpg";
                                string captionFullName = caption_temp_url + "\\" + captionFileName;
                                try
                                {
                                    if (!Directory.Exists(caption_temp_url))
                                    {
                                        Directory.CreateDirectory(caption_temp_url);
                                    }
                                }
                                catch (Exception ex) { }
                                if (!System.IO.File.Exists(captionFullName))
                                {
                                    string failreason = "File not existed!";
                                    if (file.Exists)
                                    {
                                        Thread.Sleep(1);
                                        try
                                        {
                                            file.MoveTo(captionFullName);
                                        }
                                        catch (Exception ex)
                                        {
                                            failreason = ex.GetType().ToString() + ": " + ex.Message;
                                        }
                                    }
                                    file.Refresh();
                                    if (file.FullName != Originalname)//判断是否成功传输
                                    {
                                        lock (currentBox.BoxLock)
                                        {
                                            currentBox.Uploaded[5] = true;
                                        }
                                    }
                                    else
                                    {
                                        lock (currentBox.BoxLock)
                                        {
                                            currentBox.Uploaded[5] = false;
                                        }
                                    }
                                }
                                else
                                {
                                    lock (currentBox.BoxLock)
                                    {
                                        currentBox.Uploaded[5] = true;
                                    }
                                }

                            }
                            else//移动到本地文件夹，复制到ftp文件夹
                            {
                                string logcontent = "";
                                string ftpTempDir = ftp_temp_url1;
                                string Year = Timestamp.ToString("yyyy");
                                string Month = Timestamp.ToString("MM");
                                string Day = Timestamp.ToString("dd");
                                string Hour = Timestamp.ToString("HH");
                                string Minute = Timestamp.ToString("mm");
                                string Second = Timestamp.ToString("ss");
                                string Millisecond = Timestamp.ToString("fff");
                                string RootDir = StorageRoot1;
                                string DirName = string.Join(@"\", SubFolders) + "\\";
                                string FullDir = "";
                                string FileName = FileNameFormat + file.Extension;
                                string FtpFileName = FtpFileNameFormat + file.Extension;
                                Regex MultiSlash = new Regex("\\\\{2,}");
                                Regex MultiSpace = new Regex("");
                                Regex Multibar = new Regex("~{2,}");
                                Regex Startbar = new Regex("^~");
                                Regex Endbar = new Regex("~$");
                                if (Convert.ToInt64(N0) % 5 == 0)
                                {
                                    ftpTempDir = ftp_temp_url1;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 1)
                                {
                                    ftpTempDir = ftp_temp_url2;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 2)
                                {
                                    ftpTempDir = ftp_temp_url3;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 3)
                                {
                                    ftpTempDir = ftp_temp_url4;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 4)
                                {
                                    ftpTempDir = ftp_temp_url5;
                                }
                                FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "NA");
                                FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "");
                                DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                FullDir = RootDir + "\\" + DirName;
                                FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                                FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "NA");
                                FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                string filename_url = FullDir + FileName;//本地文件全名含路径
                                string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                                temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "");
                                temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                                temp_filename_header = Startbar.Replace(temp_filename_header, "");
                                temp_filename_header = Endbar.Replace(temp_filename_header, "");

                                string temp_filename_url = ftpTempDir + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                                try
                                {
                                    if (!Directory.Exists(FullDir))
                                    {
                                        Directory.CreateDirectory(FullDir);
                                    }
                                    else { }
                                    if (!Directory.Exists(ftpTempDir))
                                    {
                                        Directory.CreateDirectory(ftpTempDir);
                                    }
                                    if (System.IO.File.Exists(filename_url))
                                    {
                                        System.IO.File.Delete(filename_url);
                                    }
                                }
                                catch (Exception ex) { }
                                //string Originalname = file.FullName;
                                file.Refresh();
                                string failreason = "File not existed!";
                                //Stopwatch sw = new Stopwatch();
                                if (file.Exists)
                                {
                                    Thread.Sleep(1);
                                    try
                                    {
                                        if (!imgEncryptEnable)
                                        {
                                            file.MoveTo(filename_url);
                                        }
                                        else
                                        {
                                            EncryptFile(file, filename_url);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        failreason = ex.GetType().ToString() + ": " + ex.Message;
                                    }
                                }
                                file.Refresh();
                                if (file.FullName != Originalname || (!file.Exists && File.Exists(filename_url)))//判断是否成功传输
                                {
                                    lock (currentBox.BoxLock)
                                    {
                                        currentBox.Uploaded[5] = true;
                                    }
                                    if (upload)
                                    {
                                        try
                                        {
                                            file.CopyTo(temp_filename_url, false);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                }
                                else
                                {
                                    lock (currentBox.BoxLock)
                                    {
                                        currentBox.Uploaded[5] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch { }
        }

        /*
        internal void Bind(Tray currentTray)
        {
            string BoxID = "000000";
            int TrayID = 9999;
            string TotalCodes = "";
            string TotalResult = "";
            FileInfo[] Images = new FileInfo[6] { null, null, null, null, null, null };
            bool[] Uploaded = new bool[6] { false, false, false, false, false, false };
            string N0 = "-1";//index
            string N1 = "xx";//读码器名称，DN1-5下层SC01，DN6-10下层SC02，DN11-15上层SC01，DN16-20上层SC02
            string N2 = "xx";//时间戳
            string N3 = "xx";//结果
            string N4 = "xx";//条码
            string N5 = "xx";
            string N6 = "xx";
            string N7 = "xx";
            string N8 = "xx";
            string N9 = "xx";
            string N10 = "xx";
            string N11 = "xx";
            string N12 = "xx";
            DateTime Timestamp;
            lock (currentTray.TrayInductorLock)
            {
                BoxID = currentTray.BoxID;
                TrayID = currentTray.TrayID;
                Images[0] = currentTray.Images[0];
                Images[1] = currentTray.Images[1];
                Images[2] = currentTray.Images[2];
                Images[3] = currentTray.Images[3];
                Images[4] = currentTray.Images[4];
                Images[5] = currentTray.Images[5];
                Uploaded[0] = currentTray.Uploaded[0];
                Uploaded[1] = currentTray.Uploaded[1];
                Uploaded[2] = currentTray.Uploaded[2];
                Uploaded[3] = currentTray.Uploaded[3];
                Uploaded[4] = currentTray.Uploaded[4];
                Uploaded[5] = currentTray.Uploaded[5];
                if (InductID1 == "一区" || InductID1 == "二区" || InductID2 == "一区" || InductID2 == "二区" || InductID3 == "一区" || InductID3 == "二区" || InductID4 == "一区" || InductID4 == "二区")
                {
                    Timestamp = currentTray.SorterTimestamp;
                }
                else
                {
                    Timestamp = currentTray.BottomTimestamp;
                }
                N0 = currentTray.N0;
                N2 = currentTray.N2;
                N3 = currentTray.N3;
                N4 = currentTray.N4;
                N5 = currentTray.N5;
                N6 = currentTray.N6;
                N7 = currentTray.N7;
                N8 = currentTray.N8;
                N9 = currentTray.N9;
                N10 = currentTray.N10;
                N11 = currentTray.N11;
                N12 = currentTray.N12;
                if (currentTray.BottomResult != "GR" && currentTray.BottomResult != "MR" && currentTray.SorterResult != "GR" && currentTray.SorterResult != "MR")
                {
                    TotalResult = "NR";
                }
                else if ((currentTray.BottomResult != "GR" && currentTray.BottomResult != "MR" && currentTray.SorterResult == "GR") || (currentTray.SorterResult != "GR" && currentTray.SorterResult != "MR" && currentTray.BottomResult == "GR"))
                {
                    TotalResult = "GR";
                }
                else
                {
                    TotalResult = "MR";
                }
                if (TotalResult == "NR")
                {
                    TotalCodes = "NoRead";
                }
                else if (TotalResult == "GR")
                {
                    TotalCodes = currentTray.SorterResult == "GR" ? currentTray.SorterCodes : currentTray.BottomCodes;
                }
                else
                {
                    string sorterresult = currentTray.SorterResult == "GR" || currentTray.SorterResult == "MR" ? currentTray.SorterCodes : "";
                    string bottomresult = currentTray.BottomResult == "GR" || currentTray.BottomResult == "MR" ? currentTray.BottomCodes : "";
                    if (sorterresult == "" && bottomresult == "")
                    {
                        TotalCodes = "NoRead";
                    }
                    else if (sorterresult == "" && bottomresult != "")
                    {
                        TotalCodes = bottomresult;
                    }
                    else if (sorterresult != "" && bottomresult == "")
                    {
                        TotalCodes = sorterresult;
                    }
                    else
                    {
                        TotalCodes = bottomresult + "+" + sorterresult;
                    }
                }
            }
            for (int i = 0; i < Images.Length; i++)
            {
                if (!Uploaded[i])
                {
                    FileInfo file = Images[i];
                    if (file != null)
                    {
                        string Originalname = file.FullName;
                        if (file.Exists && file.DirectoryName == inductor_sorter_temp_url)
                        {
                            
                            int N0pos;
                            int N1pos;
                            int N2pos;
                            int N3pos;
                            int N4pos;
                            int N5pos;
                            int N6pos;
                            int N7pos;
                            int N8pos;
                            int N9pos;
                            int N10pos;
                            int N11pos;
                            int N12pos;//StationNo.
                            int extpos;
                            ArrayList name = new ArrayList();
                            if ((N0pos = file.Name.IndexOf("_0,")) > -1) { name.Add(N0pos); };
                            if ((N1pos = file.Name.IndexOf("_1,")) > -1) { name.Add(N1pos); };
                            if ((N2pos = file.Name.IndexOf("_2,")) > -1) { name.Add(N2pos); };
                            if ((N3pos = file.Name.IndexOf("_3,")) > -1) { name.Add(N3pos); };
                            if ((N4pos = file.Name.IndexOf("_4,")) > -1) { name.Add(N4pos); };
                            if ((N5pos = file.Name.IndexOf("_5,")) > -1) { name.Add(N5pos); };
                            if ((N6pos = file.Name.IndexOf("_6,")) > -1) { name.Add(N6pos); };
                            if ((N7pos = file.Name.IndexOf("_7,")) > -1) { name.Add(N7pos); };
                            if ((N8pos = file.Name.IndexOf("_8,")) > -1) { name.Add(N8pos); };
                            if ((N9pos = file.Name.IndexOf("_9,")) > -1) { name.Add(N9pos); };
                            if ((N10pos = file.Name.IndexOf("_10,")) > -1) { name.Add(N10pos); };
                            if ((N11pos = file.Name.IndexOf("_11,")) > -1) { name.Add(N11pos); };
                            if ((N12pos = file.Name.IndexOf("_12,")) > -1) { name.Add(N12pos); };
                            if ((extpos = file.Name.LastIndexOf(file.Extension)) > -1) { name.Add(extpos); };
                            name.Sort();
                            if (name.IndexOf(N1pos) < name.Count - 1 && (name[name.IndexOf(N1pos) + 1] is int) && name.IndexOf(N1pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N1pos) + 1];
                                N1 = file.Name.Substring(N1pos + 3, next - N1pos - 3);
                            }

                            if (SorterTrayIDFormat == "#N0")
                            {
                                N0 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N1")
                            {
                                N1 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N2")
                            {
                                N2 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N3")
                            {
                                N3 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N4")
                            {
                                N4 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N5")
                            {
                                N5 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N6")
                            {
                                N6 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N7")
                            {
                                N7 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N8")
                            {
                                N8 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N9")
                            {
                                N9 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N10")
                            {
                                N10 = TrayID.ToString();
                            }
                            else if (SorterTrayIDFormat == "#N11")
                            {
                                N11 = TrayID.ToString();
                            }

                            if (SorterBoxIDFormat == "#N0")
                            {
                                N0 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N1")
                            {
                                N1 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N2")
                            {
                                N2 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N3")
                            {
                                N3 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N4")
                            {
                                N4 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N5")
                            {
                                N5 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N6")
                            {
                                N6 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N7")
                            {
                                N7 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N8")
                            {
                                N8 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N9")
                            {
                                N9 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N10")
                            {
                                N10 = BoxID;
                            }
                            else if (SorterBoxIDFormat == "#N11")
                            {
                                N11 = BoxID;
                            }

                            if (SorterBarcodeFormat == "#N0")
                            {
                                N0 = TotalCodes;
                            }
                            else if(SorterBarcodeFormat == "#N1")
                            { 
                                N1 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N2")
                            {
                                N2 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N3")
                            {
                                N3 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N4")
                            {
                                N4 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N5")
                            {
                                N5 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N6")
                            {
                                N6 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N7")
                            {
                                N7 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N8")
                            {
                                N8 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N9")
                            {
                                N9 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N10")
                            {
                                N10 = TotalCodes;
                            }
                            else if (SorterBarcodeFormat == "#N11")
                            {
                                N11 = TotalCodes;
                            }

                            if (SorterResultFormat == "#N0")
                            {
                                N0 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N1")
                            {
                                N1 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N2")
                            {
                                N2 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N3")
                            {
                                N3 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N4")
                            {
                                N4 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N5")
                            {
                                N5 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N6")
                            {
                                N6 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N7")
                            {
                                N7 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N8")
                            {
                                N8 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N9")
                            {
                                N9 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N10")
                            {
                                N10 = TotalResult;
                            }
                            else if (SorterResultFormat == "#N11")
                            {
                                N11 = TotalResult;
                            }
                            if (captionEnable)//移动到水印文件夹
                            {
                                string caption_temp_url = caption_url1;
                                if (Convert.ToInt64(N0) % 5 == 0)
                                {
                                    caption_temp_url = caption_url1;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 1)
                                {
                                    caption_temp_url = caption_url2;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 2)
                                {
                                    caption_temp_url = caption_url3; 
                                }
                                else if (Convert.ToInt64(N0) % 5 == 3)
                                {
                                    caption_temp_url = caption_url4;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 4)
                                {
                                    caption_temp_url = caption_url5;
                                }
                                else
                                {
                                    caption_temp_url = caption_url1; 
                                }
                                string captionFileName = "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + ".jpg";
                                string captionFullName = caption_temp_url + "\\" + captionFileName;
                                try
                                {
                                    if (!Directory.Exists(caption_temp_url))
                                    {
                                        Directory.CreateDirectory(caption_temp_url);
                                    }
                                }
                                catch (Exception ex) { }
                                if (!System.IO.File.Exists(captionFullName))
                                {
                                    string failreason = "File not existed!";
                                    if (file.Exists)
                                    {
                                        Thread.Sleep(1);
                                        try
                                        {
                                            file.MoveTo(captionFullName);
                                        }
                                        catch (Exception ex)
                                        {
                                            failreason = ex.GetType().ToString() + ": " + ex.Message;
                                        }
                                    }
                                    file.Refresh();
                                    if (file.FullName != Originalname)//判断是否成功传输
                                    {
                                        lock (currentTray.TrayInductorLock)
                                        {
                                            currentTray.Uploaded[i] = true;
                                        }
                                    }
                                    else
                                    {
                                        lock (currentTray.TrayInductorLock)
                                        {
                                            currentTray.Uploaded[i] = false;
                                        }
                                    }
                                }
                                else
                                {
                                    lock (currentTray.TrayInductorLock)
                                    {
                                        currentTray.Uploaded[i] = true;
                                    }
                                }
                            }
                            else//移动到本地文件夹，复制到ftp文件夹，若需要上传HTTP创建HTTP对象
                            {
                                MasterImage master = null;
                                
                                ArrayList LogContentArray = null;
                                string logcontent = "";
                                ArrayList HttpArray = HTTPArraySta1_1; ;
                                Object HttpArrayLock = HTTPArraySta1_1Lock;
                                Object HttpInfoLock = HTTPInfoSta1_1Lock;
                                string ftpTempDir = ftp_temp_url1;
                                string Year = Timestamp.ToString("yyyy");
                                string Month = Timestamp.ToString("MM");
                                string Day = Timestamp.ToString("dd");
                                string Hour = Timestamp.ToString("HH");
                                string Minute = Timestamp.ToString("mm");
                                string Second = Timestamp.ToString("ss");
                                string Millisecond = Timestamp.ToString("fff");
                                string RootDir = StorageRoot1;
                                string DirName = string.Join(@"\", SubFolders) + "\\";
                                string FullDir = "";
                                string FileName = FileNameFormat + file.Extension;
                                string FtpFileName = FtpFileNameFormat + file.Extension;
                                Regex MultiSlash = new Regex("\\\\{2,}");
                                Regex MultiSpace = new Regex("");
                                Regex Multibar = new Regex("~{2,}");
                                Regex Startbar = new Regex("^~");
                                Regex Endbar = new Regex("~$");
                                switch (N12)
                                {
                                    case "1":
                                        RootDir = StorageRoot1;
                                        LogContentArray = LogContentArray1;
                                        if (Convert.ToInt64(N0) % 5 == 0)
                                        {
                                            ftpTempDir = ftp_temp_url1;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 1)
                                        {
                                            ftpTempDir = ftp_temp_url2;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 2)
                                        {
                                            ftpTempDir = ftp_temp_url3;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 3)
                                        {
                                            ftpTempDir = ftp_temp_url4;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 4)
                                        {
                                            ftpTempDir = ftp_temp_url5;
                                        }
                                        if (Convert.ToInt64(N0) % 2 == 0)
                                        {
                                            HttpArray = HTTPArraySta1_1;
                                            HttpArrayLock = HTTPArraySta1_1Lock;
                                            HttpInfoLock = HTTPInfoSta1_1Lock;
                                        }
                                        else
                                        {
                                            HttpArray = HTTPArraySta1_2;
                                            HttpArrayLock = HTTPArraySta1_2Lock;
                                            HttpInfoLock = HTTPInfoSta1_2Lock;
                                        }
                                        break;
                                    case "2":
                                        RootDir = StorageRoot2;
                                        LogContentArray = LogContentArray2;
                                        if (Convert.ToInt64(N0) % 5 == 0)
                                        {
                                            ftpTempDir = ftp_temp_url1;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 1)
                                        {
                                            ftpTempDir = ftp_temp_url2;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 2)
                                        {
                                            ftpTempDir = ftp_temp_url3;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 3)
                                        {
                                            ftpTempDir = ftp_temp_url4;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 4)
                                        {
                                            ftpTempDir = ftp_temp_url5;
                                        }
                                        if (Convert.ToInt64(N0) % 2 == 0)
                                        {
                                            HttpArray = HTTPArraySta2_1;
                                            HttpArrayLock = HTTPArraySta2_1Lock;
                                            HttpInfoLock = HTTPInfoSta2_1Lock;
                                        }
                                        else
                                        {
                                            HttpArray = HTTPArraySta2_2;
                                            HttpArrayLock = HTTPArraySta2_2Lock;
                                            HttpInfoLock = HTTPInfoSta2_2Lock;
                                        }
                                        break;
                                    case "3":
                                        RootDir = StorageRoot3;
                                        LogContentArray = LogContentArray3;
                                        if (Convert.ToInt64(N0) % 5 == 0)
                                        {
                                            ftpTempDir = ftp_temp_url1;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 1)
                                        {
                                            ftpTempDir = ftp_temp_url2;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 2)
                                        {
                                            ftpTempDir = ftp_temp_url3;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 3)
                                        {
                                            ftpTempDir = ftp_temp_url4;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 4)
                                        {
                                            ftpTempDir = ftp_temp_url5;
                                        }
                                        if (Convert.ToInt64(N0) % 2 == 0)
                                        {
                                            HttpArray = HTTPArraySta3_1;
                                            HttpArrayLock = HTTPArraySta3_1Lock;
                                            HttpInfoLock = HTTPInfoSta3_1Lock;
                                        }
                                        else
                                        {
                                            HttpArray = HTTPArraySta3_2;
                                            HttpArrayLock = HTTPArraySta3_2Lock;
                                            HttpInfoLock = HTTPInfoSta3_2Lock;
                                        }
                                        break;
                                    case "4":
                                        RootDir = StorageRoot4;
                                        LogContentArray = LogContentArray4;
                                        if (Convert.ToInt64(N0) % 5 == 0)
                                        {
                                            ftpTempDir = ftp_temp_url1;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 1)
                                        {
                                            ftpTempDir = ftp_temp_url2;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 2)
                                        {
                                            ftpTempDir = ftp_temp_url3;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 3)
                                        {
                                            ftpTempDir = ftp_temp_url4;
                                        }
                                        else if (Convert.ToInt64(N0) % 5 == 4)
                                        {
                                            ftpTempDir = ftp_temp_url5;
                                        }
                                        if (Convert.ToInt64(N0) % 2 == 0)
                                        {
                                            HttpArray = HTTPArraySta4_1;
                                            HttpArrayLock = HTTPArraySta4_1Lock;
                                            HttpInfoLock = HTTPInfoSta4_1Lock;
                                        }
                                        else
                                        {
                                            HttpArray = HTTPArraySta4_2;
                                            HttpArrayLock = HTTPArraySta4_2Lock;
                                            HttpInfoLock = HTTPInfoSta4_2Lock;
                                        }
                                        break;
                                    default:
                                        RootDir = StorageRoot1;
                                        LogContentArray = LogContentArray1;
                                        HttpArray = HTTPArraySta1_1;
                                        HttpArrayLock = HTTPArraySta1_1Lock;
                                        HttpInfoLock = HTTPInfoSta1_1Lock;
                                        ftpTempDir = ftp_temp_url1;
                                        break;
                                }
                                FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                FullDir = RootDir + "\\" + DirName;
                                FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                                FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                string filename_url = FullDir + FileName;//本地文件全名含路径
                                string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                                temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                                temp_filename_header = Startbar.Replace(temp_filename_header, "");
                                temp_filename_header = Endbar.Replace(temp_filename_header, "");
                                string temp_filename_url = ftpTempDir + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径                                                                                        
                                try
                                {
                                    if (!Directory.Exists(FullDir))
                                    {
                                        Directory.CreateDirectory(FullDir);
                                    }
                                    else { }
                                    if (!Directory.Exists(ftpTempDir))
                                    {
                                        Directory.CreateDirectory(ftpTempDir);
                                    }
                                    if (System.IO.File.Exists(filename_url))
                                    {
                                        System.IO.File.Delete(filename_url);
                                    }
                                }
                                catch (Exception ex) { }
                                //string Originalname = file.FullName;
                                file.Refresh();
                                string failreason = "File not existed!";
                                //Stopwatch sw = new Stopwatch();
                                if (file.Exists)
                                {
                                    Thread.Sleep(1);
                                    try
                                    {
                                        file.MoveTo(filename_url);
                                    }
                                    catch (Exception ex)
                                    {
                                        failreason = ex.GetType().ToString() + ": " + ex.Message;
                                    }
                                }
                                file.Refresh();
                                if (file.FullName != Originalname)//判断是否成功传输
                                {
                                    lock (currentTray.TrayInductorLock)
                                    {
                                        currentTray.Uploaded[i] = true;
                                    }
                                    if (upload)
                                    {
                                        try
                                        {
                                            file.CopyTo(temp_filename_url, false);
                                        }
                                        catch (Exception ex)
                                        {

                                        }
                                    }
                                    if (httpUpload)
                                    {
                                        if (N1.StartsWith("UP") && file != null)
                                        {
                                            int MstIndex = -1;
                                            lock (HttpArrayLock)
                                            {
                                                MstIndex = FindMstIndex(N0, HttpArray);
                                                if (MstIndex > -1)
                                                {
                                                    master = HttpArray[MstIndex] as MasterImage;
                                                }
                                            }
                                            if (MstIndex < 0)
                                            {
                                                master = new MasterImage(N0, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, Timestamp, Timestamp);
                                                lock (HttpArrayLock)
                                                {
                                                    HttpArray.Add(master);
                                                }
                                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " added to httpArray! " + file.Name + " is the current file!";
                                                AddLog(logcontent, LogContentArray);
                                            }
                                            if (master != null && file != null && master.Images.Count == 0)
                                            {
                                                lock (HttpInfoLock)
                                                {
                                                    master.Images.Add(file);
                                                    master.ImageTime.Add(Timestamp);
                                                    master.BcrName.Add(N1);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    lock (currentTray.TrayInductorLock)
                                    {
                                        currentTray.Uploaded[i] = false;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        */
            /*
            public void HandleTrayArray()
            {
                string logcontent = "";
                for (int i = 0; i < TrayObjects.Length; i++)
                {
                    try
                    {
                        DateTime now = DateTime.Now;
                        Tray currentTray = TrayObjects[i];
                        if (currentTray != null)
                        {
                            ArrayList LogContentArray = LogContentArray1;
                            bool bindingflag = false;
                            lock (currentTray.TrayInductorLock)
                            {
                                switch (currentTray.N12)
                                {
                                    case "1":
                                        LogContentArray = LogContentArray1;
                                        break;
                                    case "2":
                                        LogContentArray = LogContentArray2;
                                        break;
                                    case "3":
                                        LogContentArray = LogContentArray3;
                                        break;
                                    case "4":
                                        LogContentArray = LogContentArray4;
                                        break;
                                    default:
                                        LogContentArray = LogContentArray1;
                                        break;
                                }
                                bindingflag = ((currentTray.BottomTimestamp != DateTime.MinValue && (now - currentTray.BottomTimestamp).TotalSeconds > InductorBindingTimeout) || ((currentTray.BottomResult == "GR" || currentTray.BottomResult == "MR" || currentTray.SorterResult == "GR" || currentTray.SorterResult == "MR") && (now - currentTray.SorterTimestamp).TotalSeconds > 10 && currentTray.SorterTimestamp !=DateTime.MinValue)) 
                                    && currentTray.SorterTimestamp > currentTray.BottomTimestamp 
                                    && currentTray.BottomTimestamp != DateTime.MinValue 
                                    && (!currentTray.Uploaded[0] || !currentTray.Uploaded[1] || !currentTray.Uploaded[2] || !currentTray.Uploaded[3] || !currentTray.Uploaded[4] || !currentTray.Uploaded[5]);
                            }
                            if (bindingflag)
                            {
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "TrayID: " + currentTray.TrayID + " starting binding! GroupID: " + currentTray.N0 + "!";
                                AddLog(logcontent, LogContentArray);
                                try
                                {
                                    Bind(currentTray);
                                }
                                catch (Exception ex) { }
                            }
                            lock (currentTray.TrayInductorLock)
                            {
                                if (((now - currentTray.BottomTimestamp).TotalSeconds > InductorBindingTimeout && currentTray.BottomTimestamp != DateTime.MinValue)
                                || (currentTray.Uploaded[0] && currentTray.Uploaded[1] && currentTray.Uploaded[2] && currentTray.Uploaded[3] && currentTray.Uploaded[4] && currentTray.Uploaded[5])
                                || (currentTray.BottomTimestamp == DateTime.MinValue && (currentTray.Uploaded[0] || currentTray.Uploaded[1] || currentTray.Uploaded[2] || currentTray.Uploaded[3] || currentTray.Uploaded[4] || currentTray.Uploaded[5] || currentTray.SorterTimestamp != DateTime.MinValue)))
                                {
                                    //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "TrayID: " + currentTray.TrayID + " Initializing! Last GroupID: " + currentTray.N0 + "!";
                                    //AddLog(logcontent, LogContentArray);

                                    InitializeTray(currentTray, false);//全部移动成功/到期/异常，重置小车

                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                    //Thread.Sleep(1);
                }
            }
           */
        public void MoveInductorPic(string FtpRoot, ArrayList LogContentArrayInductor)
        {
            try
            {
                string logcontent = "";
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + " MoveInductorPic Timer Restart!";
                AddLog(logcontent, LogContentArrayInductor);
                if (!Directory.Exists(FtpRoot))
                {
                    Directory.CreateDirectory(FtpRoot);
                }
                DirectoryInfo directoryinfo = new DirectoryInfo(FtpRoot);
                FileInfo[] Files = directoryinfo.GetFiles();
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + Files.Length + " files found!";
                AddLog(logcontent, LogContentArrayInductor);
                foreach (FileInfo file in Files)
                {
                    try
                    {
                        if ((long)(DateTime.Now - file.CreationTime).TotalMilliseconds > deleteRawImageInductor)
                        {
                            try
                            {
                                file.Refresh();
                                if (file.Exists)
                                {
                                    file.Delete();
                                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: timeout! File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                    AddLog(logcontent, LogContentArrayInductor);
                                    //SystemLog(logpath, logcontent,MasterArray);
                                }
                            }
                            catch (Exception ex)
                            {
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: timeout! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                AddLog(logcontent, LogContentArrayInductor);
                                //SystemLog(logpath, logcontent, MasterArray);
                            }
                            continue;
                        }
                        if ((file.Extension.ToLower() == ".bmp" || file.Extension.ToLower() == ".jpg") && (long)(DateTime.Now - file.CreationTime).TotalMilliseconds >= HandleWaitTime)
                        {
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " is the current file!" + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            AddLog(logcontent, LogContentArrayInductor);
                            string N0 = "-1";//index
                            string N1 = "xx";//读码器名称，DN1-5下层SC01，DN6-10下层SC02，DN11-15上层SC01，DN16-20上层SC02
                            string N2 = "xx";//时间戳
                            string N3 = "xx";//结果
                            string N4 = "xx";//条码
                            string N5 = "xx";//索引号
                            string N6 = "xx";
                            string N7 = "xx";
                            string N8 = "xx";
                            string N9 = "xx";
                            string N10 = "xx";
                            string N11 = "xx";
                            string extension = file.Extension;
                            int N0pos;
                            int N1pos;
                            int N2pos;
                            int N3pos;
                            int N4pos;
                            int N5pos;
                            int N6pos;
                            int N7pos;
                            int N8pos;
                            int N9pos;
                            int N10pos;
                            int N11pos;
                            int extpos;
                            ArrayList name = new ArrayList();
                            if ((N0pos = file.Name.IndexOf("_0,")) > -1) { name.Add(N0pos); };
                            if ((N1pos = file.Name.IndexOf("_1,")) > -1) { name.Add(N1pos); };
                            if ((N2pos = file.Name.IndexOf("_2,")) > -1) { name.Add(N2pos); };
                            if ((N3pos = file.Name.IndexOf("_3,")) > -1) { name.Add(N3pos); };
                            if ((N4pos = file.Name.IndexOf("_4,")) > -1) { name.Add(N4pos); };
                            if ((N5pos = file.Name.IndexOf("_5,")) > -1) { name.Add(N5pos); };
                            if ((N6pos = file.Name.IndexOf("_6,")) > -1) { name.Add(N6pos); };
                            if ((N7pos = file.Name.IndexOf("_7,")) > -1) { name.Add(N7pos); };
                            if ((N8pos = file.Name.IndexOf("_8,")) > -1) { name.Add(N8pos); };
                            if ((N9pos = file.Name.IndexOf("_9,")) > -1) { name.Add(N9pos); };
                            if ((N10pos = file.Name.IndexOf("_10,")) > -1) { name.Add(N10pos); };
                            if ((N11pos = file.Name.IndexOf("_11,")) > -1) { name.Add(N11pos); };
                            if ((extpos = file.Name.LastIndexOf(file.Extension)) > -1) { name.Add(extpos); };
                            name.Sort();
                            if (name.IndexOf(N0pos) < name.Count - 1 && (name[name.IndexOf(N0pos) + 1] is int) && name.IndexOf(N0pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N0pos) + 1];
                                N0 = file.Name.Substring(N0pos + 3, next - N0pos - 3);
                            }
                            if (name.IndexOf(N1pos) < name.Count - 1 && (name[name.IndexOf(N1pos) + 1] is int) && name.IndexOf(N1pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N1pos) + 1];
                                N1 = file.Name.Substring(N1pos + 3, next - N1pos - 3);
                            }
                            if (name.IndexOf(N2pos) < name.Count - 1 && (name[name.IndexOf(N2pos) + 1] is int) && name.IndexOf(N2pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N2pos) + 1];
                                N2 = file.Name.Substring(N2pos + 3, next - N2pos - 3);
                            }
                            if (name.IndexOf(N3pos) < name.Count - 1 && (name[name.IndexOf(N3pos) + 1] is int) && name.IndexOf(N3pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N3pos) + 1];
                                N3 = file.Name.Substring(N3pos + 3, next - N3pos - 3);
                            }
                            if (name.IndexOf(N4pos) < name.Count - 1 && (name[name.IndexOf(N4pos) + 1] is int) && name.IndexOf(N4pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N4pos) + 1];
                                N4 = file.Name.Substring(N4pos + 3, next - N4pos - 3);
                            }
                            if (name.IndexOf(N5pos) < name.Count - 1 && (name[name.IndexOf(N5pos) + 1] is int) && name.IndexOf(N5pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N5pos) + 1];
                                N5 = file.Name.Substring(N5pos + 3, next - N5pos - 3);
                            }
                            if (name.IndexOf(N6pos) < name.Count - 1 && (name[name.IndexOf(N6pos) + 1] is int) && name.IndexOf(N6pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N6pos) + 1];
                                N6 = file.Name.Substring(N6pos + 3, next - N6pos - 3);
                            }
                            if (name.IndexOf(N7pos) < name.Count - 1 && (name[name.IndexOf(N7pos) + 1] is int) && name.IndexOf(N7pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N7pos) + 1];
                                N7 = file.Name.Substring(N7pos + 3, next - N7pos - 3);
                            }
                            if (name.IndexOf(N8pos) < name.Count - 1 && (name[name.IndexOf(N8pos) + 1] is int) && name.IndexOf(N8pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N8pos) + 1];
                                N8 = file.Name.Substring(N8pos + 3, next - N8pos - 3);
                            }
                            if (name.IndexOf(N9pos) < name.Count - 1 && (name[name.IndexOf(N9pos) + 1] is int) && name.IndexOf(N9pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N9pos) + 1];
                                N9 = file.Name.Substring(N9pos + 3, next - N9pos - 3);
                            }
                            if (name.IndexOf(N10pos) < name.Count - 1 && (name[name.IndexOf(N10pos) + 1] is int) && name.IndexOf(N10pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N10pos) + 1];
                                N10 = file.Name.Substring(N10pos + 4, next - N10pos - 4);
                            }
                            if (name.IndexOf(N11pos) < name.Count - 1 && (name[name.IndexOf(N11pos) + 1] is int) && name.IndexOf(N11pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N11pos) + 1];
                                N11 = file.Name.Substring(N11pos + 4, next - N11pos - 4);
                            }
                            var format = "yyyyMMddHHmmssfff";
                            DateTime filedt = DateTime.ParseExact(N2, format, CultureInfo.InvariantCulture);
                            IndexInfo[] IndexObjects = null;
                            if (N1 == "DN01" || N1 == "DN11")
                            {
                                IndexObjects = IndexObjects11;
                            }
                            else if (N1 == "DN02" || N1 == "DN12")
                            {
                                IndexObjects = IndexObjects12;
                            }
                            else if (N1 == "DN03" || N1 == "DN13")
                            {
                                IndexObjects = IndexObjects13;
                            }
                            else if (N1 == "DN04" || N1 == "DN14")
                            {
                                IndexObjects = IndexObjects14;
                            }
                            else if (N1 == "DN05" || N1 == "DN15")
                            {
                                IndexObjects = IndexObjects15;
                            }
                            else if (N1 == "DN06" || N1 == "DN16")
                            {
                                IndexObjects = IndexObjects21;
                            }
                            else if (N1 == "DN07" || N1 == "DN17")
                            {
                                IndexObjects = IndexObjects22;
                            }
                            else if (N1 == "DN08" || N1 == "DN18")
                            {
                                IndexObjects = IndexObjects23;
                            }
                            else if (N1 == "DN09" || N1 == "DN19")
                            {
                                IndexObjects = IndexObjects24;
                            }
                            else if (N1 == "DN10" || N1 == "DN20")
                            {
                                IndexObjects = IndexObjects25;
                            }
                            else
                            {
                                IndexObjects = null;
                            }
                            if (IndexObjects != null)
                            {
                                int id = Convert.ToInt32(N0) - 1;
                                string boxid = "000000";
                                string Originalname = file.FullName;
                                string filename_url = inductor_sorter_temp_url + "\\" + file.Name;
                                if (id >= 0 && id < IndexObjects.Length)
                                {
                                    //if (Math.Abs((filedt - IndexObjects[id].BottomTimestamp).TotalSeconds) < 5)//minvalue肯定不满足
                                    if (filedt == IndexObjects[id].BottomTimestamp)
                                    {
                                        if (!Directory.Exists(inductor_sorter_temp_url))
                                        {
                                            Directory.CreateDirectory(inductor_sorter_temp_url);
                                        }
                                        file.Refresh();
                                        string failreason = "File not existed!";
                                        //Stopwatch sw = new Stopwatch();
                                        if (file.Exists)
                                        {
                                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to inductor temp position: " + filename_url + "!";
                                            AddLog(logcontent, LogContentArrayInductor);
                                            Thread.Sleep(1);
                                            try
                                            {
                                                file.MoveTo(filename_url);
                                            }
                                            catch (Exception ex)
                                            {
                                                failreason = ex.GetType().ToString() + ": " + ex.Message;
                                            }
                                        }
                                        file.Refresh();
                                        if (file.FullName != Originalname)//判断是否成功传输
                                        {
                                            lock (IndexObjects[id].InductInfoLock)
                                            {
                                                if (IndexObjects[id].BottomImage == null && filedt == IndexObjects[id].BottomTimestamp)
                                                {
                                                    boxid = IndexObjects[id].BoxID;
                                                    IndexObjects[id].BottomImage = file;
                                                }
                                                
                                            }
                                            if (boxid != "000000")
                                            {
                                                int index = -1;
                                                Box box = null;
                                                lock (BoxListLock)
                                                {
                                                    index = FindBoxListIndex(boxid);
                                                    if (index > -1)
                                                    {
                                                        box = BoxList[index];
                                                    }
                                                }
                                                if (box != null)
                                                {
                                                    lock (box.BoxLock)
                                                    {
                                                        if (box.Images[5] == null && box.BottomTimestamp == filedt)
                                                        {
                                                            box.Images[5] = file;
                                                            box.BottomN0 = N0;
                                                            box.BottomN1 = N1;

                                                        }
                                                    }
                                                }
                                            }
                                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to inductor temp position: " + filename_url + "!";
                                            AddLog(logcontent, LogContentArrayInductor);
                                            //SystemLog(logpath, logcontent, MasterArray);
                                        }
                                        else
                                        {
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to inductor temp position: " + filename_url + "!" + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + " Reason: " + failreason;
                                            AddLog(logcontent, LogContentArrayInductor);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            catch (Exception ex) { }
        }
        public void MovePic(string FtpRoot, string StorageRoot, ArrayList MasterArray, Object MasterArrayLock, int CamCount, string StationLogDir, ArrayList LogContentArray, string InductID)
        {
            try
            {
                string logpath = "";
                string logcontent = "";
                string SystemLogDirectory = systemLog_dir + "\\" + StationLogDir;
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + " MovePic Timer Restart!";
                AddLog(logcontent, LogContentArray);
                if (!Directory.Exists(FtpRoot))
                {
                    Directory.CreateDirectory(FtpRoot);
                }
                DirectoryInfo directoryinfo = new DirectoryInfo(FtpRoot);
                FileInfo[] Files = directoryinfo.GetFiles();
                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + Files.Length + " files found!";
                AddLog(logcontent, LogContentArray);
                //SystemLog(logpath, logcontent, MasterArray);
                foreach (FileInfo file in Files)
                {
                    
                    try
                    {
                        if ((long)(DateTime.Now - file.CreationTime).TotalMilliseconds > deleteRawImage)
                        {
                            try
                            {
                                file.Refresh();
                                if (file.Exists)
                                {
                                    file.Delete();
                                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: timeout! File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                    AddLog(logcontent, LogContentArray);
                                    //SystemLog(logpath, logcontent,MasterArray);
                                }
                            }
                            catch(Exception ex)
                            {
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: timeout! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                            }
                            continue;
                        }
                        if ((file.Extension.ToLower() == ".bmp" || file.Extension.ToLower() == ".jpg") && (long)(DateTime.Now - file.CreationTime).TotalMilliseconds >= HandleWaitTime && file.Length > 0)
                        {
                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " is the current file!" + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            AddLog(logcontent, LogContentArray);
                            //SystemLog(logpath, logcontent, MasterArray);
                            MasterImage CurrentMaster = new MasterImage();
                            string N0 = "0";
                            string N1 = "xx";
                            string N2 = "xx";
                            string N3 = "xx";
                            string N4 = "xx";
                            string N5 = "xx";
                            string N6 = "xx";
                            string N7 = "xx";
                            string N8 = "xx";
                            string N9 = "xx";
                            string N10 = "xx";
                            string N11 = "xx";
                            string extension = file.Extension;
                            bool handleflag = false;
                            int N0pos;
                            int N1pos;
                            int N2pos;
                            int N3pos;
                            int N4pos;
                            int N5pos;
                            int N6pos;
                            int N7pos;
                            int N8pos;
                            int N9pos;
                            int N10pos;
                            int N11pos;
                            int extpos;
                            DateTime Timestamp = file.CreationTime;//文件生成时间
                            DateTime TriggerTimestamp = file.CreationTime;//触发时间
                            /*string timestamptext = SorterTimeStampFormat;
                            timestamptext = timestamptext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            timestamptext = timestamptext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            timestamptext = timestamptext.Replace("#None", "");
                            if (timestamptext != "")
                            {
                                try
                                {
                                    TriggerTimestamp = DateTime.ParseExact(timestamptext, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                                }
                                catch (Exception ex)
                                {
                                    TriggerTimestamp = file.CreationTime;
                                }
                            }*/
                            ArrayList name = new ArrayList();
                            if ((N0pos = file.Name.IndexOf("_0,")) > -1) { name.Add(N0pos); };
                            if ((N1pos = file.Name.IndexOf("_1,")) > -1) { name.Add(N1pos); };
                            if ((N2pos = file.Name.IndexOf("_2,")) > -1) { name.Add(N2pos); };
                            if ((N3pos = file.Name.IndexOf("_3,")) > -1) { name.Add(N3pos); };
                            if ((N4pos = file.Name.IndexOf("_4,")) > -1) { name.Add(N4pos); };
                            if ((N5pos = file.Name.IndexOf("_5,")) > -1) { name.Add(N5pos); };
                            if ((N6pos = file.Name.IndexOf("_6,")) > -1) { name.Add(N6pos); };
                            if ((N7pos = file.Name.IndexOf("_7,")) > -1) { name.Add(N7pos); };
                            if ((N8pos = file.Name.IndexOf("_8,")) > -1) { name.Add(N8pos); };
                            if ((N9pos = file.Name.IndexOf("_9,")) > -1) { name.Add(N9pos); };
                            if ((N10pos = file.Name.IndexOf("_10,")) > -1) { name.Add(N10pos); };
                            if ((N11pos = file.Name.IndexOf("_11,")) > -1) { name.Add(N11pos); };
                            if ((extpos = file.Name.LastIndexOf(file.Extension)) > -1) { name.Add(extpos); };
                            name.Sort();
                            if (name.IndexOf(N0pos) < name.Count - 1 && (name[name.IndexOf(N0pos) + 1] is int) && name.IndexOf(N0pos) > -1) 
                            {
                                int next = (int)name[name.IndexOf(N0pos) + 1];
                                N0 = file.Name.Substring(N0pos + 3, next - N0pos - 3);
                            }
                            if (name.IndexOf(N1pos) < name.Count - 1 && (name[name.IndexOf(N1pos) + 1] is int) && name.IndexOf(N1pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N1pos) + 1];
                                N1 = file.Name.Substring(N1pos + 3, next - N1pos - 3);
                            }
                            if (name.IndexOf(N2pos) < name.Count - 1 && (name[name.IndexOf(N2pos) + 1] is int) && name.IndexOf(N2pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N2pos) + 1];
                                N2 = file.Name.Substring(N2pos + 3, next - N2pos - 3);
                            }
                            if (name.IndexOf(N3pos) < name.Count - 1 && (name[name.IndexOf(N3pos) + 1] is int) && name.IndexOf(N3pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N3pos) + 1];
                                N3 = file.Name.Substring(N3pos + 3, next - N3pos - 3);
                            }
                            if (name.IndexOf(N4pos) < name.Count - 1 && (name[name.IndexOf(N4pos) + 1] is int) && name.IndexOf(N4pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N4pos) + 1];
                                N4 = file.Name.Substring(N4pos + 3, next - N4pos - 3);
                            }
                            if (name.IndexOf(N5pos) < name.Count - 1 && (name[name.IndexOf(N5pos) + 1] is int) && name.IndexOf(N5pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N5pos) + 1];
                                N5 = file.Name.Substring(N5pos + 3, next - N5pos - 3);
                            }
                            if (name.IndexOf(N6pos) < name.Count - 1 && (name[name.IndexOf(N6pos) + 1] is int) && name.IndexOf(N6pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N6pos) + 1];
                                N6 = file.Name.Substring(N6pos + 3, next - N6pos - 3);
                            }
                            if (name.IndexOf(N7pos) < name.Count - 1 && (name[name.IndexOf(N7pos) + 1] is int) && name.IndexOf(N7pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N7pos) + 1];
                                N7 = file.Name.Substring(N7pos + 3, next - N7pos - 3);
                            }
                            if (name.IndexOf(N8pos) < name.Count - 1 && (name[name.IndexOf(N8pos) + 1] is int) && name.IndexOf(N8pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N8pos) + 1];
                                N8 = file.Name.Substring(N8pos + 3, next - N8pos - 3);
                            }
                            if (name.IndexOf(N9pos) < name.Count - 1 && (name[name.IndexOf(N9pos) + 1] is int) && name.IndexOf(N9pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N9pos) + 1];
                                N9 = file.Name.Substring(N9pos + 3, next - N9pos - 3);
                            }
                            if (name.IndexOf(N10pos) < name.Count - 1 && (name[name.IndexOf(N10pos) + 1] is int) && name.IndexOf(N10pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N10pos) + 1];
                                N10 = file.Name.Substring(N10pos + 4, next - N10pos - 4);
                            }
                            if (name.IndexOf(N11pos) < name.Count - 1 && (name[name.IndexOf(N11pos) + 1] is int) && name.IndexOf(N11pos) > -1)
                            {
                                int next = (int)name[name.IndexOf(N11pos) + 1];
                                N11 = file.Name.Substring(N11pos + 4, next - N11pos - 4);
                            }
                            string barcodepos = "xx";
                            barcodepos = BarcodePosFormat;
                            barcodepos = barcodepos.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            barcodepos = barcodepos.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            barcodepos = barcodepos.Replace("#None", "xx");
                            /*
                            ***存图的命名规则为_0,N0_1,N1_2,N2_3,N3_4,N4_5,N5_6,N6_7,N7_8,N8_9,N9_10,N10_11,N11
                               _n, 为第n块信息的标识符；Nn为第n块信息
                                其中N0必须是GroupID，N1必须是读码器名称，N2必须为读到码的那台读码器的名称（decodeResult.source）；其余信息可随意分配。
                               最多包含12块信息（可以少于12块信息）
                               建议条码放在最后一块信息中
                            *** 信息不全的缺省值用xx表示
                            示例：
                            主机图片：_0,N0_1,N1_2,N2_3,N3_4,N4_5,N5_6,N6_7,N7_8,N8_9,N9_10,N10_11,N11
                            从机图片：_0,N0_1,N1_2,xx_3,xx_4,xx_5,xx_6,xx_7,xx_8,xx_9,xx_10,xx_11,xx
                            */
                            ArrayList HttpArray = HTTPArraySta1_1;
                            Object HttpInfoLock = HTTPInfoSta1_1Lock;
                            Object HttpArrayLock = HTTPArraySta1_1Lock;
                            if (MasterArray == MasterArraySta1)
                            {
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta1_1;
                                    HttpArrayLock = HTTPArraySta1_1Lock;
                                    HttpInfoLock = HTTPInfoSta1_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta1_2;
                                    HttpArrayLock = HTTPArraySta1_2Lock;
                                    HttpInfoLock = HTTPInfoSta1_2Lock;
                                }
                            }
                            else if (MasterArray == MasterArraySta2)
                            {
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta2_1;
                                    HttpArrayLock = HTTPArraySta2_1Lock;
                                    HttpInfoLock = HTTPInfoSta2_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta2_2;
                                    HttpArrayLock = HTTPArraySta2_2Lock;
                                    HttpInfoLock = HTTPInfoSta2_2Lock;
                                }
                            }
                            else if (MasterArray == MasterArraySta3)
                            {
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta3_1;
                                    HttpArrayLock = HTTPArraySta3_1Lock;
                                    HttpInfoLock = HTTPInfoSta3_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta3_2;
                                    HttpArrayLock = HTTPArraySta3_2Lock;
                                    HttpInfoLock = HTTPInfoSta3_2Lock;
                                }
                            }
                            else if (MasterArray == MasterArraySta4)
                            {
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta4_1;
                                    HttpArrayLock = HTTPArraySta4_1Lock;
                                    HttpInfoLock = HTTPInfoSta4_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta4_2;
                                    HttpArrayLock = HTTPArraySta4_2Lock;
                                    HttpInfoLock = HTTPInfoSta4_2Lock;
                                }
                            }
                            if (IsMaster(N1))//主机图片
                            {
                                
                                string timestamptext = SorterTimeStampFormat;
                                timestamptext = timestamptext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                timestamptext = timestamptext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                timestamptext = timestamptext.Replace("#None", "");
                                if (timestamptext != "" && timestamptext != "xx")
                                {
                                    try
                                    {
                                        TriggerTimestamp = DateTime.ParseExact(timestamptext, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                                    }
                                    catch (Exception ex)
                                    {
                                        TriggerTimestamp = file.CreationTime;
                                    }
                                }
                                ArrayList resultArray = MasterImageFiletoArray(TriggerTimestamp,Timestamp, CamCount, file, MasterArray, HttpArray, MasterArrayLock, HttpArrayLock, N0, N1, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, StationLogDir, LogContentArray, InductID);
                                if (resultArray[0] is MasterImage)
                                {
                                    CurrentMaster = resultArray[0] as MasterImage;
                                }
                                if (resultArray[1] is bool)
                                {
                                    handleflag = (bool)resultArray[1];
                                }
                            }
                            else//从机图片
                            {
                                ArrayList resultArray = SlaveImageFiletoArray(CamCount, file, MasterArray, HttpArray, MasterArrayLock, HttpArrayLock, N0, StationLogDir, LogContentArray, N1);
                                if (resultArray[0] is MasterImage)
                                {
                                    CurrentMaster = resultArray[0] as MasterImage;
                                    N2 = CurrentMaster.N2;
                                    N3 = CurrentMaster.N3;
                                    N4 = CurrentMaster.N4;
                                    N5 = CurrentMaster.N5;
                                    N6 = CurrentMaster.N6;
                                    N7 = CurrentMaster.N7;
                                    N8 = CurrentMaster.N8;
                                    N9 = CurrentMaster.N9;
                                    N10 = CurrentMaster.N10;
                                    N11= CurrentMaster.N11;
                                    TriggerTimestamp = CurrentMaster.TriggerTimeStamp;
                                }
                                if (resultArray[1] is bool)
                                {
                                    handleflag = (bool)resultArray[1];
                                }
                            }
                            if (handleflag)
                            {
                                string ftp_temp_url = ftp_temp_url1;
                                if (Convert.ToInt64(N0) % 5 == 0)
                                {
                                    ftp_temp_url = ftp_temp_url1;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 1)
                                {
                                    ftp_temp_url = ftp_temp_url2;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 2)
                                {
                                    ftp_temp_url = ftp_temp_url3;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 3)
                                {
                                    ftp_temp_url = ftp_temp_url4;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 4)
                                {
                                    ftp_temp_url = ftp_temp_url5;
                                }
                                else
                                {
                                    ftp_temp_url = ftp_temp_url1;
                                }
                                string http_temp_url = http_temp_url1;
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    http_temp_url = http_temp_url1;
                                }
                                else if (Convert.ToInt64(N0) % 2 == 1)
                                {
                                    http_temp_url = http_temp_url2;
                                }
                                else
                                {
                                    http_temp_url = http_temp_url1;
                                }
                                string caption_temp_url = caption_url1;
                                string stitch_temp_url = stitch_url1;
                                string barcode_stitch_temp_url = barcode_stitch_url1;
                                List<BottomStitchObject> BottomStitchArray = BottomStitchArray1;
                                Object BottomStitchArrayLock = BottomStitchArrayLock1;
                                List<BarcodeStitchObject> BarcodeStitchArray = BarcodeStitchArray1;
                                Object BarcodeStitchArrayLock = BarcodeStitchArrayLock1;
                                if (Convert.ToInt64(N0) % 5 == 0)
                                {
                                    caption_temp_url = caption_url1;
                                    stitch_temp_url = stitch_url1;
                                    barcode_stitch_temp_url = barcode_stitch_url1;
                                    BottomStitchArray = BottomStitchArray1;
                                    BottomStitchArrayLock = BottomStitchArrayLock1;
                                    BarcodeStitchArray = BarcodeStitchArray1;
                                    BarcodeStitchArrayLock = BarcodeStitchArrayLock1;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 1)
                                {
                                    caption_temp_url = caption_url2;
                                    stitch_temp_url = stitch_url2;
                                    barcode_stitch_temp_url = barcode_stitch_url2;
                                    BottomStitchArray = BottomStitchArray2;
                                    BottomStitchArrayLock = BottomStitchArrayLock2;
                                    BarcodeStitchArray = BarcodeStitchArray2;
                                    BarcodeStitchArrayLock = BarcodeStitchArrayLock2;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 2)
                                {
                                    caption_temp_url = caption_url3;
                                    stitch_temp_url = stitch_url3;
                                    barcode_stitch_temp_url = barcode_stitch_url3;
                                    BottomStitchArray = BottomStitchArray3;
                                    BottomStitchArrayLock = BottomStitchArrayLock3;
                                    BarcodeStitchArray = BarcodeStitchArray3;
                                    BarcodeStitchArrayLock = BarcodeStitchArrayLock3;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 3)
                                {
                                    caption_temp_url = caption_url4;
                                    stitch_temp_url = stitch_url4;
                                    barcode_stitch_temp_url = barcode_stitch_url4;
                                    BottomStitchArray = BottomStitchArray4;
                                    BottomStitchArrayLock = BottomStitchArrayLock4;
                                    BarcodeStitchArray = BarcodeStitchArray4;
                                    BarcodeStitchArrayLock = BarcodeStitchArrayLock4;
                                }
                                else if (Convert.ToInt64(N0) % 5 == 4)
                                {
                                    caption_temp_url = caption_url5;
                                    stitch_temp_url = stitch_url5;
                                    barcode_stitch_temp_url = barcode_stitch_url5;
                                    BottomStitchArray = BottomStitchArray5;
                                    BottomStitchArrayLock = BottomStitchArrayLock5;
                                    BarcodeStitchArray = BarcodeStitchArray5;
                                    BarcodeStitchArrayLock = BarcodeStitchArrayLock5;
                                }
                                else
                                {
                                    caption_temp_url = caption_url1;
                                    stitch_temp_url = stitch_url1;
                                    barcode_stitch_temp_url = barcode_stitch_url1;
                                    BottomStitchArray = BottomStitchArray1;
                                    BottomStitchArrayLock = BottomStitchArrayLock1;
                                    BarcodeStitchArray = BarcodeStitchArray1;
                                    BarcodeStitchArrayLock = BarcodeStitchArrayLock1;
                                }
                                int handleresult = HandleImage(file, delimiter, StorageRoot, extension, ftp_temp_url, http_temp_url, caption_temp_url, stitch_temp_url, BottomStitchArray, BottomStitchArrayLock, BarcodeStitchArray, BarcodeStitchArrayLock, barcode_stitch_temp_url, TriggerTimestamp, SubFolders, FileNameFormat, FtpFileNameFormat, HttpFileNameFormat, N0, N1, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, StationLogDir, MasterArray, LogContentArray, CurrentMaster, InductID, barcodepos);
                                if (handleresult == 1)
                                {
                                    if (N1.StartsWith("UP"))
                                    {
                                        lock (HttpInfoLock)
                                        {
                                            CurrentMaster.Images.Add(file);
                                            CurrentMaster.ImageTime.Add(TriggerTimestamp);
                                            CurrentMaster.BcrName.Add(N1);
                                        }
                                    }
                                    else if (N1 == N2)
                                    {
                                        CurrentMaster.BarcodeImagePresent = true;
                                    }
                                    CurrentMaster.time++;
                                }
                                else if (handleresult == 2)
                                {
                                    if (N1.StartsWith("UP"))
                                    {
                                        CurrentMaster.time++;
                                    }
                                    else if(N1 == N2)
                                    {
                                        CurrentMaster.BarcodeImagePresent = true;
                                        CurrentMaster.time++;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex) { }
                    
                }
                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + " Files clear!";
                AddLog(logcontent, LogContentArray);
                //SystemLog(logpath, logcontent, MasterArray);
            }
            catch (Exception ex) { }
        }
        private int HandleImage(FileInfo file, string delimiter, string StorageRoot, string extension, string ftp_temp_url, string http_temp_url, string caption_temp_url, string stitch_temp_url, List<BottomStitchObject> BottomStitchArray, Object BottomStitchArrayLock, List<BarcodeStitchObject> BarcodeStitchArray, Object BarcodeStitchArrayLock, string barcode_stitch_url, DateTime Timestamp, string[] SubFolders, string FileNameFormat, string FtpFileNameFormat, string HttpFileNameFormat, string N0, string N1, string N2, string N3, string N4, string N5, string N6, string N7, string N8, string N9, string N10, string N11,string StationLogDir, ArrayList MasterArray, ArrayList LogContentArray, MasterImage currentmaster, string InductID,string barcodepos)
        {
            //string logpath = "";
            List<ExtCaptionBox> ExtCaptionBoxList = ExtCaptionBoxList1;
            Object ExtCaptionBoxListLock = ExtCaptionBoxListLock1;
            bool TopOnlyOnRead = false;
            string logcontent = "";
            //string SystemLogDirectory = systemLog_dir + "\\" + StationLogDir;
            int HandleOK = 0;//0:失败;1:局部图片拼接禁用时存图相机成功、局部图片拼接启用时非顶面存图相机成功;2:局部图片拼接启用时读码局部图片成功或顶面存图相机成功
            string Year = Timestamp.ToString("yyyy");
            string Month = Timestamp.ToString("MM");
            string Day = Timestamp.ToString("dd");
            string Hour = Timestamp.ToString("HH");
            string Minute = Timestamp.ToString("mm");
            string Second = Timestamp.ToString("ss");
            string Millisecond = Timestamp.ToString("fff");
            string RootDir = StorageRoot;
            string DirName = string.Join(@"\", SubFolders)+"\\";
            string extCaptionDir = ext_caption_url1;
            string ftpTempDir = ftp_temp_url;
            string FullDir = "";
            string FileName = FileNameFormat + extension;
            string FtpFileName = FtpFileNameFormat + extension;
            string Originalname = file.FullName;
            Regex MultiSlash = new Regex("\\\\{2,}");
            Regex MultiSpace = new Regex("");
            Regex Multibar = new Regex("~{2,}");
            Regex Startbar = new Regex("^~");
            Regex Endbar = new Regex("~$");
            string N12 = "1";
            string BottomStitchArrayName = "BottomStitchArray1";
            string BarcodeStitchArrayName = "BarcodeStitchArray1";
            if (StorageRoot == StorageRoot1 && MasterArray == MasterArraySta1)
            {
                N12 = "1";
                TopOnlyOnRead = TopOnlyOnRead1;
                extCaptionDir = ext_caption_url1;
                ExtCaptionBoxList = ExtCaptionBoxList1;
                ExtCaptionBoxListLock = ExtCaptionBoxListLock1;
            }
            else if (StorageRoot == StorageRoot2 && MasterArray == MasterArraySta2)
            {
                N12 = "2";
                TopOnlyOnRead = TopOnlyOnRead2;
                extCaptionDir = ext_caption_url2;
                ExtCaptionBoxList = ExtCaptionBoxList2;
                ExtCaptionBoxListLock = ExtCaptionBoxListLock2;
            }
            else if (StorageRoot == StorageRoot3 && MasterArray == MasterArraySta3)
            {
                N12 = "3";
                TopOnlyOnRead = TopOnlyOnRead3;
                extCaptionDir = ext_caption_url3;
                ExtCaptionBoxList = ExtCaptionBoxList3;
                ExtCaptionBoxListLock = ExtCaptionBoxListLock3;
            }
            else if (StorageRoot == StorageRoot4 && MasterArray == MasterArraySta4)
            {
                N12 = "4";
                TopOnlyOnRead = TopOnlyOnRead4;
                extCaptionDir = ext_caption_url4;
                ExtCaptionBoxList = ExtCaptionBoxList4;
                ExtCaptionBoxListLock = ExtCaptionBoxListLock4;
            }
            else
            {
                N12 = "1";
                TopOnlyOnRead = false;
                extCaptionDir = ext_caption_url1;
                ExtCaptionBoxList = ExtCaptionBoxList1;
                ExtCaptionBoxListLock = ExtCaptionBoxListLock1;
            }
            if (BottomStitchArray == BottomStitchArray1)
            {
                BottomStitchArrayName = "BottomStitchArray1";
            }
            else if (BottomStitchArray == BottomStitchArray2)
            {
                BottomStitchArrayName = "BottomStitchArray2";
            }
            else if (BottomStitchArray == BottomStitchArray3)
            {
                BottomStitchArrayName = "BottomStitchArray3";
            }
            else if (BottomStitchArray == BottomStitchArray4)
            {
                BottomStitchArrayName = "BottomStitchArray4";
            }
            else if (BottomStitchArray == BottomStitchArray5)
            {
                BottomStitchArrayName = "BottomStitchArray5";
            }
            else
            {
                BottomStitchArrayName = "BottomStitchArray1";
            }
            if (BarcodeStitchArray == BarcodeStitchArray1)
            {
                BarcodeStitchArrayName = "BarcodeStitchArray1";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray2)
            {
                BarcodeStitchArrayName = "BarcodeStitchArray2";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray3)
            {
                BarcodeStitchArrayName = "BarcodeStitchArray3";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray4)
            {
                BarcodeStitchArrayName = "BarcodeStitchArray4";
            }
            else if (BarcodeStitchArray == BarcodeStitchArray5)
            {
                BarcodeStitchArrayName = "BarcodeStitchArray5";
            }
            else
            {
                BarcodeStitchArrayName = "BarcodeStitchArray1";
            }
            if (!N1.StartsWith("BT"))//除底扫外的五面
            {
                if (upload2 && N1.StartsWith("UP") && !currentmaster.upload2)//多路输出
                {
                    string FtpFileName2 = FtpFileNameFormat2 + extension;
                    FtpFileName2 = FtpFileName2.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    FtpFileName2 = FtpFileName2.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    FtpFileName2 = FtpFileName2.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                    FtpFileName2 = FtpFileName2.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    string temp_filename_url2 = ftp_temp_url6 + "\\" + delimiter + FtpFileName2;
                    if (!Directory.Exists(ftp_temp_url6))
                    {
                        Directory.CreateDirectory(ftp_temp_url6);
                    }
                    try
                    {
                        file.CopyTo(temp_filename_url2, false);
                        //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been copied to FTP temp3!";
                        AddLog(logcontent, LogContentArray);
                        currentmaster.upload2 = true;
                        //SystemLog(logpath, logcontent, MasterArray);
                    }
                    catch (Exception ex)
                    {
                        //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has failed copying to FTP temp3: " + temp_filename_url2 + "! Reason: " + ex.GetType().ToString() + ": " + ex.Message;
                        AddLog(logcontent, LogContentArray);
                        //SystemLog(logpath, logcontent, MasterArray);
                    }
                }
                if (BarcodeImgEnable && (BarcodeImgMode == "拼接"|| BarcodeImgMode == "stitched")  && (N1.StartsWith("UP") || (N1 == N2 && !currentmaster.BarcodeImagePresent)) && InductID != "一区" && InductID != "二区" && N2 != "None")//局部图片开启、处于拼接模式、供包台禁用、成功读取时顶部以及局部图片需要加入BarcodeStitchObj中，并移到暂存文件夹
                {
                    string boxidtext = SorterBoxIDFormat;
                    boxidtext = boxidtext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    boxidtext = boxidtext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    boxidtext = boxidtext.Replace("#None", "");
                    FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "NA");
                    FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "");
                    DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    FullDir = RootDir + "\\" + DirName;
                    FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                    FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "NA");
                    FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    /*HttpFileName = HttpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    HttpFileName = HttpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master","").Replace("master","")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    HttpFileName = HttpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                    HttpFileName = HttpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");*/
                    string filename_url = FullDir + FileName;//本地文件全名含路径
                    string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                    temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "");
                    temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                    temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                    temp_filename_header = Startbar.Replace(temp_filename_header, "");
                    temp_filename_header = Endbar.Replace(temp_filename_header, "");
                    string temp_filename_url = ftpTempDir + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                    string BarcodeStitchFullName = barcode_stitch_url + "\\" + "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + ".jpg";
                    string description = "";
                    description = CaptionFormat;
                    description = description.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    description = description.Replace("#N0", N0).Replace("#N1", "DN01").Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    description = description.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "N/A"); ;
                    string pos = "";
                    pos = PosFormat;
                    pos = pos.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    pos = pos.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    pos = pos.Replace("#None", "");
                    string DimWeight = DimAndWeightFormat;
                    DimWeight = DimWeight.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                    DimWeight = DimWeight.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                    DimWeight = DimWeight.Replace("#None", "0;0;-1;0");
                    string[] dimweight = DimWeight.Split(';');
                    string parcelHeight = "-1";
                    if (dimweight.Length >= 3)
                    {
                        parcelHeight = dimweight[2];
                    }
                    BarcodeStitchObject BarcodeStitchObj = null;
                    int BarcodeStitchIndex = -1;
                    lock (BarcodeStitchArrayLock)
                    {
                        BarcodeStitchIndex = FindStitchIndex(N0, N12, BarcodeStitchArray);
                        if (BarcodeStitchIndex > -1)
                        {
                            BarcodeStitchObj = BarcodeStitchArray[BarcodeStitchIndex];
                        }
                    }
                    if (BarcodeStitchIndex < 0)//没找到BarcodeStitchObj新建一个
                    {
                        if (currentmaster.barcodestitchenabled)
                        {
                            if (N1.StartsWith("UP"))
                            {
                                //BottomObj = new BottomStitchObject(N0, N12, description, file.CreationTime, FullDir, filename_url, ftpTempDir, temp_filename_url);
                                BarcodeStitchObj = new BarcodeStitchObject(N0, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, N12, description, DateTime.Now, FullDir, filename_url, ftpTempDir, temp_filename_url, pos, "0;0;0;0;0;0;0;0", Timestamp,boxidtext, parcelHeight);
                                lock (BarcodeStitchArrayLock)
                                {
                                    BarcodeStitchArray.Add(BarcodeStitchObj);
                                }
                                currentmaster.barcodestitchenabled = false;
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + N12 + " GroupID: " + N0 + " is not found in " + BarcodeStitchArrayName + " so a new object is created " + file.Name + " is the current file!";
                                AddLog(logcontent, LogContentArray);
                            }
                            else
                            {
                                HandleOK = 0;
                                return HandleOK;
                            }
                        }
                        else
                        {
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + N12 + " GroupID: " + N0 + " is not found in " + BarcodeStitchArrayName + " but barcode stitch is disabled for this master! " + file.Name + " is the current file!";
                            AddLog(logcontent, LogContentArray);
                        }
                    }
                    else
                    {
                        if (N1 == N2)
                        {
                            BarcodeStitchObj.barcodePos = barcodepos;
                        }
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + N12 + " GroupID: " + N0 + " is found in " + BarcodeStitchArrayName + "! " + file.Name + " is the current file!";
                        AddLog(logcontent, LogContentArray);
                    }
                    try
                    {
                        if (!Directory.Exists(barcode_stitch_url))
                        {
                            Directory.CreateDirectory(barcode_stitch_url);
                        }
                    }
                    catch (Exception ex) { }
                    if (!System.IO.File.Exists(BarcodeStitchFullName) && BarcodeStitchObj != null)
                    {
                        string failreason = "File not existed!";
                        if (file.Exists)
                        {
                            if ((N1.StartsWith("UP") && BarcodeStitchObj.UPImage == null) || (!N1.StartsWith("UP") && BarcodeStitchObj.BarcodeImage == null))
                            {
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to barcode stitch temp position: " + barcode_stitch_url + "!";
                                AddLog(logcontent, LogContentArray);
                                Thread.Sleep(1);
                                try
                                {
                                    file.MoveTo(BarcodeStitchFullName);
                                }
                                catch (Exception ex)
                                {
                                    failreason = ex.GetType().ToString() + ": " + ex.Message;
                                }
                            }
                            else
                            {
                                failreason = "Barcode/UP Image already existed!";
                            }
                        }
                        file.Refresh();
                        if (file.FullName != Originalname)//判断是否成功传输
                        {
                            if (N1.StartsWith("UP"))
                            {
                                //这边要加入顶部图片;
                                BarcodeStitchObj.N1 = N1;
                                BarcodeStitchObj.UPImage = file;
                                HandleOK = 2;
                            }
                            else 
                            {
                                //这边要局部图片
                                BarcodeStitchObj.BarcodeImage = file;
                                HandleOK = 2;
                            }
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to barcode stitch temp position: " + barcode_stitch_url + "!";
                            AddLog(logcontent, LogContentArray);
                        }
                        else
                        {
                            HandleOK = 0;
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to barcode stitch temp position: " + barcode_stitch_url + "!" + " Reason: " + failreason;
                            AddLog(logcontent, LogContentArray);
                        }
                    }
                    else
                    {
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Not moving file: " + file.Name + " to barcode stitch temp position because same image already existed!";
                        AddLog(logcontent, LogContentArray);
                        HandleOK = 0;
                    }

                }
                else//不参与拼接的图片
                {
                    if (InductID != "一区" && InductID != "二区")//供包台禁用，直接移动到水印区域或者本地存储区域
                    {
                        bool flag1 = N1.StartsWith("UP");
                        bool flag2 = N1 == N2 && BarcodeImgEnable && N2 != "None" && !currentmaster.BarcodeImagePresent;
                        bool flag22 = BarcodeImgEnable && BarcodeImgMode != "拼接" && BarcodeImgMode != "stitched" && N2 != "None" && !(N1.StartsWith("LF") || N1.StartsWith("LB") || N1.StartsWith("RF") || N1.StartsWith("RB") || N1.StartsWith("DN") || N1.StartsWith("TF") || N1.StartsWith("TB") || N1.StartsWith("LT") || N1.StartsWith("RT"));
                        bool flag3 = (!TopOnlyOnRead || N2 == "None") && (N1.StartsWith("LF") || N1.StartsWith("LB") || N1.StartsWith("RF") || N1.StartsWith("RB") || N1.StartsWith("DN") || N1.StartsWith("TF") || N1.StartsWith("TB") || N1.StartsWith("LT") || N1.StartsWith("RT"));
                        if (flag1 || flag22 || flag3)//要处理的图片
                        {
                            if (!captionEnable)//不加水印，直接移动到storageRoot和ftp_temp_url
                            {
                                FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "NA");
                                FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "");
                                DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                FullDir = RootDir + "\\" + DirName;
                                FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                                FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "NA");
                                FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                /*HttpFileName = HttpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                HttpFileName = HttpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master","").Replace("master","")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                HttpFileName = HttpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                HttpFileName = HttpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");*/
                                string filename_url = FullDir + FileName;//本地文件全名含路径
                                string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                                temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond).Replace("#ANA", "");
                                temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                                temp_filename_header = Startbar.Replace(temp_filename_header, "");
                                temp_filename_header = Endbar.Replace(temp_filename_header, "");
                                string temp_filename_url = ftpTempDir + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                                //string http_filename_url = httpTempDir + "\\" + HttpFileName;//http文件夹中文件全名含路径
                                try
                                {
                                    if (!Directory.Exists(FullDir))
                                    {
                                        Directory.CreateDirectory(FullDir);
                                    }
                                    else { }
                                    if (!Directory.Exists(ftpTempDir))
                                    {
                                        Directory.CreateDirectory(ftpTempDir);
                                    }
                                    if (System.IO.File.Exists(filename_url))
                                    {
                                        System.IO.File.Delete(filename_url);
                                    }
                                }
                                catch (Exception ex) { }
                                //string Originalname = file.FullName;
                                file.Refresh();
                                string failreason = "File not existed!";
                                //Stopwatch sw = new Stopwatch();
                                if (file.Exists)
                                {
                                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to storage position: " + filename_url + "!";
                                    AddLog(logcontent, LogContentArray);
                                    Thread.Sleep(1);
                                    try
                                    {
                                        if (!imgEncryptEnable)
                                        {
                                            file.MoveTo(filename_url);
                                        }
                                        else
                                        {
                                            EncryptFile(file, filename_url);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        failreason = ex.GetType().ToString() + ": " + ex.Message;
                                    }
                                }
                                file.Refresh();
                                if (file.FullName != Originalname || (!file.Exists && File.Exists(filename_url)))//判断是否成功传输
                                {
                                    HandleOK = 1;
                                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to storage position: " + filename_url + "!";
                                    AddLog(logcontent, LogContentArray);
                                    //SystemLog(logpath, logcontent, MasterArray);
                                    if (upload)
                                    {
                                        try
                                        {
                                            file.CopyTo(temp_filename_url, false);
                                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been copied to FTP temp!";
                                            AddLog(logcontent, LogContentArray);
                                            //SystemLog(logpath, logcontent, MasterArray);
                                        }
                                        catch (Exception ex)
                                        {
                                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has failed copying to FTP temp: " + temp_filename_url + "! Reason: " + ex.GetType().ToString() + ": " + ex.Message;
                                            AddLog(logcontent, LogContentArray);
                                            //SystemLog(logpath, logcontent, MasterArray);
                                        }
                                    }
                                }
                                else
                                {
                                    HandleOK = 0;
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to storage position: " + filename_url + "!" + "and not copied! File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + " Reason: " + failreason;
                                    AddLog(logcontent, LogContentArray);
                                }
                            }
                            else//要启用水印
                            {
                                string boxidtext = SorterBoxIDFormat;
                                boxidtext = boxidtext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                boxidtext = boxidtext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                boxidtext = boxidtext.Replace("#None", "");
                                if (!extCaptionEnable || boxidtext == "" || boxidtext == "xx")
                                {
                                    string captionFileName = "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + "_13," + "" + ".jpg";
                                    string captionFullName = caption_temp_url + "\\" + captionFileName;
                                    try
                                    {
                                        if (!Directory.Exists(caption_temp_url))
                                        {
                                            Directory.CreateDirectory(caption_temp_url);
                                        }
                                    }
                                    catch (Exception ex) { }
                                    if (!System.IO.File.Exists(captionFullName))
                                    {
                                        string failreason = "File not existed!";
                                        if (file.Exists)
                                        {
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to caption temp position: " + caption_temp_url + "!";
                                            AddLog(logcontent, LogContentArray);
                                            Thread.Sleep(1);
                                            try
                                            {
                                                file.MoveTo(captionFullName);
                                            }
                                            catch (Exception ex)
                                            {
                                                failreason = ex.GetType().ToString() + ": " + ex.Message;
                                            }
                                        }
                                        file.Refresh();
                                        if (file.FullName != Originalname)//判断是否成功传输
                                        {
                                            HandleOK = 1;
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to caption temp position: " + caption_temp_url + "!";
                                            AddLog(logcontent, LogContentArray);
                                        }
                                        else
                                        {
                                            HandleOK = 0;
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to caption temp position: " + caption_temp_url + "!" + " Reason: " + failreason;
                                            AddLog(logcontent, LogContentArray);
                                        }
                                    }
                                    else
                                    {
                                        HandleOK = 0;
                                    }
                                }
                                else//移动到extcaption文件夹,注意HandleOK的处理
                                {
                                    
                                    string captionFileName = "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + "_13," + "" + ".jpg";
                                    string extCaptionFullName = extCaptionDir + "\\" + captionFileName;
                                    try
                                    {
                                        if (!Directory.Exists(extCaptionDir))
                                        {
                                            Directory.CreateDirectory(extCaptionDir);
                                        }
                                    }
                                    catch (Exception ex) { }
                                    if (!System.IO.File.Exists(extCaptionFullName))
                                    {
                                        string failreason = "File not existed!";
                                        if (file.Exists)
                                        {
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to external caption temp position: " + extCaptionDir + "!";
                                            AddLog(logcontent, LogContentArray);
                                            Thread.Sleep(1);
                                            try
                                            {
                                                file.MoveTo(extCaptionFullName);
                                                file.CreationTime = DateTime.Now;
                                            }
                                            catch (Exception ex)
                                            {
                                                failreason = ex.GetType().ToString() + ": " + ex.Message;
                                            }
                                        }
                                        file.Refresh();
                                        if (file.FullName != Originalname)//判断是否成功传输
                                        {
                                            int index = -1;
                                            ExtCaptionBox currentBox = null;
                                            lock (ExtCaptionBoxListLock)
                                            {
                                                index = FindExtCaptionBoxListIndex(N0, N12, boxidtext, ExtCaptionBoxList);
                                                if (index > -1)
                                                {
                                                    currentBox = ExtCaptionBoxList[index];
                                                }
                                                else
                                                {
                                                    currentBox = new ExtCaptionBox(N0, N12, boxidtext);
                                                    ExtCaptionBoxList.Add(currentBox);
                                                }
                                            }
                                            lock (currentBox.BoxLock)
                                            {
                                                currentBox.Images.Add(file);
                                            }
                                            HandleOK = 1;
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to external caption temp position: " + extCaptionDir + "!";
                                            AddLog(logcontent, LogContentArray);
                                        }
                                        else
                                        {
                                            HandleOK = 0;
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to external caption temp position: " + extCaptionDir + "!" + " Reason: " + failreason;
                                            AddLog(logcontent, LogContentArray);
                                        }
                                    }
                                    else
                                    {
                                        HandleOK = 0;
                                    }
                                }
                            }
                        }
                        else//不处理，删
                        {
                            file.Refresh();
                            try
                            {
                                file.Delete();
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: Top Only On Read or Not Source or Barcode Image Disabled!";
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                                HandleOK = 1;
                            }
                            catch (Exception ex)
                            {
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: Top Only On Read or Not Source or Barcode Image Disabled! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                                HandleOK = 0;
                            }
                        }
                    }
                    else//供包台启用
                    {
                        bool flag1 = N1.StartsWith("UP") || N1.StartsWith("LF") || N1.StartsWith("LB") || N1.StartsWith("RF") || N1.StartsWith("RB") || N1.StartsWith("TF") || N1.StartsWith("TB") || N1.StartsWith("LT") || N1.StartsWith("RT");
                        if (flag1)//要处理的图片
                        {
                            int BCRNameID = -1;
                            int inductindex = InductID == "一区" ? 1 : 2;
                            string result = SorterResultFormat;
                            string codes = SorterBarcodeFormat;
                            string trayidtext = SorterTrayIDFormat;
                            string boxidtext = SorterBoxIDFormat;
                            string timestamptext = SorterTimeStampFormat;
                            var format = "yyyyMMddHHmmssfff";
                            string sorter_temp_filename_url = inductor_sorter_temp_url + "\\" + "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + ".jpg";
                            int trayid = 9999;
                            string boxid = "000000";
                            DateTime timestamp = DateTime.MinValue;
                            result = result.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            result = result.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            result = result.Replace("#None", "NR");
                            codes = codes.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            codes = codes.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            codes = codes.Replace("#None", "NoRead");
                            trayidtext = trayidtext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            trayidtext = trayidtext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            trayidtext = trayidtext.Replace("#None", "9999");
                            boxidtext = boxidtext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            boxidtext = boxidtext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            boxidtext = boxidtext.Replace("#None", "000000");
                            timestamptext = timestamptext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                            timestamptext = timestamptext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                            timestamptext = timestamptext.Replace("#None", DateTime.MinValue.ToString("yyyyMMddHHmmssfff"));
                            try
                            {
                                boxid = boxidtext;
                                trayid = Convert.ToInt32(trayidtext);
                                timestamp = DateTime.ParseExact(timestamptext, format, CultureInfo.InvariantCulture);
                            }
                            catch (Exception ex)
                            {
                                boxid = boxidtext;
                                trayid = 9999;
                                timestamp = DateTime.MinValue;
                            }
                            if (N1.StartsWith("UP"))
                            {
                                BCRNameID = 0;
                            }
                            else if (N1.StartsWith("LF") || N1.StartsWith("TF"))
                            {
                                BCRNameID = 1;
                            }
                            else if (N1.StartsWith("LB") || N1.StartsWith("LT"))
                            {
                                BCRNameID = 2;
                            }
                            else if (N1.StartsWith("RF") || N1.StartsWith("RT"))
                            {
                                BCRNameID = 3;
                            }
                            else if (N1.StartsWith("RB") || N1.StartsWith("TB"))
                            {
                                BCRNameID = 4;
                            }
                            else
                            {
                                BCRNameID = -1;
                            }
                            /*
                            if (trayid > 0 && trayid <= TrayObjects.Length && BCRNameID >= 0 && BCRNameID <= 4)
                            {
                                bool bindflag = false;
                                bool transferflag = false;
                                FileInfo[] Images = new FileInfo[6] { null, null, null, null, null, null };
                                //Tray currenttray = TrayObjects[trayid - 1];
                                
                                lock (currenttray.TrayInductorLock)
                                {
                                    Images[0] = currenttray.Images[0];
                                    Images[1] = currenttray.Images[1];
                                    Images[2] = currenttray.Images[2];
                                    Images[3] = currenttray.Images[3];
                                    Images[4] = currenttray.Images[4];
                                    Images[5] = currenttray.Images[5];
                                    if (timestamp > currenttray.BottomTimestamp && currenttray.BottomTimestamp != DateTime.MinValue && boxid == currenttray.BoxID)
                                    {
                                        if (currenttray.BottomResult == "GR" || currenttray.BottomResult == "MR")//底扫读到条码
                                        {
                                            if (inductindex == currenttray.InductID && (currenttray.SorterTimestamp == DateTime.MinValue || currenttray.SorterTimestamp == timestamp))//同区首次保留，其余删除；不同区删除
                                            {
                                                bindflag = true;
                                            }
                                            else
                                            {
                                                bindflag = false;
                                            }
                                        }
                                        else//底扫无读或无数据
                                        {
                                            if (inductindex == currenttray.InductID)//同区
                                            {
                                                if (currenttray.SorterTimestamp == DateTime.MinValue || currenttray.SorterTimestamp == timestamp || ((currenttray.SorterResult != "GR" && currenttray.SorterResult != "MR") && (result == "GR" || result == "MR" || currenttray.CurrentImageInductID != inductindex)))
                                                {
                                                    bindflag = true;
                                                }
                                                else
                                                {
                                                    bindflag = false;
                                                }
                                            }
                                            else if (currenttray.InductID != 0)//不同区,但tray有数据
                                            {
                                                if (currenttray.SorterTimestamp == DateTime.MinValue || currenttray.SorterTimestamp == timestamp || ((currenttray.SorterResult != "GR" && currenttray.SorterResult != "MR") && (result == "GR" || result == "MR")))
                                                {
                                                    bindflag = true;
                                                }
                                                else
                                                {
                                                    bindflag = false;
                                                }
                                            }
                                            else//tray无数据，比如上个包裹已经上传新的包裹还没来
                                            {
                                                bindflag = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (currenttray.BottomTimestamp == DateTime.MinValue)//没有上包信息直接移动
                                        {
                                            transferflag = true;
                                        }
                                        bindflag = false;
                                    }
                                }
                                
                                if (bindflag)//绑
                                {
                                    if (!Directory.Exists(inductor_sorter_temp_url))
                                    {
                                        Directory.CreateDirectory(inductor_sorter_temp_url);
                                    }
                                    file.Refresh();
                                    string failreason = "File not existed!";
                                    //Stopwatch sw = new Stopwatch();
                                    if (file.Exists)
                                    {
                                        //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to inductor temp position: " + sorter_temp_filename_url + "!";
                                        AddLog(logcontent, LogContentArray);
                                        Thread.Sleep(1);
                                        try
                                        {
                                            file.MoveTo(sorter_temp_filename_url);
                                        }
                                        catch (Exception ex)
                                        {
                                            failreason = ex.GetType().ToString() + ": " + ex.Message;
                                        }
                                    }
                                    file.Refresh();
                                    if (file.FullName != Originalname)//判断是否成功传输
                                    {
                                        HandleOK = 1;
                                        if (Images[BCRNameID] != null)
                                        {
                                            Images[BCRNameID].Refresh();
                                            if (Images[BCRNameID].Exists && Images[BCRNameID].DirectoryName == inductor_sorter_temp_url)
                                            {
                                                try
                                                {
                                                    Images[BCRNameID].Delete();
                                                }
                                                catch (Exception ex) { }
                                            }
                                        }
                                        
                                        lock (currenttray.TrayInductorLock)
                                        {
                                            currenttray.SorterTimestamp = timestamp;
                                            currenttray.CurrentImageInductID = inductindex;
                                            currenttray.SorterCodes = codes;
                                            currenttray.SorterResult = result;
                                            currenttray.N0 = N0;
                                            currenttray.N2 = N2;
                                            currenttray.N3 = N3;
                                            currenttray.N4 = N4;
                                            currenttray.N5 = N5;
                                            currenttray.N6 = N6;
                                            currenttray.N7 = N7;
                                            currenttray.N8 = N8;
                                            currenttray.N9 = N9;
                                            currenttray.N10 = N10;
                                            currenttray.N11 = N11;
                                            currenttray.N12 = N12;
                                            currenttray.Images[BCRNameID] = file;
                                        }
                                        //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to inductor temp position: " + sorter_temp_filename_url + "!";
                                        AddLog(logcontent, LogContentArray);
                                        //SystemLog(logpath, logcontent, MasterArray);
                                    }
                                    else
                                    {
                                        HandleOK = 0;
                                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to inductor temp position: " + sorter_temp_filename_url + "!" + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + " Reason: " + failreason;
                                        AddLog(logcontent, LogContentArray);
                                    }
                                }
                                else if (transferflag)//没有上包信息直接移动
                                {
                                    //
                                    if (!captionEnable)//不加水印，直接移动到storageRoot和ftp_temp_url
                                    {
                                        FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                        FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                        FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                        FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                        DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                        DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                        DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                        DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                        FullDir = RootDir + "\\" + DirName;
                                        FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                        FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                                        FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                        FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                        FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                        FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                        string filename_url = FullDir + FileName;//本地文件全名含路径
                                        string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                                        temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                        temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                        temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                        temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                        temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                                        temp_filename_header = Startbar.Replace(temp_filename_header, "");
                                        temp_filename_header = Endbar.Replace(temp_filename_header, "");
                                        string temp_filename_url = ftpTempDir + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                                        //string http_filename_url = httpTempDir + "\\" + HttpFileName;//http文件夹中文件全名含路径
                                        try
                                        {
                                            if (!Directory.Exists(FullDir))
                                            {
                                                Directory.CreateDirectory(FullDir);
                                            }
                                            else { }
                                            if (!Directory.Exists(ftpTempDir))
                                            {
                                                Directory.CreateDirectory(ftpTempDir);
                                            }
                                            if (System.IO.File.Exists(filename_url))
                                            {
                                                System.IO.File.Delete(filename_url);
                                            }
                                        }
                                        catch (Exception ex) { }
                                        //string Originalname = file.FullName;
                                        file.Refresh();
                                        string failreason = "File not existed!";
                                        //Stopwatch sw = new Stopwatch();
                                        if (file.Exists)
                                        {
                                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to storage position: " + filename_url + "!";
                                            AddLog(logcontent, LogContentArray);
                                            Thread.Sleep(1);
                                            try
                                            {
                                                file.MoveTo(filename_url);
                                            }
                                            catch (Exception ex)
                                            {
                                                failreason = ex.GetType().ToString() + ": " + ex.Message;
                                            }
                                        }
                                        file.Refresh();
                                        if (file.FullName != Originalname)//判断是否成功传输
                                        {
                                            HandleOK = 1;
                                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to storage position: " + filename_url + "!";
                                            AddLog(logcontent, LogContentArray);
                                            //SystemLog(logpath, logcontent, MasterArray);
                                            if (upload)
                                            {
                                                try
                                                {
                                                    file.CopyTo(temp_filename_url, false);
                                                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been copied to FTP temp!";
                                                    AddLog(logcontent, LogContentArray);
                                                    //SystemLog(logpath, logcontent, MasterArray);
                                                }
                                                catch (Exception ex)
                                                {
                                                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has failed copying to FTP temp: " + temp_filename_url + "! Reason: " + ex.GetType().ToString() + ": " + ex.Message;
                                                    AddLog(logcontent, LogContentArray);
                                                    //SystemLog(logpath, logcontent, MasterArray);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            HandleOK = 0;
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to storage position: " + filename_url + "!" + "and not copied! File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff") + " Reason: " + failreason;
                                            AddLog(logcontent, LogContentArray);
                                        }
                                    }
                                    else//要启用水印
                                    {

                                        string captionFileName = "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + ".jpg";
                                        string captionFullName = caption_temp_url + "\\" + captionFileName;
                                        try
                                        {
                                            if (!Directory.Exists(caption_temp_url))
                                            {
                                                Directory.CreateDirectory(caption_temp_url);
                                            }
                                        }
                                        catch (Exception ex) { }
                                        if (!System.IO.File.Exists(captionFullName))
                                        {
                                            string failreason = "File not existed!";
                                            if (file.Exists)
                                            {
                                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to caption temp position: " + caption_temp_url + "!";
                                                AddLog(logcontent, LogContentArray);
                                                Thread.Sleep(1);
                                                try
                                                {
                                                    file.MoveTo(captionFullName);
                                                }
                                                catch (Exception ex)
                                                {
                                                    failreason = ex.GetType().ToString() + ": " + ex.Message;
                                                }
                                            }
                                            file.Refresh();
                                            if (file.FullName != Originalname)//判断是否成功传输
                                            {
                                                HandleOK = 1;
                                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to caption temp position: " + caption_temp_url + "!";
                                                AddLog(logcontent, LogContentArray);
                                            }
                                            else
                                            {
                                                HandleOK = 0;
                                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to caption temp position: " + caption_temp_url + "!" + " Reason: " + failreason;
                                                AddLog(logcontent, LogContentArray);
                                            }
                                        }
                                        else
                                        {
                                            HandleOK = 0;
                                        }
                                    }
                                    //
                                }
                                else//删
                                {
                                    if (file.Exists)
                                    {
                                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to delete file: " + file.Name + " since the tray has GR/MR sorter images or NR sorter images from the same region!";
                                        AddLog(logcontent, LogContentArray);
                                        Thread.Sleep(1);
                                        try
                                        {
                                            file.Delete();
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted since the tray has GR/MR sorter images or NR sorter images from the same region!";
                                            AddLog(logcontent, LogContentArray);
                                            HandleOK = 1;
                                        }
                                        catch
                                        {
                                            HandleOK = 0;
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted since the tray has GR/MR sorter images or NR sorter images from the same region!";
                                            AddLog(logcontent, LogContentArray);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                HandleOK = 0;
                            }
                            */
                        }
                        else//不处理，删
                        {
                            file.Refresh();
                            try
                            {
                                file.Delete();
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: Only Acquisition Image is needed when Induct is enabled!";
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                                HandleOK = 1;
                            }
                            catch (Exception ex)
                            {
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: Only Acquisition Image is needed when Induct is enabled! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                                HandleOK = 0;
                            }
                        }
                    }
                }
            }
            else//底扫
            {
                string boxidtext = SorterBoxIDFormat;
                boxidtext = boxidtext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                boxidtext = boxidtext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                boxidtext = boxidtext.Replace("#None", "");
                FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", "DN01").Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                DirName = DirName.Replace("#N0", N0).Replace("#N1", "DN01").Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                FullDir = RootDir + "\\" + DirName;
                FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                string stitchFileName = "_0," + N0 + "_1," + N1 + "_2," + N2 + "_3," + N3 + "_4," + N4 + "_5," + N5 + "_6," + N6 + "_7," + N7 + "_8," + N8 + "_9," + N9 + "_10," + N10 + "_11," + N11 + "_12," + N12 + ".jpg";
                /*string stitchFileName = FileNameFormat + "_" + N12 + extension;
                stitchFileName = stitchFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                stitchFileName = stitchFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                stitchFileName = stitchFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                stitchFileName = stitchFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                */
                FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                FileName = FileName.Replace("#N0", N0).Replace("#N1", "DN01").Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                string filename_url = FullDir + FileName;//本地文件全名含路径
                string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", "DN01").Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                temp_filename_header = Startbar.Replace(temp_filename_header, "");
                temp_filename_header = Endbar.Replace(temp_filename_header, "");
                string temp_filename_url = ftpTempDir + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                string stitch_filename_url = stitch_temp_url + "\\" + stitchFileName;//缝图文件夹中全名
                string description = "";
                description = CaptionFormat;
                description = description.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                description = description.Replace("#N0", N0).Replace("#N1", "DN01").Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                description = description.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                string result = SorterResultFormat;
                result = result.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                result = result.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                result = result.Replace("#None", "GR");
                BottomStitchObject BottomObj = null;
                int BottomIndex = -1;
                lock (BottomStitchArrayLock)
                {
                    BottomIndex = FindStitchIndex(N0,N12,BottomStitchArray);
                    if (BottomIndex > -1)
                    {
                        BottomObj = BottomStitchArray[BottomIndex];
                    }
                }
                if (BottomIndex < 0)//没找到BottomObj新建一个
                {
                    if (currentmaster.bottomenabled)
                    {
                        //BottomObj = new BottomStitchObject(N0, N12, description, file.CreationTime, FullDir, filename_url, ftpTempDir, temp_filename_url);
                        BottomObj = new BottomStitchObject(N0, N12, result, description, DateTime.Now, FullDir, FileName, ftpTempDir, temp_filename_header, FtpFileName, boxidtext);
                        lock (BottomStitchArrayLock)
                        {
                            BottomStitchArray.Add(BottomObj);
                        }
                        currentmaster.bottomenabled = false;
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + N12 + " GroupID: " + N0 + " is not found in " + BottomStitchArrayName + " so a new object is created " + file.Name + " is the current file!";
                        AddLog(logcontent, LogContentArray);
                    }
                    else
                    {
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + N12 + " GroupID: " + N0 + " is not found in " + BottomStitchArrayName + " and bottom is disabled for this master! " + file.Name + " is the current file!";
                        AddLog(logcontent, LogContentArray);
                    }
                }
                else
                {
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + N12 + " GroupID: " + N0 + " is found in " + BottomStitchArrayName + "! " + file.Name + " is the current file!";
                    AddLog(logcontent, LogContentArray);
                }
                if (BottomObj != null)
                {
                    int id = -1;
                    List<FileInfo> BT = null;
                    if (N1.StartsWith("BT01"))
                    {
                        BT = BottomObj.BT01;
                        //id = 0;
                    }
                    else if (N1.StartsWith("BT02"))
                    {
                        BT = BottomObj.BT02;
                        //id = 1;
                    }
                    else if (N1.StartsWith("BT03"))
                    {
                        BT = BottomObj.BT03;
                        //id = 2;
                    }
                    else if (N1.StartsWith("BT04"))
                    {
                        BT = BottomObj.BT04;
                        //id = 3;
                    }
                    else if (N1.StartsWith("BT05"))
                    {
                        BT = BottomObj.BT05;
                        //id = 4;
                    }
                    else if (N1.StartsWith("BT06"))
                    {
                        BT = BottomObj.BT06;
                        //id = 5;
                    }
                    if (N1.EndsWith("0"))
                    {
                        id = 0;
                    }
                    else if (N1.EndsWith("1"))
                    {
                        id = 1;
                    }
                    else if (N1.EndsWith("2"))
                    {
                        id = 2;
                    }
                    else if (N1.EndsWith("3"))
                    {
                        id = 3;
                    }
                    if (id > -1 && id < 4 && BT != null)
                    {
                        try
                        {
                            if (!Directory.Exists(stitch_temp_url))
                            {
                                Directory.CreateDirectory(stitch_temp_url);
                            }
                        }
                        catch { }
                        if (!System.IO.File.Exists(stitch_filename_url) && BT[id] == null)
                        {
                            string failreason = "File not existed!";
                            if (file.Exists)
                            {
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] Trying to move file: " + file.Name + " to stitch temp position: " + stitch_temp_url + "!";
                                AddLog(logcontent, LogContentArray);
                                Thread.Sleep(1);
                                try
                                {
                                    file.MoveTo(stitch_filename_url);
                                }
                                catch (Exception ex)
                                {
                                    failreason = ex.GetType().ToString() + ": " + ex.Message;
                                }
                            }
                            file.Refresh();
                            if (file.FullName != Originalname)//判断是否成功传输
                            {
                                HandleOK = 1;
                                BT[id] = file;
                                //BottomObj.ImagesNum++;
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been moved to stitch temp position: " + stitch_temp_url + "!";
                                AddLog(logcontent, LogContentArray);
                            }
                            else
                            {
                                HandleOK = 0;
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being moved to stitch temp position: " + stitch_temp_url + "!" + " Reason: " + failreason;
                                AddLog(logcontent, LogContentArray);
                            }
                        }
                        else
                        {
                            HandleOK = 0;
                        }

                    }
                    else
                    {
                        HandleOK = 0;
                    }
                }
                else
                {
                    HandleOK = 0;
                }
            }
            
            //sw.Stop();
            //MessageBox.Show(sw.ElapsedMilliseconds + "ms");
            return HandleOK;
        }
        private ArrayList SlaveImageFiletoArray(int CamCount, FileInfo file, ArrayList MasterArray, ArrayList HttpArray, Object MasterArrayLock, Object HttpArrayLock, string N0, string StationLogDir, ArrayList LogContentArray,string N1)
        {
            string logpath = "";
            string logcontent = "";
            string SystemLogDirectory = systemLog_dir + "\\" + StationLogDir;
            string MasterName = "MasterArraySta1";
            if (MasterArray == MasterArraySta1)
            {
                MasterName = "MasterArraySta1";
            }
            else if (MasterArray == MasterArraySta2)
            {
                MasterName = "MasterArraySta2";
            }
            else if (MasterArray == MasterArraySta3)
            {
                MasterName = "MasterArraySta3";
            }
            else if (MasterArray == MasterArraySta4)
            {
                MasterName = "MasterArraySta4";
            }
            bool handleflag = false;
            ArrayList ReturnArray = new ArrayList();
            MasterImage Emptymaster = new MasterImage();
            MasterImage master = Emptymaster;
            int MstIndex = -1;
            lock (MasterArrayLock)
            {
                MstIndex = FindMstIndex(N0, MasterArray);
                if (MstIndex > -1)
                {
                    master = MasterArray[MstIndex] as MasterImage;
                }
            }
            if (MstIndex > -1 && N0 == master.N0 && master.time < CamCount && Math.Abs((master.TimeStamp - file.CreationTime).TotalSeconds) < 5)//找到的master是合适的master
            {
                handleflag = true;
                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID:" + master.N0 + " has already been found in " + MasterName + " so slave finds its master! " + file.Name + " is the current file!";
                AddLog(logcontent, LogContentArray);
                if (!N1.StartsWith("UP") && !N1.StartsWith("LB") && !N1.StartsWith("LF") && !N1.StartsWith("RB") && !N1.StartsWith("RF") && !N1.StartsWith("BT") && !N1.StartsWith("DN") && !N1.StartsWith("TF") && !N1.StartsWith("TB") && !N1.StartsWith("LT") && !N1.StartsWith("RT"))
                {
                    if (!BarcodeImgEnable)
                    {
                        handleflag = false;
                        try
                        {
                            file.Delete();
                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: slave && BarcodeImgDisabled && not Image Reader!";
                            AddLog(logcontent, LogContentArray);
                            //SystemLog(logpath, logcontent, MasterArray);
                        }
                        catch (Exception ex)
                        {
                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: slave && BarcodeImgDisabled && not Image Reader! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            AddLog(logcontent, LogContentArray);
                            //SystemLog(logpath, logcontent,MasterArray);
                        }
                    }
                }


            }
            else//没找到或找到了不合适的master
            {
                master = Emptymaster;
                handleflag = false;
            }
            ReturnArray.Add(master);
            ReturnArray.Add(handleflag);
            return ReturnArray;
        }
        private ArrayList MasterImageFiletoArray(DateTime TriggerTimestamp, DateTime Timestamp,int CamCount, FileInfo file, ArrayList MasterArray, ArrayList HttpArray, Object MasterArrayLock, Object HttpArrayLock, string N0, string N1, string N2, string N3, string N4, string N5, string N6, string N7, string N8, string N9, string N10, string N11, string StationLogDir, ArrayList LogContentArray, string InductID)
        {
            string logpath = "";
            string logcontent = "";
            string SystemLogDirectory = systemLog_dir + "\\" + StationLogDir;
            string MasterName = "MasterArraySta1";
            if (MasterArray == MasterArraySta1)
            {
                MasterName = "MasterArraySta1";
            }
            else if (MasterArray == MasterArraySta2)
            {
                MasterName = "MasterArraySta2";
            }
            else if (MasterArray == MasterArraySta3)
            {
                MasterName = "MasterArraySta3";
            }
            else if (MasterArray == MasterArraySta4)
            {
                MasterName = "MasterArraySta4";
            }
            bool handleflag = false;
            ArrayList ReturnArray = new ArrayList();
            MasterImage Emptymaster = new MasterImage();
            MasterImage master = Emptymaster;
            int MstIndex = -1;
            lock (MasterArrayLock)
            {
                MstIndex = FindMstIndex(N0, MasterArray);
                if (MstIndex > -1)
                {
                    master = MasterArray[MstIndex] as MasterImage;
                }
            }
            if (MstIndex < 0)//没找到master新建一个
            {
                master = new MasterImage(N0, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, Timestamp, TriggerTimestamp);
                lock (MasterArrayLock)
                {
                    MasterArray.Add(master);
                }
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " is not found in " + MasterName + " so a new master is created " + file.Name + " is the current file!";
                AddLog(logcontent, LogContentArray);
                if (httpUpload && !captionEnable && InductID != "一区" && InductID != "二区" && !(BarcodeImgEnable && (BarcodeImgMode == "拼接" || BarcodeImgMode == "stitched")))//启用局部读码图片拼接或启用水印或者打开供包台绑定功能的工位暂时不把master加入HttpArray
                {
                    lock (HttpArrayLock)
                    {
                        HttpArray.Add(master);
                    }
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " added to httpArray! " + file.Name + " is the current file!";
                    AddLog(logcontent, LogContentArray);
                }
                handleflag = true;
                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                
                //SystemLog(logpath, logcontent, MasterArray);
                if (!N1.StartsWith("UP") && !N1.StartsWith("LB") && !N1.StartsWith("LF") && !N1.StartsWith("RB") && !N1.StartsWith("RF") && !N1.StartsWith("BT") && !N1.StartsWith("DN") && !N1.StartsWith("TF") && !N1.StartsWith("TB") && !N1.StartsWith("LT") && !N1.StartsWith("RT"))
                {
                    //if (N1 != master.N2 || !BarcodeImgEnable)
                    if (!BarcodeImgEnable)
                    {
                        handleflag = false;
                        try
                        {
                            file.Delete();
                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: added to masterArray && BarcodeImgDisabled && not Image Reader!";
                            AddLog(logcontent, LogContentArray);
                            //SystemLog(logpath, logcontent, MasterArray);
                        }
                        catch (Exception ex)
                        {
                            //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: added to masterArray && BarcodeImgDisabled && not Image Reader! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                            AddLog(logcontent, LogContentArray);
                            //SystemLog(logpath, logcontent,MasterArray);
                        }
                    }
                }
            }
            else
            {
                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " has already been found in " + MasterName + " so a new master is not created! " + file.Name + " is the current file!";
                AddLog(logcontent, LogContentArray);
                //SystemLog(logpath, logcontent, MasterArray);
                if (Math.Abs((master.TimeStamp - file.CreationTime).TotalSeconds) < 5 && N0 == master.N0 && master.time < CamCount)//找到的master是合适的master
                {
                    handleflag = true;
                    if (!N1.StartsWith("UP") && !N1.StartsWith("LB") && !N1.StartsWith("LF") && !N1.StartsWith("RB") && !N1.StartsWith("RF") && !N1.StartsWith("BT") && !N1.StartsWith("DN") && !N1.StartsWith("TF") && !N1.StartsWith("TB") && !N1.StartsWith("LT") && !N1.StartsWith("RT"))
                    {
                        //if (N1 != master.N2 || !BarcodeImgEnable)
                        if (!BarcodeImgEnable)
                        {
                            handleflag = false;
                            try
                            {
                                file.Delete();
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " has been deleted for reason: added to masterArray && not Image Reader!";
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                            }
                            catch (Exception ex)
                            {
                                //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + file.Name + " failed being deleted for reason: added to masterArray && not Image Reader! Reason:" + ex.GetType().ToString() + " " + ex.Message + " File creation time: " + file.CreationTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
                                AddLog(logcontent, LogContentArray);
                                //SystemLog(logpath, logcontent, MasterArray);
                            }
                        }
                    }
                }
                else//找到的master不是合适的master
                {
                    
                    handleflag = false;
                    //logpath = SystemLogDirectory + "\\log " + DateTime.Now.ToString("yyyyMMdd") + ".txt";
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " has already been found in " + MasterName + " but it is the GroupID for another trigger! " + file.Name + " is the current file!";
                    AddLog(logcontent, LogContentArray);
                    master = Emptymaster;
                    //SystemLog(logpath, logcontent, MasterArray);
                }
            }
            ReturnArray.Add(master);
            ReturnArray.Add(handleflag);
            return ReturnArray;
        }
        private bool IsMaster(string BCRName)
        {
            if (BCRName.IndexOf("master") > -1 || BCRName.IndexOf("Master") > -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static int FindExtCaptionBoxListIndex(string N0, string N12, string boxid, List<ExtCaptionBox> ExtCaptionBoxList)
        {
            int index = -1;
            for (int i = 0; i < ExtCaptionBoxList.Count; i++)
            {
                if (ExtCaptionBoxList[i] is ExtCaptionBox)
                {
                    if (ExtCaptionBoxList[i].BoxID == boxid && ExtCaptionBoxList[i].N0 == N0 && ExtCaptionBoxList[i].N12 == N12)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;
        }
        public static int FindExternalCaptionInfoListIndex(string boxid)
        {
            int index = -1;
            for (int i = 0; i < ExternalCaptionInfoList.Count; i++)
            {
                if (ExternalCaptionInfoList[i] is ExternalCaptionInfo)
                {
                    if (ExternalCaptionInfoList[i].BoxID == boxid)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;
        }
        public static int FindBoxListIndex(string boxid)
        {
            int index = -1;
            for (int i = 0; i < BoxList.Count; i++)
            {
                if (BoxList[i] is Box)
                {
                    if (BoxList[i].BoxID == boxid)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;
        }
        private int FindMstIndex(string N0, ArrayList MasterArray)
        {
            int index = -1;
            for (int i = 0; i < MasterArray.Count; i++)
            {
                if (MasterArray[i] is MasterImage)
                {
                    MasterImage mst = MasterArray[i] as MasterImage;
                    if (mst.N0 == N0)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;

        }
        private int FindStitchIndex(string N0, string StationName, List<BottomStitchObject> StitchArray)
        {
            int index = -1;
            for (int i = 0; i < StitchArray.Count; i++)
            {
                if (StitchArray[i] is BottomStitchObject)
                {
                    if (StitchArray[i].N0 == N0 && StitchArray[i].StationName == StationName)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;
        }
        private int FindStitchIndex(string N0, string StationName, List<BarcodeStitchObject> StitchArray)
        {
            int index = -1;
            for (int i = 0; i < StitchArray.Count; i++)
            {
                if (StitchArray[i] is BarcodeStitchObject)
                {
                    if (StitchArray[i].N0 == N0 && StitchArray[i].StationName == StationName)
                    {
                        index = i;
                        return index;
                    }
                }
            }
            return index;
        }
        private void InductFtpRootButton_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                InductFtpRootBox.Text = dialog.SelectedPath;
            }
        }
        private void selectFtpRootButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FtpRootBox1.Text = dialog.SelectedPath;
            }
            
        }
        private void SelectFtpRootButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FtpRootBox2.Text = dialog.SelectedPath;
            }
        }
        private void selectFtpRootButton3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FtpRootBox3.Text = dialog.SelectedPath;
            } 
        }
        private void SelectFtpRootButton4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                FtpRootBox4.Text = dialog.SelectedPath;
            }
        }
        private void SelectImageRootButton1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ImageRootBox1.Text = dialog.SelectedPath;
            }
        }
        private void SelectImageRootButton2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ImageRootBox2.Text = dialog.SelectedPath;
            }
        }
        private void SelectImageRootButton3_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ImageRootBox3.Text = dialog.SelectedPath;
            }
        }
        private void selectImageRootButton4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.RootFolder = Environment.SpecialFolder.MyComputer;
            dialog.Description = "请选择文件路径";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ImageRootBox4.Text = dialog.SelectedPath;
            }
        }
        private void Sta1OnBox_CheckedChanged(object sender, EventArgs e)
        {
            lock(OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (Sta1OnBox.Checked)
                    {
                        config.AppSettings.Settings["Sta1On"].Value = "true";
                        Sta1On = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["Sta1On"].Value = "false";
                        Sta1On = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void Sta2OnBox_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (Sta2OnBox.Checked)
                    {
                        config.AppSettings.Settings["Sta2On"].Value = "true";
                        Sta2On = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["Sta2On"].Value = "false";
                        Sta2On = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void Sta3OnBox_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (Sta3OnBox.Checked)
                    {
                        config.AppSettings.Settings["Sta3On"].Value = "true";
                        Sta3On = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["Sta3On"].Value = "false";
                        Sta3On = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void Sta4OnBox_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (Sta4OnBox.Checked)
                    {
                        config.AppSettings.Settings["Sta4On"].Value = "true";
                        Sta4On = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["Sta4On"].Value = "false";
                        Sta4On = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void UploadCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock(OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (UploadCheck.Checked)
                    {
                        config.AppSettings.Settings["Upload"].Value = "true";
                        upload = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["Upload"].Value = "false";
                        upload = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void label7_Click(object sender, EventArgs e)
        {

        }
        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void label10_Click(object sender, EventArgs e)
        {

        }

        private void ftpRootBox3_TextChanged(object sender, EventArgs e)
        {

        }

        

        private void CamNoCombo3_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label13_Click(object sender, EventArgs e)
        {

        }
        private void autorunwithshell()//加入快速启动栏
        {

            if (!System.IO.File.Exists(startup_path))
            {
                try
                {
                    /*var shell = new WshShellClass();
                    IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut("D:\\test\\test.lnk");
                    shortcut.TargetPath ="D:\\test\\test.txt";
                    shortcut.WorkingDirectory = target_dir;
                    shortcut.WindowStyle = 1;
                    shortcut.Description = "存图助手开机自启动";
                    shortcut.IconLocation = target_dir + @"\存图助手.exe";
                    shortcut.Save();*/
                    System.IO.File.WriteAllBytes(startup_path, new byte[0]);
                    Shell32.Shell shl = new Shell32.Shell();
                    Shell32.Folder dir = shl.NameSpace(startup_dir);
                    Shell32.FolderItem itm = dir.Items().Item("存图助手标准版.lnk");
                    Shell32.ShellLinkObject lnk = (Shell32.ShellLinkObject)itm.GetLink;
                    lnk.Path = target_path;
                    lnk.WorkingDirectory = target_dir;
                    lnk.Save(startup_path);
                }
                catch (Exception ex)
                { }
            }

        }
        private void autorun()
        {
            if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA)
            {
                autorunwithshell();
            }
            else
            {
                Thread staThream = new Thread(new ThreadStart(autorunwithshell));
                staThream.IsBackground = true;
                staThream.SetApartmentState(ApartmentState.STA);
                staThream.Start();
                staThream.Join();
            }
        }

        private void LogEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (LogEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["LogEnable"].Value = "true";
                        logEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["LogEnable"].Value = "false";
                        logEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void HTTPUploadCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (HTTPUploadCheck.Checked)
                    {
                        config.AppSettings.Settings["httpUpload"].Value = "true";
                        httpUpload = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["httpUpload"].Value = "false";
                        httpUpload = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void UploadStatusCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (UploadStatusCheck.Checked)
                    {
                        config.AppSettings.Settings["UploadStatus"].Value = "true";
                        uploadStatus = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["UploadStatus"].Value = "false";
                        uploadStatus = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        public void DrawCaptionOpencv(FileInfo File, string description,string local_fullname, string pos,bool drawborder, calObject calobj, double parcelHeight, bool isUP)
        {
            Mat srcImg = null;
            string[] des = Regex.Split(description, "\r\n");
            int x = 50;
            int y = 150;
            int x1 = -1, y1 = -1, x2 = -1, y2 = -1, x3 = -1, y3 = -1, x4 = -1, y4 = -1;
            int X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0;
            double r11 = calobj.r11;
            double r12 = calobj.r12;
            double r13 = calobj.r13;
            double r14 = calobj.r14;
            double r21 = calobj.r21;
            double r22 = calobj.r22;
            double r23 = calobj.r23;
            double r24 = calobj.r24;
            double r31 = calobj.r31;
            double r32 = calobj.r32;
            double r33 = calobj.r33;
            bool posValid = false;
            try
            {
                if (allowPackageBorder && calobj.calOK && drawborder)
                {
                    string[] posstring = pos.Split(';');
                    if (posstring.Length == 13)
                    {
                        try
                        {
                            X1 = Convert.ToInt32(posstring[1]);
                            Y1 = Convert.ToInt32(posstring[2]);
                            Z1 = Convert.ToInt32(posstring[3]);
                            X2 = Convert.ToInt32(posstring[4]);
                            Y2 = Convert.ToInt32(posstring[5]);
                            Z2 = Convert.ToInt32(posstring[6]);
                            X3 = Convert.ToInt32(posstring[7]);
                            Y3 = Convert.ToInt32(posstring[8]);
                            Z3 = Convert.ToInt32(posstring[9]);
                            X4 = Convert.ToInt32(posstring[10]);
                            Y4 = Convert.ToInt32(posstring[11]);
                            Z4 = Convert.ToInt32(posstring[12]);
                            posValid = true;
                        }
                        catch (Exception ex) { posValid = false; }
                    }
                    else
                    {
                        posValid = false;
                    }
                }
                if (File is FileInfo)
                {
                    File.Refresh();
                    if ((File.Extension.ToLower() == ".bmp" || File.Extension.ToLower() == ".jpg") && File.Exists)
                    {
                        srcImg = Cv2.ImRead(File.FullName);
                        if (!srcImg.Empty())
                        {
                            if (isUP && GridLineEnable && parcelHeight > 0 && parcelHeight < MountingHeight)
                            {
                                GridLine(srcImg, srcImg.Cols, srcImg.Rows, parcelHeight, 20);
                            }
                            x = (int)(srcImg.Cols / 80);
                            y = (int)(srcImg.Rows / 20);
                            for (int i = 0; i < des.Length; i++)
                            {
                                if (des[i] != "")
                                {
                                    Cv2.PutText(srcImg, des[i], new OpenCvSharp.Point(x, y), HersheyFonts.HersheySimplex, Math.Max(1.0 * srcImg.Rows / 750.0, 1), new Scalar(0, 255, 0), (int)(Math.Max(1.0 * srcImg.Rows / 500.0, 2)));
                                }
                                y += (int)(40.0 * srcImg.Rows / 750.0);
                            }
                            if (posValid && allowPackageBorder && calobj.calOK && drawborder)
                            {
                                x1 = Math.Max(0, Math.Min((int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), srcImg.Cols));
                                y1 = Math.Max(0, Math.Min((int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), srcImg.Rows));
                                x2 = Math.Max(0, Math.Min((int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), srcImg.Cols));
                                y2 = Math.Max(0, Math.Min((int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), srcImg.Rows));
                                x3 = Math.Max(0, Math.Min((int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), srcImg.Cols));
                                y3 = Math.Max(0, Math.Min((int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), srcImg.Rows));
                                x4 = Math.Max(0, Math.Min((int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), srcImg.Cols));
                                y4 = Math.Max(0, Math.Min((int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), srcImg.Rows));
                                Cv2.Line(srcImg, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), new Scalar(0, 255, 0), (int)(srcImg.Cols/1000));
                                Cv2.Line(srcImg, new OpenCvSharp.Point(x2, y2), new OpenCvSharp.Point(x3, y3), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                                Cv2.Line(srcImg, new OpenCvSharp.Point(x3, y3), new OpenCvSharp.Point(x4, y4), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                                Cv2.Line(srcImg, new OpenCvSharp.Point(x4, y4), new OpenCvSharp.Point(x1, y1), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                            }
                            if (!imgEncryptEnable)
                            {
                                Cv2.ImWrite(local_fullname, srcImg, new ImageEncodingParam(ImwriteFlags.JpegQuality, 70));
                            }
                            else
                            {
                                EncryptMat(srcImg, local_fullname, new ImageEncodingParam(ImwriteFlags.JpegQuality, 70));
                            }
                        }
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (srcImg != null)
                {
                    srcImg.Dispose();
                }
            }
        }
        public void DrawCaptionOpencv(Mat srcImg, string description, string local_fullname, string pos, bool drawborder, calObject calobj, bool drawbarcode = false, string barcodepos = "")
        {
            string[] des = Regex.Split(description, "\r\n");
            int x = 50;
            int y = 150;
            int x1 = -1, y1 = -1, x2 = -1, y2 = -1, x3 = -1, y3 = -1, x4 = -1, y4 = -1;
            int X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0;
            double r11 = calobj.r11;
            double r12 = calobj.r12;
            double r13 = calobj.r13;
            double r14 = calobj.r14;
            double r21 = calobj.r21;
            double r22 = calobj.r22;
            double r23 = calobj.r23;
            double r24 = calobj.r24;
            double r31 = calobj.r31;
            double r32 = calobj.r32;
            double r33 = calobj.r33;
            int xx1 = 0, yy1 = 0, xx2 = 0, yy2 = 0, xx3 = 0, yy3 = 0, xx4 = 0, yy4 = 0;
            bool posValid = false;
            bool barcodeposValid = false;
            try
            {
                if (allowPackageBorder && calobj.calOK && drawborder)
                {
                    string[] posstring = pos.Split(';');
                    if (posstring.Length == 13)
                    {
                        try
                        {
                            X1 = Convert.ToInt32(posstring[1]);
                            Y1 = Convert.ToInt32(posstring[2]);
                            Z1 = Convert.ToInt32(posstring[3]);
                            X2 = Convert.ToInt32(posstring[4]);
                            Y2 = Convert.ToInt32(posstring[5]);
                            Z2 = Convert.ToInt32(posstring[6]);
                            X3 = Convert.ToInt32(posstring[7]);
                            Y3 = Convert.ToInt32(posstring[8]);
                            Z3 = Convert.ToInt32(posstring[9]);
                            X4 = Convert.ToInt32(posstring[10]);
                            Y4 = Convert.ToInt32(posstring[11]);
                            Z4 = Convert.ToInt32(posstring[12]);
                            posValid = true;
                        }
                        catch (Exception ex) { posValid = false; }
                    }
                    else
                    {
                        posValid = false;
                    }
                }
                if (drawbarcode && barcodepos != "")
                { 
                    string[] barcodeposstring = barcodepos.Split(';');
                    if (barcodeposstring.Length == 8)
                    {
                        try
                        {
                            xx1 = Convert.ToInt32(barcodeposstring[0]);
                            yy1 = Convert.ToInt32(barcodeposstring[1]);
                            xx2 = Convert.ToInt32(barcodeposstring[2]);
                            yy2 = Convert.ToInt32(barcodeposstring[3]);
                            xx3 = Convert.ToInt32(barcodeposstring[4]);
                            yy3 = Convert.ToInt32(barcodeposstring[5]);
                            xx4 = Convert.ToInt32(barcodeposstring[6]);
                            yy4 = Convert.ToInt32(barcodeposstring[7]);
                            barcodeposValid = true;
                        }
                        catch (Exception ex) { barcodeposValid = false; }
                    }
                    else
                    {
                        barcodeposValid = false;
                    }
                }
                if (!srcImg.Empty())
                {
                    x = (int)(srcImg.Cols / 80);
                    y = (int)(srcImg.Rows / 20);
                    for (int i = 0; i < des.Length; i++)
                    {
                        if (des[i] != "")
                        {
                            Cv2.PutText(srcImg, des[i], new OpenCvSharp.Point(x, y), HersheyFonts.HersheySimplex, Math.Max(1.0 * srcImg.Rows / 750.0, 1), new Scalar(0, 255, 0), (int)(Math.Max(1.0 * srcImg.Rows / 500.0, 2)));
                        }
                        y += (int)(40.0 * srcImg.Rows / 750.0);
                    }
                    if (posValid && allowPackageBorder && calobj.calOK && drawborder)
                    {
                        x1 = Math.Max(0, Math.Min((int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), srcImg.Cols));
                        y1 = Math.Max(0, Math.Min((int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), srcImg.Rows));
                        x2 = Math.Max(0, Math.Min((int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), srcImg.Cols));
                        y2 = Math.Max(0, Math.Min((int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), srcImg.Rows));
                        x3 = Math.Max(0, Math.Min((int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), srcImg.Cols));
                        y3 = Math.Max(0, Math.Min((int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), srcImg.Rows));
                        x4 = Math.Max(0, Math.Min((int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), srcImg.Cols));
                        y4 = Math.Max(0, Math.Min((int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), srcImg.Rows));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(x1, y1), new OpenCvSharp.Point(x2, y2), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(x2, y2), new OpenCvSharp.Point(x3, y3), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(x3, y3), new OpenCvSharp.Point(x4, y4), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(x4, y4), new OpenCvSharp.Point(x1, y1), new Scalar(0, 255, 0), (int)(srcImg.Cols / 1000));
                    }
                    if (drawbarcode && barcodeposValid && barcodepos != "")
                    {
                        Cv2.Line(srcImg, new OpenCvSharp.Point(xx1, yy1), new OpenCvSharp.Point(xx2, yy2), new Scalar(0, 255, 0), (int)(srcImg.Cols / 500));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(xx2, yy2), new OpenCvSharp.Point(xx3, yy3), new Scalar(0, 255, 0), (int)(srcImg.Cols / 500));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(xx3, yy3), new OpenCvSharp.Point(xx4, yy4), new Scalar(0, 255, 0), (int)(srcImg.Cols / 500));
                        Cv2.Line(srcImg, new OpenCvSharp.Point(xx4, yy4), new OpenCvSharp.Point(xx1, yy1), new Scalar(0, 255, 0), (int)(srcImg.Cols / 500));
                    }
                    if (!imgEncryptEnable)
                    {
                        Cv2.ImWrite(local_fullname, srcImg, new ImageEncodingParam(ImwriteFlags.JpegQuality, 70));
                    }
                    else
                    {
                        EncryptMat(srcImg, local_fullname, new ImageEncodingParam(ImwriteFlags.JpegQuality, 70));
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                
            }
        }
        public void DrawCaptionGDI(FileInfo File, string description, string local_fullname, string pos, bool drawborder, calObject calobj, double parcelHeight, bool isUP)
        {
            Image newImage = null;
            Image image = null;
            Graphics g = null;
            Font font = null;
            SolidBrush sb = null;
            Pen pen = null;
            Font font2 = null;
            SolidBrush sb2 = null;
            Pen pen2 = null;
            int x1 = -1, y1 = -1, x2 = -1, y2 = -1, x3 = -1, y3 = -1, x4 = -1, y4 = -1;
            int X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0;
            double r11 = calobj.r11;
            double r12 = calobj.r12;
            double r13 = calobj.r13;
            double r14 = calobj.r14;
            double r21 = calobj.r21;
            double r22 = calobj.r22;
            double r23 = calobj.r23;
            double r24 = calobj.r24;
            double r31 = calobj.r31;
            double r32 = calobj.r32;
            double r33 = calobj.r33;
            bool posValid = false;
            try
            {
                if (allowPackageBorder && calobj.calOK && drawborder)
                {
                    string[] posstring = pos.Split(';');
                    if (posstring.Length == 13)
                    {
                        try
                        {
                            X1 = Convert.ToInt32(posstring[1]);
                            Y1 = Convert.ToInt32(posstring[2]);
                            Z1 = Convert.ToInt32(posstring[3]);
                            X2 = Convert.ToInt32(posstring[4]);
                            Y2 = Convert.ToInt32(posstring[5]);
                            Z2 = Convert.ToInt32(posstring[6]);
                            X3 = Convert.ToInt32(posstring[7]);
                            Y3 = Convert.ToInt32(posstring[8]);
                            Z3 = Convert.ToInt32(posstring[9]);
                            X4 = Convert.ToInt32(posstring[10]);
                            Y4 = Convert.ToInt32(posstring[11]);
                            Z4 = Convert.ToInt32(posstring[12]);
                            posValid = true;
                        }
                        catch (Exception ex) { posValid = false; }
                    }
                    else
                    {
                        posValid = false;
                    }
                }
                
                sb = new SolidBrush(Color.FromArgb(255, 0, 255, 0));
                sb2 = new SolidBrush(Color.White);
                if (File is FileInfo)
                {
                    File.Refresh();
                    if ((File.Extension.ToLower() == ".bmp" || File.Extension.ToLower() == ".jpg") && File.Exists)
                    {
                        image = System.Drawing.Image.FromFile(File.FullName, false);
                        newImage = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
                        g = Graphics.FromImage(newImage);     
                        g.DrawImage(image, 0, 0, image.Width, image.Height);
                        PointF p = new PointF(image.Width/80, image.Height/60);
                        font = new Font("宋体", image.Width/50);
                        font2 = new Font("宋体", image.Width / 80);
                        pen2 = new Pen(sb2, image.Width / 1000);
                        if (isUP && GridLineEnable && parcelHeight > 0 && parcelHeight < MountingHeight)
                        {
                            
                            GridLine(g, image.Width, image.Height, parcelHeight, 20, font2, sb2, pen2);
                            
                        }
                        if (description != "")
                        {
                            g.DrawString(description, font, sb, p);
                        }
                        if (posValid && allowPackageBorder && calobj.calOK && drawborder)
                        {
                            pen = new Pen(sb, image.Width/1000);
                            
                            x1 = Math.Max(0, Math.Min((int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), image.Width));
                            y1 = Math.Max(0, Math.Min((int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), image.Height));
                            x2 = Math.Max(0, Math.Min((int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), image.Width));
                            y2 = Math.Max(0, Math.Min((int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), image.Height));
                            x3 = Math.Max(0, Math.Min((int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), image.Width));
                            y3 = Math.Max(0, Math.Min((int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), image.Height));
                            x4 = Math.Max(0, Math.Min((int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), image.Width));
                            y4 = Math.Max(0, Math.Min((int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), image.Height));
                            g.DrawLine(pen, x1, y1, x2, y2);
                            g.DrawLine(pen, x2, y2, x3, y3);
                            g.DrawLine(pen, x3, y3, x4, y4);
                            g.DrawLine(pen, x4, y4, x1, y1);
                        }
                        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                        g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                        if (!imgEncryptEnable)
                        {
                            newImage.Save(local_fullname, ImageCodecInfo.GetImageEncoders().FirstOrDefault(j => j.MimeType == "image/jpeg"), ep);
                        }
                        else
                        {
                            EncryptImage(newImage, local_fullname, ImageCodecInfo.GetImageEncoders().FirstOrDefault(j => j.MimeType == "image/jpeg"), ep);
                        }
                    }
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (newImage != null)
                {
                    newImage.Dispose();
                }
                if (font != null)
                {
                    font.Dispose();
                }
                if (sb != null)
                {
                    sb.Dispose();
                }
                if (g != null)
                {
                    g.Dispose();
                }
                if (image != null)
                {
                    image.Dispose();
                }
                if (pen != null)
                {
                    pen.Dispose();
                }
                if (font2 != null)
                {
                    font2.Dispose();
                }
                if (sb2 != null)
                {
                    sb2.Dispose();
                }
                if (pen2 != null)
                {
                    pen2.Dispose();
                }
            }
        }
        public void DrawCaptionGDI(Mat SrcImg, string description, string local_fullname, string pos, bool drawborder, calObject calobj, bool drawbarcode = false, string barcodepos = "")
        {
            MemoryStream ms = null;
            Image newImage = null;
            Graphics g = null;
            Font font = null;
            SolidBrush redsb = null;
            Pen redpen = null;
            SolidBrush greensb = null;
            Pen greenpen = null;
            int x1 = -1, y1 = -1, x2 = -1, y2 = -1, x3 = -1, y3 = -1, x4 = -1, y4 = -1;
            int X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0;
            double r11 = calobj.r11;
            double r12 = calobj.r12;
            double r13 = calobj.r13;
            double r14 = calobj.r14;
            double r21 = calobj.r21;
            double r22 = calobj.r22;
            double r23 = calobj.r23;
            double r24 = calobj.r24;
            double r31 = calobj.r31;
            double r32 = calobj.r32;
            double r33 = calobj.r33;
            int xx1 = 0, yy1 = 0, xx2 = 0, yy2 = 0, xx3 = 0, yy3 = 0, xx4 = 0, yy4 = 0;
            bool posValid = false;
            bool barcodeposValid = false;
            try
            {
                if (allowPackageBorder && calobj.calOK && drawborder)
                {
                    string[] posstring = pos.Split(';');
                    if (posstring.Length == 13)
                    {
                        try
                        {
                            X1 = Convert.ToInt32(posstring[1]);
                            Y1 = Convert.ToInt32(posstring[2]);
                            Z1 = Convert.ToInt32(posstring[3]);
                            X2 = Convert.ToInt32(posstring[4]);
                            Y2 = Convert.ToInt32(posstring[5]);
                            Z2 = Convert.ToInt32(posstring[6]);
                            X3 = Convert.ToInt32(posstring[7]);
                            Y3 = Convert.ToInt32(posstring[8]);
                            Z3 = Convert.ToInt32(posstring[9]);
                            X4 = Convert.ToInt32(posstring[10]);
                            Y4 = Convert.ToInt32(posstring[11]);
                            Z4 = Convert.ToInt32(posstring[12]);
                            posValid = true;
                        }
                        catch (Exception ex) { posValid = false; }
                    }
                    else
                    {
                        posValid = false;
                    }
                }
                if (drawbarcode && barcodepos != "")
                {
                    string[] barcodeposstring = barcodepos.Split(';');
                    if (barcodeposstring.Length == 8)
                    {
                        try
                        {
                            xx1 = Convert.ToInt32(barcodeposstring[0]);
                            yy1 = Convert.ToInt32(barcodeposstring[1]);
                            xx2 = Convert.ToInt32(barcodeposstring[2]);
                            yy2 = Convert.ToInt32(barcodeposstring[3]);
                            xx3 = Convert.ToInt32(barcodeposstring[4]);
                            yy3 = Convert.ToInt32(barcodeposstring[5]);
                            xx4 = Convert.ToInt32(barcodeposstring[6]);
                            yy4 = Convert.ToInt32(barcodeposstring[7]);
                            barcodeposValid = true;
                        }
                        catch (Exception ex) { barcodeposValid = false; }
                    }
                    else
                    {
                        barcodeposValid = false;
                    }
                }
                redsb = new SolidBrush(Color.FromArgb(255, 255, 0, 0));
                greensb = new SolidBrush(Color.FromArgb(255, 0, 255, 0));
                ms = SrcImg.ToMemoryStream();
                newImage = Image.FromStream(ms);
                g = Graphics.FromImage(newImage);
                PointF p = new PointF(newImage.Width / 80, newImage.Height / 60);
                font = new Font("宋体", newImage.Width / 50);
                redpen = new Pen(redsb, newImage.Width / 1000);
                greenpen = new Pen(greensb, newImage.Width / 500);
                if (description != "")
                {
                    g.DrawString(description, font, greensb, p);
                }
                if (posValid && allowPackageBorder && calobj.calOK && drawborder)
                {
                    x1 = Math.Max(0, Math.Min((int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), newImage.Width));
                    y1 = Math.Max(0, Math.Min((int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1)), newImage.Height));
                    x2 = Math.Max(0, Math.Min((int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), newImage.Width));
                    y2 = Math.Max(0, Math.Min((int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1)), newImage.Height));
                    x3 = Math.Max(0, Math.Min((int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), newImage.Width));
                    y3 = Math.Max(0, Math.Min((int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1)), newImage.Height));
                    x4 = Math.Max(0, Math.Min((int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), newImage.Width));
                    y4 = Math.Max(0, Math.Min((int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1)), newImage.Height));
                    g.DrawLine(greenpen, x1, y1, x2, y2);
                    g.DrawLine(greenpen, x2, y2, x3, y3);
                    g.DrawLine(greenpen, x3, y3, x4, y4);
                    g.DrawLine(greenpen, x4, y4, x1, y1);
                }
                if (drawbarcode && barcodeposValid && barcodepos != "")
                {
                    
                    g.DrawLine(greenpen, xx1, yy1, xx2, yy2);
                    g.DrawLine(greenpen, xx2, yy2, xx3, yy3);
                    g.DrawLine(greenpen, xx3, yy3, xx4, yy4);
                    g.DrawLine(greenpen, xx4, yy4, xx1, yy1);
                }
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
                g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
                if (!imgEncryptEnable)
                {
                    newImage.Save(local_fullname, ImageCodecInfo.GetImageEncoders().FirstOrDefault(j => j.MimeType == "image/jpeg"), ep);
                }
                else
                {
                    EncryptImage(newImage, local_fullname, ImageCodecInfo.GetImageEncoders().FirstOrDefault(j => j.MimeType == "image/jpeg"), ep);
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (ms != null)
                {
                    ms.Dispose();
                }
                if (newImage != null)
                {
                    newImage.Dispose();
                }
                if (font != null)
                {
                    font.Dispose();
                }
                if (redsb != null)
                {
                    redsb.Dispose();
                }
                if (greensb != null)
                {
                    greensb.Dispose();
                }
                if (g != null)
                {
                    g.Dispose();
                }
                if (redpen != null)
                {
                    redpen.Dispose();
                }
                if (greenpen != null)
                {
                    greenpen.Dispose();
                }
            }
        }
        /*public void AddCaptionInThreadMethod(object folderobj)
        {
            AddCaptionInThreads(folderobj);
        }*/
        /*
        public void AddCaptionInThreads(object folderobj, bool ManualEnable = false)
        {
            string folder = folderobj.ToString();
            while (true)
            {
                try
                {
                    if (captionEnable || ManualEnable)
                    {
                        //var ep = new EncoderParameters(1);
                        //ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L);
                        if (!Directory.Exists(folder))
                        {
                            Directory.CreateDirectory(folder);
                        }
                        DirectoryInfo directoryinfo = new DirectoryInfo(folder);
                        FileInfo[] Files = directoryinfo.GetFiles();
                        string ftpFolder = ftp_temp_url1;
                        string logcontent = "";
                        ArrayList LogContentArray = null;
                        CogImageFileTool mImageFileTool = mImageFileTool1;
                        if (folder == caption_url1)
                        {
                            ftpFolder = ftp_temp_url1;
                            mImageFileTool = mImageFileTool1;
                        }
                        else if (folder == caption_url2)
                        {
                            ftpFolder = ftp_temp_url2;
                            mImageFileTool = mImageFileTool2;
                        }
                        else if (folder == caption_url3)
                        {
                            ftpFolder = ftp_temp_url3;
                            mImageFileTool = mImageFileTool3;
                        }
                        else if (folder == caption_url4)
                        {
                            ftpFolder = ftp_temp_url4;
                            mImageFileTool = mImageFileTool4;
                        }
                        else if (folder == caption_url5)
                        {
                            ftpFolder = ftp_temp_url5;
                            mImageFileTool = mImageFileTool5;
                        }
                        else
                        {
                            ftpFolder = ftp_temp_url1;
                            mImageFileTool = mImageFileTool1;
                        }
                        foreach (FileInfo file in Files)
                        {
                            try
                            {
                                if ((DateTime.Now - file.CreationTime).TotalMilliseconds >= 800)
                                {
                                    MasterImage master = null;
                                    FileInfo fileInfo = null;//加完水印的图片
                                    bool drawOK = false;
                                    string StorageRoot = StorageRoot1;
                                    ArrayList HttpArray = HTTPArraySta1_1; ;
                                    Object HttpArrayLock = HTTPArraySta1_1Lock;
                                    Object HttpInfoLock = HTTPInfoSta1_1Lock;
                                    string N0 = "0";
                                    string N1 = "xx";
                                    string N2 = "xx";
                                    string N3 = "xx";
                                    string N4 = "xx";
                                    string N5 = "xx";
                                    string N6 = "xx";
                                    string N7 = "xx";
                                    string N8 = "xx";
                                    string N9 = "xx";
                                    string N10 = "xx";
                                    string N11 = "xx";
                                    string N12 = "1";
                                    string N13 = "";//外部水印信息
                                    string extension = file.Extension;
                                    int N0pos;
                                    int N1pos;
                                    int N2pos;
                                    int N3pos;
                                    int N4pos;
                                    int N5pos;
                                    int N6pos;
                                    int N7pos;
                                    int N8pos;
                                    int N9pos;
                                    int N10pos;
                                    int N11pos;
                                    int N12pos;//StationNo.
                                    int N13pos;
                                    int extpos;
                                    DateTime Timestamp = file.CreationTime;
                                    DateTime TriggerTimestamp = file.CreationTime;
                                    ArrayList name = new ArrayList();
                                    calObject calobj = cal1_1;
                                    if ((N0pos = file.Name.IndexOf("_0,")) > -1) { name.Add(N0pos); };
                                    if ((N1pos = file.Name.IndexOf("_1,")) > -1) { name.Add(N1pos); };
                                    if ((N2pos = file.Name.IndexOf("_2,")) > -1) { name.Add(N2pos); };
                                    if ((N3pos = file.Name.IndexOf("_3,")) > -1) { name.Add(N3pos); };
                                    if ((N4pos = file.Name.IndexOf("_4,")) > -1) { name.Add(N4pos); };
                                    if ((N5pos = file.Name.IndexOf("_5,")) > -1) { name.Add(N5pos); };
                                    if ((N6pos = file.Name.IndexOf("_6,")) > -1) { name.Add(N6pos); };
                                    if ((N7pos = file.Name.IndexOf("_7,")) > -1) { name.Add(N7pos); };
                                    if ((N8pos = file.Name.IndexOf("_8,")) > -1) { name.Add(N8pos); };
                                    if ((N9pos = file.Name.IndexOf("_9,")) > -1) { name.Add(N9pos); };
                                    if ((N10pos = file.Name.IndexOf("_10,")) > -1) { name.Add(N10pos); };
                                    if ((N11pos = file.Name.IndexOf("_11,")) > -1) { name.Add(N11pos); };
                                    if ((N12pos = file.Name.IndexOf("_12,")) > -1) { name.Add(N12pos); };
                                    if ((N13pos = file.Name.IndexOf("_13,")) > -1) { name.Add(N13pos); };
                                    if ((extpos = file.Name.LastIndexOf(file.Extension)) > -1) { name.Add(extpos); };
                                    name.Sort();
                                    if (name.IndexOf(N0pos) < name.Count - 1 && (name[name.IndexOf(N0pos) + 1] is int) && name.IndexOf(N0pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N0pos) + 1];
                                        N0 = file.Name.Substring(N0pos + 3, next - N0pos - 3);
                                    }
                                    if (name.IndexOf(N1pos) < name.Count - 1 && (name[name.IndexOf(N1pos) + 1] is int) && name.IndexOf(N1pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N1pos) + 1];
                                        N1 = file.Name.Substring(N1pos + 3, next - N1pos - 3);
                                    }
                                    if (name.IndexOf(N2pos) < name.Count - 1 && (name[name.IndexOf(N2pos) + 1] is int) && name.IndexOf(N2pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N2pos) + 1];
                                        N2 = file.Name.Substring(N2pos + 3, next - N2pos - 3);
                                    }
                                    if (name.IndexOf(N3pos) < name.Count - 1 && (name[name.IndexOf(N3pos) + 1] is int) && name.IndexOf(N3pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N3pos) + 1];
                                        N3 = file.Name.Substring(N3pos + 3, next - N3pos - 3);
                                    }
                                    if (name.IndexOf(N4pos) < name.Count - 1 && (name[name.IndexOf(N4pos) + 1] is int) && name.IndexOf(N4pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N4pos) + 1];
                                        N4 = file.Name.Substring(N4pos + 3, next - N4pos - 3);
                                    }
                                    if (name.IndexOf(N5pos) < name.Count - 1 && (name[name.IndexOf(N5pos) + 1] is int) && name.IndexOf(N5pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N5pos) + 1];
                                        N5 = file.Name.Substring(N5pos + 3, next - N5pos - 3);
                                    }
                                    if (name.IndexOf(N6pos) < name.Count - 1 && (name[name.IndexOf(N6pos) + 1] is int) && name.IndexOf(N6pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N6pos) + 1];
                                        N6 = file.Name.Substring(N6pos + 3, next - N6pos - 3);
                                    }
                                    if (name.IndexOf(N7pos) < name.Count - 1 && (name[name.IndexOf(N7pos) + 1] is int) && name.IndexOf(N7pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N7pos) + 1];
                                        N7 = file.Name.Substring(N7pos + 3, next - N7pos - 3);
                                    }
                                    if (name.IndexOf(N8pos) < name.Count - 1 && (name[name.IndexOf(N8pos) + 1] is int) && name.IndexOf(N8pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N8pos) + 1];
                                        N8 = file.Name.Substring(N8pos + 3, next - N8pos - 3);
                                    }
                                    if (name.IndexOf(N9pos) < name.Count - 1 && (name[name.IndexOf(N9pos) + 1] is int) && name.IndexOf(N9pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N9pos) + 1];
                                        N9 = file.Name.Substring(N9pos + 3, next - N9pos - 3);
                                    }
                                    if (name.IndexOf(N10pos) < name.Count - 1 && (name[name.IndexOf(N10pos) + 1] is int) && name.IndexOf(N10pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N10pos) + 1];
                                        N10 = file.Name.Substring(N10pos + 4, next - N10pos - 4);
                                    }
                                    if (name.IndexOf(N11pos) < name.Count - 1 && (name[name.IndexOf(N11pos) + 1] is int) && name.IndexOf(N11pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N11pos) + 1];
                                        N11 = file.Name.Substring(N11pos + 4, next - N11pos - 4);
                                    }
                                    if (name.IndexOf(N12pos) < name.Count - 1 && (name[name.IndexOf(N12pos) + 1] is int) && name.IndexOf(N12pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N12pos) + 1];
                                        N12 = file.Name.Substring(N12pos + 4, next - N12pos - 4);
                                    }
                                    if (name.IndexOf(N13pos) < name.Count - 1 && (name[name.IndexOf(N13pos) + 1] is int) && name.IndexOf(N13pos) > -1)
                                    {
                                        int next = (int)name[name.IndexOf(N13pos) + 1];
                                        N13 = file.Name.Substring(N13pos + 4, next - N13pos - 4);
                                    }

                                    string ext0 = "";
                                    string ext1 = "";
                                    string ext2 = "";
                                    string ext3 = "";
                                    string ext4 = "";
                                    string ext5 = "";
                                    string ext6 = "";
                                    string ext7 = "";
                                    string ext8 = "";
                                    string ext9 = "";

                                    if (N13 != null)
                                    {
                                        string[] items = N13.Split(';');
                                        if (items.Length >= 1)
                                        {
                                            ext0 = items[0];
                                        }
                                        if (items.Length >= 2)
                                        {
                                            ext1 = items[1];
                                        }
                                        if (items.Length >= 3)
                                        {
                                            ext2 = items[2];
                                        }
                                        if (items.Length >= 4)
                                        {
                                            ext3 = items[3];
                                        }
                                        if (items.Length >= 5)
                                        {
                                            ext4 = items[4];
                                        }
                                        if (items.Length >= 6)
                                        {
                                            ext5 = items[5];
                                        }
                                        if (items.Length >= 7)
                                        {
                                            ext6 = items[6];
                                        }
                                        if (items.Length >= 8)
                                        {
                                            ext7 = items[7];
                                        }
                                        if (items.Length >= 9)
                                        {
                                            ext8 = items[8];
                                        }
                                        if (items.Length >= 10)
                                        {
                                            ext9 = items[9];
                                        }
                                    }

                                    string timestamptext = SorterTimeStampFormat;
                                    timestamptext = timestamptext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    timestamptext = timestamptext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    timestamptext = timestamptext.Replace("#None", "");
                                    if (timestamptext != "" && timestamptext != "xx")
                                    {
                                        try
                                        {
                                            TriggerTimestamp = DateTime.ParseExact(timestamptext, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                                        }
                                        catch (Exception ex)
                                        {
                                            TriggerTimestamp = file.CreationTime;
                                        }
                                    }
                                    string pos = "";
                                    pos = PosFormat;
                                    pos = pos.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    pos = pos.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    pos = pos.Replace("#None", "");
                                    int dimNo = 1;
                                    string[] posstring = pos.Split(';');
                                    if (posstring.Length == 13)
                                    {
                                        if (posstring[0] == "0")
                                        {
                                            dimNo = 1;
                                        }
                                        else if (posstring[0] == "1")
                                        {
                                            dimNo = 2;
                                        }
                                    }
                                    switch (N12)
                                    {
                                        case "1":
                                            StorageRoot = StorageRoot1;
                                            LogContentArray = LogContentArray1;
                                            if (dimNo == 1)
                                            {
                                                calobj = cal1_1;
                                            }
                                            else
                                            {
                                                calobj = cal1_2;
                                            }
                                            if (Convert.ToInt64(N0) % 2 == 0)
                                            {
                                                HttpArray = HTTPArraySta1_1;
                                                HttpArrayLock = HTTPArraySta1_1Lock;
                                                HttpInfoLock = HTTPInfoSta1_1Lock;
                                            }
                                            else
                                            {
                                                HttpArray = HTTPArraySta1_2;
                                                HttpArrayLock = HTTPArraySta1_2Lock;
                                                HttpInfoLock = HTTPInfoSta1_2Lock;
                                            }
                                            break;
                                        case "2":
                                            StorageRoot = StorageRoot2;
                                            LogContentArray = LogContentArray2;
                                            if (dimNo == 1)
                                            {
                                                calobj = cal2_1;
                                            }
                                            else
                                            {
                                                calobj = cal2_2;
                                            }
                                            if (Convert.ToInt64(N0) % 2 == 0)
                                            {
                                                HttpArray = HTTPArraySta2_1;
                                                HttpArrayLock = HTTPArraySta2_1Lock;
                                                HttpInfoLock = HTTPInfoSta2_1Lock;
                                            }
                                            else
                                            {
                                                HttpArray = HTTPArraySta2_2;
                                                HttpArrayLock = HTTPArraySta2_2Lock;
                                                HttpInfoLock = HTTPInfoSta2_2Lock;
                                            }
                                            break;
                                        case "3":
                                            StorageRoot = StorageRoot3;
                                            LogContentArray = LogContentArray3;
                                            if (dimNo == 1)
                                            {
                                                calobj = cal3_1;
                                            }
                                            else
                                            {
                                                calobj = cal3_2;
                                            }
                                            if (Convert.ToInt64(N0) % 2 == 0)
                                            {
                                                HttpArray = HTTPArraySta3_1;
                                                HttpArrayLock = HTTPArraySta3_1Lock;
                                                HttpInfoLock = HTTPInfoSta3_1Lock;
                                            }
                                            else
                                            {
                                                HttpArray = HTTPArraySta3_2;
                                                HttpArrayLock = HTTPArraySta3_2Lock;
                                                HttpInfoLock = HTTPInfoSta3_2Lock;
                                            }
                                            break;
                                        case "4":
                                            StorageRoot = StorageRoot4;
                                            LogContentArray = LogContentArray4;
                                            if (dimNo == 1)
                                            {
                                                calobj = cal4_1;
                                            }
                                            else
                                            {
                                                calobj = cal4_2;
                                            }
                                            if (Convert.ToInt64(N0) % 2 == 0)
                                            {
                                                HttpArray = HTTPArraySta4_1;
                                                HttpArrayLock = HTTPArraySta4_1Lock;
                                                HttpInfoLock = HTTPInfoSta4_1Lock;
                                            }
                                            else
                                            {
                                                HttpArray = HTTPArraySta4_2;
                                                HttpArrayLock = HTTPArraySta4_2Lock;
                                                HttpInfoLock = HTTPInfoSta4_2Lock;
                                            }
                                            break;
                                        default:
                                            StorageRoot = StorageRoot1;
                                            LogContentArray = LogContentArray1;
                                            if (dimNo == 1)
                                            {
                                                calobj = cal1_1;
                                            }
                                            else
                                            {
                                                calobj = cal1_2;
                                            }
                                            HttpArray = HTTPArraySta1_1;
                                            HttpArrayLock = HTTPArraySta1_1Lock;
                                            HttpInfoLock = HTTPInfoSta1_1Lock;
                                            break;
                                    }
                                    string Year = TriggerTimestamp.ToString("yyyy");
                                    string Month = TriggerTimestamp.ToString("MM");
                                    string Day = TriggerTimestamp.ToString("dd");
                                    string Hour = TriggerTimestamp.ToString("HH");
                                    string Minute = TriggerTimestamp.ToString("mm");
                                    string Second = TriggerTimestamp.ToString("ss");
                                    string Millisecond = TriggerTimestamp.ToString("fff");
                                    string DirName = string.Join(@"\", SubFolders) + "\\";
                                    string FullDir = "";
                                    string FileName = FileNameFormat + extension;
                                    string FtpFileName = FtpFileNameFormat + extension;
                                    //string HttpFileName = HttpFileNameFormat + extension;
                                    Regex MultiSlash = new Regex("\\\\{2,}");
                                    Regex MultiSpace = new Regex("");
                                    Regex Multibar = new Regex("~{2,}");
                                    Regex Startbar = new Regex("^~");
                                    Regex Endbar = new Regex("~$");

                                    FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                    FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");

                                    DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                    DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");

                                    FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                    FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");

                                    string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                                    temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                    temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");


                                    string result = SorterResultFormat;
                                    result = result.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    result = result.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    result = result.Replace("#None", "GR");
                                    string description = "";
                                    description = CaptionFormat;
                                    description = description.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                                    description = description.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                                    description = description.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                                    description = description.Replace("#EXT0", ext0).Replace("#EXT1", ext1).Replace("#EXT2", ext2).Replace("#EXT3", ext3);
                                    description = description.Replace("#EXT4", ext4).Replace("#EXT5", ext5).Replace("#EXT6", ext6).Replace("#EXT7", ext7);
                                    description = description.Replace("#EXT8", ext8).Replace("#EXT9", ext9);
                                    if (NoReadAnalysisEnable)
                                    {
                                        string NoReadReason = "N/A";
                                        if (result == "NR")
                                        {
                                            //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件: " + file.FullName + " Starting No Read Analysis!";
                                            //AddLog(logcontent, LogContentArray);
                                            NoReadReason = NoReadAnalyzeFromFile(file, mImageFileTool, LogContentArray, file.FullName);
                                            //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件: " + file.FullName + " Finished No Read Analysis!";
                                            //AddLog(logcontent, LogContentArray);
                                        }
                                        description = description.Replace("#ANA", NoReadReason);
                                        string NoReadReasonPath = NoReadReason.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                        if (NoReadReason != "N/A")
                                        {
                                            FtpFileName = FtpFileName.Replace("#ANA", NoReadReasonPath);
                                            temp_filename_header = temp_filename_header.Replace("#ANA", NoReadReasonPath);
                                            DirName = DirName.Replace("#ANA", NoReadReasonPath);
                                            FileName = FileName.Replace("#ANA", NoReadReasonPath);
                                        }
                                        else
                                        {
                                            FtpFileName = FtpFileName.Replace("#ANA", "NA");
                                            temp_filename_header = temp_filename_header.Replace("#ANA", "");
                                            DirName = DirName.Replace("#ANA", "");
                                            FileName = FileName.Replace("#ANA", "NA");
                                        }
                                    }
                                    else
                                    {
                                        description = description.Replace("#ANA", "N/A");
                                        FtpFileName = FtpFileName.Replace("#ANA", "NA");
                                        temp_filename_header = temp_filename_header.Replace("#ANA", "");
                                        DirName = DirName.Replace("#ANA", "");
                                        FileName = FileName.Replace("#ANA", "NA");

                                    }
                                    temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                                    temp_filename_header = Startbar.Replace(temp_filename_header, "");
                                    temp_filename_header = Endbar.Replace(temp_filename_header, "");
                                    FullDir = StorageRoot + "\\" + DirName;
                                    FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                                    FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                                    string local_fullname = FullDir + FileName;//本地文件全名含路径
                                    string ftp_fullname = ftpFolder + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                                    if (!Directory.Exists(ftpFolder))
                                    {
                                        Directory.CreateDirectory(ftpFolder);
                                    }
                                    if (!Directory.Exists(FullDir))
                                    {
                                        Directory.CreateDirectory(FullDir);
                                    }
                                    if (!System.IO.File.Exists(local_fullname))
                                    {
                                        Stopwatch sw = new Stopwatch();
                                        sw.Start();
                                        bool drawborder = false;
                                        if (N1.StartsWith("UP"))
                                        {
                                            drawborder = true;
                                        }
                                        else
                                        {
                                            drawborder = false;
                                        }
                                        try
                                        {
                                            if (allowChinese)
                                            {
                                                DrawCaptionGDI(file, description, local_fullname, pos, drawborder, calobj);
                                            }
                                            else
                                            {
                                                DrawCaptionOpencv(file, description, local_fullname, pos, drawborder, calobj);
                                            }
                                            if (System.IO.File.Exists(local_fullname))
                                            {
                                                fileInfo = new FileInfo(local_fullname);
                                                if (upload)
                                                {
                                                    fileInfo.CopyTo(ftp_fullname, false);
                                                }
                                            }
                                            sw.Stop();
                                            logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件名: " + FileName + "水印: " + sw.ElapsedMilliseconds + "ms";
                                            AddLog(logcontent, LogContentArray);
                                        }
                                        catch (Exception ex)
                                        {
                                            string s = ex.StackTrace;
                                            string str = ex.Message;
                                        }
                                    }
                                    if (System.IO.File.Exists(local_fullname))
                                    {
                                        try
                                        {
                                            file.Delete();
                                        }
                                        catch (Exception ex) { }
                                    }
                                    if (httpUpload)
                                    {
                                        if (N1.StartsWith("UP") && fileInfo != null)
                                        {
                                            int MstIndex = -1;
                                            lock (HttpArrayLock)
                                            {
                                                MstIndex = FindMstIndex(N0, HttpArray);
                                                if (MstIndex > -1)
                                                {
                                                    master = HttpArray[MstIndex] as MasterImage;
                                                }
                                            }
                                            if (MstIndex < 0)
                                            {
                                                master = new MasterImage(N0, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, Timestamp, TriggerTimestamp);
                                                lock (HttpArrayLock)
                                                {
                                                    HttpArray.Add(master);
                                                }
                                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " added to httpArray! " + file.Name + " is the current file!";
                                                AddLog(logcontent, LogContentArray);
                                            }
                                            if (master != null && fileInfo != null && master.Images.Count == 0)
                                            {
                                                lock (HttpInfoLock)
                                                {
                                                    master.Images.Add(fileInfo);
                                                    master.ImageTime.Add(TriggerTimestamp);
                                                    master.BcrName.Add(N1);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex) { }
                        }
                    }
                }
                catch { }
                Thread.Sleep(1000);
            }
        }
        */
       
        public void AddCaption(string folder)
        {
            //var ep = new EncoderParameters(1);
            //ep.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 70L);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            DirectoryInfo directoryinfo = new DirectoryInfo(folder);
            FileInfo[] Files = directoryinfo.GetFiles();
            string ftpFolder = ftp_temp_url1;
            string logcontent = "";
            ArrayList LogContentArray = null;
#if FEATURE_VPRO
            CogImageFileTool mImageFileTool = mImageFileTool1;
            if (folder == caption_url1)
            {
                ftpFolder = ftp_temp_url1;
                mImageFileTool = mImageFileTool1;
            }
            else if (folder == caption_url2)
            {
                ftpFolder = ftp_temp_url2;
                mImageFileTool = mImageFileTool2;
            }
            else if (folder == caption_url3)
            {
                ftpFolder = ftp_temp_url3;
                mImageFileTool = mImageFileTool3;
            }
            else if (folder == caption_url4)
            {
                ftpFolder = ftp_temp_url4;
                mImageFileTool = mImageFileTool4;
            }
            else if (folder == caption_url5)
            {
                ftpFolder = ftp_temp_url5;
                mImageFileTool = mImageFileTool5;
            }
            else
            {
                ftpFolder = ftp_temp_url1;
                mImageFileTool = mImageFileTool1;
            }
#endif
            foreach (FileInfo file in Files)
            {
                /*
                if (ftpFolder == ftp_temp_url1)
                {
                    ftpFolder = ftp_temp_url2;
                }
                else 
                {
                    ftpFolder = ftp_temp_url1;
                }
                */
                try
                {
                    if ((DateTime.Now - file.CreationTime).TotalMilliseconds >= 300)
                    {
                        MasterImage master = null;
                        FileInfo fileInfo = null;//加完水印的图片
                        bool drawOK = false;
                        string StorageRoot = StorageRoot1;
                        ArrayList HttpArray = HTTPArraySta1_1; ;
                        Object HttpArrayLock = HTTPArraySta1_1Lock;
                        Object HttpInfoLock = HTTPInfoSta1_1Lock;
                        string N0 = "0";
                        string N1 = "xx";
                        string N2 = "xx";
                        string N3 = "xx";
                        string N4 = "xx";
                        string N5 = "xx";
                        string N6 = "xx";
                        string N7 = "xx";
                        string N8 = "xx";
                        string N9 = "xx";
                        string N10 = "xx";
                        string N11 = "xx";
                        string N12 = "1";
                        string N13 = "";//外部水印信息
                        string extension = file.Extension;
                        int N0pos;
                        int N1pos;
                        int N2pos;
                        int N3pos;
                        int N4pos;
                        int N5pos;
                        int N6pos;
                        int N7pos;
                        int N8pos;
                        int N9pos;
                        int N10pos;
                        int N11pos;
                        int N12pos;//StationNo.
                        int N13pos;
                        int extpos;
                        DateTime Timestamp = file.CreationTime;
                        DateTime TriggerTimestamp = file.CreationTime;
                        ArrayList name = new ArrayList();
                        calObject calobj = cal1_1;
                        if ((N0pos = file.Name.IndexOf("_0,")) > -1) { name.Add(N0pos); };
                        if ((N1pos = file.Name.IndexOf("_1,")) > -1) { name.Add(N1pos); };
                        if ((N2pos = file.Name.IndexOf("_2,")) > -1) { name.Add(N2pos); };
                        if ((N3pos = file.Name.IndexOf("_3,")) > -1) { name.Add(N3pos); };
                        if ((N4pos = file.Name.IndexOf("_4,")) > -1) { name.Add(N4pos); };
                        if ((N5pos = file.Name.IndexOf("_5,")) > -1) { name.Add(N5pos); };
                        if ((N6pos = file.Name.IndexOf("_6,")) > -1) { name.Add(N6pos); };
                        if ((N7pos = file.Name.IndexOf("_7,")) > -1) { name.Add(N7pos); };
                        if ((N8pos = file.Name.IndexOf("_8,")) > -1) { name.Add(N8pos); };
                        if ((N9pos = file.Name.IndexOf("_9,")) > -1) { name.Add(N9pos); };
                        if ((N10pos = file.Name.IndexOf("_10,")) > -1) { name.Add(N10pos); };
                        if ((N11pos = file.Name.IndexOf("_11,")) > -1) { name.Add(N11pos); };
                        if ((N12pos = file.Name.IndexOf("_12,")) > -1) { name.Add(N12pos); };
                        if ((N13pos = file.Name.IndexOf("_13,")) > -1) { name.Add(N13pos); };
                        if ((extpos = file.Name.LastIndexOf(file.Extension)) > -1) { name.Add(extpos); };
                        name.Sort();
                        if (name.IndexOf(N0pos) < name.Count - 1 && (name[name.IndexOf(N0pos) + 1] is int) && name.IndexOf(N0pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N0pos) + 1];
                            N0 = file.Name.Substring(N0pos + 3, next - N0pos - 3);
                        }
                        if (name.IndexOf(N1pos) < name.Count - 1 && (name[name.IndexOf(N1pos) + 1] is int) && name.IndexOf(N1pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N1pos) + 1];
                            N1 = file.Name.Substring(N1pos + 3, next - N1pos - 3);
                        }
                        if (name.IndexOf(N2pos) < name.Count - 1 && (name[name.IndexOf(N2pos) + 1] is int) && name.IndexOf(N2pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N2pos) + 1];
                            N2 = file.Name.Substring(N2pos + 3, next - N2pos - 3);
                        }
                        if (name.IndexOf(N3pos) < name.Count - 1 && (name[name.IndexOf(N3pos) + 1] is int) && name.IndexOf(N3pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N3pos) + 1];
                            N3 = file.Name.Substring(N3pos + 3, next - N3pos - 3);
                        }
                        if (name.IndexOf(N4pos) < name.Count - 1 && (name[name.IndexOf(N4pos) + 1] is int) && name.IndexOf(N4pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N4pos) + 1];
                            N4 = file.Name.Substring(N4pos + 3, next - N4pos - 3);
                        }
                        if (name.IndexOf(N5pos) < name.Count - 1 && (name[name.IndexOf(N5pos) + 1] is int) && name.IndexOf(N5pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N5pos) + 1];
                            N5 = file.Name.Substring(N5pos + 3, next - N5pos - 3);
                        }
                        if (name.IndexOf(N6pos) < name.Count - 1 && (name[name.IndexOf(N6pos) + 1] is int) && name.IndexOf(N6pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N6pos) + 1];
                            N6 = file.Name.Substring(N6pos + 3, next - N6pos - 3);
                        }
                        if (name.IndexOf(N7pos) < name.Count - 1 && (name[name.IndexOf(N7pos) + 1] is int) && name.IndexOf(N7pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N7pos) + 1];
                            N7 = file.Name.Substring(N7pos + 3, next - N7pos - 3);
                        }
                        if (name.IndexOf(N8pos) < name.Count - 1 && (name[name.IndexOf(N8pos) + 1] is int) && name.IndexOf(N8pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N8pos) + 1];
                            N8 = file.Name.Substring(N8pos + 3, next - N8pos - 3);
                        }
                        if (name.IndexOf(N9pos) < name.Count - 1 && (name[name.IndexOf(N9pos) + 1] is int) && name.IndexOf(N9pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N9pos) + 1];
                            N9 = file.Name.Substring(N9pos + 3, next - N9pos - 3);
                        }
                        if (name.IndexOf(N10pos) < name.Count - 1 && (name[name.IndexOf(N10pos) + 1] is int) && name.IndexOf(N10pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N10pos) + 1];
                            N10 = file.Name.Substring(N10pos + 4, next - N10pos - 4);
                        }
                        if (name.IndexOf(N11pos) < name.Count - 1 && (name[name.IndexOf(N11pos) + 1] is int) && name.IndexOf(N11pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N11pos) + 1];
                            N11 = file.Name.Substring(N11pos + 4, next - N11pos - 4);
                        }
                        if (name.IndexOf(N12pos) < name.Count - 1 && (name[name.IndexOf(N12pos) + 1] is int) && name.IndexOf(N12pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N12pos) + 1];
                            N12 = file.Name.Substring(N12pos + 4, next - N12pos - 4);
                        }
                        if (name.IndexOf(N13pos) < name.Count - 1 && (name[name.IndexOf(N13pos) + 1] is int) && name.IndexOf(N13pos) > -1)
                        {
                            int next = (int)name[name.IndexOf(N13pos) + 1];
                            N13 = file.Name.Substring(N13pos + 4, next - N13pos - 4);
                        }
                        
                        string ext0 = "";
                        string ext1 = "";
                        string ext2 = "";
                        string ext3 = "";
                        string ext4 = "";
                        string ext5 = "";
                        string ext6 = "";
                        string ext7 = "";
                        string ext8 = "";
                        string ext9 = "";

                        if (N13 != null)
                        {
                            string[] items = N13.Split(';');
                            if (items.Length >= 1)
                            {
                                ext0 = items[0];
                            }
                            if (items.Length >= 2)
                            {
                                ext1 = items[1];
                            }
                            if (items.Length >= 3)
                            {
                                ext2 = items[2];
                            }
                            if (items.Length >= 4)
                            {
                                ext3 = items[3];
                            }
                            if (items.Length >= 5)
                            {
                                ext4 = items[4];
                            }
                            if (items.Length >= 6)
                            {
                                ext5 = items[5];
                            }
                            if (items.Length >= 7)
                            {
                                ext6 = items[6];
                            }
                            if (items.Length >= 8)
                            {
                                ext7 = items[7];
                            }
                            if (items.Length >= 9)
                            {
                                ext8 = items[8];
                            }
                            if (items.Length >= 10)
                            {
                                ext9 = items[9];
                            }
                        }
                        
                        string timestamptext = SorterTimeStampFormat;
                        timestamptext = timestamptext.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        timestamptext = timestamptext.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        timestamptext = timestamptext.Replace("#None", "");
                        if (timestamptext != "" && timestamptext != "xx")
                        {
                            try
                            {
                                TriggerTimestamp = DateTime.ParseExact(timestamptext, "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                            }
                            catch (Exception ex)
                            {
                                TriggerTimestamp = file.CreationTime;
                            }
                        }
                        string pos = "";
                        pos = PosFormat;
                        pos = pos.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        pos = pos.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        pos = pos.Replace("#None", "");
                        int dimNo = 1;
                        string[] posstring = pos.Split(';');
                        if (posstring.Length == 13)
                        {
                            if(posstring[0] == "0")
                            {
                                dimNo = 1;
                            }
                            else if(posstring[0] == "1")
                            {
                                dimNo = 2;
                            }
                        }
                        switch (N12)
                        {
                            case "1":
                                StorageRoot = StorageRoot1;
                                LogContentArray = LogContentArray1;
                                if (dimNo == 1)
                                {
                                    calobj = cal1_1;
                                }
                                else
                                {
                                    calobj = cal1_2;
                                }
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta1_1;
                                    HttpArrayLock = HTTPArraySta1_1Lock;
                                    HttpInfoLock = HTTPInfoSta1_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta1_2;
                                    HttpArrayLock = HTTPArraySta1_2Lock;
                                    HttpInfoLock = HTTPInfoSta1_2Lock;
                                }
                                break;
                            case "2":
                                StorageRoot = StorageRoot2;
                                LogContentArray = LogContentArray2;
                                if (dimNo == 1)
                                {
                                    calobj = cal2_1;
                                }
                                else
                                {
                                    calobj = cal2_2;
                                }
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta2_1;
                                    HttpArrayLock = HTTPArraySta2_1Lock;
                                    HttpInfoLock = HTTPInfoSta2_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta2_2;
                                    HttpArrayLock = HTTPArraySta2_2Lock;
                                    HttpInfoLock = HTTPInfoSta2_2Lock;
                                }
                                break;
                            case "3":
                                StorageRoot = StorageRoot3;
                                LogContentArray = LogContentArray3;
                                if (dimNo == 1)
                                {
                                    calobj = cal3_1;
                                }
                                else
                                {
                                    calobj = cal3_2;
                                }
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta3_1;
                                    HttpArrayLock = HTTPArraySta3_1Lock;
                                    HttpInfoLock = HTTPInfoSta3_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta3_2;
                                    HttpArrayLock = HTTPArraySta3_2Lock;
                                    HttpInfoLock = HTTPInfoSta3_2Lock;
                                }
                                break;
                            case "4":
                                StorageRoot = StorageRoot4;
                                LogContentArray = LogContentArray4;
                                if (dimNo == 1)
                                {
                                    calobj = cal4_1;
                                }
                                else
                                {
                                    calobj = cal4_2;
                                }
                                if (Convert.ToInt64(N0) % 2 == 0)
                                {
                                    HttpArray = HTTPArraySta4_1;
                                    HttpArrayLock = HTTPArraySta4_1Lock;
                                    HttpInfoLock = HTTPInfoSta4_1Lock;
                                }
                                else
                                {
                                    HttpArray = HTTPArraySta4_2;
                                    HttpArrayLock = HTTPArraySta4_2Lock;
                                    HttpInfoLock = HTTPInfoSta4_2Lock;
                                }
                                break;
                            default:
                                StorageRoot = StorageRoot1;
                                LogContentArray = LogContentArray1;
                                if (dimNo == 1)
                                {
                                    calobj = cal1_1;
                                }
                                else
                                {
                                    calobj = cal1_2;
                                }
                                HttpArray = HTTPArraySta1_1; 
                                HttpArrayLock = HTTPArraySta1_1Lock;
                                HttpInfoLock = HTTPInfoSta1_1Lock;
                                break;
                        }
                        string Year = TriggerTimestamp.ToString("yyyy");
                        string Month = TriggerTimestamp.ToString("MM");
                        string Day = TriggerTimestamp.ToString("dd");
                        string Hour = TriggerTimestamp.ToString("HH");
                        string Minute = TriggerTimestamp.ToString("mm");
                        string Second = TriggerTimestamp.ToString("ss");
                        string Millisecond = TriggerTimestamp.ToString("fff");
                        string DirName = string.Join(@"\", SubFolders) + "\\";
                        string FullDir = "";
                        string FileName = FileNameFormat + extension;
                        string FtpFileName = FtpFileNameFormat + extension;
                        //string HttpFileName = HttpFileNameFormat + extension;
                        Regex MultiSlash = new Regex("\\\\{2,}");
                        Regex MultiSpace = new Regex("");
                        Regex Multibar = new Regex("~{2,}");
                        Regex Startbar = new Regex("^~");
                        Regex Endbar = new Regex("~$");
                        
                        FtpFileName = FtpFileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        FtpFileName = FtpFileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        FtpFileName = FtpFileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                        FtpFileName = FtpFileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                        
                        DirName = DirName.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        DirName = DirName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        DirName = DirName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                        DirName = DirName.Replace(@"/", @"\").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                        
                        FileName = FileName.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        FileName = FileName.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        FileName = FileName.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                        FileName = FileName.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                        
                        string temp_filename_header = FtpSubFolder1 + "~" + FtpSubFolder2 + "~" + FtpSubFolder3 + "~" + FtpSubFolder4 + "~" + FtpSubFolder5;
                        temp_filename_header = temp_filename_header.Replace("#None", "").Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        temp_filename_header = temp_filename_header.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        temp_filename_header = temp_filename_header.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                        temp_filename_header = temp_filename_header.Replace(@"/", "~").Replace(@"\", "~").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                        

                        string result = SorterResultFormat;
                        result = result.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        result = result.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        result = result.Replace("#None", "GR");
                        string description = "";
                        description = CaptionFormat;
                        description = description.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        description = description.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        description = description.Replace("#yyyy", Year).Replace("#MM", Month).Replace("#dd", Day).Replace("#HH", Hour).Replace("#mm", Minute).Replace("#ss", Second).Replace("#fff", Millisecond);
                        description = description.Replace("#EXT0", ext0).Replace("#EXT1", ext1).Replace("#EXT2", ext2).Replace("#EXT3", ext3);
                        description = description.Replace("#EXT4", ext4).Replace("#EXT5", ext5).Replace("#EXT6", ext6).Replace("#EXT7", ext7);
                        description = description.Replace("#EXT8", ext8).Replace("#EXT9", ext9);
                        string DimWeight = DimAndWeightFormat;
                        DimWeight = DimWeight.Replace("#N7", N7).Replace("#N8", N8).Replace("#N9", N9).Replace("#N10", N10).Replace("#N11", N11);
                        DimWeight = DimWeight.Replace("#N0", N0).Replace("#N1", N1.Replace("Master", "").Replace("master", "")).Replace("#N2", N2).Replace("#N3", N3).Replace("#N4", N4).Replace("#N5", N5).Replace("#N6", N6);
                        DimWeight = DimWeight.Replace("#None", "0;0;-1;0");
                        string[] dimweight = DimWeight.Split(';');
                        double parcelHeight = -1;
                        if (dimweight.Length >= 3)
                        {
                            try
                            {
                                parcelHeight = Convert.ToDouble(dimweight[2]);
                            }
                            catch 
                            { 
                                parcelHeight = -1;
                            }
                        }
                        if (NoReadAnalysisEnable)
                        {
                            string NoReadReason = "N/A";
                            if (result == "NR")
                            {
                                //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件: " + file.FullName + " Starting No Read Analysis!";
                                //AddLog(logcontent, LogContentArray);
#if FEATURE_VPRO
                                NoReadReason = NoReadAnalyzeFromFile(file, mImageFileTool, LogContentArray, file.FullName);
#endif
                                //logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件: " + file.FullName + " Finished No Read Analysis!";
                                //AddLog(logcontent, LogContentArray);
                            }
                            description = description.Replace("#ANA", NoReadReason);
                            string NoReadReasonPath = NoReadReason.Replace(@"/", "").Replace(@"\", "").Replace(@":", "").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                            if (NoReadReason != "N/A")
                            {
                                FtpFileName = FtpFileName.Replace("#ANA", NoReadReasonPath);
                                temp_filename_header = temp_filename_header.Replace("#ANA", NoReadReasonPath);
                                DirName = DirName.Replace("#ANA", NoReadReasonPath);
                                FileName = FileName.Replace("#ANA", NoReadReasonPath);
                            }
                            else
                            {
                                FtpFileName = FtpFileName.Replace("#ANA", "NA");
                                temp_filename_header = temp_filename_header.Replace("#ANA", "");
                                DirName = DirName.Replace("#ANA", "");
                                FileName = FileName.Replace("#ANA", "NA");
                            }
                        }
                        else
                        { 
                            description = description.Replace("#ANA", "N/A");
                            FtpFileName = FtpFileName.Replace("#ANA", "NA");
                            temp_filename_header = temp_filename_header.Replace("#ANA", "");
                            DirName = DirName.Replace("#ANA", "");
                            FileName = FileName.Replace("#ANA", "NA");

                        }
                        temp_filename_header = Multibar.Replace(temp_filename_header, "~");
                        temp_filename_header = Startbar.Replace(temp_filename_header, "");
                        temp_filename_header = Endbar.Replace(temp_filename_header, "");
                        FullDir = StorageRoot + "\\" + DirName;
                        FullDir = FullDir.Replace(@"/", @"\").Replace(@"*", "").Replace(@"?", "").Replace("\"", "").Replace(@"<", "").Replace(@">", "").Replace(@"|", "");
                        FullDir = MultiSlash.Replace(FullDir, "\\");//本地路径
                        string local_fullname = FullDir + FileName;//本地文件全名含路径
                        string ftp_fullname = ftpFolder + "\\" + temp_filename_header + delimiter + FtpFileName;//ftp文件夹中文件全名含路径
                        if (!Directory.Exists(ftpFolder))
                        {
                            Directory.CreateDirectory(ftpFolder);
                        }
                        if (!Directory.Exists(FullDir))
                        {
                            Directory.CreateDirectory(FullDir);
                        }
                        if (!System.IO.File.Exists(local_fullname))
                        {
                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            bool drawborder = false;
                            bool isUP = false;
                            if (N1.StartsWith("UP"))
                            {
                                drawborder = true;
                                isUP = true;
                            }
                            else
                            {
                                drawborder = false;
                                isUP = false;
                            }
                            try
                            {
                                if (allowChinese)
                                {
                                    /*Mat srcImg = Cv2.ImRead(file.FullName);
                                    if (!srcImg.Empty())
                                    {
                                        DrawCaptionGDI(srcImg, description, local_fullname, pos, drawborder, calobj);
                                    }*/
                                    DrawCaptionGDI(file, description, local_fullname, pos, drawborder, calobj, parcelHeight, isUP);
                                }
                                else
                                {
                                    DrawCaptionOpencv(file, description, local_fullname, pos, drawborder, calobj, parcelHeight,isUP);
                                }
                                if (System.IO.File.Exists(local_fullname))
                                {
                                    fileInfo = new FileInfo(local_fullname);
                                    if (upload)
                                    {
                                        fileInfo.CopyTo(ftp_fullname, false);
                                    }
                                }
                                sw.Stop();
                                logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件名: " + FileName + "水印: " + sw.ElapsedMilliseconds + "ms";
                                AddLog(logcontent, LogContentArray);
                            }
                            catch (Exception ex)
                            {
                                string s = ex.StackTrace;
                                string str = ex.Message;
                            }
                        }
                        if (System.IO.File.Exists(local_fullname))
                        {
                            try
                            {
                                file.Delete();
                            }
                            catch (Exception ex) { }
                        }
                        if (httpUpload)
                        {
                            if (N1.StartsWith("UP") && fileInfo != null)
                            {
                                int MstIndex = -1;
                                lock (HttpArrayLock)
                                {
                                    MstIndex = FindMstIndex(N0, HttpArray);
                                    if (MstIndex > -1)
                                    {
                                        master = HttpArray[MstIndex] as MasterImage;
                                    }
                                }
                                if(MstIndex < 0)
                                {
                                    master = new MasterImage(N0, N2, N3, N4, N5, N6, N7, N8, N9, N10, N11, Timestamp, TriggerTimestamp);
                                    lock (HttpArrayLock)
                                    {
                                        HttpArray.Add(master);
                                    }
                                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "GroupID: " + master.N0 + " added to httpArray! " + file.Name + " is the current file!";
                                    AddLog(logcontent, LogContentArray);
                                }
                                if (master != null && fileInfo != null && master.Images.Count == 0)
                                {
                                    lock (HttpInfoLock)
                                    {
                                        master.Images.Add(fileInfo);
                                        master.ImageTime.Add(TriggerTimestamp);
                                        master.BcrName.Add(N1);
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex) { }
            }
        }
#if FEATURE_VPRO
        private CogImage8Grey ConvertMat2CogImage8Grey(Mat m)
        {
            CogImage8Grey cogImage8Grey = new CogImage8Grey(m.Cols, m.Rows);
            ICogImage8PixelMemory cogImage8GreyMem = null;
            try
            {
                cogImage8GreyMem = cogImage8Grey.Get8GreyPixelMemory(Cognex.VisionPro.CogImageDataModeConstants.Read, 0, 0, cogImage8Grey.Width, cogImage8Grey.Height);
                IntPtr Image8GreyIntPtr = cogImage8GreyMem.Scan0;
                unsafe
                {
                    byte* curCVPointer = (byte*)m.Data.ToPointer();
                    byte* curCogPointer = (byte*)Image8GreyIntPtr.ToPointer();
                    for (int i = 0; i < m.Rows; i++)
                    {
                        for (int j = 0; j < m.Cols; j++)
                        {
                            *curCogPointer = *curCVPointer;
                            curCVPointer++;
                            curCogPointer++;
                        }
                        for (int k = 0; k < cogImage8GreyMem.Stride - m.Cols; k++)
                        {
                            curCogPointer++;
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (cogImage8GreyMem != null)
                {
                    cogImage8GreyMem.Dispose();
                }
            }
            return cogImage8Grey;
        }

        private string NoReadAnalyzeFromMat(Mat m, ArrayList LogContentArray, string StationName, string GroupID)
        {
            string logcontent = "";
            string NoReadReason = "Unknown";
            if (m != null && mTB != null)
            {
                CogImage8Grey cogImage8Grey = null;
                try
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();
                    cogImage8Grey = ConvertMat2CogImage8Grey(m);
                    sw.Stop();
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] "  + "StationName: " + StationName + "GroupID: " + GroupID + " Bottom mat2cogimg Convert Time:" + sw.ElapsedMilliseconds + "ms!" ;
                    AddLog(logcontent, LogContentArray);
                    lock (TBLock)
                    {
                        sw.Restart();
                        mTB.Inputs["OutputImage"].Value = cogImage8Grey;
                        mTB.Run();
                        NoReadReason = mTB.Outputs["NoReadReason"].Value.ToString();
                        /*if (this.InvokeRequired)
                        {
                            this.Invoke(new DoNothing(DoNothingFunction));
                        }*/
                        sw.Stop();
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "StationName: " + StationName + "GroupID: " + GroupID + " Bottom NoRead Analysis Time:" + sw.ElapsedMilliseconds + "ms!";
                        AddLog(logcontent, LogContentArray);
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    if (cogImage8Grey != null)
                    {
                        cogImage8Grey.Dispose();
                    }
                }
            }
            return NoReadReason;
        }
        private string NoReadAnalyzeFromFile(FileInfo file, CogImageFileTool mImageFileTool, ArrayList LogContentArray, string FileFullName)
        {
            string logcontent = "";
            string NoReadReason = "Unknown";
            if (file != null && mImageFileTool != null && mTB != null)
            {
                try
                {
                    Stopwatch sw = new Stopwatch();
                    Stopwatch sw1 = new Stopwatch();
                    sw.Start();
                    mImageFileTool.Operator.Open(file.FullName, CogImageFileModeConstants.Read);
                    mImageFileTool.Run();
                    sw.Stop();
                    logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件: " + FileFullName + " No Read ImageFile Time: " + sw.ElapsedMilliseconds + "ms!";
                    AddLog(logcontent, LogContentArray);
                    lock (TBLock)
                    {
                        sw1.Start();
                        mTB.Inputs["OutputImage"].Value = mImageFileTool.OutputImage;
                        
                        mTB.Run();
                        
                        NoReadReason = mTB.Outputs["NoReadReason"].Value.ToString();
                        /*if (this.InvokeRequired)
                        {
                            this.Invoke(new DoNothing(DoNothingFunction));
                        }*/
                        sw1.Stop();
                        //MessageBox.Show("Info", sw1.ElapsedMilliseconds + "ms!");
                        logcontent = "[" + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff") + "] " + "文件: " + FileFullName + " No Read Analysis Time: " + sw1.ElapsedMilliseconds + "ms!";
                        AddLog(logcontent, LogContentArray);
                    }
                }
                catch (Exception ex) { }
                finally
                {
                    if ((mImageFileTool.OutputImage as CogImage8Grey) != null)
                    {
                        (mImageFileTool.OutputImage as CogImage8Grey).Dispose();
                    }
                }
            }
            return NoReadReason;
        }
#endif
        private void CaptionEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (CaptionEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["CaptionEnable"].Value = "true";
                        captionEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["CaptionEnable"].Value = "false";
                        captionEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void PackageBorderCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (PackageBorderCheck.Checked)
                    {
                        config.AppSettings.Settings["AllowPackageBorder"].Value = "true";
                        allowPackageBorder = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["AllowPackageBorder"].Value = "false";
                        allowPackageBorder = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void InductCommBtn_Click(object sender, EventArgs e)
        {
            ind.ShowDialog();
        }
        private void calibrateBtn_Click(object sender, EventArgs e)
        {
            
            cal.ShowDialog();
            /*while (cal.dialogExit == false && !cal.IsDisposed)
            {
                if (!cal.Visible)
                {
                    break;
                }
            }
            cal.Dispose();*/
        }

        private void HttpResetBtn_Click(object sender, EventArgs e)
        {
            HttpSetLabelWhite();
        }

        private void FtpResetBtn_Click(object sender, EventArgs e)
        {
            setLabelWhite();
        }

        private void label30_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void SCADAPortBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void SCADAIPBox_TextChanged(object sender, EventArgs e)
        {

        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                GC.Collect();
                if (!Start.Enabled && Stop.Enabled)
                //if (true)
                {
                    if (!Timer1Running && Timer1Finished && Timer1Enabled)
                    {
                        Timer1.Stop();
                        Timer1.Start();
                    }
                    if (!Timer2Running && Timer2Finished && Timer2Enabled)
                    {
                        Timer2.Stop();
                        Timer2.Start();
                    }
                    if (!Timer3Running && Timer3Finished && Timer3Enabled)
                    {
                        Timer3.Stop();
                        Timer3.Start();
                    }
                    if (!Timer4Running && Timer4Finished && Timer4Enabled)
                    {
                        Timer4.Stop();
                        Timer4.Start();
                    }
                    if (!Timer5Running && Timer5Finished && Timer5Enabled)
                    {
                        Timer5.Stop();
                        Timer5.Start();
                    }
                    if (!Timer6Running && Timer6Finished && Timer6Enabled)
                    {
                        Timer6.Stop();
                        Timer6.Start();
                    }
                    if (!Timer7Running && Timer7Finished && Timer7Enabled)
                    {
                        Timer7.Stop();
                        Timer7.Start();
                    }
                    if (!Timer8Running && Timer8Finished && Timer8Enabled)
                    {
                        Timer8.Stop();
                        Timer8.Start();
                    }
                    if (!Timer9Running && Timer9Finished && Timer9Enabled)
                    {
                        Timer9.Stop();
                        Timer9.Start();
                    }
                    if (!Timer10Running && Timer10Finished && Timer10Enabled)
                    {
                        Timer10.Stop();
                        Timer10.Start();
                    }
                    if (!Timer11Running && Timer11Finished && Timer11Enabled)
                    {
                        Timer11.Stop();
                        Timer11.Start();
                    }
                    if (!Timer12Running && Timer12Finished && Timer12Enabled)
                    {
                        Timer12.Stop();
                        Timer12.Start();
                    }
                    if (!Timer13Running && Timer13Finished && Timer13Enabled)
                    {
                        Timer13.Stop();
                        Timer13.Start();
                    }
                    if (!Timer14Running && Timer14Finished && Timer14Enabled)
                    {
                        Timer14.Stop();
                        Timer14.Start();
                    }
                    if (!Timer15Running && Timer15Finished && Timer15Enabled)
                    {
                        Timer15.Stop();
                        Timer15.Start();
                    }
                    if (!Timer16Running && Timer16Finished && Timer16Enabled)
                    {
                        Timer16.Stop();
                        Timer16.Start();
                    }
                    if (!Timer17Running && Timer17Finished && Timer17Enabled)
                    {
                        Timer17.Stop();
                        Timer17.Start();
                    }
                    if (!Timer18Running && Timer18Finished && Timer18Enabled)
                    {
                        Timer18.Stop();
                        Timer18.Start();
                    }
                    if (!Timer19Running && Timer19Finished && Timer19Enabled)
                    {
                        Timer19.Stop();
                        Timer19.Start();
                    }
                    if (!Timer20Running && Timer20Finished && Timer20Enabled)
                    {
                        Timer20.Stop();
                        Timer20.Start();
                    }
                    if (!Timer21Running && Timer21Finished && Timer21Enabled)
                    {
                        Timer21.Stop();
                        Timer21.Start();
                    }
                    if (!Timer22Running && Timer22Finished && Timer22Enabled)
                    {
                        Timer22.Stop();
                        Timer22.Start();
                    }
                    if (!Timer23Running && Timer23Finished && Timer23Enabled)
                    {
                        Timer23.Stop();
                        Timer23.Start();
                    }
                    if (!Timer24Running && Timer24Finished && Timer24Enabled)
                    {
                        Timer24.Stop();
                        Timer24.Start();
                    }
                    if (!Timer25Running && Timer25Finished && Timer25Enabled)
                    {
                        Timer25.Stop();
                        Timer25.Start();
                    }
                    if (!Timer26Running && Timer26Finished && Timer26Enabled)
                    {
                        Timer26.Stop();
                        Timer26.Start();
                    }
                    if (!Timer27Running && Timer27Finished && Timer27Enabled)
                    {
                        Timer27.Stop();
                        Timer27.Start();
                    }
                    if (!Timer28Running && Timer28Finished && Timer28Enabled)
                    {
                        Timer28.Stop();
                        Timer28.Start();
                    }
                    if (!Timer29Running && Timer29Finished && Timer29Enabled)
                    {
                        Timer29.Stop();
                        Timer29.Start();
                    }
                    if (!Timer30Running && Timer30Finished && Timer30Enabled)
                    {
                        Timer30.Stop();
                        Timer30.Start();
                    }
                    if (!Timer31Running && Timer31Finished && Timer31Enabled)
                    {
                        Timer31.Stop();
                        Timer31.Start();
                    }
                    if (!Timer32Running && Timer32Finished && Timer32Enabled)
                    {
                        Timer32.Stop();
                        Timer32.Start();
                    }
                    if (!Timer33Running && Timer33Finished && Timer33Enabled)
                    {
                        Timer33.Stop();
                        Timer33.Start();
                    }
                    if (!Timer34Running && Timer34Finished && Timer34Enabled)
                    {
                        Timer34.Stop();
                        Timer34.Start();
                    }
                    if (!Timer35Running && Timer35Finished && Timer35Enabled)
                    {
                        Timer35.Stop();
                        Timer35.Start();
                    }
                    if (!Timer36Running && Timer36Finished && Timer36Enabled)
                    {
                        Timer36.Stop();
                        Timer36.Start();
                    }
                    if (!Timer37Running && Timer37Finished && Timer37Enabled)
                    {
                        Timer37.Stop();
                        Timer37.Start();
                    }
                    if (!Timer38Running && Timer38Finished && Timer38Enabled)
                    {
                        Timer38.Stop();
                        Timer38.Start();
                    }
                    if (!Timer39Running && Timer39Finished && Timer39Enabled)
                    {
                        Timer39.Stop();
                        Timer39.Start();
                    }
                    if (!Timer40Running && Timer40Finished && Timer40Enabled)
                    {
                        Timer40.Stop();
                        Timer40.Start();
                    }
                    if (!Timer41Running && Timer41Finished && Timer41Enabled)
                    {
                        Timer41.Stop();
                        Timer41.Start();
                    }
                    if (!Timer42Running && Timer42Finished && Timer42Enabled)
                    {
                        Timer42.Stop();
                        Timer42.Start();
                    }
                    if (!Timer43Running && Timer43Finished && Timer43Enabled)
                    {
                        Timer43.Stop();
                        Timer43.Start();
                    }
                    if (!Timer44Running && Timer44Finished && Timer44Enabled)
                    {
                        Timer44.Stop();
                        Timer44.Start();
                    }
                    if (!Timer45Running && Timer45Finished && Timer45Enabled)
                    {
                        Timer45.Stop();
                        Timer45.Start();
                    }
                    if (!Timer46Running && Timer46Finished && Timer46Enabled)
                    {
                        Timer46.Stop();
                        Timer46.Start();
                    }
                    if (!Timer47Running && Timer47Finished && Timer47Enabled)
                    {
                        Timer47.Stop();
                        Timer47.Start();
                    }
                    if (!Timer48Running && Timer48Finished && Timer48Enabled)
                    {
                        Timer48.Stop();
                        Timer48.Start();
                    }
                    if (!Timer49Running && Timer49Finished && Timer49Enabled)
                    {
                        Timer49.Stop();
                        Timer49.Start();
                    }
                    if (Timer1Finished && Timer1Running)
                    {
                        Timer1Running = false;
                    }
                    if (Timer2Finished && Timer2Running)
                    {
                        Timer2Running = false;
                    }
                    if (Timer3Finished && Timer3Running)
                    {
                        Timer3Running = false;
                    }
                    if (Timer4Finished && Timer4Running)
                    {
                        Timer4Running = false;
                    }
                    if (Timer5Finished && Timer5Running)
                    {
                        Timer5Running = false;
                    }
                    if (Timer6Finished && Timer6Running)
                    {
                        Timer6Running = false;
                    }
                    if (Timer7Finished && Timer7Running)
                    {
                        Timer7Running = false;
                    }
                    if (Timer8Finished && Timer8Running)
                    {
                        Timer8Running = false;
                    }
                    if (Timer9Finished && Timer9Running)
                    {
                        Timer9Running = false;
                    }
                    if (Timer10Finished && Timer10Running)
                    {
                        Timer10Running = false;
                    }
                    if (Timer11Finished && Timer11Running)
                    {
                        Timer11Running = false;
                    }
                    if (Timer12Finished && Timer12Running)
                    {
                        Timer12Running = false;
                    }
                    if (Timer13Finished && Timer13Running)
                    {
                        Timer13Running = false;
                    }
                    if (Timer14Finished && Timer14Running)
                    {
                        Timer14Running = false;
                    }
                    if (Timer15Finished && Timer15Running)
                    {
                        Timer15Running = false;
                    }
                    if (Timer16Finished && Timer16Running)
                    {
                        Timer16Running = false;
                    }
                    if (Timer17Finished && Timer17Running)
                    {
                        Timer17Running = false;
                    }
                    if (Timer18Finished && Timer18Running)
                    {
                        Timer18Running = false;
                    }
                    if (Timer19Finished && Timer19Running)
                    {
                        Timer19Running = false;
                    }
                    if (Timer20Finished && Timer20Running)
                    {
                        Timer20Running = false;
                    }
                    if (Timer21Finished && Timer21Running)
                    {
                        Timer21Running = false;
                    }
                    if (Timer22Finished && Timer22Running)
                    {
                        Timer22Running = false;
                    }
                    if (Timer23Finished && Timer23Running)
                    {
                        Timer23Running = false;
                    }
                    if (Timer24Finished && Timer24Running)
                    {
                        Timer24Running = false;
                    }
                    if (Timer25Finished && Timer25Running)
                    {
                        Timer25Running = false;
                    }
                    if (Timer26Finished && Timer26Running)
                    {
                        Timer26Running = false;
                    }
                    if (Timer27Finished && Timer27Running)
                    {
                        Timer27Running = false;
                    }
                    if (Timer28Finished && Timer28Running)
                    {
                        Timer28Running = false;
                    }
                    if (Timer29Finished && Timer29Running)
                    {
                        Timer29Running = false;
                    }
                    if (Timer30Finished && Timer30Running)
                    {
                        Timer30Running = false;
                    }
                    if (Timer31Finished && Timer31Running)
                    {
                        Timer31Running = false;
                    }
                    if (Timer32Finished && Timer32Running)
                    {
                        Timer32Running = false;
                    }
                    if (Timer33Finished && Timer33Running)
                    {
                        Timer33Running = false;
                    }
                    if (Timer34Finished && Timer34Running)
                    {
                        Timer34Running = false;
                    }
                    if (Timer35Finished && Timer35Running)
                    {
                        Timer35Running = false;
                    }
                    if (Timer36Finished && Timer36Running)
                    {
                        Timer36Running = false;
                    }
                    if (Timer37Finished && Timer37Running)
                    {
                        Timer37Running = false;
                    }
                    if (Timer38Finished && Timer38Running)
                    {
                        Timer38Running = false;
                    }
                    if (Timer39Finished && Timer39Running)
                    {
                        Timer39Running = false;
                    }
                    if (Timer40Finished && Timer40Running)
                    {
                        Timer40Running = false;
                    }
                    if (Timer41Finished && Timer41Running)
                    {
                        Timer41Running = false;
                    }
                    if (Timer42Finished && Timer42Running)
                    {
                        Timer42Running = false;
                    }
                    if (Timer43Finished && Timer43Running)
                    {
                        Timer43Running = false;
                    }
                    if (Timer44Finished && Timer44Running)
                    {
                        Timer44Running = false;
                    }
                    if (Timer45Finished && Timer45Running)
                    {
                        Timer45Running = false;
                    }
                    if (Timer46Finished && Timer46Running)
                    {
                        Timer46Running = false;
                    }
                    if (Timer47Finished && Timer47Running)
                    {
                        Timer47Running = false;
                    }
                    if (Timer48Finished && Timer48Running)
                    {
                        Timer48Running = false;
                    }
                    if (Timer49Finished && Timer49Running)
                    {
                        Timer49Running = false;
                    }
                }
            }
            catch (Exception ex) { }
        }

        private void UploadCheck2_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (UploadCheck2.Checked)
                    {
                        config.AppSettings.Settings["Upload2"].Value = "true";
                        upload2 = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["Upload2"].Value = "false";
                        upload2 = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void FtpResetBtn2_Click(object sender, EventArgs e)
        {
            setLabelWhite2();
        }


        private void BarcodeImgEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (BarcodeImgEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["BarcodeImgEnable"].Value = "true";
                        BarcodeImgEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["BarcodeImgEnable"].Value = "false";
                        BarcodeImgEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void TopOnlyCheck1_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (TopOnlyCheck1.Checked)
                    {
                        config.AppSettings.Settings["TopOnlyOnRead1"].Value = "true";
                        TopOnlyOnRead1 = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["TopOnlyOnRead1"].Value = "false";
                        TopOnlyOnRead1 = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void TopOnlyCheck2_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (TopOnlyCheck2.Checked)
                    {
                        config.AppSettings.Settings["TopOnlyOnRead2"].Value = "true";
                        TopOnlyOnRead2 = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["TopOnlyOnRead2"].Value = "false";
                        TopOnlyOnRead2 = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void TopOnlyCheck3_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (TopOnlyCheck3.Checked)
                    {
                        config.AppSettings.Settings["TopOnlyOnRead3"].Value = "true";
                        TopOnlyOnRead3 = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["TopOnlyOnRead3"].Value = "false";
                        TopOnlyOnRead3 = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void TopOnlyCheck4_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (TopOnlyCheck4.Checked)
                    {
                        config.AppSettings.Settings["TopOnlyOnRead4"].Value = "true";
                        TopOnlyOnRead4 = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["TopOnlyOnRead4"].Value = "false";
                        TopOnlyOnRead4 = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void ExtCaptionEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (ExtCaptionEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["ExtCaptionEnable"].Value = "true";
                        extCaptionEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["ExtCaptionEnable"].Value = "false";
                        extCaptionEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void ExtCaptionCommBtn_Click(object sender, EventArgs e)
        {
            ind.ShowDialog();
        }

        private void regexSta1Check_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (regexSta1Check.Checked)
                    {
                        config.AppSettings.Settings["regexSta1Enable"].Value = "true";
                        regexSta1Enable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["regexSta1Enable"].Value = "false";
                        regexSta1Enable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void regexSta2Check_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (regexSta2Check.Checked)
                    {
                        config.AppSettings.Settings["regexSta2Enable"].Value = "true";
                        regexSta2Enable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["regexSta2Enable"].Value = "false";
                        regexSta2Enable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void regexSta3Check_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (regexSta3Check.Checked)
                    {
                        config.AppSettings.Settings["regexSta3Enable"].Value = "true";
                        regexSta3Enable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["regexSta3Enable"].Value = "false";
                        regexSta3Enable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        private void regexSta4Check_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (regexSta4Check.Checked)
                    {
                        config.AppSettings.Settings["regexSta4Enable"].Value = "true";
                        regexSta4Enable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["regexSta4Enable"].Value = "false";
                        regexSta4Enable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void GridLineEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (GridLineEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["GridLineEnable"].Value = "true";
                        GridLineEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["GridLineEnable"].Value = "false";
                        GridLineEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void EncryptionEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (EncryptionEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["EncryptionEnable"].Value = "true";
                        imgEncryptEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["EncryptionEnable"].Value = "false";
                        imgEncryptEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }
        private void DWSEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (DWSEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["DWSEnable"].Value = "true";
                        DWSInterfaceEnabled = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["DWSEnable"].Value = "false";
                        DWSInterfaceEnabled = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
            try
            {
                if (DWS1Handler != null)
                {
                    DWS1Handler.Dispose();
                }
                if (DWSInterfaceEnabled)
                {
                    if (DWS1IP != "" && DWS1Port != "")
                    {
                        DWS1Handler = new SocketLib.TcpClientBase(DWS1IP, Convert.ToInt32(DWS1Port));
                        DWS1Handler.OnExpectedData += OnExpectedData;
                        DWS1Handler.logmsg += logmsg;
                        DWS1Handler.OnConnect += OnConnect;
                    }
                }
            }
            catch { }
        }

        private void NRAnalysisEnableCheck_CheckedChanged(object sender, EventArgs e)
        {
            lock (OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    if (NRAnalysisEnableCheck.Checked)
                    {
                        config.AppSettings.Settings["NRAnalysisEnable"].Value = "true";
                        NoReadAnalysisEnable = true;
                    }
                    else
                    {
                        config.AppSettings.Settings["NRAnalysisEnable"].Value = "false";
                        NoReadAnalysisEnable = false;
                    }
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                }
                catch (Exception ex) { }
            }
        }

        
        private void shiftLang()
        {
            if (LangCombo.Text == "Chinese")
            {
                CamCountLabel.Text = "存图相机数量：";
                工位一.Text = "主线工位一";
                工位二.Text = "主线工位二";
                label1.Text = "FTP根目录：";
                label2.Text = "存图位置根目录：";
                label3.Text = "服务器ip：";
                label4.Text = "端口：";
                label5.Text = "用户名：";
                label6.Text = "密码：";
                label7.Text = "多路复用命名：";
                label8.Text = "本地路径：";
                label9.Text = "本地图片命名：";
                label10.Text = "主线工位三";
                label11.Text = "存图相机数量：";
                label12.Text = "FTP根目录：";
                label13.Text = "存图相机数量：";
                label14.Text = "FTP根目录：";
                label15.Text = "主线工位四";
                label16.Text = "存图相机数量：";
                label17.Text = "FTP根目录：";
                label18.Text = "存图位置根目录：";
                label19.Text = "存图位置根目录：";
                label20.Text = "存图位置根目录：";
                label21.Text = "远程路径：";
                label22.Text = "若需要以时间命名，使用yyyy、MM、dd、HH、mm、ss、fff来表示";
                label23.Text = "图片保存天数：";
                label24.Text = "上传url：";
                label25.Text = "设备编码：";
                label26.Text = "正则url：";
                label27.Text = "图片命名：";
                label28.Text = "端口：";
                label29.Text = "密码：";
                label30.Text = "SCADA服务器ip：";
                label31.Text = "FTP正常状态报文：";
                label32.Text = "FTP异常状态报文：";
                label33.Text = "HTTP异常状态报文：";
                label34.Text = "HTTP正常状态报文：";
                label35.Text = "坐标信息格式:         X1;Y1;X2;Y2;X3;Y3;X4;Y4";
                label36.Text = "坐标信息：";
                label37.Text = "坐标信息格式:        体积测量编号（0或1）;X1;Y1;Z1;X2;Y2;Z2;X3;Y3;Z3;X4;Y4;Z4";
                label38.Text = "校验url：";
                label39.Text = "体积重量信息：";
                label40.Text = "(格式：长;宽;高;重量)";
                label41.Text = "多代码分隔符：";
                label42.Text = "DMBS数量：";
                label43.Text = "DMBS数量：";
                label44.Text = "DMBS数量：";
                label45.Text = "DMBS数量：";
                label46.Text = "供包台区域：";
                label47.Text = "供包台区域：";
                label48.Text = "供包台区域：";
                label49.Text = "供包台区域：";
                label50.Text = "供包台图片路径：";
                label51.Text = "供包台图片命名格式：_0,序列号_1,读码器名称_2,时间戳_3,结果_4,条码";
                label52.Text = "主线结果信息：\r\n(GR/MR/NR)";
                label53.Text = "主线条码信息：";
                label54.Text = "主线小车号信息：";
                label55.Text = "主线时间戳信息：";
                label56.Text = "远程图片命名：";
                label57.Text = "主线索引号信息：";
                label58.Text = "用户名：";
                label59.Text = "端口：";
                label60.Text = "服务器ip：";
                label61.Text = "条码坐标信息：";
                label62.Text = "主线工站号信息：";
                label63.Text = "供包台一区工站号：";
                label64.Text = "供包台二区工站号：";
                label65.Text = "外部水印等待：";
                label66.Text = "外部水印等待：";
                label67.Text = "外部水印等待：";
                label68.Text = "外部水印等待：";
                label69.Text = "正则编码：";
                label74.Text = "当前语言：";
                UploadCheck.Text = "图片服务器";
                Stop.Text = "停止";
                Start.Text = "开始";
                LogEnableCheck.Text = "是否启用日志";
                HTTPUploadCheck.Text = "是否上传HTTP服务器";
                UploadStatusCheck.Text = "是否上传状态报文";
                CaptionEnableCheck.Text = "是否添加图片水印";
                PackageBorderCheck.Text = "显示包裹边框";
                calibrateBtn.Text = "标定";
                InductCommBtn.Text = "供包台通讯配置";
                UploadCheck2.Text = "中邮多路输出";
                BarcodeImgEnableCheck.Text = "局部图片启用";
                TopOnlyCheck1.Text = "成功读取只存顶面";
                TopOnlyCheck2.Text = "成功读取只存顶面";
                TopOnlyCheck3.Text = "成功读取只存顶面";
                TopOnlyCheck4.Text = "成功读取只存顶面";
                ExtCaptionEnableCheck.Text = "外部水印信息";
                ExtCaptionCommBtn.Text = "外部水印通讯配置";
                regexSta1Check.Text = "主机一";
                regexSta2Check.Text = "主机二";
                regexSta3Check.Text = "主机三";
                regexSta4Check.Text = "主机四";
                DWSEnableCheck.Text = "是否启用DWS界面";
                EncryptionEnableCheck.Text = "是否图片加密";
                GridLineEnableCheck.Text = "显示网格";
                NRAnalysisEnableCheck.Text = "未读取分类";
                //VPPConfigureButton.Text = "测试";
                Login.label1text = "用户名";
                Login.label2text = "密码";
                Login.BtnCanceltext = "取消";
                Login.BtnOKtext = "确定";
                ind.BtnClose.Text = "关闭窗口";
                ind.BtnSave.Text = "保存配置";
                ind.label1.Text = "一区";
                ind.label2.Text = "导入台十";
                ind.label3.Text = "导入台一";
                ind.label4.Text = "底扫IP：";
                ind.label5.Text = "底扫端口：";
                ind.label7.Text = "本地服务器端口:";
                ind.label8.Text = "本地服务器IP:";
                ind.label9.Text = "工控机作为客户端与底扫通讯";
                ind.label10.Text = "工控机作为服务器与PLC通讯";
                ind.label11.Text = "底扫端口：";
                ind.label13.Text = "底扫IP：";
                ind.label14.Text = "底扫端口：";
                ind.label16.Text = "底扫IP：";
                ind.label17.Text = "导入台二";
                ind.label18.Text = "导入台三";
                ind.label19.Text = "导入台四";
                ind.label20.Text = "底扫端口：";
                ind.label22.Text = "底扫IP：";
                ind.label23.Text = "导入台五";
                ind.label24.Text = "底扫端口：";
                ind.label26.Text = "底扫IP：";
                ind.label27.Text = "底扫端口：";
                ind.label29.Text = "底扫IP：";
                ind.label30.Text = "导入台九";
                ind.label31.Text = "底扫端口：";
                ind.label33.Text = "底扫IP：";
                ind.label34.Text = "导入台八";
                ind.label35.Text = "导入台七";
                ind.label36.Text = "底扫端口：";
                ind.label38.Text = "底扫IP：";
                ind.label39.Text = "底扫端口：";
                ind.label41.Text = "底扫IP：";
                ind.label42.Text = "底扫端口：";
                ind.label44.Text = "底扫IP：";
                ind.label45.Text = "导入台六";
                ind.label46.Text = "二区";
                ind.label47.Text = "本地服务器";
                ind.label48.Text = "小车无读绑定超时:";
                ind.label49.Text = "秒";
                ind.label50.Text = "外部水印客户端 IP:";
                ind.label52.Text = "DWS1服务器:";
                gs.BtnClose.Text = "关闭窗口";
                gs.BtnSave.Text = "保存配置";

                if (BarcodeImgModeCombo.Text == "stitched" || BarcodeImgModeCombo.Text == "拼接")
                {
                    BarcodeImgModeCombo.Items[0] = "拼接";
                    BarcodeImgModeCombo.Items[1] = "不拼接";
                    BarcodeImgModeCombo.Text = "拼接";
                }
                else
                {
                    BarcodeImgModeCombo.Items[0] = "拼接";
                    BarcodeImgModeCombo.Items[1] = "不拼接";
                    BarcodeImgModeCombo.Text = "不拼接";
                }
                if (captionOptionList.Text == "AllowChinese" || captionOptionList.Text == "包含中文字符")
                {
                    captionOptionList.Items[0] = "包含中文字符";
                    captionOptionList.Items[1] = "仅英文字符";
                    captionOptionList.Text = "包含中文字符";
                }
                else
                {
                    captionOptionList.Items[0] = "包含中文字符";
                    captionOptionList.Items[1] = "仅英文字符";
                    captionOptionList.Text = "仅英文字符";
                }
            }
            else if (LangCombo.Text == "English")
            {
                CamCountLabel.Text = "CamCount：";
                工位一.Text = "StationA";
                工位二.Text = "StationB";
                label1.Text = "FTP Root：";
                label2.Text = "Image Root：";
                label3.Text = "ServerIP：";
                label4.Text = "Port：";
                label5.Text = "User：";
                label6.Text = "Secret：";
                label7.Text = "CPSTName：";
                label8.Text = "LocalDir：";
                label9.Text = "LocalName：";
                label10.Text = "StationC";
                label11.Text = "CamCount：";
                label12.Text = "FTP Root：";
                label13.Text = "CamCount：";
                label14.Text = "FTP Root：";
                label15.Text = "StationD";
                label16.Text = "CamCount：";
                label17.Text = "FTP Root：";
                label18.Text = "Image Root：";
                label19.Text = "Image Root：";
                label20.Text = "Image Root：";
                label21.Text = "RemoteDir：";
                label22.Text = "Use yyyy、MM、dd、HH、mm、ss、fff as placeholder for Timestamp";
                label23.Text = "Image Expiry(Days)：";
                label24.Text = "Upload_url：";
                label25.Text = "MachCode：";
                label26.Text = "Regex_url：";
                label27.Text = "ImgName：";
                label28.Text = "Port：";
                label29.Text = "Secret：";
                label30.Text = "SCADA IP：";
                label31.Text = "FTP OK Message：";
                label32.Text = "FTP Err Message：";
                label33.Text = "HTTP Err Message：";
                label34.Text = "HTTP OK Message：";
                label35.Text = "CodePos Format:        X1;Y1;X2;Y2;X3;Y3;X4;Y4";
                label36.Text = "Pos：";
                label37.Text = "DimPos Format:        DIMID(0/1);X1;Y1;Z1;X2;Y2;Z2;X3;Y3;Z3;X4;Y4;Z4";
                label38.Text = "Check_url：";
                label39.Text = "W&SInfo：";
                label40.Text = "(Format：L;W;H;Weight)";
                label41.Text = "Delimiter：";
                label42.Text = "DMBSCnt：";
                label43.Text = "DMBSCnt：";
                label44.Text = "DMBSCnt：";
                label45.Text = "DMBSCnt：";
                label46.Text = "Inductor：";
                label47.Text = "Inductor：";
                label48.Text = "Inductor：";
                label49.Text = "Inductor：";
                label50.Text = "InductorRoot：";
                label51.Text = "InductorImgName：_0,ID_1,BCRname_2,Time_3,Result_4,Code";
                label52.Text = "SorterResult：\r\n(GR/MR/NR)";
                label53.Text = "SorterCode：";
                label54.Text = "SorterTrayID：";
                label55.Text = "SorterTime：";
                label56.Text = "RemoteName：";
                label57.Text = "SorterGUID：";
                label58.Text = "User：";
                label59.Text = "Port：";
                label60.Text = "ServerIP：";
                label61.Text = "CodePos：";
                label62.Text = "SorterStaNo：";
                label63.Text = "InductAName：";
                label64.Text = "InductBName：";
                label65.Text = "ExtCapTimeout：";
                label66.Text = "ExtCapTimeout：";
                label67.Text = "ExtCapTimeout：";
                label68.Text = "ExtCapTimeout：";
                label69.Text = "RegCode：";
                label74.Text = "Lang：";
                UploadCheck.Text = "FTPServer";
                Stop.Text = "Stop";
                Start.Text = "Start";
                LogEnableCheck.Text = "LogEnable";
                HTTPUploadCheck.Text = "HTTPEnable";
                UploadStatusCheck.Text = "ScadaEnable";
                CaptionEnableCheck.Text = "Caption";
                PackageBorderCheck.Text = "ShowBorder";
                calibrateBtn.Text = "Calib";
                InductCommBtn.Text = "InductorComm";
                UploadCheck2.Text = "CPSTEnable";
                BarcodeImgEnableCheck.Text = "BCRImg";
                TopOnlyCheck1.Text = "TopOnlyGR";
                TopOnlyCheck2.Text = "TopOnlyGR";
                TopOnlyCheck3.Text = "TopOnlyGR";
                TopOnlyCheck4.Text = "TopOnlyGR";
                ExtCaptionEnableCheck.Text = "ExtCaption";
                ExtCaptionCommBtn.Text = "ExtCapComm";
                regexSta1Check.Text = "MasterA";
                regexSta2Check.Text = "MasterB";
                regexSta3Check.Text = "MasterC";
                regexSta4Check.Text = "MasterD";
                DWSEnableCheck.Text = "DWSInterface";
                EncryptionEnableCheck.Text = "Encryption";
                GridLineEnableCheck.Text = "Grid";
                NRAnalysisEnableCheck.Text = "NR_ANA";
                //VPPConfigureButton.Text = "Test";
                Login.label1text = "User";
                Login.label2text = "Secret";
                Login.BtnCanceltext = "Cancel";
                Login.BtnOKtext = "OK";
                ind.BtnClose.Text = "Close";
                ind.BtnSave.Text = "Save";
                ind.label1.Text = "Zone1";
                ind.label2.Text = "Ind10";
                ind.label3.Text = "Ind1";
                ind.label4.Text = "BtmIP：";
                ind.label5.Text = "BtmPort：";
                ind.label7.Text = "LocSPort:";
                ind.label8.Text = "LocSIP:";
                ind.label9.Text = "Local as client vs. Bottom";
                ind.label10.Text = "Local as server vs. PLC";
                ind.label11.Text = "BtmPort：";
                ind.label13.Text = "BtmIP：";
                ind.label14.Text = "BtmPort：";
                ind.label16.Text = "BtmIP：";
                ind.label17.Text = "Ind2";
                ind.label18.Text = "Ind3";
                ind.label19.Text = "Ind4";
                ind.label20.Text = "BtmPort：";
                ind.label22.Text = "BtmIP：";
                ind.label23.Text = "Ind5";
                ind.label24.Text = "BtmPort：";
                ind.label26.Text = "BtmIP：";
                ind.label27.Text = "BtmPort：";
                ind.label29.Text = "BtmIP：";
                ind.label30.Text = "Ind9";
                ind.label31.Text = "BtmPort：";
                ind.label33.Text = "BtmIP：";
                ind.label34.Text = "Ind8";
                ind.label35.Text = "Ind7";
                ind.label36.Text = "BtmPort：";
                ind.label38.Text = "BtmIP：";
                ind.label39.Text = "BtmPort：";
                ind.label41.Text = "BtmIP：";
                ind.label42.Text = "BtmPort：";
                ind.label44.Text = "BtmIP：";
                ind.label45.Text = "Ind6";
                ind.label46.Text = "Zone2";
                ind.label47.Text = "LocServer";
                ind.label48.Text = "BindingTimeout:";
                ind.label49.Text = "Sec";
                ind.label50.Text = "ExtCapClientIP:";
                ind.label52.Text = "DWS1Server:";
                gs.BtnClose.Text = "Close";
                gs.BtnSave.Text = "Save";
                if (BarcodeImgModeCombo.Text == "stitched" || BarcodeImgModeCombo.Text == "拼接")
                {
                    BarcodeImgModeCombo.Items[0] = "stitched";
                    BarcodeImgModeCombo.Items[1] = "normal";
                    BarcodeImgModeCombo.Text = "stitched";
                }
                else
                {
                    BarcodeImgModeCombo.Items[0] = "stitched";
                    BarcodeImgModeCombo.Items[1] = "normal";
                    BarcodeImgModeCombo.Text = "separate";
                }
                if (captionOptionList.Text == "AllowChinese" || captionOptionList.Text == "包含中文字符")
                {
                    captionOptionList.Items[0] = "AllowChinese";
                    captionOptionList.Items[1] = "EnglishOnly";
                    captionOptionList.Text = "AllowChinese";
                }
                else
                {
                    captionOptionList.Items[0] = "AllowChinese";
                    captionOptionList.Items[1] = "EnglishOnly";
                    captionOptionList.Text = "EnglishOnly";
                }
            }
        }
        private void LangCombo_SelectedIndexChanged(object sender, EventArgs e)
        {
            
                try
                {
                    lock (OpenConfigLock)
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings["CurLang"].Value = LangCombo.Text;
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                    }
                    shiftLang();
                    
                }
                catch (Exception ex) { }
            
        }
        public byte[] Encrypt(byte[] clearText, byte[] Key, byte[] IV)
        {
            Aes aesAlg = null;
            ICryptoTransform encryptor = null;
            byte[] EncryptedBytes = null;
            try
            {
                aesAlg = Aes.Create();
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC; // 使用CBC模式
                aesAlg.Padding = PaddingMode.PKCS7; // 使用PKCS7填充
                encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                EncryptedBytes = encryptor.TransformFinalBlock(clearText, 0, clearText.Length);
            }
            catch 
            { 
                EncryptedBytes = clearText; 
            }
            finally
            {
                if (aesAlg != null)
                {
                    aesAlg.Dispose();
                }
                if (encryptor != null)
                {
                    encryptor.Dispose();
                }
            }
            return EncryptedBytes;
        }
        /*
        public byte[] Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                aesAlg.Mode = CipherMode.CBC; // 使用CBC模式
                aesAlg.Padding = PaddingMode.PKCS7; // 使用PKCS7填充

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                return decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
            }
        }
        */
        private string readIV()
        {
            string registryPath = @"SOFTWARE\SIH";
            string keyname = "IV";
            string defaultvalue = "Herbert199391003";
            string value = "Herbert199391003";
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(registryPath, true);
                if (key == null)
                {
                    key = Registry.LocalMachine.CreateSubKey(registryPath, true);
                }
                if (key.GetValue(keyname) == null)
                {
                    key.SetValue(keyname, defaultvalue);
                }
                value = key.GetValue(keyname).ToString();
                while (value.Length < 16)
                {
                    value += "\x00";
                }
                value = value.Substring(0, 16);
            }
            catch { }
            finally
            {
                if (key != null)
                {
                    key.Dispose();
                }
            }
            return value;
        }
        private string readPSW()
        {
            string registryPath = @"SOFTWARE\SIH";
            string keyname = "psw";
            string defaultvalue = "Cognex";
            string value = "Cognex";
            RegistryKey key = null;
            try
            {
                key = Registry.LocalMachine.OpenSubKey(registryPath, true);
                if (key == null)
                {
                    key = Registry.LocalMachine.CreateSubKey(registryPath, true);
                }
                if (key.GetValue(keyname) == null)
                {
                    key.SetValue(keyname, defaultvalue);
                }
                value = key.GetValue(keyname).ToString();
                while (value.Length < 16)
                {
                    value += "\x00";
                }
                value = value.Substring(0, 16);
            }
            catch { }
            finally
            {
                if (key != null)
                {
                    key.Dispose();
                }
            }
            return value;
        }
        private void EncryptMat(Mat m, string destinaton, ImageEncodingParam iep)
        {
            byte[] IV = System.Text.Encoding.ASCII.GetBytes(imgLockIV);
            byte[] key = System.Text.Encoding.ASCII.GetBytes(imgLockPSW);
            byte[] rawBytes = null;
            if (m == null)
            {
                return;
            }
            if (m.Empty())
            {
                return;
            }
            if (System.IO.File.Exists(destinaton))
            {
                try
                {
                    File.Delete(destinaton);
                }
                catch { }
            }
            FileStream fs1 = null;
            try
            {
                Cv2.ImEncode(".jpg", m, out rawBytes, iep);
                if (rawBytes == null)
                {
                    return;
                }
                byte[] encryptedbytes = Encrypt(rawBytes, key, IV);
                fs1 = new FileStream(destinaton, FileMode.CreateNew);
                fs1.Write(encryptedbytes, 0, encryptedbytes.Length);
                fs1.Close();
            }
            catch { }
            finally
            {
                if (fs1 != null)
                {
                    fs1.Dispose();
                }
            }
        }
        private void EncryptImage(Image img, string destinaton, ImageCodecInfo Encoder, EncoderParameters eps)
        {
            byte[] IV = System.Text.Encoding.ASCII.GetBytes(imgLockIV);
            byte[] key = System.Text.Encoding.ASCII.GetBytes(imgLockPSW);
            byte[] rawBytes = null;
            if (img == null)
            {
                return;
            }
            if (System.IO.File.Exists(destinaton))
            {
                try
                {
                    File.Delete(destinaton);
                }
                catch { }
            }
            FileStream fs1 = null;
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    img.Save(ms, Encoder, eps);
                    rawBytes = ms.ToArray();
                }
                if (rawBytes == null)
                {
                    return;
                }
                byte[] encryptedbytes = Encrypt(rawBytes, key, IV);
                fs1 = new FileStream(destinaton, FileMode.CreateNew);
                fs1.Write(encryptedbytes, 0, encryptedbytes.Length);
                fs1.Close();
            }
            catch { }
            finally
            {
                if (fs1 != null)
                {
                    fs1.Dispose();
                }
            }
        }
        private void EncryptFile(FileInfo file, string destinaton)
        {
            byte[] IV = System.Text.Encoding.ASCII.GetBytes(imgLockIV);
            byte[] key = System.Text.Encoding.ASCII.GetBytes(imgLockPSW);
            FileStream fs = null;
            FileStream fs1 = null;
            if (System.IO.File.Exists(destinaton))
            {
                try
                {
                    File.Delete(destinaton);
                }
                catch { }
            }
            if(!file.Exists)
            {
                return;
            }
            try
            {
                fs = file.OpenRead();
                byte[] rawBytes = new byte[fs.Length];
                fs.Read(rawBytes, 0, rawBytes.Length);
                fs.Close();
                byte[] encryptedbytes = Encrypt(rawBytes, key, IV);
                fs1 = new FileStream(destinaton, FileMode.CreateNew);
                fs1.Write(encryptedbytes, 0, encryptedbytes.Length);
                fs1.Close();
                file.Delete();
            }
            catch { }
            finally
            {
                if (fs != null)
                {
                    fs.Dispose();
                }
                if (fs1 != null)
                {
                    fs1.Dispose();
                }
            }
        }
        static void GridLine(Graphics g, int cols, int rows, double parcelHeight, int dotlength, Font font, SolidBrush sb, Pen pen)
        {
            int interval = CalculateInterval(parcelHeight);
            if (interval > 0)
            {
                try
                {
                    DrawGrids(g, cols, rows, interval, dotlength, font, sb, pen);
                }
                catch { }
            }
        }
        static void GridLine(Mat m, int cols, int rows, double parcelHeight, int dotlength)
        {
            int interval = CalculateInterval(parcelHeight);
            if (interval > 0)
            {
                try
                {
                    DrawGrids(m, cols, rows, interval, dotlength);
                }
                catch { }
            }
        }
        static int CalculateInterval(double parcelHeight)
        {
            double WD = (MountingHeight - parcelHeight) / Math.Cos(ReadAngle / 180.0 * Math.PI);
            double u = (MountingHeight - 0.5 * MaxParcelHeight) / Math.Cos(ReadAngle / 180.0 * Math.PI);//对焦平面，物距
            double v = 1 / (1 / FocusLength - 1 / u);
            double pxWD = PixelSize * WD;
            if (Math.Abs(pxWD) > 1E-6)
            {
                return (int)Math.Round(100000 * v / pxWD);
            }
            return -1;
        }
        
        static void DrawGrids(Graphics g, int cols, int rows, int interval, int dotlength, Font font, SolidBrush sb, Pen pen)
        {
            int stopCol = 0;
            for (int i = 0; i < cols; i += interval)
            {
                int starty = rows - 1;
                if (i > 0 && i + interval < cols)
                {
                    g.DrawString(10 * i / interval + "cm", font, sb, new PointF(i, rows - rows/40));
                }
                else if (i > 0)
                {
                    stopCol = i;
                }
                while (starty > 0)
                {
                    if (starty - 2 * dotlength > 0)
                    {
                        g.DrawLine(pen, i, starty, i, starty - dotlength);
                    }
                    else
                    {
                        g.DrawLine(pen, i, starty, i, 0);
                    }
                    starty -= 2 * dotlength;
                }
            }
            for (int j = rows - 1; j > -1; j -= interval)
            {
                if (j < rows - 1)
                {
                    g.DrawString(10 * ((rows - 1 - j) / interval) + "cm", font, sb, new PointF(0, j));
                }
                int startx = 0;
                while (startx < stopCol - 1)
                {
                    if (startx + 2 * dotlength < stopCol - 1)
                    {
                        g.DrawLine(pen, startx, j, startx + dotlength, j);
                    }
                    else
                    {
                        g.DrawLine(pen, startx, j, stopCol - 1, j);
                    }
                    startx += 2 * dotlength;
                }
            }
        }
        
        static void DrawGrids(Mat m, int cols, int rows, int interval, int dotlength)
        {
            //int cols = m.Cols;
            //int rows = m.Rows;
            int stopCol = 0;
            for (int i = 0; i < cols; i += interval)
            {
                int starty = rows - 1;
                if (i > 0 && i + interval < cols)
                {
                    Cv2.PutText(m, 10 * i / interval + "cm", new OpenCvSharp.Point(i + m.Rows/256, rows - m.Rows / 256), HersheyFonts.HersheySimplex, m.Rows*1.0/1600.0, Scalar.White, (int)(Math.Max(1.0 * m.Rows / 500.0, 2)));
                }
                else if (i > 0)
                {
                    stopCol = i;
                }
                while (starty > 0)
                {
                    if (starty - 2 * dotlength > 0)
                    {
                        Cv2.Line(m, new OpenCvSharp.Point(i, starty), new OpenCvSharp.Point(i, starty - dotlength), Scalar.White, (int)Math.Round(m.Rows * 1.0 / 640.0));
                    }
                    else
                    {
                        Cv2.Line(m, new OpenCvSharp.Point(i, starty), new OpenCvSharp.Point(i, 0), Scalar.White, (int)Math.Round(m.Rows*1.0/ 640.0));
                    }
                    starty -= 2 * dotlength;
                }
            }
            for (int j = rows - 1; j > -1; j -= interval)
            {
                if (j < rows - 1)
                {
                    Cv2.PutText(m, 10 * ((rows - 1 - j) / interval) + "cm", new OpenCvSharp.Point(m.Rows / 256, j + m.Rows / 50), HersheyFonts.HersheySimplex, m.Rows * 1.0 / 1600.0, Scalar.White, (int)(Math.Max(1.0 * m.Rows / 500.0, 2)));
                }
                int startx = 0;
                while (startx < stopCol - 1)
                {
                    if (startx + 2 * dotlength < stopCol - 1)
                    {
                        Cv2.Line(m, new OpenCvSharp.Point(startx, j), new OpenCvSharp.Point(startx + dotlength, j), Scalar.White, (int)Math.Round(m.Rows * 1.0 / 640.0));
                    }
                    else
                    {
                        Cv2.Line(m, new OpenCvSharp.Point(startx, j), new OpenCvSharp.Point(stopCol - 1, j), Scalar.White, (int)Math.Round(m.Rows * 1.0 / 640.0));
                    }
                    startx += 2 * dotlength;
                }
            }
        }
        private void GridConfigBtn_Click(object sender, EventArgs e)
        {

        }

        private void GridConfigBtn_Click_1(object sender, EventArgs e)
        {
            gs.ShowDialog();
        }
    }
}
