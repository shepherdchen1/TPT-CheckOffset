namespace CheckOffset
{
    partial class For_Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pnlSetting = new System.Windows.Forms.Panel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.btnSettingLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelectFile = new System.Windows.Forms.Button();
            this.tbImgFile = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.rbtnRT45 = new System.Windows.Forms.RadioButton();
            this.rbtnLT45 = new System.Windows.Forms.RadioButton();
            this.rbtnVert = new System.Windows.Forms.RadioButton();
            this.rbtnHorz = new System.Windows.Forms.RadioButton();
            this.btnDeleteROI = new System.Windows.Forms.Button();
            this.chkNewInsp_Rect = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnFit = new System.Windows.Forms.Button();
            this.btn1X = new System.Windows.Forms.Button();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.chkSubPixel = new System.Windows.Forms.CheckBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numMaxValidPixel = new System.Windows.Forms.NumericUpDown();
            this.chkDisplayBinary = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnDetect = new System.Windows.Forms.Button();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.btnBmp2Array = new System.Windows.Forms.Button();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.labelReason = new System.Windows.Forms.Label();
            this.labelCheckResult = new System.Windows.Forms.Label();
            this.pnlFooter = new System.Windows.Forms.Panel();
            this.labelScale = new System.Windows.Forms.Label();
            this.labelGrayLevel = new System.Windows.Forms.Label();
            this.labelCursorPos = new System.Windows.Forms.Label();
            this.tabUser = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.openFileDialog_Img = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openFileDialog_Setting = new System.Windows.Forms.OpenFileDialog();
            this.pnlSetting.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxValidPixel)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            this.tabPage4.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlFooter.SuspendLayout();
            this.tabUser.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSetting
            // 
            this.pnlSetting.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pnlSetting.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pnlSetting.Controls.Add(this.tabControl1);
            this.pnlSetting.Dock = System.Windows.Forms.DockStyle.Right;
            this.pnlSetting.Location = new System.Drawing.Point(1232, 0);
            this.pnlSetting.Name = "pnlSetting";
            this.pnlSetting.Size = new System.Drawing.Size(386, 907);
            this.pnlSetting.TabIndex = 0;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Location = new System.Drawing.Point(15, 15);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.Padding = new System.Drawing.Point(20, 3);
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(358, 872);
            this.tabControl1.TabIndex = 15;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.groupBox4);
            this.tabPage1.Controls.Add(this.groupBox3);
            this.tabPage1.Controls.Add(this.groupBox2);
            this.tabPage1.Controls.Add(this.groupBox1);
            this.tabPage1.Location = new System.Drawing.Point(4, 32);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(350, 836);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "影像";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.btnSettingLoad);
            this.groupBox4.Controls.Add(this.btnSave);
            this.groupBox4.Location = new System.Drawing.Point(12, 8);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(308, 93);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Setting";
            // 
            // btnSettingLoad
            // 
            this.btnSettingLoad.Location = new System.Drawing.Point(21, 37);
            this.btnSettingLoad.Margin = new System.Windows.Forms.Padding(5);
            this.btnSettingLoad.Name = "btnSettingLoad";
            this.btnSettingLoad.Size = new System.Drawing.Size(106, 35);
            this.btnSettingLoad.TabIndex = 7;
            this.btnSettingLoad.Text = "Load";
            this.btnSettingLoad.UseVisualStyleBackColor = true;
            this.btnSettingLoad.Click += new System.EventHandler(this.btnSettingLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(160, 37);
            this.btnSave.Margin = new System.Windows.Forms.Padding(5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(106, 35);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.label1);
            this.groupBox3.Controls.Add(this.btnSelectFile);
            this.groupBox3.Controls.Add(this.tbImgFile);
            this.groupBox3.Location = new System.Drawing.Point(12, 117);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(308, 121);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "影像";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(16, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Select File";
            // 
            // btnSelectFile
            // 
            this.btnSelectFile.Location = new System.Drawing.Point(200, 29);
            this.btnSelectFile.Name = "btnSelectFile";
            this.btnSelectFile.Size = new System.Drawing.Size(79, 34);
            this.btnSelectFile.TabIndex = 2;
            this.btnSelectFile.Text = "...";
            this.btnSelectFile.UseVisualStyleBackColor = true;
            this.btnSelectFile.Click += new System.EventHandler(this.btnSelectFile_Click);
            // 
            // tbImgFile
            // 
            this.tbImgFile.Location = new System.Drawing.Point(15, 72);
            this.tbImgFile.Name = "tbImgFile";
            this.tbImgFile.Size = new System.Drawing.Size(264, 30);
            this.tbImgFile.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.rbtnRT45);
            this.groupBox2.Controls.Add(this.rbtnLT45);
            this.groupBox2.Controls.Add(this.rbtnVert);
            this.groupBox2.Controls.Add(this.rbtnHorz);
            this.groupBox2.Controls.Add(this.btnDeleteROI);
            this.groupBox2.Controls.Add(this.chkNewInsp_Rect);
            this.groupBox2.Location = new System.Drawing.Point(12, 376);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(308, 173);
            this.groupBox2.TabIndex = 14;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "檢測區";
            // 
            // rbtnRT45
            // 
            this.rbtnRT45.AutoSize = true;
            this.rbtnRT45.Location = new System.Drawing.Point(95, 62);
            this.rbtnRT45.Name = "rbtnRT45";
            this.rbtnRT45.Size = new System.Drawing.Size(77, 27);
            this.rbtnRT45.TabIndex = 17;
            this.rbtnRT45.Text = "RT45";
            this.rbtnRT45.UseVisualStyleBackColor = true;
            // 
            // rbtnLT45
            // 
            this.rbtnLT45.AutoSize = true;
            this.rbtnLT45.Location = new System.Drawing.Point(95, 29);
            this.rbtnLT45.Name = "rbtnLT45";
            this.rbtnLT45.Size = new System.Drawing.Size(74, 27);
            this.rbtnLT45.TabIndex = 16;
            this.rbtnLT45.Text = "LT45";
            this.rbtnLT45.UseVisualStyleBackColor = true;
            // 
            // rbtnVert
            // 
            this.rbtnVert.AutoSize = true;
            this.rbtnVert.Location = new System.Drawing.Point(18, 62);
            this.rbtnVert.Name = "rbtnVert";
            this.rbtnVert.Size = new System.Drawing.Size(71, 27);
            this.rbtnVert.TabIndex = 15;
            this.rbtnVert.Text = "垂直";
            this.rbtnVert.UseVisualStyleBackColor = true;
            // 
            // rbtnHorz
            // 
            this.rbtnHorz.AutoSize = true;
            this.rbtnHorz.Checked = true;
            this.rbtnHorz.Location = new System.Drawing.Point(18, 29);
            this.rbtnHorz.Name = "rbtnHorz";
            this.rbtnHorz.Size = new System.Drawing.Size(71, 27);
            this.rbtnHorz.TabIndex = 14;
            this.rbtnHorz.TabStop = true;
            this.rbtnHorz.Text = "水平";
            this.rbtnHorz.UseVisualStyleBackColor = true;
            // 
            // btnDeleteROI
            // 
            this.btnDeleteROI.Location = new System.Drawing.Point(147, 109);
            this.btnDeleteROI.Margin = new System.Windows.Forms.Padding(5);
            this.btnDeleteROI.Name = "btnDeleteROI";
            this.btnDeleteROI.Size = new System.Drawing.Size(129, 35);
            this.btnDeleteROI.TabIndex = 13;
            this.btnDeleteROI.Text = "刪除";
            this.btnDeleteROI.UseVisualStyleBackColor = true;
            this.btnDeleteROI.Click += new System.EventHandler(this.btnDeleteROI_Click);
            // 
            // chkNewInsp_Rect
            // 
            this.chkNewInsp_Rect.Appearance = System.Windows.Forms.Appearance.Button;
            this.chkNewInsp_Rect.AutoSize = true;
            this.chkNewInsp_Rect.FlatAppearance.CheckedBackColor = System.Drawing.Color.Blue;
            this.chkNewInsp_Rect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.chkNewInsp_Rect.Location = new System.Drawing.Point(184, 42);
            this.chkNewInsp_Rect.Name = "chkNewInsp_Rect";
            this.chkNewInsp_Rect.Size = new System.Drawing.Size(92, 33);
            this.chkNewInsp_Rect.TabIndex = 4;
            this.chkNewInsp_Rect.Text = "新增矩形";
            this.chkNewInsp_Rect.UseVisualStyleBackColor = true;
            this.chkNewInsp_Rect.CheckedChanged += new System.EventHandler(this.chkNewInsp_Rect_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btnFit);
            this.groupBox1.Controls.Add(this.btn1X);
            this.groupBox1.Location = new System.Drawing.Point(12, 244);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(308, 79);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Display";
            // 
            // btnFit
            // 
            this.btnFit.Location = new System.Drawing.Point(21, 33);
            this.btnFit.Name = "btnFit";
            this.btnFit.Size = new System.Drawing.Size(112, 34);
            this.btnFit.TabIndex = 11;
            this.btnFit.Text = "Fit";
            this.btnFit.UseVisualStyleBackColor = true;
            this.btnFit.Click += new System.EventHandler(this.btnFit_Click);
            // 
            // btn1X
            // 
            this.btn1X.Location = new System.Drawing.Point(160, 33);
            this.btn1X.Name = "btn1X";
            this.btn1X.Size = new System.Drawing.Size(112, 34);
            this.btn1X.TabIndex = 10;
            this.btn1X.Text = "1X";
            this.btn1X.UseVisualStyleBackColor = true;
            this.btn1X.Click += new System.EventHandler(this.btn1X_Click);
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.chkSubPixel);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.numMaxValidPixel);
            this.tabPage3.Controls.Add(this.chkDisplayBinary);
            this.tabPage3.Controls.Add(this.label5);
            this.tabPage3.Controls.Add(this.label6);
            this.tabPage3.Controls.Add(this.btnDetect);
            this.tabPage3.Controls.Add(this.numThreshold);
            this.tabPage3.Location = new System.Drawing.Point(4, 32);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(350, 836);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "過濾";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // chkSubPixel
            // 
            this.chkSubPixel.AutoSize = true;
            this.chkSubPixel.Location = new System.Drawing.Point(16, 287);
            this.chkSubPixel.Name = "chkSubPixel";
            this.chkSubPixel.Size = new System.Drawing.Size(108, 27);
            this.chkSubPixel.TabIndex = 17;
            this.chkSubPixel.Text = "SubPixel";
            this.chkSubPixel.UseVisualStyleBackColor = true;
            this.chkSubPixel.CheckedChanged += new System.EventHandler(this.chkSubPixel_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(11, 114);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(139, 23);
            this.label7.TabIndex = 15;
            this.label7.Text = "Max Valid pixel";
            // 
            // numMaxValidPixel
            // 
            this.numMaxValidPixel.Location = new System.Drawing.Point(156, 107);
            this.numMaxValidPixel.Name = "numMaxValidPixel";
            this.numMaxValidPixel.Size = new System.Drawing.Size(112, 30);
            this.numMaxValidPixel.TabIndex = 16;
            this.numMaxValidPixel.Value = new decimal(new int[] {
            4,
            0,
            0,
            0});
            // 
            // chkDisplayBinary
            // 
            this.chkDisplayBinary.AutoSize = true;
            this.chkDisplayBinary.Location = new System.Drawing.Point(16, 26);
            this.chkDisplayBinary.Name = "chkDisplayBinary";
            this.chkDisplayBinary.Size = new System.Drawing.Size(140, 27);
            this.chkDisplayBinary.TabIndex = 14;
            this.chkDisplayBinary.Text = "Show binary";
            this.chkDisplayBinary.UseVisualStyleBackColor = true;
            this.chkDisplayBinary.CheckedChanged += new System.EventHandler(this.chkDisplayBinary_CheckedChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(16, 179);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 23);
            this.label5.TabIndex = 10;
            this.label5.Text = "Detect";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(11, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 23);
            this.label6.TabIndex = 11;
            this.label6.Text = "Threshold";
            // 
            // btnDetect
            // 
            this.btnDetect.Location = new System.Drawing.Point(156, 173);
            this.btnDetect.Margin = new System.Windows.Forms.Padding(5);
            this.btnDetect.Name = "btnDetect";
            this.btnDetect.Size = new System.Drawing.Size(112, 35);
            this.btnDetect.TabIndex = 9;
            this.btnDetect.Text = "Detect";
            this.btnDetect.UseVisualStyleBackColor = true;
            this.btnDetect.Click += new System.EventHandler(this.btnDetect_Click);
            // 
            // numThreshold
            // 
            this.numThreshold.Location = new System.Drawing.Point(156, 63);
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(112, 30);
            this.numThreshold.TabIndex = 13;
            this.numThreshold.Value = new decimal(new int[] {
            40,
            0,
            0,
            0});
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.btnBmp2Array);
            this.tabPage4.Location = new System.Drawing.Point(4, 32);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Size = new System.Drawing.Size(350, 836);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "測試";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // btnBmp2Array
            // 
            this.btnBmp2Array.Location = new System.Drawing.Point(48, 70);
            this.btnBmp2Array.Name = "btnBmp2Array";
            this.btnBmp2Array.Size = new System.Drawing.Size(173, 34);
            this.btnBmp2Array.TabIndex = 0;
            this.btnBmp2Array.Text = "bmp to 2d array";
            this.btnBmp2Array.UseVisualStyleBackColor = true;
            this.btnBmp2Array.Click += new System.EventHandler(this.btnBmp2Array_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pnlHeader.Controls.Add(this.labelReason);
            this.pnlHeader.Controls.Add(this.labelCheckResult);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(1232, 52);
            this.pnlHeader.TabIndex = 2;
            // 
            // labelReason
            // 
            this.labelReason.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelReason.AutoSize = true;
            this.labelReason.Location = new System.Drawing.Point(991, 17);
            this.labelReason.Name = "labelReason";
            this.labelReason.Size = new System.Drawing.Size(72, 23);
            this.labelReason.TabIndex = 1;
            this.labelReason.Text = "Reason";
            // 
            // labelCheckResult
            // 
            this.labelCheckResult.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCheckResult.AutoSize = true;
            this.labelCheckResult.Font = new System.Drawing.Font("Microsoft JhengHei UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.labelCheckResult.Location = new System.Drawing.Point(526, 10);
            this.labelCheckResult.Name = "labelCheckResult";
            this.labelCheckResult.Size = new System.Drawing.Size(83, 30);
            this.labelCheckResult.TabIndex = 0;
            this.labelCheckResult.Text = "Result";
            // 
            // pnlFooter
            // 
            this.pnlFooter.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.pnlFooter.Controls.Add(this.labelScale);
            this.pnlFooter.Controls.Add(this.labelGrayLevel);
            this.pnlFooter.Controls.Add(this.labelCursorPos);
            this.pnlFooter.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlFooter.Location = new System.Drawing.Point(0, 825);
            this.pnlFooter.Name = "pnlFooter";
            this.pnlFooter.Size = new System.Drawing.Size(1232, 82);
            this.pnlFooter.TabIndex = 3;
            // 
            // labelScale
            // 
            this.labelScale.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelScale.AutoSize = true;
            this.labelScale.Location = new System.Drawing.Point(958, 47);
            this.labelScale.Name = "labelScale";
            this.labelScale.Size = new System.Drawing.Size(31, 23);
            this.labelScale.TabIndex = 2;
            this.labelScale.Text = "1X";
            // 
            // labelGrayLevel
            // 
            this.labelGrayLevel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelGrayLevel.AutoSize = true;
            this.labelGrayLevel.Location = new System.Drawing.Point(1069, 14);
            this.labelGrayLevel.Name = "labelGrayLevel";
            this.labelGrayLevel.Size = new System.Drawing.Size(20, 23);
            this.labelGrayLevel.TabIndex = 1;
            this.labelGrayLevel.Text = "0";
            // 
            // labelCursorPos
            // 
            this.labelCursorPos.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelCursorPos.AutoSize = true;
            this.labelCursorPos.Location = new System.Drawing.Point(949, 14);
            this.labelCursorPos.Name = "labelCursorPos";
            this.labelCursorPos.Size = new System.Drawing.Size(46, 23);
            this.labelCursorPos.TabIndex = 0;
            this.labelCursorPos.Text = "(0,0)";
            // 
            // tabUser
            // 
            this.tabUser.Controls.Add(this.tabPage2);
            this.tabUser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabUser.Location = new System.Drawing.Point(0, 52);
            this.tabUser.Name = "tabUser";
            this.tabUser.SelectedIndex = 0;
            this.tabUser.Size = new System.Drawing.Size(1232, 773);
            this.tabUser.TabIndex = 4;
            this.tabUser.SelectedIndexChanged += new System.EventHandler(this.tabUser_SelectedIndexChanged);
            // 
            // tabPage2
            // 
            this.tabPage2.Location = new System.Drawing.Point(4, 32);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(1224, 737);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "tabPage";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // openFileDialog_Img
            // 
            this.openFileDialog_Img.FileName = "openFileDialogFile";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "json";
            this.saveFileDialog.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            // 
            // openFileDialog_Setting
            // 
            this.openFileDialog_Setting.FileName = "openFileDialog1";
            this.openFileDialog_Setting.Filter = "json files (*.json)|*.json|All files (*.*)|*.*";
            // 
            // For_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1618, 907);
            this.Controls.Add(this.tabUser);
            this.Controls.Add(this.pnlFooter);
            this.Controls.Add(this.pnlHeader);
            this.Controls.Add(this.pnlSetting);
            this.Name = "For_Main";
            this.Text = "Form1";
            this.pnlSetting.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMaxValidPixel)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            this.tabPage4.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFooter.ResumeLayout(false);
            this.pnlFooter.PerformLayout();
            this.tabUser.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel pnlSetting;
        private Button btnSelectFile;
        private TextBox tbImgFile;
        private Label label1;
        private Panel pnlHeader;
        private Panel pnlFooter;
        private TabControl tabUser;
        private TabPage tabPage2;
        private OpenFileDialog openFileDialog_Img;
        private CheckBox chkNewInsp_Rect;
        private Button btnSave;
        private SaveFileDialog saveFileDialog;
        private Button btnSettingLoad;
        private OpenFileDialog openFileDialog_Setting;
        private Label label5;
        private Button btnDetect;
        private NumericUpDown numThreshold;
        private Label label6;
        private Label labelGrayLevel;
        private Label labelCursorPos;
        private CheckBox chkDisplayBinary;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage3;
        private Label label7;
        private NumericUpDown numMaxValidPixel;
        private Label labelCheckResult;
        private Label labelReason;
        private Label labelScale;
        private Button btn1X;
        private Button btnFit;
        private GroupBox groupBox1;
        private Button btnDeleteROI;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private RadioButton rbtnRT45;
        private RadioButton rbtnLT45;
        private RadioButton rbtnVert;
        private RadioButton rbtnHorz;
        private GroupBox groupBox4;
        private CheckBox chkSubPixel;
        private TabPage tabPage4;
        private Button btnBmp2Array;
    }
}