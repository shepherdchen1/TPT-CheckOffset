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
//using CheckOffset.Controls;

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
            User_Ctrls = new List<Control>();

            TNUserCtrl_Rect editing_ctrl = new TNUserCtrl_Rect();
            editing_ctrl.Editing_Rect = new Rectangle(10, 10, 20, 20);
            Editing_Ctrl = editing_ctrl;

            MouseWheel += new MouseEventHandler(PB_MouseWheel);
            MouseDown += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseDown);
            MouseMove += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseMove);
            MouseUp += new System.Windows.Forms.MouseEventHandler(pb_Image_MouseUp);
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
            Draw_Ctrl_Refresh();
        }

        //////////////////////////////////////////////////
        /// data member...
        private Bitmap? m_image      = null; // 8bit bitmap original file's bitmap.
        public  Bitmap? m_show_image = null; // 8bit bitmap scaled display bitmap only visible rect.
        private Bitmap? m_show_color_image = null; // transfer m_show_image to color bitmap.
        public Point m_pt_LBtnDown;
        public Point m_pt_Current;

        private float m_scale = 1.0f;
        private Point m_offset;     // 原始影像座標(m_image)顯示的起點: 往右拉，X值為負值，代表影像原點座標要往負方向開始截取.

        bool m_busy = false;
        bool m_enable_zoom = true;

        Point m_move_start_pt = new Point(0, 0);
        Point m_move_start_offset = new Point(0, 0);

        public event Delegate_Report_GrayLevel_Gray? Report_GrayLevel_Gray;

        public Control? Editing_Ctrl
        {
            get;
            set;
        }

        public List<Control>? User_Ctrls
        {
            get;
            set;
        }
        public float Image_Scale 
        { 
            get => m_scale;
            set
            {
                m_scale = value;
                DrawImageToBuffer();
                Draw_Ctrl_Refresh();
            }
        }
        public Point Image_Offset 
        { 
            get => m_offset;
            set
            {
                m_offset = value;
                DrawImageToBuffer();
                Draw_Ctrl_Refresh();
            }
        }
        public Bitmap? Image_Bmp 
        { 
            get => m_image; 
            set {
                m_image = value;
                m_show_image = value;
                //Image = m_show_image;
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

            if (m_enable_zoom && !m_busy)
            {
                m_busy = true;

                //Point center_pt = GetImagePointFromPB(new Point(Width / 2, Height / 2));
                // zoomin/out by cursor
                Point center_pt = GetImagePointFromPB(e.Location);

                if (e.Delta > 0)
                    ZoomIn(e.Location /*center_pt*/);
                else if (e.Delta < 0)
                    ZoomOut(e.Location/*center_pt*/);

                m_busy = false;
            }

            //PB_Image_Refresh();
        }

        private void pb_Image_MouseDown(object? sender, MouseEventArgs e)
        {
            m_pt_LBtnDown = e.Location;
            //ll_Test.Text = String.Format("{0}, {1}, {2}, {3}", m_pt_LBtnDown.X, m_pt_LBtnDown.Y, 0, 0);
            if (null != Editing_Ctrl)
            {
                TNUserCtrl_Rect editing_cttl = (TNUserCtrl_Rect)Editing_Ctrl;
                //using (Graphics g = Graphics.FromImage(pb_Image.Image))
                //{

                //    Pen pen = new Pen(Color.White, 1);
                //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
                //}

                m_move_start_pt = e.Location;
                m_move_start_offset = m_offset;

                if ( !editing_cttl.Editing )
                    editing_cttl.Editing_Rect = new Rectangle(m_pt_LBtnDown.X, m_pt_LBtnDown.Y, 0, 0);
            }
        }

        private void pb_Image_MouseMove(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                m_busy = true;

                m_offset = m_move_start_offset;
                m_offset.X += (Int32)((m_move_start_pt.X - e.Location.X) / m_scale);
                m_offset.Y += (Int32)((m_move_start_pt.Y - e.Location.Y) / m_scale);

                m_offset.X = Math.Max(m_offset.X, 0);
                m_offset.Y = Math.Max(m_offset.Y, 0);
                //OffsetChanged(this, gcnew EventArgs());

                DrawImageToBuffer();
                Draw_Ctrl_Refresh();

                m_busy = false;
            }
            else if (e.Button == MouseButtons.Left)
            {
                m_pt_Current = PointToClient(e.Location);

                Draw_Ctrl_Refresh();
                //DrawXORRectangle
                //m_pt_Current = e.Location;

                //ll_Test.Text = String.Format("{0}, {1}, {2}, {3}", m_pt_LBtnDown.X, m_pt_LBtnDown.Y, m_pt_Current.X, m_pt_Current.Y);

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
                m_busy = true;

                m_busy = false;
            }
            else if (e.Button == MouseButtons.Left)
            {
                m_pt_Current = PointToClient(e.Location);

                Draw_Ctrl_Refresh();
                //DrawXORRectangle
                //m_pt_Current = e.Location;

                //ll_Test.Text = String.Format("{0}, {1}, {2}, {3}", m_pt_LBtnDown.X, m_pt_LBtnDown.Y, m_pt_Current.X, m_pt_Current.Y);

                //if (null != Editing_Ctrl)
                //{
                //    PB_Image_Refresh();
                //}
            }

            Report_Cursor_Gray_Level(e);
        }

        private void PB_Image_Refresh()
        {
            //pb_Image.Refresh();
            //Refresh();

            Bitmap color_bmp = new Bitmap(Width, Height);


            if (null != User_Ctrls && User_Ctrls.Count > 0)
            {
                Pen pen = new Pen(Color.White, 1);

                foreach (Control enum_ctrl in User_Ctrls)
                {
                    TNUserCtrl_Rect user_ctrl = (TNUserCtrl_Rect)enum_ctrl;
                    Rectangle show_rect = CalcShowRect();  // 要畫在UI上的Image區域
                    Point pt_offset = new Point(-show_rect.X, -show_rect.Y);
                    Rectangle rt_draw = user_ctrl.Editing_Rect;
                    rt_draw.Offset(pt_offset);
                    Rectangle rt_final_draw = new Rectangle((int)(rt_draw.Left * m_scale), (int)(rt_draw.Top * m_scale)
                                                            , (int)(rt_draw.Width * m_scale), (int)(rt_draw.Height * m_scale));
                    CreateGraphics().DrawRectangle(pen, rt_final_draw);
                }
            }

            if (null != Editing_Ctrl)
            {
                TNUserCtrl_Rect editing_cttl = (TNUserCtrl_Rect)Editing_Ctrl;
                //using (Graphics g = Graphics.FromImage(pb_Image.Image))
                //{

                //    Pen pen = new Pen(Color.White, 1);
                //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
                //}

                //editing_cttl.Editing_Rect = new Rectangle(Math.Min(m_pt_LBtnDown.X, m_pt_Current.X)
                //                      , Math.Min(m_pt_LBtnDown.Y, m_pt_Current.Y)
                //                      , Math.Abs(m_pt_Current.X - m_pt_LBtnDown.X)
                //                      , Math.Abs(m_pt_Current.Y - m_pt_LBtnDown.Y));

                //Bitmap color_bmp = new Bitmap(pb_Image.Image.Width, pb_Image.Image.Height);

                using (Graphics g = Graphics.FromImage(color_bmp))
                {
                    if (null != m_show_image)
                    {
                        g.DrawImageUnscaled(m_show_image, 0, 0);
                        Image = m_show_image;
                        //m_show_image.Save("d:\\temp\\test.bmp");
                    }

                    Pen pen = new Pen(Color.White, 1);
                    //g.DrawRectangle(pen, editing_cttl.Editing_Rect);

                    CreateGraphics().DrawRectangle(pen, editing_cttl.Editing_Rect);
                }
            }

            //Image = m_show_image;
        }

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
                unsafe
                {
                    byte* ptr = (byte*)ptr_buffer.ToPointer();
                    gray_level = (int)*(ptr + pt_Current.Y * bmp_data.Stride + pt_Current.X * pixel_size);
                }

                Image_Buffer_Gray.ReleaseBuffer((Bitmap)Image, bmp_data);

                Point pt_image = new Point((int)(pt_Current.X / m_scale + m_offset.X), (int)(pt_Current.Y / m_scale + m_offset.Y));

                Report_Display_Info report_display_info = new Report_Display_Info
                {
                    Pos = pt_image
                                ,
                    Gray_Level = gray_level
                                ,
                    Scale = m_scale
                };
                if (null != Report_GrayLevel_Gray)
                {
                    Report_GrayLevel_Gray(report_display_info);
                }
            }
            catch (Exception ex)
            {
                Log_Utl.Log_Event(Event_Level.Error, System.Reflection.MethodBase.GetCurrentMethod()?.Name
                   , string.Format("Exception catched: error:{0}", ex.Message));
                // 儲存Exception到檔案
                TN.Tools.Debug.ExceptionDump.SaveToDefaultFile(ex);
            }
            finally
            {
                if (null != bmp_data)
                    Image_Buffer_Gray.ReleaseBuffer((Bitmap)Image, bmp_data);
            }
        }

        Point GetImagePointFromPB(Point pt)
        {
            Point real_pt = new Point((Int32)Math.Floor(pt.X / m_scale + m_offset.X)
                                     , (Int32)Math.Floor(pt.Y / m_scale + m_offset.Y));
            return real_pt;
        }

        Point GetPBPointFromImage(Point pt)
        {
            Point pb_pt = new Point((Int32)Math.Round((pt.X - m_offset.X) * m_scale, MidpointRounding.AwayFromZero)
                                    , (Int32)Math.Round((pt.Y - m_offset.Y) * m_scale, MidpointRounding.AwayFromZero));
            return pb_pt;
        }

        System.Drawing.Point CalcOffset(System.Drawing.Point display_cursor_pt, float old_scale, float new_scale)
        {
            // 1. 以原有倍率取得滑鼠座標 在 m_image 坐標系的位置
            m_scale = old_scale;
            Point pt_new = GetImagePointFromPB(display_cursor_pt);
            m_scale = new_scale;

            // 轉到新倍率坐標系
            pt_new.X = (int)(pt_new.X * new_scale);
            pt_new.Y = (int)(pt_new.Y * new_scale);

            // 找到新坐標系顯示左上角原點
            pt_new.X -= display_cursor_pt.X;
            pt_new.Y -= display_cursor_pt.Y;

            // 找到新坐標系原點在 m_image 坐標系位置
            pt_new.X = (int)(pt_new.X / new_scale);
            pt_new.Y = (int)(pt_new.Y / new_scale);

            pt_new.X = Math.Max(pt_new.X, 0);
            pt_new.Y = Math.Max(pt_new.Y, 0);

            return pt_new;
        }

        float GetBestFitScale()
        {
            if (null != m_image)
            {
                float scale_x = ClientSize.Width / (float)m_image.Width;
                float scale_y = ClientSize.Height / (float)m_image.Height;

                return ((scale_x > scale_y) ? scale_y : scale_x);
            }

            return 1.0f;
        }

        public void ZoomFit()
        {
            m_scale = GetBestFitScale();
            m_offset = new Point(0, 0);
            DrawImageToBuffer();
            Draw_Ctrl_Refresh();
            //pb_Image.Image = m_show_image;
        }

        private void ZoomIn(Point display_cursor_pt)
        {
            float old_scale = m_scale;
            m_scale *= 1.25f;

            if (old_scale != m_scale)
            {
                //m_offset = CalcOffset(pt_center);
                m_offset = CalcOffset(display_cursor_pt, old_scale, m_scale);
                //ScaleChanged(this, new EventArgs());

                DrawImageToBuffer();
                Draw_Ctrl_Refresh();
                //PB_Image_Refresh();

                //buf_g->DrawImageUnscaled(m_show_image, 0, 0);
            }

            //OffsetChanged(this, new EventArgs());
        }

        private void ZoomOut(Point display_cursor_pt)
        {
            float old_scale = m_scale;
            m_scale /= 1.25f;

            if (old_scale != m_scale)
            {
                //m_offset = CalcOffset(pt_center);
                m_offset = CalcOffset(display_cursor_pt, old_scale, m_scale);
                //ScaleChanged(this, new EventArgs());

                DrawImageToBuffer();
                Draw_Ctrl_Refresh();
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
            //System.Drawing.Rectangle show_rect = new Rectangle(m_offset.X, m_offset.Y, 0, 0);

            if (null != m_show_image)
            {
                show_rect.Location = m_offset;
                show_rect.Width = (Int32)(ClientSize.Width / m_scale + 0.5f);
                show_rect.Width = Math.Max(show_rect.Width, m_show_image.Width);
                show_rect.Height = (Int32)(ClientSize.Height / m_scale + 0.5f);
                show_rect.Height = Math.Max(show_rect.Height, m_show_image.Height);
            }

            return show_rect;
        }

        System.Drawing.Rectangle CalcLockRect()
        {
            System.Drawing.Rectangle show_rect = CalcShowRect();
            if (null != m_image)
            {
                System.Drawing.Rectangle img_rect = new System.Drawing.Rectangle(0, 0, m_image.Width, m_image.Height);
                show_rect = System.Drawing.Rectangle.Intersect(show_rect, img_rect);
            }
            return show_rect;
        }

        void DrawImageToBuffer()
        {
            if (null == m_image)
            {
                //if (m_show_image)
                //{
                //    delete m_show_image;
                //    m_show_image = nullptr;
                //}
                return;
            }

            // Create new bitmap
            m_show_image = new Bitmap(ClientSize.Width, ClientSize.Height, m_image.PixelFormat);

            Rectangle lock_rect = CalcLockRect();  // 取得要Lock的影像範圍
            Rectangle show_rect = CalcShowRect();  // 要畫在UI上的Image區域

            if (lock_rect.Width == 0 || lock_rect.Height == 0)
                return;

            Int32 bytes_per_pixel = GetPixelLength(m_image.PixelFormat);

            if (bytes_per_pixel == 0)
                return;

            //m_image.Save("d:\\temp\\Source.bmp");

            // Get raw data
            System.Drawing.Imaging.BitmapData fromBitmapData = m_image.LockBits(lock_rect, System.Drawing.Imaging.ImageLockMode.ReadOnly, m_image.PixelFormat);
            System.Drawing.Imaging.BitmapData destBitmapData = m_show_image.LockBits(new Rectangle(0, 0, m_show_image.Width, m_show_image.Height), System.Drawing.Imaging.ImageLockMode.WriteOnly, m_show_image.PixelFormat);

            unsafe
            {
                byte* from_ptr = (byte*)(fromBitmapData.Scan0.ToPointer());
                byte* dest_ptr = (byte*)(destBitmapData.Scan0.ToPointer());
                Int32 from_pitch = fromBitmapData.Stride;
                Int32 dest_pitch = destBitmapData.Stride;

                Int32 copy_width = m_show_image.Width;
                Int32 copy_height = m_show_image.Height;
                byte* from_line_ptr = from_ptr;        // 來源端某行的位置
                byte* dest_line_ptr = dest_ptr;        // 目的端某行的位置
                byte* from_now_ptr = from_line_ptr;    // 來源端目前的位置
                byte* dest_now_ptr = dest_line_ptr;    // 目的端目前的位置

                Int32 offset_h = 0; // 來源端與目的端高度offset

                for (int h = 0; h < copy_height; h++)
                {
                    Int32 from_height = (Int32)(h / m_scale + 0.5) - lock_rect.Y;

                    if (show_rect.Y + from_height < 0)
                    {
                        offset_h++;
                        continue;
                    }

                    // 是否已達bottom.
                    if (show_rect.Y + from_height >= lock_rect.Height)
                        break;

                    from_line_ptr = from_ptr + from_pitch * (Int32)(((h - offset_h) / m_scale));
                    //from_line_ptr = from_ptr + from_pitch * (Int32)(((h - offset_h) / m_scale) + lock_rect.Y);
                    // 1018- dest_line_ptr = dest_ptr + dest_pitch * h;
                    dest_line_ptr = dest_ptr + dest_pitch * (Int32)(h - offset_h);
                    from_now_ptr = from_line_ptr;
                    dest_now_ptr = dest_line_ptr;

                    Int32 offset_w = 0; // 來源端與目的端寬度offset
                    Int32 show_rect_x = show_rect.X;
                    Int32 lock_rect_x = lock_rect.X;

                    for (int w = 0; w < copy_width; w++)
                    {
                        Int32 from_weight = (Int32)(w / m_scale + 0.5) - lock_rect_x;

                        if (show_rect_x + from_weight < 0)
                        {
                            offset_w++;
                            //1018-   dest_now_ptr += bytes_per_pixel;
                            continue;
                        }
                        else if (show_rect_x + from_weight >= lock_rect.Width)
                            break;

                        from_now_ptr = from_line_ptr + (Int32)((w - offset_w) / m_scale) * bytes_per_pixel;
                        //from_now_ptr = from_line_ptr + (Int32)((w - offset_w) / m_scale + lock_rect_x) * bytes_per_pixel;
                        for (int n = 0; n < bytes_per_pixel; n++)
                            dest_now_ptr[n] = from_now_ptr[n];

                        dest_now_ptr += bytes_per_pixel;
                    }
                }
            }

            m_image.UnlockBits(fromBitmapData);
            m_show_image.UnlockBits(destBitmapData);

            if (m_image.PixelFormat == System.Drawing.Imaging.PixelFormat.Format8bppIndexed)
                m_show_image.Palette = m_image.Palette;

            //m_show_image.Save("d:\\temp\\test.bmp");

            m_show_color_image = new Bitmap(m_show_image.Width, m_show_image.Height);

            Image = m_show_image;
            Invalidate();
        }

        private void Draw_Ctrl_Refresh()
        {
            if (null == m_show_image)
                return;                

            m_show_color_image = new Bitmap(m_show_image.Width, m_show_image.Height);
            using (Graphics graphics_show = Graphics.FromImage(m_show_color_image))
            {
                graphics_show.DrawImage(m_show_image, 0, 0);
                //m_show_image.Save("d:\\temp\\test1.bmp");
                //m_show_color_image.Save("d:\\temp\\testcolor1.bmp");

                Pen pen = new Pen(Color.White, 1);

                if (null != User_Ctrls && User_Ctrls.Count > 0)
                {
                    Pen pen_user_ctrl = new Pen(Color.White, 1);

                    foreach (Control enum_ctrl in User_Ctrls)
                    {
                        TNUserCtrl_Rect user_ctrl = (TNUserCtrl_Rect)enum_ctrl;
                        Rectangle show_rect = CalcShowRect();  // 要畫在UI上的Image區域
                        Point pt_offset = new Point(-show_rect.X, -show_rect.Y);
                        Rectangle rt_draw = user_ctrl.Editing_Rect;
                        rt_draw.Offset(pt_offset);
                        Rectangle rt_final_draw = new Rectangle((int)(rt_draw.Left * m_scale), (int)(rt_draw.Top * m_scale)
                                                                , (int)(rt_draw.Width * m_scale), (int)(rt_draw.Height * m_scale));
                        //CreateGraphics().DrawRectangle(pen, rt_final_draw);
                        //graphics_show.DrawRectangle(pen_user_ctrl, rt_final_draw);

                        user_ctrl.Paint(graphics_show);
                    }
                }

                if (null != Editing_Ctrl)
                {
                    TNUserCtrl_Rect editing_cttl = (TNUserCtrl_Rect)Editing_Ctrl;
                    //using (Graphics g = Graphics.FromImage(pb_Image.Image))
                    //{

                    //    Pen pen = new Pen(Color.White, 1);
                    //    DrawXORRectangle(g, pen, editing_cttl.Editing_Rect);
                    //}

                    //editing_cttl.Editing_Rect = new Rectangle(Math.Min(m_pt_LBtnDown.X, m_pt_Current.X)
                    //                      , Math.Min(m_pt_LBtnDown.Y, m_pt_Current.Y)
                    //                      , Math.Abs(m_pt_Current.X - m_pt_LBtnDown.X)
                    //                      , Math.Abs(m_pt_Current.Y - m_pt_LBtnDown.Y));

                    //Bitmap color_bmp = new Bitmap(pb_Image.Image.Width, pb_Image.Image.Height);
                    //Bitmap color_bmp = new Bitmap(Width, Height);

                    //using (Graphics g = Graphics.FromImage(color_bmp))
                    //{
                        if (null != m_show_image)
                        {
                            //g.DrawImageUnscaled(m_show_image, 0, 0);
                            //Image = m_show_image;
                            //m_show_image.Save("d:\\temp\\test.bmp");
                        }

                        Pen pen_edit_ctrl = new Pen(Color.White, 1);
                        //g.DrawRectangle(pen, editing_cttl.Editing_Rect);

                        //CreateGraphics().DrawRectangle(pen, editing_cttl.Editing_Rect);
                        //graphics_show.DrawRectangle(pen_edit_ctrl, editing_cttl.Editing_Rect);

                    editing_cttl.Paint(graphics_show);
                    //}
                }
            }
            //m_show_color_image.Save("d:\\temp\\testcolor.bmp");
            Image = m_show_color_image;
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
        public float Scale;
    }
}
