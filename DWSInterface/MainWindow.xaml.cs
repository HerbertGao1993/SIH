using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Diagnostics;
using OpenCvSharp;
using System.Globalization;
using System.Collections.ObjectModel;
using MySql.Data.MySqlClient;


namespace DWSInterface
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private int Retries = 3;
        private static string connStr = "server=localhost;user=root;database=dws;port=3306;password=123456";
        QueryWindow qw;
        bool DeviceStatusMessageReceived = false;
        public ObservableCollection<BCRStatus> T { get; set; }
        private MyViewModel MainWindowViewModel = new MyViewModel();
        //private MyViewModel dwsRateViewModel = new MyViewModel();
        private UInt64 TotalCount = 0;
        private UInt64 ReadCount = 0;
        private UInt64 InfoIntactCount = 0;
        private const int DWSMessageExpiry = 10;//10s
        private DateTime LastHandleHisTimeStamp = DateTime.Now;
        private Thread th1;
        private Thread th2;
        private Thread th3;
        private readonly static Object StatsLock = new object();
        private readonly static Object DWSMessageListLock = new object();
        private readonly static Object HistDWSMessageListLock = new object();
        public List<DWSMessage> DWSMessageList = new List<DWSMessage>(1000);
        public static List<DWSMessage> HistDWSMessageList = new List<DWSMessage>(1000);
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MAXIMIZE = 3;
        
        public class ProgressBarData
        {
            public string content;
        }
        public class DWSMessage
        {
            public string Barcode;
            public string Length;
            public string Width;
            public string Height;
            public string Weight;
            public string Volume;
            public string ImageFullName;
            public DateTime TimeStamp;
            public DateTime TriggerOnTimeStamp;
            public DateTime ExpiryTimeStamp;
            public bool ReadStatus;
            public bool InfoIntact;
            public string ParcelID;
            public UInt64 TriggerOnTimeStampUTC;
            public string Destination;
            public DWSMessage(string Barcode, string Length, string Width, string Height, string Weight, string Volume, string ImageFullName, bool ReadStatus, bool InfoIntact, string ParcelID, DateTime TriggerOnTimeStamp,  UInt64 TriggerOnTimeStampUTC, string Destination)
            {
                this.Barcode = Barcode;
                this.Length = Length; 
                this.Width = Width;
                this.Height = Height;
                this.Weight = Weight;
                this.Volume = Volume;
                this.ImageFullName = ImageFullName;
                this.TimeStamp = DateTime.Now;
                this.ReadStatus = ReadStatus;
                this.InfoIntact = InfoIntact;
                this.ParcelID = ParcelID;
                this.TriggerOnTimeStampUTC = TriggerOnTimeStampUTC;
                this.TriggerOnTimeStamp = TriggerOnTimeStamp;
                this.ExpiryTimeStamp = TriggerOnTimeStamp.AddDays(14);
                this.Destination = Destination;
            }
            
        }
        public void MonitorDeviceStatus()
        {
            Thread.Sleep(10000);
            while (true)
            {
                /*FileStream fs = null;
                StreamWriter sr = null;
                try
                {
                    fs = new FileStream("D:\\log2.txt", FileMode.Append, FileAccess.Write);
                    sr = new StreamWriter(fs);
                    sr.WriteLine("WPF:" + Process.GetCurrentProcess().MainWindowHandle.ToString());
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
                }*/
                if (DeviceStatusMessageReceived)
                {
                    DeviceStatusMessageReceived = false;
                }
                else
                {
                    MainWindowViewModel.TFBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.TRBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.LFBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.LRBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.RFBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.RRBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BT01Background = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BT02Background = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BT03Background = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BT04Background = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BT05Background = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BT06Background = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.BCRBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.DIMBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.WeightBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.PanoromBackground = System.Windows.Media.Brushes.Red;
                    MainWindowViewModel.PLCBackground = System.Windows.Media.Brushes.Red;
                }
                Thread.Sleep(30000);
            }
        }
        public void HandleHistDWSMessageList()
        {
            while (true)
            {
                try
                {
                    /*this.Dispatcher.Invoke(() =>
                    {
                        HistDWSListCountBox.Text = HistDWSMessageList.Count.ToString();
                    });*/
                    MainWindowViewModel.HisListCount = HistDWSMessageList.Count.ToString();
                    if (HistDWSMessageList.Count >= 500 || (DateTime.Now - LastHandleHisTimeStamp).TotalSeconds > 60)
                    {
                        
                        
                        int count = HistDWSMessageList.Count;
                        MySqlConnection connection = null;
                        MySqlTransaction transaction = null;
                        MySqlCommand command = null;
                        try
                        {
                            if (count > 0)
                            {
                                while ((DateTime.Now - HistDWSMessageList[0].TimeStamp).TotalSeconds > 300)
                                {
                                    lock (HistDWSMessageListLock)
                                    {
                                        HistDWSMessageList.RemoveAt(0);
                                    }
                                }
                                int CurrentIndex = -1;
                                connection = new MySqlConnection(connStr);
                                connection.Open();
                                transaction = connection.BeginTransaction();
                                command = new MySqlCommand("INSERT INTO info (TimeStamp, Expiry, Barcode, Length, Width, Height, Weight, Volume, ImageFullName, ReadStatus, InfoIntact, ParcelID, Destination) VALUES (?TimeStamp, ?Expiry, ?Barcode, ?Length, ?Width, ?Height, ?Weight, ?Volume, ?ImageFullName, ?ReadStatus, ?InfoIntact, ?ParcelID, ?Destination)", connection, transaction);
                                List<string> queries = new List<string>(500);
                                bool flag = false;
                                try
                                {
                                    for (int i = 0; i < count; i++)
                                    {
                                        CurrentIndex = i;
                                        DWSMessage mes = HistDWSMessageList[i];
                                        if (mes is DWSMessage)
                                        {
                                            command.Parameters.Clear();
                                            command.Parameters.AddWithValue("?TimeStamp", mes.TriggerOnTimeStampUTC);
                                            command.Parameters.AddWithValue("?Expiry", mes.ExpiryTimeStamp);
                                            command.Parameters.AddWithValue("?Barcode", mes.Barcode);
                                            command.Parameters.AddWithValue("?Length", mes.Length);
                                            command.Parameters.AddWithValue("?Width", mes.Width);
                                            command.Parameters.AddWithValue("?Height", mes.Height);
                                            command.Parameters.AddWithValue("?Weight", mes.Weight);
                                            command.Parameters.AddWithValue("?Volume", mes.Volume);
                                            command.Parameters.AddWithValue("?ImageFullName", mes.ImageFullName);
                                            command.Parameters.AddWithValue("?ReadStatus", mes.ReadStatus ? "1" : "0");
                                            command.Parameters.AddWithValue("?InfoIntact", mes.InfoIntact ? "1" : "0");
                                            command.Parameters.AddWithValue("?ParcelID", mes.ParcelID);
                                            command.Parameters.AddWithValue("?Destination", mes.Destination);
                                            command.ExecuteNonQuery();
                                        }

                                    }
                                    flag = true;
                                }
                                catch 
                                {
                                    flag = false;
                                    Retries--;
                                    if (Retries <= 0)
                                    {
                                        lock (HistDWSMessageListLock)
                                        {
                                            HistDWSMessageList.RemoveAt(CurrentIndex);
                                        }
                                        Retries = 3;
                                    }
                                    transaction.Rollback(); 
                                }
                                if (flag)
                                {
                                    LastHandleHisTimeStamp = DateTime.Now;
                                    Retries = 3;
                                    transaction.Commit();
                                    lock (HistDWSMessageListLock)
                                    {
                                        if (HistDWSMessageList.Count >= count)
                                        {
                                            HistDWSMessageList.RemoveRange(0, count);
                                        }
                                    }
                                }
                            }
                        }
                        catch { }
                        finally
                        {
                            if (command != null)
                            {
                                command.Dispose();
                            }
                            if (transaction != null)
                            {
                                transaction.Dispose();
                            }
                            if (connection != null)
                            {
                                connection.Dispose();
                            }
                        }

                    }
                }
                catch { }
                Thread.Sleep(3000);
            }
        }
        public void HandleDWSMessageList()
        {
            while (true)
            {
                try
                {
                    /*this.Dispatcher.Invoke(() =>
                    {
                        DWSListCountBox.Text = DWSMessageList.Count.ToString();
                    });*/
                    MainWindowViewModel.DWSListCount = DWSMessageList.Count.ToString();
                    if (DWSMessageList.Count > 0)
                    {
                        while (DWSMessageList.Count > 1000)
                        {
                            lock (HistDWSMessageListLock)
                            {
                                HistDWSMessageList.Add(DWSMessageList[0]);
                            }
                            lock (DWSMessageListLock)
                            {
                                DWSMessageList.RemoveAt(0);
                            }
                        }
                        DWSMessage mes = DWSMessageList[0];
                        if (mes is DWSMessage)
                        {
                            if (File.Exists(mes.ImageFullName))
                            {
                                FileInfo file = new FileInfo(mes.ImageFullName);
                                double timeElapsed = (DateTime.Now - file.CreationTime).TotalMilliseconds;
                                if (timeElapsed >= 1000)
                                {
                                    GC.Collect();
                                    BitmapImage bmp = null;
                                    using (var stream = new FileStream(mes.ImageFullName, FileMode.Open))
                                    {
                                        bmp = new BitmapImage();
                                        bmp.BeginInit();
                                        bmp.StreamSource = stream;
                                        bmp.CacheOption = BitmapCacheOption.OnLoad;
                                        bmp.EndInit();
                                        bmp.Freeze();
                                    }
                                    lock (StatsLock)
                                    {
                                        TotalCount++;
                                        if (mes.InfoIntact)
                                        {
                                            InfoIntactCount++;
                                        }
                                        if (mes.ReadStatus)
                                        {
                                            ReadCount++;
                                        }
                                    }
                                    MainWindowViewModel.ReadRateBarRate = (100*(ReadCount * 1.0) / (TotalCount * 1.0)).ToString("F2") + "%";
                                    MainWindowViewModel.ReadRateBarValue = ((ReadCount * 1.0) / (TotalCount * 1.0)) * 360;
                                    MainWindowViewModel.ReadRateBarForegroundCount = ReadCount.ToString();
                                    MainWindowViewModel.ReadRateBarBackgroundCount = (TotalCount - ReadCount).ToString();
                                    MainWindowViewModel.DWSBarRate = (100*(InfoIntactCount * 1.0) / (TotalCount * 1.0)).ToString("F2") + "%";
                                    MainWindowViewModel.DWSBarValue = ((InfoIntactCount * 1.0) / (TotalCount * 1.0)) * 360;
                                    MainWindowViewModel.DWSBarForegroundCount = InfoIntactCount.ToString();
                                    MainWindowViewModel.DWSBarBackgroundCount = (TotalCount - InfoIntactCount).ToString();
                                    MainWindowViewModel.Barcode = mes.Barcode;
                                    MainWindowViewModel.DimString = mes.Length + "*" + mes.Width + "*" + mes.Height;
                                    MainWindowViewModel.ID = mes.ParcelID;
                                    MainWindowViewModel.WeightString = mes.Weight;
                                    MainWindowViewModel.VolumeString = mes.Volume;
                                    MainWindowViewModel.BMP = bmp;
                                    MainWindowViewModel.Destination = mes.Destination;
                                    lock (HistDWSMessageListLock)
                                    {
                                        HistDWSMessageList.Add(mes);
                                    }
                                    lock (DWSMessageListLock)
                                    {
                                        DWSMessageList.RemoveAt(0);
                                    }
                                    /*this.Dispatcher.Invoke(() =>
                                    {
                                        BarcodeTextBlock.Text = mes.Barcode;
                                        DimTextBlock.Text = mes.Length + "*" + mes.Width + "*" + mes.Height;
                                        IDTextBlock.Text = mes.ParcelID;
                                        WeightTextBlock.Text = mes.Weight;
                                        VolumeTextBlock.Text = mes.Volume;
                                        ImageBox.Source = bmp;
                                        
                                    });*/


                                }
                            }
                            else if ((DateTime.Now - mes.TimeStamp).TotalSeconds >= DWSMessageExpiry)
                            {
                                lock (StatsLock)
                                {
                                    TotalCount++;
                                    if (mes.InfoIntact)
                                    {
                                        InfoIntactCount++;
                                    }
                                    if (mes.ReadStatus)
                                    {
                                        ReadCount++;
                                    }
                                }
                                MainWindowViewModel.ReadRateBarRate = (100 * (ReadCount * 1.0) / (TotalCount * 1.0)).ToString("F2") + "%";
                                MainWindowViewModel.ReadRateBarValue = ((ReadCount * 1.0) / (TotalCount * 1.0)) * 360;
                                MainWindowViewModel.ReadRateBarForegroundCount = ReadCount.ToString();
                                MainWindowViewModel.ReadRateBarBackgroundCount = (TotalCount - ReadCount).ToString();
                                MainWindowViewModel.DWSBarRate = (100 * (InfoIntactCount * 1.0) / (TotalCount * 1.0)).ToString("F2") + "%";
                                MainWindowViewModel.DWSBarValue = ((InfoIntactCount * 1.0) / (TotalCount * 1.0)) * 360;
                                MainWindowViewModel.DWSBarForegroundCount = InfoIntactCount.ToString();
                                MainWindowViewModel.DWSBarBackgroundCount = (TotalCount - InfoIntactCount).ToString();
                                MainWindowViewModel.Barcode = mes.Barcode;
                                MainWindowViewModel.DimString = mes.Length + "*" + mes.Width + "*" + mes.Height;
                                MainWindowViewModel.ID = mes.ParcelID;
                                MainWindowViewModel.WeightString = mes.Weight;
                                MainWindowViewModel.VolumeString = mes.Volume;
                                MainWindowViewModel.BMP = null;
                                MainWindowViewModel.Destination = mes.Destination;
                                /*this.Dispatcher.Invoke(() =>
                                {
                                    BarcodeTextBlock.Text = mes.Barcode;
                                    DimTextBlock.Text = mes.Length + "*" + mes.Width + "*" + mes.Height;
                                    IDTextBlock.Text = mes.ParcelID;
                                    WeightTextBlock.Text = mes.Weight;
                                    VolumeTextBlock.Text = mes.Volume;
                                    ImageBox.Source = null;
                                });*/
                                lock (HistDWSMessageListLock)
                                {
                                    HistDWSMessageList.Add(mes);
                                }
                                lock (DWSMessageListLock)
                                {
                                    DWSMessageList.RemoveAt(0);
                                }
                            }
                        }
                        else
                        {
                            lock (DWSMessageListLock)
                            {
                                DWSMessageList.RemoveAt(0);
                            }
                        }
                    }
                }
                catch { }
                Thread.Sleep(200);
            }

        }
        public struct COPYDATASTRUCT
        {
            public IntPtr dwData;  // 若发送较小数据可用此字段传输
            public int cbData;     // 发送数据的大小（以字节为单位）
            [MarshalAs(UnmanagedType.LPStr)]
            public string lpData;  // 发送数据
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            HwndSource hwndSource = PresentationSource.FromVisual(this) as HwndSource;
            if (hwndSource != null)
            {
                IntPtr handle = hwndSource.Handle;
                hwndSource.AddHook(new HwndSourceHook(WndProc));
            }
        }
        IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == 0x004A)
            {
                try
                {
                    COPYDATASTRUCT cds = (COPYDATASTRUCT)Marshal.PtrToStructure(lParam, typeof(COPYDATASTRUCT));// 接收封装的消息
                    string message = cds.lpData;// 获取消息内容
                    if (message.Length == 16)
                    {
                        MainWindowViewModel.TFBackground = message[0] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.TRBackground = message[1] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.LFBackground = message[2] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.LRBackground = message[3] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.RFBackground = message[4] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.RRBackground = message[5] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.BT01Background = message[6] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.BT02Background = message[7] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.BT03Background = message[8] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.BT04Background = message[9] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.BT05Background = message[10] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.BT06Background = message[11] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.PanoromBackground = message[12] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.DIMBackground = message[13] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.WeightBackground = message[14] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        MainWindowViewModel.PLCBackground = message[15] == '1' ? System.Windows.Media.Brushes.LimeGreen : System.Windows.Media.Brushes.Red;
                        if (MainWindowViewModel.TFBackground == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.TRBackground == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.LFBackground == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.LRBackground == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.RFBackground == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.RRBackground == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.BT01Background == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.BT02Background == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.BT03Background == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.BT04Background == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.BT05Background == System.Windows.Media.Brushes.LimeGreen
                           && MainWindowViewModel.BT06Background == System.Windows.Media.Brushes.LimeGreen)
                        {
                            MainWindowViewModel.BCRBackground = System.Windows.Media.Brushes.LimeGreen;
                        }
                        else
                        {
                            MainWindowViewModel.BCRBackground = System.Windows.Media.Brushes.Red;
                        }
                        DeviceStatusMessageReceived = true;
                    }
                    else
                    {
                        JObject jobj = JsonConvert.DeserializeObject<JObject>(message);
                        if (jobj.SelectToken("Barcode").Value<string>() != null
                            && jobj.SelectToken("Length").Value<string>() != null
                            && jobj.SelectToken("Width").Value<string>() != null
                            && jobj.SelectToken("Height").Value<string>() != null
                            && jobj.SelectToken("Weight").Value<string>() != null
                            && jobj.SelectToken("Volume").Value<string>() != null
                            && jobj.SelectToken("ImageFullName").Value<string>() != null
                            && jobj.SelectToken("ParcelID").Value<string>() != null
                            && jobj.SelectToken("TriggerOnTimeStamp").Value<string>() != null
                            && jobj.SelectToken("Destination").Value<string>() != null)
                        {
                            string Barcode = jobj.SelectToken("Barcode").Value<string>();
                            string Length = jobj.SelectToken("Length").Value<string>();
                            string Width = jobj.SelectToken("Width").Value<string>();
                            string Height = jobj.SelectToken("Height").Value<string>();
                            string Weight = jobj.SelectToken("Weight").Value<string>();
                            string Volume = jobj.SelectToken("Volume").Value<string>();
                            string ImageFullName = jobj.SelectToken("ImageFullName").Value<string>();
                            bool ReadStatus = jobj.SelectToken("ReadStatus").Value<bool>();
                            bool InfoIntact = jobj.SelectToken("InfoIntact").Value<bool>();
                            string ParcelID = jobj.SelectToken("ParcelID").Value<string>();
                            string Destination = jobj.SelectToken("Destination").Value<string>();
                            DateTime DT = DateTime.ParseExact(jobj.SelectToken("TriggerOnTimeStamp").Value<string>(), "yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
                            UInt64 UTC = (UInt64)(DT - DateTime.Parse("1970-1-1", CultureInfo.InvariantCulture)).TotalMilliseconds;
                            DWSMessage dm = new DWSMessage(Barcode, Length, Width, Height, Weight, Volume, ImageFullName, ReadStatus, InfoIntact, ParcelID, DT, UTC, Destination);
                            lock (DWSMessageListLock)
                            {
                                DWSMessageList.Add(dm);
                            }
                        }
                    }
                }
                catch { }
            }
            return hwnd;
        }
        public MainWindow()
        {
            InitializeComponent();
            qw = new QueryWindow();
            qw.Hide();
            th1 = new Thread(new ThreadStart(HandleDWSMessageList));
            th2 = new Thread(new ThreadStart(HandleHistDWSMessageList));
            th3 = new Thread(new ThreadStart(MonitorDeviceStatus));
            th1.Start();
            th2.Start();
            th3.Start();
            MainWindowViewModel.ReadRateBarTitle = "读取率";
            MainWindowViewModel.ReadRateBarRate = "0%";
            MainWindowViewModel.ReadRateBarForegroundText = "有条码";
            MainWindowViewModel.ReadRateBarForegroundCount = "0";
            MainWindowViewModel.ReadRateBarBackgroundText = "未读取";
            MainWindowViewModel.ReadRateBarBackgroundCount = "0";
            MainWindowViewModel.ReadRateBarValue = 0;
            MainWindowViewModel.DWSBarTitle = "信息完整率";
            MainWindowViewModel.DWSBarRate = "0%";
            MainWindowViewModel.DWSBarForegroundText = "信息全";
            MainWindowViewModel.DWSBarForegroundCount = "0";
            MainWindowViewModel.DWSBarBackgroundText = "缺信息";
            MainWindowViewModel.DWSBarBackgroundCount = "0";
            MainWindowViewModel.DWSBarValue = 0;
            MainWindowViewModel.BCRBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.DIMBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.LFBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.LRBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.PanoromBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.PLCBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.RFBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.RRBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.TFBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.TRBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.BT01Background = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.BT02Background = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.BT03Background = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.BT04Background = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.BT05Background = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.BT06Background = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.WeightBackground = System.Windows.Media.Brushes.LimeGreen;
            MainWindowViewModel.Barcode = "";
            MainWindowViewModel.DimString = "";
            MainWindowViewModel.ID = "";
            MainWindowViewModel.WeightString = "";
            MainWindowViewModel.VolumeString = "";
            MainWindowViewModel.BMP = null;
            MainWindowViewModel.Destination = "";
            MainWindowViewModel.Language = "Chinese";
            MainWindowViewModel.ResetTitle = "清零";
            MainWindowViewModel.LHWTitle = "长*宽*高 mm";
            MainWindowViewModel.TrackIDTitle = "索引号";
            MainWindowViewModel.ChuteTitle = "格口";
            MainWindowViewModel.WeightTitle = "重量 kg";
            MainWindowViewModel.VolumeTitle = "体积 mm³";
            MainWindowViewModel.QueryTitle = "查询";
            MainWindowViewModel.BCRTitle = "读码器";
            MainWindowViewModel.PanoromTitle = "全景相机";
            MainWindowViewModel.DimTitle = "体积测量";
            MainWindowViewModel.ScaleTitle = "秤";
            qw.qvm.ReadRate = "读取率: ";
            qw.qvm.IDTitle = "序号";
            qw.qvm.BarcodeTitle = "条码";
            qw.qvm.DestTitle = "格口";
            qw.qvm.LengthTitle = "长(mm)";
            qw.qvm.WidthTitle = "宽(mm)";
            qw.qvm.HeightTitle = "高(mm)";
            qw.qvm.VolumeTitle = "体积(mm³)";
            qw.qvm.WeightTitle = "重量(kg)";
            qw.qvm.TimestampTitle = "时间戳";
            qw.qvm.PicTitle = "显示图片";
            qw.qvm.SearchTimeTitle = "起止时间: ";
            qw.qvm.SearchCodeTitle = "条码: ";
            qw.qvm.QueryTitle = "查询";
            qw.qvm.TotalTitle = "共";
            qw.qvm.PageTitle = "页";
            this.DataContext = MainWindowViewModel;
            
            /*
            Stopwatch sw = new Stopwatch();
            sw.Start();
            MySqlConnection connection = null;
            MySqlTransaction transaction = null;
            MySqlCommand command = null;
            for (int ii = 0; ii < 50000; ii++)
            {
                try
                {
                    connection = new MySqlConnection(connStr);
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command = new MySqlCommand("INSERT INTO info (TimeStamp, Expiry, Barcode, Length, Width, Height, Weight, Volume, ImageFullName, ReadStatus, InfoIntact, ParcelID) VALUES (?TimeStamp, ?Expiry, ?Barcode, ?Length, ?Width, ?Height, ?Weight, ?Volume, ?ImageFullName, ?ReadStatus, ?InfoIntact, ?ParcelID)", connection, transaction);
                    List<string> queries = new List<string>(500);
                    bool flag = false;

                    try
                    {
                        for (int i = 0; i < 200; i++)
                        {


                            command.Parameters.Clear();
                            command.Parameters.AddWithValue("?TimeStamp", ii * 200 + i);
                            command.Parameters.AddWithValue("?Expiry", DateTime.Now);
                            command.Parameters.AddWithValue("?Barcode", i.ToString());
                            command.Parameters.AddWithValue("?Length", i.ToString());
                            command.Parameters.AddWithValue("?Width", i.ToString());
                            command.Parameters.AddWithValue("?Height", i.ToString());
                            command.Parameters.AddWithValue("?Weight", i.ToString());
                            command.Parameters.AddWithValue("?Volume", i.ToString());
                            command.Parameters.AddWithValue("?ImageFullName", i.ToString());
                            command.Parameters.AddWithValue("?ReadStatus", "1");
                            command.Parameters.AddWithValue("?InfoIntact", "0");
                            command.Parameters.AddWithValue("?ParcelID", i.ToString());
                            command.ExecuteNonQuery();


                        }
                        flag = true;
                    }
                    catch { transaction.Rollback(); }
                    if (flag)
                    {
                        transaction.Commit();

                    }
                }
                catch { }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }
            }


            sw.Stop();
            long iii = sw.ElapsedMilliseconds;
            */
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveBeforeExit();
            Environment.Exit(0);
            Process.GetCurrentProcess().Kill();
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void DWSListCountBox_MouseEnter(object sender, MouseEventArgs e)
        {
            DWSListCountBox.Foreground = System.Windows.Media.Brushes.White;
        }

        private void HistDWSListCountBox_MouseEnter(object sender, MouseEventArgs e)
        {
            HistDWSListCountBox.Foreground = System.Windows.Media.Brushes.White;
        }

        private void DWSListCountBox_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#69839C");
            SolidColorBrush brush = new SolidColorBrush(color);
            DWSListCountBox.Foreground = brush;
        }

        private void HistDWSListCountBox_MouseLeave(object sender, MouseEventArgs e)
        {
            System.Windows.Media.Color color = (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#69839C");
            SolidColorBrush brush = new SolidColorBrush(color);
            HistDWSListCountBox.Foreground = brush;
        }

        private void DWSBar_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

        }

        private void ButtonHis_Click(object sender, RoutedEventArgs e)
        {
            qw.Show();
            qw.Focus();
        }
        public static void SaveBeforeExit()
        {
            
            if (HistDWSMessageList.Count > 0)
            {
                int count = HistDWSMessageList.Count;
                MySqlConnection connection = null;
                MySqlTransaction transaction = null;
                MySqlCommand command = null;
                try
                {
                    connection = new MySqlConnection(connStr);
                    connection.Open();
                    transaction = connection.BeginTransaction();
                    command = new MySqlCommand("INSERT INTO info (TimeStamp, Expiry, Barcode, Length, Width, Height, Weight, Volume, ImageFullName, ReadStatus, InfoIntact, ParcelID, Destination) VALUES (?TimeStamp, ?Expiry, ?Barcode, ?Length, ?Width, ?Height, ?Weight, ?Volume, ?ImageFullName, ?ReadStatus, ?InfoIntact, ?ParcelID, ?Destination)", connection, transaction);
                    List<string> queries = new List<string>(500);
                    bool flag = false;
                    try
                    {
                        for (int i = 0; i < count; i++)
                        {
                            DWSMessage mes = HistDWSMessageList[i];
                            if (mes is DWSMessage)
                            {
                                command.Parameters.Clear();
                                command.Parameters.AddWithValue("?TimeStamp", mes.TriggerOnTimeStampUTC);
                                command.Parameters.AddWithValue("?Expiry", mes.ExpiryTimeStamp);
                                command.Parameters.AddWithValue("?Barcode", mes.Barcode);
                                command.Parameters.AddWithValue("?Length", mes.Length);
                                command.Parameters.AddWithValue("?Width", mes.Width);
                                command.Parameters.AddWithValue("?Height", mes.Height);
                                command.Parameters.AddWithValue("?Weight", mes.Weight);
                                command.Parameters.AddWithValue("?Volume", mes.Volume);
                                command.Parameters.AddWithValue("?ImageFullName", mes.ImageFullName);
                                command.Parameters.AddWithValue("?ReadStatus", mes.ReadStatus ? "1" : "0");
                                command.Parameters.AddWithValue("?InfoIntact", mes.InfoIntact ? "1" : "0");
                                command.Parameters.AddWithValue("?ParcelID", mes.ParcelID);
                                command.Parameters.AddWithValue("?Destination", mes.Destination);
                                command.ExecuteNonQuery();
                            }

                        }
                        flag = true;
                    }
                    catch { transaction.Rollback(); }
                    if (flag)
                    {
                        transaction.Commit();
                        lock (HistDWSMessageListLock)
                        {
                            if (HistDWSMessageList.Count > 0)
                            {
                                HistDWSMessageList.RemoveRange(0, count);
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    if (command != null)
                    {
                        command.Dispose();
                    }
                    if (transaction != null)
                    {
                        transaction.Dispose();
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                }
            }
            FileStream fs = null;
            StreamWriter sr = null;
            try
            {
                //fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                //sr = new StreamWriter(fs);
                //sr.WriteLine("save before exit");
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
        public static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //MessageBox.Show("捕获到其他类型的异常！");
            SaveBeforeExit();
        }

        private void LangShift_Click(object sender, RoutedEventArgs e)
        {
            if (MainWindowViewModel.Language == "Chinese")
            {
                MainWindowViewModel.Language = "English";
                MainWindowViewModel.ResetTitle = "Reset";
                MainWindowViewModel.LHWTitle = "L*W*H mm";
                MainWindowViewModel.TrackIDTitle = "ID";
                MainWindowViewModel.ChuteTitle = "Chute";
                MainWindowViewModel.WeightTitle = "Weight kg";
                MainWindowViewModel.VolumeTitle = "Volume mm³";
                MainWindowViewModel.QueryTitle = "Query";
                MainWindowViewModel.ReadRateBarTitle = "ReadRate";
                MainWindowViewModel.ReadRateBarForegroundText = "OK";
                MainWindowViewModel.ReadRateBarBackgroundText = "NG";
                MainWindowViewModel.DWSBarTitle = "Information";
                MainWindowViewModel.DWSBarForegroundText = "OK";
                MainWindowViewModel.DWSBarBackgroundText = "NG";
                MainWindowViewModel.BCRTitle = "Readers";
                MainWindowViewModel.PanoromTitle = "Panorom";
                MainWindowViewModel.DimTitle = "Dimensioner";
                MainWindowViewModel.ScaleTitle = "Scales";
                qw.qvm.ReadRate = "ReadRate" + qw.qvm.ReadRate.Substring(qw.qvm.ReadRate.IndexOf(": "));
                qw.qvm.IDTitle = "ID";
                qw.qvm.BarcodeTitle = "Barcode";
                qw.qvm.DestTitle = "Chute";
                qw.qvm.LengthTitle = "Length(mm)";
                qw.qvm.WidthTitle = "Width(mm)";
                qw.qvm.HeightTitle = "Height(mm)";
                qw.qvm.VolumeTitle = "Volume(mm³)";
                qw.qvm.WeightTitle = "Weight(kg)";
                qw.qvm.TimestampTitle = "Timestamp";
                qw.qvm.PicTitle = "Image";
                qw.qvm.SearchTimeTitle = "Timespan: ";
                qw.qvm.SearchCodeTitle = "Barcode: ";
                qw.qvm.QueryTitle = "Query";
                qw.qvm.TotalTitle = "of";
                qw.qvm.PageTitle = "";
            }
            else if (MainWindowViewModel.Language == "English")
            {
                MainWindowViewModel.Language = "Romanian";
                MainWindowViewModel.ResetTitle = "Reset";
                MainWindowViewModel.LHWTitle = "L*W*H mm";
                MainWindowViewModel.TrackIDTitle = "ID";
                MainWindowViewModel.ChuteTitle = "Chute";
                MainWindowViewModel.WeightTitle = "Greutate kg";
                MainWindowViewModel.VolumeTitle = "Volum mm³";
                MainWindowViewModel.QueryTitle = "Căută";
                MainWindowViewModel.ReadRateBarTitle = "Rata citire";
                MainWindowViewModel.ReadRateBarForegroundText = "OK";
                MainWindowViewModel.ReadRateBarBackgroundText = "NG";
                MainWindowViewModel.DWSBarTitle = "Informații";
                MainWindowViewModel.DWSBarForegroundText = "OK";
                MainWindowViewModel.DWSBarBackgroundText = "NG";
                MainWindowViewModel.BCRTitle = "de cod";
                MainWindowViewModel.PanoromTitle = "Panorom";
                MainWindowViewModel.DimTitle = "Dimensioner";
                MainWindowViewModel.ScaleTitle = "Scales";
                qw.qvm.ReadRate = "Rata citire" + qw.qvm.ReadRate.Substring(qw.qvm.ReadRate.IndexOf(": "));
                qw.qvm.IDTitle = "ID";
                qw.qvm.BarcodeTitle = "Cod de bară";
                qw.qvm.DestTitle = "Destinație";
                qw.qvm.LengthTitle = "Lungimea(mm)";
                qw.qvm.WidthTitle = "Lățime(mm)";
                qw.qvm.HeightTitle = "Înălțimea(mm)";
                qw.qvm.VolumeTitle = "Volum(mm³)";
                qw.qvm.WeightTitle = "Greutate(kg)";
                qw.qvm.TimestampTitle = "Timp";
                qw.qvm.PicTitle = "Imagine";
                qw.qvm.SearchTimeTitle = "Timp: ";
                qw.qvm.SearchCodeTitle = "Cod de bară: ";
                qw.qvm.QueryTitle = "Căută";
                qw.qvm.TotalTitle = "of";
                qw.qvm.PageTitle = "";
            }
            else
            {
                MainWindowViewModel.Language = "Chinese";
                MainWindowViewModel.LHWTitle = "长*宽*高 mm";
                MainWindowViewModel.TrackIDTitle = "索引号";
                MainWindowViewModel.ChuteTitle = "格口";
                MainWindowViewModel.WeightTitle = "重量 kg";
                MainWindowViewModel.VolumeTitle = "体积 mm³";
                MainWindowViewModel.QueryTitle = "查询";
                MainWindowViewModel.ReadRateBarTitle = "读取率";
                MainWindowViewModel.ReadRateBarForegroundText = "有条码";
                MainWindowViewModel.ReadRateBarBackgroundText = "未读取";
                MainWindowViewModel.DWSBarTitle = "信息完整率";
                MainWindowViewModel.DWSBarForegroundText = "信息全";
                MainWindowViewModel.DWSBarBackgroundText = "缺信息";
                MainWindowViewModel.BCRTitle = "读码器";
                MainWindowViewModel.PanoromTitle = "全景相机";
                MainWindowViewModel.DimTitle = "体积测量";
                MainWindowViewModel.ScaleTitle = "秤";
                qw.qvm.ReadRate = "读取率" + qw.qvm.ReadRate.Substring(qw.qvm.ReadRate.IndexOf(": "));
                qw.qvm.IDTitle = "序号";
                qw.qvm.BarcodeTitle = "条码";
                qw.qvm.DestTitle = "格口";
                qw.qvm.LengthTitle = "长(mm)";
                qw.qvm.WidthTitle = "宽(mm)";
                qw.qvm.HeightTitle = "高(mm)";
                qw.qvm.VolumeTitle = "体积(mm³)";
                qw.qvm.WeightTitle = "重量(kg)";
                qw.qvm.TimestampTitle = "时间戳";
                qw.qvm.PicTitle = "显示图片";
                qw.qvm.SearchTimeTitle = "起止时间: ";
                qw.qvm.SearchCodeTitle = "条码: ";
                qw.qvm.QueryTitle = "查询";
                qw.qvm.TotalTitle = "共";
                qw.qvm.PageTitle = "页";
            }
        }

        private void ResetStats_Click(object sender, RoutedEventArgs e)
        {
            lock (StatsLock)
            {
                ReadCount = 0;
                TotalCount = 0;
                InfoIntactCount = 0;
            }
            MainWindowViewModel.ReadRateBarRate = "0%";
            MainWindowViewModel.ReadRateBarForegroundCount = "0";
            MainWindowViewModel.ReadRateBarBackgroundCount = "0";
            MainWindowViewModel.ReadRateBarValue = 0;
            MainWindowViewModel.DWSBarRate = "0%";
            MainWindowViewModel.DWSBarForegroundCount = "0";
            MainWindowViewModel.DWSBarBackgroundCount = "0";
            MainWindowViewModel.DWSBarValue = 0;
        }
    }
}
