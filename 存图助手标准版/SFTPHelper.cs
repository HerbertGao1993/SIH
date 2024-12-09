using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;
using System.IO;
using System.Collections;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace 存图助手标准版
{
    public class SFTPHelper : IDisposable
    {
        #region 字段或属性
        private Object syncLock = new object();
        private sftpclient sftp;
        private string IP = "";
        private int Port;
        private string UserName = "";
        private string PWD = "";
        private List<AuthenticationMethod> methods;
        private bool _disposed = false;
        public bool Disposed
        {
            get
            {
                return _disposed;
            }
        }
        ConnectionInfo con;
        /// <summary>
        /// SFTP连接状态
        /// </summary>
        public bool Connected
        {
            get
            {
                return sftp.IsConnected;
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 构造
        /// </summary>
        /*
        public SFTPHelper(string IP, int Port, string UserName, string PWD)
        {
            this.IP = IP;
            this.Port = Port;
            this.UserName = UserName;
            this.PWD = PWD;
            //string key = Application.StartupPath + "\\keys\\privatekey.ppk";
            //PrivateKeyFile pk = new PrivateKeyFile(Application.StartupPath + "\\keys\\privatekey.txt", "cognex");
            //PrivateKeyFile[] pks = { pk };
            //sftp = new sftpclient(IP, Port, UserName, pks);
            sftp = new sftpclient(IP, Port, UserName, PWD);
            sftp.OperationTimeout = TimeSpan.FromMilliseconds(200);
            sftp.KeepAliveInterval = TimeSpan.FromMilliseconds(30000);
            sftp.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(5000);
            //sftp.ConnectionInfo.MaxSessions = 2;
            Connect();
        }
        public SFTPHelper(string IP, int Port, string UserName, PrivateKeyFile[] pks)
        {
            this.IP = IP;
            this.Port = Port;
            this.UserName = UserName;
            this.pks = pks;
            sftp = new sftpclient(IP, Port, UserName, pks);
            sftp.OperationTimeout = TimeSpan.FromMilliseconds(200);
            sftp.KeepAliveInterval = TimeSpan.FromMilliseconds(30000);
            sftp.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(5000);
            Connect();
        }
        */
        public SFTPHelper(string IP, string Port, string UserName, string PWD, string KeyPath, string PassPhrase)
        {
            try
            {
                this.IP = IP;
                this.Port = Convert.ToInt32(Port);
                this.UserName = UserName;
                this.PWD = PWD;
                List<AuthenticationMethod> methods = new List<AuthenticationMethod>();
                if (System.IO.File.Exists(KeyPath))
                {
                    PrivateKeyFile[] pks = new PrivateKeyFile[] { new PrivateKeyFile(KeyPath, PassPhrase) };
                    methods.Add(new PrivateKeyAuthenticationMethod(UserName, pks));
                }
                methods.Add(new PasswordAuthenticationMethod(UserName, PWD));
                this.con = new ConnectionInfo(IP, this.Port, UserName, methods.ToArray());
                sftp = new sftpclient(con);
                sftp.OperationTimeout = TimeSpan.FromMilliseconds(3000);
                sftp.KeepAliveInterval = TimeSpan.FromMilliseconds(30000);
                sftp.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(5000);
            }
            catch { }
        }
        ~SFTPHelper()
        {
            Dispose(false);
        }
        #endregion


        #region 连接SFTP
        /// <summary>
        /// 连接SFTP
        /// </summary>
        /// <returns>true成功</returns>
        public bool Connect()
        {
            bool returnflag = false;
            lock (syncLock)
            {
                try
                {
                    if (!_disposed)
                    {
                        if (sftp == null)
                        {
                            sftp = new sftpclient(con);
                            sftp.OperationTimeout = TimeSpan.FromMilliseconds(3000);
                            sftp.KeepAliveInterval = TimeSpan.FromMilliseconds(30000);
                            sftp.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(5000);
                        }
                        else if (sftp.isDisposed)
                        {
                            sftp = new sftpclient(con);
                            sftp.OperationTimeout = TimeSpan.FromMilliseconds(3000);
                            sftp.KeepAliveInterval = TimeSpan.FromMilliseconds(30000);
                            sftp.ConnectionInfo.Timeout = TimeSpan.FromMilliseconds(5000);
                        }
                        if (!Connected)
                        {
                            sftp.Connect();
                        }
                        returnflag = true;
                    }
                    else
                    {
                        returnflag = false;
                        throw new ObjectDisposedException("Client is Disposed");
                    }
                }
                catch (Exception ex)
                {
                    string mes = ex.Message;
                    returnflag = false;
                    // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("连接SFTP失败，原因：{0}", ex.Message));
                    //throw new Exception(string.Format("连接SFTP失败，原因：{0}", ex.Message));
                }
            }
            return returnflag;
        }
        #endregion

        #region 断开SFTP
        /// <summary>
        /// 断开SFTP
        /// </summary> 
        public void Disconnect()
        {
            try
            {
                if (sftp != null)
                {
                    if (!sftp.isDisposed)
                    {
                        if (Connected)
                        {
                            sftp.Disconnect();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // TxtLog.WriteTxt(CommonMethod.GetProgramName(), string.Format("断开SFTP失败，原因：{0}", ex.Message));
                //throw new Exception(string.Format("断开SFTP失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region SFTP上传文件
        /// <summary>
        /// SFTP上传文件
        /// </summary>
        /// <param name="localPath">本地路径</param>
        /// <param name="remotePath">远程路径</param>
        public bool Put(string localPath, string remotePath, string fileName)
        {
            bool returnflag = false;
            if (Connect())
            {
                lock (syncLock)
                {
                    FileStream file = null;
                    try
                    {
                        if (!_disposed)
                        {
                            //判断路径是否存在
                            bool flag = sftp.Exists(remotePath);
                            if (!flag)
                            {
                                CreateDirectory(remotePath);
                            }
                            file = File.OpenRead(localPath);
                            sftp.UploadFile(file, remotePath + fileName);
                            returnflag = true;
                        }
                        else
                        {
                            returnflag = false;
                            throw new ObjectDisposedException("Client is Disposed");
                        }
                    }
                    catch (Exception ex)
                    {
                        returnflag = false;
                        string mes = ex.Message;
                    }
                    finally
                    {
                        if (file != null)
                        {
                            file.Dispose();
                        }
                    }
                }
            }
            else
            {
                returnflag = false;
            }
            return returnflag;
        }
        #endregion

        #region SFTP获取文件
        /// <summary>
        /// SFTP获取文件
        /// </summary>
        /// <param name="remotePath">远程路径</param>
        /// <param name="localPath">本地路径</param>
        public void Get(string remotePath, string localPath)
        {
            if (Connect())
            {
                lock (syncLock)
                {
                    try
                    {
                        if (!_disposed)
                        {
                            var byt = sftp.ReadAllBytes(remotePath);
                            //Disconnect();
                            File.WriteAllBytes(localPath, byt);
                        }
                        else
                        {
                            throw new ObjectDisposedException("Client is Disposed");
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }
        #endregion

        #region 获取SFTP文件列表
        /// <summary>
        /// 获取SFTP文件列表
        /// </summary>
        /// <param name="remotePath">远程目录</param>
        /// <param name="fileSuffix">文件后缀</param>
        /// <returns></returns>
        public ArrayList GetFileList(string remotePath, string fileSuffix)
        {
            var objList = new ArrayList();
            if (Connect())
            {
                lock (syncLock)
                {
                    try
                    {
                        if (!_disposed)
                        {
                            var files = sftp.ListDirectory(remotePath);
                            //Disconnect();
                            foreach (var file in files)
                            {
                                string name = file.Name;
                                if (name.Length > (fileSuffix.Length + 1) && fileSuffix == name.Substring(name.Length - fileSuffix.Length))
                                {
                                    objList.Add(name);
                                }
                            }
                        }
                        else
                        {
                            throw new ObjectDisposedException("Client is Disposed");
                        }
                    }
                    catch (Exception ex) { }
                }
            }
            return objList;
        }
        #endregion

        #region 移动SFTP文件
        /// <summary>
        /// 移动SFTP文件
        /// </summary>
        /// <param name="oldRemotePath">旧远程路径</param>
        /// <param name="newRemotePath">新远程路径</param>
        public void Move(string oldRemotePath, string newRemotePath)
        {
            if (Connect())
            {
                lock (syncLock)
                {
                    try
                    {
                        if (!_disposed)
                        {
                            sftp.RenameFile(oldRemotePath, newRemotePath);
                        }
                        else
                        {
                            throw new ObjectDisposedException("Client is Disposed");
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }
        #endregion

        #region 删除SFTP文件
        public void Delete(string remoteFile)
        {
            if (Connect())
            {
                lock (syncLock)
                {
                    try
                    {
                        if (!_disposed)
                        {
                            sftp.Delete(remoteFile);
                        }
                        else
                        {
                            throw new ObjectDisposedException("Client is Disposed");
                        }
                    }
                    catch (Exception ex) { }
                }
            }
        }
        #endregion

        #region 创建目录
        /// <summary>
        /// 循环创建目录
        /// </summary>
        /// <param name="remotePath">远程目录</param>
        private void CreateDirectory(string remotePath)
        {
            try
            {
                string[] paths = remotePath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                string curPath = "/";
                for (int i = 0; i < paths.Length; i++)
                {
                    curPath += paths[i];
                    bool flag = sftp.Exists(curPath);
                    if (!flag)
                    {
                        sftp.CreateDirectory(curPath);
                    }
                    if (i < paths.Length - 1)
                    {
                        curPath += "/";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("创建目录失败，原因：{0}", ex.Message));
            }
        }
        #endregion

        #region 释放资源
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            lock (syncLock)
            {
                if (_disposed)
                {
                    return;
                }
                if (disposing)
                {

                }
                if (sftp != null)
                {
                    sftp.Dispose();
                }
                _disposed = true;
            }
        }
        #endregion
    }
}
