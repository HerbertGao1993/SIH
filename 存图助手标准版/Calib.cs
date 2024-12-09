using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Forms;
using System.IO;
using Newtonsoft.Json;

namespace 存图助手标准版
{
    public partial class Calib : Form
    {
        public bool dialogExit = false;
        private bool _cancel = false;
        double Errtemp = 100000;
        double r11 = 0, r12 = 0, r13 = 0, r14 = 0, r21 = 0, r22 = 0, r23 = 0, r24 = 0, r31 = 0, r32 = 0, r33 = 0, r34 = 1;//矩阵待求参数
        public delegate void AddLogTextDelegate(string text);
        public delegate void EnableAndDisableButtonDelegate();
        
        public void AddLogText(string text)
        {
            logBox.Text += text;
            logBox.SelectionStart = logBox.Text.Length;
            logBox.ScrollToCaret();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
       

        public void EnableAndDisableButton()
        {
            if (!BtnUpdateCalib.IsDisposed)
            {
                if (!_cancel)
                {
                    BtnUpdateCalib.Enabled = true;
                }
                else
                {
                    BtnUpdateCalib.Enabled = false;
                }
            }
            if (!BtnCalib.IsDisposed)
            {
                BtnCalib.Enabled = true;
            }
            if (!BtnExit.IsDisposed)
            {
                BtnExit.Enabled = true;
            }
            if (!BtnCancel.IsDisposed)
            {
                BtnCancel.Enabled = false;
            }
            if (!StaCombo.IsDisposed)
            {
                StaCombo.Enabled = true;
            }
            if (!dimCombo.IsDisposed)
            {
                dimCombo.Enabled = true;
            }
            EpochTxt.Enabled = true;
            LrTxt.Enabled = true;
            logIntervalTxt.Enabled = true;
            AdditionalDataCheck.Enabled = true;
            X1Txt.Enabled = true;
            X2Txt.Enabled = true;
            X3Txt.Enabled = true;
            X4Txt.Enabled = true;
            X5Txt.Enabled = true;
            X6Txt.Enabled = true;
            X7Txt.Enabled = true;
            X8Txt.Enabled = true;
            X9Txt.Enabled = true;
            X10Txt.Enabled = true;
            X11Txt.Enabled = true;
            X12Txt.Enabled = true;
            Y1Txt.Enabled = true;
            Y2Txt.Enabled = true;
            Y3Txt.Enabled = true;
            Y4Txt.Enabled = true;
            Y5Txt.Enabled = true;
            Y6Txt.Enabled = true;
            Y7Txt.Enabled = true;
            Y8Txt.Enabled = true;
            Y9Txt.Enabled = true;
            Y10Txt.Enabled = true;
            Y11Txt.Enabled = true;
            Y12Txt.Enabled = true;
            Z1Txt.Enabled = true;
            Z2Txt.Enabled = true;
            Z3Txt.Enabled = true;
            Z4Txt.Enabled = true;
            Z5Txt.Enabled = true;
            Z6Txt.Enabled = true;
            Z7Txt.Enabled = true;
            Z8Txt.Enabled = true;
            Z9Txt.Enabled = true;
            Z10Txt.Enabled = true;
            Z11Txt.Enabled = true;
            Z12Txt.Enabled = true;
            u1Txt.Enabled = true;
            u2Txt.Enabled = true;
            u3Txt.Enabled = true;
            u4Txt.Enabled = true;
            u5Txt.Enabled = true;
            u6Txt.Enabled = true;
            u7Txt.Enabled = true;
            u8Txt.Enabled = true;
            u9Txt.Enabled = true;
            u10Txt.Enabled = true;
            u11Txt.Enabled = true;
            u12Txt.Enabled = true;
            v1Txt.Enabled = true;
            v2Txt.Enabled = true;
            v3Txt.Enabled = true;
            v4Txt.Enabled = true;
            v5Txt.Enabled = true;
            v6Txt.Enabled = true;
            v7Txt.Enabled = true;
            v8Txt.Enabled = true;
            v9Txt.Enabled = true;
            v10Txt.Enabled = true;
            v11Txt.Enabled = true;
            v12Txt.Enabled = true;
        }
        private void Calib_Load(object sender, EventArgs e)
        {

        }

        public Calib()
        {
            InitializeComponent();
            StaCombo.Text = "工位一";
            dimCombo.Text = "DIM0";
            BtnUpdateCalib.Enabled = false;
            BtnCancel.Enabled = false;
            BtnCalib.Enabled = true;
            BtnExit.Enabled = true;
            StaCombo.Enabled = true;
            dimCombo.Enabled = true;
            X1Txt.Enabled = true;
            X2Txt.Enabled = true;
            X3Txt.Enabled = true;
            X4Txt.Enabled = true;
            X5Txt.Enabled = true;
            X6Txt.Enabled = true;
            X7Txt.Enabled = true;
            X8Txt.Enabled = true;
            X9Txt.Enabled = true;
            X10Txt.Enabled = true;
            X11Txt.Enabled = true;
            X12Txt.Enabled = true;
            Y1Txt.Enabled = true;
            Y2Txt.Enabled = true;
            Y3Txt.Enabled = true;
            Y4Txt.Enabled = true;
            Y5Txt.Enabled = true;
            Y6Txt.Enabled = true;
            Y7Txt.Enabled = true;
            Y8Txt.Enabled = true;
            Y9Txt.Enabled = true;
            Y10Txt.Enabled = true;
            Y11Txt.Enabled = true;
            Y12Txt.Enabled = true;
            Z1Txt.Enabled = true;
            Z2Txt.Enabled = true;
            Z3Txt.Enabled = true;
            Z4Txt.Enabled = true;
            Z5Txt.Enabled = true;
            Z6Txt.Enabled = true;
            Z7Txt.Enabled = true;
            Z8Txt.Enabled = true;
            Z9Txt.Enabled = true;
            Z10Txt.Enabled = true;
            Z11Txt.Enabled = true;
            Z12Txt.Enabled = true;
            u1Txt.Enabled = true;
            u2Txt.Enabled = true;
            u3Txt.Enabled = true;
            u4Txt.Enabled = true;
            u5Txt.Enabled = true;
            u6Txt.Enabled = true;
            u7Txt.Enabled = true;
            u8Txt.Enabled = true;
            u9Txt.Enabled = true;
            u10Txt.Enabled = true;
            u11Txt.Enabled = true;
            u12Txt.Enabled = true;
            v1Txt.Enabled = true;
            v2Txt.Enabled = true;
            v3Txt.Enabled = true;
            v4Txt.Enabled = true;
            v5Txt.Enabled = true;
            v6Txt.Enabled = true;
            v7Txt.Enabled = true;
            v8Txt.Enabled = true;
            v9Txt.Enabled = true;
            v10Txt.Enabled = true;
            v11Txt.Enabled = true;
            v12Txt.Enabled = true;

        }

        private void BtnExit_Click(object sender, EventArgs e)
        {
            dialogExit = true;
            this.Hide();
        }
        public double[] LMS(double[] X, double[] Y, double[] Z, double[] x, double[] y, int n)
        {
            double[] r = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            double xerr = 0;
            double yerr = 0;
            double[,] XYZ = new double[2 * n, 11];
            double[,] xy = new double[2 * n, 1];
            for (int i = 0; i < n; i++)
            {
                int i1 = 2 * i;
                int i2 = 2 * i + 1;
                XYZ[i1, 0] = X[i];
                XYZ[i1, 1] = Y[i];
                XYZ[i1, 2] = Z[i];
                XYZ[i1, 3] = 1;
                XYZ[i1, 4] = 0;
                XYZ[i1, 5] = 0;
                XYZ[i1, 6] = 0;
                XYZ[i1, 7] = 0;
                XYZ[i1, 8] = -X[i] * x[i];
                XYZ[i1, 9] = -Y[i] * x[i];
                XYZ[i1, 10] = -Z[i] * x[i];
                XYZ[i2, 0] = 0;
                XYZ[i2, 1] = 0;
                XYZ[i2, 2] = 0;
                XYZ[i2, 3] = 0;
                XYZ[i2, 4] = X[i];
                XYZ[i2, 5] = Y[i];
                XYZ[i2, 6] = Z[i];
                XYZ[i2, 7] = 1;
                XYZ[i2, 8] = -X[i] * y[i];
                XYZ[i2, 9] = -Y[i] * y[i];
                XYZ[i2, 10] = -Z[i] * y[i];
                //XYZ[i1] = { X[i], Y[i], Z[i], 1, 0, 0, 0, 0, -X[i] * x[i], -Y[i] * x[i], -Z[i] * x[i]};
                //XYZ[i2] = { 0, 0, 0, 0, X[i], Y[i], Z[i], 1, -X[i] * y[i], -Y[i] * y[i], -Z[i] * y[i]};
                xy[i1,0] = x[i];
                xy[i2,0] = y[i];
            }
            Matrix3 XYZMatrix = new Matrix3(XYZ);
            Matrix3 xyMatrix = new Matrix3(xy);
            Matrix3 R = (XYZMatrix.Transpose() * XYZMatrix).Inverse() * XYZMatrix.Transpose() * xyMatrix;
            r11 = R[0, 0];
            r12 = R[1, 0];
            r13 = R[2, 0];
            r14 = R[3, 0];
            r21 = R[4, 0];
            r22 = R[5, 0];
            r23 = R[6, 0];
            r24 = R[7, 0];
            r31 = R[8, 0];
            r32 = R[9, 0];
            r33 = R[10, 0];
            for (int i = 0; i < n; i++)
            {
                double XX = X[i];
                double YY = Y[i];
                double ZZ = Z[i];
                double xx = x[i];
                double yy = y[i];
                double xest = (R[0, 0] * XX + R[1, 0] * YY + R[2, 0] * ZZ + R[3, 0]) / (R[8, 0] * XX + R[9, 0] * YY + R[10, 0] * ZZ + 1);
                double yest = (R[4, 0] * XX + R[5, 0] * YY + R[6, 0] * ZZ + R[7, 0]) / (R[8, 0] * XX + R[9, 0] * YY + R[10, 0] * ZZ + 1);
                xerr += Math.Pow(xx - xest, 2);
                yerr += Math.Pow(yy - yest, 2);
            }
            double[] Err = {Math.Sqrt(xerr / n), Math.Sqrt(yerr / n)};
            return Err;
        }
        public void Calculate(double[] X, double[] Y, double[] Z, double[] x, double[] y, int p1, int p2, int p3, int p4, int p5, int p6)
        {
            try
            {
                double xerr = 0;
                double yerr = 0;
                double[,] XYZ = { { X[p1-1], Y[p1-1], Z[p1-1], 1, 0, 0, 0, 0, -X[p1-1]*x[p1-1], -Y[p1-1]*x[p1-1], -Z[p1-1]*x[p1-1] },
                                  { 0, 0, 0, 0, X[p1-1], Y[p1-1], Z[p1-1], 1, -X[p1-1]*y[p1-1], -Y[p1-1]*y[p1-1], -Z[p1-1]*y[p1-1] },
                                  { X[p2-1], Y[p2-1], Z[p2-1], 1, 0, 0, 0, 0, -X[p2-1]*x[p2-1], -Y[p2-1]*x[p2-1], -Z[p2-1]*x[p2-1] },
                                  { 0, 0, 0, 0, X[p2-1], Y[p2-1], Z[p2-1], 1, -X[p2-1]*y[p2-1], -Y[p2-1]*y[p2-1], -Z[p2-1]*y[p2-1] },
                                  { X[p3-1], Y[p3-1], Z[p3-1], 1, 0, 0, 0, 0, -X[p3-1]*x[p3-1], -Y[p3-1]*x[p3-1], -Z[p3-1]*x[p3-1] },
                                  { 0, 0, 0, 0, X[p3-1], Y[p3-1], Z[p3-1], 1, -X[p3-1]*y[p3-1], -Y[p3-1]*y[p3-1], -Z[p3-1]*y[p3-1] },
                                  { X[p4-1], Y[p4-1], Z[p4-1], 1, 0, 0, 0, 0, -X[p4-1]*x[p4-1], -Y[p4-1]*x[p4-1], -Z[p4-1]*x[p4-1] },
                                  { 0, 0, 0, 0, X[p4-1], Y[p4-1], Z[p4-1], 1, -X[p4-1]*y[p4-1], -Y[p4-1]*y[p4-1], -Z[p4-1]*y[p4-1] },
                                  { X[p5-1], Y[p5-1], Z[p5-1], 1, 0, 0, 0, 0, -X[p5-1]*x[p5-1], -Y[p5-1]*x[p5-1], -Z[p5-1]*x[p5-1] },
                                  { 0, 0, 0, 0, X[p5-1], Y[p5-1], Z[p5-1], 1, -X[p5-1]*y[p5-1], -Y[p5-1]*y[p5-1], -Z[p5-1]*y[p5-1] },
                                  { X[p6-1], Y[p6-1], Z[p6-1], 1, 0, 0, 0, 0, -X[p6-1]*x[p6-1], -Y[p6-1]*x[p6-1], -Z[p6-1]*x[p6-1] }};
                Matrix3 XYZMatrix = new Matrix3(XYZ);
                double[,] xy = new double[11, 1];
                xy[0, 0] = x[p1 - 1];
                xy[1, 0] = y[p1 - 1];
                xy[2, 0] = x[p2 - 1];
                xy[3, 0] = y[p2 - 1];
                xy[4, 0] = x[p3 - 1];
                xy[5, 0] = y[p3 - 1];
                xy[6, 0] = x[p4 - 1];
                xy[7, 0] = y[p4 - 1];
                xy[8, 0] = x[p5 - 1];
                xy[9, 0] = y[p5 - 1];
                xy[10, 0] = x[p6 - 1];
                Matrix3 xyMatrix = new Matrix3(xy);
                Matrix3 R1 = XYZMatrix.Inverse() * xyMatrix;
                for (int i = 0; i < X.Length; i++)
                {
                    double XX = X[i];
                    double YY = Y[i];
                    double ZZ = Z[i];
                    double xx = x[i];
                    double yy = y[i];
                    double xest = (R1[0, 0] * XX + R1[1, 0] * YY + R1[2, 0] * ZZ + R1[3, 0]) / (R1[8, 0] * XX + R1[9, 0] * YY + R1[10, 0] * ZZ + 1);
                    double yest = (R1[4, 0] * XX + R1[5, 0] * YY + R1[6, 0] * ZZ + R1[7, 0]) / (R1[8, 0] * XX + R1[9, 0] * YY + R1[10, 0] * ZZ + 1);
                    xerr += Math.Pow(xx - xest, 2);
                    yerr += Math.Pow(yy - yest, 2);
                }
                double Err = Math.Sqrt(xerr / X.Length) + Math.Sqrt(yerr / X.Length);
                if (Errtemp == 100000 || Err < Errtemp)
                {
                    Errtemp = Err;
                    r11 = R1[0, 0];
                    r12 = R1[1, 0];
                    r13 = R1[2, 0];
                    r14 = R1[3, 0];
                    r21 = R1[4, 0];
                    r22 = R1[5, 0];
                    r23 = R1[6, 0];
                    r24 = R1[7, 0];
                    r31 = R1[8, 0];
                    r32 = R1[9, 0];
                    r33 = R1[10, 0];
                }
            }
            catch (Exception ex) { }
        }
        public double[] gradientdescent(double[,] input, double[,] output, double[] initialarg, Int64 epoch, double lr = 1E-12, double loginterval = 100000)
        {
            double lasterr = 0;
            double p = 0.1;
            double curlr = lr;
            double[] finalarg = initialarg;
            double r11 = finalarg[0];
            double r12 = finalarg[1];
            double r13 = finalarg[2];
            double r14 = finalarg[3];
            double r21 = finalarg[4];
            double r22 = finalarg[5];
            double r23 = finalarg[6];
            double r24 = finalarg[7];
            double r31 = finalarg[8];
            double r32 = finalarg[9];
            double r33 = finalarg[10];
            double xerr = 0;
            double yerr = 0;
            double err = 0;
            for (int i = 0; i < input.GetLength(0); i++)
            {
                double X = input[i, 0];
                double Y = input[i, 1];
                double Z = input[i, 2];
                double x = output[i, 0];
                double y = output[i, 1];
                double xest = (r11 * X + r12 * Y + r13 * Z + r14) / (r31 * X + r32 * Y + r33 * Z + 1);
                double yest = (r21 * X + r22 * Y + r23 * Z + r24) / (r31 * X + r32 * Y + r33 * Z + 1);
                xerr += 0.5 * Math.Pow(x - xest, 2);
                yerr += 0.5 * Math.Pow(y - yest, 2);
            }
            lasterr = (xerr + yerr) / input.GetLength(0);
            if (InvokeRequired)
            {
                if (!logBox.IsDisposed)
                {
                    string txt = "";
                    txt += ("initial err: " + (xerr + yerr) / input.GetLength(0)) + "\r\n";
                    txt += ("initial xerr: " + xerr / input.GetLength(0)) + "\r\n";
                    txt += ("initial xerr: " + yerr / input.GetLength(0)) + "\r\n";
                    Invoke(new AddLogTextDelegate(AddLogText), txt);
                }
            }
            /*
            logBox.Text += ("initial err: " + (xerr + yerr) / input.GetLength(0)) + "\r\n";
            logBox.Text += ("initial xerr: " + xerr / input.GetLength(0)) + "\r\n";
            logBox.Text += ("initial xerr: " + yerr / input.GetLength(0)) + "\r\n";*/
            for (int j = 0; j < epoch; j++)
            {
                if (_cancel)
                {
                    break;
                }
                curlr = lr;
                if (lasterr < 1000)
                {
                    p = 0.01;
                }
                if (lasterr < 100)
                {
                    p = 0.001;
                    curlr = lr / 10;
                }
                double[] gradienterrr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                for (int k = 0; k < input.GetLength(0); k++)
                {
                    double[] gradientxr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    double[] gradientyr = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
                    double X = input[k, 0];
                    double Y = input[k, 1];
                    double Z = input[k, 2];
                    double x = output[k, 0];
                    double y = output[k, 1];
                    double xest = (r11 * X + r12 * Y + r13 * Z + r14) / (r31 * X + r32 * Y + r33 * Z + 1);
                    double yest = (r21 * X + r22 * Y + r23 * Z + r24) / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientxr[0] = X / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientxr[1] = Y / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientxr[2] = Z / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientxr[3] = 1 / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientxr[4] = 0;
                    gradientxr[5] = 0;
                    gradientxr[6] = 0;
                    gradientxr[7] = 0;
                    gradientxr[8] = ((-1) * (r11 * X + r12 * Y + r13 * Z + r14) * X) / Math.Pow((r31 * X + r32 * Y + r33 * Z + 1), 2);
                    gradientxr[9] = ((-1) * (r11 * X + r12 * Y + r13 * Z + r14) * Y) / Math.Pow((r31 * X + r32 * Y + r33 * Z + 1), 2);
                    gradientxr[10] = ((-1) * (r11 * X + r12 * Y + r13 * Z + r14) * Z) / Math.Pow((r31 * X + r32 * Y + r33 * Z + 1), 2);
                    gradientyr[0] = 0;
                    gradientyr[1] = 0;
                    gradientyr[2] = 0;
                    gradientyr[3] = 0;
                    gradientyr[4] = X / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientyr[5] = Y / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientyr[6] = Z / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientyr[7] = 1 / (r31 * X + r32 * Y + r33 * Z + 1);
                    gradientyr[8] = ((-1) * (r21 * X + r22 * Y + r23 * Z + r24) * X) / Math.Pow((r31 * X + r32 * Y + r33 * Z + 1), 2);
                    gradientyr[9] = ((-1) * (r21 * X + r22 * Y + r23 * Z + r24) * Y) / Math.Pow((r31 * X + r32 * Y + r33 * Z + 1), 2);
                    gradientyr[10] = ((-1) * (r21 * X + r22 * Y + r23 * Z + r24) * Z) / Math.Pow((r31 * X + r32 * Y + r33 * Z + 1), 2);
                    for (int l = 0; l < gradienterrr.Length; l++)
                    {
                        gradienterrr[l] += (xest - x) * gradientxr[l] + (yest - y) * gradientyr[l];
                    }
                }
                
                bool flag1 = Math.Abs(curlr * gradienterrr[0] / input.GetLength(0)) >= Math.Abs(r11 * p);
                bool flag2 = Math.Abs(curlr * gradienterrr[1] / input.GetLength(0)) >= Math.Abs(r12 * p);
                bool flag3 = Math.Abs(curlr * gradienterrr[2] / input.GetLength(0)) >= Math.Abs(r13 * p);
                bool flag4 = Math.Abs(curlr * gradienterrr[3] / input.GetLength(0)) >= Math.Abs(r14 * p);
                bool flag5 = Math.Abs(curlr * gradienterrr[4] / input.GetLength(0)) >= Math.Abs(r21 * p);
                bool flag6 = Math.Abs(curlr * gradienterrr[5] / input.GetLength(0)) >= Math.Abs(r22 * p);
                bool flag7 = Math.Abs(curlr * gradienterrr[6] / input.GetLength(0)) >= Math.Abs(r23 * p);
                bool flag8 = Math.Abs(curlr * gradienterrr[7] / input.GetLength(0)) >= Math.Abs(r24 * p);
                bool flag9 = Math.Abs(curlr * gradienterrr[8] / input.GetLength(0)) >= Math.Abs(r31 * p);
                bool flag10 = Math.Abs(curlr * gradienterrr[9] / input.GetLength(0)) >= Math.Abs(r32 * p);
                bool flag11 = Math.Abs(curlr * gradienterrr[10] / input.GetLength(0)) >= Math.Abs(r33 * p);
                bool _flag1 = Math.Abs(curlr * gradienterrr[0] / input.GetLength(0)) <= Math.Abs(r11 * p / 10);
                bool _flag2 = Math.Abs(curlr * gradienterrr[1] / input.GetLength(0)) <= Math.Abs(r12 * p / 10);
                bool _flag3 = Math.Abs(curlr * gradienterrr[2] / input.GetLength(0)) <= Math.Abs(r13 * p / 10);
                bool _flag4 = Math.Abs(curlr * gradienterrr[3] / input.GetLength(0)) <= Math.Abs(r14 * p / 10);
                bool _flag5 = Math.Abs(curlr * gradienterrr[4] / input.GetLength(0)) <= Math.Abs(r21 * p / 10);
                bool _flag6 = Math.Abs(curlr * gradienterrr[5] / input.GetLength(0)) <= Math.Abs(r22 * p / 10);
                bool _flag7 = Math.Abs(curlr * gradienterrr[6] / input.GetLength(0)) <= Math.Abs(r23 * p / 10);
                bool _flag8 = Math.Abs(curlr * gradienterrr[7] / input.GetLength(0)) <= Math.Abs(r24 * p / 10);
                bool _flag9 = Math.Abs(curlr * gradienterrr[8] / input.GetLength(0)) <= Math.Abs(r31 * p / 10);
                bool _flag10 = Math.Abs(curlr * gradienterrr[9] / input.GetLength(0)) <= Math.Abs(r32 * p / 10);
                bool _flag11 = Math.Abs(curlr * gradienterrr[10] / input.GetLength(0)) <= Math.Abs(r33 * p / 10);
                while (_flag1 && _flag2 && _flag3 && _flag4 && _flag5 && _flag6 && _flag7 && _flag8 && _flag9 && _flag10 && _flag11)
                {
                    curlr = curlr * 10;
                    _flag1 = Math.Abs(curlr * gradienterrr[0] / input.GetLength(0)) <= Math.Abs(r11 * p / 10);
                    _flag2 = Math.Abs(curlr * gradienterrr[1] / input.GetLength(0)) <= Math.Abs(r12 * p / 10);
                    _flag3 = Math.Abs(curlr * gradienterrr[2] / input.GetLength(0)) <= Math.Abs(r13 * p / 10);
                    _flag4 = Math.Abs(curlr * gradienterrr[3] / input.GetLength(0)) <= Math.Abs(r14 * p / 10);
                    _flag5 = Math.Abs(curlr * gradienterrr[4] / input.GetLength(0)) <= Math.Abs(r21 * p / 10);
                    _flag6 = Math.Abs(curlr * gradienterrr[5] / input.GetLength(0)) <= Math.Abs(r22 * p / 10);
                    _flag7 = Math.Abs(curlr * gradienterrr[6] / input.GetLength(0)) <= Math.Abs(r23 * p / 10);
                    _flag8 = Math.Abs(curlr * gradienterrr[7] / input.GetLength(0)) <= Math.Abs(r24 * p / 10);
                    _flag9 = Math.Abs(curlr * gradienterrr[8] / input.GetLength(0)) <= Math.Abs(r31 * p / 10);
                    _flag10 = Math.Abs(curlr * gradienterrr[9] / input.GetLength(0)) <= Math.Abs(r32 * p / 10);
                    _flag11 = Math.Abs(curlr * gradienterrr[10] / input.GetLength(0)) <= Math.Abs(r33 * p / 10);
                }
               
                r11 -= curlr * gradienterrr[0] / input.GetLength(0);
                r12 -= curlr * gradienterrr[1] / input.GetLength(0);
                r13 -= curlr * gradienterrr[2] / input.GetLength(0);
                r14 -= curlr * gradienterrr[3] / input.GetLength(0);
                r21 -= curlr * gradienterrr[4] / input.GetLength(0);
                r22 -= curlr * gradienterrr[5] / input.GetLength(0);
                r23 -= curlr * gradienterrr[6] / input.GetLength(0);
                r24 -= curlr * gradienterrr[7] / input.GetLength(0);
                r31 -= curlr * gradienterrr[8] / input.GetLength(0);
                r32 -= curlr * gradienterrr[9] / input.GetLength(0);
                r33 -= curlr * gradienterrr[10] / input.GetLength(0);

                xerr = 0;
                yerr = 0;
                for (int m = 0; m < input.GetLength(0); m++)
                {
                    double X = input[m, 0];
                    double Y = input[m, 1];
                    double Z = input[m, 2];
                    double x = output[m, 0];
                    double y = output[m, 1];
                    double xest = (r11 * X + r12 * Y + r13 * Z + r14) / (r31 * X + r32 * Y + r33 * Z + 1);
                    double yest = (r21 * X + r22 * Y + r23 * Z + r24) / (r31 * X + r32 * Y + r33 * Z + 1);
                    xerr += 0.5 * Math.Pow(x - xest, 2);
                    yerr += 0.5 * Math.Pow(y - yest, 2);
                }
                lasterr = (xerr + yerr) / input.GetLength(0);
                if ((j + 1) % loginterval == 0)
                {
                    if (InvokeRequired)
                    {
                        if (!logBox.IsDisposed)
                        {
                            string txt = "";
                            txt += ("epoch " + (j + 1) + " err: " + (xerr + yerr) / input.GetLength(0)) + "\r\n";
                            txt += ("epoch " + (j + 1) + " xerr: " + xerr / input.GetLength(0)) + "\r\n";
                            txt += ("epoch " + (j + 1) + " yerr: " + yerr / input.GetLength(0)) + "\r\n";
                            Invoke(new AddLogTextDelegate(AddLogText), txt);
                        }
                    }
                    /*
                    logBox.Text += ("epoch " + (j + 1) + " err: " + (xerr + yerr) / input.GetLength(0)) + "\r\n";
                    logBox.Text += ("epoch " + (j + 1) + " xerr: " + xerr / input.GetLength(0)) + "\r\n";
                    logBox.Text += ("epoch " + (j + 1) + " yerr: " + yerr / input.GetLength(0)) + "\r\n";
                    */
                }
            }
            finalarg[0] = r11;
            finalarg[1] = r12;
            finalarg[2] = r13;
            finalarg[3] = r14;
            finalarg[4] = r21;
            finalarg[5] = r22;
            finalarg[6] = r23;
            finalarg[7] = r24;
            finalarg[8] = r31;
            finalarg[9] = r32;
            finalarg[10] = r33;
            return finalarg;
        }
        private void CalculateLMS()
        {
            bool inputOK = false;
            double[] Err = { 100000,100000 };
            double X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0, X5 = 0, Y5 = 0, Z5 = 0, X6 = 0, Y6 = 0, Z6 = 0;
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0, x5 = 0, y5 = 0, x6 = 0, y6 = 0;
            double X7 = 0, Y7 = 0, Z7 = 0, X8 = 0, Y8 = 0, Z8 = 0, X9 = 0, Y9 = 0, Z9 = 0, X10 = 0, Y10 = 0, Z10 = 0, X11 = 0, Y11 = 0, Z11 = 0, X12 = 0, Y12 = 0, Z12 = 0;
            double x7 = 0, y7 = 0, x8 = 0, y8 = 0, x9 = 0, y9 = 0, x10 = 0, y10 = 0, x11 = 0, y11 = 0, x12 = 0, y12 = 0;
            try
            {
                try
                {
                    X1 = Convert.ToInt32(X1Txt.Text);
                    X2 = Convert.ToInt32(X2Txt.Text);
                    X3 = Convert.ToInt32(X3Txt.Text);
                    X4 = Convert.ToInt32(X4Txt.Text);
                    X5 = Convert.ToInt32(X5Txt.Text);
                    X6 = Convert.ToInt32(X6Txt.Text);
                    X7 = Convert.ToInt32(X7Txt.Text);
                    X8 = Convert.ToInt32(X8Txt.Text);
                    Y1 = Convert.ToInt32(Y1Txt.Text);
                    Y2 = Convert.ToInt32(Y2Txt.Text);
                    Y3 = Convert.ToInt32(Y3Txt.Text);
                    Y4 = Convert.ToInt32(Y4Txt.Text);
                    Y5 = Convert.ToInt32(Y5Txt.Text);
                    Y6 = Convert.ToInt32(Y6Txt.Text);
                    Y7 = Convert.ToInt32(Y7Txt.Text);
                    Y8 = Convert.ToInt32(Y8Txt.Text);
                    Z1 = Convert.ToInt32(Z1Txt.Text);
                    Z2 = Convert.ToInt32(Z2Txt.Text);
                    Z3 = Convert.ToInt32(Z3Txt.Text);
                    Z4 = Convert.ToInt32(Z4Txt.Text);
                    Z5 = Convert.ToInt32(Z5Txt.Text);
                    Z6 = Convert.ToInt32(Z6Txt.Text);
                    Z7 = Convert.ToInt32(Z7Txt.Text);
                    Z8 = Convert.ToInt32(Z8Txt.Text);
                    x1 = Convert.ToInt32(u1Txt.Text);
                    x2 = Convert.ToInt32(u2Txt.Text);
                    x3 = Convert.ToInt32(u3Txt.Text);
                    x4 = Convert.ToInt32(u4Txt.Text);
                    x5 = Convert.ToInt32(u5Txt.Text);
                    x6 = Convert.ToInt32(u6Txt.Text);
                    x7 = Convert.ToInt32(u7Txt.Text);
                    x8 = Convert.ToInt32(u8Txt.Text);
                    y1 = Convert.ToInt32(v1Txt.Text);
                    y2 = Convert.ToInt32(v2Txt.Text);
                    y3 = Convert.ToInt32(v3Txt.Text);
                    y4 = Convert.ToInt32(v4Txt.Text);
                    y5 = Convert.ToInt32(v5Txt.Text);
                    y6 = Convert.ToInt32(v6Txt.Text);
                    y7 = Convert.ToInt32(v7Txt.Text);
                    y8 = Convert.ToInt32(v8Txt.Text);
                    if (AdditionalDataCheck.Checked)
                    {
                        X9 = Convert.ToInt32(X9Txt.Text);
                        X10 = Convert.ToInt32(X10Txt.Text);
                        X11 = Convert.ToInt32(X11Txt.Text);
                        X12 = Convert.ToInt32(X12Txt.Text);
                        Y9 = Convert.ToInt32(Y9Txt.Text);
                        Y10 = Convert.ToInt32(Y10Txt.Text);
                        Y11 = Convert.ToInt32(Y11Txt.Text);
                        Y12 = Convert.ToInt32(Y12Txt.Text);
                        Z9 = Convert.ToInt32(Z9Txt.Text);
                        Z10 = Convert.ToInt32(Z10Txt.Text);
                        Z11 = Convert.ToInt32(Z11Txt.Text);
                        Z12 = Convert.ToInt32(Z12Txt.Text);
                        x9 = Convert.ToInt32(u9Txt.Text);
                        x10 = Convert.ToInt32(u10Txt.Text);
                        x11 = Convert.ToInt32(u11Txt.Text);
                        x12 = Convert.ToInt32(u12Txt.Text);
                        y9 = Convert.ToInt32(v9Txt.Text);
                        y10 = Convert.ToInt32(v10Txt.Text);
                        y11 = Convert.ToInt32(v11Txt.Text);
                        y12 = Convert.ToInt32(v12Txt.Text);
                    }
                    inputOK = true;
                }
                catch (Exception ex)
                {
                    inputOK = false;
                    MessageBox.Show("请正确填写标定参数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (inputOK)
                {
                    double[] X = { X1, X2, X3, X4, X5, X6, X7, X8, X9, X10, X11, X12 };
                    double[] Y = { Y1, Y2, Y3, Y4, Y5, Y6, Y7, Y8, Y9, Y10, Y11, Y12 };
                    double[] Z = { Z1, Z2, Z3, Z4, Z5, Z6, Z7, Z8, Z9, Z10, Z11, Z12 };
                    double[] x = { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12 };
                    double[] y = { y1, y2, y3, y4, y5, y6, y7, y8, y9, y10, y11, y12 };
                    double[] Xraw = { X1, X2, X3, X4, X5, X6, X7, X8 };
                    double[] Yraw = { Y1, Y2, Y3, Y4, Y5, Y6, Y7, Y8 };
                    double[] Zraw = { Z1, Z2, Z3, Z4, Z5, Z6, Z7, Z8 };
                    double[] xraw = { x1, x2, x3, x4, x5, x6, x7, x8 };
                    double[] yraw = { y1, y2, y3, y4, y5, y6, y7, y8 };
                    if (AdditionalDataCheck.Checked)
                    {
                        Err = LMS(X, Y, Z, x, y, 12);
                        
                    }
                    else
                    {
                        Err = LMS(Xraw, Yraw, Zraw, xraw, yraw, 8);
                        
                    }
                    int x1cal = (int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1));
                    int errx1 = (int)(x1cal - x1);
                    int x2cal = (int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1));
                    int errx2 = (int)(x2cal - x2);
                    int x3cal = (int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1));
                    int errx3 = (int)(x3cal - x3);
                    int x4cal = (int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1));
                    int errx4 = (int)(x4cal - x4);
                    int x5cal = (int)((X5 * r11 + Y5 * r12 + Z5 * r13 + r14) / (X5 * r31 + Y5 * r32 + Z5 * r33 + 1));
                    int errx5 = (int)(x5cal - x5);
                    int x6cal = (int)((X6 * r11 + Y6 * r12 + Z6 * r13 + r14) / (X6 * r31 + Y6 * r32 + Z6 * r33 + 1));
                    int errx6 = (int)(x6cal - x6);
                    int x7cal = (int)((X7 * r11 + Y7 * r12 + Z7 * r13 + r14) / (X7 * r31 + Y7 * r32 + Z7 * r33 + 1));
                    int errx7 = (int)(x7cal - x7);
                    int x8cal = (int)((X8 * r11 + Y8 * r12 + Z8 * r13 + r14) / (X8 * r31 + Y8 * r32 + Z8 * r33 + 1));
                    int errx8 = (int)(x8cal - x8);
                    int x9cal = (int)((X9 * r11 + Y9 * r12 + Z9 * r13 + r14) / (X9 * r31 + Y9 * r32 + Z9 * r33 + 1));
                    int errx9 = (int)(x9cal - x9);
                    int x10cal = (int)((X10 * r11 + Y10 * r12 + Z10 * r13 + r14) / (X10 * r31 + Y10 * r32 + Z10 * r33 + 1));
                    int errx10 = (int)(x10cal - x10);
                    int x11cal = (int)((X11 * r11 + Y11 * r12 + Z11 * r13 + r14) / (X11 * r31 + Y11 * r32 + Z11 * r33 + 1));
                    int errx11 = (int)(x11cal - x11);
                    int x12cal = (int)((X12 * r11 + Y12 * r12 + Z12 * r13 + r14) / (X12 * r31 + Y12 * r32 + Z12 * r33 + 1));
                    int errx12 = (int)(x12cal - x12);
                    int y1cal = (int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1));
                    int erry1 = (int)(y1cal - y1);
                    int y2cal = (int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1));
                    int erry2 = (int)(y2cal - y2);
                    int y3cal = (int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1));
                    int erry3 = (int)(y3cal - y3);
                    int y4cal = (int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1));
                    int erry4 = (int)(y4cal - y4);
                    int y5cal = (int)((X5 * r21 + Y5 * r22 + Z5 * r23 + r24) / (X5 * r31 + Y5 * r32 + Z5 * r33 + 1));
                    int erry5 = (int)(y5cal - y5);
                    int y6cal = (int)((X6 * r21 + Y6 * r22 + Z6 * r23 + r24) / (X6 * r31 + Y6 * r32 + Z6 * r33 + 1));
                    int erry6 = (int)(y6cal - y6);
                    int y7cal = (int)((X7 * r21 + Y7 * r22 + Z7 * r23 + r24) / (X7 * r31 + Y7 * r32 + Z7 * r33 + 1));
                    int erry7 = (int)(y7cal - y7);
                    int y8cal = (int)((X8 * r21 + Y8 * r22 + Z8 * r23 + r24) / (X8 * r31 + Y8 * r32 + Z8 * r33 + 1));
                    int erry8 = (int)(y8cal - y8);
                    int y9cal = (int)((X9 * r21 + Y9 * r22 + Z9 * r23 + r24) / (X9 * r31 + Y9 * r32 + Z9 * r33 + 1));
                    int erry9 = (int)(y9cal - y9);
                    int y10cal = (int)((X10 * r21 + Y10 * r22 + Z10 * r23 + r24) / (X10 * r31 + Y10 * r32 + Z10 * r33 + 1));
                    int erry10 = (int)(y10cal - y10);
                    int y11cal = (int)((X11 * r21 + Y11 * r22 + Z11 * r23 + r24) / (X11 * r31 + Y11 * r32 + Z11 * r33 + 1));
                    int erry11 = (int)(y11cal - y11);
                    int y12cal = (int)((X12 * r21 + Y12 * r22 + Z12 * r23 + r24) / (X12 * r31 + Y12 * r32 + Z12 * r33 + 1));
                    int erry12 = (int)(y12cal - y12);

                    if (!logBox.IsDisposed)
                    {
                        string txt = "";
                        txt += ("x1= " + x1 + "; x1cal= " + x1cal + "; err= " + errx1) + "\r\n";
                        txt += ("x2= " + x2 + "; x2cal= " + x2cal + "; err= " + errx2) + "\r\n";
                        txt += ("x3= " + x3 + "; x3cal= " + x3cal + "; err= " + errx3) + "\r\n";
                        txt += ("x4= " + x4 + "; x4cal= " + x4cal + "; err= " + errx4) + "\r\n";
                        txt += ("x5= " + x5 + "; x5cal= " + x5cal + "; err= " + errx5) + "\r\n";
                        txt += ("x6= " + x6 + "; x6cal= " + x6cal + "; err= " + errx6) + "\r\n";
                        txt += ("x7= " + x7 + "; x7cal= " + x7cal + "; err= " + errx7) + "\r\n";
                        txt += ("x8= " + x8 + "; x8cal= " + x8cal + "; err= " + errx8) + "\r\n";
                        if (AdditionalDataCheck.Checked)
                        {
                            txt += ("x9= " + x9 + "; x9cal= " + x9cal + "; err= " + errx9) + "\r\n";
                            txt += ("x10= " + x10 + "; x10cal= " + x10cal + "; err= " + errx10) + "\r\n";
                            txt += ("x11= " + x11 + "; x11cal= " + x11cal + "; err= " + errx11) + "\r\n";
                            txt += ("x12= " + x12 + "; x12cal= " + x12cal + "; err= " + errx12) + "\r\n";
                        }
                        txt += ("y1= " + y1 + "; y1cal= " + y1cal + "; err= " + erry1) + "\r\n";
                        txt += ("y2= " + y2 + "; y2cal= " + y2cal + "; err= " + erry2) + "\r\n";
                        txt += ("y3= " + y3 + "; y3cal= " + y3cal + "; err= " + erry3) + "\r\n";
                        txt += ("y4= " + y4 + "; y4cal= " + y4cal + "; err= " + erry4) + "\r\n";
                        txt += ("y5= " + y5 + "; y5cal= " + y5cal + "; err= " + erry5) + "\r\n";
                        txt += ("y6= " + y6 + "; y6cal= " + y6cal + "; err= " + erry6) + "\r\n";
                        txt += ("y7= " + y7 + "; y7cal= " + y7cal + "; err= " + erry7) + "\r\n";
                        txt += ("y8= " + y8 + "; y8cal= " + y8cal + "; err= " + erry8) + "\r\n";
                        if (AdditionalDataCheck.Checked)
                        {
                            txt += ("y9= " + y9 + "; y9cal= " + y9cal + "; err= " + erry9) + "\r\n";
                            txt += ("y10= " + y10 + "; y10cal= " + y10cal + "; err= " + erry10) + "\r\n";
                            txt += ("y11= " + y11 + "; y11cal= " + y11cal + "; err= " + erry11) + "\r\n";
                            txt += ("y12= " + y12 + "; y12cal= " + y12cal + "; err= " + erry12) + "\r\n";
                        }
                        txt += "Mean x Err= " + Err[0] + "; Mean y Err= " + Err[1] + "\r\n";
                        if (InvokeRequired)
                        {
                            Invoke(new AddLogTextDelegate(AddLogText), txt);
                        }
                        else
                        {
                            AddLogText(txt);
                        }
                    }
                    
                }
            }
            catch (Exception ex)
            {
                /*
                logBox.Text += StaCombo.Text + " error in calibration: " + ex.StackTrace + "\r\n";
                */
                
                    if (!logBox.IsDisposed)
                    {
                        string txt = "";
                        txt += StaCombo.Text + dimCombo.Text + " error in calibration: " + ex.StackTrace + "\r\n";
                    if (InvokeRequired)
                    {
                        Invoke(new AddLogTextDelegate(AddLogText), txt);
                    }
                    else
                    {
                        AddLogText(txt);
                    }
                }
                
            }
            if (InvokeRequired)
            {
                Invoke(new EnableAndDisableButtonDelegate(EnableAndDisableButton));
            }
            else
            {
                EnableAndDisableButton();
            }
            _cancel = false;
        }
        private void CalculateOptimize()
        {
            bool inputOK = false;
            Errtemp = 100000;
            Int64 epoch = 0;
            Int64 logInterval = 100000;
            double lr = 1E-12;
            double X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0, X5 = 0, Y5 = 0, Z5 = 0, X6 = 0, Y6 = 0, Z6 = 0;
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0, x5 = 0, y5 = 0, x6 = 0, y6 = 0;
            double X7 = 0, Y7 = 0, Z7 = 0, X8 = 0, Y8 = 0, Z8 = 0, X9 = 0, Y9 = 0, Z9 = 0, X10 = 0, Y10 = 0, Z10 = 0, X11 = 0, Y11 = 0, Z11 = 0, X12 = 0, Y12 = 0, Z12 = 0;
            double x7 = 0, y7 = 0, x8 = 0, y8 = 0, x9 = 0, y9 = 0, x10 = 0, y10 = 0, x11 = 0, y11 = 0, x12 = 0, y12 = 0;
            try
            {
                try
                {
                    epoch = Convert.ToInt64(EpochTxt.Text);
                    lr = Convert.ToDouble(LrTxt.Text);
                    logInterval = Convert.ToInt64(logIntervalTxt.Text);
                    X1 = Convert.ToInt32(X1Txt.Text);
                    X2 = Convert.ToInt32(X2Txt.Text);
                    X3 = Convert.ToInt32(X3Txt.Text);
                    X4 = Convert.ToInt32(X4Txt.Text);
                    X5 = Convert.ToInt32(X5Txt.Text);
                    X6 = Convert.ToInt32(X6Txt.Text);
                    X7 = Convert.ToInt32(X7Txt.Text);
                    X8 = Convert.ToInt32(X8Txt.Text);
                    Y1 = Convert.ToInt32(Y1Txt.Text);
                    Y2 = Convert.ToInt32(Y2Txt.Text);
                    Y3 = Convert.ToInt32(Y3Txt.Text);
                    Y4 = Convert.ToInt32(Y4Txt.Text);
                    Y5 = Convert.ToInt32(Y5Txt.Text);
                    Y6 = Convert.ToInt32(Y6Txt.Text);
                    Y7 = Convert.ToInt32(Y7Txt.Text);
                    Y8 = Convert.ToInt32(Y8Txt.Text);
                    Z1 = Convert.ToInt32(Z1Txt.Text);
                    Z2 = Convert.ToInt32(Z2Txt.Text);
                    Z3 = Convert.ToInt32(Z3Txt.Text);
                    Z4 = Convert.ToInt32(Z4Txt.Text);
                    Z5 = Convert.ToInt32(Z5Txt.Text);
                    Z6 = Convert.ToInt32(Z6Txt.Text);
                    Z7 = Convert.ToInt32(Z7Txt.Text);
                    Z8 = Convert.ToInt32(Z8Txt.Text);
                    x1 = Convert.ToInt32(u1Txt.Text);
                    x2 = Convert.ToInt32(u2Txt.Text);
                    x3 = Convert.ToInt32(u3Txt.Text);
                    x4 = Convert.ToInt32(u4Txt.Text);
                    x5 = Convert.ToInt32(u5Txt.Text);
                    x6 = Convert.ToInt32(u6Txt.Text);
                    x7 = Convert.ToInt32(u7Txt.Text);
                    x8 = Convert.ToInt32(u8Txt.Text);
                    y1 = Convert.ToInt32(v1Txt.Text);
                    y2 = Convert.ToInt32(v2Txt.Text);
                    y3 = Convert.ToInt32(v3Txt.Text);
                    y4 = Convert.ToInt32(v4Txt.Text);
                    y5 = Convert.ToInt32(v5Txt.Text);
                    y6 = Convert.ToInt32(v6Txt.Text);
                    y7 = Convert.ToInt32(v7Txt.Text);
                    y8 = Convert.ToInt32(v8Txt.Text);
                    if (AdditionalDataCheck.Checked)
                    {
                        X9 = Convert.ToInt32(X9Txt.Text);
                        X10 = Convert.ToInt32(X10Txt.Text);
                        X11 = Convert.ToInt32(X11Txt.Text);
                        X12 = Convert.ToInt32(X12Txt.Text);
                        Y9 = Convert.ToInt32(Y9Txt.Text);
                        Y10 = Convert.ToInt32(Y10Txt.Text);
                        Y11 = Convert.ToInt32(Y11Txt.Text);
                        Y12 = Convert.ToInt32(Y12Txt.Text);
                        Z9 = Convert.ToInt32(Z9Txt.Text);
                        Z10 = Convert.ToInt32(Z10Txt.Text);
                        Z11 = Convert.ToInt32(Z11Txt.Text);
                        Z12 = Convert.ToInt32(Z12Txt.Text);
                        x9 = Convert.ToInt32(u9Txt.Text);
                        x10 = Convert.ToInt32(u10Txt.Text);
                        x11 = Convert.ToInt32(u11Txt.Text);
                        x12 = Convert.ToInt32(u12Txt.Text);
                        y9 = Convert.ToInt32(v9Txt.Text);
                        y10 = Convert.ToInt32(v10Txt.Text);
                        y11 = Convert.ToInt32(v11Txt.Text);
                        y12 = Convert.ToInt32(v12Txt.Text);
                    }
                    inputOK = true;
                }
                catch (Exception ex)
                {
                    inputOK = false;
                    MessageBox.Show("请正确填写标定参数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (inputOK)
                {
                    double[] X = { X1, X2, X3, X4, X5, X6, X7, X8, X9, X10, X11, X12 };
                    double[] Y = { Y1, Y2, Y3, Y4, Y5, Y6, Y7, Y8, Y9, Y10, Y11, Y12 };
                    double[] Z = { Z1, Z2, Z3, Z4, Z5, Z6, Z7, Z8, Z9, Z10, Z11, Z12 };
                    double[] x = { x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11, x12 };
                    double[] y = { y1, y2, y3, y4, y5, y6, y7, y8, y9, y10, y11, y12 };
                    double[] Xraw = { X1, X2, X3, X4, X5, X6, X7, X8 };
                    double[] Yraw = { Y1, Y2, Y3, Y4, Y5, Y6, Y7, Y8 };
                    double[] Zraw = { Z1, Z2, Z3, Z4, Z5, Z6, Z7, Z8 };
                    double[] xraw = { x1, x2, x3, x4, x5, x6, x7, x8 };
                    double[] yraw = { y1, y2, y3, y4, y5, y6, y7, y8 };
                    /*
                    double[,] XYZ = { { X1, Y1, Z1, 1, 0, 0, 0, 0, -X1*x1, -Y1*x1, -Z1*x1 },
                                        { 0, 0, 0, 0, X1, Y1, Z1, 1, -X1*y1, -Y1*y1, -Z1*y1 },
                                        { X2, Y2, Z2, 1, 0, 0, 0, 0, -X2*x2, -Y2*x2, -Z2*x2 },
                                        { 0, 0, 0, 0, X2, Y2, Z2, 1, -X2*y2, -Y2*y2, -Z2*y2 },
                                        { X4, Y4, Z4, 1, 0, 0, 0, 0, -X4*x4, -Y4*x4, -Z4*x4 },
                                        { 0, 0, 0, 0, X4, Y4, Z4, 1, -X4*y4, -Y4*y4, -Z4*y4 },
                                        { X7, Y7, Z7, 1, 0, 0, 0, 0, -X7*x7, -Y7*x7, -Z7*x7 },
                                        { 0, 0, 0, 0, X7, Y7, Z7, 1, -X7*y7, -Y7*y7, -Z7*y7 },
                                        { X6, Y6, Z6, 1, 0, 0, 0, 0, -X6*x6, -Y6*x6, -Z6*x6 },
                                        { 0, 0, 0, 0, X6, Y6, Z6, 1, -X6*y6, -Y6*y6, -Z6*y6 },
                                        { X8, Y8, Z8, 1, 0, 0, 0, 0, -X8*x8, -Y8*x8, -Z8*x8 }};
                    Matrix3 XYZMatrix = new Matrix3(XYZ);
                    double[,] xy = new double[11, 1];
                    xy[0, 0] = x1;
                    xy[1, 0] = y1;
                    xy[2, 0] = x2;
                    xy[3, 0] = y2;
                    xy[4, 0] = x4;
                    xy[5, 0] = y4;
                    xy[6, 0] = x7;
                    xy[7, 0] = y7;
                    xy[8, 0] = x6;
                    xy[9, 0] = y6;
                    xy[10, 0] = x8;
                    Matrix3 xyMatrix = new Matrix3(xy);
                    Matrix3 R1 = XYZMatrix.Inverse() * xyMatrix;
                    r11 = R1[0, 0];
                    r12 = R1[1, 0];
                    r13 = R1[2, 0];
                    r14 = R1[3, 0];
                    r21 = R1[4, 0];
                    r22 = R1[5, 0];
                    r23 = R1[6, 0];
                    r24 = R1[7, 0];
                    r31 = R1[8, 0];
                    r32 = R1[9, 0];
                    r33 = R1[10, 0];
                    */
                    
                    double[,] input = { { X1, Y1, Z1 }, { X2, Y2, Z2 }, { X3, Y3, Z3 }, { X4, Y4, Z4 }, { X5, Y5, Z5 }, { X6, Y6, Z6 }, { X7, Y7, Z7 }, { X8, Y8, Z8 }, { X9, Y9, Z9 }, { X10, Y10, Z10 }, { X11, Y11, Z11 }, { X12, Y12, Z12 } };
                    double[,] output = { { x1, y1 }, { x2, y2 }, { x3, y3 }, { x4, y4 }, { x5, y5 }, { x6, y6 }, { x7, y7 }, { x8, y8 }, { x9, y9 }, { x10, y10 }, { x11, y11 }, { x12, y12 } };
                    double[,] inputraw = { { X1, Y1, Z1 }, { X2, Y2, Z2 }, { X3, Y3, Z3 }, { X4, Y4, Z4 }, { X5, Y5, Z5 }, { X6, Y6, Z6 }, { X7, Y7, Z7 }, { X8, Y8, Z8 } };
                    double[,] outputraw = { { x1, y1 }, { x2, y2 }, { x3, y3 }, { x4, y4 }, { x5, y5 }, { x6, y6 }, { x7, y7 }, { x8, y8 } };
                    if (AdditionalDataCheck.Checked)
                    {
                        Calculate(X, Y, Z, x, y, 1, 2, 3, 5, 8, 7);
                        Calculate(X, Y, Z, x, y, 1, 2, 3, 7, 8, 5);
                        Calculate(X, Y, Z, x, y, 1, 2, 4, 6, 7, 8);
                        Calculate(X, Y, Z, x, y, 1, 2, 4, 8, 7, 6);
                        Calculate(X, Y, Z, x, y, 1, 3, 4, 5, 6, 7);
                        Calculate(X, Y, Z, x, y, 1, 3, 4, 7, 6, 5);
                        Calculate(X, Y, Z, x, y, 2, 3, 4, 8, 5, 6);
                        Calculate(X, Y, Z, x, y, 2, 3, 4, 6, 5, 8);
                        Calculate(X, Y, Z, x, y, 5, 6, 7, 1, 4, 3);
                        Calculate(X, Y, Z, x, y, 5, 6, 7, 3, 4, 1);
                        Calculate(X, Y, Z, x, y, 5, 6, 8, 2, 3, 4);
                        Calculate(X, Y, Z, x, y, 5, 6, 8, 4, 3, 2);
                        Calculate(X, Y, Z, x, y, 5, 7, 8, 1, 2, 3);
                        Calculate(X, Y, Z, x, y, 5, 7, 8, 3, 2, 1);
                        Calculate(X, Y, Z, x, y, 6, 7, 8, 2, 1, 4);
                        Calculate(X, Y, Z, x, y, 6, 7, 8, 4, 1, 2);
                        double[] initial = { r11, r12, r13, r14, r21, r22, r23, r24, r31, r32, r33 };
                        double[] refined = gradientdescent(input, output, initial, epoch, lr, logInterval);
                        r11 = refined[0];
                        r12 = refined[1];
                        r13 = refined[2];
                        r14 = refined[3];
                        r21 = refined[4];
                        r22 = refined[5];
                        r23 = refined[6];
                        r24 = refined[7];
                        r31 = refined[8];
                        r32 = refined[9];
                        r33 = refined[10];
                    }
                    else
                    {
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 1, 2, 3, 5, 8, 7);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 1, 2, 3, 7, 8, 5);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 1, 2, 4, 6, 7, 8);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 1, 2, 4, 8, 7, 6);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 1, 3, 4, 5, 6, 7);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 1, 3, 4, 7, 6, 5);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 2, 3, 4, 8, 5, 6);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 2, 3, 4, 6, 5, 8);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 5, 6, 7, 1, 4, 3);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 5, 6, 7, 3, 4, 1);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 5, 6, 8, 2, 3, 4);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 5, 6, 8, 4, 3, 2);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 5, 7, 8, 1, 2, 3);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 5, 7, 8, 3, 2, 1);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 6, 7, 8, 2, 1, 4);
                        Calculate(Xraw, Yraw, Zraw, xraw, yraw, 6, 7, 8, 4, 1, 2);
                        double[] initial = { r11, r12, r13, r14, r21, r22, r23, r24, r31, r32, r33 };
                        double[] refinedraw = gradientdescent(inputraw, outputraw, initial, epoch, lr, logInterval);
                        r11 = refinedraw[0];
                        r12 = refinedraw[1];
                        r13 = refinedraw[2];
                        r14 = refinedraw[3];
                        r21 = refinedraw[4];
                        r22 = refinedraw[5];
                        r23 = refinedraw[6];
                        r24 = refinedraw[7];
                        r31 = refinedraw[8];
                        r32 = refinedraw[9];
                        r33 = refinedraw[10];
                    }
                    int x1cal = (int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1));
                    int errx1 = (int)(x1cal - x1);
                    int x2cal = (int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1));
                    int errx2 = (int)(x2cal - x2);
                    int x3cal = (int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1));
                    int errx3 = (int)(x3cal - x3);
                    int x4cal = (int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1));
                    int errx4 = (int)(x4cal - x4);
                    int x5cal = (int)((X5 * r11 + Y5 * r12 + Z5 * r13 + r14) / (X5 * r31 + Y5 * r32 + Z5 * r33 + 1));
                    int errx5 = (int)(x5cal - x5);
                    int x6cal = (int)((X6 * r11 + Y6 * r12 + Z6 * r13 + r14) / (X6 * r31 + Y6 * r32 + Z6 * r33 + 1));
                    int errx6 = (int)(x6cal - x6);
                    int x7cal = (int)((X7 * r11 + Y7 * r12 + Z7 * r13 + r14) / (X7 * r31 + Y7 * r32 + Z7 * r33 + 1));
                    int errx7 = (int)(x7cal - x7);
                    int x8cal = (int)((X8 * r11 + Y8 * r12 + Z8 * r13 + r14) / (X8 * r31 + Y8 * r32 + Z8 * r33 + 1));
                    int errx8 = (int)(x8cal - x8);
                    int x9cal = (int)((X9 * r11 + Y9 * r12 + Z9 * r13 + r14) / (X9 * r31 + Y9 * r32 + Z9 * r33 + 1));
                    int errx9 = (int)(x9cal - x9);
                    int x10cal = (int)((X10 * r11 + Y10 * r12 + Z10 * r13 + r14) / (X10 * r31 + Y10 * r32 + Z10 * r33 + 1));
                    int errx10 = (int)(x10cal - x10);
                    int x11cal = (int)((X11 * r11 + Y11 * r12 + Z11 * r13 + r14) / (X11 * r31 + Y11 * r32 + Z11 * r33 + 1));
                    int errx11 = (int)(x11cal - x11);
                    int x12cal = (int)((X12 * r11 + Y12 * r12 + Z12 * r13 + r14) / (X12 * r31 + Y12 * r32 + Z12 * r33 + 1));
                    int errx12 = (int)(x12cal - x12);
                    int y1cal = (int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1));
                    int erry1 = (int)(y1cal - y1);
                    int y2cal = (int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1));
                    int erry2 = (int)(y2cal - y2);
                    int y3cal = (int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1));
                    int erry3 = (int)(y3cal - y3);
                    int y4cal = (int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1));
                    int erry4 = (int)(y4cal - y4);
                    int y5cal = (int)((X5 * r21 + Y5 * r22 + Z5 * r23 + r24) / (X5 * r31 + Y5 * r32 + Z5 * r33 + 1));
                    int erry5 = (int)(y5cal - y5);
                    int y6cal = (int)((X6 * r21 + Y6 * r22 + Z6 * r23 + r24) / (X6 * r31 + Y6 * r32 + Z6 * r33 + 1));
                    int erry6 = (int)(y6cal - y6);
                    int y7cal = (int)((X7 * r21 + Y7 * r22 + Z7 * r23 + r24) / (X7 * r31 + Y7 * r32 + Z7 * r33 + 1));
                    int erry7 = (int)(y7cal - y7);
                    int y8cal = (int)((X8 * r21 + Y8 * r22 + Z8 * r23 + r24) / (X8 * r31 + Y8 * r32 + Z8 * r33 + 1));
                    int erry8 = (int)(y8cal - y8);
                    int y9cal = (int)((X9 * r21 + Y9 * r22 + Z9 * r23 + r24) / (X9 * r31 + Y9 * r32 + Z9 * r33 + 1));
                    int erry9 = (int)(y9cal - y9);
                    int y10cal = (int)((X10 * r21 + Y10 * r22 + Z10 * r23 + r24) / (X10 * r31 + Y10 * r32 + Z10 * r33 + 1));
                    int erry10 = (int)(y10cal - y10);
                    int y11cal = (int)((X11 * r21 + Y11 * r22 + Z11 * r23 + r24) / (X11 * r31 + Y11 * r32 + Z11 * r33 + 1));
                    int erry11 = (int)(y11cal - y11);
                    int y12cal = (int)((X12 * r21 + Y12 * r22 + Z12 * r23 + r24) / (X12 * r31 + Y12 * r32 + Z12 * r33 + 1));
                    int erry12 = (int)(y12cal - y12);
                    /*
                    logBox.Text += ("x1= " + x1 + "; x1cal= " + x1cal + "; err= " + errx1) + "\r\n";
                    logBox.Text += ("x2= " + x2 + "; x2cal= " + x2cal + "; err= " + errx2) + "\r\n";
                    logBox.Text += ("x3= " + x3 + "; x3cal= " + x3cal + "; err= " + errx3) + "\r\n";
                    logBox.Text += ("x4= " + x4 + "; x4cal= " + x4cal + "; err= " + errx4) + "\r\n";
                    logBox.Text += ("x5= " + x5 + "; x5cal= " + x5cal + "; err= " + errx5) + "\r\n";
                    logBox.Text += ("x6= " + x6 + "; x6cal= " + x6cal + "; err= " + errx6) + "\r\n";
                    logBox.Text += ("x7= " + x7 + "; x7cal= " + x7cal + "; err= " + errx7) + "\r\n";
                    logBox.Text += ("x8= " + x8 + "; x8cal= " + x8cal + "; err= " + errx8) + "\r\n";
                    logBox.Text += ("x9= " + x9 + "; x9cal= " + x9cal + "; err= " + errx9) + "\r\n";
                    logBox.Text += ("x10= " + x10 + "; x10cal= " + x10cal + "; err= " + errx10) + "\r\n";
                    logBox.Text += ("x11= " + x11 + "; x11cal= " + x11cal + "; err= " + errx11) + "\r\n";
                    logBox.Text += ("x12= " + x12 + "; x12cal= " + x12cal + "; err= " + errx12) + "\r\n";
                    logBox.Text += ("y1= " + y1 + "; y1cal= " + y1cal + "; err= " + erry1) + "\r\n";
                    logBox.Text += ("y2= " + y2 + "; y2cal= " + y2cal + "; err= " + erry2) + "\r\n";
                    logBox.Text += ("y3= " + y3 + "; y3cal= " + y3cal + "; err= " + erry3) + "\r\n";
                    logBox.Text += ("y4= " + y4 + "; y4cal= " + y4cal + "; err= " + erry4) + "\r\n";
                    logBox.Text += ("y5= " + y5 + "; y5cal= " + y5cal + "; err= " + erry5) + "\r\n";
                    logBox.Text += ("y6= " + y6 + "; y6cal= " + y6cal + "; err= " + erry6) + "\r\n";
                    logBox.Text += ("y7= " + y7 + "; y7cal= " + y7cal + "; err= " + erry7) + "\r\n";
                    logBox.Text += ("y8= " + y8 + "; y8cal= " + y8cal + "; err= " + erry8) + "\r\n";
                    logBox.Text += ("y9= " + y9 + "; y9cal= " + y9cal + "; err= " + erry9) + "\r\n";
                    logBox.Text += ("y10= " + y10 + "; y10cal= " + y10cal + "; err= " + erry10) + "\r\n";
                    logBox.Text += ("y11= " + y11 + "; y11cal= " + y11cal + "; err= " + erry11) + "\r\n";
                    logBox.Text += ("y12= " + y12 + "; y12cal= " + y12cal + "; err= " + erry12) + "\r\n";
                    */
                    if (InvokeRequired)
                    {
                        if (!logBox.IsDisposed)
                        {
                            string txt = "";
                            txt += ("x1= " + x1 + "; x1cal= " + x1cal + "; err= " + errx1) + "\r\n";
                            txt += ("x2= " + x2 + "; x2cal= " + x2cal + "; err= " + errx2) + "\r\n";
                            txt += ("x3= " + x3 + "; x3cal= " + x3cal + "; err= " + errx3) + "\r\n";
                            txt += ("x4= " + x4 + "; x4cal= " + x4cal + "; err= " + errx4) + "\r\n";
                            txt += ("x5= " + x5 + "; x5cal= " + x5cal + "; err= " + errx5) + "\r\n";
                            txt += ("x6= " + x6 + "; x6cal= " + x6cal + "; err= " + errx6) + "\r\n";
                            txt += ("x7= " + x7 + "; x7cal= " + x7cal + "; err= " + errx7) + "\r\n";
                            txt += ("x8= " + x8 + "; x8cal= " + x8cal + "; err= " + errx8) + "\r\n";
                            if (AdditionalDataCheck.Checked)
                            {
                                txt += ("x9= " + x9 + "; x9cal= " + x9cal + "; err= " + errx9) + "\r\n";
                                txt += ("x10= " + x10 + "; x10cal= " + x10cal + "; err= " + errx10) + "\r\n";
                                txt += ("x11= " + x11 + "; x11cal= " + x11cal + "; err= " + errx11) + "\r\n";
                                txt += ("x12= " + x12 + "; x12cal= " + x12cal + "; err= " + errx12) + "\r\n";
                            }
                            txt += ("y1= " + y1 + "; y1cal= " + y1cal + "; err= " + erry1) + "\r\n";
                            txt += ("y2= " + y2 + "; y2cal= " + y2cal + "; err= " + erry2) + "\r\n";
                            txt += ("y3= " + y3 + "; y3cal= " + y3cal + "; err= " + erry3) + "\r\n";
                            txt += ("y4= " + y4 + "; y4cal= " + y4cal + "; err= " + erry4) + "\r\n";
                            txt += ("y5= " + y5 + "; y5cal= " + y5cal + "; err= " + erry5) + "\r\n";
                            txt += ("y6= " + y6 + "; y6cal= " + y6cal + "; err= " + erry6) + "\r\n";
                            txt += ("y7= " + y7 + "; y7cal= " + y7cal + "; err= " + erry7) + "\r\n";
                            txt += ("y8= " + y8 + "; y8cal= " + y8cal + "; err= " + erry8) + "\r\n";
                            if (AdditionalDataCheck.Checked)
                            {
                                txt += ("y9= " + y9 + "; y9cal= " + y9cal + "; err= " + erry9) + "\r\n";
                                txt += ("y10= " + y10 + "; y10cal= " + y10cal + "; err= " + erry10) + "\r\n";
                                txt += ("y11= " + y11 + "; y11cal= " + y11cal + "; err= " + erry11) + "\r\n";
                                txt += ("y12= " + y12 + "; y12cal= " + y12cal + "; err= " + erry12) + "\r\n";
                            }
                            Invoke(new AddLogTextDelegate(AddLogText), txt);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                /*
                logBox.Text += StaCombo.Text + " error in calibration: " + ex.StackTrace + "\r\n";
                */
                if (InvokeRequired)
                {
                    if (!logBox.IsDisposed)
                    {
                        string txt = "";
                        txt += StaCombo.Text + dimCombo.Text + " error in calibration: " + ex.StackTrace + "\r\n";
                        Invoke(new AddLogTextDelegate(AddLogText), txt);
                    }
                }
            }
            if (InvokeRequired)
            {
                Invoke(new EnableAndDisableButtonDelegate(EnableAndDisableButton));
            }
            _cancel = false;
        }
        private void BtnCalib_Click(object sender, EventArgs e)
        {
            bool inputOK = false;
            logBox.Text = "";
            _cancel = false;
            BtnUpdateCalib.Enabled = false;
            BtnCancel.Enabled = true;
            BtnCalib.Enabled = false;
            BtnExit.Enabled = false;
            StaCombo.Enabled = false;
            dimCombo.Enabled = false;
            //
            AdditionalDataCheck.Enabled = false;
            EpochTxt.Enabled = false;
            LrTxt.Enabled = false;
            logIntervalTxt.Enabled = false;
            X1Txt.Enabled = false;
            X2Txt.Enabled = false;
            X3Txt.Enabled = false;
            X4Txt.Enabled = false;
            X5Txt.Enabled = false;
            X6Txt.Enabled = false;
            X7Txt.Enabled = false;
            X8Txt.Enabled = false;
            X9Txt.Enabled = false;
            X10Txt.Enabled = false;
            X11Txt.Enabled = false;
            X12Txt.Enabled = false;
            Y1Txt.Enabled = false;
            Y2Txt.Enabled = false;
            Y3Txt.Enabled = false;
            Y4Txt.Enabled = false;
            Y5Txt.Enabled = false;
            Y6Txt.Enabled = false;
            Y7Txt.Enabled = false;
            Y8Txt.Enabled = false;
            Y9Txt.Enabled = false;
            Y10Txt.Enabled = false;
            Y11Txt.Enabled = false;
            Y12Txt.Enabled = false;
            Z1Txt.Enabled = false;
            Z2Txt.Enabled = false;
            Z3Txt.Enabled = false;
            Z4Txt.Enabled = false;
            Z5Txt.Enabled = false;
            Z6Txt.Enabled = false;
            Z7Txt.Enabled = false;
            Z8Txt.Enabled = false;
            Z9Txt.Enabled = false;
            Z10Txt.Enabled = false;
            Z11Txt.Enabled = false;
            Z12Txt.Enabled = false;
            u1Txt.Enabled = false;
            u2Txt.Enabled = false;
            u3Txt.Enabled = false;
            u4Txt.Enabled = false;
            u5Txt.Enabled = false;
            u6Txt.Enabled = false;
            u7Txt.Enabled = false;
            u8Txt.Enabled = false;
            u9Txt.Enabled = false;
            u10Txt.Enabled = false;
            u11Txt.Enabled = false;
            u12Txt.Enabled = false;
            v1Txt.Enabled = false;
            v2Txt.Enabled = false;
            v3Txt.Enabled = false;
            v4Txt.Enabled = false;
            v5Txt.Enabled = false;
            v6Txt.Enabled = false;
            v7Txt.Enabled = false;
            v8Txt.Enabled = false;
            v9Txt.Enabled = false;
            v10Txt.Enabled = false;
            v11Txt.Enabled = false;
            v12Txt.Enabled = false;
            CalculateLMS();
            //
            /*new Thread(CalculateOptimize).Start();
            AdditionalDataCheck.Enabled = false;
            EpochTxt.Enabled = false;
            LrTxt.Enabled = false;
            logIntervalTxt.Enabled = false;
            X1Txt.Enabled = false;
            X2Txt.Enabled = false;
            X3Txt.Enabled = false;
            X4Txt.Enabled = false;
            X5Txt.Enabled = false;
            X6Txt.Enabled = false;
            X7Txt.Enabled = false;
            X8Txt.Enabled = false;
            X9Txt.Enabled = false;
            X10Txt.Enabled = false;
            X11Txt.Enabled = false;
            X12Txt.Enabled = false;
            Y1Txt.Enabled = false;
            Y2Txt.Enabled = false;
            Y3Txt.Enabled = false;
            Y4Txt.Enabled = false;
            Y5Txt.Enabled = false;
            Y6Txt.Enabled = false;
            Y7Txt.Enabled = false;
            Y8Txt.Enabled = false;
            Y9Txt.Enabled = false;
            Y10Txt.Enabled = false;
            Y11Txt.Enabled = false;
            Y12Txt.Enabled = false;
            Z1Txt.Enabled = false;
            Z2Txt.Enabled = false;
            Z3Txt.Enabled = false;
            Z4Txt.Enabled = false;
            Z5Txt.Enabled = false;
            Z6Txt.Enabled = false;
            Z7Txt.Enabled = false;
            Z8Txt.Enabled = false;
            Z9Txt.Enabled = false;
            Z10Txt.Enabled = false;
            Z11Txt.Enabled = false;
            Z12Txt.Enabled = false;
            u1Txt.Enabled = false;
            u2Txt.Enabled = false;
            u3Txt.Enabled = false;
            u4Txt.Enabled = false;
            u5Txt.Enabled = false;
            u6Txt.Enabled = false;
            u7Txt.Enabled = false;
            u8Txt.Enabled = false;
            u9Txt.Enabled = false;
            u10Txt.Enabled = false;
            u11Txt.Enabled = false;
            u12Txt.Enabled = false;
            v1Txt.Enabled = false;
            v2Txt.Enabled = false;
            v3Txt.Enabled = false;
            v4Txt.Enabled = false;
            v5Txt.Enabled = false;
            v6Txt.Enabled = false;
            v7Txt.Enabled = false;
            v8Txt.Enabled = false;
            v9Txt.Enabled = false;
            v10Txt.Enabled = false;
            v11Txt.Enabled = false;
            v12Txt.Enabled = false;
            */
            /*
            Int64 epoch = 0;
            Int64 logInterval = 100000;
            double lr = 1E-12;
            double X1 = 0, Y1 = 0, Z1 = 0, X2 = 0, Y2 = 0, Z2 = 0, X3 = 0, Y3 = 0, Z3 = 0, X4 = 0, Y4 = 0, Z4 = 0, X5 = 0, Y5 = 0, Z5 = 0, X6 = 0, Y6 = 0, Z6 = 0;
            double x1 = 0, y1 = 0, x2 = 0, y2 = 0, x3 = 0, y3 = 0, x4 = 0, y4 = 0, x5 = 0, y5 = 0, x6 = 0, y6 = 0;
            double X7 = 0, Y7 = 0, Z7 = 0, X8 = 0, Y8 = 0, Z8 = 0, X9 = 0, Y9 = 0, Z9 = 0, X10 = 0, Y10 = 0, Z10 = 0, X11 = 0, Y11 = 0, Z11 = 0, X12 = 0, Y12 = 0, Z12 = 0;
            double x7 = 0, y7 = 0, x8 = 0, y8 = 0, x9 = 0, y9 = 0, x10 = 0, y10 = 0, x11 = 0, y11 = 0, x12 = 0, y12 = 0;
            try
            {
                try
                {
                    epoch = Convert.ToInt64(EpochTxt.Text);
                    lr = Convert.ToDouble(LrTxt.Text);
                    logInterval = Convert.ToInt64(logIntervalTxt.Text);
                    X1 = Convert.ToInt32(X1Txt.Text);
                    X2 = Convert.ToInt32(X2Txt.Text);
                    X3 = Convert.ToInt32(X3Txt.Text);
                    X4 = Convert.ToInt32(X4Txt.Text);
                    X5 = Convert.ToInt32(X5Txt.Text);
                    X6 = Convert.ToInt32(X6Txt.Text);
                    X7 = Convert.ToInt32(X7Txt.Text);
                    X8 = Convert.ToInt32(X8Txt.Text);
                    Y1 = Convert.ToInt32(Y1Txt.Text);
                    Y2 = Convert.ToInt32(Y2Txt.Text);
                    Y3 = Convert.ToInt32(Y3Txt.Text);
                    Y4 = Convert.ToInt32(Y4Txt.Text);
                    Y5 = Convert.ToInt32(Y5Txt.Text);
                    Y6 = Convert.ToInt32(Y6Txt.Text);
                    Y7 = Convert.ToInt32(Y7Txt.Text);
                    Y8 = Convert.ToInt32(Y8Txt.Text);
                    Z1 = Convert.ToInt32(Z1Txt.Text);
                    Z2 = Convert.ToInt32(Z2Txt.Text);
                    Z3 = Convert.ToInt32(Z3Txt.Text);
                    Z4 = Convert.ToInt32(Z4Txt.Text);
                    Z5 = Convert.ToInt32(Z5Txt.Text);
                    Z6 = Convert.ToInt32(Z6Txt.Text);
                    Z7 = Convert.ToInt32(Z7Txt.Text);
                    Z8 = Convert.ToInt32(Z8Txt.Text);
                    x1 = Convert.ToInt32(u1Txt.Text);
                    x2 = Convert.ToInt32(u2Txt.Text);
                    x3 = Convert.ToInt32(u3Txt.Text);
                    x4 = Convert.ToInt32(u4Txt.Text);
                    x5 = Convert.ToInt32(u5Txt.Text);
                    x6 = Convert.ToInt32(u6Txt.Text);
                    x7 = Convert.ToInt32(u7Txt.Text);
                    x8 = Convert.ToInt32(u8Txt.Text);
                    y1 = Convert.ToInt32(v1Txt.Text);
                    y2 = Convert.ToInt32(v2Txt.Text);
                    y3 = Convert.ToInt32(v3Txt.Text);
                    y4 = Convert.ToInt32(v4Txt.Text);
                    y5 = Convert.ToInt32(v5Txt.Text);
                    y6 = Convert.ToInt32(v6Txt.Text);
                    y7 = Convert.ToInt32(v7Txt.Text);
                    y8 = Convert.ToInt32(v8Txt.Text);
                    if (AdditionalDataCheck.Checked)
                    {
                        X9 = Convert.ToInt32(X9Txt.Text);
                        X10 = Convert.ToInt32(X10Txt.Text);
                        X11 = Convert.ToInt32(X11Txt.Text);
                        X12 = Convert.ToInt32(X12Txt.Text);
                        Y9 = Convert.ToInt32(Y9Txt.Text);
                        Y10 = Convert.ToInt32(Y10Txt.Text);
                        Y11 = Convert.ToInt32(Y11Txt.Text);
                        Y12 = Convert.ToInt32(Y12Txt.Text);
                        Z9 = Convert.ToInt32(Z9Txt.Text);
                        Z10 = Convert.ToInt32(Z10Txt.Text);
                        Z11 = Convert.ToInt32(Z11Txt.Text);
                        Z12 = Convert.ToInt32(Z12Txt.Text);
                        x9 = Convert.ToInt32(u9Txt.Text);
                        x10 = Convert.ToInt32(u10Txt.Text);
                        x11 = Convert.ToInt32(u11Txt.Text);
                        x12 = Convert.ToInt32(u12Txt.Text);
                        y9 = Convert.ToInt32(v9Txt.Text);
                        y10 = Convert.ToInt32(v10Txt.Text);
                        y11 = Convert.ToInt32(v11Txt.Text);
                        y12 = Convert.ToInt32(v12Txt.Text);
                    }
                    inputOK = true;
                }
                catch (Exception ex)
                {
                    inputOK = false;
                    MessageBox.Show("请正确填写标定参数", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                if (inputOK)
                {
                    double[,] XYZ = { { X1, Y1, Z1, 1, 0, 0, 0, 0, -X1*x1, -Y1*x1, -Z1*x1 },
                                        { 0, 0, 0, 0, X1, Y1, Z1, 1, -X1*y1, -Y1*y1, -Z1*y1 },
                                        { X2, Y2, Z2, 1, 0, 0, 0, 0, -X2*x2, -Y2*x2, -Z2*x2 },
                                        { 0, 0, 0, 0, X2, Y2, Z2, 1, -X2*y2, -Y2*y2, -Z2*y2 },
                                        { X4, Y4, Z4, 1, 0, 0, 0, 0, -X4*x4, -Y4*x4, -Z4*x4 },
                                        { 0, 0, 0, 0, X4, Y4, Z4, 1, -X4*y4, -Y4*y4, -Z4*y4 },
                                        { X7, Y7, Z7, 1, 0, 0, 0, 0, -X7*x7, -Y7*x7, -Z7*x7 },
                                        { 0, 0, 0, 0, X7, Y7, Z7, 1, -X7*y7, -Y7*y7, -Z7*y7 },
                                        { X6, Y6, Z6, 1, 0, 0, 0, 0, -X6*x6, -Y6*x6, -Z6*x6 },
                                        { 0, 0, 0, 0, X6, Y6, Z6, 1, -X6*y6, -Y6*y6, -Z6*y6 },
                                        { X8, Y8, Z8, 1, 0, 0, 0, 0, -X8*x8, -Y8*x8, -Z8*x8 }};
                    Matrix3 XYZMatrix = new Matrix3(XYZ);
                    double[,] xy = new double[11, 1];
                    xy[0, 0] = x1;
                    xy[1, 0] = y1;
                    xy[2, 0] = x2;
                    xy[3, 0] = y2;
                    xy[4, 0] = x4;
                    xy[5, 0] = y4;
                    xy[6, 0] = x7;
                    xy[7, 0] = y7;
                    xy[8, 0] = x6;
                    xy[9, 0] = y6;
                    xy[10, 0] = x8;
                    Matrix3 xyMatrix = new Matrix3(xy);
                    Matrix3 R1 = XYZMatrix.Inverse() * xyMatrix;
                    r11 = R1[0, 0];
                    r12 = R1[1, 0];
                    r13 = R1[2, 0];
                    r14 = R1[3, 0];
                    r21 = R1[4, 0];
                    r22 = R1[5, 0];
                    r23 = R1[6, 0];
                    r24 = R1[7, 0];
                    r31 = R1[8, 0];
                    r32 = R1[9, 0];
                    r33 = R1[10, 0];
                    double[] initial = { r11, r12, r13, r14, r21, r22, r23, r24, r31, r32, r33 };
                    double[,] input = { { X1, Y1, Z1 }, { X2, Y2, Z2 }, { X3, Y3, Z3 }, { X4, Y4, Z4 }, { X5, Y5, Z5 }, { X6, Y6, Z6 }, { X7, Y7, Z7 }, { X8, Y8, Z8 }, { X9, Y9, Z9 }, { X10, Y10, Z10 }, { X11, Y11, Z11 }, { X12, Y12, Z12 } };
                    double[,] output = { { x1, y1 }, { x2, y2 }, { x3, y3 }, { x4, y4 }, { x5, y5 }, { x6, y6 }, { x7, y7 }, { x8, y8 }, { x9, y9 }, { x10, y10 }, { x11, y11 }, { x12, y12 } };
                    double[,] inputraw = { { X1, Y1, Z1 }, { X2, Y2, Z2 }, { X3, Y3, Z3 }, { X4, Y4, Z4 }, { X5, Y5, Z5 }, { X6, Y6, Z6 }, { X7, Y7, Z7 }, { X8, Y8, Z8 } };
                    double[,] outputraw = { { x1, y1 }, { x2, y2 }, { x3, y3 }, { x4, y4 }, { x5, y5 }, { x6, y6 }, { x7, y7 }, { x8, y8 } };
                    if (AdditionalDataCheck.Checked)
                    {
                        double[] refined = gradientdescent(input, output, initial, epoch, lr, logInterval);
                        r11 = refined[0];
                        r12 = refined[1];
                        r13 = refined[2];
                        r14 = refined[3];
                        r21 = refined[4];
                        r22 = refined[5];
                        r23 = refined[6];
                        r24 = refined[7];
                        r31 = refined[8];
                        r32 = refined[9];
                        r33 = refined[10];
                    }
                    else
                    {
                        double[] refinedraw = gradientdescent(inputraw, outputraw, initial, epoch, lr, logInterval);
                        r11 = refinedraw[0];
                        r12 = refinedraw[1];
                        r13 = refinedraw[2];
                        r14 = refinedraw[3];
                        r21 = refinedraw[4];
                        r22 = refinedraw[5];
                        r23 = refinedraw[6];
                        r24 = refinedraw[7];
                        r31 = refinedraw[8];
                        r32 = refinedraw[9];
                        r33 = refinedraw[10];
                    }
                    int x1cal = (int)((X1 * r11 + Y1 * r12 + Z1 * r13 + r14) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1));
                    int errx1 = (int)(x1cal - x1);
                    int x2cal = (int)((X2 * r11 + Y2 * r12 + Z2 * r13 + r14) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1));
                    int errx2 = (int)(x2cal - x2);
                    int x3cal = (int)((X3 * r11 + Y3 * r12 + Z3 * r13 + r14) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1));
                    int errx3 = (int)(x3cal - x3);
                    int x4cal = (int)((X4 * r11 + Y4 * r12 + Z4 * r13 + r14) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1));
                    int errx4 = (int)(x4cal - x4);
                    int x5cal = (int)((X5 * r11 + Y5 * r12 + Z5 * r13 + r14) / (X5 * r31 + Y5 * r32 + Z5 * r33 + 1));
                    int errx5 = (int)(x5cal - x5);
                    int x6cal = (int)((X6 * r11 + Y6 * r12 + Z6 * r13 + r14) / (X6 * r31 + Y6 * r32 + Z6 * r33 + 1));
                    int errx6 = (int)(x6cal - x6);
                    int x7cal = (int)((X7 * r11 + Y7 * r12 + Z7 * r13 + r14) / (X7 * r31 + Y7 * r32 + Z7 * r33 + 1));
                    int errx7 = (int)(x7cal - x7);
                    int x8cal = (int)((X8 * r11 + Y8 * r12 + Z8 * r13 + r14) / (X8 * r31 + Y8 * r32 + Z8 * r33 + 1));
                    int errx8 = (int)(x8cal - x8);
                    int x9cal = (int)((X9 * r11 + Y9 * r12 + Z9 * r13 + r14) / (X9 * r31 + Y9 * r32 + Z9 * r33 + 1));
                    int errx9 = (int)(x9cal - x9);
                    int x10cal = (int)((X10 * r11 + Y10 * r12 + Z10 * r13 + r14) / (X10 * r31 + Y10 * r32 + Z10 * r33 + 1));
                    int errx10 = (int)(x10cal - x10);
                    int x11cal = (int)((X11 * r11 + Y11 * r12 + Z11 * r13 + r14) / (X11 * r31 + Y11 * r32 + Z11 * r33 + 1));
                    int errx11 = (int)(x11cal - x11);
                    int x12cal = (int)((X12 * r11 + Y12 * r12 + Z12 * r13 + r14) / (X12 * r31 + Y12 * r32 + Z12 * r33 + 1));
                    int errx12 = (int)(x12cal - x12);
                    int y1cal = (int)((X1 * r21 + Y1 * r22 + Z1 * r23 + r24) / (X1 * r31 + Y1 * r32 + Z1 * r33 + 1));
                    int erry1 = (int)(y1cal - y1);
                    int y2cal = (int)((X2 * r21 + Y2 * r22 + Z2 * r23 + r24) / (X2 * r31 + Y2 * r32 + Z2 * r33 + 1));
                    int erry2 = (int)(y2cal - y2);
                    int y3cal = (int)((X3 * r21 + Y3 * r22 + Z3 * r23 + r24) / (X3 * r31 + Y3 * r32 + Z3 * r33 + 1));
                    int erry3 = (int)(y3cal - y3);
                    int y4cal = (int)((X4 * r21 + Y4 * r22 + Z4 * r23 + r24) / (X4 * r31 + Y4 * r32 + Z4 * r33 + 1));
                    int erry4 = (int)(y4cal - y4);
                    int y5cal = (int)((X5 * r21 + Y5 * r22 + Z5 * r23 + r24) / (X5 * r31 + Y5 * r32 + Z5 * r33 + 1));
                    int erry5 = (int)(y5cal - y5);
                    int y6cal = (int)((X6 * r21 + Y6 * r22 + Z6 * r23 + r24) / (X6 * r31 + Y6 * r32 + Z6 * r33 + 1));
                    int erry6 = (int)(y6cal - y6);
                    int y7cal = (int)((X7 * r21 + Y7 * r22 + Z7 * r23 + r24) / (X7 * r31 + Y7 * r32 + Z7 * r33 + 1));
                    int erry7 = (int)(y7cal - y7);
                    int y8cal = (int)((X8 * r21 + Y8 * r22 + Z8 * r23 + r24) / (X8 * r31 + Y8 * r32 + Z8 * r33 + 1));
                    int erry8 = (int)(y8cal - y8);
                    int y9cal = (int)((X9 * r21 + Y9 * r22 + Z9 * r23 + r24) / (X9 * r31 + Y9 * r32 + Z9 * r33 + 1));
                    int erry9 = (int)(y9cal - y9);
                    int y10cal = (int)((X10 * r21 + Y10 * r22 + Z10 * r23 + r24) / (X10 * r31 + Y10 * r32 + Z10 * r33 + 1));
                    int erry10 = (int)(y10cal - y10);
                    int y11cal = (int)((X11 * r21 + Y11 * r22 + Z11 * r23 + r24) / (X11 * r31 + Y11 * r32 + Z11 * r33 + 1));
                    int erry11 = (int)(y11cal - y11);
                    int y12cal = (int)((X12 * r21 + Y12 * r22 + Z12 * r23 + r24) / (X12 * r31 + Y12 * r32 + Z12 * r33 + 1));
                    int erry12 = (int)(y12cal - y12);

                    logBox.Text += ("x1= " + x1 + "; x1cal= " + x1cal + "; err= " + errx1) + "\r\n";
                    logBox.Text += ("x2= " + x2 + "; x2cal= " + x2cal + "; err= " + errx2) + "\r\n";
                    logBox.Text += ("x3= " + x3 + "; x3cal= " + x3cal + "; err= " + errx3) + "\r\n";
                    logBox.Text += ("x4= " + x4 + "; x4cal= " + x4cal + "; err= " + errx4) + "\r\n";
                    logBox.Text += ("x5= " + x5 + "; x5cal= " + x5cal + "; err= " + errx5) + "\r\n";
                    logBox.Text += ("x6= " + x6 + "; x6cal= " + x6cal + "; err= " + errx6) + "\r\n";
                    logBox.Text += ("x7= " + x7 + "; x7cal= " + x7cal + "; err= " + errx7) + "\r\n";
                    logBox.Text += ("x8= " + x8 + "; x8cal= " + x8cal + "; err= " + errx8) + "\r\n";
                    logBox.Text += ("x9= " + x9 + "; x9cal= " + x9cal + "; err= " + errx9) + "\r\n";
                    logBox.Text += ("x10= " + x10 + "; x10cal= " + x10cal + "; err= " + errx10) + "\r\n";
                    logBox.Text += ("x11= " + x11 + "; x11cal= " + x11cal + "; err= " + errx11) + "\r\n";
                    logBox.Text += ("x12= " + x12 + "; x12cal= " + x12cal + "; err= " + errx12) + "\r\n";
                    logBox.Text += ("y1= " + y1 + "; y1cal= " + y1cal + "; err= " + erry1) + "\r\n";
                    logBox.Text += ("y2= " + y2 + "; y2cal= " + y2cal + "; err= " + erry2) + "\r\n";
                    logBox.Text += ("y3= " + y3 + "; y3cal= " + y3cal + "; err= " + erry3) + "\r\n";
                    logBox.Text += ("y4= " + y4 + "; y4cal= " + y4cal + "; err= " + erry4) + "\r\n";
                    logBox.Text += ("y5= " + y5 + "; y5cal= " + y5cal + "; err= " + erry5) + "\r\n";
                    logBox.Text += ("y6= " + y6 + "; y6cal= " + y6cal + "; err= " + erry6) + "\r\n";
                    logBox.Text += ("y7= " + y7 + "; y7cal= " + y7cal + "; err= " + erry7) + "\r\n";
                    logBox.Text += ("y8= " + y8 + "; y8cal= " + y8cal + "; err= " + erry8) + "\r\n";
                    logBox.Text += ("y9= " + y9 + "; y9cal= " + y9cal + "; err= " + erry9) + "\r\n";
                    logBox.Text += ("y10= " + y10 + "; y10cal= " + y10cal + "; err= " + erry10) + "\r\n";
                    logBox.Text += ("y11= " + y11 + "; y11cal= " + y11cal + "; err= " + erry11) + "\r\n";
                    logBox.Text += ("y12= " + y12 + "; y12cal= " + y12cal + "; err= " + erry12) + "\r\n";
                }
            }
            catch (Exception ex) { logBox.Text += StaCombo.Text + " error in calibration: " + ex.StackTrace+"\r\n"; }
            if (!_cancel)
            {
                BtnUpdateCalib.Enabled = true;
            }
            else
            {
                BtnUpdateCalib.Enabled = false;
            }
            BtnCalib.Enabled = true;
            BtnExit.Enabled = true;
            BtnCancel.Enabled = false;
            StaCombo.Enabled = true;
            X1Txt.Enabled = true;
            X2Txt.Enabled = true;
            X3Txt.Enabled = true;
            X4Txt.Enabled = true;
            X5Txt.Enabled = true;
            X6Txt.Enabled = true;
            X7Txt.Enabled = true;
            X8Txt.Enabled = true;
            X9Txt.Enabled = true;
            X10Txt.Enabled = true;
            X11Txt.Enabled = true;
            X12Txt.Enabled = true;
            Y1Txt.Enabled = true;
            Y2Txt.Enabled = true;
            Y3Txt.Enabled = true;
            Y4Txt.Enabled = true;
            Y5Txt.Enabled = true;
            Y6Txt.Enabled = true;
            Y7Txt.Enabled = true;
            Y8Txt.Enabled = true;
            Y9Txt.Enabled = true;
            Y10Txt.Enabled = true;
            Y11Txt.Enabled = true;
            Y12Txt.Enabled = true;
            Z1Txt.Enabled = true;
            Z2Txt.Enabled = true;
            Z3Txt.Enabled = true;
            Z4Txt.Enabled = true;
            Z5Txt.Enabled = true;
            Z6Txt.Enabled = true;
            Z7Txt.Enabled = true;
            Z8Txt.Enabled = true;
            Z9Txt.Enabled = true;
            Z10Txt.Enabled = true;
            Z11Txt.Enabled = true;
            Z12Txt.Enabled = true;
            u1Txt.Enabled = true;
            u2Txt.Enabled = true;
            u3Txt.Enabled = true;
            u4Txt.Enabled = true;
            u5Txt.Enabled = true;
            u6Txt.Enabled = true;
            u7Txt.Enabled = true;
            u8Txt.Enabled = true;
            u9Txt.Enabled = true;
            u10Txt.Enabled = true;
            u11Txt.Enabled = true;
            u12Txt.Enabled = true;
            v1Txt.Enabled = true;
            v2Txt.Enabled = true;
            v3Txt.Enabled = true;
            v4Txt.Enabled = true;
            v5Txt.Enabled = true;
            v6Txt.Enabled = true;
            v7Txt.Enabled = true;
            v8Txt.Enabled = true;
            v9Txt.Enabled = true;
            v10Txt.Enabled = true;
            v11Txt.Enabled = true;
            v12Txt.Enabled = true;
            _cancel = false;*/
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            _cancel = true;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void BtnUpdateCalib_Click(object sender, EventArgs e)
        {
            calObject cal = MainForm.cal1_1;
            string path = MainForm.cal_path1_1;
            if (StaCombo.Text == "工位一" && dimCombo.Text == "DIM0")
            {
                cal = MainForm.cal1_1;
                path = MainForm.cal_path1_1;
            }
            else if (StaCombo.Text == "工位一" && dimCombo.Text == "DIM1")
            {
                cal = MainForm.cal1_2;
                path = MainForm.cal_path1_2;
            }
            else if (StaCombo.Text == "工位二" && dimCombo.Text == "DIM0")
            {
                cal = MainForm.cal2_1;
                path = MainForm.cal_path2_1;
            }
            else if (StaCombo.Text == "工位二" && dimCombo.Text == "DIM1")
            {
                cal = MainForm.cal2_2;
                path = MainForm.cal_path2_2;
            }
            else if (StaCombo.Text == "工位三" && dimCombo.Text == "DIM0")
            {
                cal = MainForm.cal3_1;
                path = MainForm.cal_path3_1;
            }
            else if (StaCombo.Text == "工位三" && dimCombo.Text == "DIM1")
            {
                cal = MainForm.cal3_2;
                path = MainForm.cal_path3_2;
            }
            else if (StaCombo.Text == "工位四" && dimCombo.Text == "DIM0")
            {
                cal = MainForm.cal4_1;
                path = MainForm.cal_path4_1;
            }
            else if (StaCombo.Text == "工位四" && dimCombo.Text == "DIM1")
            {
                cal = MainForm.cal4_2;
                path = MainForm.cal_path4_2;
            }
            DialogResult result = MessageBox.Show("是否确定要更新标定信息", "警告", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.OK)//确定标定
            {
                cal.r11 = r11;
                cal.r12 = r12;
                cal.r13 = r13;
                cal.r14 = r14;
                cal.r21 = r21;
                cal.r22 = r22;
                cal.r23 = r23;
                cal.r24 = r24;
                cal.r31 = r31;
                cal.r32 = r32;
                cal.r33 = r33;
                cal.calOK = cal.r11 != 0 || cal.r12 != 0 || cal.r13 != 0 || cal.r14 != 0 || cal.r21 != 0 || cal.r22 != 0 || cal.r23 != 0 || cal.r24 != 0 || cal.r31 != 0 || cal.r32 != 0 || cal.r33 != 0;
                FileStream fs = null;
                StreamWriter sw = null;
                try
                {
                    string dir = MainForm.cal_dir;
                    if (!Directory.Exists(dir))
                    {
                        Directory.CreateDirectory(dir);
                    }
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                    var calibjsonObject = new
                    {
                        r11 = r11,
                        r12 = r12,
                        r13 = r13,
                        r14 = r14,
                        r21 = r21,
                        r22 = r22,
                        r23 = r23,
                        r24 = r24,
                        r31 = r31,
                        r32 = r32,
                        r33 = r33
                    };
                    string jsonstr = JsonConvert.SerializeObject(calibjsonObject);
                    fs = new FileStream(path, FileMode.Append, FileAccess.Write);
                    sw = new StreamWriter(fs);
                    sw.WriteLine(jsonstr);
                    sw.Close();
                    fs.Close();
                    logBox.Text += StaCombo.Text + dimCombo.Text + " calibration updated successfully!\r\n";
                    BtnUpdateCalib.Enabled = false;
                }
                catch (Exception ex)
                {
                    logBox.Text += StaCombo.Text + dimCombo.Text + " error updating calibration!" + ex.StackTrace + "\r\n";
                    BtnUpdateCalib.Enabled = true;
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
            else
            {
                BtnUpdateCalib.Enabled = true;
            }
        }
    }
}
