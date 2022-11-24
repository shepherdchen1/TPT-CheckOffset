namespace CheckOffset.Controls
{
    partial class TNHistogram
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelPixelNum = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelGrayLevel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pbHistogram = new System.Windows.Forms.PictureBox();
            this.labelNormalDistri = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHistogram)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelNormalDistri);
            this.panel1.Controls.Add(this.labelPixelNum);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.labelGrayLevel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 551);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(965, 62);
            this.panel1.TabIndex = 1;
            // 
            // labelPixelNum
            // 
            this.labelPixelNum.AutoSize = true;
            this.labelPixelNum.Location = new System.Drawing.Point(350, 19);
            this.labelPixelNum.Name = "labelPixelNum";
            this.labelPixelNum.Size = new System.Drawing.Size(20, 23);
            this.labelPixelNum.TabIndex = 3;
            this.labelPixelNum.Text = "0";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 19);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(57, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Num:";
            // 
            // labelGrayLevel
            // 
            this.labelGrayLevel.AutoSize = true;
            this.labelGrayLevel.Location = new System.Drawing.Point(181, 19);
            this.labelGrayLevel.Name = "labelGrayLevel";
            this.labelGrayLevel.Size = new System.Drawing.Size(20, 23);
            this.labelGrayLevel.TabIndex = 1;
            this.labelGrayLevel.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(58, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(101, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Gray Level:";
            // 
            // pbHistogram
            // 
            this.pbHistogram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pbHistogram.Location = new System.Drawing.Point(0, 0);
            this.pbHistogram.Name = "pbHistogram";
            this.pbHistogram.Size = new System.Drawing.Size(965, 551);
            this.pbHistogram.TabIndex = 2;
            this.pbHistogram.TabStop = false;
            this.pbHistogram.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pbHistogram_MouseMove);
            // 
            // labelNormalDistri
            // 
            this.labelNormalDistri.AutoSize = true;
            this.labelNormalDistri.Location = new System.Drawing.Point(561, 19);
            this.labelNormalDistri.Name = "labelNormalDistri";
            this.labelNormalDistri.Size = new System.Drawing.Size(43, 23);
            this.labelNormalDistri.TabIndex = 4;
            this.labelNormalDistri.Text = "0~0";
            // 
            // TNHistogram
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(965, 613);
            this.Controls.Add(this.pbHistogram);
            this.Controls.Add(this.panel1);
            this.Name = "TNHistogram";
            this.Text = "TNHistogram";
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TNHistogram_Paint);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbHistogram)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label labelPixelNum;
        private Label label2;
        private Label labelGrayLevel;
        private Label label1;
        private PictureBox pbHistogram;
        private Label labelNormalDistri;
    }
}