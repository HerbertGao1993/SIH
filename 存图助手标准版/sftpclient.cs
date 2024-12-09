using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace 存图助手标准版
{
    class sftpclient : SftpClient
    {

        public bool isDisposed
        {
            get
            {
                try
                {
                    CheckDisposed();
                    return false;
                }
                catch { return true; }
            }
        }
        public sftpclient(string IP, int Port, string UserName, PrivateKeyFile[] pks) : base(IP, Port, UserName, pks)
        {

        }
        public sftpclient(string IP, int Port, string UserName, string PWD) : base(IP, Port, UserName, PWD)
        {

        }
        public sftpclient(ConnectionInfo con) : base(con)
        {

        }
    }
}
