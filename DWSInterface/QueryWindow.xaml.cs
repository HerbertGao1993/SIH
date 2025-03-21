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
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using MySql.Data.MySqlClient;
using System.IO;
using OpenCvSharp;
using System.Threading;

namespace DWSInterface
{
    /// <summary>
    /// QueryWindow.xaml 的交互逻辑
    /// </summary>
    public partial class QueryWindow : System.Windows.Window
    {
        
        string connStr = "server=localhost;user=root;database=dws;port=3306;password=123456";
        internal QueryViewModel qvm = new QueryViewModel();
        Process CurrentImageProcess = null;
        Mat CurrentImage = null;
        public static class Utils
        {
            public static T GetVisualParent<T>(DependencyObject child) where T : DependencyObject
            {
                // 获取父级元素
                DependencyObject parentObject = VisualTreeHelper.GetParent(child);
                if (parentObject != null && parentObject is T)
                {
                    return (T)parentObject;
                }
                else if (parentObject != null)
                {
                    return GetVisualParent<T>(parentObject);
                }
                else
                {
                    return null;
                }
            }
        }
        public QueryWindow()
        {

            InitializeComponent();
            List<History> Histories = new List<History>();
            for (int i = 0;i < 20;i++)
            {
                Histories.Add(new History(i.ToString(), "", "", "", "", "", "", "", "", false,""));
            }
            qvm.Source = Histories;
            /*qvm.ReadRate = "读取率： ";
            qvm.IDTitle = "序号";
            qvm.BarcodeTitle = "条码";
            qvm.DestTitle = "格口";
            qvm.LengthTitle = "长(mm)";
            qvm.WidthTitle = "宽(mm)";
            qvm.HeightTitle = "高(mm)";
            qvm.VolumeTitle = "体积(mm³)";
            qvm.WeightTitle = "重量(kg)";
            qvm.TimestampTitle = "时间戳";
            qvm.PicTitle = "显示图片";
            qvm.SearchTimeTitle = "起止时间: ";
            qvm.SearchCodeTitle = "条码: ";
            qvm.QueryTitle = "查询";
            qvm.TotalTitle = "共";
            qvm.PageTitle = "页";*/
            this.DataContext = qvm;
        }

