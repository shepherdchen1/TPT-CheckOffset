//using CheckOffset.Controls;
using TNControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TN.ImageTools;
using TN.Tools.Debug;

using OpenCvSharp;

using static System.Net.Mime.MediaTypeNames;
using CheckOffset.ImageTools;

namespace CheckOffset
{
    //public delegate int Delegate_Report_GrayLevel(Color clr);
    //public delegate int Delegate_Report_GrayLevel_Gray(int x, int y, int gray_level);
    public delegate Editing_Mode Delegate_Query_Editing_Mode();

    public delegate void Delegate_Mouse_Up();

    public partial class UserCtrl_Image : UserControl
    {
        //private Bitmap? _Image = null;
        //public Bitmap? _show_image = null;
        public OpenCvSharp.Point _pt_LBtnDown; // image 坐標系
        public OpenCvSharp.Point _pt_Current;  // image 坐標系

        //private float _scale = 1.0f;
        //private Point _offset;     // 原始影像座標(_image)顯示的起點: 往右拉，X值為負值，代表影像原點座標要往負方向開始截取.

        OpenCvSharp.Point _move_start_pt = new OpenCvSharp.Point(0, 0);
        OpenCvSharp.Point _move_start_offset = new OpenCvSharp.Point(0, 0);

        Editing_Mode _editing_mode = Editing_Mode.EDT_None;


        //public static event Delegate_Report_GrayLevel      Report_GrayLevel;
        public event Delegate_Report_GrayLevel_Gray? Report_GrayLevel_Gray;
        public event Delegate_Query_Editing_Mode?    Query_Editing_Mode;

        public event Delegate_Mouse_Up?     CBMouse_Up;

        public List<Control> User_Ctrls
        {
            get => pb_Image.User_Ctrls;
            set => pb_Image.User_Ctrls = value;
        }

        public Control? Editing_Ctrl
        {
            get => pb_Image.Editing_Ctrl;
            set
            {
                if ( pb_Image.Editing_Ctrl != value)
                {
                    // reset old editing control.
                    TNCustCtrl_Rect? rt_ctrl_cur_setting = pb_Image.Editing_Ctrl as TNCustCtrl_Rect;
                    if (rt_ctrl_cur_setting != null)
                        rt_ctrl_cur_setting.Editing = false;
                }

                pb_Image.Editing_Ctrl = value;
                if (null != value)
                {
                    TNCustCtrl_Rect? rt_ctrl_cur_setting = pb_Image.Editing_Ctrl as TNCustCtrl_Rect;
                    if ( null != rt_ctrl_cur_setting )
                        rt_ctrl_cur_setting.Editing = true;
                }
            } 
        }

        public List<Control> Cache_Ctrl
        {
            get => pb_Image.Cache_Ctrl;
            set => pb_Image.Cache_Ctrl = value;
        }

        public Bitmap? Image 
        { 
            get => pb_Image.Image_Bmp;
            set { 
                if (pb_Image != null)
                {
                    pb_Image.Image_Bmp = value;
                    pb_Image.Report_GrayLevel_Gray -= Report_GrayLevel_Gray;
                    pb_Image.Report_GrayLevel_Gray += Report_GrayLevel_Gray;
                }
            }
        }

        public float Image_Scale
        {
            get => pb_Image.Image_Scale;
            set => pb_Image.Image_Scale = value;
        }

        public OpenCvSharp.Point Offset 
        { 
            get => pb_Image.Image_Offset; 
            set => pb_Image.Image_Offset = value;
        }
        public Editing_Mode Editing_mode { get => _editing_mode; set => _editing_mode = value; }

        public UserCtrl_Image()
        {
            InitializeComponent();

            Init_Data();
        }

