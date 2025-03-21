using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;
using System.IO;
using Newtonsoft.Json;
using Cognex.VisionPro;
using Cognex.VisionPro.ID;
using Cognex.VisionPro.ImageFile;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Calib
{
    public partial class Form1 : Form
    {
        private string NumsPattern = @"^\d+$";
        private string lastdir = Application.StartupPath;
        private CogIDTool mTool = null;
        private CogImageFile aImageFile = null;
        private double ratio = 1;        // 图片的起始显示比例
        private double ratioStep = 0.1;
        private System.Drawing.Size pic_size; 
        private int xPos;
        private int yPos;
        public string CurrentStation = "";
        public Mat mat1;
        public Mat mat2;
        public Mat mat3;
        public Mat mat4;
        public Mat mat5;
        public Mat mat6;
        private static double[] defaulthomo = { 1.0, 0.0, 0.0, 0.0, 1.0, 0.0, 0.0, 0.0, 0.0 };
        //private static Mat defaultMat = new Mat(3, 3, MatType.CV_64FC1, defaulthomo);
        private static Mat defaultMat = Mat.FromPixelData(3, 3, MatType.CV_64FC1, defaulthomo);
        public Mat H12 = defaultMat;
        public Mat H23 = defaultMat;
        public Mat H34 = defaultMat;
        public Mat H45 = defaultMat;
        public Mat H56 = defaultMat;
        public Mat H12ratio = defaultMat;
        public Mat H23ratio = defaultMat;
        public Mat H34ratio = defaultMat;
        public Mat H45ratio = defaultMat;
        public Mat H56ratio = defaultMat;
        private List<Point2d> P11 = new List<Point2d>();
        private List<Point2d> P12 = new List<Point2d>();
        private List<Point2d> P13 = new List<Point2d>();
        private List<Point2d> P14 = new List<Point2d>();
        private List<Point2d> P21 = new List<Point2d>();
        private List<Point2d> P22 = new List<Point2d>();
        private List<Point2d> P23 = new List<Point2d>();
        private List<Point2d> P24 = new List<Point2d>();
        private List<Point2d> P31 = new List<Point2d>();
        private List<Point2d> P32 = new List<Point2d>();
        private List<Point2d> P33 = new List<Point2d>();
        private List<Point2d> P34 = new List<Point2d>();
        private List<Point2d> P41 = new List<Point2d>();
        private List<Point2d> P42 = new List<Point2d>();
        private List<Point2d> P43 = new List<Point2d>();
        private List<Point2d> P44 = new List<Point2d>();
        private List<Point2d> P51 = new List<Point2d>();
        private List<Point2d> P52 = new List<Point2d>();
        private List<Point2d> P53 = new List<Point2d>();
        private List<Point2d> P54 = new List<Point2d>();
        private List<Point2d> P61 = new List<Point2d>();
        private List<Point2d> P62 = new List<Point2d>();
        private List<Point2d> P63 = new List<Point2d>();
        private List<Point2d> P64 = new List<Point2d>();
        /*
        private List<Point2d> P1ratio = new List<Point2d>();
        private List<Point2d> P2ratio = new List<Point2d>();
        private List<Point2d> P3ratio = new List<Point2d>();
        private List<Point2d> P4ratio = new List<Point2d>();
        private List<Point2d> P5ratio = new List<Point2d>();
        private List<Point2d> P6ratio = new List<Point2d>();
        */
        private List<string> Codes1 = new List<string>();
        private List<string> Codes2 = new List<string>();
        private List<string> Codes3 = new List<string>();
        private List<string> Codes4 = new List<string>();
        private List<string> Codes5 = new List<string>();
        private List<string> Codes6 = new List<string>();
        public Form1()
        {
            InitializeComponent();
            RatioCombo.Text = "1/16";
            aImageFile = new CogImageFile();
            mTool = new CogIDTool();
            mTool.RunParams.DisableAllCodes();
            mTool.RunParams.DataMatrix.Enabled = true;
            mTool.RunParams.ProcessingMode = CogIDProcessingModeConstants.IDMax;
            mTool.RunParams.DataMatrix.PerspectiveEnabled = true;
            //mTool.RunParams.DataMatrix.PrintDeformation = CogIDPrintDeformationConstants.Moderate;
            mTool.RunParams.NumToFind = 80;
            mTool.RunParams.AllowPartialResults = true;
            mTool.RunParams.Timeout = 4000;
            mTool.RunParams.TimeoutEnabled = true;
            mTool.Run();
            pic_size = CurrentPicBox.Size;
            CalResultBox.Text += "H12 : " + MattoString(H12ratio) + "\r\n";
            CalResultBox.Text += "H23 : " + MattoString(H23ratio) + "\r\n";
            CalResultBox.Text += "H34 : " + MattoString(H34ratio) + "\r\n";
            CalResultBox.Text += "H45 : " + MattoString(H45ratio) + "\r\n";
            CalResultBox.Text += "H56 : " + MattoString(H56ratio) + "\r\n";
            CurrentPicBox.MouseWheel += new MouseEventHandler(CurrentPicBox_MouseWheel);
            this.panel1.MouseWheel += new MouseEventHandler(CurrentPicBox_MouseWheel);
        }
        private string MattoString(Mat mat)
        {
            double[] matArray;
            Mat matt = mat != null ? mat : defaultMat;
            matt.GetArray<double>(out matArray);
            string s = "[";
            for (int i = 0; i < matArray.Length; i++)
            {
                s += matArray[i] + ",";
            }
            if (s.Length >= 1)
            {
                s = s.Substring(0, s.Length - 1);
            }
            s += "]";
            return s;
        }
        private void CurrentPicBox_MouseWheel(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.Delta > 0)
                {
                    ratio += ratioStep;
                    if (ratio > 5) // 放大上限
                    {
                        ratio = 5;
                    }

                    this.changePictureBoxSize(ratio, true);

                }
                else
                {
                    ratio -= ratioStep;
                    if (ratio < 0.5)  // 放大下限
                    {
                        ratio = 0.5;
                    }
                    this.changePictureBoxSize(ratio, false);

                }
            }
            catch
            { }
        }

        private void changePictureBoxSize(double ratio,bool flag)
        {
            var t = CurrentPicBox.Size;
            t.Width = Convert.ToInt32(pic_size.Width * ratio);
            t.Height = Convert.ToInt32(pic_size.Height * ratio);
            CurrentPicBox.Size = t;
            if (!flag && (CurrentPicBox.Width <= panel1.Width || CurrentPicBox.Height <= panel1.Height))
            {
                System.Drawing.Point location = new System.Drawing.Point();
                location.Y = 0;
                location.X = 0;
                CurrentPicBox.Location = location;
            }
        }

        private void CurrentPicBox_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                // 鼠标按下拖拽图片
                if (e.Button == MouseButtons.Left || e.Button == MouseButtons.Right)
                {
                    // 限制拖拽出框
                    if ((CurrentPicBox.Height - panel1.Height) >= 0)
                    {
                        /*
                        if ((CurrentPicBox.Top + Convert.ToInt16(e.Y - yPos)) <= 0
                            || (CurrentPicBox.Left + Convert.ToInt16(e.X - xPos)) <= 0
                            || (CurrentPicBox.Right + Convert.ToInt16(e.X - xPos)) >= panel1.Width
                            || (CurrentPicBox.Bottom + Convert.ToInt16(e.Y - yPos)) >= panel1.Height)
                        {
                            CurrentPicBox.Left += Convert.ToInt16(e.X - xPos);//设置x坐标.
                            CurrentPicBox.Top += Convert.ToInt16(e.Y - yPos);//设置y坐标.
                        }*/
                        if ((CurrentPicBox.Top + Convert.ToInt16(e.Y - yPos)) <= 0 && (CurrentPicBox.Bottom + Convert.ToInt16(e.Y - yPos)) >= panel1.Height)
                        {
                            CurrentPicBox.Top += Convert.ToInt16(e.Y - yPos);
                        }
                        else if ((CurrentPicBox.Top + Convert.ToInt16(e.Y - yPos)) > 0)
                        {
                            CurrentPicBox.Top = 0;
                        }
                        else
                        {
                            CurrentPicBox.Top = panel1.Height - CurrentPicBox.Height;
                        }
                    }
                    if ((CurrentPicBox.Width - panel1.Width) >= 0)
                    { 
                        if ((CurrentPicBox.Left + Convert.ToInt16(e.X - xPos)) <= 0
                            && (CurrentPicBox.Right + Convert.ToInt16(e.X - xPos)) >= panel1.Width)
                        {
                            CurrentPicBox.Left += Convert.ToInt16(e.X - xPos);
                        }
                        else if ((CurrentPicBox.Left + Convert.ToInt16(e.X - xPos)) > 0)
                        {
                            CurrentPicBox.Left = 0;
                        }
                        else
                        {
                            CurrentPicBox.Left = panel1.Width - CurrentPicBox.Width;
                        }
                    }
                }
            }
            catch (Exception dd)
            {
                MessageBox.Show(dd.Message);
            }
        }

        private void CurrentPicBox_MouseDown(object sender, MouseEventArgs e)
        {
            xPos = e.X;//当前x坐标.
            yPos = e.Y;//当前y坐标.
        }



       
        private void Decode(string path, Mat mat, List<Point2d> p1, List<Point2d> p2, List<Point2d> p3, List<Point2d> p4, List<string> codes)
        {
            p1.Clear();
            p2.Clear();
            p3.Clear();
            p4.Clear();
            //pr.Clear();
            codes.Clear();
            Bitmap image = null;
            CogImage8Grey aImage = null;
            CogIDResults results = null;
            try
            {
                aImageFile.Open(path, CogImageFileModeConstants.Read);
                image = aImageFile[0].ToBitmap();
                aImage = new CogImage8Grey(image);
                aImageFile.Close();
                mTool.InputImage = aImage;
                mTool.RunParams.Train(aImage,null);
                mTool.Run();
                ICogRunStatus aRunStatus1 = mTool.RunStatus;
                if (aRunStatus1.Result == CogToolResultConstants.Error)
                {
                    MessageBox.Show("解码错误\r\n详细信息: " + aRunStatus1.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Console.WriteLine(aRunStatus1.Message);
                }
                else
                {
                    results = mTool.Results;
                    int count1 = results.Count;
                    for (int i = 0; i < count1; i++)
                    {
                        if (mTool.Results[i].DecodedData != null)
                        {
                            if (mTool.Results[i].DecodedData.DecodedString != "" && mTool.Results[i].DecodedData.DecodedString != null)
                            {
                                CogPolygon pol = mTool.Results[i].BoundsPolygon;
                                codes.Add(mTool.Results[i].DecodedData.DecodedString);
                                //p.Add(new Point2d(mTool.Results[i].CenterX, mTool.Results[i].CenterY));
                                p1.Add(new Point2d(pol.GetVertexX(0), pol.GetVertexY(0)));
                                p2.Add(new Point2d(pol.GetVertexX(1), pol.GetVertexY(1)));
                                p3.Add(new Point2d(pol.GetVertexX(2), pol.GetVertexY(2)));
                                p4.Add(new Point2d(pol.GetVertexX(3), pol.GetVertexY(3)));
                                //pr.Add(new Point2d(ratio * pol.GetVertexX(0), ratio * pol.GetVertexY(0)));
                                Cv2.Circle(mat, new OpenCvSharp.Point(pol.GetVertexX(0), pol.GetVertexY(0)), 25, new Scalar(0, 0, 0), 3);
                            }
                        }
                    }
                }
            }
            catch { }
            finally
            {
                if (results != null)
                {
                    results.Dispose();
                }
                if (image != null)
                {
                    image.Dispose();
                }
                if (aImage != null)
                {
                    aImage.Dispose();
                }
            }
            

        }
        private void SelectImageBtn1_Click(object sender, EventArgs e)
        {
            int i = 0;
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择图片";
            dialog.Multiselect = false;
            if (!Directory.Exists(lastdir))
            {
                lastdir = Application.StartupPath;
            }
            dialog.InitialDirectory = lastdir;
            dialog.RestoreDirectory = true;
            dialog.Filter = "图片|*.jpg;*.png;*.bmp";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lastdir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                    if (mat1 != null)
                    {
                        mat1.Dispose();
                        mat1 = null;
                    }
                    string imgpath = dialog.FileName;
                    mat1 = Cv2.ImRead(imgpath, ImreadModes.Grayscale);
                    if (!mat1.Empty())
                    {
                        //Decode(imgpath, mat1, P1, P1ratio, Codes1);
                        Decode(imgpath, mat1, P11, P12, P13, P14, Codes1);
                    }
                    else
                    {
                        if (mat1 != null)
                        {
                            mat1.Dispose();
                            mat1 = null;
                        }
                        MessageBox.Show("图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat1);
                }
                catch(Exception ex) 
                {
                    MessageBox.Show("图片读取错误\r\n详细信息:\r\n"+ex.Message+"\r\n"+ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SelectImageBtn2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择图片";
            dialog.Multiselect = false;
            if (!Directory.Exists(lastdir))
            {
                lastdir = Application.StartupPath;
            }
            dialog.InitialDirectory = lastdir;
            dialog.RestoreDirectory = true;
            dialog.Filter = "图片|*.jpg;*.png;*.bmp";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lastdir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                    if (mat2 != null)
                    {
                        mat2.Dispose();
                        mat2 = null;
                    }
                    string imgpath = dialog.FileName;
                    mat2 = Cv2.ImRead(imgpath, ImreadModes.Grayscale);
                    if (!mat2.Empty())
                    {
                        //Decode(imgpath, mat2, P2, P2ratio, Codes2);
                        Decode(imgpath, mat2, P21, P22, P23, P24, Codes2);
                    }
                    else
                    {
                        if (mat2 != null)
                        {
                            mat2.Dispose();
                            mat2 = null;
                        }
                        MessageBox.Show("图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat2);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("图片读取错误\r\n详细信息:\r\n" + ex.Message + "\r\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void SelectImageBtn3_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择图片";
            dialog.Multiselect = false;
            if (!Directory.Exists(lastdir))
            {
                lastdir = Application.StartupPath;
            }
            dialog.InitialDirectory = lastdir;
            dialog.RestoreDirectory = true;
            dialog.Filter = "图片|*.jpg;*.png;*.bmp";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lastdir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                    if (mat3 != null)
                    {
                        mat3.Dispose();
                        mat3 = null;
                    }
                    string imgpath = dialog.FileName;
                    mat3 = Cv2.ImRead(imgpath, ImreadModes.Grayscale);
                    if (!mat3.Empty())
                    {
                        //Decode(imgpath, mat3, P3, P3ratio, Codes3);
                        Decode(imgpath, mat3, P31, P32, P33, P34, Codes3);
                    }
                    else
                    {
                        if (mat3 != null)
                        {
                            mat3.Dispose();
                            mat3 = null;
                        }
                        MessageBox.Show("图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat3);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("图片读取错误\r\n详细信息:\r\n" + ex.Message + "\r\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SelectImageBtn4_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择图片";
            dialog.Multiselect = false;
            if (!Directory.Exists(lastdir))
            {
                lastdir = Application.StartupPath;
            }
            dialog.InitialDirectory = lastdir;
            dialog.RestoreDirectory = true;
            dialog.Filter = "图片|*.jpg;*.png;*.bmp";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lastdir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                    if (mat4 != null)
                    {
                        mat4.Dispose();
                        mat4 = null;
                    }
                    string imgpath = dialog.FileName;
                    mat4 = Cv2.ImRead(imgpath, ImreadModes.Grayscale);
                    if (!mat4.Empty())
                    {
                        //Decode(imgpath, mat4, P4, P4ratio, Codes4);
                        Decode(imgpath, mat4, P41, P42, P43, P44, Codes4);
                    }
                    else
                    {
                        if (mat4 != null)
                        {
                            mat4.Dispose();
                            mat4 = null;
                        }
                        MessageBox.Show("图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat4);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("图片读取错误\r\n详细信息:\r\n" + ex.Message + "\r\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SelectImageBtn5_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择图片";
            dialog.Multiselect = false;
            if (!Directory.Exists(lastdir))
            {
                lastdir = Application.StartupPath;
            }
            dialog.InitialDirectory = lastdir;
            dialog.RestoreDirectory = true;
            dialog.Filter = "图片|*.jpg;*.png;*.bmp";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lastdir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                    if (mat5 != null)
                    {
                        mat5.Dispose();
                        mat5 = null;
                    }
                    string imgpath = dialog.FileName;
                    mat5 = Cv2.ImRead(imgpath, ImreadModes.Grayscale);
                    if (!mat5.Empty())
                    {
                        //Decode(imgpath, mat5, P5, P5ratio, Codes5);
                        Decode(imgpath, mat5, P51, P52, P53, P54, Codes5);
                    }
                    else
                    {
                        if (mat5 != null)
                        {
                            mat5.Dispose();
                            mat5 = null;
                        }
                        MessageBox.Show("图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat5);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("图片读取错误\r\n详细信息:\r\n" + ex.Message + "\r\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void SelectImageBtn6_Click(object sender, EventArgs e)
        {
            
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择图片";
            dialog.Multiselect = false;
            if (!Directory.Exists(lastdir))
            {
                lastdir = Application.StartupPath;
            }
            dialog.InitialDirectory = lastdir;
            dialog.RestoreDirectory = true;
            dialog.Filter = "图片|*.jpg;*.png;*.bmp";
            dialog.FilterIndex = 2;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    lastdir = dialog.FileName.Substring(0, dialog.FileName.LastIndexOf(@"\"));
                    if (mat6 != null)
                    {
                        mat6.Dispose();
                        mat6 = null;
                    }
                    string imgpath = dialog.FileName;
                    mat6 = Cv2.ImRead(imgpath, ImreadModes.Grayscale);
                    if (!mat6.Empty())
                    {
                        //Decode(imgpath, mat6, P6, P6ratio, Codes6);
                        Decode(imgpath, mat6, P61, P62, P63, P64, Codes6);
                    }
                    else
                    {
                        if (mat6 != null)
                        {
                            mat6.Dispose();
                            mat6 = null;
                        }
                        MessageBox.Show("图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat6);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("图片读取错误\r\n详细信息:\r\n" + ex.Message + "\r\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void preview(Mat mat)
        {
            if (CurrentPicBox.Image != null)
            {
                CurrentPicBox.Image.Dispose();
            }
            if (mat != null)
            {
                CurrentPicBox.Width = mat.Cols/10;
                CurrentPicBox.Height = mat.Rows/10;
                //panel1.Width = mat.Cols / 10;
                //panel1.Height = mat.Rows / 10;
                System.Drawing.Point location = new System.Drawing.Point();
                location.Y = 0;
                location.X = 0;
                CurrentPicBox.Location = location;
                pic_size = CurrentPicBox.Size;
                MemoryStream ms = null;
                try
                {
                    ms = mat.ToMemoryStream();
                    CurrentPicBox.Image = Image.FromStream(ms);
                    ratio = 1;
                }
                catch { }
                finally
                {
                    if (ms != null)
                    {
                        ms.Dispose();
                    }
                }
            }
            else
            {
                CurrentPicBox.Image = null;
            }
        }
        private void PreviewBtn1_Click(object sender, EventArgs e)
        {
            preview(mat1);
        }

        private void PreviewBtn2_Click(object sender, EventArgs e)
        {
            preview(mat2);
        }

        private void PreviewBtn3_Click(object sender, EventArgs e)
        {
            preview(mat3);
        }

        private void PreviewBtn4_Click(object sender, EventArgs e)
        {
            preview(mat4);
        }

        private void PreviewBtn5_Click(object sender, EventArgs e)
        {
            preview(mat5);
        }

        private void PreviewBtn6_Click(object sender, EventArgs e)
        {
            preview(mat6);
        }
        private void StartCal_Btn_Click(object sender, EventArgs e)
        {
            try
            {
                double imgratio = 0.25;
                if (RatioCombo.Text == "1")
                {
                    imgratio = 1;
                }
                else if (RatioCombo.Text == "1/4")
                {
                    imgratio = 0.5;
                }
                else if (RatioCombo.Text == "1/16")
                {
                    imgratio = 0.25;
                }
                else if (RatioCombo.Text == "1/64")
                {
                    imgratio = 0.125;
                }
                List<Point2d> P121 = new List<Point2d>();
                List<Point2d> P122 = new List<Point2d>();
                List<Point2d> P232 = new List<Point2d>();
                List<Point2d> P233 = new List<Point2d>();
                List<Point2d> P343 = new List<Point2d>();
                List<Point2d> P344 = new List<Point2d>();
                List<Point2d> P454 = new List<Point2d>();
                List<Point2d> P455 = new List<Point2d>();
                List<Point2d> P565 = new List<Point2d>();
                List<Point2d> P566 = new List<Point2d>();
                List<Point2d> P121ratio = new List<Point2d>();
                List<Point2d> P122ratio = new List<Point2d>();
                List<Point2d> P232ratio = new List<Point2d>();
                List<Point2d> P233ratio = new List<Point2d>();
                List<Point2d> P343ratio = new List<Point2d>();
                List<Point2d> P344ratio = new List<Point2d>();
                List<Point2d> P454ratio = new List<Point2d>();
                List<Point2d> P455ratio = new List<Point2d>();
                List<Point2d> P565ratio = new List<Point2d>();
                List<Point2d> P566ratio = new List<Point2d>();
                if (H12 != null && H12 != defaultMat)
                {
                    H12.Dispose();
                    H12 = defaultMat;
                }
                if (H23 != null && H23 != defaultMat)
                {
                    H23.Dispose();
                    H23 = defaultMat;
                }
                if (H34 != null && H34 != defaultMat)
                {
                    H34.Dispose();
                    H34 = defaultMat;
                }
                if (H45 != null && H45 != defaultMat)
                {
                    H45.Dispose();
                    H45 = defaultMat;
                }
                if (H56 != null && H56 != defaultMat)
                {
                    H56.Dispose();
                    H56 = defaultMat;
                }
                if (H12ratio != null && H12ratio != defaultMat)
                {
                    H12ratio.Dispose();
                    H12ratio = defaultMat;
                }
                if (H23ratio != null && H23ratio != defaultMat)
                {
                    H23ratio.Dispose();
                    H23ratio = defaultMat;
                }
                if (H34ratio != null && H34ratio != defaultMat)
                {
                    H34ratio.Dispose();
                    H34ratio = defaultMat;
                }
                if (H45ratio != null && H45ratio != defaultMat)
                {
                    H45ratio.Dispose();
                    H45ratio = defaultMat;
                }
                if (H56ratio != null && H56ratio != defaultMat)
                {
                    H56ratio.Dispose();
                    H56ratio = defaultMat;
                }
                for (int i = 0; i < Codes1.Count; i++)
                {
                    int j = Codes2.IndexOf(Codes1[i]);
                    if (j > -1)
                    {
                        P121.Add(P11[i]);
                        P122.Add(P21[j]);
                        P121ratio.Add(P11[i] * imgratio);
                        P122ratio.Add(P21[j] * imgratio);
                        P121.Add(P12[i]);
                        P122.Add(P22[j]);
                        P121ratio.Add(P12[i] * imgratio);
                        P122ratio.Add(P22[j] * imgratio);
                        P121.Add(P13[i]);
                        P122.Add(P23[j]);
                        P121ratio.Add(P13[i] * imgratio);
                        P122ratio.Add(P23[j] * imgratio);
                        P121.Add(P14[i]);
                        P122.Add(P24[j]);
                        P121ratio.Add(P14[i] * imgratio);
                        P122ratio.Add(P24[j] * imgratio);
                    }
                }
                for (int i = 0; i < Codes2.Count; i++)
                {
                    int j = Codes3.IndexOf(Codes2[i]);
                    if (j > -1)
                    {
                        P232.Add(P21[i]);
                        P233.Add(P31[j]);
                        P232ratio.Add(P21[i] * imgratio);
                        P233ratio.Add(P31[j] * imgratio);
                        P232.Add(P22[i]);
                        P233.Add(P32[j]);
                        P232ratio.Add(P22[i] * imgratio);
                        P233ratio.Add(P32[j] * imgratio);
                        P232.Add(P23[i]);
                        P233.Add(P33[j]);
                        P232ratio.Add(P23[i] * imgratio);
                        P233ratio.Add(P33[j] * imgratio);
                        P232.Add(P24[i]);
                        P233.Add(P34[j]);
                        P232ratio.Add(P24[i] * imgratio);
                        P233ratio.Add(P34[j] * imgratio);
                    }
                }
                for (int i = 0; i < Codes3.Count; i++)
                {
                    int j = Codes4.IndexOf(Codes3[i]);
                    if (j > -1)
                    {
                        P343.Add(P31[i]);
                        P344.Add(P41[j]);
                        P343ratio.Add(P31[i] * imgratio);
                        P344ratio.Add(P41[j] * imgratio);
                        P343.Add(P32[i]);
                        P344.Add(P42[j]);
                        P343ratio.Add(P32[i] * imgratio);
                        P344ratio.Add(P42[j] * imgratio);
                        P343.Add(P33[i]);
                        P344.Add(P43[j]);
                        P343ratio.Add(P33[i] * imgratio);
                        P344ratio.Add(P43[j] * imgratio);
                        P343.Add(P34[i]);
                        P344.Add(P44[j]);
                        P343ratio.Add(P34[i] * imgratio);
                        P344ratio.Add(P44[j] * imgratio);
                    }
                }
                for (int i = 0; i < Codes4.Count; i++)
                {
                    int j = Codes5.IndexOf(Codes4[i]);
                    if (j > -1)
                    {
                        P454.Add(P41[i]);
                        P455.Add(P51[j]);
                        P454ratio.Add(P41[i] * imgratio);
                        P455ratio.Add(P51[j] * imgratio);
                        P454.Add(P42[i]);
                        P455.Add(P52[j]);
                        P454ratio.Add(P42[i] * imgratio);
                        P455ratio.Add(P52[j] * imgratio);
                        P454.Add(P43[i]);
                        P455.Add(P53[j]);
                        P454ratio.Add(P43[i] * imgratio);
                        P455ratio.Add(P53[j] * imgratio);
                        P454.Add(P44[i]);
                        P455.Add(P54[j]);
                        P454ratio.Add(P44[i] * imgratio);
                        P455ratio.Add(P54[j] * imgratio);
                    }
                }
                for (int i = 0; i < Codes5.Count; i++)
                {
                    int j = Codes6.IndexOf(Codes5[i]);
                    if (j > -1)
                    {
                        P565.Add(P51[i]);
                        P566.Add(P61[j]);
                        P565ratio.Add(P51[i] * imgratio);
                        P566ratio.Add(P61[j] * imgratio);
                        P565.Add(P52[i]);
                        P566.Add(P62[j]);
                        P565ratio.Add(P52[i] * imgratio);
                        P566ratio.Add(P62[j] * imgratio);
                        P565.Add(P53[i]);
                        P566.Add(P63[j]);
                        P565ratio.Add(P53[i] * imgratio);
                        P566ratio.Add(P63[j] * imgratio);
                        P565.Add(P54[i]);
                        P566.Add(P64[j]);
                        P565ratio.Add(P54[i] * imgratio);
                        P566ratio.Add(P64[j] * imgratio);
                    }
                }
                if (P122.Count >= 4 && P121.Count >= 4 && P122ratio.Count >= 4 && P121ratio.Count >= 4)
                {
                    H12 = Cv2.FindHomography(P122, P121, HomographyMethods.Rho);
                    H12ratio = Cv2.FindHomography(P122ratio, P121ratio, HomographyMethods.Rho);
                }
                else
                {
                    MessageBox.Show("图片一与图片二之间标定错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (P233.Count >= 4 && P232.Count >= 4 && P233ratio.Count >= 4 && P232ratio.Count >= 4)
                {
                    H23 = Cv2.FindHomography(P233, P232, HomographyMethods.Rho);
                    H23ratio = Cv2.FindHomography(P233ratio, P232ratio, HomographyMethods.Rho);
                }
                else
                {
                    MessageBox.Show("图片二与图片三之间标定错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (P344.Count >= 4 && P343.Count >= 4 && P344ratio.Count >= 4 && P343ratio.Count >= 4)
                {
                    H34 = Cv2.FindHomography(P344, P343, HomographyMethods.Rho);
                    H34ratio = Cv2.FindHomography(P344ratio, P343ratio, HomographyMethods.Rho);
                }
                else
                {
                    MessageBox.Show("图片三与图片四之间标定错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (P455.Count >= 4 && P454.Count >= 4 && P455ratio.Count >= 4 && P454ratio.Count >= 4)
                {
                    H45 = Cv2.FindHomography(P455, P454, HomographyMethods.Rho);
                    H45ratio = Cv2.FindHomography(P455ratio, P454ratio, HomographyMethods.Rho);
                }
                else
                {
                    if (mat5 != null)
                    {
                        MessageBox.Show("图片四与图片五之间标定错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                if (P566.Count >= 4 && P565.Count >= 4 && P566ratio.Count >= 4 && P565ratio.Count >= 4)
                {
                    H56 = Cv2.FindHomography(P566, P565, HomographyMethods.Rho);
                    H56ratio = Cv2.FindHomography(P566ratio, P565ratio, HomographyMethods.Rho);
                }
                else
                {
                    if (mat5 != null && mat6 != null)
                    {
                        MessageBox.Show("图片五与图片六之间标定错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                Mat[] H = { H12, H23, H34, H45, H56 };
                Mat[] mats = { mat1, mat2, mat3, mat4, mat5, mat6 };
                CalResultBox.Text = "";
                CalResultBox.Text += "H12 : " + MattoString(H12ratio) + "\r\n";
                CalResultBox.Text += "H23 : " + MattoString(H23ratio) + "\r\n";
                CalResultBox.Text += "H34 : " + MattoString(H34ratio) + "\r\n";
                CalResultBox.Text += "H45 : " + MattoString(H45ratio) + "\r\n";
                CalResultBox.Text += "H56 : " + MattoString(H56ratio) + "\r\n";
                if (mat1 != null)
                {
                    Mat warp = null;
                    try
                    {
                        warp = new Mat(mat1.Rows, 4 * mat1.Cols, MatType.CV_8UC1);
                        for (int i = mats.Length - 1; i > 0; i--)
                        {
                            if (mats[i] != null && H[i - 1] != null && H[i - 1] != defaultMat)
                            {
                                warp[0, Math.Min(warp.Rows,mats[i].Rows), 0, mats[i].Cols] = mats[i][0, Math.Min(warp.Rows, mats[i].Rows), 0, mats[i].Cols];
                                Cv2.WarpPerspective(warp, warp, H[i - 1], warp.Size());
                            }
                        }
                        warp[0, mat1.Rows, 0, mat1.Cols] = mat1;
                        preview(warp);
                    }
                    catch { }
                    finally
                    {
                        if (warp != null)
                        {
                            warp.Dispose();
                        }
                    }
                }
                else
                {
                    preview(null);
                }
            }
            catch { }
        }

        private void SAVE_Btn_Click(object sender, EventArgs e)
        {
            string dir = Application.StartupPath + "\\bottomscan calibration";
            string path = "";
            if (StaCombo.Text == "工位一")
            {
                path = dir + "\\bottom1.txt";
            }
            else if (StaCombo.Text == "工位二")
            {
                path = dir + "\\bottom2.txt";
            }
            else if (StaCombo.Text == "工位三")
            {
                path = dir + "\\bottom3.txt";
            }
            else if (StaCombo.Text == "工位四")
            {
                path = dir + "\\bottom4.txt";
            }
            else
            {
                MessageBox.Show("请选择工位", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            double imgratio = 0.25;
            if (RatioCombo.Text == "1")
            {
                imgratio = 1;
            }
            else if (RatioCombo.Text == "1/4")
            {
                imgratio = 0.5;
            }
            else if (RatioCombo.Text == "1/16")
            {
                imgratio = 0.25;
            }
            else if (RatioCombo.Text == "1/64")
            {
                imgratio = 0.125;
            }
            DialogResult result = MessageBox.Show("是否确定要保存标定信息", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)
            {
                if (path != "")
                {
                    FileStream fs = null;
                    StreamWriter sw = null;
                    try
                    {
                        if (!Directory.Exists(dir))
                        {
                            Directory.CreateDirectory(dir);
                        }
                        if (System.IO.File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                        double[] H12array;
                        double[] H23array;
                        double[] H34array;
                        double[] H45array;
                        double[] H56array;
                        int ol1 = 0;
                        int ol2 = 0;
                        int ol3 = 0;
                        int ol4 = 0;
                        int ol5 = 0;
                        int ol6 = 0;
                        if (Regex.IsMatch(ol1text.Text, NumsPattern))
                        {
                            try
                            {
                                ol1 = Convert.ToInt32(Convert.ToInt32(ol1text.Text) * imgratio);
                            }
                            catch
                            {
                                ol1 = 0;
                            }
                        }
                        else
                        {
                            ol1 = 0;
                        }
                        if (Regex.IsMatch(ol2text.Text, NumsPattern))
                        {
                            try
                            {
                                ol2 = Convert.ToInt32(Convert.ToInt32(ol2text.Text) * imgratio);
                            }
                            catch
                            {
                                ol2 = 0;
                            }
                        }
                        else
                        {
                            ol2 = 0;
                        }
                        if (Regex.IsMatch(ol3text.Text, NumsPattern))
                        {
                            try
                            {
                                ol3 = Convert.ToInt32(Convert.ToInt32(ol3text.Text) * imgratio);
                            }
                            catch
                            {
                                ol3 = 0;
                            }
                        }
                        else
                        {
                            ol3 = 0;
                        }
                        if (Regex.IsMatch(ol4text.Text, NumsPattern))
                        {
                            try
                            {
                                ol4 = Convert.ToInt32(Convert.ToInt32(ol4text.Text) * imgratio);
                            }
                            catch
                            {
                                ol4 = 0;
                            }
                        }
                        else
                        {
                            ol4 = 0;
                        }
                        if (Regex.IsMatch(ol5text.Text, NumsPattern))
                        {
                            try
                            {
                                ol5 = Convert.ToInt32(Convert.ToInt32(ol5text.Text) * imgratio);
                            }
                            catch
                            {
                                ol5 = 0;
                            }
                        }
                        else
                        {
                            ol5 = 0;
                        }
                        if (Regex.IsMatch(ol6text.Text, NumsPattern))
                        {
                            try
                            {
                                ol6 = Convert.ToInt32(Convert.ToInt32(ol6text.Text) * imgratio);
                            }
                            catch
                            {
                                ol6 = 0;
                            }
                        }
                        else
                        {
                            ol6 = 0;
                        }
                        int[] overlaparray = { ol1, ol2, ol3, ol4, ol5, ol6 };
                        if (H12ratio != null && H23ratio != null && H34ratio != null && H45ratio != null && H56ratio != null)
                        {
                            H12ratio.GetArray<double>(out H12array);
                            H23ratio.GetArray<double>(out H23array);
                            H34ratio.GetArray<double>(out H34array);
                            H45ratio.GetArray<double>(out H45array);
                            H56ratio.GetArray<double>(out H56array);
                            var calibjsonObject = new
                            {
                                H12 = H12array,
                                H23 = H23array,
                                H34 = H34array,
                                H45 = H45array,
                                H56 = H56array,
                                overlap = overlaparray
                            };
                            string jsonstr = JsonConvert.SerializeObject(calibjsonObject);
                            fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                            sw = new StreamWriter(fs);
                            sw.WriteLine(jsonstr);
                            sw.Close();
                            fs.Close();
                            MessageBox.Show(StaCombo.Text + "标定结果保存成功", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        { 
                            MessageBox.Show(StaCombo.Text + "标定结果保存失败\r\n无标定信息！", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(StaCombo.Text + "标定结果保存失败\r\n" + ex.StackTrace, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        if (sw != null)
                        {
                            sw.Dispose();
                        }
                        if (fs != null)
                        {
                            fs.Dispose();
                        }
                    }
                }
            }
        }

        private void DeleteImageBtn1_Click(object sender, EventArgs e)
        {
            P11.Clear();
            P12.Clear();
            P13.Clear();
            P14.Clear();
            //P1ratio.Clear();
            Codes1.Clear();
            if (mat1 != null)
            {
                mat1.Dispose();
                mat1 = null;
            }
        }

        private void DeleteImageBtn2_Click(object sender, EventArgs e)
        {
            P21.Clear();
            P22.Clear();
            P23.Clear();
            P24.Clear();
            //P2ratio.Clear();
            Codes2.Clear();
            if (mat2 != null)
            {
                mat2.Dispose();
                mat2 = null;
            }
        }

        private void DeleteImageBtn3_Click(object sender, EventArgs e)
        {
            P31.Clear();
            P32.Clear();
            P33.Clear();
            P34.Clear();
            //P3ratio.Clear();
            Codes3.Clear();
            if (mat3 != null)
            {
                mat3.Dispose();
                mat3 = null;
            }

        }

        private void DeleteImageBtn4_Click(object sender, EventArgs e)
        {
            P41.Clear();
            P42.Clear();
            P43.Clear();
            P44.Clear();
            //P4ratio.Clear();
            Codes4.Clear();
            if (mat4 != null)
            {
                mat4.Dispose();
                mat4 = null;
            }
        }

        private void DeleteImageBtn5_Click(object sender, EventArgs e)
        {
            P51.Clear();
            P52.Clear();
            P53.Clear();
            P54.Clear();
            //P5ratio.Clear();
            Codes5.Clear();
            if (mat5 != null)
            {
                mat5.Dispose();
                mat5 = null;
            }

        }

        private void DeleteImageBtn6_Click(object sender, EventArgs e)
        {
            P61.Clear();
            P62.Clear();
            P63.Clear();
            P64.Clear();
            //P6ratio.Clear();
            Codes6.Clear();
            if (mat6 != null)
            {
                mat6.Dispose();
                mat6 = null;
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
            //System.Environment.Exit(0);
            Process.GetCurrentProcess().Kill();
        }

        private void SelectMultiImageButton_Click(object sender, EventArgs e)
        {
            string path = "";
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.Description = "请选择文件路径";
            Mat[] BT01 = { null, null, null, null };
            Mat[] BT02 = { null, null, null, null };
            Mat[] BT03 = { null, null, null, null };
            Mat[] BT04 = { null, null, null, null };
            Mat[] BT05 = { null, null, null, null };
            Mat[] BT06 = { null, null, null, null };
            try
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    path = dialog.SelectedPath;
                    P11.Clear();
                    P12.Clear();
                    P13.Clear();
                    P14.Clear();
                    //P1ratio.Clear();
                    Codes1.Clear();
                    if (mat1 != null)
                    {
                        mat1.Dispose();
                        mat1 = null;
                    }
                    P21.Clear();
                    P22.Clear();
                    P23.Clear();
                    P24.Clear();
                    //P2ratio.Clear();
                    Codes2.Clear();
                    if (mat2 != null)
                    {
                        mat2.Dispose();
                        mat2 = null;
                    }
                    P31.Clear();
                    P32.Clear();
                    P33.Clear();
                    P34.Clear();
                    //P3ratio.Clear();
                    Codes3.Clear();
                    if (mat3 != null)
                    {
                        mat3.Dispose();
                        mat3 = null;
                    }
                    P41.Clear();
                    P42.Clear();
                    P43.Clear();
                    P44.Clear();
                    //P4ratio.Clear();
                    Codes4.Clear();
                    if (mat4 != null)
                    {
                        mat4.Dispose();
                        mat4 = null;
                    }
                    P51.Clear();
                    P52.Clear();
                    P53.Clear();
                    P54.Clear();
                    //P5ratio.Clear();
                    Codes5.Clear();
                    if (mat5 != null)
                    {
                        mat5.Dispose();
                        mat5 = null;
                    }
                    P61.Clear();
                    P62.Clear();
                    P63.Clear();
                    P64.Clear();
                    //P6ratio.Clear();
                    Codes6.Clear();
                    if (mat6 != null)
                    {
                        mat6.Dispose();
                        mat6 = null;
                    }
                    DirectoryInfo directoryinfo = new DirectoryInfo(path);
                    FileInfo[] Files = directoryinfo.GetFiles();
                    int cnt = 0;
                    foreach (FileInfo file in Files)
                    {
                        int N1pos = file.Name.IndexOf("_1,");
                        int N2pos = file.Name.IndexOf("_2,");
                        if (N1pos > -1 && N2pos > -1)
                        {
                            string N1 = file.Name.Substring(N1pos + 3, N2pos - N1pos - 3);
                            Mat[] marray = null;
                            if (N1.StartsWith("BT01"))
                            {
                                marray = BT01;
                            }
                            else if (N1.StartsWith("BT02"))
                            {
                                marray = BT02;
                            }
                            else if (N1.StartsWith("BT03"))
                            {
                                marray = BT03;
                            }
                            else if (N1.StartsWith("BT04"))
                            {
                                marray = BT04;
                            }
                            else if (N1.StartsWith("BT05"))
                            {
                                marray = BT05;
                            }
                            else if (N1.StartsWith("BT06"))
                            {
                                marray = BT06;
                            }
                            int id = -1;
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
                            if (id > -1 && id < 4 && marray != null)
                            {
                                if (marray[id] == null && file.Exists)
                                {
                                    marray[id] = Cv2.ImRead(file.FullName, ImreadModes.Grayscale);
                                    if (marray[id] != null)
                                    {
                                        if (!marray[id].Empty())
                                        {
                                            cnt++;
                                        }
                                        else
                                        {
                                            marray[id].Dispose();
                                            marray[id] = null;
                                        }
                                    }
                                }
                            }
                            if (cnt >= 24)
                            {
                                break;
                            }
                        }
                    }
                    int ol1 = 0;
                    int ol2 = 0;
                    int ol3 = 0;
                    int ol4 = 0;
                    int ol5 = 0;
                    int ol6 = 0;
                    if (Regex.IsMatch(ol1text.Text, NumsPattern))
                    {
                        try
                        {
                            ol1 = Convert.ToInt32(ol1text.Text);
                        }
                        catch
                        {
                            ol1 = 0;
                        }
                    }
                    else
                    {
                        ol1 = 0;
                    }
                    if (Regex.IsMatch(ol2text.Text, NumsPattern))
                    {
                        try
                        {
                            ol2 = Convert.ToInt32(ol2text.Text);
                        }
                        catch
                        {
                            ol2 = 0;
                        }
                    }
                    else
                    {
                        ol2 = 0;
                    }
                    if (Regex.IsMatch(ol3text.Text, NumsPattern))
                    {
                        try
                        {
                            ol3 = Convert.ToInt32(ol3text.Text);
                        }
                        catch
                        {
                            ol3 = 0;
                        }
                    }
                    else
                    {
                        ol3 = 0;
                    }
                    if (Regex.IsMatch(ol4text.Text, NumsPattern))
                    {
                        try
                        {
                            ol4 = Convert.ToInt32(ol4text.Text);
                        }
                        catch
                        {
                            ol4 = 0;
                        }
                    }
                    else
                    {
                        ol4 = 0;
                    }
                    if (Regex.IsMatch(ol5text.Text, NumsPattern))
                    {
                        try
                        {
                            ol5 = Convert.ToInt32(ol5text.Text);
                        }
                        catch
                        {
                            ol5 = 0;
                        }
                    }
                    else
                    {
                        ol5 = 0;
                    }
                    if (Regex.IsMatch(ol6text.Text, NumsPattern))
                    {
                        try
                        {
                            ol6 = Convert.ToInt32(ol6text.Text);
                        }
                        catch
                        {
                            ol6 = 0;
                        }
                    }
                    else
                    {
                        ol6 = 0;
                    }
                    mat1 = StitchHorizontal(BT01, ol1);
                    mat2 = StitchHorizontal(BT02, ol2);
                    mat3 = StitchHorizontal(BT03, ol3);
                    mat4 = StitchHorizontal(BT04, ol4);
                    mat5 = StitchHorizontal(BT05, ol5);
                    mat6 = StitchHorizontal(BT06, ol6);
                    if (mat1 != null)
                    {
                        if (!mat1.Empty())
                        {
                            //Decode(imgpath, mat1, P1, P1ratio, Codes1);
                            Cv2.ImWrite(path + "\\BT01.jpg", mat1, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            Decode(path + "\\BT01.jpg", mat1, P11, P12, P13, P14, Codes1);
                        }
                        else
                        {
                            if (mat1 != null)
                            {
                                mat1.Dispose();
                                mat1 = null;
                            }
                            MessageBox.Show("BT01图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("BT01图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    preview(mat1);
                    if (mat2 != null)
                    {
                        if (!mat2.Empty())
                        {
                            Cv2.ImWrite(path + "\\BT02.jpg", mat2, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            //Decode(imgpath, mat2, P2, P2ratio, Codes2);
                            Decode(path + "\\BT02.jpg", mat2, P21, P22, P23, P24, Codes2);
                        }
                        else
                        {
                            if (mat2 != null)
                            {
                                mat2.Dispose();
                                mat2 = null;
                            }
                            MessageBox.Show("BT02图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("BT02图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (mat3 != null)
                    {
                        if (!mat3.Empty())
                        {
                            Cv2.ImWrite(path + "\\BT03.jpg", mat3, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            //Decode(imgpath, mat3, P3, P3ratio, Codes3);
                            Decode(path + "\\BT03.jpg", mat3, P31, P32, P33, P34, Codes3);
                        }
                        else
                        {
                            if (mat3 != null)
                            {
                                mat3.Dispose();
                                mat3 = null;
                            }
                            MessageBox.Show("BT03图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("BT03图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (mat4 != null)
                    {
                        if (!mat4.Empty())
                        {
                            Cv2.ImWrite(path + "\\BT04.jpg", mat4, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            //Decode(imgpath, mat4, P4, P4ratio, Codes4);
                            Decode(path + "\\BT04.jpg", mat4, P41, P42, P43, P44, Codes4);
                        }
                        else
                        {
                            if (mat4 != null)
                            {
                                mat4.Dispose();
                                mat4 = null;
                            }
                            MessageBox.Show("BT04图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("BT04图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    if (mat5 != null)
                    {
                        if (!mat5.Empty())
                        {
                            Cv2.ImWrite(path + "\\BT05.jpg", mat5, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            //Decode(imgpath, mat5, P5, P5ratio, Codes5);
                            Decode(path + "\\BT05.jpg", mat5, P51, P52, P53, P54, Codes5);
                        }
                        else
                        {
                            if (mat5 != null)
                            {
                                mat5.Dispose();
                                mat5 = null;
                            }
                            MessageBox.Show("BT05图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    if (mat6 != null)
                    {
                        if (!mat6.Empty())
                        {
                            Cv2.ImWrite(path + "\\BT06.jpg", mat6, new ImageEncodingParam(ImwriteFlags.JpegQuality, 90));
                            //Decode(imgpath, mat6, P6, P6ratio, Codes6);
                            Decode(path + "\\BT06.jpg", mat6, P61, P62, P63, P64, Codes6);
                        }
                        else
                        {
                            if (mat6 != null)
                            {
                                mat6.Dispose();
                                mat6 = null;
                            }
                            MessageBox.Show("BT06图片读取错误", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch { }
            finally
            {
                for (int j = 0; j < BT01.Length; j++)
                {
                    if (BT01[j] != null)
                    {
                        BT01[j].Dispose();
                    }
                }
                for (int j = 0; j < BT02.Length; j++)
                {
                    if (BT02[j] != null)
                    {
                        BT02[j].Dispose();
                    }
                }
                for (int j = 0; j < BT03.Length; j++)
                {
                    if (BT03[j] != null)
                    {
                        BT03[j].Dispose();
                    }
                }
                for (int j = 0; j < BT04.Length; j++)
                {
                    if (BT04[j] != null)
                    {
                        BT04[j].Dispose();
                    }
                }
                for (int j = 0; j < BT05.Length; j++)
                {
                    if (BT05[j] != null)
                    {
                        BT05[j].Dispose();
                    }
                }
                for (int j = 0; j < BT06.Length; j++)
                {
                    if (BT06[j] != null)
                    {
                        BT06[j].Dispose();
                    }
                }
            }
        }
        private Mat StitchHorizontal(Mat[] BT, int ol)
        {
            Mat mat = null;
            try
            {
                int BT01col = 0;
                int BT01row = ol;
                int endid01 = -1;
                int currow01 = 0;
                for (int i1 = 0; i1 < BT.Length; i1++)
                {
                    if (BT[i1] != null)
                    {
                        if (!BT[i1].Empty())
                        {
                            BT01row += BT[i1].Rows;
                            BT01row -= ol;
                            BT01col = Math.Max(BT01col, BT[i1].Cols);
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
                if (endid01 > -1)
                {
                    if (BT[0] != null)
                    {
                        mat = new Mat(BT01row, BT01col, MatType.CV_8UC1, new Scalar(0));
                        mat[0, BT[0].Rows, 0, BT[0].Cols] = BT[0];
                        currow01 = BT[0].Rows;
                        for (int ii1 = 1; ii1 <= endid01; ii1++)
                        {
                            if (BT[ii1] != null)
                            {
                                mat[currow01, currow01 + BT[ii1].Rows - ol, 0, BT[ii1].Cols] = BT[ii1][ol, BT[ii1].Rows, 0, BT[ii1].Cols];
                                currow01 += BT[ii1].Rows - ol;
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
            return mat;
        }
    }
}