        private void Image_MouseUP(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;
            if (img != null)
            {
                DataGridRow row = Utils.GetVisualParent<DataGridRow>(img);
                if (row != null)
                {
                    int rowIndex = DataGridHis.Items.IndexOf(row.DataContext);
                    string fullname = qvm.TempSource[rowIndex].FileFullName;
                    try
                    {
                        if (CurrentImage != null)
                        {
                            CurrentImage.Dispose();
                        }
                        Cv2.DestroyAllWindows();
                        CurrentImage = Cv2.ImRead(fullname);
                        Cv2.NamedWindow("Image", WindowFlags.KeepRatio);
                       // Cv2.SetWindowProperty("Image", WindowProperty.WindowMaximize, 1);
                        
                        int screenWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
                        int screenHeight = (int)System.Windows.SystemParameters.PrimaryScreenHeight;
                        Cv2.MoveWindow("Image", 0, 0);
                        Cv2.ResizeWindow("Image", screenWidth, screenHeight);
                        Cv2.ImShow("Image", CurrentImage);
                        /*
                        IntPtr windowHandle = Cv2.GetWindowHandle("Image");
                        if (windowHandle != IntPtr.Zero)
                        {
                            App.SetForegroundWindow(windowHandle);
                            App.ShowWindow(windowHandle, 3);
                        }*/
                        /*if (CurrentImageProcess != null)
                        {
                            
                            //CurrentImageProcess.Kill();
                            CurrentImageProcess.Dispose();
                        }
                        if (File.Exists(fullname))
                        {
                            CurrentImageProcess = Process.Start(fullname);
                        }
                        */
                    }
                    catch { }
                    
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
            long startDTUnix = -1;
            long endDTUnix = -1;
            string Barcode = "";
            string Command = "";
            if ((string.IsNullOrEmpty(qvm.StartTime) || string.IsNullOrEmpty(qvm.EndTime)) && string.IsNullOrEmpty(qvm.Barcode))
            {
                MessageBox.Show("查询条件为空，请重新输入", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (!string.IsNullOrEmpty(qvm.StartTime) && !string.IsNullOrEmpty(qvm.EndTime))
            {
                try
                {
                    DateTime startDT = DateTime.ParseExact(qvm.StartTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                    startDTUnix = (long)(startDT - DateTime.Parse("1970-1-1", CultureInfo.InvariantCulture)).TotalMilliseconds;
                    DateTime endDT = DateTime.ParseExact(qvm.EndTime, "yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture);
                    endDTUnix = (long)(endDT - DateTime.Parse("1970-1-1", CultureInfo.InvariantCulture)).TotalMilliseconds;
                    if (startDTUnix >= endDTUnix)
                    {
                        MessageBox.Show("起始时间晚于结束时间", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(qvm.Barcode))
                        {
                            Barcode = qvm.Barcode;
                            Command = String.Format("SELECT * FROM info WHERE TimeStamp >= {0} AND TimeStamp <= {1} AND Barcode = \'{2}\'; ", startDTUnix.ToString(), endDTUnix.ToString(), Barcode);
                        }
                        else
                        {
                            Command = String.Format("SELECT * FROM info WHERE TimeStamp >= {0} AND TimeStamp <= {1};", startDTUnix.ToString(), endDTUnix.ToString());
                        }
                    }
                }
                catch
                {
                    startDTUnix = -1;
                    endDTUnix = -1;
                }
            }
            else 
            {
                Barcode = qvm.Barcode;
                Command = String.Format("SELECT * FROM info WHERE Barcode = \'{0}\'; ", Barcode);
            }
            Stopwatch sw = new Stopwatch();
            sw.Start();
            if (!string.IsNullOrEmpty(Command))
            {
                List<History> histories = new List<History>();
                MySqlConnection connection = null;
                MySqlCommand command = null;
                MySqlDataReader reader = null;
                
                try
                {
                    connection = new MySqlConnection(connStr);
                    connection.Open();
                    command = new MySqlCommand(Command, connection);
                    reader = command.ExecuteReader();
                   
                    while (reader.Read())
                    {
                        UInt64 TimeStampUnix = reader.GetUInt64("TimeStamp");
                        DateTime DT = DateTime.Parse("1970-1-1", CultureInfo.InvariantCulture).AddMilliseconds(TimeStampUnix);
                        string TimeStamp = DT.ToString("yyyy/MM/dd HH:mm:ss.fff");
                        string Code = reader.GetString("Barcode");
                        string Length = reader.GetString("Length");
                        string Width = reader.GetString("Width");
                        string Height = reader.GetString("Height");
                        string Weight = reader.GetString("Weight");
                        string Volume = reader.GetString("Volume");
                        string ImageFullName = reader.GetString("ImageFullName");
                        bool ReadStatus = reader.GetByte("ReadStatus") == 1 ? true : false;
                        bool InfoIntact = reader.GetByte("InfoIntact") == 1 ? true : false;
                        string ParcelID = reader.GetString("ParcelID");
                        string Destination = reader.GetString("Destination");
                        History his = new History(ParcelID, Code, Length, Width, Height, Volume, Weight, TimeStamp, ImageFullName, ReadStatus, Destination);
                        histories.Add(his);
                    }
                    sw.Stop();
                    long iii = sw.ElapsedMilliseconds;
                }
                catch { }
                finally
                {
                    string ReadRateNum = "";
                    if (histories.Count == 0)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            histories.Add(new History(i.ToString(), "", "", "", "", "", "", "", "", false, ""));
                        }

                        //histories = null;
                    }
                    else
                    {
                        UInt64 total = 0;
                        UInt64 read = 0;
                        for (int i = 0; i < histories.Count; i++)
                        {
                            histories[i].Id = i.ToString();
                            total++;
                            if (histories[i].ReadStatus)
                            {
                                read++;
                            }
                        }
                        if (total > 0)
                        {
                            ReadRateNum = (100 * (read * 1.0) / (total * 1.0)).ToString("F2") + "%";
                        }
                    }
                    qvm.ReadRate = qvm.ReadRate.Substring(0, qvm.ReadRate.IndexOf(": ") + 2) + ReadRateNum;
                    qvm.Source = histories;
                    if (reader != null)
                    {
                        reader.Dispose();
                    }
                    if (connection != null)
                    {
                        connection.Dispose();
                    }
                    if (command != null)
                    {
                        command.Dispose();
                    }
                }
                
            }
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }
    }
}
