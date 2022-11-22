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
using CheckOffset.ImageTools;
using System;
using CheckOffset.ProjectInspInfo;

//using TNFullImageTools;
using System.Runtime.InteropServices;
using Dll_Bridge;
using System.Drawing.Imaging;
using static CheckOffset.For_Main.Image_Tools;
using CheckOffset.CSharp2Cpp;

//#include <opencv2/core.hpp>
//#include <opencv2/imgproc.hpp>
//#include <opencv2/opencv.hpp>

//#include "opencv2/opencv.hpp"

//using cv;
using OpenCvSharp.DebuggerVisualizers;
using OpenCvSharp;
using System.ComponentModel;
using CheckOffset.Insp;

namespace CheckOffset
{
    public partial class For_Main : Form
    {
        public For_Main()
        {
            InitializeComponent();

            Init_Data();
        }


        private UserCtrl_Image  _userctrl_image = new UserCtrl_Image();
        //private MatProxy        _mat_proxy      = new MatProxy();

        private InspGlueOverflow _inspGlueOverflow = null;

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

            OpenCvSharp.Rect rt_fill = new OpenCvSharp.Rect(0, 0, _userctrl_image.Image.Width, _userctrl_image.Image.Height);
        }

        private void chkNewInsp_Rect_CheckedChanged(object sender, EventArgs e)
        {
            if (null == tnGlobal.CAM_Info.Detect_Pin_Infos)
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
                    foreach (DS_CAM_Pin_Info cur_detect_infos in tnGlobal.CAM_Info.Detect_Pin_Infos)
                    {
                        TNCustCtrl_Rect exist_added_rect = new TNCustCtrl_Rect();
                        TNPictureBox tn_pb = _userctrl_image.pb_Image as TNPictureBox;
                        exist_added_rect.Pos_Info.Editing_Rect = cur_detect_infos.Detect_Rect;
                        //exist_added_rect.Insp_param = cur_detect_infos.Detect_Insp_param;
                        _userctrl_image.User_Ctrls.Add(exist_added_rect);
                    }
                }

