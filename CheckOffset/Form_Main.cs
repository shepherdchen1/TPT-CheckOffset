using Newtonsoft.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Drawing;

using System.IO;
using CheckOffset;
using TN.ImageTools;
using static System.Net.Mime.MediaTypeNames;
using TN.Tools.Debug;
//using CheckOffset.Controls;
using TNControls;

namespace CheckOffset
{
    public partial class Form_Main : Form
    {
        public Form_Main()
        {
            InitializeComponent();

            Init_Data();
        }


        private UserCtrl_Image m_userctrl_image = null; 

        private void Init_Data()
        {
            m_userctrl_image = new UserCtrl_Image();

            tabUser.TabPages.Clear();

            //////////////////////////////////////
            // manual add tab...
            TabPage new_tab_page = new TabPage("Image");
            m_userctrl_image.Dock = DockStyle.Fill;
            new_tab_page.Controls.Add(m_userctrl_image);
            tabUser.TabPages.Add(new_tab_page);

            m_userctrl_image.Report_GrayLevel_Gray += Event_Update_Gray_Level;
            m_userctrl_image.Query_Editing_Mode    += Query_Editing_Mode;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            DialogResult dlg_res = openFileDialog_Img.ShowDialog();
            if (dlg_res != DialogResult.OK)
                return;

            tbImgFile.Text = openFileDialog_Img.FileName;
            m_userctrl_image.Image = (Bitmap) System.Drawing.Image.FromFile(tbImgFile.Text);
            m_userctrl_image.pb_Image.ZoomFit();

            m_userctrl_image.ll_Test.Text = tbImgFile.Text;
        }

        private void chkNewInsp_Rect_CheckedChanged(object sender, EventArgs e)
        {
            if (null == tnGlobal.Detect_Pos)
                return;

            //aaa
            if (chkNewInsp_Rect.Checked)
            {
                m_userctrl_image.pb_Image.Editing_Ctrl = null;

                /////////////////////////////////////////////////
                /// add exist rect.
                if (null != m_userctrl_image.User_Ctrls)
                {
                    m_userctrl_image.User_Ctrls.Clear();
                    foreach (Rectangle cur_rect in tnGlobal.Detect_Pos.Detect_Rects)
                    {
                        TNUserCtrl_Rect exist_added_rect = new TNUserCtrl_Rect();
                        TNPictureBox tn_pb = m_userctrl_image.pb_Image as TNPictureBox;
                        exist_added_rect.Editing_Rect = cur_rect;
                        m_userctrl_image.User_Ctrls.Add(exist_added_rect);
                    }
                }

                /////////////////////////////////////////////////
                // add new editing rect.
                TNUserCtrl_Rect new_added_rect = new TNUserCtrl_Rect();
                m_userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Editing_Rect = new Rectangle(0, 0, 100, 100);
            }
            else
            {
                // 新增完畢
                if (null != m_userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNUserCtrl_Rect editing_rect = (TNUserCtrl_Rect)m_userctrl_image.pb_Image.Editing_Ctrl;
                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Editing_Rect.Width > min_roi_valid_size && editing_rect.Editing_Rect.Height > min_roi_valid_size)
                        tnGlobal.Detect_Pos.Detect_Rects.Add( editing_rect.Editing_Rect);

                    m_userctrl_image.Apply_GlobalSetting_To_Ctrls();
                }

                m_userctrl_image.pb_Image.Editing_Ctrl = null;
            }
        }

