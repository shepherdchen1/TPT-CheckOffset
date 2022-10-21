using TN;

namespace CheckOffset
{
    partial class UserCtrl_Image
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserCtrl_Image));
            this.ll_Test = new System.Windows.Forms.Label();
            this.pb_Image = new TNControls.TNPictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb_Image)).BeginInit();
            this.SuspendLayout();
            // 
            // ll_Test
            // 
            this.ll_Test.AutoSize = true;
            this.ll_Test.Location = new System.Drawing.Point(5, 0);
            this.ll_Test.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.ll_Test.Name = "ll_Test";
            this.ll_Test.Size = new System.Drawing.Size(61, 23);
            this.ll_Test.TabIndex = 1;
            this.ll_Test.Text = "label1";
            this.ll_Test.Visible = false;
            // 
            // pb_Image
            // 
            this.pb_Image.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pb_Image.Image_Bmp = null;
            this.pb_Image.Image_Offset = new System.Drawing.Point(0, 0);
            this.pb_Image.Image_Scale = 1F;
            this.pb_Image.Location = new System.Drawing.Point(0, 0);
            this.pb_Image.Name = "tnPictureBox1";
            this.pb_Image.Size = new System.Drawing.Size(900, 547);
            this.pb_Image.TabIndex = 2;
            this.pb_Image.TabStop = false;
            // 
            // UserCtrl_Image
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.ll_Test);
            this.Controls.Add(this.pb_Image);
            this.Name = "UserCtrl_Image";
            this.Size = new System.Drawing.Size(900, 547);
            ((System.ComponentModel.ISupportInitialize)(this.pb_Image)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        public Label ll_Test;
        public TNControls.TNPictureBox pb_Image;
    }
}
