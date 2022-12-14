using CheckOffset;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Tracing;
using System.Drawing;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using OpenCvSharp;
using TN.Insp_Param;
using CheckOffset.ImageTools;

namespace TNControls
{
    public partial class TNCustCtrl_Rect : Control
    {
        public TNCustCtrl_Rect()
        {
            InitializeComponent();

            Display_Pen_Width = 1;
            Display_Cross = false;
        }

        /// <summary>
        /// properties.
        /// </summary>
        public class DS_Pos_Info
        {
            public Rect Editing_Rect;

            public DS_Pos_Info()
            {
                Editing_Rect = new Rect(0,0,0,0);
            }
        }

        public DS_Pos_Info Pos_Info = new DS_Pos_Info();

        //public DS_Pos_Info Pos_Info { get => m_pos_info; set => m_pos_info = value; }

        /// <summary>
        /// Image 座標系位置 
        /// </summary>

        public bool Editing { get => _bEditing; set => _bEditing = value; }

        public Color Display_Color { get; set; }
        public int   Display_Pen_Width { get; set; }
        public bool Display_Cross { get; set; }

        public HitTest_Result HitTest_Rresult { get => _HitTest_Rresult; set => _HitTest_Rresult = value; }
        public DS_Insp_Param_Pin Insp_param { get => _insp_param; set => _insp_param = value; }
        public DS_Insp_Result Insp_result { get => _insp_result; set => _insp_result = value; }

        /// <summary>
        /// data member
        /// </summary>
        private HitTest_Result _HitTest_Rresult = HitTest_Result.None;

        private Rect _Editing_Rect_Before_Modify = new Rect( 0, 0, 0, 0);
        private OpenCvSharp.Point _Editing_LBtn_Down = new OpenCvSharp.Point(0, 0);

        private bool _bEditing = false;

        private DS_Insp_Param_Pin _insp_param = new DS_Insp_Param_Pin();

        private DS_Insp_Result _insp_result = new DS_Insp_Result();


