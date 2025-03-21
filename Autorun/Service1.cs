using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using Cjwdev.WindowsApi;
using System.Runtime.InteropServices;
using System.Threading;
using System.Security;

namespace Autorun
{
    public partial class Service1 : ServiceBase
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(
        IntPtr tokenHandle,
        int tokenInformationClass,
        ref IntPtr tokenInformation,
        int tokenInformationLength,
        out int returnLength);
        System.Timers.Timer Timer1 = new System.Timers.Timer();
        public Service1()
        {
            InitializeComponent();
            this.CanPauseAndContinue = false;
            this.CanStop = true; 
            
            Timer1.Interval = 3000;  //设置计时器事件间隔执行时间
            Timer1.Elapsed += new System.Timers.ElapsedEventHandler(timer1_Elapsed);
            Timer1.AutoReset = true;
        }
        private void timer1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                string appName = "存图助手标准版";//the path of the exe file
                string appPath = Application.StartupPath + "\\存图助手标准版.exe";
                bool runFlag = false;
                
                Process[] myProcesses = Process.GetProcesses();
                foreach (Process myProcess in myProcesses)
                {
                    if (myProcess.ProcessName.CompareTo(appName) == 0)
                    {
                        runFlag = true;
                    }
                }
                
                FileStream fs = null;
                StreamWriter sr = null;
                try
                {
                    //fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                    //sr = new StreamWriter(fs);
                    //sr.WriteLine("timer called! runflag= " + runFlag + "appPath: " + appPath);
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
                
                if (!runFlag)   //如果程序没有启动
                {
                    /*
                    Process proc = new Process();
                    proc.StartInfo.FileName = appName;
                    proc.StartInfo.WorkingDirectory = Path.GetDirectoryName(appPath);
                    proc.StartInfo.UseShellExecute = false;
                    proc.StartInfo.Verb = "runas";
                    proc.StartInfo.CreateNoWindow = false;
                    proc.Start();
                    */
                    AppStart(appPath);
                    //ApplicationLoader.PROCESS_INFORMATION procInfo;
                    //ApplicationLoader.StartProcessAndBypassUAC(appPath, out procInfo);
                }
                
            }
            catch (Exception ex) 
            {
                FileStream fs = null;
                StreamWriter sr = null;
                try
                {
                    //fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                    //sr = new StreamWriter(fs);
                    //sr.WriteLine(ex.StackTrace);
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
        
        protected override void OnStart(string[] args)
        {
            
            
            
            Timer1.Enabled = true;
            
        }

        protected override void OnStop()
        {
            Timer1.Enabled = false;
        }
        protected override void OnPause()
        {
            base.OnPause();
            Timer1.Enabled = false;
        }

        protected override void OnContinue()
        {
            base.OnContinue();
            Timer1.Enabled = true;
        }

        public void AppStart(string appPath)
        {
            FileStream fs = null;
            StreamWriter sr = null;
            try
            {
                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                IntPtr unfiltereduserTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);
                /*try
                {
                    fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                    sr = new StreamWriter(fs);
                    sr.WriteLine("NormalToken:" + userTokenHandle.ToInt32());
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
                if (userTokenHandle != IntPtr.Zero)
                {
                    int length = 0;
                    unsafe
                    {
                        GetTokenInformation(userTokenHandle, 19, ref unfiltereduserTokenHandle, sizeof(IntPtr), out length);
                    }
                    /*try
                    {
                        fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                        sr = new StreamWriter(fs);
                        sr.WriteLine("AdminToken:" + unfiltereduserTokenHandle.ToInt32());
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
                    if (unfiltereduserTokenHandle != IntPtr.Zero)
                    {
                        ApiDefinitions.CreateProcessAsUser(
                            unfiltereduserTokenHandle,
                            appStartPath,
                            "",
                            IntPtr.Zero,
                            IntPtr.Zero,
                            false,
                            0,
                            IntPtr.Zero,
                            null,
                            ref startInfo,
                            out procInfo);
                        ApiDefinitions.CloseHandle(unfiltereduserTokenHandle);
                    }
                    else
                    {
                        ApiDefinitions.CreateProcessAsUser(
                            userTokenHandle,
                            appStartPath,
                            "",
                            IntPtr.Zero,
                            IntPtr.Zero,
                            false,
                            0,
                            IntPtr.Zero,
                            null,
                            ref startInfo,
                            out procInfo);
                        ApiDefinitions.CloseHandle(userTokenHandle);
                    }
                    int _currentAquariusProcessId = (int)procInfo.dwProcessId;
                }
            }
            catch (Exception ex)
            {
                /*
                try
                {
                    fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                    sr = new StreamWriter(fs);
                    sr.WriteLine(ex.StackTrace);
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
                */
            }
        }
        public void AppStartOrigin(string appPath)
        {
            FileStream fs = null;
            StreamWriter sr = null;
            try
            {
                string appStartPath = appPath;
                IntPtr userTokenHandle = IntPtr.Zero;
                ApiDefinitions.WTSQueryUserToken(ApiDefinitions.WTSGetActiveConsoleSessionId(), ref userTokenHandle);

                ApiDefinitions.PROCESS_INFORMATION procInfo = new ApiDefinitions.PROCESS_INFORMATION();
                ApiDefinitions.STARTUPINFO startInfo = new ApiDefinitions.STARTUPINFO();
                startInfo.cb = (uint)Marshal.SizeOf(startInfo);
                try
                {
                    //fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                    //sr = new StreamWriter(fs);
                    //sr.WriteLine(userTokenHandle.ToInt32());
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
                if (userTokenHandle != IntPtr.Zero)
                {
                    ApiDefinitions.CreateProcessAsUser(
                        userTokenHandle,
                        appStartPath,
                        "",
                        IntPtr.Zero,
                        IntPtr.Zero,
                        false,
                        0,
                        IntPtr.Zero,
                        null,
                        ref startInfo,
                        out procInfo);

                    if (userTokenHandle != IntPtr.Zero)
                        ApiDefinitions.CloseHandle(userTokenHandle);

                    int _currentAquariusProcessId = (int)procInfo.dwProcessId;
                }
            }
            catch (Exception ex)
            {
                /*
                try
                {
                    fs = new FileStream("D:\\log1.txt", FileMode.Append, FileAccess.Write);
                    sr = new StreamWriter(fs);
                    sr.WriteLine(ex.StackTrace);
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
                */
            }
        }
    }
}
