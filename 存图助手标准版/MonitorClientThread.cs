using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SocketLib;
using System.Threading;
using System.Drawing;
using System.Windows.Forms;

namespace 存图助手标准版
{
    class MonitorClientThread
    {
        public delegate void SetStatusDelegate(System.Windows.Forms.Label label,Color color);
        public void setLabelGray(System.Windows.Forms.Label label, Color color)
        {
            label.BackColor = color;
        }
        private string _handler = "";
        public MonitorClientThread(string handler)
        {
            this._handler = handler;
        }
        public void Monitor()
        {
            while (true)
            {
                try
                {
                    System.Windows.Forms.Label label = null;
                    SocketLib.TcpClientBase client = null;
                    switch (_handler)
                    {
                        case "Bottom11Handler":
                            client = MainForm.Bottom11Handler;
                            break;
                        case "Bottom12Handler":
                            client = MainForm.Bottom12Handler;
                            break;
                        case "Bottom13Handler":
                            client = MainForm.Bottom13Handler;
                            break;
                        case "Bottom14Handler":
                            client = MainForm.Bottom14Handler;
                            break;
                        case "Bottom15Handler":
                            client = MainForm.Bottom15Handler;
                            break;
                        case "Bottom21Handler":
                            client = MainForm.Bottom21Handler;
                            break;
                        case "Bottom22Handler":
                            client = MainForm.Bottom22Handler;
                            break;
                        case "Bottom23Handler":
                            client = MainForm.Bottom23Handler;
                            break;
                        case "Bottom24Handler":
                            client = MainForm.Bottom24Handler;
                            break;
                        case "Bottom25Handler":
                            client = MainForm.Bottom25Handler;
                            break;
                        case "DWS1Handler":
                            client = MainForm.DWS1Handler;
                            break;
                        default:
                            client = null;
                            break;
                    }
                    if (client != null)
                    {
                        if (!client.Disposed && !client.isalive)
                        {
                            MainForm.AddLog("[" + DateTime.Now.AddMilliseconds(0).ToString("yyyy-MM-dd_HH:mm:ss:fff") + "]" + _handler + " Attempting Connection", MainForm.LogContentArrayComm);
                            client.Start();
                        }
                        else if(client.Disposed)
                        {
                            MainForm.AddLog("[" + DateTime.Now.AddMilliseconds(0).ToString("yyyy-MM-dd_HH:mm:ss:fff") + "]" + _handler + "Not Attempting Connection because disposed", MainForm.LogContentArrayComm);
                        }
                    }
                    Thread.Sleep(1000);
                }
                catch (Exception ex) { MainForm.AddLog("[" + DateTime.Now.AddMilliseconds(0).ToString("yyyy-MM-dd_HH:mm:ss:fff") + "]" +  _handler + "failed Attempting Connection StackTrace: " + ex.StackTrace, MainForm.LogContentArrayComm); }
            }
        }
    }
}
