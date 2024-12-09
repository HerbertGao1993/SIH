using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.Collections;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace ConfigureMysql
{
    class Program
    {
        private static string connStr = "server=localhost;user=root;database=dws;port=3306;password=123456";
        async static Task<string> read(StreamReader sr)
        {
            return await sr.ReadToEndAsync();
        }
        static private void appendPath(string path)
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
        static void Main(string[] args)
        {
            //Console.Read();
            FileStream fs0 = null;
            StreamWriter sr0 = null;
            try
            {
                fs0 = new FileStream("C:\\log.txt", FileMode.Append, FileAccess.Write);
                sr0 = new StreamWriter(fs0);
                sr0.WriteLine("start");
                string path = Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf("\\")) + "\\mysql-9.0.1-winx64.zip";
                string zipPath = Process.GetCurrentProcess().MainModule.FileName.Substring(0, Process.GetCurrentProcess().MainModule.FileName.LastIndexOf("\\")).Substring(0, 2);
                sr0.WriteLine("zipPath: " + zipPath);
                sr0.WriteLine("Path: " + path);
                sr0.WriteLine("zip exist: " + File.Exists(path));
                if (!Directory.Exists(zipPath + "\\mysql-9.0.1-winx64"))
                {
                    System.IO.Compression.ZipFile.ExtractToDirectory(path, zipPath + "\\");
                    if (Directory.Exists(zipPath + "\\mysql-9.0.1-winx64\\data"))
                    {
                        Directory.Delete(zipPath + "\\mysql-9.0.1-winx64\\data", true);
                    }
                    Directory.CreateDirectory(zipPath + "\\mysql-9.0.1-winx64\\data");
                    FileStream fs = null;
                    StreamWriter sw = null;
                    try
                    {
                        fs = new FileStream(zipPath + "\\mysql-9.0.1-winx64\\my.ini", FileMode.Append);
                        sw = new StreamWriter(fs);
                        sw.WriteLine("[mysqld]");
                        sw.WriteLine("# 设置3306端口");
                        sw.WriteLine("port=3306");
                        sw.WriteLine("# 设置mysql的安装目录   ----------Mysql解压之后的文件路径-------------");
                        sw.WriteLine("basedir=" + zipPath + "\\mysql-9.0.1-winx64");
                        sw.WriteLine("# 设置mysql数据库的数据的存放目录  ---------文件夹自行创建");
                        sw.WriteLine("datadir=" + zipPath + "\\mysql-9.0.1-winx64\\data");
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
                sr0.WriteLine("zip complete");
                try
                {
                    System.Environment.SetEnvironmentVariable("MYSQL_HOME", zipPath + "\\mysql-9.0.1-winx64", EnvironmentVariableTarget.Machine);
                    appendPath(zipPath + "\\mysql-9.0.1-winx64\\bin");
                    sr0.WriteLine("env complete");
                }
                catch { }
                Process p = new Process();
                byte[] bytes = new byte[102400];
                string InitialPassword = "";
                string str = "";
                try
                {
                    p.StartInfo.FileName = "cmd.exe";
                    p.StartInfo.RedirectStandardInput = true;
                    p.StartInfo.RedirectStandardOutput = true;
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = false;
                    p.Start();
                    using (StreamWriter sw = p.StandardInput)
                    {
                        using (StreamReader sr = p.StandardOutput)
                        {
                            //bytes = new byte[1024000];
                            //sr.BaseStream.Read(bytes, 0, 1024000);
                            //str = System.Text.Encoding.GetEncoding("GBK").GetString(bytes).Replace("\x00", "");
                            //Console.WriteLine(str);
                            if (sw.BaseStream.CanWrite)
                            {
                                sw.WriteLine(zipPath);
                                sw.WriteLine("CD " + zipPath + "\\mysql-9.0.1-winx64\\bin");
                                sw.WriteLine("mysqld --initialize --console");
                                Thread.Sleep(10000);
                                Console.WriteLine("InitialPassword: ");
                                InitialPassword = Console.ReadLine();
                                sw.WriteLine("mysqld --install mysql");
                                Thread.Sleep(6000);
                                bytes = new byte[1024000];
                                sr.BaseStream.Read(bytes, 0, 1024000);
                                str = System.Text.Encoding.GetEncoding("GBK").GetString(bytes).Replace("\x00", "");
                                Console.WriteLine(str);
                                sw.WriteLine("net start mysql");
                                Thread.Sleep(6000);
                                bytes = new byte[1024000];
                                sr.BaseStream.Read(bytes, 0, 1024000);
                                str = System.Text.Encoding.GetEncoding("GBK").GetString(bytes).Replace("\x00", "");
                                Console.WriteLine(str);
                                sw.WriteLine("mysqladmin -uroot -p\"" + InitialPassword + "\"" + " password 123456 create dws");
                                Thread.Sleep(6000);
                                bytes = new byte[1024000];
                                sr.BaseStream.Read(bytes, 0, 1024000);
                                str = System.Text.Encoding.GetEncoding("GBK").GetString(bytes).Replace("\x00", "");
                                Console.WriteLine(str);
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
                using (MySqlConnection conn = new MySqlConnection(connStr))
                {
                    try
                    {
                        conn.Open();

                        string query = "CREATE TABLE IF NOT EXISTS info (" +
                                       "TimeStamp bigint unsigned NOT NULL," +
                                       "Expiry datetime NOT NULL," +
                                       "Barcode varchar(255)," +
                                       "Length varchar(10)," +
                                       "Width varchar(10)," +
                                       "Height varchar(10)," +
                                       "Weight varchar(10)," +
                                       "Volume varchar(30)," +
                                       "ImageFullName varchar(255)," +
                                       "ReadStatus tinyint," +
                                       "InfoIntact tinyint," +
                                       "ParcelID varchar(10)," +
                                       "INDEX _barcode(Barcode)," +
                                       "Destination varchar(10)," +
                                       "PRIMARY KEY(TimeStamp)" +
                                       ");";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        Thread.Sleep(1000);
                        query = @"GRANT EVENT ON *.* TO 'root'@'localhost';";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                        Thread.Sleep(1000);
                        query = @"SET GLOBAL event_scheduler = ON;";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        Thread.Sleep(1000);
                        query = @"CREATE EVENT IF NOT EXISTS delete_expired_data
                        ON SCHEDULE EVERY 1 DAY
                        STARTS CURRENT_TIMESTAMP
                        DO
                        DELETE FROM info WHERE Expiry < CURRENT_TIMESTAMP;";

                        using (MySqlCommand cmd = new MySqlCommand(query, conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                    }
                    catch (MySqlException ex)
                    {
                        Console.WriteLine("Error: " + ex.Message);
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.Message + "\r\n" + ex.StackTrace); }
            finally
            {
                if (sr0 != null)
                {
                    sr0.Dispose();
                }
                if (fs0 != null)
                {
                    fs0.Dispose();
                }

            }
            //Console.ReadKey();
        }
    }
}
