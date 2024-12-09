using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using Shell32;
using System.IO;
using Microsoft.Win32;
using System.IO.Compression;
using System.Threading;
using System.ServiceProcess;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Library
{
    [RunInstaller(true)]
    public partial class InstallerTest : System.Configuration.Install.Installer
    {
        [DllImport("kernel32.dll")]
        static extern bool AllocConsole();
        public InstallerTest()
        {
            InitializeComponent();
            this.BeforeUninstall += new InstallEventHandler(InstallerTest_BeforeUninstall);
            this.AfterInstall += new InstallEventHandler(InstallerTest_AfterInstall);
        }
        /*
        private void appendPath(string path)
        {
            string pathStr = "";
            RegistryKey key = null;

            RegistryKey env = null;
            try
            {
                key = Registry.LocalMachine;

                env = key.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Environment", true); //该项必须已存在
                                                                                                             // 从注册表读取系统环境变量Path值（%SystemRoot%系统变量不会被替换为C:\Windows）
                pathStr = (string)env.GetValue("PATH", "", RegistryValueOptions.DoNotExpandEnvironmentNames);
            }
            catch
            { }
            finally
            {
                if (env != null)
                {
                    env.Dispose();
                }
                if (key != null)
                {
                    key.Dispose();
                }
            }
            // 读取系统环境变量Path值（%SystemRoot%会自动转换为真实的C:\Windows）
            // string pathStr = System.Environment.GetEnvironmentVariable("Path", EnvironmentVariableTarget.Machine);
            
            // 字符串转string[]再转ArrayList方便添加路径
            string[] pathArray = pathStr.Split(';');
            ArrayList pathList = new ArrayList(pathArray);
            if (!pathList.Contains(path))
            {
                pathList.Add(path); // 检查不存在则添加
            }

            // ArrayList转换string[]再连接成字符串
            pathArray = (string[])pathList.ToArray(typeof(string));
            pathStr = string.Join(";", pathArray);
            // 修改环境变量Path值
            Environment.SetEnvironmentVariable("Path", pathStr, EnvironmentVariableTarget.Machine);


        }
        */
        /*
        private void InstallerTest_AfterInstall(object sender, InstallEventArgs e)
        {
            try
            {
                string bottomcalib_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\bottomscan calibration";
                string calib_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\calib";
                string ftplog_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\FtpLog";
                string systemlog_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\SystemLog";
                string keys_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\keys";
                string servicePath = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\Image Handler Autorun.exe";
                string serviceName = "Image Handler Autorun";
                string path = Application.StartupPath + "\\mysql-9.0.1-winx64.zip";
                string zipPath = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].IndexOf("\\"));
                if (!Directory.Exists(zipPath + "\\mysql-9.0.1-winx64"))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(path, zipPath);
                    if (Directory.Exists(zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64\\data"))
                    {
                        Directory.Delete(zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64\\data", true);
                    }
                    Directory.CreateDirectory(zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64\\data");
                    FileStream fs = null;
                    StreamWriter sw = null;
                    try
                    {
                        fs = new FileStream(zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64\\my.ini", FileMode.Append);
                        sw = new StreamWriter(fs);
                        sw.WriteLine("[mysqld]");
                        sw.WriteLine("# 设置3306端口");
                        sw.WriteLine("port=3306");
                        sw.WriteLine("# 设置mysql的安装目录   ----------Mysql解压之后的文件路径-------------");
                        sw.WriteLine("basedir=" + zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64");
                        sw.WriteLine("# 设置mysql数据库的数据的存放目录  ---------文件夹自行创建");
                        sw.WriteLine("datadir=" + zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64\\data");
                        sw.WriteLine("# 允许最大连接数");
                        sw.WriteLine("max_connections=200");
                        sw.WriteLine("# 允许连接失败的次数");
                        sw.WriteLine("max_connect_errors=10");
                        sw.WriteLine("# 服务端使用的字符集默认为utf8");
                        sw.WriteLine("character-set-server=utf8");
                        sw.WriteLine("# 创建新表时将使用的默认存储引擎");
                        sw.WriteLine("default-storage-engine=INNODB");
                        sw.WriteLine("# 默认使用“mysql_native_password”插件认证");
                        sw.WriteLine("#mysql_native_password");
                        sw.WriteLine("#default_authentication_plugin=caching_sha2_password");
                        sw.WriteLine("[mysql]");
                        sw.WriteLine("# 设置mysql客户端默认字符集");
                        sw.WriteLine("default-character-set=utf8");
                        sw.WriteLine("[client]");
                        sw.WriteLine("# 设置mysql客户端连接服务端时默认使用的端口");
                        sw.WriteLine("port=3306");
                        sw.WriteLine("default-character-set=utf8");
                    }
                    catch { }
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
                try
                {
                    System.Environment.SetEnvironmentVariable("MYSQL_HOME", zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64", EnvironmentVariableTarget.Machine);
                    appendPath("%MYSQL_HOME%\\bin");
                }
                catch { }
                Process p = new Process();
                byte[] bytes = new byte[102400];
                string InitialPassword = "";
                try
                {
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.Start();
                    using (StreamWriter sw = p.StandardInput)
                    {
                        using (StreamReader sr = p.StandardOutput)
                        {
                            bytes = new byte[102400];
                            sr.BaseStream.Read(bytes, 0, 102400);
                            if (sw.BaseStream.CanWrite)
                            {
                                sw.WriteLine(zipPath);
                                sw.WriteLine("CD " + this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2));
                                sw.WriteLine("if exist VisionProDependencies (rd /S /Q VisionProDependencies)"); // 输入命令
                                sw.WriteLine("mklink /D VisionProDependencies \"%VPRO_ROOT%\\bin\"");
                                Thread.Sleep(2000);
                                sw.WriteLine("CD " + zipPath + "\\mysql-9.0.1-winx64\\mysql-9.0.1-winx64");
                                sw.WriteLine("mysqld --initialize --console");
                                Thread.Sleep(10000);
                                bytes = new byte[102400];
                                sr.BaseStream.Read(bytes, 0, 102400);
                                InitialPassword = System.Text.Encoding.GetEncoding("GBK").GetString(bytes).Replace("\x00","");
                                InitialPassword = InitialPassword.Substring(InitialPassword.IndexOf("A temporary"), InitialPassword.Length - InitialPassword.IndexOf("A temporary"));
                                InitialPassword = InitialPassword.Substring(InitialPassword.IndexOf(": ") + 2, InitialPassword.IndexOf("\r\n") - InitialPassword.IndexOf(": ") - 2);
                                sw.WriteLine("mysqld --install mysql");
                                Thread.Sleep(5000);
                                sw.WriteLine("mysql -uroot -p");
                                Thread.Sleep(2000);
                                sw.WriteLine(InitialPassword);
                                Thread.Sleep(1000);
                                sw.WriteLine("ALTER USER 'root'@'localhost' IDENTIFIED BY '123456';");
                                Thread.Sleep(1000);
                                sw.WriteLine("CREATE DATABASE dws;");
                                Thread.Sleep(1000);
                                sw.WriteLine("USE dws;");
                                Thread.Sleep(1000);
                                sw.WriteLine(@"CREATE TABLE info(
                                    TimeStamp bigint unsigned NOT NULL,
                                    Expiry datetime NOT NULL,
                                    Barcode varchar(255),
                                    Length varchar(10),
                                    Width varchar(10),
                                    Height varchar(10),
                                    Weight varchar(10),
                                    Volume varchar(30),
                                    ImageFullName varchar(255),
                                    ReadStatus tinyint,
                                    InfoIntact tinyint,
                                    ParcelID varchar(10),
                                    INDEX _barcode(Barcode),
                                    PRIMARY KEY(TimeStamp)
                                );");
                                Thread.Sleep(500);
                                sw.WriteLine("GRANT EVENT ON *.* TO 'root'@'localhost';");
                                Thread.Sleep(500);
                                sw.WriteLine("SET GLOBAL event_scheduler = ON;");
                                Thread.Sleep(500);
                                sw.WriteLine(@"CREATE EVENT delete_expired_data
                                    ON SCHEDULE EVERY 1 DAY
                                    STARTS CURRENT_TIMESTAMP
                                    DO
                                    DELETE FROM info WHERE Expiry < CURRENT_TIMESTAMP;
                                ");
                                Thread.Sleep(1000);
                                sw.WriteLine("exit"); // 退出cmd
                            }
                        }
                    }
                    p.WaitForExit();
                }
                catch { }
                finally
                {
                    if (p != null)
                    {
                        p.Dispose();
                    }
                }
                if (!Directory.Exists(bottomcalib_path))
                {
                    try
                    {
                        Directory.CreateDirectory(bottomcalib_path);
                    }
                    catch
                    { }
                }
                if (!Directory.Exists(calib_path))
                {
                    try
                    {
                        Directory.CreateDirectory(calib_path);
                    }
                    catch { }
                }
                if (!Directory.Exists(ftplog_path))
                {
                    try
                    {
                        Directory.CreateDirectory(ftplog_path);
                    }
                    catch { }
                }
                if (!Directory.Exists(systemlog_path))
                {
                    try
                    {
                        Directory.CreateDirectory(systemlog_path);
                    }
                    catch { }
                }
                if (!Directory.Exists(keys_path))
                {
                    try
                    {
                        Directory.CreateDirectory(keys_path);
                    }
                    catch { }
                }
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
                try
                {
                    if (this.IsServiceExisted(serviceName))
                    { 
                        this.UninstallService(serviceName); 
                    }
                    this.InstallService(servicePath);
                    //MessageBox.Show("安装服务完成");
                    if (this.IsServiceExisted(serviceName))
                    { 
                        this.ServiceStart(serviceName);
                        //MessageBox.Show("启动服务完成");
                    }
                }
                catch (Exception ex) { }
            }
            catch (Exception ex) { }
        }
        */
        private void InstallerTest_AfterInstall(object sender, InstallEventArgs e)
        {
            
            try
            {
                string bottomcalib_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\bottomscan calibration";
                string calib_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\calib";
                string ftplog_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\FtpLog";
                string systemlog_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\SystemLog";
                string keys_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\keys";
                string servicePath = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\Image Handler Autorun.exe";
                string serviceName = "Image Handler Autorun";
                Process p = new Process();
                try
                {
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.Start();
                    using (StreamWriter sw = p.StandardInput)
                    {
                        if (sw.BaseStream.CanWrite)
                        {
                            //sw.WriteLine("CD C:\\Program Files\\Cognex\\存图软件标准版");
                            sw.WriteLine(this.Context.Parameters["targetdir"].Substring(0, 2));
                            sw.WriteLine("CD " + this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2));
                            sw.WriteLine("if exist VisionProDependencies (rd /S /Q VisionProDependencies)"); // 输入命令
                            sw.WriteLine("mklink /D VisionProDependencies \"%VPRO_ROOT%\\bin\"");
                            sw.WriteLine("exit"); // 退出cmd
                        }
                    }
                    p.WaitForExit();
                }
                catch { }
                finally
                {
                    if (p != null)
                    {
                        p.Dispose();
                    }
                }
                /*MessageBox.Show(this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\ConfigureMysql.exe");
                MessageBox.Show(this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\mysql-9.0.1-winx64.zip");
                MessageBox.Show(File.Exists(this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\ConfigureMysql.exe").ToString());
                MessageBox.Show(File.Exists(this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\mysql-9.0.1-winx64.zip").ToString());*/
                /*AllocConsole();
                Console.SetIn(new System.IO.StreamReader(Console.OpenStandardInput()));
                Console.SetOut(new System.IO.StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true });
                Console.SetError(new System.IO.StreamWriter(Console.OpenStandardError()) { AutoFlush = true });
                */
                /*
                Process pp = new Process();
                try
                {
                    pp.StartInfo.FileName = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\ConfigureMysql.exe";
                    pp.StartInfo.RedirectStandardInput = true;
                    pp.StartInfo.UseShellExecute = false;
                    pp.StartInfo.CreateNoWindow = false;
                    pp.Start();
                    pp.WaitForExit();
                }
                catch { }
                finally
                {
                    if (pp != null)
                    {
                        pp.Dispose();
                    }
                }
                */
                if (!Directory.Exists(bottomcalib_path))
                {
                    try
                    {
                        Directory.CreateDirectory(bottomcalib_path);
                    }
                    catch
                    { }
                }
                if (!Directory.Exists(calib_path))
                {
                    try
                    {
                        Directory.CreateDirectory(calib_path);
                    }
                    catch { }
                }
                if (!Directory.Exists(ftplog_path))
                {
                    try
                    {
                        Directory.CreateDirectory(ftplog_path);
                    }
                    catch { }
                }
                if (!Directory.Exists(systemlog_path))
                {
                    try
                    {
                        Directory.CreateDirectory(systemlog_path);
                    }
                    catch { }
                }
                if (!Directory.Exists(keys_path))
                {
                    try
                    {
                        Directory.CreateDirectory(keys_path);
                    }
                    catch { }
                }
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
                try
                {
                    if (this.IsServiceExisted(serviceName))
                    {
                        this.UninstallService(serviceName);
                    }
                    this.InstallService(servicePath);
                    //MessageBox.Show("安装服务完成");
                    if (this.IsServiceExisted(serviceName))
                    {
                        this.ServiceStart(serviceName);
                        //MessageBox.Show("启动服务完成");
                    }
                }
                catch (Exception ex) { }
            }
            catch (Exception ex) { }
        }
        private void InstallerTest_BeforeUninstall(object sender, InstallEventArgs e)
        {
            try
            {
                string startup_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + "\\存图助手标准版.lnk";
                string bottomcalib_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\bottomscan calibration";
                string calib_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\calib";
                string ftplog_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\FtpLog";
                string systemlog_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\SystemLog";
                string keys_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\keys";
                string servicePath = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + "\\Image Handler Autorun.exe";
                string serviceName = "Image Handler Autorun";
                //string dir = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2);
                if (System.IO.File.Exists(startup_path))
                {
                    try
                    {
                        System.IO.File.Delete(startup_path);
                    }
                    catch (Exception ex)
                    {

                    }
                }
                if (Directory.Exists(bottomcalib_path))
                {
                    try
                    {
                        Directory.Delete(bottomcalib_path, true);
                    }
                    catch
                    { }
                }
                if (Directory.Exists(calib_path))
                {
                    try
                    {
                        Directory.Delete(calib_path, true);
                    }
                    catch { }
                }
                if (Directory.Exists(ftplog_path))
                {
                    try
                    {
                        Directory.Delete(ftplog_path, true);
                    }
                    catch { }
                }
                if (Directory.Exists(systemlog_path))
                {
                    try
                    {
                        Directory.Delete(systemlog_path, true);
                    }
                    catch { }
                }
                if (Directory.Exists(keys_path))
                {
                    try
                    {
                        Directory.Delete(keys_path, true);
                    }
                    catch { }
                }
                try
                {
                    if (this.IsServiceExisted(serviceName))
                    {
                        this.ServiceStop(serviceName);
                        this.UninstallService(servicePath);
                        //MessageBox.Show("卸载服务完成");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("无法卸载服务");
                }
            }
            catch { }
        }
        private bool IsServiceExisted(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            foreach (ServiceController sc in services)
            {
                if (sc.ServiceName.ToLower() == serviceName.ToLower())
                {
                    return true;
                }
            }
            return false;
        }
        private void InstallService(string serviceFilePath)
        {
            AssemblyInstaller installer = null;
            try
            {
                installer = new AssemblyInstaller();
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                IDictionary savedState = new Hashtable();
                installer.Install(savedState);
                installer.Commit(savedState);
            }
            catch (Exception ex) { }
            finally
            {
                if (installer != null)
                {
                    installer.Dispose();
                }
            }
        }
        private void UninstallService(string serviceFilePath)
        {
            AssemblyInstaller installer = null;
            try
            {
                installer = new AssemblyInstaller();
                installer.UseNewContext = true;
                installer.Path = serviceFilePath;
                installer.Uninstall(null);
            }
            catch (Exception ex) { }
            finally
            {
                if (installer != null)
                {
                    installer.Dispose();
                }
            }

        }
        private void ServiceStart(string serviceName)
        {
            ServiceController control = null;
            try
            {
                control = new ServiceController(serviceName);
                if (control.Status == ServiceControllerStatus.Stopped)
                {
                    control.Start();
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (control != null)
                {
                    control.Dispose();
                }
            }
        }
        private void ServiceStop(string serviceName)
        {
            ServiceController control = null;
            try
            {
                control = new ServiceController(serviceName);
                if (control.Status == ServiceControllerStatus.Running)
                {
                    control.Stop();
                }
            }
            catch (Exception ex) { }
            finally
            {
                if (control != null)
                {
                    control.Dispose();
                }
            }
        }
        private void autorunwithshell()//加入快速启动栏
        {
            try
            {
                string startup_path = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup) + "\\存图助手标准版.lnk";
                string startup_dir = Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup);
                string target_dir = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"] .Length - 2);
                string target_path = this.Context.Parameters["targetdir"].Substring(0, this.Context.Parameters["targetdir"].Length - 2) + @"\存图助手标准版.exe";
                if (!System.IO.File.Exists(startup_path) && System.IO.File.Exists(target_path))
                {
                    try
                    {
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
                    {
                        
                    }
                    finally
                    {
                    }
                }
            }
            catch { }

        }
    }
    
}
