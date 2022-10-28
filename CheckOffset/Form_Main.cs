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
using TN.Insp_Param;

using OpenSource;

namespace CheckOffset
{
    public partial class For_Main : Form
    {
        public For_Main()
        {
            InitializeComponent();

            Init_Data();
        }


        private UserCtrl_Image _userctrl_image = new UserCtrl_Image(); 

        private void Init_Data()
        {
            //_userctrl_image = new UserCtrl_Image();

            tabUser.TabPages.Clear();

            //////////////////////////////////////
            // manual add tab...
            TabPage new_tab_page = new TabPage("Image");
            _userctrl_image.Dock = DockStyle.Fill;
            new_tab_page.Controls.Add(_userctrl_image);
            tabUser.TabPages.Add(new_tab_page);

            _userctrl_image.Report_GrayLevel_Gray += Event_Update_Gray_Level;
            _userctrl_image.Query_Editing_Mode    += Query_Editing_Mode;
        }

        private void btnSelectFile_Click(object sender, EventArgs e)
        {
            DialogResult dlg_res = openFileDialog_Img.ShowDialog();
            if (dlg_res != DialogResult.OK)
                return;

            if (null != _userctrl_image.Image )
                _userctrl_image.Image.Dispose();

            tbImgFile.Text = openFileDialog_Img.FileName;
            _userctrl_image.Image = (Bitmap) System.Drawing.Image.FromFile(tbImgFile.Text);
            _userctrl_image.Editing_Ctrl = null;
            _userctrl_image.User_Ctrls.Clear();
            _userctrl_image.Cache_Ctrl.Clear();
            _userctrl_image.pb_Image.ZoomFit();

            _userctrl_image.ll_Test.Text = tbImgFile.Text;
        }

        private void chkNewInsp_Rect_CheckedChanged(object sender, EventArgs e)
        {
            if (null == tnGlobal.Detect_Infos)
                return;

            //aaa
            if (chkNewInsp_Rect.Checked)
            {
                _userctrl_image.pb_Image.Editing_Ctrl = null;

                /////////////////////////////////////////////////
                /// add exist rect.
                if (null != _userctrl_image.User_Ctrls)
                {
                    _userctrl_image.User_Ctrls.Clear();
                    foreach (DS_Detect_Info cur_detect_infos in tnGlobal.Detect_Infos)
                    {
                        TNCustCtrl_Rect exist_added_rect = new TNCustCtrl_Rect();
                        TNPictureBox tn_pb = _userctrl_image.pb_Image as TNPictureBox;
                        exist_added_rect.Pos_Info.Editing_Rect = cur_detect_infos.Detect_Rect;
                        exist_added_rect.Insp_param = cur_detect_infos.Detect_Insp_param;
                        _userctrl_image.User_Ctrls.Add(exist_added_rect);
                    }
                }

                /////////////////////////////////////////////////
                // add new editing rect.
                TNCustCtrl_Rect new_added_rect = new TNCustCtrl_Rect();
                _userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Pos_Info.Editing_Rect = new Rectangle(0, 0, 100, 100);
                DS_Insp_Param new_insp_param = new DS_Insp_Param();
                new_insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                new_added_rect.Insp_param = new_insp_param;
            }
            else
            {
                // 新增完畢
                if (null != _userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

                    DS_Insp_Param new_insp_param = new DS_Insp_Param();
                    new_insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                    editing_rect.Insp_param = new_insp_param;

                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
                    {
                        DS_Detect_Info new_defect_info = new DS_Detect_Info();
                        new_defect_info.Detect_Rect = editing_rect.Pos_Info.Editing_Rect;
                        new_defect_info.Detect_Insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                        tnGlobal.Detect_Infos.Add(new_defect_info);
                    }

                    _userctrl_image.Apply_GlobalSetting_To_Ctrls();
                }

                _userctrl_image.pb_Image.Editing_Ctrl = null;
            }
        }

        private void tabUser_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (tabUser.SelectedTab.Name == tabpage_Image.Name)
            //{
            //    _userctrl_image.Visible = true;
            //}
        }

