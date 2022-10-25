﻿//using CheckOffset.Controls;
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
using static System.Net.Mime.MediaTypeNames;

namespace CheckOffset
{
    //public delegate int Delegate_Report_GrayLevel(Color clr);
    //public delegate int Delegate_Report_GrayLevel_Gray(int x, int y, int gray_level);
    public delegate Editing_Mode Delegate_Query_Editing_Mode();

    public partial class UserCtrl_Image : UserControl
    {
        //private Bitmap? _Image = null;
        //public Bitmap? _show_image = null;
        public Point _pt_LBtnDown; // image 坐標系
        public Point _pt_Current;  // image 坐標系

        //private float _scale = 1.0f;
        //private Point _offset;     // 原始影像座標(_image)顯示的起點: 往右拉，X值為負值，代表影像原點座標要往負方向開始截取.

        Point _move_start_pt = new Point(0, 0);
        Point _move_start_offset = new Point(0, 0);

        Editing_Mode _editing_mode = Editing_Mode.EDT_None;


        //public static event Delegate_Report_GrayLevel      Report_GrayLevel;
        public event Delegate_Report_GrayLevel_Gray? Report_GrayLevel_Gray;
        public event Delegate_Query_Editing_Mode?    Query_Editing_Mode;

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
                    TNUserCtrl_Rect? rt_ctrl_cur_setting = pb_Image.Editing_Ctrl as TNUserCtrl_Rect;
                    if (rt_ctrl_cur_setting != null)
                        rt_ctrl_cur_setting.Editing = false;
                }

