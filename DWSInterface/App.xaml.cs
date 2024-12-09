using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DWSInterface
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
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
        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        private const int SW_MAXIMIZE = 3;
        private const int WS_SHOWNORMAL = 1;
        public App()
        {
            this.Startup += App_Startup;
            AppDomain.CurrentDomain.UnhandledException += DWSInterface.MainWindow.CurrentDomain_UnhandledException;
        }
        private void App_Startup(object sender, StartupEventArgs e)
        {
            Process instance = RunningInstance();
            if (instance != null)
            {
                HandleRunningInstance(instance);
                Environment.Exit(0);
                Process.GetCurrentProcess().Kill();
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
                //ShowWindowAsync(instance.MainWindowHandle, SW_MAXIMIZE); //显示，可以注释掉
                ShowWindow(instance.MainWindowHandle, SW_MAXIMIZE);
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
