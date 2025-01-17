using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Reflection;


namespace 存图助手标准版
{
   
    static class Program
    {
        /// <summary>
        /// 该函数设置由不同线程产生的窗口的显示状态。
        /// </summary>
        /// <param name="hWnd">窗口句柄</param>
        /// <param name="cmdShow">指定窗口如何显示。查看允许值列表，请查阅ShowWlndow函数的说明部分。</param>
        /// <returns>如果函数原来可见，返回值为非零；如果函数原来被隐藏，返回值为零。</returns>
        [DllImport("User32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int cmdShow);
        /// <summary>
        /// 该函数将创建指定窗口的线程设置到前台，并且激活该窗口。键盘输入转向该窗口，并为用户改各种可视的记号。系统给创建前台窗口的线程分配的权限稍高于其他线程。
        /// </summary>
        /// <param name="hWnd">将被激活并被调入前台的窗口句柄。</param>
        /// <returns>如果窗口设入了前台，返回值为非零；如果窗口未被设入前台，返回值为零。</returns>
        [DllImport("User32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        private const int WS_SHOWNORMAL = 1;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Process instance = RunningInstance();
            
            
            if (instance == null)
            {
                try
                {
#if FEATURE_VPRO
                    Cognex.Vision.Startup.Initialize(Cognex.Vision.Startup.ProductKey.VProX);
#endif
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    Application.Run(new MainForm());
#if FEATURE_VPRO
                    Cognex.Vision.Startup.Shutdown();
#endif
                }
                catch(Exception ex)
                {
                    string s = ex.Message + "\r\n" + ex.StackTrace;
                    while (ex.InnerException != null)
                    {
                        ex = ex.InnerException;
                        s += ex.Message + "\r\n" + ex.StackTrace;
                    }
                    MessageBox.Show(s, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                HandleRunningInstance(instance);
            }
        }
        public static Process RunningInstance()
        {
            Process returnPro = null;
            Process current = Process.GetCurrentProcess();
            try
            {
                Process[] processes = Process.GetProcessesByName(current.ProcessName);
                try
                {
                    foreach (Process process in processes)
                    {
                        if (process != null)
                        {
                            if (process.Id != current.Id)
                            {
                                if (process.MainModule.FileName == current.MainModule.FileName)
                                {
                                    returnPro = process;
                                }
                            }
                        }
                    }
                }
                catch { }
                finally
                {
                    if (current != null)
                    {
                        current.Dispose();
                    }
                    foreach (Process process in processes)
                    {
                        if (process != returnPro && process != null)
                        {
                            process.Dispose();
                        }
                    }
                }
            }
            catch { }
            return returnPro;
        }

        /// <summary>
        /// 显示已运行的程序。
        /// </summary>
        public static void HandleRunningInstance(Process instance)
        {
            try
            {
                ShowWindowAsync(instance.MainWindowHandle, WS_SHOWNORMAL); //显示，可以注释掉
                SetForegroundWindow(instance.MainWindowHandle);            //放到前端
            }
            catch { }
            finally
            {
                if (instance != null)
                {
                    instance.Dispose();
                }
            }  
        }
    }
}