        /// <summary>
        /// member functions.
        /// </summary>
        /// <param name="pe"></param>
        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Display_Color, Display_Pen_Width);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(Insp_Base_Tools.To_System_Rect( Pos_Info.Editing_Rect)  ) );

                if ( Display_Cross )
                {
                    Rectangle display_rt = pb.GetPBRectFromImage(Insp_Base_Tools.To_System_Rect(Pos_Info.Editing_Rect));
                    System.Drawing.Point pt_left  = new System.Drawing.Point(display_rt.X,     display_rt.Y + display_rt.Height / 2 );
                    System.Drawing.Point pt_right = new System.Drawing.Point(display_rt.Right, display_rt.Y + display_rt.Height / 2);
                    graphics_show.DrawLine(pen_ctrl, pt_left, pt_right);
                    System.Drawing.Point pt_top    = new System.Drawing.Point(display_rt.X + display_rt.Width / 2, display_rt.Y);
                    System.Drawing.Point pt_bottom = new System.Drawing.Point(display_rt.X + display_rt.Width / 2, display_rt.Y + display_rt.Height);
                    graphics_show.DrawLine(pen_ctrl, pt_top, pt_bottom);
                }
            }
            else
            {
                // 框本身
                Pen pen_ctrl = new Pen(Color.Red, 1);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(Insp_Base_Tools.To_System_Rect(Pos_Info.Editing_Rect)));

                // 左上
                Rectangle rt_left_top = new Rectangle(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_left_top));

                // 左
                Rectangle rt_left = new Rectangle(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height / 2 - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_left));

                // 左下
                Rectangle rt_left_bottom = new Rectangle(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_left_bottom));

                // 上中
                Rectangle rt_top = new Rectangle(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width / 2- 5, Pos_Info.Editing_Rect.Y - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_top));

                // 下中
                Rectangle rt_bottom = new Rectangle(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width / 2 - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_bottom));

                // 右上
                Rectangle rt_right_top = new Rectangle(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width - 5, Pos_Info.Editing_Rect.Y - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_right_top));

                // 右
                Rectangle rt_right = new Rectangle(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height / 2 - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_right));

                // 右下
                Rectangle rt_right_bottom = new Rectangle(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, pb.GetPBRectFromImage(rt_right_bottom));
            }

            Font draw_font = new Font("Arial", 8);
            string draw_string = _insp_param.Draw_String() + "\r\n" + _insp_result.Draw_String();
            Rectangle pb_rect = pb.GetPBRectFromImage(Insp_Base_Tools.To_System_Rect(Pos_Info.Editing_Rect));
            graphics_show.DrawString(_insp_param.Draw_String(), draw_font, new SolidBrush(Color.Green)
                                   , pb_rect.X, pb_rect.Y );

            _insp_result.Paint_Defect(graphics_show, pb);
        }

        public bool Contain(OpenCvSharp.Point pt)
        {
            return Pos_Info.Editing_Rect.Contains(pt);
        }

        public HitTest_Result HitTest(OpenCvSharp.Point pt)
        {
            // 左上
            Rect rt_left_top = new Rect(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y - 5, 10, 10);
            if (rt_left_top.Contains(pt))
                return HitTest_Result.HT_LT;

            // 左下
            Rect rt_left_bottom = new Rect(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height - 5, 10, 10);
            if (rt_left_bottom.Contains(pt))
                return HitTest_Result.HT_LB;

            // 右上
            Rect rt_right_top = new Rect(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width - 5, Pos_Info.Editing_Rect.Y - 5, 10, 10);
            if (rt_right_top.Contains(pt))
                return HitTest_Result.HT_RT;

            // 右下
            Rect rt_right_bottom = new Rect(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height - 5, 10, 10);
            if (rt_right_bottom.Contains(pt))
                return HitTest_Result.HT_RB;

            // 左
            Rect rt_left = new Rect(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y - 5, 10, Pos_Info.Editing_Rect.Height + 10);
            if (rt_left.Contains(pt))
                return HitTest_Result.HT_Left;

            // 上中
            Rect rt_top = new Rect(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y - 5, Pos_Info.Editing_Rect.Width + 10, 10);
            if (rt_top.Contains(pt))
                return HitTest_Result.HT_Top;


            // 下中
            Rect rt_bottom = new Rect(Pos_Info.Editing_Rect.X - 5, Pos_Info.Editing_Rect.Y + Pos_Info.Editing_Rect.Height - 5, Pos_Info.Editing_Rect.Width + 10, 10);
            if (rt_bottom.Contains(pt))
                return HitTest_Result.HT_Bottom;

            // 右
            Rect rt_right = new Rect(Pos_Info.Editing_Rect.X + Pos_Info.Editing_Rect.Width - 5, Pos_Info.Editing_Rect.Y - 5, 10, Pos_Info.Editing_Rect.Height + 10);
            if (rt_right.Contains(pt))
                return HitTest_Result.HT_Right;

            if (Contain(pt))
                return HitTest_Result.HT_Client;

            return HitTest_Result.None;
        }


        private void UserCtrl_Rect_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Red, 1);
            e.Graphics.DrawRectangle(pen, Insp_Base_Tools.To_System_Rect(Pos_Info.Editing_Rect) );

            if (_bEditing)
            {

            }
        }

        // HitTest + translation.
        public void Modify_Begin(OpenCvSharp.Point pt_start)
        {
            _Editing_Rect_Before_Modify = Pos_Info.Editing_Rect;
            _Editing_LBtn_Down = pt_start;
        }

        public Rect Modify_Doing(OpenCvSharp.Point pt_cur)
        {
            int offset_x = pt_cur.X - _Editing_LBtn_Down.X;
            int offset_y = pt_cur.Y - _Editing_LBtn_Down.Y;

            OpenCvSharp.Point pt_left_top       = new OpenCvSharp.Point(_Editing_Rect_Before_Modify.X,                                      _Editing_Rect_Before_Modify.Y);
            OpenCvSharp.Point pt_right_top      = new OpenCvSharp.Point(_Editing_Rect_Before_Modify.X + _Editing_Rect_Before_Modify.Width, _Editing_Rect_Before_Modify.Y);
            OpenCvSharp.Point pt_right_bottom   = new OpenCvSharp.Point(_Editing_Rect_Before_Modify.X + _Editing_Rect_Before_Modify.Width, _Editing_Rect_Before_Modify.Y + _Editing_Rect_Before_Modify.Height);
            OpenCvSharp.Point pt_left_bottom    = new OpenCvSharp.Point(_Editing_Rect_Before_Modify.X,                                      _Editing_Rect_Before_Modify.Y + _Editing_Rect_Before_Modify.Height);

            switch (_HitTest_Rresult)
            {
                case HitTest_Result.HT_LT:
                    {
                        // use LT + RB.
                        pt_left_top.X += offset_x;
                        pt_left_top.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }

                case HitTest_Result.HT_Top:
                    {
                        // use LT + RB.
                        pt_left_top.Y = pt_left_top.Y + offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }

                case HitTest_Result.HT_RT:
                    {
                        // use RT + LB.
                        pt_right_top.X += offset_x;
                        pt_right_top.Y += offset_y;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }

                case HitTest_Result.HT_Right:
                    {
                        // use RT + LB.
                        pt_right_top.X += offset_x;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }

                case HitTest_Result.HT_RB:
                    {
                        // use LT + RB.
                        pt_right_bottom.X += offset_x;
                        pt_right_bottom.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }

                case HitTest_Result.HT_Bottom:
                    {
                        // use LT + RB.
                        pt_right_bottom.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }

                case HitTest_Result.HT_LB:
                    {
                        // use RT + LB.
                        pt_left_bottom.X += offset_x;
                        pt_left_bottom.Y += offset_y;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }

                case HitTest_Result.HT_Left:
                    {
                        // use RT + LB.
                        pt_left_bottom.X += offset_x;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }

                case HitTest_Result.HT_Client:
                    {
                        pt_left_top.X += offset_x;
                        pt_left_top.Y += offset_y;

                        pt_right_bottom.X += offset_x;
                        pt_right_bottom.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }

                default:
                    break;
            }

            return new Rect(0, 0, 0, 0);
        }

        public void Modify_Done()
        {
            _HitTest_Rresult = HitTest_Result.None;
            _Editing_LBtn_Down = new OpenCvSharp.Point(0, 0);
        }

        public static void Normalize(ref Rect rt)
        {
            Rect rt_dest = rt;
            if (rt.Width < 0)
            {
                rt_dest.X = rt.X + rt.Width;
                rt_dest.Width = -rt.Width;
            }

            if (rt.Height < 0)
            {
                rt_dest.Y = rt.Y + rt.Height;
                rt_dest.Height = -rt.Height;
            }

            rt = rt_dest;
        }

        public static Rect Normalize(OpenCvSharp.Point pt_1, OpenCvSharp.Point pt_2)
        {
            Rect rt_dest = new Rect( Math.Min( pt_1.X, pt_2.X)
                                            ,  Math.Min( pt_1.Y, pt_2.Y )
                                            ,  Math.Abs( pt_1.X - pt_2.X)
                                            ,  Math.Abs( pt_1.Y - pt_2.Y) );
            return rt_dest;
        }

    }

    public enum HitTest_Result
    {
        None = 0
        , HT_LT
        , HT_Top
        , HT_RT
        , HT_Right
        , HT_RB
        , HT_Bottom
        , HT_LB
        , HT_Left
        , HT_Client
    }
}