                /////////////////////////////////////////////////
                // add new editing rect.
                TNCustCtrl_Rect new_added_rect = new TNCustCtrl_Rect();
                _userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Pos_Info.Editing_Rect = new Rect(0, 0, 100, 100);
                DS_Insp_Param_Pin new_insp_param = new DS_Insp_Param_Pin();
                new_insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                new_added_rect.Insp_param = new_insp_param;
            }
            else
            {
                // 新增完畢
                if (null != _userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

                    DS_Insp_Param_Pin new_insp_param = new DS_Insp_Param_Pin();
                    new_insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                    editing_rect.Insp_param = new_insp_param;

                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
                    {
                        DS_CAM_Pin_Info new_defect_info = new DS_CAM_Pin_Info();
                        new_defect_info.Detect_Rect = editing_rect.Pos_Info.Editing_Rect;
                        new_defect_info.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                        tnGlobal.CAM_Info.Detect_Pin_Infos.Add(new_defect_info);
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
            tnGlobal.CAM_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<DS_CAM_Info>(jsonString);

            _userctrl_image.User_Ctrls.Clear();

            Paint_Align();
            Paint_Pin_Pos(null);

            _userctrl_image.Apply_GlobalSetting_To_Ctrls();
            _userctrl_image.Refresh();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = $"json files(*.json)| *.json | All files(*.*) | *.*";
            if (DialogResult.OK != saveFileDialog.ShowDialog())
                return;

            if (null == tnGlobal.CAM_Info)
                return;

            //string jsonString_align = System.Text.Json.JsonSerializer.Serialize<DS_Detect_Align_Info>(tnGlobal.Align_Info); // tnGlobal.Setting);
            string jsonString_align = Newtonsoft.Json.JsonConvert.SerializeObject(tnGlobal.CAM_Info);
            File.WriteAllText(saveFileDialog.FileName, jsonString_align);
        }

        private void btnDetect_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = (Bitmap) System.Drawing.Image.FromFile(tbImgFile.Text);

                _userctrl_image.Cache_Ctrl.Clear();

                bool check_res = true;
                if (null != tnGlobal.CAM_Info)
                {
                    //bool first_white = false;
                    int max_valid_gap = (int)numMaxValidPixel.Value;
                    foreach (Control ctrl_defect_info in _userctrl_image.User_Ctrls)
                    {
                        TNCustCtrl_Rect rect_user_ctrl = (TNCustCtrl_Rect)ctrl_defect_info;
                        DS_Insp_Param_Pin chk_insp_param = rect_user_ctrl.Insp_param;
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
            _userctrl_image.Offset = new OpenCvSharp.Point(0, 0);

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

            if (chkSelectPattern.Checked)
                return Editing_Mode.EDT_New_Align;

            if (chkSelectChip.Checked || chkSelectABF.Checked)
                return Editing_Mode.EDT_New_Chip;

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
            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);

            Canny edge_detect = new Canny(bmp, 1000, (float) numMinHysteresisThreshold.Value);

            List<System.Drawing.Point> detected_edge_points = new List<System.Drawing.Point>();
            // NOTE: edge_detect.EdgeMap is [width, height]
            for ( int y = 0; y < edge_detect.EdgeMap.GetLength(1); y++ )
            {
                for ( int x = 0; x < edge_detect.EdgeMap.GetLength(0); x++ )
                {
                    if (edge_detect.EdgeMap[x, y] < 128)
                        continue;

                    detected_edge_points.Add(new System.Drawing.Point(x, y));
                }
            }

            TNControls.TNCustCtrl_Points edge_points = new TNCustCtrl_Points();
            edge_points.Display_Color = Color.YellowGreen;
            edge_points.Pos_Info.Points = detected_edge_points.ToArray();
            _userctrl_image.Cache_Ctrl.Add(edge_points);

            _userctrl_image.pb_Image.Repaint();


        }

        private void btnSaveBinary_Click(object sender, EventArgs e)
        {
            saveFileDialog.Filter = $"Image files (*.bmp)|*.bmp|All files (*.*)|*.*";
            if (DialogResult.OK != saveFileDialog.ShowDialog())
                return;

            _userctrl_image.pb_Image.Image.Save(saveFileDialog.FileName);
            if (null != _userctrl_image.Image)
            {
                _userctrl_image.Image.Save(saveFileDialog.FileName); // $"D:\\test\\SD2_000\\221014_092537\\My\\Binary.bmp");
            }
        }

        private void btnDetectPins_Click(object sender, EventArgs e)
        {
            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            byte[,]? buffer = (byte[,]?) Image_Buffer_Gray.Clone_Bmp_2_2DArray(bmp);
            if ( null == buffer )
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"buffer is null");
                return;
            }

            if (null == tnGlobal._IT_Detect)
            {
                tnGlobal._IT_Detect = new IT_Detect(buffer);
            }
            else
            {
                tnGlobal._IT_Detect.Clear_Result();
            }

            tnGlobal._IT_Detect.Detect_Pin();

            for (int y = 0; y < tnGlobal._IT_Detect.Pins.GetLength(0); y++)
            {
                for (int x = 0; x < tnGlobal._IT_Detect.Pins.GetLength(1); x++)
                {
                    if (tnGlobal._IT_Detect.Pins[y, x] <= 0)
                        continue;
                    else if (tnGlobal._IT_Detect.Pins[y, x] == 1)
                    {
                        TNCustCtrl_Points ctrl_string = new TNCustCtrl_Points();
                        ctrl_string.Pos_Info.Points = new System.Drawing.Point[1];
                        ctrl_string.Pos_Info.Points[0].X = x;
                        ctrl_string.Pos_Info.Points[0].Y = y;
                        ctrl_string.Display_Color = Color.Blue;

                        _userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);

                        //TNCustCtrl_String ctrl_string = new TNCustCtrl_String();
                        //ctrl_string.Pos_Info.Point_LT.X = x;
                        //ctrl_string.Pos_Info.Point_LT.Y = y;
                        //ctrl_string.Display_Str = $"{iT_Detect.Pins[y, x]}";
                        ////ctrl_string.Display_Str = $"{iT_Detect.Pins[y, x]}:{iT_Detect._v_weight[y, x]}";
                        //ctrl_string.Display_Str = $"{iT_Detect.Pins[y, x]}:{iT_Detect._h_weight[y, x]}";
                        //ctrl_string.Display_Font_Size = 8;
                        //_userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);
                    }
                    else if (tnGlobal._IT_Detect.Pins[y, x] >= 2)
                    {
                        TNCustCtrl_Points ctrl_string = new TNCustCtrl_Points();
                        ctrl_string.Pos_Info.Points = new System.Drawing.Point[1];
                        ctrl_string.Pos_Info.Points[0].X = x;
                        ctrl_string.Pos_Info.Points[0].Y = y;
                        ctrl_string.Display_Color = Color.BlueViolet;

                        _userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);
                    }
                }
            }

            _userctrl_image.pb_Image.Repaint();
        }

        private void btnProjectFile_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != openFileDialog_Setting.ShowDialog())
                return;

            tbProjectFile.Text = openFileDialog_Img.FileName;
            string jsonString = File.ReadAllText(openFileDialog_Setting.FileName);
            tnGlobal.CAM_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<DS_CAM_Info>(jsonString);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string jsonString = File.ReadAllText(tbProjectFile.Text);
            //tnGlobal.Detect_Pins = Newtonsoft.Json.JsonConvert.DeserializeObject<DS_Defect_Pin_Info> (jsonString);
            tnGlobal.CAM_Info = Newtonsoft.Json.JsonConvert.DeserializeObject<DS_CAM_Info> (jsonString);
        }

        private void btnClearCacheItems_Click(object sender, EventArgs e)
        {
            _userctrl_image.Cache_Ctrl.Clear();
        }

        //private void btnDetectBlob_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
        //        BitmapData? bmp_data = null;
        //        Image_Buffer_Gray.GetBuffer(bmp, ref bmp_data);

        //        byte[]? buffer = (byte[]?)Image_Buffer_Gray.Transfer_Bmp_2_Gray_1DArray(bmp);
        //        if (null == bmp_data)
        //        {
        //            Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //                                , $"bmp_data is null");
        //            return;
        //        }

        //        //Image_Tools_CLR fast_blob = new Image_Tools_CLR();
        //        //btnDetectBlob.Text = String.Format("{0}", fast_blob.test());

        //        //ImageTool_Buffer img_buf = new ImageTool_Buffer(bmp_data.Scan0, bmp_data.Stride, bmp_data.Height);
        //        //ImageTool_Buffer img_buf = new ImageTool_Buffer(); // bmp_data.Scan0, bmp_data.Stride, bmp_data.Height);
        //        //img_buf.Data = bmp_data.Scan0;
        //        //img_buf.Stride = bmp.Width;
        //        //img_buf.Height = bmp.Height;
        //        //unsafe
        //        //{
        //        //    fast_blob.compute( (byte*) bmp_data.Scan0.ToPointer(), bmp.Width, bmp.Height);

        //        //    Image_Tools.Blob_Info output = new Image_Tools.Blob_Info();
        //        //    fast_blob.blobInfo(output);

        //        //}

        //        /////////////////////////////
        //        // remove for can't work
        //        //Image_Tools_CLR img_tool_clr = Image_Tools.CreateImageToolsCLRClass();

        //        //Image_Tools.Compute(img_tool_clr, bmp_data.Scan0, bmp_data.Stride, bmp_data.Height);

        //        //int blob_size = 0;
        //        //Image_Tools.Get_Blob_Size(img_tool_clr, ref blob_size);

        //        //Blob_Info_Base[] blob_res = new Blob_Info_Base[blob_size];
        //        //for(int i = 0; i < blob_size; i++)
        //        //    blob_res[i] = new Blob_Info_Base();
        //        ////IntPtr pnt = Marshal.AllocHGlobal(Marshal.SizeOf(Blob_Info_Base));

        //        //    //Image_Tools.Get_Blob_Res(img_tool_clr, ref Blob_Info[] blob_res);
        //        //Image_Tools.Get_Blob_Res(img_tool_clr, ref blob_res);

        //        //Image_Tools.DisposeImageToolsCLRClass(img_tool_clr);

        //        //_userctrl_image.pb_Image.Repaint();



        //        CImgTools_Bridge img_tool_bridge = new CImgTools_Bridge();

        //        unsafe
        //        {
        //            img_tool_bridge.compute((byte*)bmp_data.Scan0, bmp_data.Stride, bmp_data.Height);
        //        }

        //        //Managed_Blob_Info_Base[] blob_info_bases = new Managed_Blob_Info_Base[0];
        //        //img_tool_bridge.Get_Blob_Res_Bridge(img_tool_bridge,  ref blob_info_bases);

        //        Managed_Blob_Info[] blob_infos = new Managed_Blob_Info[0];
        //        img_tool_bridge.Get_Blob_Res(ref blob_infos);

        //        for (int blob_id = 0; blob_id < blob_infos.Length; blob_id++)
        //        {
        //            if (blob_infos[blob_id]._blob_points.Length <= 0 )
        //            {
        //                Log_Utl.Log_Step("AAA"
        //                                , "_blob_points <= 0");
        //                continue;
        //            }

        //            TNCustCtrl_Points cust_ctrl = new TNCustCtrl_Points();
        //            cust_ctrl.Pos_Info.Points = new Point[blob_infos[blob_id]._blob_points.Length];
        //            cust_ctrl.Display_Color = Color.Blue;
        //            for (int pt_id = 0; pt_id < blob_infos[blob_id]._blob_points.Length; pt_id++)
        //            {
        //                cust_ctrl.Pos_Info.Points[pt_id].X = (int) blob_infos[blob_id]._blob_points[pt_id].x;
        //                cust_ctrl.Pos_Info.Points[pt_id].Y = (int) blob_infos[blob_id]._blob_points[pt_id].y;
        //            }

        //            _userctrl_image.pb_Image.Cache_Ctrl.Add(cust_ctrl);
        //        }

        //        _userctrl_image.pb_Image.Repaint();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //           , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }
        //}

        private void btnDetectBlob_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);

                byte[] buffer = (byte[])Image_Buffer_Gray.Transfer_Bmp_2_Gray_1DArray(bmp);
                if (null == buffer)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                        , $"bmp_data is null");
                    return;
                }

                CImgTools_Bridge img_tool_bridge = new CImgTools_Bridge();

                unsafe
                {
                    // transfer to un managed buffer for fast.blob.
                    fixed (byte *p_buffer = buffer)
                    {
                        img_tool_bridge.compute((byte*) (IntPtr)  p_buffer, bmp.Width, bmp.Height);
                    }
                }

                Managed_Blob_Info[] blob_infos = new Managed_Blob_Info[0];
                img_tool_bridge.Get_Blob_Res(ref blob_infos);

                for (int blob_id = 0; blob_id < blob_infos.Length; blob_id++)
                {
                    if (blob_infos[blob_id]._blob_points.Length <= 0)
                    {
                        Log_Utl.Log_Step("AAA"
                                        , "_blob_points <= 0");
                        continue;
                    }

                    TNCustCtrl_Lines cust_ctrl = new TNCustCtrl_Lines();
                    cust_ctrl.Pos_Info.Points = new System.Drawing.Point[blob_infos[blob_id]._blob_points.Length];
                    int color_id = blob_id % 4;
                    switch ( color_id )
                    {
                        case 0: cust_ctrl.Display_Color = Color.Blue;  break;
                        case 1: cust_ctrl.Display_Color = Color.Green; break;
                        case 2: cust_ctrl.Display_Color = Color.Yellow; break;
                        case 3: cust_ctrl.Display_Color = Color.Purple; break;
                    }

                    for (int pt_id = 0; pt_id < blob_infos[blob_id]._blob_points.Length; pt_id++)
                    {
                        cust_ctrl.Pos_Info.Points[pt_id].X = (int)blob_infos[blob_id]._blob_points[pt_id].x;
                        cust_ctrl.Pos_Info.Points[pt_id].Y = (int)blob_infos[blob_id]._blob_points[pt_id].y;
                    }

                    _userctrl_image.pb_Image.Cache_Ctrl.Add(cust_ctrl);
                }

                _userctrl_image.pb_Image.Repaint();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }



        private void button3_Click(object sender, EventArgs e)
        {
            //Blob_Analyze_Adapter blob_adapter = new Blob_Analyze_Adapter();
            int test = 0;
            //Blob_Analyze_Adapter blob_adapter = new Blob_Analyze_Adapter();

            //Image_Tools_CLR fast_blob = new Image_Tools_CLR();
        }

        private void btnDetectBlobOld_Click(object sender, EventArgs e)
        {
            try
            {
                Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
                byte[,]? buffer = (byte[,]?)Image_Buffer_Gray.Clone_Bmp_2_2DArray(bmp);
                if (null == buffer)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                        , $"buffer is null");
                    return;
                }

                IntPtr blob_analyze_inst = ImportImageTools.CreateClass();

                Blob_Analyze blob_analyze = new Blob_Analyze();
                blob_analyze.Blob_Detect(buffer);

                for (int y = 0; y < blob_analyze.Blob.GetLength(0); y++)
                {
                    for (int x = 0; x < blob_analyze.Blob.GetLength(1); x++)
                    {
                        if (blob_analyze.Blob[y, x] <= 0)
                            continue;

                        TNCustCtrl_String ctrl_string = new TNCustCtrl_String();
                        ctrl_string.Pos_Info.Point_LT.X = x;
                        ctrl_string.Pos_Info.Point_LT.Y = y;
                        ctrl_string.Display_Str = $"{blob_analyze.Blob[y, x]}";
                        ctrl_string.Display_Font_Size = 8;
                        _userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);
                    }
                }

                _userctrl_image.pb_Image.Repaint();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }

        }
        public class Image_Tools
        {

            [StructLayout(LayoutKind.Sequential)]
            public class Marshal_Buffer
            {
                public IntPtr   _buffer;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class Blob_Info
            {
                public Blob_Info_Base _blob_info;

                public Blob_Info_Contour _contour;

                public Blob_Info()
                {
                    _blob_info = new Blob_Info_Base();

                    _contour = new Blob_Info_Contour();
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public class Blob_Info_Base
            {
                // Sample like : void New3([MarshalAs(UnmanagedType.SafeArray, SafeArraySubType=VT_BSTR)]
                //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
                public int _id;
                public int _rect_x;
                public int _rect_y;
                public int _rect_width;
                public int _rect_height;

                public double _centeriod_x;
                public double _centeriod_y;

                public Blob_Info_Base()
                {
                    _id = 0;
                    _rect_x = 0;
                    _rect_y = 0;
                    _rect_width = 0;
                    _rect_height = 0;

                    _centeriod_x = 0.0;
                    _centeriod_y = 0.0;
            }
            }

            [StructLayout(LayoutKind.Sequential)]
            public class Blob_Info_Contour
            {
                //[MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VaEnum)]
                [MarshalAs(UnmanagedType.SafeArray)]
                public Blob_Info_Contour_calPoint[] _blob_points;

                public Blob_Info_Contour()
                {
                    _blob_points = new Blob_Info_Contour_calPoint[0];
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public class Blob_Info_Contour_calPoint
            {
                public double d_x;
                public double d_y;

                public Blob_Info_Contour_calPoint(double x, double y)
                {
                    this.d_x = x;
                    this.d_y = y;
                }
            }
            //VT_PTR

            [StructLayout(LayoutKind.Sequential)]
            public class Blob_Infos
            {
                [MarshalAs(UnmanagedType.SafeArray, SafeArraySubType = VarEnum.VT_USERDEFINED)]
                public Blob_Info[]? blob_Infos;

                public Blob_Infos()
                {
                    blob_Infos = null;
                }
            }

            //[MarshalAs(UnmanagedType.ByValArray)]

            //    string dll_file = "D:\\Source\\CheckOffset\\x64\\Debug\\Dll_Adapter.dll";

            //[DllImport("Dll_Adapter.dll")]
            //public static extern Image_Tools_CLR CreateImageToolsCLRClass();

            //[DllImport("Dll_Adapter.dll")]
            //public static extern void DisposeImageToolsCLRClass(Image_Tools_CLR img_tool_clr);

            //[DllImport("Dll_Adapter.dll")]
            //public static extern bool Compute(Image_Tools_CLR img_tool_clr, IntPtr buffer, int stride, int height);

            //[DllImport("Dll_Adapter.dll")]
            //public static extern bool Get_Blob_Size(Image_Tools_CLR img_tool_clr, ref int blob_size);

            //[DllImport("Dll_Adapter.dll")]
            ////public static extern IntPtr Get_Blob_Res(
            ////        Image_Tools_CLR img_tool_clr);
            //public static extern bool Get_Blob_Res(
            //        Image_Tools_CLR img_tool_clr
            //    ,   [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BlobAnalyzeMarshaler_Base))]
            //        ref Blob_Info_Base[] blob_res);

            //[DllImport("Dll_Adapter.dll")]
            ////public static extern IntPtr Get_Blob_Res(
            ////        Image_Tools_CLR img_tool_clr);
            //public static extern bool Get_Blob_Contour(
            //        Image_Tools_CLR img_tool_clr
            //        , int blob_id  // 0-base.
            //        , [In, Out, MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = typeof(BlobAnalyzeMarshaler_Contour))]
            //        ref Blob_Info_Contour_calPoint[] blob_contour);

            //[DllImport("opencv_world460d.dll")]
            //public static extern Mat imread(String filename, int flags);
        }


        private void btnLoadInspPins_Click(object sender, EventArgs e)
        {

        }

        private void btnSaveInspPins_Click(object sender, EventArgs e)
        {

        }

        //private void btnOpenCV_Click(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        //OpenCV_Bridge.OpenCB_1();
        //        //Mat image = imread("D:\\test\\blobSource_24.bmp", 0);

        //        var img = @"D:\test\blobSource_24.bmp";
        //        //Mat image = new Mat(@"D:\test\blobSource_24.bmp");
        //        Mat image = Cv2.ImRead(@"D:\\test\\blobSource_opencv_8.bmp", ImreadModes.Unchanged); // ImreadModes.GrayScale);
        //        //var mainForm = new ImageViewer(img);

        //        //using (var src = new Mat(@"D:\test\blobSource_24.bmp", ImreadModes.Unchanged))
        //        //{
        //            //using (var window = new Window("window", image: src, flags: WindowMode.AutoSize))
        //            //{
        //            //    Cv2.WaitKey();
        //            //}
        //        //}

        //        //mainForm.FormBorderStyle = FormBorderStyle.None;
        //        _userctrl_image.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
        //        //mainForm.Owner = (Form) _userctrl_image;
        //        //mainForm.ShowDialog();

        //        //imshow("Grayscale image", image);

        //        //// Save grayscale image
        //        //imwrite("boyGray.jpg", image);

        //        //imshow("Grayscale image", image);
        //        //waitKey(0);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
        //                    , $"Exception catched: error:{ex.Message}");
        //        // 儲存Exception到檔案
        //        TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
        //    }
        //}

        private void btnOpenCV_Click(object sender, EventArgs e)
        {
            try
            {
                var img = @"D:\test\blobSource_24.bmp";
                Mat image = Cv2.ImRead(@"D:\\test\\blobSource_opencv_8.bmp", ImreadModes.Unchanged); // ImreadModes.GrayScale);
                                                                                                     //var mainForm = new ImageViewer(img);
                _userctrl_image.Image = OpenCvSharp.Extensions.BitmapConverter.ToBitmap(image);
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void btnFindPinPosition_Click(object sender, EventArgs e)
        {
            tnGlobal.CAM_Info.Detect_Pin_Infos.Clear();

            FindPinPosition pin_finder = new FindPinPosition();
            pin_finder.Find_Pin_Position(tnGlobal._IT_Detect);

            _userctrl_image.pb_Image.Cache_Ctrl.Clear();

            for (int contour_id = 0; contour_id < pin_finder.Found_Contours.GetLength(0); contour_id++)
            {
                TNCustCtrl_Polygon exist_added_pgn = new TNCustCtrl_Polygon();
                exist_added_pgn.Pos_Info.Points = new System.Drawing.Point[ pin_finder.Found_Contours[contour_id].Length ];
                for (int pt_id = 0; pt_id < pin_finder.Found_Contours[contour_id].Length; pt_id++)
                {
                    exist_added_pgn.Pos_Info.Points[pt_id].X = pin_finder.Found_Contours[contour_id][pt_id].X;
                    exist_added_pgn.Pos_Info.Points[pt_id].Y = pin_finder.Found_Contours[contour_id][pt_id].Y;
                }

                exist_added_pgn.Display_Color = Color.Purple;
                _userctrl_image.Cache_Ctrl.Add(exist_added_pgn);
            }

            Paint_Pin_Pos(null);

            labelCheckResult.Text = $"Found blob:{tnGlobal.CAM_Info.Detect_Pin_Infos.Count}";
    }

        private void numPinMinWH_ValueChanged(object sender, EventArgs e)
        {
            //tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W = (float) numPinMinW.Value;
            //tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_H = (float) numPinMinW.Value;
        }

        private void numPinMinW_ValueChanged(object sender, EventArgs e)
        {
            tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_W = (int) numPinMinW.Value;
        }

        private void numPinMinH_ValueChanged(object sender, EventArgs e)
        {
            tnGlobal.Insp_Param.Insp_Param_Pin.Pin_Tol_H = (int) numPinMinH.Value;
        }

        private void Paint_Pin_Pos(DS_Single_Insp_Info inspecting_info)
        {
            try
            {
                int pin_idx = 1;
                foreach (DS_CAM_Pin_Info insp_pin in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    TNCustCtrl_Rect exist_added_rect = new TNCustCtrl_Rect();
                    TNPictureBox tn_pb = _userctrl_image.pb_Image as TNPictureBox;
                    if (null == inspecting_info)
                    {
                        exist_added_rect.Pos_Info.Editing_Rect = insp_pin.Detect_Rect;
                    }
                    else
                    {
                        exist_added_rect.Pos_Info.Editing_Rect = new OpenCvSharp.Rect(insp_pin.Detect_Rect.X + inspecting_info._insp_inst._align_offset.Width
                                                                                , insp_pin.Detect_Rect.Y + inspecting_info._insp_inst._align_offset.Height
                                                                                , insp_pin.Detect_Rect.Width, insp_pin.Detect_Rect.Height);
                    }

                    exist_added_rect.Insp_param.Pin_Idx = pin_idx;
                    pin_idx++;

                    //exist_added_rect.Insp_param = cur_detect_infos.Detect_Insp_param;
                    exist_added_rect.Display_Color = Color.Green;
                    _userctrl_image.User_Ctrls.Add(exist_added_rect);
                }

                _userctrl_image.pb_Image.Repaint();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void Paint_Align()
        {
            try
            {
                TNCustCtrl_Rect align_rect = new TNCustCtrl_Rect();
                align_rect.Display_Color = Color.Green;

                if (tnGlobal.Insp_Pools.Count <= 0)
                {
                    align_rect.Pos_Info.Editing_Rect = tnGlobal.CAM_Info.Align_Info.Align_Rect;
                }
                else
                {
                    for( int insp_id = 0; insp_id < tnGlobal.Insp_Pools.Count; insp_id++ )
                    {
                        if (!tnGlobal.Insp_Pools[insp_id]._inspected || null == tnGlobal.Insp_Pools[insp_id]._insp_inst )
                            continue;

                        align_rect.Pos_Info.Editing_Rect = new OpenCvSharp.Rect(tnGlobal.CAM_Info.Align_Info.Align_Rect.X + tnGlobal.Insp_Pools[insp_id]._insp_inst._align_offset.Width
                                                                              , tnGlobal.CAM_Info.Align_Info.Align_Rect.Y + tnGlobal.Insp_Pools[insp_id]._insp_inst._align_offset.Height
                                                                              , tnGlobal.CAM_Info.Align_Info.Align_Rect.Width, tnGlobal.CAM_Info.Align_Info.Align_Rect.Height );
                    }
                }

                _userctrl_image.User_Ctrls.Add(align_rect);

                _userctrl_image.pb_Image.Repaint();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void saveFileDialog_FileOk(object sender, CancelEventArgs e)
        {

        }

        private void btnSelectPattern_Click(object sender, EventArgs e)
        {

        }

        private void chkSelectPattern_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectPattern.Checked)
            {
                _userctrl_image.pb_Image.Editing_Ctrl = null;

                /////////////////////////////////////////////////
                /// add exist rect.
                if (null != _userctrl_image.User_Ctrls)
                {
                    _userctrl_image.User_Ctrls.Clear();
                    foreach (DS_CAM_Pin_Info cur_detect_infos in tnGlobal.CAM_Info.Detect_Pin_Infos)
                    {
                        TNCustCtrl_Rect exist_added_rect = new TNCustCtrl_Rect();
                        TNPictureBox tn_pb = _userctrl_image.pb_Image as TNPictureBox;
                        exist_added_rect.Pos_Info.Editing_Rect = cur_detect_infos.Detect_Rect;
                        //exist_added_rect.Insp_param = cur_detect_infos.Detect_Insp_param;
                        _userctrl_image.User_Ctrls.Add(exist_added_rect);
                    }

                    _userctrl_image.CBMouse_Up += UserCtrl_Img_Mouse_Up;
                }

                /////////////////////////////////////////////////
                // add new editing rect.
                TNCustCtrl_Rect new_added_rect = new TNCustCtrl_Rect();
                _userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Pos_Info.Editing_Rect = new Rect(0, 0, 100, 100);
                new_added_rect.Display_Color = Color.Blue;
                //DS_Insp_Param_Pin new_insp_param = new DS_Insp_Param_Pin();
                //new_insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                //new_added_rect.Insp_param = new_insp_param;
            }
            else
            {
                _userctrl_image.CBMouse_Up -= UserCtrl_Img_Mouse_Up;
                // 新增完畢
                if (null != _userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
                    {
                        DS_CAMt_Align_Info new_align_info = new DS_CAMt_Align_Info();
                        new_align_info.Align_Rect = editing_rect.Pos_Info.Editing_Rect;

                        tnGlobal.CAM_Info.Align_Info = new_align_info;
                    }

                    _userctrl_image.Apply_GlobalSetting_To_Ctrls();
                }

                _userctrl_image.pb_Image.Editing_Ctrl = null;
            }
        }

        void UserCtrl_Img_Mouse_Up()
        {
            if (!chkSelectPattern.Checked)
                return;

            // 新增完畢
            if (null == _userctrl_image.pb_Image.Editing_Ctrl)
                return;

            TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

            const int min_roi_valid_size = 2;
            if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
            {
                DS_CAMt_Align_Info new_align_info = new DS_CAMt_Align_Info();
                new_align_info.Align_Rect = editing_rect.Pos_Info.Editing_Rect;

                new_align_info.Align_Rect.Width = (new_align_info.Align_Rect.Width + 3) / 4 * 4;

                tnGlobal.CAM_Info.Align_Info = new_align_info;
            }

            _userctrl_image.pb_Image.Editing_Ctrl = null;

            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            Mat<byte>? src_buffer = OpenCVMatTool.Clone_Bmp_2_Mat(bmp);
            if (!chkPatternIsWhite.Checked)
                Cv2.BitwiseNot(src_buffer, src_buffer);

            FindAlignment find_align = new FindAlignment(src_buffer);
            find_align.Find_Alignment(Insp_Base_Tools.To_System_Rect( tnGlobal.CAM_Info.Align_Info.Align_Rect) );

            tnGlobal.CAM_Info.Align_Info.Align_Is_White = chkPatternIsWhite.Checked;

            // repaint align.
            _userctrl_image.User_Ctrls.Clear();
            Paint_Align();
            Paint_Pin_Pos( null );
            _userctrl_image.Refresh();
        }

        private void btnCreateInspMap_Click(object sender, EventArgs e)
        {
            try
            {
                Mat<byte> insp_mat = new Mat<byte>(480, 640);

                if ( null != tnGlobal.CAM_Info.Detect_Pin_Infos )
                {
                    //foreach(CheckOffset.tnGlobal.DS_Detect_Pin_Info pin_info in tnGlobal.Detect_Info.Detect_Pin_Infos )
                    foreach (DS_CAM_Pin_Info pin_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                    {
                        Insp_Base_Tools.Mat_Fill(insp_mat, pin_info.Detect_Rect, 0x11);
                    }

                    insp_mat.SaveImage("d:\\test\\Mat_Fill.jpg");
                }
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void btnTestInsp_Click(object sender, EventArgs e)
        {
            tnGlobal.Insp_Pools.Clear();

            DS_Single_Insp_Info new_tobe_insp_info = new DS_Single_Insp_Info();
            new_tobe_insp_info._tobe_insp_file = tbImgFile.Text;
            tnGlobal.Insp_Pools.Add(new_tobe_insp_info);

            _inspGlueOverflow = null; 
            DS_Single_Insp_Info inspecting_info = null;
            foreach ( DS_Single_Insp_Info tobe_insp_info in tnGlobal.Insp_Pools )
            {
                if (tobe_insp_info._inspected)
                {
                    Log_Utl.Log_Event(Event_Level.Warning, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                      , $"file inspected:{tobe_insp_info._tobe_insp_file}");
                    continue;
                }

                tobe_insp_info._insp_inst = new InspGlueOverflow();
                if (   tobe_insp_info._tobe_insp_buffer.Width <= 0
                    || tobe_insp_info._tobe_insp_buffer.Height <= 0 )
                {
                    if (tobe_insp_info._tobe_insp_file.Length <= 0)
                        continue;

                    Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tobe_insp_info._tobe_insp_file);

                    Image_Binary.Binary_Image(bmp, (int)numThreshold.Value, out Bitmap? dest_bmp);

                    tobe_insp_info._tobe_insp_buffer = OpenCVMatTool.Clone_Bmp_2_Mat(dest_bmp);
                }

                tobe_insp_info._insp_inst.Insp(tobe_insp_info._tobe_insp_buffer);
                tobe_insp_info._inspected = true;

                inspecting_info = tobe_insp_info;
                _inspGlueOverflow = tobe_insp_info._insp_inst;
            }

            _userctrl_image.User_Ctrls.Clear();
            Paint_Align();
            Paint_Pin_Pos(inspecting_info);
            _userctrl_image.Refresh();
        }

        private void chkDisplayDefectMaskPos_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                _userctrl_image.Cache_Ctrl.Clear();

                if (!chkDisplayDefectMaskPos.Checked)
                {
                    return;
                }

                if (null == _inspGlueOverflow)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                        , $"_inspGlueOverflow is null");
                    return;
                }

                TNControls.TNCustCtrl_Points defect_mask_ctrl = new TNCustCtrl_Points();
                defect_mask_ctrl.Display_Color = Color.Red;
                List<System.Drawing.Point> defect_mask_pts = new List<System.Drawing.Point>();

                for (int y = 0; y < _inspGlueOverflow._defect_candidate.Height; y++)
                {
                    for (int x = 0; x < _inspGlueOverflow._defect_candidate.Width; x++)
                    {
                        if (_inspGlueOverflow._defect_candidate.Get<Byte>(y, x) < 1)
                            continue;

                        defect_mask_pts.Add(new System.Drawing.Point(x, y));
                    }
                }
                defect_mask_ctrl.Pos_Info.Points = defect_mask_pts.ToArray();

                _userctrl_image.Cache_Ctrl.Add(defect_mask_ctrl);

                _userctrl_image.Refresh();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void chKDisplayDefect_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                _userctrl_image.Cache_Ctrl.Clear();

                if (!chKDisplayDefect.Checked)
                {
                    return;
                }

                if (null == _inspGlueOverflow)
                {
                    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                        , $"_inspGlueOverflow is null");
                    return;
                }

                TNControls.TNCustCtrl_Points cust_pts_ctrl = new TNCustCtrl_Points();
                cust_pts_ctrl.Display_Color = Color.Red;
                List<System.Drawing.Point> defect_mask_pts = new List<System.Drawing.Point>();

                for (int y = 0; y < _inspGlueOverflow._diff_result.Height; y++)
                {
                    for (int x = 0; x < _inspGlueOverflow._diff_result.Width; x++)
                    {
                        if (_inspGlueOverflow._diff_result.Get<Byte>(y, x) < 1)
                            continue;

                        defect_mask_pts.Add(new System.Drawing.Point(x, y));
                    }
                }
                cust_pts_ctrl.Pos_Info.Points = defect_mask_pts.ToArray();

                _userctrl_image.Cache_Ctrl.Add(cust_pts_ctrl);

                _userctrl_image.Refresh();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        private void btnSelectChip_Click(object sender, EventArgs e)
        {
            if (chkSelectPattern.Checked)
            {
                _userctrl_image.pb_Image.Editing_Ctrl = null;

                /////////////////////////////////////////////////
                /// add exist rect.
                if (null != _userctrl_image.User_Ctrls)
                {
                    _userctrl_image.User_Ctrls.Clear();
                    foreach (DS_CAM_Pin_Info cur_detect_infos in tnGlobal.CAM_Info.Detect_Pin_Infos)
                    {
                        TNCustCtrl_Rect exist_added_rect = new TNCustCtrl_Rect();
                        TNPictureBox tn_pb = _userctrl_image.pb_Image as TNPictureBox;
                        exist_added_rect.Pos_Info.Editing_Rect = cur_detect_infos.Detect_Rect;
                        //exist_added_rect.Insp_param = cur_detect_infos.Detect_Insp_param;
                        _userctrl_image.User_Ctrls.Add(exist_added_rect);
                    }

                    _userctrl_image.CBMouse_Up += UserCtrl_Img_Mouse_Up;
                }

                /////////////////////////////////////////////////
                // add new editing rect.
                TNCustCtrl_Rect new_added_rect = new TNCustCtrl_Rect();
                _userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Pos_Info.Editing_Rect = new Rect(0, 0, 100, 100);
                new_added_rect.Display_Color = Color.Blue;
                //DS_Insp_Param_Pin new_insp_param = new DS_Insp_Param_Pin();
                //new_insp_param.Insp_Tol_Dir = Get_Insp_Tol_Dir();
                //new_added_rect.Insp_param = new_insp_param;
            }
            else
            {
                _userctrl_image.CBMouse_Up -= UserCtrl_Img_Mouse_Up;
                // 新增完畢
                if (null != _userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
                    {
                        DS_CAMt_Align_Info new_align_info = new DS_CAMt_Align_Info();
                        new_align_info.Align_Rect = editing_rect.Pos_Info.Editing_Rect;

                        tnGlobal.CAM_Info.Align_Info = new_align_info;
                    }

                    _userctrl_image.Apply_GlobalSetting_To_Ctrls();
                }

                _userctrl_image.pb_Image.Editing_Ctrl = null;
            }
        }


        private void btnDetectChip_Click(object sender, EventArgs e)
        {
            if ( null == tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder )
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                    , $"_Chip_Pos_Finder is null");
                return;
            }

            Bitmap bmp = (Bitmap)System.Drawing.Image.FromFile(tbImgFile.Text);
            if (null == bmp)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                    , $"bmp is null");
                return;
            }

            tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder.Find_Chip_Position(bmp, tbImgFile.Text);


            //byte[,]? buffer = (byte[,]?)Image_Buffer_Gray.Clone_Bmp_2_2DArray(bmp);
            //if (null == buffer)
            //{
            //    Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
            //                        , $"buffer is null");
            //    return;
            //}

            //if (null == tnGlobal._IT_Detect)
            //{
            //    tnGlobal._IT_Detect = new IT_Detect(buffer);
            //}
            //else
            //{
            //    tnGlobal._IT_Detect.Clear_Result();
            //}

            //tnGlobal._IT_Detect.Detect_Pin();

            //for (int y = 0; y < tnGlobal._IT_Detect.Pins.GetLength(0); y++)
            //{
            //    for (int x = 0; x < tnGlobal._IT_Detect.Pins.GetLength(1); x++)
            //    {
            //        if (tnGlobal._IT_Detect.Pins[y, x] <= 0)
            //            continue;
            //        else if (tnGlobal._IT_Detect.Pins[y, x] == 1)
            //        {
            //            TNCustCtrl_Points ctrl_string = new TNCustCtrl_Points();
            //            ctrl_string.Pos_Info.Points = new System.Drawing.Point[1];
            //            ctrl_string.Pos_Info.Points[0].X = x;
            //            ctrl_string.Pos_Info.Points[0].Y = y;
            //            ctrl_string.Display_Color = Color.Blue;

            //            _userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);

            //            //TNCustCtrl_String ctrl_string = new TNCustCtrl_String();
            //            //ctrl_string.Pos_Info.Point_LT.X = x;
            //            //ctrl_string.Pos_Info.Point_LT.Y = y;
            //            //ctrl_string.Display_Str = $"{iT_Detect.Pins[y, x]}";
            //            ////ctrl_string.Display_Str = $"{iT_Detect.Pins[y, x]}:{iT_Detect._v_weight[y, x]}";
            //            //ctrl_string.Display_Str = $"{iT_Detect.Pins[y, x]}:{iT_Detect._h_weight[y, x]}";
            //            //ctrl_string.Display_Font_Size = 8;
            //            //_userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);
            //        }
            //        else if (tnGlobal._IT_Detect.Pins[y, x] >= 2)
            //        {
            //            TNCustCtrl_Points ctrl_string = new TNCustCtrl_Points();
            //            ctrl_string.Pos_Info.Points = new System.Drawing.Point[1];
            //            ctrl_string.Pos_Info.Points[0].X = x;
            //            ctrl_string.Pos_Info.Points[0].Y = y;
            //            ctrl_string.Display_Color = Color.BlueViolet;

            //            _userctrl_image.pb_Image.Cache_Ctrl.Add(ctrl_string);
            //        }
            //    }
            //}

            _userctrl_image.pb_Image.Repaint();
        }

        private void chkSelectChip_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectChip.Checked)
            {
                _userctrl_image.pb_Image.Editing_Ctrl = null;

                /////////////////////////////////////////////////
                /// add exist rect.
                _userctrl_image.CBMouse_Up += UserCtrl_Img_Mouse_Up;

                /////////////////////////////////////////////////
                // add new editing rect.
                TNCustCtrl_Rect new_added_rect = new TNCustCtrl_Rect();
                _userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Pos_Info.Editing_Rect = new Rect(0, 0, 100, 100);
                new_added_rect.Display_Color = Color.Blue;
            }
            else
            {
                _userctrl_image.CBMouse_Up -= UserCtrl_Img_Mouse_Up;
                // 新增完畢
                if (null != _userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
                    {
                        if (null != tnGlobal.CAM_Info.Chip_Info)
                        {
                            tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder.Select_Chip = editing_rect.Pos_Info.Editing_Rect;
                        }
                        else
                        {
                            DS_CAM_Chip_Info new_chip_info = new DS_CAM_Chip_Info();
                            new_chip_info.Chip_Pos_Finder.Select_Chip = editing_rect.Pos_Info.Editing_Rect;

                            tnGlobal.CAM_Info.Chip_Info = new_chip_info;
                        }
                    }

                    _userctrl_image.Apply_GlobalSetting_To_Ctrls();
                }

                _userctrl_image.pb_Image.Editing_Ctrl = null;
            }
        }

        private void chkSelectABF_CheckedChanged(object sender, EventArgs e)
        {
            if (chkSelectABF.Checked)
            {
                _userctrl_image.pb_Image.Editing_Ctrl = null;

                /////////////////////////////////////////////////
                /// add exist rect.
                _userctrl_image.CBMouse_Up += UserCtrl_Img_Mouse_Up;

                /////////////////////////////////////////////////
                // add new editing rect.
                TNCustCtrl_Rect new_added_rect = new TNCustCtrl_Rect();
                _userctrl_image.pb_Image.Editing_Ctrl = new_added_rect;

                new_added_rect.Pos_Info.Editing_Rect = new Rect(0, 0, 100, 100);
                new_added_rect.Display_Color = Color.Blue;
            }
            else
            {
                _userctrl_image.CBMouse_Up -= UserCtrl_Img_Mouse_Up;
                // 新增完畢
                if (null != _userctrl_image.pb_Image.Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_rect = (TNCustCtrl_Rect)_userctrl_image.pb_Image.Editing_Ctrl;

                    const int min_roi_valid_size = 2;
                    if (null != editing_rect && editing_rect.Pos_Info.Editing_Rect.Width > min_roi_valid_size && editing_rect.Pos_Info.Editing_Rect.Height > min_roi_valid_size)
                    {
                        if (null != tnGlobal.CAM_Info.Chip_Info)
                        {
                            tnGlobal.CAM_Info.Chip_Info.Chip_Pos_Finder.Select_ABF = editing_rect.Pos_Info.Editing_Rect;
                        }
                        else
                        {
                            DS_CAM_Chip_Info new_chip_info = new DS_CAM_Chip_Info();
                            new_chip_info.Chip_Pos_Finder.Select_ABF = editing_rect.Pos_Info.Editing_Rect;

                            tnGlobal.CAM_Info.Chip_Info = new_chip_info;
                        }
                    }

                    _userctrl_image.Apply_GlobalSetting_To_Ctrls();
                }

                _userctrl_image.pb_Image.Editing_Ctrl = null;
            }
        }
    } // end of     public partial class For_Main : Form
} // end of namespace CheckOffset