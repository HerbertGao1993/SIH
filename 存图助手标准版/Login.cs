using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace 存图助手标准版
{
    public partial class Login : Form
    {
        public bool dialogOK = false;
        public bool dialogNOK = false;
        public Login()
        {
            InitializeComponent();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            username.Focus();
        }

        private void Login_Shown(object sender, EventArgs e)
        {
            username.Focus();
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            if (username.Text == MainForm.AdminUser && password.Text == MainForm.AdminPassword)
            {
                dialogOK = true;
                this.Hide();
            }

            else
            {
                MessageBox.Show("请输入正确的用户名和密码", "错误");
                username.Text = "";
                password.Text = "";
                //dialogNOK = true;
                //this.Hide();
            }
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            dialogNOK = true;
            this.Hide();
        }
    }
}
