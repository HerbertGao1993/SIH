using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Text.RegularExpressions;

namespace 存图助手标准版
{
    public partial class InductorSetting : Form
    {
        private string IPPattern = @"^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$";
        private string PortPattern = @"^\d{1,5}$";
        public InductorSetting()
        {
            InitializeComponent();
            MonitorTimer.Enabled = true;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void InductorSetting_VisibleChanged(object sender, EventArgs e)
        {
            if (Visible)
            {
                if (ConfigurationManager.AppSettings["LocalServerEnable"] == "true")
                {
                    LocalServerOnBox.Checked = true;
                }
                else if (ConfigurationManager.AppSettings["LocalServerEnable"] == "false")
                {
                    LocalServerOnBox.Checked = false;
                }
                if (ConfigurationManager.AppSettings["Induct11Enable"] == "true")
                {
                    Ind11OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct11Enable"] == "false")
                {
                    Ind11OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct12Enable"] == "true")
                {
                    Ind12OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct12Enable"] == "false")
                {
                    Ind12OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct13Enable"] == "true")
                {
                    Ind13OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct13Enable"] == "false")
                {
                    Ind13OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct14Enable"] == "true")
                {
                    Ind14OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct14Enable"] == "false")
                {
                    Ind14OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct15Enable"] == "true")
                {
                    Ind15OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct15Enable"] == "false")
                {
                    Ind15OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct21Enable"] == "true")
                {
                    Ind21OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct21Enable"] == "false")
                {
                    Ind21OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct22Enable"] == "true")
                {
                    Ind22OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct22Enable"] == "false")
                {
                    Ind22OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct23Enable"] == "true")
                {
                    Ind23OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct23Enable"] == "false")
                {
                    Ind23OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct24Enable"] == "true")
                {
                    Ind24OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct24Enable"] == "false")
                {
                    Ind24OnBox.Checked = false;
                    
                }
                if (ConfigurationManager.AppSettings["Induct25Enable"] == "true")
                {
                    Ind25OnBox.Checked = true;
                    
                }
                else if (ConfigurationManager.AppSettings["Induct25Enable"] == "false")
                {
                    Ind25OnBox.Checked = false;
                    
                }
                if (Regex.IsMatch(ConfigurationManager.AppSettings["BindingTimeout"], MainForm.DaysPattern))
                {
                    BindingTimeoutBox.Text = ConfigurationManager.AppSettings["BindingTimeout"];
                }
                else
                {
                    BindingTimeoutBox.Text = MainForm.DefaultBindingTimeoutText;
                }
                LocalIPBox.Text = ConfigurationManager.AppSettings["LocalIP"];//每次开机访问配置文件，获取与供包台plc通讯的本地服务器IP地址
                LocalPortBox.Text = ConfigurationManager.AppSettings["LocalPort"];//每次开机访问配置文件，获取与供包台plc通讯的本地服务器端口
                DWS1IPBox.Text = ConfigurationManager.AppSettings["DWS1IP"];//每次开机访问配置文件，获取DWS1IP
                ExtCaptionIPBox.Text = ConfigurationManager.AppSettings["ExtCaptionIP"];//每次开机访问配置文件，获取ExtCaptionIP
                Plc11IPBox.Text = ConfigurationManager.AppSettings["Plc11IP"];//每次开机访问配置文件，获取供包台plcIP
                Plc12IPBox.Text = ConfigurationManager.AppSettings["Plc12IP"];
                Plc13IPBox.Text = ConfigurationManager.AppSettings["Plc13IP"];
                Plc14IPBox.Text = ConfigurationManager.AppSettings["Plc14IP"];
                Plc15IPBox.Text = ConfigurationManager.AppSettings["Plc15IP"];
                Plc21IPBox.Text = ConfigurationManager.AppSettings["Plc21IP"];
                Plc22IPBox.Text = ConfigurationManager.AppSettings["Plc22IP"];
                Plc23IPBox.Text = ConfigurationManager.AppSettings["Plc23IP"];
                Plc24IPBox.Text = ConfigurationManager.AppSettings["Plc24IP"];
                Plc25IPBox.Text = ConfigurationManager.AppSettings["Plc25IP"];
                DWS1PortBox.Text = ConfigurationManager.AppSettings["DWS1Port"];//每次开机访问配置文件，获取DWS1端口
                Bot11PortBox.Text = ConfigurationManager.AppSettings["Bottom11Port"];//每次开机访问配置文件，获取供包台底扫端口
                Bot12PortBox.Text = ConfigurationManager.AppSettings["Bottom12Port"];
                Bot13PortBox.Text = ConfigurationManager.AppSettings["Bottom13Port"];
                Bot14PortBox.Text = ConfigurationManager.AppSettings["Bottom14Port"];
                Bot15PortBox.Text = ConfigurationManager.AppSettings["Bottom15Port"];
                Bot21PortBox.Text = ConfigurationManager.AppSettings["Bottom21Port"];
                Bot22PortBox.Text = ConfigurationManager.AppSettings["Bottom22Port"];
                Bot23PortBox.Text = ConfigurationManager.AppSettings["Bottom23Port"];
                Bot24PortBox.Text = ConfigurationManager.AppSettings["Bottom24Port"];
                Bot25PortBox.Text = ConfigurationManager.AppSettings["Bottom25Port"];
                Bot11IPBox.Text = ConfigurationManager.AppSettings["Bottom11IP"];//每次开机访问配置文件，获取供包台底扫IP地址
                Bot12IPBox.Text = ConfigurationManager.AppSettings["Bottom12IP"];
                Bot13IPBox.Text = ConfigurationManager.AppSettings["Bottom13IP"];
                Bot14IPBox.Text = ConfigurationManager.AppSettings["Bottom14IP"];
                Bot15IPBox.Text = ConfigurationManager.AppSettings["Bottom15IP"];
                Bot21IPBox.Text = ConfigurationManager.AppSettings["Bottom21IP"];
                Bot22IPBox.Text = ConfigurationManager.AppSettings["Bottom22IP"];
                Bot23IPBox.Text = ConfigurationManager.AppSettings["Bottom23IP"];
                Bot24IPBox.Text = ConfigurationManager.AppSettings["Bottom24IP"];
                Bot25IPBox.Text = ConfigurationManager.AppSettings["Bottom25IP"];
                if (MainForm.running)//运行中不可改变设置
                {
                    LocalServerOnBox.Enabled = false;
                    Ind11OnBox.Enabled = false;
                    Ind12OnBox.Enabled = false;
                    Ind13OnBox.Enabled = false;
                    Ind14OnBox.Enabled = false;
                    Ind15OnBox.Enabled = false;
                    Ind21OnBox.Enabled = false;
                    Ind22OnBox.Enabled = false;
                    Ind23OnBox.Enabled = false;
                    Ind24OnBox.Enabled = false;
                    Ind25OnBox.Enabled = false;
                    Bot11IPBox.Enabled = false;
                    Bot12IPBox.Enabled = false;
                    Bot13IPBox.Enabled = false;
                    Bot14IPBox.Enabled = false;
                    Bot15IPBox.Enabled = false;
                    Bot21IPBox.Enabled = false;
                    Bot22IPBox.Enabled = false;
                    Bot23IPBox.Enabled = false;
                    Bot24IPBox.Enabled = false;
                    Bot25IPBox.Enabled = false;
                    Bot11PortBox.Enabled = false;
                    Bot12PortBox.Enabled = false;
                    Bot13PortBox.Enabled = false;
                    Bot14PortBox.Enabled = false;
                    Bot15PortBox.Enabled = false;
                    Bot21PortBox.Enabled = false;
                    Bot22PortBox.Enabled = false;
                    Bot23PortBox.Enabled = false;
                    Bot24PortBox.Enabled = false;
                    Bot25PortBox.Enabled = false;
                    DWS1IPBox.Enabled = false;
                    DWS1PortBox.Enabled = false;
                    ExtCaptionIPBox.Enabled = false;
                    Plc11IPBox.Enabled = false;
                    Plc12IPBox.Enabled = false;
                    Plc13IPBox.Enabled = false;
                    Plc14IPBox.Enabled = false;
                    Plc15IPBox.Enabled = false;
                    Plc21IPBox.Enabled = false;
                    Plc22IPBox.Enabled = false;
                    Plc23IPBox.Enabled = false;
                    Plc24IPBox.Enabled = false;
                    Plc25IPBox.Enabled = false;
                    LocalIPBox.Enabled = false;
                    LocalPortBox.Enabled = false;
                    BindingTimeoutBox.Enabled = false;
                    BtnSave.Enabled = false;
                }
                else
                {
                    LocalServerOnBox.Enabled = true;
                    Ind11OnBox.Enabled = true;
                    Ind12OnBox.Enabled = true;
                    Ind13OnBox.Enabled = true;
                    Ind14OnBox.Enabled = true;
                    Ind15OnBox.Enabled = true;
                    Ind21OnBox.Enabled = true;
                    Ind22OnBox.Enabled = true;
                    Ind23OnBox.Enabled = true;
                    Ind24OnBox.Enabled = true;
                    Ind25OnBox.Enabled = true;
                    Bot11IPBox.Enabled = true;
                    Bot12IPBox.Enabled = true;
                    Bot13IPBox.Enabled = true;
                    Bot14IPBox.Enabled = true;
                    Bot15IPBox.Enabled = true;
                    Bot21IPBox.Enabled = true;
                    Bot22IPBox.Enabled = true;
                    Bot23IPBox.Enabled = true;
                    Bot24IPBox.Enabled = true;
                    Bot25IPBox.Enabled = true;
                    Bot11PortBox.Enabled = true;
                    Bot12PortBox.Enabled = true;
                    Bot13PortBox.Enabled = true;
                    Bot14PortBox.Enabled = true;
                    Bot15PortBox.Enabled = true;
                    Bot21PortBox.Enabled = true;
                    Bot22PortBox.Enabled = true;
                    Bot23PortBox.Enabled = true;
                    Bot24PortBox.Enabled = true;
                    Bot25PortBox.Enabled = true;
                    DWS1IPBox.Enabled = true;
                    DWS1PortBox.Enabled = true;
                    ExtCaptionIPBox.Enabled = true;
                    Plc11IPBox.Enabled = true;
                    Plc12IPBox.Enabled = true;
                    Plc13IPBox.Enabled = true;
                    Plc14IPBox.Enabled = true;
                    Plc15IPBox.Enabled = true;
                    Plc21IPBox.Enabled = true;
                    Plc22IPBox.Enabled = true;
                    Plc23IPBox.Enabled = true;
                    Plc24IPBox.Enabled = true;
                    Plc25IPBox.Enabled = true;
                    LocalIPBox.Enabled = true;
                    LocalPortBox.Enabled = true;
                    BindingTimeoutBox.Enabled = true;
                    BtnSave.Enabled = true;
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            
            List<string> ips = new List<string>();
            int OnCount = 0;
            ips.Add(ExtCaptionIPBox.Text);
            OnCount += 1;
            ips.Add(DWS1IPBox.Text);
            OnCount += 1;
            if (Ind11OnBox.Checked)
            {
                ips.Add(Plc11IPBox.Text);
                ips.Add(Bot11IPBox.Text);
                OnCount += 2;
            }
            if (Ind12OnBox.Checked)
            {
                ips.Add(Plc12IPBox.Text);
                ips.Add(Bot12IPBox.Text);
                OnCount += 2;
            }
            if (Ind13OnBox.Checked)
            {
                ips.Add(Plc13IPBox.Text);
                ips.Add(Bot13IPBox.Text);
                OnCount += 2;
            }
            if (Ind14OnBox.Checked)
            {
                ips.Add(Plc14IPBox.Text);
                ips.Add(Bot14IPBox.Text);
                OnCount += 2;
            }
            if (Ind15OnBox.Checked)
            {
                ips.Add(Plc15IPBox.Text);
                ips.Add(Bot15IPBox.Text);
                OnCount += 2;
            }
            if (Ind21OnBox.Checked)
            {
                ips.Add(Plc21IPBox.Text);
                ips.Add(Bot21IPBox.Text);
                OnCount += 2;
            }
            if (Ind22OnBox.Checked)
            {
                ips.Add(Plc22IPBox.Text);
                ips.Add(Bot22IPBox.Text);
                OnCount += 2;
            }
            if (Ind23OnBox.Checked)
            {
                ips.Add(Plc23IPBox.Text);
                ips.Add(Bot23IPBox.Text);
                OnCount += 2;
            }
            if (Ind24OnBox.Checked)
            {
                ips.Add(Plc24IPBox.Text);
                ips.Add(Bot24IPBox.Text);
                OnCount += 2;
            }
            if (Ind25OnBox.Checked)
            {
                ips.Add(Plc25IPBox.Text);
                ips.Add(Bot25IPBox.Text);
                OnCount += 2;
            }
            if (LocalServerOnBox.Checked)
            {
                ips.Add(LocalIPBox.Text);
                OnCount += 1;
            }
            BindingTimeoutBox.Text = Regex.IsMatch(BindingTimeoutBox.Text, MainForm.DaysPattern) ? BindingTimeoutBox.Text : MainForm.DefaultBindingTimeoutText;
            bool flag1 = ips.Distinct().ToList().Count == OnCount;
            bool flag2 = (Regex.IsMatch(Bot11IPBox.Text, IPPattern) && Regex.IsMatch(Bot11PortBox.Text, PortPattern) && Regex.IsMatch(Plc11IPBox.Text, IPPattern)) || !Ind11OnBox.Checked;
            bool flag3 = (Regex.IsMatch(Bot12IPBox.Text, IPPattern) && Regex.IsMatch(Bot12PortBox.Text, PortPattern) && Regex.IsMatch(Plc12IPBox.Text, IPPattern)) || !Ind12OnBox.Checked;
            bool flag4 = (Regex.IsMatch(Bot13IPBox.Text, IPPattern) && Regex.IsMatch(Bot13PortBox.Text, PortPattern) && Regex.IsMatch(Plc13IPBox.Text, IPPattern)) || !Ind13OnBox.Checked;
            bool flag5 = (Regex.IsMatch(Bot14IPBox.Text, IPPattern) && Regex.IsMatch(Bot14PortBox.Text, PortPattern) && Regex.IsMatch(Plc14IPBox.Text, IPPattern)) || !Ind14OnBox.Checked;
            bool flag6 = (Regex.IsMatch(Bot15IPBox.Text, IPPattern) && Regex.IsMatch(Bot15PortBox.Text, PortPattern) && Regex.IsMatch(Plc15IPBox.Text, IPPattern)) || !Ind15OnBox.Checked;
            bool flag7 = (Regex.IsMatch(Bot21IPBox.Text, IPPattern) && Regex.IsMatch(Bot21PortBox.Text, PortPattern) && Regex.IsMatch(Plc21IPBox.Text, IPPattern)) || !Ind21OnBox.Checked;
            bool flag8 = (Regex.IsMatch(Bot22IPBox.Text, IPPattern) && Regex.IsMatch(Bot22PortBox.Text, PortPattern) && Regex.IsMatch(Plc22IPBox.Text, IPPattern)) || !Ind22OnBox.Checked;
            bool flag9 = (Regex.IsMatch(Bot23IPBox.Text, IPPattern) && Regex.IsMatch(Bot23PortBox.Text, PortPattern) && Regex.IsMatch(Plc23IPBox.Text, IPPattern)) || !Ind23OnBox.Checked;
            bool flag10 = (Regex.IsMatch(Bot24IPBox.Text, IPPattern) && Regex.IsMatch(Bot24PortBox.Text, PortPattern) && Regex.IsMatch(Plc24IPBox.Text, IPPattern)) || !Ind24OnBox.Checked;
            bool flag11 = (Regex.IsMatch(Bot25IPBox.Text, IPPattern) && Regex.IsMatch(Bot25PortBox.Text, PortPattern) && Regex.IsMatch(Plc25IPBox.Text, IPPattern)) || !Ind25OnBox.Checked;
            bool flag12 = (Regex.IsMatch(LocalIPBox.Text, IPPattern) && Regex.IsMatch(LocalPortBox.Text, PortPattern)) || !LocalServerOnBox.Checked;
            bool flag13 = Regex.IsMatch(ExtCaptionIPBox.Text, IPPattern);
            bool flag14 = Regex.IsMatch(DWS1IPBox.Text, IPPattern) && Regex.IsMatch(DWS1PortBox.Text, PortPattern);
            if (flag1 && flag2 && flag3 && flag4 && flag5 && flag6 && flag7 && flag8 && flag9 && flag10 && flag11 && flag12 && flag13 && flag14)
            {
                lock (MainForm.OpenConfigLock)
                {
                    try
                    {
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                        config.AppSettings.Settings["BindingTimeout"].Value = BindingTimeoutBox.Text;
                        MainForm.InductorBindingTimeout = Convert.ToInt32(BindingTimeoutBox.Text);
                        if (LocalServerOnBox.Checked)
                        {
                            config.AppSettings.Settings["LocalServerEnable"].Value = "true";
                            MainForm.LocalServerEnable = true;
                            config.AppSettings.Settings["LocalIP"].Value = LocalIPBox.Text;
                            MainForm.LocalIP = LocalIPBox.Text;
                            config.AppSettings.Settings["LocalPort"].Value = LocalPortBox.Text;
                            MainForm.LocalPort = LocalPortBox.Text;
                            if (MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.LocalServerHandler.LocalIP != MainForm.LocalIP || MainForm.LocalServerHandler.LocalPort != Convert.ToInt32(MainForm.LocalPort) || MainForm.LocalServerHandler.Disposed)
                                {
                                    MainForm.LocalServerHandler.Dispose();
                                    MainForm.LocalServerHandler = new SocketLib.TcpServerBase(MainForm.LocalIP, Convert.ToInt32(MainForm.LocalPort));
                                    MainForm.LocalServerHandler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.LocalServerHandler.logmsg += MainForm.logmsg;
                                    MainForm.LocalServerHandler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.LocalServerHandler = new SocketLib.TcpServerBase(MainForm.LocalIP, Convert.ToInt32(MainForm.LocalPort));
                                MainForm.LocalServerHandler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.LocalServerHandler.logmsg += MainForm.logmsg;
                                MainForm.LocalServerHandler.OnConnect += MainForm.OnConnect;
                            }
                        }
                        else
                        {
                            config.AppSettings.Settings["LocalServerEnable"].Value = "false";
                            MainForm.LocalServerEnable = false;
                            LocalIPBox.Text = "";
                            config.AppSettings.Settings["LocalIP"].Value = LocalIPBox.Text;
                            MainForm.LocalIP = LocalIPBox.Text;
                            LocalPortBox.Text = "";
                            config.AppSettings.Settings["LocalPort"].Value = LocalPortBox.Text;
                            MainForm.LocalPort = LocalPortBox.Text;
                            if (MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Dispose();
                                //MainForm.LocalServerHandler = null;
                            }
                        }
                        if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                        {
                            if (MainForm.ExtCaptionIP != ExtCaptionIPBox.Text)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.ExtCaptionIP);
                            }
                            /*if (MainForm.DWS1IP != DWS1IPBox.Text)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.DWS1IP);
                            }*/
                        }
                        config.AppSettings.Settings["ExtCaptionIP"].Value = ExtCaptionIPBox.Text;
                        MainForm.ExtCaptionIP = ExtCaptionIPBox.Text;
                        if (MainForm.DWSInterfaceEnabled)
                        {
                            config.AppSettings.Settings["DWS1Port"].Value = DWS1PortBox.Text;
                            MainForm.DWS1Port = DWS1PortBox.Text;
                            config.AppSettings.Settings["DWS1IP"].Value = DWS1IPBox.Text;
                            MainForm.DWS1IP = DWS1IPBox.Text;
                            if (MainForm.DWS1Handler != null)
                            {
                                if (MainForm.DWS1Handler.RemoteIP != MainForm.DWS1IP || MainForm.DWS1Handler.RemotePort != Convert.ToInt32(MainForm.DWS1Port) || MainForm.DWS1Handler.Disposed)
                                {
                                    MainForm.DWS1Handler.Dispose();
                                    MainForm.DWS1Handler = new SocketLib.TcpClientBase(MainForm.DWS1IP, Convert.ToInt32(MainForm.DWS1Port));
                                    MainForm.DWS1Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.DWS1Handler.logmsg += MainForm.logmsg;
                                    MainForm.DWS1Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.DWS1Handler = new SocketLib.TcpClientBase(MainForm.DWS1IP, Convert.ToInt32(MainForm.DWS1Port));
                                MainForm.DWS1Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.DWS1Handler.logmsg += MainForm.logmsg;
                                MainForm.DWS1Handler.OnConnect += MainForm.OnConnect;
                            }
                            
                        }
                        else
                        {
                            config.AppSettings.Settings["DWS1Port"].Value = DWS1PortBox.Text;
                            MainForm.DWS1Port = DWS1PortBox.Text;
                            config.AppSettings.Settings["DWS1IP"].Value = DWS1IPBox.Text;
                            MainForm.DWS1IP = DWS1IPBox.Text;
                            if (MainForm.DWS1Handler != null)
                            {
                                MainForm.DWS1Handler.Dispose();
                                //MainForm.DWS1Handler = null;
                            }
                        }
                        if (Ind11OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct11Enable"].Value = "true";
                            MainForm.Induct11Enable = true;
                            config.AppSettings.Settings["Bottom11Port"].Value = Bot11PortBox.Text;
                            MainForm.Bottom11Port = Bot11PortBox.Text;
                            config.AppSettings.Settings["Bottom11IP"].Value = Bot11IPBox.Text;
                            MainForm.Bottom11IP = Bot11IPBox.Text;
                            if (MainForm.Bottom11Handler != null)
                            {
                                if (MainForm.Bottom11Handler.RemoteIP != MainForm.Bottom11IP || MainForm.Bottom11Handler.RemotePort != Convert.ToInt32(MainForm.Bottom11Port) || MainForm.Bottom11Handler.Disposed)
                                {
                                    MainForm.Bottom11Handler.Dispose();
                                    MainForm.Bottom11Handler = new SocketLib.TcpClientBase(MainForm.Bottom11IP, Convert.ToInt32(MainForm.Bottom11Port));
                                    MainForm.Bottom11Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom11Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom11Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom11Handler = new SocketLib.TcpClientBase(MainForm.Bottom11IP, Convert.ToInt32(MainForm.Bottom11Port));
                                MainForm.Bottom11Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom11Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom11Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc11IP != Plc11IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc11IP);
                                }
                            }
                            config.AppSettings.Settings["Plc11IP"].Value = Plc11IPBox.Text;
                            MainForm.Plc11IP = Plc11IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct11Enable"].Value = "false";
                            MainForm.Induct11Enable = false;
                            Bot11PortBox.Text = "";
                            config.AppSettings.Settings["Bottom11Port"].Value = Bot11PortBox.Text;
                            MainForm.Bottom11Port = Bot11PortBox.Text;
                            Bot11IPBox.Text = "";
                            config.AppSettings.Settings["Bottom11IP"].Value = Bot11IPBox.Text;
                            MainForm.Bottom11IP = Bot11IPBox.Text;
                            if (MainForm.Bottom11Handler != null)
                            {
                                MainForm.Bottom11Handler.Dispose();
                                //MainForm.Bottom11Handler = null;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc11IP);
                            }
                            Plc11IPBox.Text = "";
                            config.AppSettings.Settings["Plc11IP"].Value = Plc11IPBox.Text;
                            MainForm.Plc11IP = Plc11IPBox.Text;
                        }
                        if (Ind12OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct12Enable"].Value = "true";
                            MainForm.Induct12Enable = true;
                            config.AppSettings.Settings["Bottom12Port"].Value = Bot12PortBox.Text;
                            MainForm.Bottom12Port = Bot12PortBox.Text;
                            config.AppSettings.Settings["Bottom12IP"].Value = Bot12IPBox.Text;
                            MainForm.Bottom12IP = Bot12IPBox.Text;
                            if (MainForm.Bottom12Handler != null)
                            {
                                if (MainForm.Bottom12Handler.RemoteIP != MainForm.Bottom12IP || MainForm.Bottom12Handler.RemotePort != Convert.ToInt32(MainForm.Bottom12Port) || MainForm.Bottom12Handler.Disposed)
                                {
                                    MainForm.Bottom12Handler.Dispose();
                                    MainForm.Bottom12Handler = new SocketLib.TcpClientBase(MainForm.Bottom12IP, Convert.ToInt32(MainForm.Bottom12Port));
                                    MainForm.Bottom12Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom12Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom12Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom12Handler = new SocketLib.TcpClientBase(MainForm.Bottom12IP, Convert.ToInt32(MainForm.Bottom12Port));
                                MainForm.Bottom12Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom12Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom12Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc12IP != Plc12IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc12IP);
                                }
                            }
                            config.AppSettings.Settings["Plc12IP"].Value = Plc12IPBox.Text;
                            MainForm.Plc12IP = Plc12IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct12Enable"].Value = "false";
                            MainForm.Induct12Enable = false;
                            Bot12PortBox.Text = "";
                            config.AppSettings.Settings["Bottom12Port"].Value = Bot12PortBox.Text;
                            MainForm.Bottom12Port = Bot12PortBox.Text;
                            Bot12IPBox.Text = "";
                            config.AppSettings.Settings["Bottom12IP"].Value = Bot12IPBox.Text;
                            MainForm.Bottom12IP = Bot12IPBox.Text;
                            if (MainForm.Bottom12Handler != null)
                            {
                                MainForm.Bottom12Handler.Dispose();
                                //MainForm.Bottom12Handler = null;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc12IP);
                            }
                            Plc12IPBox.Text = "";
                            config.AppSettings.Settings["Plc12IP"].Value = Plc12IPBox.Text;
                            MainForm.Plc12IP = Plc12IPBox.Text;
                        }
                        if (Ind13OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct13Enable"].Value = "true";
                            MainForm.Induct13Enable = true;
                            config.AppSettings.Settings["Bottom13Port"].Value = Bot13PortBox.Text;
                            MainForm.Bottom13Port = Bot13PortBox.Text;
                            config.AppSettings.Settings["Bottom13IP"].Value = Bot13IPBox.Text;
                            MainForm.Bottom13IP = Bot13IPBox.Text;
                            if (MainForm.Bottom13Handler != null)
                            {
                                if (MainForm.Bottom13Handler.RemoteIP != MainForm.Bottom13IP || MainForm.Bottom13Handler.RemotePort != Convert.ToInt32(MainForm.Bottom13Port) || MainForm.Bottom13Handler.Disposed)
                                {
                                    MainForm.Bottom13Handler.Dispose();
                                    MainForm.Bottom13Handler = new SocketLib.TcpClientBase(MainForm.Bottom13IP, Convert.ToInt32(MainForm.Bottom13Port));
                                    MainForm.Bottom13Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom13Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom13Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom13Handler = new SocketLib.TcpClientBase(MainForm.Bottom13IP, Convert.ToInt32(MainForm.Bottom13Port));
                                MainForm.Bottom13Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom13Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom13Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc13IP != Plc13IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc13IP);
                                }
                            }
                            config.AppSettings.Settings["Plc13IP"].Value = Plc13IPBox.Text;
                            MainForm.Plc13IP = Plc13IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct13Enable"].Value = "false";
                            MainForm.Induct13Enable = false;
                            Bot13PortBox.Text = "";
                            config.AppSettings.Settings["Bottom13Port"].Value = Bot13PortBox.Text;
                            MainForm.Bottom13Port = Bot13PortBox.Text;
                            Bot13IPBox.Text = "";
                            config.AppSettings.Settings["Bottom13IP"].Value = Bot13IPBox.Text;
                            MainForm.Bottom13IP = Bot13IPBox.Text;
                            if (MainForm.Bottom13Handler != null)
                            {
                                MainForm.Bottom13Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc13IP);
                            }
                            Plc13IPBox.Text = "";
                            config.AppSettings.Settings["Plc13IP"].Value = Plc13IPBox.Text;
                            MainForm.Plc13IP = Plc13IPBox.Text;
                        }
                        if (Ind14OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct14Enable"].Value = "true";
                            MainForm.Induct14Enable = true;
                            config.AppSettings.Settings["Bottom14Port"].Value = Bot14PortBox.Text;
                            MainForm.Bottom14Port = Bot14PortBox.Text;
                            config.AppSettings.Settings["Bottom14IP"].Value = Bot14IPBox.Text;
                            MainForm.Bottom14IP = Bot14IPBox.Text;
                            if (MainForm.Bottom14Handler != null)
                            {
                                if (MainForm.Bottom14Handler.RemoteIP != MainForm.Bottom14IP || MainForm.Bottom14Handler.RemotePort != Convert.ToInt32(MainForm.Bottom14Port) || MainForm.Bottom14Handler.Disposed)
                                {
                                    MainForm.Bottom14Handler.Dispose();
                                    MainForm.Bottom14Handler = new SocketLib.TcpClientBase(MainForm.Bottom14IP, Convert.ToInt32(MainForm.Bottom14Port));
                                    MainForm.Bottom14Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom14Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom14Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom14Handler = new SocketLib.TcpClientBase(MainForm.Bottom14IP, Convert.ToInt32(MainForm.Bottom14Port));
                                MainForm.Bottom14Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom14Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom14Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc14IP != Plc14IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc14IP);
                                }
                            }
                            config.AppSettings.Settings["Plc14IP"].Value = Plc14IPBox.Text;
                            MainForm.Plc14IP = Plc14IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct14Enable"].Value = "false";
                            MainForm.Induct14Enable = false;
                            Bot14PortBox.Text = "";
                            config.AppSettings.Settings["Bottom14Port"].Value = Bot14PortBox.Text;
                            MainForm.Bottom14Port = Bot14PortBox.Text;
                            Bot14IPBox.Text = "";
                            config.AppSettings.Settings["Bottom14IP"].Value = Bot14IPBox.Text;
                            MainForm.Bottom14IP = Bot14IPBox.Text;
                            if (MainForm.Bottom14Handler != null)
                            {
                                MainForm.Bottom14Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc14IP);
                            }
                            Plc14IPBox.Text = "";
                            config.AppSettings.Settings["Plc14IP"].Value = Plc14IPBox.Text;
                            MainForm.Plc14IP = Plc14IPBox.Text;
                        }
                        if (Ind15OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct15Enable"].Value = "true";
                            MainForm.Induct15Enable = true;
                            config.AppSettings.Settings["Bottom15Port"].Value = Bot15PortBox.Text;
                            MainForm.Bottom15Port = Bot15PortBox.Text;
                            config.AppSettings.Settings["Bottom15IP"].Value = Bot15IPBox.Text;
                            MainForm.Bottom15IP = Bot15IPBox.Text;
                            if (MainForm.Bottom15Handler != null)
                            {
                                if (MainForm.Bottom15Handler.RemoteIP != MainForm.Bottom15IP || MainForm.Bottom15Handler.RemotePort != Convert.ToInt32(MainForm.Bottom15Port) || MainForm.Bottom15Handler.Disposed)
                                {
                                    MainForm.Bottom15Handler.Dispose();
                                    MainForm.Bottom15Handler = new SocketLib.TcpClientBase(MainForm.Bottom15IP, Convert.ToInt32(MainForm.Bottom15Port));
                                    MainForm.Bottom15Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom15Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom15Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom15Handler = new SocketLib.TcpClientBase(MainForm.Bottom15IP, Convert.ToInt32(MainForm.Bottom15Port));
                                MainForm.Bottom15Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom15Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom15Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc15IP != Plc15IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc15IP);
                                }
                            }
                            config.AppSettings.Settings["Plc15IP"].Value = Plc15IPBox.Text;
                            MainForm.Plc15IP = Plc15IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct15Enable"].Value = "false";
                            MainForm.Induct15Enable = false;
                            Bot15PortBox.Text = "";
                            config.AppSettings.Settings["Bottom15Port"].Value = Bot15PortBox.Text;
                            MainForm.Bottom15Port = Bot15PortBox.Text;
                            Bot15IPBox.Text = "";
                            config.AppSettings.Settings["Bottom15IP"].Value = Bot15IPBox.Text;
                            MainForm.Bottom15IP = Bot15IPBox.Text;
                            if (MainForm.Bottom15Handler != null)
                            {
                                MainForm.Bottom15Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc15IP);
                            }
                            Plc15IPBox.Text = "";
                            config.AppSettings.Settings["Plc15IP"].Value = Plc15IPBox.Text;
                            MainForm.Plc15IP = Plc15IPBox.Text;
                        }
                        if (Ind21OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct21Enable"].Value = "true";
                            MainForm.Induct21Enable = true;
                            config.AppSettings.Settings["Bottom21Port"].Value = Bot21PortBox.Text;
                            MainForm.Bottom21Port = Bot21PortBox.Text;
                            config.AppSettings.Settings["Bottom21IP"].Value = Bot21IPBox.Text;
                            MainForm.Bottom21IP = Bot21IPBox.Text;
                            if (MainForm.Bottom21Handler != null)
                            {
                                if (MainForm.Bottom21Handler.RemoteIP != MainForm.Bottom21IP || MainForm.Bottom21Handler.RemotePort != Convert.ToInt32(MainForm.Bottom21Port) || MainForm.Bottom21Handler.Disposed)
                                {
                                    MainForm.Bottom21Handler.Dispose();
                                    MainForm.Bottom21Handler = new SocketLib.TcpClientBase(MainForm.Bottom21IP, Convert.ToInt32(MainForm.Bottom21Port));
                                    MainForm.Bottom21Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom21Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom21Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom21Handler = new SocketLib.TcpClientBase(MainForm.Bottom21IP, Convert.ToInt32(MainForm.Bottom21Port));
                                MainForm.Bottom21Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom21Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom21Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc21IP != Plc21IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc21IP);
                                }
                            }
                            config.AppSettings.Settings["Plc21IP"].Value = Plc21IPBox.Text;
                            MainForm.Plc21IP = Plc21IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct21Enable"].Value = "false";
                            MainForm.Induct21Enable = false;
                            Bot21PortBox.Text = "";
                            config.AppSettings.Settings["Bottom21Port"].Value = Bot21PortBox.Text;
                            MainForm.Bottom21Port = Bot21PortBox.Text;
                            Bot21IPBox.Text = "";
                            config.AppSettings.Settings["Bottom21IP"].Value = Bot21IPBox.Text;
                            MainForm.Bottom21IP = Bot21IPBox.Text;
                            if (MainForm.Bottom21Handler != null)
                            {
                                MainForm.Bottom21Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc21IP);
                            }
                            Plc21IPBox.Text = "";
                            config.AppSettings.Settings["Plc21IP"].Value = Plc21IPBox.Text;
                            MainForm.Plc21IP = Plc21IPBox.Text;
                        }
                        if (Ind22OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct22Enable"].Value = "true";
                            MainForm.Induct22Enable = true;
                            config.AppSettings.Settings["Bottom22Port"].Value = Bot22PortBox.Text;
                            MainForm.Bottom22Port = Bot22PortBox.Text;
                            config.AppSettings.Settings["Bottom22IP"].Value = Bot22IPBox.Text;
                            MainForm.Bottom22IP = Bot22IPBox.Text;
                            if (MainForm.Bottom22Handler != null)
                            {
                                if (MainForm.Bottom22Handler.RemoteIP != MainForm.Bottom22IP || MainForm.Bottom22Handler.RemotePort != Convert.ToInt32(MainForm.Bottom22Port) || MainForm.Bottom22Handler.Disposed)
                                {
                                    MainForm.Bottom22Handler.Dispose();
                                    MainForm.Bottom22Handler = new SocketLib.TcpClientBase(MainForm.Bottom22IP, Convert.ToInt32(MainForm.Bottom22Port));
                                    MainForm.Bottom22Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom22Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom22Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom22Handler = new SocketLib.TcpClientBase(MainForm.Bottom22IP, Convert.ToInt32(MainForm.Bottom22Port));
                                MainForm.Bottom22Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom22Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom22Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc22IP != Plc22IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc22IP);
                                }
                            }
                            config.AppSettings.Settings["Plc22IP"].Value = Plc22IPBox.Text;
                            MainForm.Plc22IP = Plc22IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct22Enable"].Value = "false";
                            MainForm.Induct22Enable = false;
                            Bot22PortBox.Text = "";
                            config.AppSettings.Settings["Bottom22Port"].Value = Bot22PortBox.Text;
                            MainForm.Bottom22Port = Bot22PortBox.Text;
                            Bot22IPBox.Text = "";
                            config.AppSettings.Settings["Bottom22IP"].Value = Bot22IPBox.Text;
                            MainForm.Bottom22IP = Bot22IPBox.Text;
                            if (MainForm.Bottom22Handler != null)
                            {
                                MainForm.Bottom22Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc22IP);
                            }
                            Plc22IPBox.Text = "";
                            config.AppSettings.Settings["Plc22IP"].Value = Plc22IPBox.Text;
                            MainForm.Plc22IP = Plc22IPBox.Text;
                        }
                        if (Ind23OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct23Enable"].Value = "true";
                            MainForm.Induct23Enable = true;
                            config.AppSettings.Settings["Bottom23Port"].Value = Bot23PortBox.Text;
                            MainForm.Bottom23Port = Bot23PortBox.Text;
                            config.AppSettings.Settings["Bottom23IP"].Value = Bot23IPBox.Text;
                            MainForm.Bottom23IP = Bot23IPBox.Text;
                            if (MainForm.Bottom23Handler != null)
                            {
                                if (MainForm.Bottom23Handler.RemoteIP != MainForm.Bottom23IP || MainForm.Bottom23Handler.RemotePort != Convert.ToInt32(MainForm.Bottom23Port) || MainForm.Bottom23Handler.Disposed)
                                {
                                    MainForm.Bottom23Handler.Dispose();
                                    MainForm.Bottom23Handler = new SocketLib.TcpClientBase(MainForm.Bottom23IP, Convert.ToInt32(MainForm.Bottom23Port));
                                    MainForm.Bottom23Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom23Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom23Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom23Handler = new SocketLib.TcpClientBase(MainForm.Bottom23IP, Convert.ToInt32(MainForm.Bottom23Port));
                                MainForm.Bottom23Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom23Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom23Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc23IP != Plc23IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc23IP);
                                }
                            }
                            config.AppSettings.Settings["Plc23IP"].Value = Plc23IPBox.Text;
                            MainForm.Plc23IP = Plc23IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct23Enable"].Value = "false";
                            MainForm.Induct23Enable = false;
                            Bot23PortBox.Text = "";
                            config.AppSettings.Settings["Bottom23Port"].Value = Bot23PortBox.Text;
                            MainForm.Bottom23Port = Bot23PortBox.Text;
                            Bot23IPBox.Text = "";
                            config.AppSettings.Settings["Bottom23IP"].Value = Bot23IPBox.Text;
                            MainForm.Bottom23IP = Bot23IPBox.Text;
                            if (MainForm.Bottom23Handler != null)
                            {
                                MainForm.Bottom23Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc23IP);
                            }
                            Plc23IPBox.Text = "";
                            config.AppSettings.Settings["Plc23IP"].Value = Plc23IPBox.Text;
                            MainForm.Plc23IP = Plc23IPBox.Text;
                        }
                        if (Ind24OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct24Enable"].Value = "true";
                            MainForm.Induct24Enable = true;
                            config.AppSettings.Settings["Bottom24Port"].Value = Bot24PortBox.Text;
                            MainForm.Bottom24Port = Bot24PortBox.Text;
                            config.AppSettings.Settings["Bottom24IP"].Value = Bot24IPBox.Text;
                            MainForm.Bottom24IP = Bot24IPBox.Text;
                            if (MainForm.Bottom24Handler != null)
                            {
                                if (MainForm.Bottom24Handler.RemoteIP != MainForm.Bottom24IP || MainForm.Bottom24Handler.RemotePort != Convert.ToInt32(MainForm.Bottom24Port) || MainForm.Bottom24Handler.Disposed)
                                {
                                    MainForm.Bottom24Handler.Dispose();
                                    MainForm.Bottom24Handler = new SocketLib.TcpClientBase(MainForm.Bottom24IP, Convert.ToInt32(MainForm.Bottom24Port));
                                    MainForm.Bottom24Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom24Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom24Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom24Handler = new SocketLib.TcpClientBase(MainForm.Bottom24IP, Convert.ToInt32(MainForm.Bottom24Port));
                                MainForm.Bottom24Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom24Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom24Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc24IP != Plc24IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc24IP);
                                }
                            }
                            config.AppSettings.Settings["Plc24IP"].Value = Plc24IPBox.Text;
                            MainForm.Plc24IP = Plc24IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct24Enable"].Value = "false";
                            MainForm.Induct24Enable = false;
                            Bot24PortBox.Text = "";
                            config.AppSettings.Settings["Bottom24Port"].Value = Bot24PortBox.Text;
                            MainForm.Bottom24Port = Bot24PortBox.Text;
                            Bot24IPBox.Text = "";
                            config.AppSettings.Settings["Bottom24IP"].Value = Bot24IPBox.Text;
                            MainForm.Bottom24IP = Bot24IPBox.Text;
                            if (MainForm.Bottom24Handler != null)
                            {
                                MainForm.Bottom24Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc24IP);
                            }
                            Plc24IPBox.Text = "";
                            config.AppSettings.Settings["Plc24IP"].Value = Plc24IPBox.Text;
                            MainForm.Plc24IP = Plc24IPBox.Text;
                        }
                        if (Ind25OnBox.Checked)
                        {
                            config.AppSettings.Settings["Induct25Enable"].Value = "true";
                            MainForm.Induct25Enable = true;
                            config.AppSettings.Settings["Bottom25Port"].Value = Bot25PortBox.Text;
                            MainForm.Bottom25Port = Bot25PortBox.Text;
                            config.AppSettings.Settings["Bottom25IP"].Value = Bot25IPBox.Text;
                            MainForm.Bottom25IP = Bot25IPBox.Text;
                            if (MainForm.Bottom25Handler != null)
                            {
                                if (MainForm.Bottom25Handler.RemoteIP != MainForm.Bottom25IP || MainForm.Bottom25Handler.RemotePort != Convert.ToInt32(MainForm.Bottom25Port) || MainForm.Bottom25Handler.Disposed)
                                {
                                    MainForm.Bottom25Handler.Dispose();
                                    MainForm.Bottom25Handler = new SocketLib.TcpClientBase(MainForm.Bottom25IP, Convert.ToInt32(MainForm.Bottom25Port));
                                    MainForm.Bottom25Handler.OnExpectedData += MainForm.OnExpectedData;
                                    MainForm.Bottom25Handler.logmsg += MainForm.logmsg;
                                    MainForm.Bottom25Handler.OnConnect += MainForm.OnConnect;
                                }
                            }
                            else
                            {
                                MainForm.Bottom25Handler = new SocketLib.TcpClientBase(MainForm.Bottom25IP, Convert.ToInt32(MainForm.Bottom25Port));
                                MainForm.Bottom25Handler.OnExpectedData += MainForm.OnExpectedData;
                                MainForm.Bottom25Handler.logmsg += MainForm.logmsg;
                                MainForm.Bottom25Handler.OnConnect += MainForm.OnConnect;
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                if (MainForm.Plc25IP != Plc25IPBox.Text)
                                {
                                    MainForm.LocalServerHandler.Remove(MainForm.Plc25IP);
                                }
                            }
                            config.AppSettings.Settings["Plc25IP"].Value = Plc25IPBox.Text;
                            MainForm.Plc25IP = Plc25IPBox.Text;
                        }
                        else
                        {
                            config.AppSettings.Settings["Induct25Enable"].Value = "false";
                            MainForm.Induct25Enable = false;
                            Bot25PortBox.Text = "";
                            config.AppSettings.Settings["Bottom25Port"].Value = Bot25PortBox.Text;
                            MainForm.Bottom25Port = Bot25PortBox.Text;
                            Bot25IPBox.Text = "";
                            config.AppSettings.Settings["Bottom25IP"].Value = Bot25IPBox.Text;
                            MainForm.Bottom25IP = Bot25IPBox.Text;
                            if (MainForm.Bottom25Handler != null)
                            {
                                MainForm.Bottom25Handler.Dispose();
                            }
                            if (MainForm.LocalServerEnable && MainForm.LocalServerHandler != null)
                            {
                                MainForm.LocalServerHandler.Remove(MainForm.Plc25IP);
                            }
                            Plc25IPBox.Text = "";
                            config.AppSettings.Settings["Plc25IP"].Value = Plc25IPBox.Text;
                            MainForm.Plc25IP = Plc25IPBox.Text;
                        }
                        config.Save(ConfigurationSaveMode.Modified);
                        ConfigurationManager.RefreshSection("appSettings");
                        MessageBox.Show("配置保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex) { MessageBox.Show("配置保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
            else
            {
                string ErrMsg = "";
                if (!flag1)
                {
                    ErrMsg += "——IP地址有重复\r\n";
                }
                if (!flag2)
                {
                    ErrMsg += "——导入台一区工站一地址格式有误\r\n";
                }
                if (!flag3)
                {
                    ErrMsg += "——导入台一区工站二地址格式有误\r\n";
                }
                if (!flag4)
                {
                    ErrMsg += "——导入台一区工站三地址格式有误\r\n";
                }
                if (!flag5)
                {
                    ErrMsg += "——导入台一区工站四地址格式有误\r\n";
                }
                if (!flag6)
                {
                    ErrMsg += "——导入台一区工站五地址格式有误\r\n";
                }
                if (!flag7)
                {
                    ErrMsg += "——导入台二区工站一地址格式有误\r\n";
                }
                if (!flag8)
                {
                    ErrMsg += "——导入台二区工站二地址格式有误\r\n";
                }
                if (!flag9)
                {
                    ErrMsg += "——导入台二区工站三地址格式有误\r\n";
                }
                if (!flag10)
                {
                    ErrMsg += "——导入台二区工站四地址格式有误\r\n";
                }
                if (!flag11)
                {
                    ErrMsg += "——导入台二区工站五地址格式有误\r\n";
                }
                if (!flag12)
                {
                    ErrMsg += "——本地服务器地址格式有误\r\n";
                }
                if (!flag13)
                {
                    ErrMsg += "——外部水印信息客户端地址格式有误\r\n";
                }
                if (!flag14)
                {
                    ErrMsg += "——DWS服务器地址格式有误\r\n";
                }
                MessageBox.Show(ErrMsg, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (MainForm.DWS1Handler != null)
                {
                    if (MainForm.DWS1Handler.isalive)
                    {
                        DWS1StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        DWS1StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    DWS1StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom11Handler != null)
                {
                    if (MainForm.Bottom11Handler.isalive)
                    {
                        Bot11StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot11StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot11StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom12Handler != null)
                {
                    if (MainForm.Bottom12Handler.isalive)
                    {
                        Bot12StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot12StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot12StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom13Handler != null)
                {
                    if (MainForm.Bottom13Handler.isalive)
                    {
                        Bot13StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot13StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot13StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom14Handler != null)
                {
                    if (MainForm.Bottom14Handler.isalive)
                    {
                        Bot14StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot14StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot14StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom15Handler != null)
                {
                    if (MainForm.Bottom15Handler.isalive)
                    {
                        Bot15StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot15StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot15StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom21Handler != null)
                {
                    if (MainForm.Bottom21Handler.isalive)
                    {
                        Bot21StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot21StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot21StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom22Handler != null)
                {
                    if (MainForm.Bottom22Handler.isalive)
                    {
                        Bot22StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot22StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot22StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom23Handler != null)
                {
                    if (MainForm.Bottom23Handler.isalive)
                    {
                        Bot23StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot23StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot23StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom24Handler != null)
                {
                    if (MainForm.Bottom24Handler.isalive)
                    {
                        Bot24StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot24StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot24StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            try
            {
                if (MainForm.Bottom25Handler != null)
                {
                    if (MainForm.Bottom25Handler.isalive)
                    {
                        Bot25StatusLabel.BackColor = Color.Green;
                    }
                    else
                    {
                        Bot25StatusLabel.BackColor = Color.Gray;
                    }
                }
                else
                {
                    Bot25StatusLabel.BackColor = Color.Gray;
                }
            }
            catch { }
            if (MainForm.LocalServerHandler != null)
            {
                if (MainForm.LocalServerHandler.Listening)
                {
                    try
                    {
                        if (MainForm.ExtCaptionIP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.ExtCaptionIP))
                            {
                                ExtCaptionStatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                ExtCaptionStatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            ExtCaptionStatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    /*try
                    {
                        if (MainForm.DWS1IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.DWS1IP))
                            {
                                DWS1StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                DWS1StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            DWS1StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }*/
                    try
                    {
                        if (MainForm.Plc11IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc11IP))
                            {
                                Plc11StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc11StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc11StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc12IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc12IP))
                            {
                                Plc12StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc12StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc12StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc13IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc13IP))
                            {
                                Plc13StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc13StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc13StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc14IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc14IP))
                            {
                                Plc14StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc14StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc14StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc15IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc15IP))
                            {
                                Plc15StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc15StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc15StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc21IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc21IP))
                            {
                                Plc21StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc21StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc21StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc22IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc22IP))
                            {
                                Plc22StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc22StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc22StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc23IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc23IP))
                            {
                                Plc23StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc23StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc23StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc24IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc24IP))
                            {
                                Plc24StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc24StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc24StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                    try
                    {
                        if (MainForm.Plc25IP != "")
                        {
                            if (MainForm.LocalServerHandler.CheckStatus(MainForm.Plc25IP))
                            {
                                Plc25StatusLabel.BackColor = Color.Green;
                            }
                            else
                            {
                                Plc25StatusLabel.BackColor = Color.Gray;
                            }
                        }
                        else
                        {
                            Plc25StatusLabel.BackColor = Color.Gray;
                        }
                    }
                    catch { }
                }
                else
                {
                    Plc11StatusLabel.BackColor = Color.Gray;
                    Plc12StatusLabel.BackColor = Color.Gray;
                    Plc13StatusLabel.BackColor = Color.Gray;
                    Plc14StatusLabel.BackColor = Color.Gray;
                    Plc15StatusLabel.BackColor = Color.Gray;
                    Plc21StatusLabel.BackColor = Color.Gray;
                    Plc22StatusLabel.BackColor = Color.Gray;
                    Plc23StatusLabel.BackColor = Color.Gray;
                    Plc24StatusLabel.BackColor = Color.Gray;
                    Plc25StatusLabel.BackColor = Color.Gray;
                }
            }
            else
            {
                Plc11StatusLabel.BackColor = Color.Gray;
                Plc12StatusLabel.BackColor = Color.Gray;
                Plc13StatusLabel.BackColor = Color.Gray;
                Plc14StatusLabel.BackColor = Color.Gray;
                Plc15StatusLabel.BackColor = Color.Gray;
                Plc21StatusLabel.BackColor = Color.Gray;
                Plc22StatusLabel.BackColor = Color.Gray;
                Plc23StatusLabel.BackColor = Color.Gray;
                Plc24StatusLabel.BackColor = Color.Gray;
                Plc25StatusLabel.BackColor = Color.Gray;
            }
        }
    }
}