        private void tabUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabUser.SelectedTab.Name == tabpage_Image.Name)
            //{
            //    m_userctrl_image.Visible = true;
            //}
        }

        private void btnSettingLoad_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != openFileDialog_Setting.ShowDialog())
                return;

            string jsonString = File.ReadAllText(openFileDialog_Setting.FileName);
            tnGlobal.Detect_Pos = Newtonsoft.Json.JsonConvert.DeserializeObject<tnGlobal.Detect_Pos_Info>(jsonString);

            m_userctrl_image.Apply_GlobalSetting_To_Ctrls();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != saveFileDialog.ShowDialog())
                return;

            if (null == tnGlobal.Detect_Pos)
                return;

            string jsonString = System.Text.Json.JsonSerializer.Serialize<tnGlobal.Detect_Pos_Info>(tnGlobal.Detect_Pos); // tnGlobal.Setting);


            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(tnGlobal.Detect_Pos);
            //if (!File.Exists(path))
            {
                // Create a file to write to.
                File.WriteAllText(saveFileDialog.FileName, jsonString );

                //using (StreamWriter sw = File.CreateText(saveFileDialog.FileName))
                //{
                //    sw.WriteLine("Hello");
                //    sw.WriteLine("And");
                //    sw.WriteLine("Welcome");
                //}
            }

        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = (Bitmap) System.Drawing.Image.FromFile(tbImgFile.Text);

                bool check_res = true;
                if (null != tnGlobal.Detect_Pos)
                {
                    //bool first_white = false;
                    int max_valid_gap = (int)numMaxValidPixel.Value;
                    foreach( Rectangle chk_rect in tnGlobal.Detect_Pos.Detect_Rects )
                    {
                        //////////////////////////////////
                        // check X dir.
                        for (int y = chk_rect.Top; y < chk_rect.Bottom; y++)
                        {
                            Get_X_Gap(bmp, chk_rect.X, chk_rect.X + chk_rect.Width, y, out float edge_pos_left, out float edge_pos_right);

                            // no edge found.
                            if (edge_pos_left < 0 || edge_pos_right < 0)
                                continue;

                            int gap_size = (int)edge_pos_right - (int)edge_pos_left + 1;
                            if (gap_size < max_valid_gap)
                                continue;

                            labelCheckResult.Text = $"NG";
                            labelReason.Text = $"X:{edge_pos_left}~{edge_pos_right}, Y:{y} ";
                            check_res = false;
                            break;
                        }

                        if (!check_res)
                            break;

                        //////////////////////////////////
                        // check Y dir.
                        for (int x = chk_rect.Left; x < chk_rect.Right; x++)
                        {
                            Get_Y_Gap(bmp, x, chk_rect.Y, chk_rect.Y + chk_rect.Height, out float edge_pos_top, out float edge_pos_bottom);

                            // no edge found.
                            if (edge_pos_top < 0 || edge_pos_bottom < 0)
                                continue;

                            int gap_size = (int)edge_pos_bottom - (int)edge_pos_top + 1;
                            if (gap_size < max_valid_gap)
                                continue;

                            labelCheckResult.Text = $"NG";
                            labelReason.Text = $"X:{x}, Y:{edge_pos_top}~{edge_pos_bottom}";
                            check_res = false;
                            break;
                        }

                        if (!check_res)
                            break;
                    }

                    if (check_res)
                    {
                        labelCheckResult.Text = $"OK";
                        labelReason.Text = $"";
                    }
                }
            }
            catch(Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                   , string.Format("Exception catched: error:{0}", ex.Message));
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private bool Get_X_Gap(Bitmap bmp, int x_start, int x_end, int y, out float edge_pos_left, out float edge_pos_right)
        {
            edge_pos_left = -1;
            edge_pos_right = -1;

            try
            {
                bool first_white = false;
                // chk_rect.X -.  chk_rect.X + chk_rect.Width
                EdgeDetect.Find_Edge_X(bmp, y, x_start, x_end, first_white, (int) numThreshold.Value, out edge_pos_left);
                // no edge found.
                if (edge_pos_left < 0)
                    return true;

                // chk_rect.X + chk_rect.Width -. chk_rect.X
                EdgeDetect.Find_Edge_X(bmp, y, x_end, x_start, first_white, (int)numThreshold.Value, out edge_pos_right);
                // no edge found.
                if (edge_pos_right < 0)
                    return true;

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                   , string.Format("Exception catched: error:{0}", ex.Message));
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        private bool Get_Y_Gap(Bitmap bmp, int x, int y_start, int y_end, out float edge_pos_top, out float edge_pos_bottom)
        {
            edge_pos_top = -1;
            edge_pos_bottom = -1;

            try
            {
                bool first_white = false;
                // chk_rect.X -.  chk_rect.X + chk_rect.Width
                EdgeDetect.Find_Edge_Y(bmp, x, y_start, y_end, first_white, (int)numThreshold.Value, out edge_pos_top);
                // no edge found.
                if (edge_pos_top < 0)
                    return true;

                // chk_rect.X + chk_rect.Width -. chk_rect.X
                EdgeDetect.Find_Edge_X(bmp, x, y_end, y_start, first_white, (int)numThreshold.Value, out edge_pos_bottom);
                // no edge found.
                if (edge_pos_bottom < 0)
                    return true;

                return true;
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                   , string.Format("Exception catched: error:{0}", ex.Message));
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

            return false;
        }

        //public int Event_Update_Gray_Level(int cur_x, int cur_y, int gray_level)
        public int Event_Update_Gray_Level(Report_Display_Info report_display_info)
        {
            labelCursorPos.Text = $"{report_display_info.Pos.X}, {report_display_info.Pos.Y}";
            labelGrayLevel.Text = $"{report_display_info.Gray_Level}";
            labelScale.Text     = $"{report_display_info.Scale}X"; 

            return -1;
        }

        private void chkDisplayBinary_CheckedChanged(object sender, EventArgs e)
        {
            if (!File.Exists(tbImgFile.Text))
                return;

            Bitmap org_bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            if ( chkDisplayBinary.Checked )
            {
                Image_Binary.Binary_Image(org_bmp, (int) numThreshold.Value, out Bitmap? dest_bmp);

                m_userctrl_image.Image = dest_bmp;
            }
            else
            {
                m_userctrl_image.Image = org_bmp;
            }
        }

        private void btn1X_Click(object sender, EventArgs e)
        {
            m_userctrl_image.Image = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            m_userctrl_image.Image_Scale = 1.0f;
            m_userctrl_image.Offset = new Point(0, 0);

            m_userctrl_image.Refresh();
        }

        private void btnFit_Click(object sender, EventArgs e)
        {
            m_userctrl_image.Image = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            m_userctrl_image.pb_Image.ZoomFit();
        }

        private void btnDeleteROI_Click(object sender, EventArgs e)
        {
            if (m_userctrl_image.Editing_Ctrl == null)
                return;

            m_userctrl_image.Delete_Editing_ROI(m_userctrl_image.Editing_Ctrl);
            m_userctrl_image.Refresh();
        }

        Editing_Mode Query_Editing_Mode()
        {
            if (chkNewInsp_Rect.Checked)
                return Editing_Mode.EDT_New_ROI;

            if (null != m_userctrl_image.Editing_Ctrl)
                return Editing_Mode.EDT_Editing_ROI;

            return Editing_Mode.EDT_None;
        }

    } // end of     public partial class Form_Main : Form
} // end of namespace CheckOffset