        private void btnSettingLoad_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != openFileDialog_Setting.ShowDialog())
                return;

            string jsonString = File.ReadAllText(openFileDialog_Setting.FileName);
            tnGlobal.Detect_Infos = Newtonsoft.Json.JsonConvert.DeserializeObject<List<DS_Detect_Info>>(jsonString);

            _userctrl_image.Apply_GlobalSetting_To_Ctrls();
            _userctrl_image.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != saveFileDialog.ShowDialog())
                return;

            if (null == tnGlobal.Detect_Infos)
                return;

            string jsonString = System.Text.Json.JsonSerializer.Serialize<List<DS_Detect_Info>>(tnGlobal.Detect_Infos); // tnGlobal.Setting);


            jsonString = Newtonsoft.Json.JsonConvert.SerializeObject(tnGlobal.Detect_Infos);
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

                _userctrl_image.Cache_Ctrl.Clear();

                bool check_res = true;
                if (null != tnGlobal.Detect_Infos)
                {
                    //bool first_white = false;
                    int max_valid_gap = (int)numMaxValidPixel.Value;
                    foreach (Control ctrl_defect_info in _userctrl_image.User_Ctrls)
                    {
                        TNCustCtrl_Rect rect_user_ctrl = (TNCustCtrl_Rect)ctrl_defect_info;
                        DS_Insp_Param chk_insp_param = rect_user_ctrl.Insp_param;
                        DS_Insp_Result chk_insp_result = rect_user_ctrl.Insp_result;

                        //////////////////////////////////
                        // check X dir.
                        if (EN_Insp_Tol_Dir.EN_Insp_Tol_Dir_Horz == chk_insp_param.Insp_Tol_Dir)
                        {
                            for (int y = rect_user_ctrl.Pos_Info.Editing_Rect.Top; y < rect_user_ctrl.Pos_Info.Editing_Rect.Bottom; y++)
                            {
                                Get_X_Gap(bmp, rect_user_ctrl.Pos_Info.Editing_Rect.X, rect_user_ctrl.Pos_Info.Editing_Rect.X + rect_user_ctrl.Pos_Info.Editing_Rect.Width, y, out float edge_pos_left, out float edge_pos_right);

                                // no edge found.
                                if (edge_pos_left < 0 || edge_pos_right < 0)
                                    continue;

                                int gap_size = (int)edge_pos_right - (int)edge_pos_left + 1;
                                if (gap_size < max_valid_gap)
                                    continue;

                                chk_insp_result.Insp_Result_Type = EN_Insp_Result_Type.EN_Insp_Result_Failure;
                                chk_insp_result.Defect_Pos = new Rectangle((int) edge_pos_left, y
                                                                    , (int) ( edge_pos_right - edge_pos_left), 1);
                                rect_user_ctrl.Insp_result = chk_insp_result;

                                labelCheckResult.ForeColor = Color.Red;
                                labelCheckResult.Text = $"NG";
                                labelReason.Text = $"X:{edge_pos_left}~{edge_pos_right}, Y:{y} ";
                                check_res = false;
                                break;
                            }

                            if (!check_res)
                                break;

                            chk_insp_result.Insp_Result_Type = EN_Insp_Result_Type.EN_Insp_Result_Success;
                            chk_insp_result.Defect_Pos = new Rectangle(0, 0, 0, 0);
                            rect_user_ctrl.Insp_result = chk_insp_result;
                        }

                        //////////////////////////////////
                        // check Y dir.
                        if (EN_Insp_Tol_Dir.EN_Insp_Tol_Dir_Vert == chk_insp_param.Insp_Tol_Dir)
                        {
                            for (int x = rect_user_ctrl.Pos_Info.Editing_Rect.Left; x < rect_user_ctrl.Pos_Info.Editing_Rect.Right; x++)
                            {
                                Get_Y_Gap(bmp, x, rect_user_ctrl.Pos_Info.Editing_Rect.Y, rect_user_ctrl.Pos_Info.Editing_Rect.Y + rect_user_ctrl.Pos_Info.Editing_Rect.Height, out float edge_pos_top, out float edge_pos_bottom);

                                // no edge found.
                                if (edge_pos_top < 0 || edge_pos_bottom < 0)
                                    continue;

                                int gap_size = (int)edge_pos_bottom - (int)edge_pos_top + 1;
                                if (gap_size < max_valid_gap)
                                    continue;

                                chk_insp_result.Insp_Result_Type = EN_Insp_Result_Type.EN_Insp_Result_Failure;
                                chk_insp_result.Defect_Pos = new Rectangle(x, (int) edge_pos_top
                                                                    , 1, (int) ( edge_pos_bottom - edge_pos_top) );
                                rect_user_ctrl.Insp_result = chk_insp_result;

                                labelCheckResult.ForeColor = Color.Red;
                                labelCheckResult.Text = $"NG";
                                labelReason.Text = $"X:{x}, Y:{edge_pos_top}~{edge_pos_bottom}";
                                check_res = false;
                                break;
                            }

                            if (!check_res)
                                break;

                            chk_insp_result.Insp_Result_Type = EN_Insp_Result_Type.EN_Insp_Result_Success;
                            chk_insp_result.Defect_Pos = new Rectangle(0, 0, 0, 0);
                            rect_user_ctrl.Insp_result = chk_insp_result;
                        }
                    }

                    if (check_res)
                    {
                        labelCheckResult.ForeColor = Color.Green;
                        labelCheckResult.Text = $"OK";
                        labelReason.Text = $"";
                    }

                    _userctrl_image.pb_Image.Repaint();
                }
            }
            catch(Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                               , $"Exception catched: error:{ex.Message}");
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
                               , $"Exception catched: error:{ex.Message}");
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
                               , $"Exception catched: error:{ex.Message}");
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

                _userctrl_image.Image = dest_bmp;
            }
            else
            {
                _userctrl_image.Image = org_bmp;
            }

            _userctrl_image.Refresh();
        }

        private void btn1X_Click(object sender, EventArgs e)
        {
            if (null != _userctrl_image.Image)
                _userctrl_image.Image.Dispose();

            _userctrl_image.Image = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            _userctrl_image.Image_Scale = 1.0f;
            _userctrl_image.Offset = new Point(0, 0);

            _userctrl_image.Refresh();
        }

        private void btnFit_Click(object sender, EventArgs e)
        {
            if (null != _userctrl_image.Image)
                _userctrl_image.Image.Dispose();

            _userctrl_image.Image = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            _userctrl_image.pb_Image.ZoomFit();
        }

        private void btnDeleteROI_Click(object sender, EventArgs e)
        {
            if (_userctrl_image.Editing_Ctrl == null)
                return;

            _userctrl_image.Delete_Editing_ROI(_userctrl_image.Editing_Ctrl);
            _userctrl_image.Refresh();
        }

        Editing_Mode Query_Editing_Mode()
        {
            if (chkNewInsp_Rect.Checked)
                return Editing_Mode.EDT_New_ROI;

            if (null != _userctrl_image.Editing_Ctrl)
                return Editing_Mode.EDT_Editing_ROI;

            return Editing_Mode.EDT_None;
        }

        EN_Insp_Tol_Dir Get_Insp_Tol_Dir()
        {
            if (rbtnHorz.Checked)
                return EN_Insp_Tol_Dir.EN_Insp_Tol_Dir_Horz;
            else if (rbtnVert.Checked)
                return EN_Insp_Tol_Dir.EN_Insp_Tol_Dir_Vert;
            else if (rbtnLT45.Checked)
                return EN_Insp_Tol_Dir.EN_Insp_Tol_Dir_LT45;
            else if (rbtnRT45.Checked)
                return EN_Insp_Tol_Dir.EN_Insp_Tol_Dir_RT45;

            return EN_Insp_Tol_Dir.EN_Insp_Tol_None;
        }

        private void chkSubPixel_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSubPixel.Checked)
            {
                Bitmap org_bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);

                if ( !SubPixel.Apply_Interpolate(org_bmp, EN_SubPixel_Type.EN_SubPixel_Linear, EN_SubPixel_Num.EN_SubPixel_3, out Bitmap bmp_dest) )
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                        , $"Apply_Interpolate failed");
                }

                bmp_dest.Save("d:\\temp\\subpixel.bmp");
            }
            else
            {

            }
        }

        private void btnBmp2Array_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            byte[,]? buffer = (byte[,]?) Image_Buffer_Gray.Clone_Bmp_2_2DArray(bmp);

        }

        private void btnCannyEdgeDetect_Click(object sender, EventArgs e)
        {
            _userctrl_image.Cache_Ctrl.Clear();

            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);

            Canny edge_detect = new Canny(bmp, 1000, (float) numMinHysteresisThreshold.Value);

            List<Point> detected_edge_points = new List<Point>();
            // NOTE: edge_detect.EdgeMap is [width, height]
            for ( int y = 0; y < edge_detect.EdgeMap.GetLength(1); y++ )
            {
                for ( int x = 0; x < edge_detect.EdgeMap.GetLength(0); x++ )
                {
                    if (edge_detect.EdgeMap[x, y] < 128)
                        continue;

                    detected_edge_points.Add(new Point(x, y));
                }
            }

            TNControls.TNCustCtrl_Points edge_points = new TNCustCtrl_Points();
            edge_points.Pos_Info.Points = detected_edge_points.ToArray();
            _userctrl_image.Cache_Ctrl.Add(edge_points);

            _userctrl_image.pb_Image.Repaint();


        }

        private void btnSaveBinary_Click(object sender, EventArgs e)
        {
            _userctrl_image.pb_Image.Image.Save($"D:\\test\\SD2_000\\221014_092537\\My\\Binary.bmp");
        }
    } // end of     public partial class For_Main : Form
} // end of namespace CheckOffset