                pb_Image.Editing_Ctrl = value;
                if (null != value)
                {
                    TNUserCtrl_Rect? rt_ctrl_cur_setting = pb_Image.Editing_Ctrl as TNUserCtrl_Rect;
                    if ( null != rt_ctrl_cur_setting )
                        rt_ctrl_cur_setting.Editing = true;
                }
            } 
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

        public Point Offset 
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
            _pt_LBtnDown = pb_Image.GetImagePointFromPB(e.Location);

            ll_Test.Text = String.Format("{0}, {1}, {2}, {3}", _pt_LBtnDown.X, _pt_LBtnDown.Y, 0, 0);

            if ( null == Query_Editing_Mode )
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                , $"Query_Editing_Mode is null");
                return;
            }

            //if (null != pb_Image.Editing_Ctrl)
            {
                TNUserCtrl_Rect? editing_cttl = Editing_Ctrl as TNUserCtrl_Rect;
                switch (Query_Editing_Mode())
                {
                    case Editing_Mode.EDT_New_ROI:
                        {
                            if (null != editing_cttl)
                            {
                                Point pt_image = pb_Image.GetImagePointFromPB(_pt_LBtnDown);
                                editing_cttl.Editing_Rect = new Rectangle(pt_image.X, pt_image.Y, 0, 0);
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

                                    TNUserCtrl_Rect? new_editing_cttl = Editing_Ctrl as TNUserCtrl_Rect;
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

                            TNUserCtrl_Rect? new_editing_cttl = Editing_Ctrl as TNUserCtrl_Rect;
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
            _pt_Current = pb_Image.GetImagePointFromPB(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                if (null == Query_Editing_Mode)
                {
                    Log_Utl.Log_Event(Event_Level.Warning
                                    , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"Query_Editing_Mode is null");
                    return;
                }

                TNUserCtrl_Rect? editing_cttl = Editing_Ctrl as TNUserCtrl_Rect;
                switch (Query_Editing_Mode())
                {
                    case Editing_Mode.EDT_New_ROI:
                        {
                            if (null == editing_cttl)
                            {
                                Log_Utl.Log_Event(Event_Level.Warning
                                                , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                                , $"editing_cttl is null");

                                return;
                            }
                            editing_cttl.Editing_Rect = TNUserCtrl_Rect.Normalize(_pt_LBtnDown, _pt_Current);
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
                                editing_cttl.Editing_Rect = editing_cttl.Modify_Doing(_pt_Current);

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
            _pt_Current = pb_Image.GetImagePointFromPB(e.Location);

            if (e.Button == MouseButtons.Left)
            {
                TNUserCtrl_Rect? editing_cttl = (TNUserCtrl_Rect?)pb_Image.Editing_Ctrl;
                if (editing_cttl != null)
                {
                    if (editing_cttl.Editing)
                    {
                        // 編輯
                        editing_cttl.Modify_Done();

                        Apply_Ctrls_To_GlobalSetting();

                        pb_Image.Refresh();
                    }
                }
            }
        }

        private void pb_Image_MouseClick(object? sender, MouseEventArgs e)
        {
            // 轉換到 Image 座標系.
            _pt_Current = pb_Image.GetImagePointFromPB(e.Location);

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
                    TNUserCtrl_Rect tn_click_ctrl = (TNUserCtrl_Rect)click_ctrl;
                    tn_click_ctrl.Editing = false;
                }

                TNUserCtrl_Rect? ctrl_new_selected = null;
                foreach (Control click_ctrl in pb_Image.User_Ctrls)
                {
                    TNUserCtrl_Rect tn_click_ctrl = (TNUserCtrl_Rect)click_ctrl;
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
                   , string.Format("Exception catched: error:{0}", ex.Message));
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
            _pt_Current = e.Location;
            if (e.Button == MouseButtons.Left)
            {
                TNUserCtrl_Rect? editing_cttl = (TNUserCtrl_Rect?)pb_Image.Editing_Ctrl;
                if (editing_cttl != null)
                {
                    editing_cttl.Editing_Rect = new Rectangle(Math.Min(_pt_LBtnDown.X, _pt_Current.X)
                          , Math.Min(_pt_LBtnDown.Y, _pt_Current.Y)
                          , Math.Abs(_pt_Current.X - _pt_LBtnDown.X)
                          , Math.Abs(_pt_Current.Y - _pt_LBtnDown.Y));
                }
            }
        }

        private void UserCtrl_Image_MouseDown(object sender, MouseEventArgs e)
        {
            _pt_LBtnDown = e.Location;
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

            if (null != tnGlobal.Detect_Infos)
            {
                foreach (Struct_Detect_Info detect_info in tnGlobal.Detect_Infos)
                {
                    TNUserCtrl_Rect new_detect = new TNUserCtrl_Rect();
                    new_detect.Editing_Rect = detect_info.Detect_Rect;
                    new_detect.Insp_param = detect_info.Detect_Insp_param;
                    User_Ctrls.Add(new_detect);
                }
            }
        }

        public void Apply_Ctrls_To_GlobalSetting()
        {
            if (null == tnGlobal.Detect_Infos)
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                    , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"Detect_Infos is null");
                return;
            }

            tnGlobal.Detect_Infos.Clear();
            if (null == User_Ctrls)
            {
                Log_Utl.Log_Event(Event_Level.Warning
                                    , System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"User_Ctrls is null");
                return;
            }

            foreach (Control user_ctrl in User_Ctrls)
            {
                TNUserCtrl_Rect? rt_user_ctrl = user_ctrl as TNUserCtrl_Rect;
                if ( null == rt_user_ctrl 
                    || rt_user_ctrl.Editing_Rect.X < 0 || rt_user_ctrl.Editing_Rect.Y < 0 || rt_user_ctrl.Editing_Rect.Width <= 0 || rt_user_ctrl.Editing_Rect.Height <= 0)
                {
                    Log_Utl.Log_Event(Event_Level.Warning, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"rt_user_ctrl is null");
                    continue;
                }

                Struct_Detect_Info new_detect_info = new Struct_Detect_Info();
                new_detect_info.Detect_Rect = rt_user_ctrl.Editing_Rect;
                new_detect_info.Detect_Insp_param = rt_user_ctrl.Insp_param;

                tnGlobal.Detect_Infos.Add(new_detect_info);
            }
        }
    } // end of     public partial class UserCtrl_Image : UserControl

    public enum Editing_Mode
    {
        EDT_None = 0
        , EDT_New_ROI
        , EDT_Editing_ROI
    }
}