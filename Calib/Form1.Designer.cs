
namespace Calib
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.SelectImageBtn1 = new System.Windows.Forms.Button();
            this.StaCombo = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.PreviewBtn2 = new System.Windows.Forms.Button();
            this.PreviewBtn1 = new System.Windows.Forms.Button();
            this.SAVE_Btn = new System.Windows.Forms.Button();
            this.StartCal_Btn = new System.Windows.Forms.Button();
            this.SelectImageBtn2 = new System.Windows.Forms.Button();
            this.SelectImageBtn3 = new System.Windows.Forms.Button();
            this.SelectImageBtn4 = new System.Windows.Forms.Button();
            this.SelectImageBtn5 = new System.Windows.Forms.Button();
            this.SelectImageBtn6 = new System.Windows.Forms.Button();
            this.PreviewBtn6 = new System.Windows.Forms.Button();
            this.PreviewBtn5 = new System.Windows.Forms.Button();
            this.PreviewBtn3 = new System.Windows.Forms.Button();
            this.PreviewBtn4 = new System.Windows.Forms.Button();
            this.CalResultBox = new System.Windows.Forms.TextBox();
            this.CurrentPicBox = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.DeleteImageBtn6 = new System.Windows.Forms.Button();
            this.DeleteImageBtn5 = new System.Windows.Forms.Button();
            this.DeleteImageBtn4 = new System.Windows.Forms.Button();
            this.DeleteImageBtn3 = new System.Windows.Forms.Button();
            this.DeleteImageBtn2 = new System.Windows.Forms.Button();
            this.DeleteImageBtn1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.RatioCombo = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ol1text = new System.Windows.Forms.TextBox();
            this.ol2text = new System.Windows.Forms.TextBox();
            this.ol3text = new System.Windows.Forms.TextBox();
            this.ol4text = new System.Windows.Forms.TextBox();
            this.ol5text = new System.Windows.Forms.TextBox();
            this.ol6text = new System.Windows.Forms.TextBox();
            this.SelectMultiImageButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurrentPicBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // SelectImageBtn1
            // 
            this.SelectImageBtn1.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectImageBtn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectImageBtn1.Location = new System.Drawing.Point(82, 774);
            this.SelectImageBtn1.Margin = new System.Windows.Forms.Padding(2);
            this.SelectImageBtn1.Name = "SelectImageBtn1";
            this.SelectImageBtn1.Size = new System.Drawing.Size(96, 30);
            this.SelectImageBtn1.TabIndex = 104;
            this.SelectImageBtn1.Text = "选择图像";
            this.SelectImageBtn1.UseVisualStyleBackColor = true;
            this.SelectImageBtn1.Click += new System.EventHandler(this.SelectImageBtn1_Click);
            // 
            // StaCombo
            // 
            this.StaCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.StaCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.StaCombo.FormattingEnabled = true;
            this.StaCombo.Items.AddRange(new object[] {
            "工位一",
            "工位二",
            "工位三",
            "工位四"});
            this.StaCombo.Location = new System.Drawing.Point(298, 34);
            this.StaCombo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.StaCombo.Name = "StaCombo";
            this.StaCombo.Size = new System.Drawing.Size(132, 40);
            this.StaCombo.TabIndex = 217;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label5.Location = new System.Drawing.Point(37, 35);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(255, 39);
            this.label5.TabIndex = 216;
            this.label5.Text = "当前标定工站：";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::Calib.Properties.Resources.bottomscan;
            this.pictureBox1.Location = new System.Drawing.Point(38, 667);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(682, 52);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 218;
            this.pictureBox1.TabStop = false;
            // 
            // PreviewBtn2
            // 
            this.PreviewBtn2.BackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreviewBtn2.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.PreviewBtn2.FlatAppearance.BorderSize = 0;
            this.PreviewBtn2.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn2.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.PreviewBtn2.Location = new System.Drawing.Point(204, 715);
            this.PreviewBtn2.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBtn2.Name = "PreviewBtn2";
            this.PreviewBtn2.Size = new System.Drawing.Size(50, 50);
            this.PreviewBtn2.TabIndex = 227;
            this.PreviewBtn2.Text = "2";
            this.PreviewBtn2.UseVisualStyleBackColor = false;
            this.PreviewBtn2.Click += new System.EventHandler(this.PreviewBtn2_Click);
            // 
            // PreviewBtn1
            // 
            this.PreviewBtn1.BackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreviewBtn1.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.PreviewBtn1.FlatAppearance.BorderSize = 0;
            this.PreviewBtn1.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn1.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.PreviewBtn1.ForeColor = System.Drawing.Color.Black;
            this.PreviewBtn1.Location = new System.Drawing.Point(104, 715);
            this.PreviewBtn1.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBtn1.Name = "PreviewBtn1";
            this.PreviewBtn1.Size = new System.Drawing.Size(50, 50);
            this.PreviewBtn1.TabIndex = 226;
            this.PreviewBtn1.Text = "1";
            this.PreviewBtn1.UseVisualStyleBackColor = false;
            this.PreviewBtn1.Click += new System.EventHandler(this.PreviewBtn1_Click);
            // 
            // SAVE_Btn
            // 
            this.SAVE_Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.SAVE_Btn.Location = new System.Drawing.Point(1316, 486);
            this.SAVE_Btn.Margin = new System.Windows.Forms.Padding(2);
            this.SAVE_Btn.Name = "SAVE_Btn";
            this.SAVE_Btn.Size = new System.Drawing.Size(121, 62);
            this.SAVE_Btn.TabIndex = 231;
            this.SAVE_Btn.Text = "保存设置";
            this.SAVE_Btn.UseVisualStyleBackColor = true;
            this.SAVE_Btn.Click += new System.EventHandler(this.SAVE_Btn_Click);
            // 
            // StartCal_Btn
            // 
            this.StartCal_Btn.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F);
            this.StartCal_Btn.Location = new System.Drawing.Point(1148, 486);
            this.StartCal_Btn.Margin = new System.Windows.Forms.Padding(2);
            this.StartCal_Btn.Name = "StartCal_Btn";
            this.StartCal_Btn.Size = new System.Drawing.Size(121, 62);
            this.StartCal_Btn.TabIndex = 234;
            this.StartCal_Btn.Text = "开始标定";
            this.StartCal_Btn.UseVisualStyleBackColor = true;
            this.StartCal_Btn.Click += new System.EventHandler(this.StartCal_Btn_Click);
            // 
            // SelectImageBtn2
            // 
            this.SelectImageBtn2.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectImageBtn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectImageBtn2.Location = new System.Drawing.Point(182, 774);
            this.SelectImageBtn2.Margin = new System.Windows.Forms.Padding(2);
            this.SelectImageBtn2.Name = "SelectImageBtn2";
            this.SelectImageBtn2.Size = new System.Drawing.Size(96, 30);
            this.SelectImageBtn2.TabIndex = 235;
            this.SelectImageBtn2.Text = "选择图像";
            this.SelectImageBtn2.UseVisualStyleBackColor = true;
            this.SelectImageBtn2.Click += new System.EventHandler(this.SelectImageBtn2_Click);
            // 
            // SelectImageBtn3
            // 
            this.SelectImageBtn3.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectImageBtn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectImageBtn3.Location = new System.Drawing.Point(282, 774);
            this.SelectImageBtn3.Margin = new System.Windows.Forms.Padding(2);
            this.SelectImageBtn3.Name = "SelectImageBtn3";
            this.SelectImageBtn3.Size = new System.Drawing.Size(96, 30);
            this.SelectImageBtn3.TabIndex = 236;
            this.SelectImageBtn3.Text = "选择图像";
            this.SelectImageBtn3.UseVisualStyleBackColor = true;
            this.SelectImageBtn3.Click += new System.EventHandler(this.SelectImageBtn3_Click);
            // 
            // SelectImageBtn4
            // 
            this.SelectImageBtn4.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectImageBtn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectImageBtn4.Location = new System.Drawing.Point(382, 774);
            this.SelectImageBtn4.Margin = new System.Windows.Forms.Padding(2);
            this.SelectImageBtn4.Name = "SelectImageBtn4";
            this.SelectImageBtn4.Size = new System.Drawing.Size(96, 30);
            this.SelectImageBtn4.TabIndex = 237;
            this.SelectImageBtn4.Text = "选择图像";
            this.SelectImageBtn4.UseVisualStyleBackColor = true;
            this.SelectImageBtn4.Click += new System.EventHandler(this.SelectImageBtn4_Click);
            // 
            // SelectImageBtn5
            // 
            this.SelectImageBtn5.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectImageBtn5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectImageBtn5.Location = new System.Drawing.Point(482, 774);
            this.SelectImageBtn5.Margin = new System.Windows.Forms.Padding(2);
            this.SelectImageBtn5.Name = "SelectImageBtn5";
            this.SelectImageBtn5.Size = new System.Drawing.Size(96, 30);
            this.SelectImageBtn5.TabIndex = 238;
            this.SelectImageBtn5.Text = "选择图像";
            this.SelectImageBtn5.UseVisualStyleBackColor = true;
            this.SelectImageBtn5.Click += new System.EventHandler(this.SelectImageBtn5_Click);
            // 
            // SelectImageBtn6
            // 
            this.SelectImageBtn6.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectImageBtn6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectImageBtn6.Location = new System.Drawing.Point(582, 774);
            this.SelectImageBtn6.Margin = new System.Windows.Forms.Padding(2);
            this.SelectImageBtn6.Name = "SelectImageBtn6";
            this.SelectImageBtn6.Size = new System.Drawing.Size(96, 30);
            this.SelectImageBtn6.TabIndex = 239;
            this.SelectImageBtn6.Text = "选择图像";
            this.SelectImageBtn6.UseVisualStyleBackColor = true;
            this.SelectImageBtn6.Click += new System.EventHandler(this.SelectImageBtn6_Click);
            // 
            // PreviewBtn6
            // 
            this.PreviewBtn6.BackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn6.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreviewBtn6.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.PreviewBtn6.FlatAppearance.BorderSize = 0;
            this.PreviewBtn6.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn6.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn6.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn6.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.PreviewBtn6.Location = new System.Drawing.Point(604, 715);
            this.PreviewBtn6.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBtn6.Name = "PreviewBtn6";
            this.PreviewBtn6.Size = new System.Drawing.Size(50, 50);
            this.PreviewBtn6.TabIndex = 240;
            this.PreviewBtn6.Text = "6";
            this.PreviewBtn6.UseVisualStyleBackColor = false;
            this.PreviewBtn6.Click += new System.EventHandler(this.PreviewBtn6_Click);
            // 
            // PreviewBtn5
            // 
            this.PreviewBtn5.BackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreviewBtn5.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.PreviewBtn5.FlatAppearance.BorderSize = 0;
            this.PreviewBtn5.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn5.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn5.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn5.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.PreviewBtn5.Location = new System.Drawing.Point(504, 715);
            this.PreviewBtn5.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBtn5.Name = "PreviewBtn5";
            this.PreviewBtn5.Size = new System.Drawing.Size(50, 50);
            this.PreviewBtn5.TabIndex = 241;
            this.PreviewBtn5.Text = "5";
            this.PreviewBtn5.UseVisualStyleBackColor = false;
            this.PreviewBtn5.Click += new System.EventHandler(this.PreviewBtn5_Click);
            // 
            // PreviewBtn3
            // 
            this.PreviewBtn3.BackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreviewBtn3.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.PreviewBtn3.FlatAppearance.BorderSize = 0;
            this.PreviewBtn3.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn3.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.PreviewBtn3.Location = new System.Drawing.Point(304, 715);
            this.PreviewBtn3.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBtn3.Name = "PreviewBtn3";
            this.PreviewBtn3.Size = new System.Drawing.Size(50, 50);
            this.PreviewBtn3.TabIndex = 242;
            this.PreviewBtn3.Text = "3";
            this.PreviewBtn3.UseVisualStyleBackColor = false;
            this.PreviewBtn3.Click += new System.EventHandler(this.PreviewBtn3_Click);
            // 
            // PreviewBtn4
            // 
            this.PreviewBtn4.BackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PreviewBtn4.FlatAppearance.BorderColor = System.Drawing.Color.Yellow;
            this.PreviewBtn4.FlatAppearance.BorderSize = 0;
            this.PreviewBtn4.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn4.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Yellow;
            this.PreviewBtn4.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PreviewBtn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.PreviewBtn4.Location = new System.Drawing.Point(404, 715);
            this.PreviewBtn4.Margin = new System.Windows.Forms.Padding(2);
            this.PreviewBtn4.Name = "PreviewBtn4";
            this.PreviewBtn4.Size = new System.Drawing.Size(50, 50);
            this.PreviewBtn4.TabIndex = 243;
            this.PreviewBtn4.Text = "4";
            this.PreviewBtn4.UseVisualStyleBackColor = false;
            this.PreviewBtn4.Click += new System.EventHandler(this.PreviewBtn4_Click);
            // 
            // CalResultBox
            // 
            this.CalResultBox.Location = new System.Drawing.Point(1148, 99);
            this.CalResultBox.Multiline = true;
            this.CalResultBox.Name = "CalResultBox";
            this.CalResultBox.Size = new System.Drawing.Size(289, 346);
            this.CalResultBox.TabIndex = 244;
            // 
            // CurrentPicBox
            // 
            this.CurrentPicBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CurrentPicBox.Location = new System.Drawing.Point(0, 0);
            this.CurrentPicBox.Name = "CurrentPicBox";
            this.CurrentPicBox.Size = new System.Drawing.Size(616, 224);
            this.CurrentPicBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.CurrentPicBox.TabIndex = 224;
            this.CurrentPicBox.TabStop = false;
            this.CurrentPicBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CurrentPicBox_MouseDown);
            this.CurrentPicBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CurrentPicBox_MouseMove);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.CurrentPicBox);
            this.panel1.Location = new System.Drawing.Point(38, 99);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1037, 543);
            this.panel1.TabIndex = 245;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.CurrentPicBox_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.CurrentPicBox_MouseMove);
            // 
            // DeleteImageBtn6
            // 
            this.DeleteImageBtn6.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteImageBtn6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.DeleteImageBtn6.Location = new System.Drawing.Point(582, 808);
            this.DeleteImageBtn6.Margin = new System.Windows.Forms.Padding(2);
            this.DeleteImageBtn6.Name = "DeleteImageBtn6";
            this.DeleteImageBtn6.Size = new System.Drawing.Size(96, 30);
            this.DeleteImageBtn6.TabIndex = 251;
            this.DeleteImageBtn6.Text = "删除图像";
            this.DeleteImageBtn6.UseVisualStyleBackColor = true;
            this.DeleteImageBtn6.Click += new System.EventHandler(this.DeleteImageBtn6_Click);
            // 
            // DeleteImageBtn5
            // 
            this.DeleteImageBtn5.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteImageBtn5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.DeleteImageBtn5.Location = new System.Drawing.Point(482, 808);
            this.DeleteImageBtn5.Margin = new System.Windows.Forms.Padding(2);
            this.DeleteImageBtn5.Name = "DeleteImageBtn5";
            this.DeleteImageBtn5.Size = new System.Drawing.Size(96, 30);
            this.DeleteImageBtn5.TabIndex = 250;
            this.DeleteImageBtn5.Text = "删除图像";
            this.DeleteImageBtn5.UseVisualStyleBackColor = true;
            this.DeleteImageBtn5.Click += new System.EventHandler(this.DeleteImageBtn5_Click);
            // 
            // DeleteImageBtn4
            // 
            this.DeleteImageBtn4.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteImageBtn4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.DeleteImageBtn4.Location = new System.Drawing.Point(382, 808);
            this.DeleteImageBtn4.Margin = new System.Windows.Forms.Padding(2);
            this.DeleteImageBtn4.Name = "DeleteImageBtn4";
            this.DeleteImageBtn4.Size = new System.Drawing.Size(96, 30);
            this.DeleteImageBtn4.TabIndex = 249;
            this.DeleteImageBtn4.Text = "删除图像";
            this.DeleteImageBtn4.UseVisualStyleBackColor = true;
            this.DeleteImageBtn4.Click += new System.EventHandler(this.DeleteImageBtn4_Click);
            // 
            // DeleteImageBtn3
            // 
            this.DeleteImageBtn3.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteImageBtn3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.DeleteImageBtn3.Location = new System.Drawing.Point(282, 808);
            this.DeleteImageBtn3.Margin = new System.Windows.Forms.Padding(2);
            this.DeleteImageBtn3.Name = "DeleteImageBtn3";
            this.DeleteImageBtn3.Size = new System.Drawing.Size(96, 30);
            this.DeleteImageBtn3.TabIndex = 248;
            this.DeleteImageBtn3.Text = "删除图像";
            this.DeleteImageBtn3.UseVisualStyleBackColor = true;
            this.DeleteImageBtn3.Click += new System.EventHandler(this.DeleteImageBtn3_Click);
            // 
            // DeleteImageBtn2
            // 
            this.DeleteImageBtn2.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteImageBtn2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.DeleteImageBtn2.Location = new System.Drawing.Point(182, 808);
            this.DeleteImageBtn2.Margin = new System.Windows.Forms.Padding(2);
            this.DeleteImageBtn2.Name = "DeleteImageBtn2";
            this.DeleteImageBtn2.Size = new System.Drawing.Size(96, 30);
            this.DeleteImageBtn2.TabIndex = 247;
            this.DeleteImageBtn2.Text = "删除图像";
            this.DeleteImageBtn2.UseVisualStyleBackColor = true;
            this.DeleteImageBtn2.Click += new System.EventHandler(this.DeleteImageBtn2_Click);
            // 
            // DeleteImageBtn1
            // 
            this.DeleteImageBtn1.Cursor = System.Windows.Forms.Cursors.Default;
            this.DeleteImageBtn1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.DeleteImageBtn1.Location = new System.Drawing.Point(82, 808);
            this.DeleteImageBtn1.Margin = new System.Windows.Forms.Padding(2);
            this.DeleteImageBtn1.Name = "DeleteImageBtn1";
            this.DeleteImageBtn1.Size = new System.Drawing.Size(96, 30);
            this.DeleteImageBtn1.TabIndex = 246;
            this.DeleteImageBtn1.Text = "删除图像";
            this.DeleteImageBtn1.UseVisualStyleBackColor = true;
            this.DeleteImageBtn1.Click += new System.EventHandler(this.DeleteImageBtn1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(497, 35);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 39);
            this.label1.TabIndex = 252;
            this.label1.Text = "图片倍率：";
            // 
            // RatioCombo
            // 
            this.RatioCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.RatioCombo.Font = new System.Drawing.Font("Microsoft Sans Serif", 17F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.RatioCombo.FormattingEnabled = true;
            this.RatioCombo.Items.AddRange(new object[] {
            "1",
            "1/4",
            "1/16",
            "1/64"});
            this.RatioCombo.Location = new System.Drawing.Point(688, 34);
            this.RatioCombo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.RatioCombo.Name = "RatioCombo";
            this.RatioCombo.Size = new System.Drawing.Size(132, 40);
            this.RatioCombo.TabIndex = 253;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 842);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 254;
            this.label2.Text = "行进方向";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 859);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(64, 17);
            this.label3.TabIndex = 255;
            this.label3.Text = "重合区：";
            // 
            // ol1text
            // 
            this.ol1text.Location = new System.Drawing.Point(82, 849);
            this.ol1text.Name = "ol1text";
            this.ol1text.Size = new System.Drawing.Size(96, 22);
            this.ol1text.TabIndex = 256;
            // 
            // ol2text
            // 
            this.ol2text.Location = new System.Drawing.Point(182, 849);
            this.ol2text.Name = "ol2text";
            this.ol2text.Size = new System.Drawing.Size(96, 22);
            this.ol2text.TabIndex = 257;
            // 
            // ol3text
            // 
            this.ol3text.Location = new System.Drawing.Point(282, 849);
            this.ol3text.Name = "ol3text";
            this.ol3text.Size = new System.Drawing.Size(96, 22);
            this.ol3text.TabIndex = 258;
            // 
            // ol4text
            // 
            this.ol4text.Location = new System.Drawing.Point(382, 849);
            this.ol4text.Name = "ol4text";
            this.ol4text.Size = new System.Drawing.Size(96, 22);
            this.ol4text.TabIndex = 259;
            // 
            // ol5text
            // 
            this.ol5text.Location = new System.Drawing.Point(482, 849);
            this.ol5text.Name = "ol5text";
            this.ol5text.Size = new System.Drawing.Size(96, 22);
            this.ol5text.TabIndex = 260;
            // 
            // ol6text
            // 
            this.ol6text.Location = new System.Drawing.Point(582, 849);
            this.ol6text.Name = "ol6text";
            this.ol6text.Size = new System.Drawing.Size(96, 22);
            this.ol6text.TabIndex = 261;
            // 
            // SelectMultiImageButton
            // 
            this.SelectMultiImageButton.Cursor = System.Windows.Forms.Cursors.Default;
            this.SelectMultiImageButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F);
            this.SelectMultiImageButton.Location = new System.Drawing.Point(974, 774);
            this.SelectMultiImageButton.Margin = new System.Windows.Forms.Padding(2);
            this.SelectMultiImageButton.Name = "SelectMultiImageButton";
            this.SelectMultiImageButton.Size = new System.Drawing.Size(101, 64);
            this.SelectMultiImageButton.TabIndex = 262;
            this.SelectMultiImageButton.Text = "一键选择图像";
            this.SelectMultiImageButton.UseVisualStyleBackColor = true;
            this.SelectMultiImageButton.Click += new System.EventHandler(this.SelectMultiImageButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1540, 1055);
            this.Controls.Add(this.SelectMultiImageButton);
            this.Controls.Add(this.ol6text);
            this.Controls.Add(this.ol5text);
            this.Controls.Add(this.ol4text);
            this.Controls.Add(this.ol3text);
            this.Controls.Add(this.ol2text);
            this.Controls.Add(this.ol1text);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.RatioCombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DeleteImageBtn6);
            this.Controls.Add(this.DeleteImageBtn5);
            this.Controls.Add(this.DeleteImageBtn4);
            this.Controls.Add(this.DeleteImageBtn3);
            this.Controls.Add(this.DeleteImageBtn2);
            this.Controls.Add(this.DeleteImageBtn1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.CalResultBox);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.PreviewBtn4);
            this.Controls.Add(this.PreviewBtn3);
            this.Controls.Add(this.PreviewBtn5);
            this.Controls.Add(this.PreviewBtn6);
            this.Controls.Add(this.SelectImageBtn6);
            this.Controls.Add(this.SelectImageBtn5);
            this.Controls.Add(this.SelectImageBtn4);
            this.Controls.Add(this.SelectImageBtn3);
            this.Controls.Add(this.SelectImageBtn2);
            this.Controls.Add(this.StartCal_Btn);
            this.Controls.Add(this.SAVE_Btn);
            this.Controls.Add(this.PreviewBtn2);
            this.Controls.Add(this.PreviewBtn1);
            this.Controls.Add(this.StaCombo);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.SelectImageBtn1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CurrentPicBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button SelectImageBtn1;
        private System.Windows.Forms.ComboBox StaCombo;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Button PreviewBtn2;
        private System.Windows.Forms.Button PreviewBtn1;
        private System.Windows.Forms.Button SAVE_Btn;
        private System.Windows.Forms.Button StartCal_Btn;
        private System.Windows.Forms.Button SelectImageBtn2;
        private System.Windows.Forms.Button SelectImageBtn3;
        private System.Windows.Forms.Button SelectImageBtn4;
        private System.Windows.Forms.Button SelectImageBtn5;
        private System.Windows.Forms.Button SelectImageBtn6;
        private System.Windows.Forms.Button PreviewBtn6;
        private System.Windows.Forms.Button PreviewBtn5;
        private System.Windows.Forms.Button PreviewBtn3;
        private System.Windows.Forms.Button PreviewBtn4;
        private System.Windows.Forms.TextBox CalResultBox;
        private System.Windows.Forms.PictureBox CurrentPicBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button DeleteImageBtn6;
        private System.Windows.Forms.Button DeleteImageBtn5;
        private System.Windows.Forms.Button DeleteImageBtn4;
        private System.Windows.Forms.Button DeleteImageBtn3;
        private System.Windows.Forms.Button DeleteImageBtn2;
        private System.Windows.Forms.Button DeleteImageBtn1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox RatioCombo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox ol1text;
        private System.Windows.Forms.TextBox ol2text;
        private System.Windows.Forms.TextBox ol3text;
        private System.Windows.Forms.TextBox ol4text;
        private System.Windows.Forms.TextBox ol5text;
        private System.Windows.Forms.TextBox ol6text;
        private System.Windows.Forms.Button SelectMultiImageButton;
    }
}

