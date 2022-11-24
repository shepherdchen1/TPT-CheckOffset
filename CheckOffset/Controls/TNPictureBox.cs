//using CheckOffset.Controls;
using TNControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TN.ImageTools;
using TN.Tools.Debug;
using System.Runtime.InteropServices;
using System.Reflection;
//using CheckOffset.Controls;
using CheckOffset.ImageTools;

//namespace CheckOffset.Controls
namespace TNControls
{
    public delegate int Delegate_Report_GrayLevel_Gray(Report_Display_Info report_display_info);

    public partial class TNPictureBox : PictureBox
    {
        public TNPictureBox()
        {
            InitializeComponent();

            Init_Data();
        }

        private void Init_Data()
        {
            Editing_Ctrl = null;
            Image = null;

            Show_Image = null;
            Show_Color_Image = null;

            MouseWheel += new MouseEventHandler(PB_MouseWheel);
            MouseDown += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseDown);
            MouseMove += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseMove);
            MouseUp += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseUp);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            //Draw_Ctrl_Refresh();
        }

        //////////////////////////////////////////////////
        /// data member...
        private Bitmap? _image      = null; // 8bit bitmap original file's bitmap. 整張影像
        private Bitmap? _show_image = null; // 8bit bitmap scaled display bitmap only visible rect. 顯示部分影像
        private Bitmap? _show_color_image = null; // transfer _show_image to color bitmap.
        public Point _pt_LBtnDown;
        public Point _pt_Current;

        private float _scale = 1.0f;
        private OpenCvSharp.Point _offset;     // 原始影像座標(_image)顯示的起點: 往右拉，X值為負值，代表影像原點座標要往負方向開始截取.

        bool _busy = false;
        bool _enable_zoom = true;

        OpenCvSharp.Point _move_start_pt     = new OpenCvSharp.Point(0, 0);
        OpenCvSharp.Point _move_start_offset = new OpenCvSharp.Point(0, 0);

        // for _user_ctrls should be none null, for compile no warning.
        private List<Control> _user_ctrls = new List<Control>();
        private List<Control> _cache_ctrls = new List<Control>();

        public event Delegate_Report_GrayLevel_Gray? Report_GrayLevel_Gray;

        public Control? Editing_Ctrl
        {
            get;
            set;
        }

        public List<Control> User_Ctrls
        {
            get => _user_ctrls;
            set => _user_ctrls = value;
        }

        public List<Control> Cache_Ctrl
        {
            get => _cache_ctrls;
            set => _cache_ctrls = value;
        }

        public float Image_Scale 
        { 
            get => _scale;
            set
            {
                _scale = value;
                DrawImageToBuffer();
                Redraw_Ctrl();
            }
        }
        public OpenCvSharp.Point Image_Offset 
        { 
            get => _offset;
            set
            {
                _offset = value;
                DrawImageToBuffer();
                Redraw_Ctrl();
            }
        }
        public Bitmap? Image_Bmp 
        { 
            get => _image; 
            set {
                _image = value;

                DrawImageToBuffer();
                Redraw_Ctrl();
            }
        }

        private Bitmap? Show_Image
        {
            get => _show_image;
            set
            {
                if (null != _show_image)
                    _show_image.Dispose();

                _show_image = value;
            }
        }

        private Bitmap? Show_Color_Image
        {
            get => _show_color_image;
            set
            {
                if (null != _show_color_image)
                    _show_color_image.Dispose();

                _show_color_image = value;
            }
        }

        //////////////////////////////////////////////////
        /// member function...
        private void PB_MouseWheel(object? sender, System.Windows.Forms.MouseEventArgs e)
        {
            //base.MouseWheel(sender, e);
            double scale = 1;
            if (Height > 0)
            {
                scale = (double)Width / (double)Height;
            }
            Width += (int)(e.Delta * scale);
            Height += e.Delta;

            if (_enable_zoom && !_busy)
            {
                _busy = true;

                //Point center_pt = GetImagePointFromPB(new Point(Width / 2, Height / 2));
                // zoomin/out by cursor
                OpenCvSharp.Point center_pt = GetImagePointFromPB(OpenCVMatTool.ToOpenCvPoint(e.Location ) );

                if (e.Delta > 0)
                    ZoomIn(OpenCVMatTool.ToOpenCvPoint(e.Location /*center_pt*/) );
                else if (e.Delta < 0)
                    ZoomOut(OpenCVMatTool.ToOpenCvPoint(e.Location/*center_pt*/) );

                _busy = false;
            }

            //PB_Image_Refresh();
        }

        private void pb_Image_MouseDown(object? sender, MouseEventArgs e)
        {
            _pt_LBtnDown = e.Location;
            //ll_Test.Text = $"{0}, {1}, {2}, {3}", _pt_LBtnDown.X, _pt_LBtnDown.Y, 0, 0);
            if (null != Editing_Ctrl)
            {
                TNCustCtrl_Rect editing_cttl = (TNCustCtrl_Rect)Editing_Ctrl;
                //using (Graphics g = Graphics.FromImage(pb_Image.Image))
                //{

                //    Pen pen = new Pen(Color.White, 1);
                //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
                //}
            }

            _move_start_pt      = OpenCVMatTool.ToOpenCvPoint(e.Location);
            _move_start_offset = _offset;
        }

        private void pb_Image_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _busy = true;

                _offset = _move_start_offset;
                _offset.X += (Int32)((_move_start_pt.X - e.Location.X) / _scale);
                _offset.Y += (Int32)((_move_start_pt.Y - e.Location.Y) / _scale);

                _offset.X = Math.Max(_offset.X, 0);
                _offset.Y = Math.Max(_offset.Y, 0);
                //OffsetChanged(this, gcnew EventArgs());

                DrawImageToBuffer();
                Redraw_Ctrl();

                _busy = false;
            }
            else if (e.Button == MouseButtons.Left)
            {
                _pt_Current = PointToClient(e.Location);

                Redraw_Ctrl();
                //DrawXORRectangle
                //_pt_Current = e.Location;

                //ll_Test.Text = $"{0}, {1}, {2}, {3}", _pt_LBtnDown.X, _pt_LBtnDown.Y, _pt_Current.X, _pt_Current.Y);

                //if (null != Editing_Ctrl)
                //{
                //    PB_Image_Refresh();
                //}
            }

            Report_Cursor_Gray_Level(e);
        }

        private void pb_Image_MouseUp(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _busy = true;

                _busy = false;
            }
            else if (e.Button == MouseButtons.Left)
            {
                _pt_Current = PointToClient(e.Location);

                Redraw_Ctrl();
                //DrawXORRectangle
                //_pt_Current = e.Location;

                //ll_Test.Text = $"{0}, {1}, {2}, {3}", _pt_LBtnDown.X, _pt_LBtnDown.Y, _pt_Current.X, _pt_Current.Y);

                //if (null != Editing_Ctrl)
                //{
                //    PB_Image_Refresh();
                //}
            }

            Report_Cursor_Gray_Level(e);
        }

        //private void PB_Image_Refresh()
        //{
        //    //pb_Image.Refresh();
        //    //Refresh();

        //    Bitmap color_bmp = new Bitmap(Width, Height);


        //    if (null != User_Ctrls && User_Ctrls.Count > 0)
        //    {
        //        Pen pen = new Pen(Color.White, 1);

        //        foreach (Control enu_ctrl in User_Ctrls)
        //        {
        //            TNUserCtrl_Rect user_ctrl = (TNUserCtrl_Rect)enu_ctrl;
        //            Rectangle show_rect = CalcShowRect();  // 要畫在UI上的Image區域
        //            Point pt_offset = new Point(-show_rect.X, -show_rect.Y);
        //            Rectangle rt_draw = user_ctrl.Editing_Rect;
        //            rt_draw.Offset(pt_offset);
        //            Rectangle rt_final_draw = new Rectangle((int)(rt_draw.Left * _scale), (int)(rt_draw.Top * _scale)
        //                                                    , (int)(rt_draw.Width * _scale), (int)(rt_draw.Height * _scale));
        //            CreateGraphics().DrawRectangle(pen, rt_final_draw);
        //        }
        //    }

        //    if (null != Editing_Ctrl)
        //    {
        //        TNUserCtrl_Rect editing_cttl = (TNUserCtrl_Rect)Editing_Ctrl;
        //        //using (Graphics g = Graphics.FromImage(pb_Image.Image))
        //        //{

        //        //    Pen pen = new Pen(Color.White, 1);
        //        //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
        //        //}

        //        //editing_cttl.Editing_Rect = new Rectangle(Math.Min(_pt_LBtnDown.X, _pt_Current.X)
        //        //                      , Math.Min(_pt_LBtnDown.Y, _pt_Current.Y)
        //        //                      , Math.Abs(_pt_Current.X - _pt_LBtnDown.X)
        //        //                      , Math.Abs(_pt_Current.Y - _pt_LBtnDown.Y));

        //        //Bitmap color_bmp = new Bitmap(pb_Image.Image.Width, pb_Image.Image.Height);

        //        using (Graphics g = Graphics.FromImage(color_bmp))
        //        {
        //            if (null != Show_Image)
        //            {
        //                g.DrawImageUnscaled(Show_Image, 0, 0);
        //                Image = _show_image;
        //                //_show_image.Save("d:\\temp\\test.bmp");
        //            }

        //            Pen pen = new Pen(Color.White, 1);
        //            //g.DrawRectangle(pen, editing_cttl.Editing_Rect);

        //            CreateGraphics().DrawRectangle(pen, editing_cttl.Editing_Rect);
        //        }
        //    }

        //    //Image = _show_image;
        //}

        private void pb_Image_Paint(object sender, PaintEventArgs e)
        {
            if (null != User_Ctrls)
            {
                foreach (UserControl user_ctrl in User_Ctrls)
                {
                    user_ctrl.Invalidate();
                }
            }

            if (null != Editing_Ctrl)
            {
                Editing_Ctrl.Invalidate();
            }
        }

        private void Report_Cursor_Gray_Level(MouseEventArgs e)
        {
            System.Drawing.Imaging.BitmapData? bmp_data = null;
            try
            {
                if (null == Image)
                    return;

                //Point pt_Current = pb_Image.PointToClient(e.Location);
                //Point pt_Current_screen = pb_Image.PointToScreen(e.Location);
                Point pt_Current = e.Location;

                if (pt_Current.X < 0 || pt_Current.Y < 0 || pt_Current.X >= Image.Width || pt_Current.Y >= Image.Height)
                    return;

                if (!Image_Buffer_Gray.GetBuffer((Bitmap)Image, ref bmp_data))
                    return;

                if (null == bmp_data)
                    return;

                IntPtr ptr_buffer = bmp_data.Scan0;

                int pixel_size = bmp_data.Stride / bmp_data.Width;
                int gray_level = 0;
                int r_val = 0, g_val = 0, b_val = 0;
                unsafe
                {
                    byte* ptr = (byte*)ptr_buffer.ToPointer();
                    //gray_level = (int)*(ptr + pt_Current.Y * bmp_data.Stride + pt_Current.X * pixel_size);
                    gray_level = (int)(ptr[pt_Current.Y * bmp_data.Stride + pt_Current.X * pixel_size]);
                    if (pixel_size >= 3 )
                    {
                        b_val = (int)(ptr[pt_Current.Y * bmp_data.Stride + pt_Current.X * pixel_size]);
                        g_val = (int)(ptr[pt_Current.Y * bmp_data.Stride + pt_Current.X * pixel_size + 1]);
                        r_val = (int)(ptr[pt_Current.Y * bmp_data.Stride + pt_Current.X * pixel_size + 2]);
                    }
                }

                //Image_Buffer_Gray.ReleaseBuffer((Bitmap)Image, bmp_data);
                //bmp_data = null;

                Point pt_image = new Point((int)(pt_Current.X / _scale + _offset.X), (int)(pt_Current.Y / _scale + _offset.Y));

                Report_Display_Info report_display_info = new Report_Display_Info
                {
                    Pos = pt_image
                                
                    , Gray_Level = gray_level
                                                
                    , B_Val = b_val                                
                    , G_Val = g_val
                    , R_Val = r_val

                    , Scale = _scale
                };
                if (null != Report_GrayLevel_Gray)
                {
                    Report_GrayLevel_Gray(report_display_info);
                }
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                                    , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
                if (null != bmp_data)
                    Image_Buffer_Gray.ReleaseBuffer((Bitmap)Image, ref bmp_data);
            }
        }

        public OpenCvSharp.Point GetImagePointFromPB(OpenCvSharp.Point pt)
        {
            OpenCvSharp.Point real_pt = new OpenCvSharp.Point((Int32)Math.Floor(pt.X / _scale + _offset.X)
                                                         , (Int32)Math.Floor(pt.Y / _scale + _offset.Y));
            return real_pt;
        }

        public Rectangle GetImageRectFromPB(Rectangle rt)
        {
            return new Rectangle((Int32)Math.Floor(rt.X / _scale + _offset.X)
                               , (Int32)Math.Floor(rt.Y / _scale + _offset.Y)
                               , (Int32)Math.Floor(rt.Width / _scale)
                               , (Int32)Math.Floor(rt.Height / _scale));
        }

        public Point GetPBPointFromImage(Point pt)
        {
            Point pb_pt = new Point((Int32)Math.Round((pt.X - _offset.X) * _scale, MidpointRounding.AwayFromZero)
                                    , (Int32)Math.Round((pt.Y - _offset.Y) * _scale, MidpointRounding.AwayFromZero));
            return pb_pt;
        }

        public Rectangle GetPBRectFromImage(Rectangle rt)
        {
            return new Rectangle((Int32)Math.Round((rt.X - _offset.X) * _scale, MidpointRounding.AwayFromZero)
                                 , (Int32)Math.Round((rt.Y - _offset.Y) * _scale, MidpointRounding.AwayFromZero)
                                 , (Int32)Math.Round(rt.Width * _scale,  MidpointRounding.AwayFromZero)
                                 , (Int32)Math.Round(rt.Height * _scale, MidpointRounding.AwayFromZero));
        }

        OpenCvSharp.Point CalcOffset(OpenCvSharp.Point display_cursor_pt, float old_scale, float new_scale)
        {
            // 1. 以原有倍率取得滑鼠座標 在 _image 坐標系的位置
            _scale = old_scale;
            OpenCvSharp.Point pt_new = GetImagePointFromPB(display_cursor_pt);
            _scale = new_scale;

            // 轉到新倍率坐標系
            pt_new.X = (int)(pt_new.X * new_scale);
            pt_new.Y = (int)(pt_new.Y * new_scale);

            // 找到新坐標系顯示左上角原點
            pt_new.X -= display_cursor_pt.X;
            pt_new.Y -= display_cursor_pt.Y;

            // 找到新坐標系原點在 _image 坐標系位置
            pt_new.X = (int)(pt_new.X / new_scale);
            pt_new.Y = (int)(pt_new.Y / new_scale);

            pt_new.X = Math.Max(pt_new.X, 0);
            pt_new.Y = Math.Max(pt_new.Y, 0);

            return pt_new;
        }

        float GetBestFitScale()
        {
            if (null != _image)
            {
                float scale_x = ClientSize.Width / (float)_image.Width;
                float scale_y = ClientSize.Height / (float)_image.Height;

                return ((scale_x > scale_y) ? scale_y : scale_x);
            }

            return 1.0f;
        }

        public void ZoomFit()
        {
            _scale = GetBestFitScale();
            _offset = new OpenCvSharp.Point(0, 0);
            DrawImageToBuffer();
            Redraw_Ctrl();
            //pb_Image.Image = _show_image;
        }

        private void ZoomIn(OpenCvSharp.Point display_cursor_pt)
        {
            float old_scale = _scale;
            _scale *= 1.25f;

            if (old_scale != _scale)
            {
                _offset = CalcOffset(display_cursor_pt, old_scale, _scale);
                //ScaleChanged(this, new EventArgs());

                DrawImageToBuffer();
                Redraw_Ctrl();
                //PB_Image_Refresh();

                //buf_g->DrawImageUnscaled(_show_image, 0, 0);
            }

            //OffsetChanged(this, new EventArgs());
        }

        private void ZoomOut(OpenCvSharp.Point display_cursor_pt)
        {
            float old_scale = _scale;
            _scale /= 1.25f;

            if (old_scale != _scale)
            {
                //_offset = CalcOffset(pt_center);
                _offset = CalcOffset(display_cursor_pt, old_scale, _scale);
                //ScaleChanged(this, new EventArgs());

                DrawImageToBuffer();
                Redraw_Ctrl();
                //PB_Image_Refresh();
            }

            //OffsetChanged(this, new EventArgs());
        }

        int GetPixelLength(System.Drawing.Imaging.PixelFormat format)
        {
            switch (format)
            {
                case System.Drawing.Imaging.PixelFormat.Format32bppArgb:
                    return 4;

                case System.Drawing.Imaging.PixelFormat.Format24bppRgb:
                    return 3;

                case System.Drawing.Imaging.PixelFormat.Format8bppIndexed:
                    return 1;

                default:
                    return 0;
            }
        }

        System.Drawing.Rectangle CalcShowRect()
        {
            System.Drawing.Rectangle show_rect = new Rectangle(0, 0, 0, 0);
            //System.Drawing.Rectangle show_rect = new Rectangle(_offset.X, _offset.Y, 0, 0);

            if (null == Image_Bmp)
            {
                Log_Utl.Log_Event(Event_Level.Error, MethodBase.GetCurrentMethod()?.Name
                    , $"Image_Bmp is null");
                return show_rect;
            }

            //show_rect.Location.X = _offset.X;
            //show_rect.Location.Y = _offset.Y;
            //show_rect.Width = (Int32)(ClientSize.Width / _scale + 0.5f);
            ////show_rect.Width = Math.Max(show_rect.Width, Show_Image.Width);
            //show_rect.Width = Math.Max(show_rect.Width, Image_Bmp.Width - _offset.X);

            int width = (Int32)(ClientSize.Width / _scale + 0.5f);
            //show_rect.Width = Math.Max(show_rect.Width, Show_Image.Width);
            width = Math.Max(width, (int) ( (Image_Bmp.Width - _offset.X) / _scale + 0.5f ) );

            int height = (Int32)(ClientSize.Height / _scale + 0.5f);
            height = Math.Max(show_rect.Height, (int) (  ( Image_Bmp.Height - _offset.Y) / _scale + 0.5f ) );

            show_rect = new Rectangle( _offset.X, _offset.Y, width, height );

            //show_rect.Height = (Int32)(ClientSize.Height / _scale + 0.5f);
            //show_rect.Height = Math.Max(show_rect.Height, Image_Bmp.Height - _offset.Y); 

            return show_rect;
        }

        System.Drawing.Rectangle CalcLockRect()
        {
            System.Drawing.Rectangle show_rect = CalcShowRect();
            if (null != _image)
            {
                System.Drawing.Rectangle img_rect = new System.Drawing.Rectangle(0, 0, _image.Width, _image.Height);
                show_rect = System.Drawing.Rectangle.Intersect(show_rect, img_rect);
            }
            return show_rect;
        }

        void DrawImageToBuffer()
        {
            try
            {
                if (null == _image)
                {
                    //if (_show_image)
                    //{
                    //    delete _show_image;
                    //    _show_image = nullptr;
                    //}
                    return;
                }

                // Create new bitmap
                Show_Image = new Bitmap(ClientSize.Width, ClientSize.Height, _image.PixelFormat);
                if (null == _show_image)
                {
                    Log_Utl.Log_Event(Event_Level.Error, MethodBase.GetCurrentMethod()?.Name
                        , $"_show_image is null");
                    return;
                }

                Rectangle lock_rect = CalcLockRect();  // 取得要Lock的影像範圍
                Rectangle show_rect = CalcShowRect();  // 要畫在UI上的Image區域

                if (lock_rect.Width == 0 || lock_rect.Height == 0)
                    return;

                Int32 bytes_per_pixel = GetPixelLength(_image.PixelFormat);

                if (bytes_per_pixel == 0)
                    return;

                //_image.Save("d:\\temp\\Source.bmp");

                // Get raw data
                System.Drawing.Imaging.BitmapData fromBitmapData = _image.LockBits(lock_rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, _image.PixelFormat);
                System.Drawing.Imaging.BitmapData destBitmapData = Show_Image.LockBits(new Rectangle(0, 0, _show_image.Width, _show_image.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, _show_image.PixelFormat);

                unsafe
                {
                    byte* fro_ptr = (byte*)(fromBitmapData.Scan0.ToPointer());
                    byte* dest_ptr = (byte*)(destBitmapData.Scan0.ToPointer());
                    Int32 fro_pitch = fromBitmapData.Stride;
                    Int32 dest_pitch = destBitmapData.Stride;

                    Int32 copy_width = Show_Image.Width;
                    Int32 copy_height = Show_Image.Height;
                    byte* fro_line_ptr = fro_ptr;        // 來源端某行的位置
                    byte* dest_line_ptr = dest_ptr;        // 目的端某行的位置
                    byte* fro_now_ptr = fro_line_ptr;    // 來源端目前的位置
                    byte* dest_now_ptr = dest_line_ptr;    // 目的端目前的位置

                    Int32 offset_h = 0; // 來源端與目的端高度offset

                    for (int h = 0; h < copy_height; h++)
                    {
                        Int32 fro_height = (Int32)(h / _scale + 0.5) - lock_rect.Y;

                        if (show_rect.Y + fro_height < 0)
                        {
                            offset_h++;
                            continue;
                        }

                        // 是否已達bottom.
                        if (show_rect.Y + fro_height >= lock_rect.Height)
                            break;

                        fro_line_ptr = fro_ptr + fro_pitch * (Int32)(((h - offset_h) / _scale));
                        //fro_line_ptr = fro_ptr + fro_pitch * (Int32)(((h - offset_h) / _scale) + lock_rect.Y);
                        // 1018- dest_line_ptr = dest_ptr + dest_pitch * h;
                        dest_line_ptr = dest_ptr + dest_pitch * (Int32)(h - offset_h);
                        fro_now_ptr = fro_line_ptr;
                        dest_now_ptr = dest_line_ptr;

                        Int32 offset_w = 0; // 來源端與目的端寬度offset
                        Int32 show_rect_x = show_rect.X;
                        Int32 lock_rect_x = lock_rect.X;

                        for (int w = 0; w < copy_width; w++)
                        {
                            Int32 fro_weight = (Int32)(w / _scale + 0.5) - lock_rect_x;

                            if (show_rect_x + fro_weight < 0)
                            {
                                offset_w++;
                                //1018-   dest_now_ptr += bytes_per_pixel;
                                continue;
                            }
                            else if (show_rect_x + fro_weight >= lock_rect.Width)
                                break;

                            fro_now_ptr = fro_line_ptr + (Int32)((w - offset_w) / _scale) * bytes_per_pixel;
                            //fro_now_ptr = fro_line_ptr + (Int32)((w - offset_w) / _scale + lock_rect_x) * bytes_per_pixel;
                            for (int n = 0; n < bytes_per_pixel; n++)
                                dest_now_ptr[n] = fro_now_ptr[n];

                            dest_now_ptr += bytes_per_pixel;
                        }
                    }
                }

                _image.UnlockBits(fromBitmapData);
                Show_Image.UnlockBits(destBitmapData);

                if (_image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                    Show_Image.Palette = _image.Palette;

                //_show_image.Save("d:\\temp\\test.bmp");
                Show_Color_Image = new Bitmap(Show_Image.Width, Show_Image.Height);

                //Image = _show_image;
                Invalidate();
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , $"Exception catched: error:{ex.Message}");
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
        }

        public void Redraw_Ctrl()
        {
            if (null == Show_Image)
                return;

            Show_Color_Image = new Bitmap(Show_Image.Width, Show_Image.Height);
            using (Graphics graphics_show = Graphics.FromImage(Show_Color_Image))
            {
                graphics_show.DrawImage(Show_Image, 0, 0);
                //_show_image.Save("d:\\temp\\test1.bmp");
                //_show_color_image.Save("d:\\temp\\testcolor1.bmp");

                Pen pen = new Pen(Color.White, 1);

                if (null != User_Ctrls && User_Ctrls.Count > 0)
                {
                    Pen pen_user_ctrl = new Pen(Color.White, 1);

                    foreach (Control enu_ctrl in User_Ctrls)
                    {
                        Redraw_Ctrl(enu_ctrl, graphics_show);

                        //TNCustCtrl_Rect user_ctrl = (TNCustCtrl_Rect)enu_ctrl;
                        //Rectangle show_rect = CalcShowRect();  // 要畫在UI上的Image區域
                        //Point pt_offset = new Point(-show_rect.X, -show_rect.Y);
                        //TNCustCtrl_Rect.DS_Pos_Info pos_info = user_ctrl.Pos_Info;
                        //Rectangle rt_draw = pos_info.Editing_Rect;
                        //rt_draw.Offset(pt_offset);
                        //Rectangle rt_final_draw = new Rectangle((int)(rt_draw.Left * _scale), (int)(rt_draw.Top * _scale)
                        //                                        , (int)(rt_draw.Width * _scale), (int)(rt_draw.Height * _scale));

                        //user_ctrl.Draw2PB(graphics_show, this);
                    }
                }

                if (null != Editing_Ctrl)
                {
                    TNCustCtrl_Rect editing_cttl = (TNCustCtrl_Rect)Editing_Ctrl;
                    //using (Graphics g = Graphics.FromImage(pb_Image.Image))
                    //{

                    //    Pen pen = new Pen(Color.White, 1);
                    //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
                    //}

                    //editing_cttl.Editing_Rect = new Rectangle(Math.Min(_pt_LBtnDown.X, _pt_Current.X)
                    //                      , Math.Min(_pt_LBtnDown.Y, _pt_Current.Y)
                    //                      , Math.Abs(_pt_Current.X - _pt_LBtnDown.X)
                    //                      , Math.Abs(_pt_Current.Y - _pt_LBtnDown.Y));

                    //Bitmap color_bmp = new Bitmap(pb_Image.Image.Width, pb_Image.Image.Height);
                    //Bitmap color_bmp = new Bitmap(Width, Height);

                    //using (Graphics g = Graphics.FromImage(color_bmp))
                    //{
                        if (null != Show_Image)
                        {
                            //g.DrawImageUnscaled(_show_image, 0, 0);
                            //Image = _show_image;
                            //_show_image.Save("d:\\temp\\test.bmp");
                        }

                        Pen pen_edit_ctrl = new Pen(Color.White, 1);
                        //g.DrawRectangle(pen, editing_cttl.Editing_Rect);

                        //CreateGraphics().DrawRectangle(pen, editing_cttl.Editing_Rect);
                        //graphics_show.DrawRectangle(pen_edit_ctrl, editing_cttl.Editing_Rect);

                    editing_cttl.Draw2PB(graphics_show, this);
                    //}
                }

                if (null != Cache_Ctrl && Cache_Ctrl.Count > 0)
                {
                    Pen pen_user_ctrl = new Pen(Color.White, 1);

                    foreach (Control enu_ctrl in Cache_Ctrl)
                    {
                        Redraw_Ctrl(enu_ctrl, graphics_show);         
                    }
                }

                if (Image_Scale > 30)
                    DrawGrayLevel2PB(graphics_show, this);
            }

            //_show_color_image.Save("d:\\temp\\testcolor.bmp");
            //if (null != Image)
            //    Image.Dispose();

            if (null != Image)
            {
                Image.Dispose();
            }

            Bitmap tempBitmap = new Bitmap(Show_Color_Image);
            Image = tempBitmap;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="graphics_show"></param>
        /// <param name="pb"></param>
        public void Redraw_Ctrl(Control enu_ctrl, Graphics graphics_show)
        {
            if (enu_ctrl.GetType() == typeof(TNCustCtrl_Points))
            {
                TNCustCtrl_Points user_ctrl = (TNCustCtrl_Points)enu_ctrl;

                user_ctrl.Draw2PB(graphics_show, this);
            }
            else if (enu_ctrl.GetType() == typeof(TNCustCtrl_String))
            {
                TNCustCtrl_String user_ctrl = (TNCustCtrl_String)enu_ctrl;

                user_ctrl.Draw2PB(graphics_show, this);
            }
            else if (enu_ctrl.GetType() == typeof(TNCustCtrl_Rect))
            {
                TNCustCtrl_Rect user_ctrl = (TNCustCtrl_Rect)enu_ctrl;

                user_ctrl.Draw2PB(graphics_show, this);
            }
            else if (enu_ctrl.GetType() == typeof(TNCustCtrl_Lines))
            {
                TNCustCtrl_Lines user_ctrl = (TNCustCtrl_Lines)enu_ctrl;

                user_ctrl.Draw2PB(graphics_show, this);
            }
            else if (enu_ctrl.GetType() == typeof(TNCustCtrl_Line))
            {
                TNCustCtrl_Line user_ctrl = (TNCustCtrl_Line)enu_ctrl;

                user_ctrl.Draw2PB(graphics_show, this);
            }
            else if (enu_ctrl.GetType() == typeof(TNCustCtrl_Polygon))
            {
                TNCustCtrl_Polygon user_ctrl = (TNCustCtrl_Polygon)enu_ctrl;

                user_ctrl.Draw2PB(graphics_show, this);
            }
        }

        public void DrawGrayLevel2PB(Graphics graphics_show, TNPictureBox pb)
        {
            System.Drawing.Imaging.BitmapData? bmp_data = null;
            if (!Image_Buffer_Gray.GetBuffer((Bitmap?)_image, ref bmp_data))
                return;

            if (null == bmp_data)
            {
                Log_Utl.Log_Event(Event_Level.Error, MethodBase.GetCurrentMethod()?.Name
                    , $"bmp_data is null");
                return;
            }

            Font draw_font = new Font("Arial", 8);
            Brush brush_ctrl = new SolidBrush(Color.Gray);
            Rectangle visible_rect = CalcShowRect();  // 要畫在UI上的Image區域
            unsafe
            {
                for (int x = visible_rect.Left; x < visible_rect.Right; x++)
                {
                    for (int y = visible_rect.Top; y < visible_rect.Bottom; y++)
                    {
                        Point pt_draw = pb.GetPBPointFromImage(new Point(x, y));
                        Rectangle rt = new Rectangle(pt_draw.X, pt_draw.Y, 1, 1);

                        //int gray_level = *(Image_Buffer_Gray.Get_Pointer(bmp_data, (byte*)bmp_data.Scan0.ToPointer()
                        //                                                , x, y));
                        int gray_level = 0;
                        if (bmp_data.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed )
                        {
                            gray_level = Image_Buffer_Gray.Get_Pixel(bmp_data, (byte*)bmp_data.Scan0.ToPointer()
                                                    , x, y);
                        }
                        else
                        {
                            gray_level = ImageBuffer.Get_Pixel(bmp_data, (byte*)bmp_data.Scan0.ToPointer()
                                                    , x, y);
                        }
                        graphics_show.DrawString($"{gray_level}", draw_font, brush_ctrl, pt_draw);
                    }
                }
            }

            if (null != bmp_data)
                Image_Buffer_Gray.ReleaseBuffer((Bitmap?)_image, ref bmp_data);
        }

        static public void DrawXORRectangle(Graphics graphics, Pen pen, Rectangle rectangle)
        {
            IntPtr hDC = graphics.GetHdc();
            int test = pen.Color.ToArgb();
            int test_2 = Gdi32.ArgbToRGB(pen.Color.ToArgb());
            int test_3 = Gdi32.ArgbToRGB(Color.White.ToArgb());
            test_3 = 0x00FFFFFF;
            IntPtr hPen = Gdi32.CreatePen(0, (int)pen.Width, (int)test_3);
            Gdi32.SelectObject(hDC, hPen);
            //Gdi32.SetROP2(hDC, (int)Gdi32.DrawingMode.R2_NOTXORPEN);
            int old_rop2 = Gdi32.SetROP2(hDC, (int)Gdi32.DrawingMode.R2_NOTXORPEN);
            Gdi32.Rectangle(hDC, rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
            Gdi32.DeleteObject(hPen);

            Gdi32.SetROP2(hDC, old_rop2);
            graphics.ReleaseHdc(hDC);
        }


        private string GetDebuggerDisplay()
        {
            return ToString();
        }

        public void Repaint()
        {
            DrawImageToBuffer();
            Redraw_Ctrl();
        }
    }

    /// <summary>
    /// Wrapper class for the gdi32.dll.
    /// </summary>
    public class Gdi32
    {
        public enum DrawingMode
        {
            R2_NOTXORPEN = 10
        }

        [DllImport("gdi32.dll")]
        public static extern bool Rectangle(IntPtr hDC, int left, int top, int right, int bottom);

        [DllImport("gdi32.dll")]
        public static extern int SetROP2(IntPtr hDC, int fnDrawMode);

        [DllImport("gdi32.dll")]
        public static extern bool MoveToEx(IntPtr hDC, int x, int y, ref Point p);

        [DllImport("gdi32.dll")]
        public static extern bool LineTo(IntPtr hdc, int x, int y);

        [DllImport("gdi32.dll")]
        public static extern IntPtr CreatePen(int fnPenStyle, int nWidth, int crColor);

        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObj);

        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObj);



        // Convert the Argb from .NET to a gdi32 RGB
        static public int ArgbToRGB(int rgb)
        {
            return ((rgb >> 16 & 0x0000FF) | (rgb & 0x00FF00) | (rgb << 16 & 0xFF0000));
        }

    }
    public class Report_Display_Info
    {
        public Point Pos;
        public int Gray_Level;
        public int R_Val;
        public int G_Val;
        public int B_Val;
        public float Scale;

        public Report_Display_Info()
        {
            Pos = new System.Drawing.Point(0,0);
            Gray_Level = 0;
            R_Val = 0;
            G_Val = 0;
            B_Val = 0;
            Scale = 0.0f;
        }
    }
}