        private void Init_Data()
        {
            User_Ctrls = new List<Control>();

            //TNUserCtrl_Rect editing_ctrl = new TNUserCtrl_Rect();
            //editing_ctrl.Editing_Rect = new Rectangle(10, 10, 20, 20);
            //Editing_Ctrl = editing_ctrl;

            // 攔 pb_Image 的 mouse event.
            pb_Image.MouseDown += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseDown);
            pb_Image.MouseMove += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseMove);
            pb_Image.MouseUp += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseUp);
            pb_Image.MouseClick += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseClick);
        }

        private void pb_Image_MouseDown(object? sender, MouseEventArgs e)
        {
            // 轉換到 Image 座標系.
            _pt_LBtnDown = pb_Image.GetImagePointFromPB( OpenCVMatTool.ToOpenCvPoint(e.Location) );

            ll_Test.Text = $"{_pt_LBtnDown.X}, {_pt_LBtnDown.Y}, 0, 0";

            if ( null == Query_Editing_Mode )
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                , $"Query_Editing_Mode is null");
                return;
            }

            //if (null != pb_Image.Editing_Ctrl)
            {
                TNCustCtrl_Rect? editing_cttl = Editing_Ctrl as TNCustCtrl_Rect;
                switch (Query_Editing_Mode())
                {
                    case Editing_Mode.EDT_New_ROI:
                    case Editing_Mode.EDT_New_Align:
                    case Editing_Mode.EDT_New_Chip:
                        {
                            if (null != editing_cttl)
                            {
                                OpenCvSharp.Point pt_image = pb_Image.GetImagePointFromPB(_pt_LBtnDown);
                                TNCustCtrl_Rect.DS_Pos_Info pos_info = editing_cttl.Pos_Info;
                                pos_info.Editing_Rect = new Rect(pt_image.X, pt_image.Y, 0, 0);
                                editing_cttl.Pos_Info = pos_info;
                            }
                        }
                        break;

                    case Editing_Mode.EDT_Editing_ROI:
                        {
                            if (null != editing_cttl && editing_cttl.Editing)
                            {
                                editing_cttl.HitTest_Rresult = editing_cttl.HitTest(_pt_LBtnDown);
                                if (HitTest_Result.None != editing_cttl.HitTest_Rresult)
                                {
                                    editing_cttl.Modify_Begin(_pt_LBtnDown);
                                }
                                else
                                {
                                    Mouse_Select_Ctrl();

                                    TNCustCtrl_Rect? new_editing_cttl = Editing_Ctrl as TNCustCtrl_Rect;
                                    if (null != new_editing_cttl && new_editing_cttl.Editing)
                                    {
                                        new_editing_cttl.HitTest_Rresult = new_editing_cttl.HitTest(_pt_LBtnDown);
                                        new_editing_cttl.Modify_Begin(_pt_LBtnDown);
                                    }
                                }

                                pb_Image.Refresh();
                            }
                        }
                        break;

                    default:
                        {
                            Mouse_Select_Ctrl();

                            TNCustCtrl_Rect? new_editing_cttl = Editing_Ctrl as TNCustCtrl_Rect;
                            if (null != new_editing_cttl && new_editing_cttl.Editing)
                            {
                                new_editing_cttl.HitTest_Rresult = new_editing_cttl.HitTest(_pt_LBtnDown);
                                new_editing_cttl.Modify_Begin( _pt_LBtnDown);

                                pb_Image.Refresh();
                            }
                        }
                        break;
                }
                //using (Graphics g = Graphics.FromImage(pb_Image.Image))
                //{

                //    Pen pen = new Pen(Color.White, 1);
                //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
                //}

                //_move_start_pt = e.Location;
                //_move_start_offset = _offset;

                //pb_Image.Editing_Ctrl = editing_cttl;
            }
        }

        private void pb_Image_MouseMove(object? sender, MouseEventArgs e)
        {
            // 轉換到 Image 座標系.
            _pt_Current = pb_Image.GetImagePointFromPB( OpenCVMatTool.ToOpenCvPoint(e.Location) );

            if (e.Button == MouseButtons.Left)
            {
                if (null == Query_Editing_Mode)
                {
                    Log_Utl.Log_Event(Event_Level.Warning
                                    , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"Query_Editing_Mode is null");
                    return;
                }

                TNCustCtrl_Rect? editing_cttl = Editing_Ctrl as TNCustCtrl_Rect;
                switch (Query_Editing_Mode())
                {
                    case Editing_Mode.EDT_New_ROI:
                    case Editing_Mode.EDT_New_Align:
                    case Editing_Mode.EDT_New_Chip:
                        {
                            if (null == editing_cttl)
                            {
                                Log_Utl.Log_Event(Event_Level.Warning
                                                , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                , $"editing_cttl is null");

                                return;
                            }
                            TNCustCtrl_Rect.DS_Pos_Info pos_info = editing_cttl.Pos_Info;
                            pos_info.Editing_Rect = TNCustCtrl_Rect.Normalize(_pt_LBtnDown, _pt_Current);
                            editing_cttl.Pos_Info = pos_info;
                        }
                        break;

                    case Editing_Mode.EDT_Editing_ROI:
                        {
                            // 編輯
                            if (null == editing_cttl)
                            {
                                Log_Utl.Log_Event(Event_Level.Warning
                                                , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                , $"editing_cttl is null");

                                return;
                            }

                            if (editing_cttl.HitTest_Rresult != HitTest_Result.None)
                            {
                                TNCustCtrl_Rect.DS_Pos_Info pos_info = editing_cttl.Pos_Info;
                                pos_info.Editing_Rect = editing_cttl.Modify_Doing(_pt_Current);
                                editing_cttl.Pos_Info = pos_info;
                            }

                            pb_Image.Refresh();
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private void pb_Image_MouseUp(object? sender, MouseEventArgs e)
        {
            // 轉換到 Image 座標系.
            _pt_Current = pb_Image.GetImagePointFromPB(OpenCVMatTool.ToOpenCvPoint(e.Location) );

            if (e.Button == MouseButtons.Left)
            {
                TNCustCtrl_Rect? editing_cttl = (TNCustCtrl_Rect?)pb_Image.Editing_Ctrl;
                if (editing_cttl != null)
                {
                    if (editing_cttl.Editing)
                    {
                        // 編輯
                        editing_cttl.Modify_Done();

                        Apply_Ctrls_To_GlobalSetting();

                        pb_Image.Refresh();
                    }

                    if (null != CBMouse_Up)
                    {
                        CBMouse_Up();
                    }
                }
            }
        }

        private void pb_Image_MouseClick(object? sender, MouseEventArgs e)
        {
            // 轉換到 Image 座標系.
            _pt_Current = pb_Image.GetImagePointFromPB(OpenCVMatTool.ToOpenCvPoint(e.Location) );

            if (null == Query_Editing_Mode)
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                , $"Query_Editing_Mode is null");
                return;
            }

            switch( Query_Editing_Mode() )
            {
                case Editing_Mode.EDT_New_ROI:
                    break;

                case Editing_Mode.EDT_Editing_ROI:
                default:
                    {
                    }
                    break;
            }

            pb_Image.Invalidate();
        }

        private bool Mouse_Select_Ctrl()
        {
            try
            {
                foreach (Control click_ctrl in pb_Image.User_Ctrls)
                {
                    TNCustCtrl_Rect tn_click_ctrl = (TNCustCtrl_Rect)click_ctrl;
                    tn_click_ctrl.Editing = false;
                }

                TNCustCtrl_Rect? ctrl_new_selected = null;
                foreach (Control click_ctrl in pb_Image.User_Ctrls)
                {
                    TNCustCtrl_Rect tn_click_ctrl = (TNCustCtrl_Rect)click_ctrl;
                    if (tn_click_ctrl == null)
                        continue;

                    if (!tn_click_ctrl.Contain(_pt_Current))
                        continue;

                    ctrl_new_selected = tn_click_ctrl;
                    break;
                }

                Editing_Ctrl = ctrl_new_selected;
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

        //protected override void OnPaint(PaintEventArgs pe)
        //{
        //    //pb_Image.Refresh();
        //}

        private void UserCtrl_Image_MouseMove(object sender, MouseEventArgs e)
        {
            _pt_Current = OpenCVMatTool.ToOpenCvPoint(e.Location);
            if (e.Button == MouseButtons.Left)
            {
                TNCustCtrl_Rect? editing_cttl = (TNCustCtrl_Rect?)pb_Image.Editing_Ctrl;
                if (editing_cttl != null)
                {
                    TNCustCtrl_Rect.DS_Pos_Info pos_info = editing_cttl.Pos_Info;
                    pos_info.Editing_Rect = new Rect(Math.Min(_pt_LBtnDown.X, _pt_Current.X)
                          , Math.Min(_pt_LBtnDown.Y, _pt_Current.Y)
                          , Math.Abs(_pt_Current.X - _pt_LBtnDown.X)
                          , Math.Abs(_pt_Current.Y - _pt_LBtnDown.Y));
                    editing_cttl.Pos_Info = pos_info;
                }
            }
        }

        private void UserCtrl_Image_MouseDown(object sender, MouseEventArgs e)
        {
            _pt_LBtnDown = new OpenCvSharp.Point( e.Location.X, e.Location.Y );
        }

        public void Delete_Editing_ROI(Control ctrl_2be_del)
        {
            if (ctrl_2be_del == null)
            {
                Log_Utl.Log_Event(Event_Level.Warning
                            , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                            , $"ctrl_2be_del is null");
                return;
            }

            foreach(Control user_ctrl in User_Ctrls)
            {
                if (user_ctrl != ctrl_2be_del)
                    continue;

                User_Ctrls.Remove(user_ctrl);
                break;
            }

            Editing_Ctrl = null;

            Apply_Ctrls_To_GlobalSetting();

            pb_Image.Redraw_Ctrl();
        }

        public void Apply_GlobalSetting_To_Ctrls()
        {
            if (null == User_Ctrls)
                return;

            User_Ctrls.Clear();

            TNCustCtrl_Rect new_align = new TNCustCtrl_Rect();
            new_align.Display_Color = Color.Green;
            //if (null != tnGlobal.CAM_Info.Align_Info)
            //{
            //    TNCustCtrl_Rect new_align = new TNCustCtrl_Rect();
            //    new_align.Pos_Info.Editing_Rect = tnGlobal.CAM_Info.Align_Info.Align_Rect;
            //    new_align.Display_Color = Color.Green;
            //    //new_detect.Insp_param = tnGlobal.Insp_Param_Pin;
            //    User_Ctrls.Add(new_align);
            //}
            if (tnGlobal.Insp_Pools.Count <= 0)
            {
                new_align.Pos_Info.Editing_Rect = tnGlobal.CAM_Info.Align_Info.Align_Rect;
            }
            else
            {
                for (int insp_id = 0; insp_id < tnGlobal.Insp_Pools.Count; insp_id++)
                {
                    if (!tnGlobal.Insp_Pools[insp_id]._inspected || null == tnGlobal.Insp_Pools[insp_id]._insp_inst)
                        continue;

                    new_align.Pos_Info.Editing_Rect = new OpenCvSharp.Rect(tnGlobal.CAM_Info.Align_Info.Align_Rect.X + tnGlobal.Insp_Pools[insp_id]._insp_inst._align_offset.Width
                                                                          , tnGlobal.CAM_Info.Align_Info.Align_Rect.Y + tnGlobal.Insp_Pools[insp_id]._insp_inst._align_offset.Height
                                                                          , tnGlobal.CAM_Info.Align_Info.Align_Rect.Width, tnGlobal.CAM_Info.Align_Info.Align_Rect.Height);
                }
            }

            if (null != tnGlobal.CAM_Info)
            {
                int pin_idx = 1;
                foreach (DS_CAM_Pin_Info detect_info in tnGlobal.CAM_Info.Detect_Pin_Infos)
                {
                    TNCustCtrl_Rect new_detect = new TNCustCtrl_Rect();
                    new_detect.Pos_Info.Editing_Rect = detect_info.Detect_Rect;
                    new_detect.Display_Color = Color.Blue;

                    new_detect.Insp_param.Pin_Idx = pin_idx;
                    pin_idx++;

                    //if (tnGlobal.Insp_Pools.Count <= 0)
                    //{
                        new_detect.Pos_Info.Editing_Rect = detect_info.Detect_Rect;
                    //}
                    //else
                    //{
                    //    new_detect.Pos_Info.Editing_Rect = new OpenCvSharp.Rect(detect_info.Detect_Rect.X + detect_info._insp_inst._align_offset.Width
                    //                                                          , detect_info.Detect_Rect.Y + detect_info._insp_inst._align_offset.Height
                    //                                                          , detect_info.Detect_Rect.Width, detect_info.Detect_Rect.Height);

                    //}
                    //new_detect.Insp_param = tnGlobal.Insp_Param_Pin;
                    User_Ctrls.Add(new_detect);
                }
            }
        }

        public void Apply_Ctrls_To_GlobalSetting()
        {
            if (null == tnGlobal.CAM_Info)
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                    , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"Detect_Info is null");
                return;
            }

            tnGlobal.CAM_Info.Detect_Pin_Infos.Clear();
            if (null == User_Ctrls)
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                    , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"User_Ctrls is null");
                return;
            }

            foreach (Control user_ctrl in User_Ctrls)
            {
                TNCustCtrl_Rect? rt_user_ctrl = user_ctrl as TNCustCtrl_Rect;
                if ( null == rt_user_ctrl 
                    || rt_user_ctrl.Pos_Info.Editing_Rect.X < 0 || rt_user_ctrl.Pos_Info.Editing_Rect.Y < 0 || rt_user_ctrl.Pos_Info.Editing_Rect.Width <= 0 || rt_user_ctrl.Pos_Info.Editing_Rect.Height <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Warning, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"rt_user_ctrl is null");
                    continue;
                }

                DS_CAM_Pin_Info new_detect_info = new DS_CAM_Pin_Info();
                new_detect_info.Detect_Rect = rt_user_ctrl.Pos_Info.Editing_Rect;
                //new_detect_info.Detect_Insp_param = tnGlobal.Insp_Param_Pin;

                tnGlobal.CAM_Info.Detect_Pin_Infos.Add(new_detect_info);
            }
        }

        public void Camera_CameraImageEvent(Bitmap bmp)
        {
            //pbCamera.Invoke(new MethodInvoker(delegate
            //{
                Bitmap old = Image as Bitmap;
                Image = bmp;
                if (null != old)
                {
                    old.Dispose();
                }
            //}
            //));
        }
    } // end of     public partial class UserCtrl_Image : UserControl

    public enum Editing_Mode
    {
        EDT_None = 0
        , EDT_New_ROI
        , EDT_Editing_ROI
        , EDT_New_Align
        , EDT_New_Chip
    }
}
