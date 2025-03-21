using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Configuration;

namespace 存图助手标准版
{
    public partial class GridSetting : Form
    {
        
        public GridSetting()
        {
            InitializeComponent();
        }

       

        

        private void BtnSave_Click(object sender, EventArgs e)
        {
            PixelSizeText.Text = Regex.IsMatch(PixelSizeText.Text, MainForm.DoublePattern) ? PixelSizeText.Text : MainForm.DefaultPixelSize;
            FocusLengthText.Text = Regex.IsMatch(FocusLengthText.Text, MainForm.DoublePattern) ? FocusLengthText.Text : MainForm.DefaultFocusLength;
            ReadAngleText.Text = Regex.IsMatch(ReadAngleText.Text, MainForm.DoublePattern) ? ReadAngleText.Text : MainForm.DefaultReadAngle;
            MountingHeightText.Text = Regex.IsMatch(MountingHeightText.Text, MainForm.DoublePattern) ? MountingHeightText.Text : MainForm.DefaultMountingHeight;
            MaxParcelHeightText.Text = Regex.IsMatch(MaxParcelHeightText.Text, MainForm.DoublePattern) ? MaxParcelHeightText.Text : MainForm.DefaultMaxParcelHeight;
            lock (MainForm.OpenConfigLock)
            {
                try
                {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["PixelSize"].Value = PixelSizeText.Text;
                    MainForm.PixelSize = Convert.ToDouble(PixelSizeText.Text);
                    config.AppSettings.Settings["FocusLength"].Value = FocusLengthText.Text;
                    MainForm.FocusLength = Convert.ToDouble(FocusLengthText.Text);
                    config.AppSettings.Settings["ReadAngle"].Value = ReadAngleText.Text;
                    MainForm.ReadAngle = Convert.ToDouble(ReadAngleText.Text);
                    config.AppSettings.Settings["MountingHeight"].Value = MountingHeightText.Text;
                    MainForm.MountingHeight = Convert.ToDouble(MountingHeightText.Text);
                    config.AppSettings.Settings["MaxParcelHeight"].Value = MaxParcelHeightText.Text;
                    MainForm.MaxParcelHeight = Convert.ToDouble(MaxParcelHeightText.Text);
                    config.Save(ConfigurationSaveMode.Modified);
                    ConfigurationManager.RefreshSection("appSettings");
                    MessageBox.Show("配置保存成功", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex) { MessageBox.Show("配置保存失败", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            }

        }

        private void GridSetting_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                if (Regex.IsMatch(ConfigurationManager.AppSettings["PixelSize"], MainForm.DoublePattern))
                {
                    PixelSizeText.Text = ConfigurationManager.AppSettings["PixelSize"];
                }
                else
                {
                    PixelSizeText.Text = MainForm.DefaultPixelSize;
                }
                if (Regex.IsMatch(ConfigurationManager.AppSettings["FocusLength"], MainForm.DoublePattern))
                {
                    FocusLengthText.Text = ConfigurationManager.AppSettings["FocusLength"];
                }
                else
                {
                    FocusLengthText.Text = MainForm.DefaultFocusLength;
                }
                if (Regex.IsMatch(ConfigurationManager.AppSettings["ReadAngle"], MainForm.DoublePattern))
                {
                    ReadAngleText.Text = ConfigurationManager.AppSettings["ReadAngle"];
                }
                else
                {
                    ReadAngleText.Text = MainForm.DefaultReadAngle;
                }
                if (Regex.IsMatch(ConfigurationManager.AppSettings["MountingHeight"], MainForm.DoublePattern))
                {
                    MountingHeightText.Text = ConfigurationManager.AppSettings["MountingHeight"];
                }
                else
                {
                    MountingHeightText.Text = MainForm.DefaultMountingHeight;
                }
                if (Regex.IsMatch(ConfigurationManager.AppSettings["MaxParcelHeight"], MainForm.DoublePattern))
                {
                    MaxParcelHeightText.Text = ConfigurationManager.AppSettings["MaxParcelHeight"];
                }
                else
                {
                    MaxParcelHeightText.Text = MainForm.DefaultMaxParcelHeight;
                }
            }
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}
