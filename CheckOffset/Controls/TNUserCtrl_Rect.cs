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

namespace TNControls
{
    public partial class TNUserCtrl_Rect : Control
    {
        public TNUserCtrl_Rect()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void Paint(Graphics graphics_show)
        {
            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                graphics_show.DrawRectangle(pen_ctrl, Editing_Rect);
            }
            else
            {
                // 框本身
                Pen pen_ctrl = new Pen(Color.Red, 1);
                graphics_show.DrawRectangle(pen_ctrl, Editing_Rect);

                // 左上
                Rectangle rt_left_top = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_left_top);

                // 左
                Rectangle rt_left = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y + Editing_Rect.Height / 2 - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_left);

                // 左下
                Rectangle rt_left_bottom = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y + Editing_Rect.Height - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_left_bottom);

                // 上中
                Rectangle rt_top = new Rectangle(Editing_Rect.X + Editing_Rect.Width / 2- 5, Editing_Rect.Y - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_top);

                // 下中
                Rectangle rt_bottom = new Rectangle(Editing_Rect.X + Editing_Rect.Width / 2 - 5, Editing_Rect.Y + Editing_Rect.Height - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_bottom);

                // 右上
                Rectangle rt_right_top = new Rectangle(Editing_Rect.X + Editing_Rect.Width - 5, Editing_Rect.Y - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_right_top);

                // 右
                Rectangle rt_right = new Rectangle(Editing_Rect.X + Editing_Rect.Width - 5, Editing_Rect.Y + Editing_Rect.Height / 2 - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_right);

                // 右下
                Rectangle rt_right_bottom = new Rectangle(Editing_Rect.X + Editing_Rect.Width - 5, Editing_Rect.Y + Editing_Rect.Height - 5, 10, 10);
                graphics_show.DrawRectangle(pen_ctrl, rt_right_bottom);
            }
        }

        public bool Contain(Point pt)
        {
            return Editing_Rect.Contains(pt);
        }

        public HitTest_Result HitTest(Point pt)
        {
            HitTest_Result result = new HitTest_Result();

            // 左上
            Rectangle rt_left_top = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y - 5, 10, 10);
            if (rt_left_top.Contains(pt))
                return HitTest_Result.HT_LT;

            // 左下
            Rectangle rt_left_bottom = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y + Editing_Rect.Height - 5, 10, 10);
            if (rt_left_bottom.Contains(pt))
                return HitTest_Result.HT_LB;

            // 右上
            Rectangle rt_right_top = new Rectangle(Editing_Rect.X + Editing_Rect.Width - 5, Editing_Rect.Y - 5, 10, 10);
            if (rt_right_top.Contains(pt))
                return HitTest_Result.HT_RT;

            // 右下
            Rectangle rt_right_bottom = new Rectangle(Editing_Rect.X + Editing_Rect.Width - 5, Editing_Rect.Y + Editing_Rect.Height - 5, 10, 10);
            if (rt_right_bottom.Contains(pt))
                return HitTest_Result.HT_RB;

            // 左
            Rectangle rt_left = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y - 5, 10, Editing_Rect.Height + 10);
            if (rt_left.Contains(pt))
                return HitTest_Result.HT_Left;

            // 上中
            Rectangle rt_top = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y - 5, Editing_Rect.Width + 10, 10);
            if (rt_top.Contains(pt))
                return HitTest_Result.HT_Top;


            // 下中
            Rectangle rt_bottom = new Rectangle(Editing_Rect.X - 5, Editing_Rect.Y + Editing_Rect.Height - 5, Editing_Rect.Width + 10, 10);
            if (rt_bottom.Contains(pt))
                return HitTest_Result.HT_Bottom;

            // 右
            Rectangle rt_right = new Rectangle(Editing_Rect.X + Editing_Rect.Width - 5, Editing_Rect.Y - 5, 10, Editing_Rect.Height + 10);
            if (rt_right.Contains(pt))
                return HitTest_Result.HT_Right;

            if (Contain(pt))
                return HitTest_Result.HT_Client;

            return HitTest_Result.None;
        }


        private void UserCtrl_Rect_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Red, 1);
            e.Graphics.DrawRectangle(pen, Editing_Rect);

            if (m_bEditing)
            {

            }
        }

        public Rectangle Editing_Rect
        {
            get;
            set;
        }

        public bool Editing { get => m_bEditing; set => m_bEditing = value; }
        private bool m_bEditing = false;


        // HitTest + translation.
        public void Modify_Begin(Point pt_start)
        {
            m_Editing_Rect_Before_Modify = Editing_Rect;
            m_Editing_LBtn_Down = pt_start;
        }

        public Rectangle Modify_Doing(Point pt_cur)
        {
            int offset_x = pt_cur.X - m_Editing_LBtn_Down.X;
            int offset_y = pt_cur.Y - m_Editing_LBtn_Down.Y;

            Point pt_left_top       = new Point(m_Editing_Rect_Before_Modify.X,                                      m_Editing_Rect_Before_Modify.Y);
            Point pt_right_top      = new Point(m_Editing_Rect_Before_Modify.X + m_Editing_Rect_Before_Modify.Width, m_Editing_Rect_Before_Modify.Y);
            Point pt_right_bottom   = new Point(m_Editing_Rect_Before_Modify.X + m_Editing_Rect_Before_Modify.Width, m_Editing_Rect_Before_Modify.Y + m_Editing_Rect_Before_Modify.Height);
            Point pt_left_bottom    = new Point(m_Editing_Rect_Before_Modify.X,                                      m_Editing_Rect_Before_Modify.Y + m_Editing_Rect_Before_Modify.Height);

            switch (m_HitTest_Rresult)
            {
                case HitTest_Result.HT_LT:
                    {
                        // use LT + RB.
                        pt_left_top.X += offset_x;
                        pt_left_top.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }
                    break;

                case HitTest_Result.HT_Top:
                    {
                        // use LT + RB.
                        pt_left_top.Y = pt_left_top.Y + offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }
                    break;

                case HitTest_Result.HT_RT:
                    {
                        // use RT + LB.
                        pt_right_top.X += offset_x;
                        pt_right_top.Y += offset_y;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }
                    break;

                case HitTest_Result.HT_Right:
                    {
                        // use RT + LB.
                        pt_right_top.X += offset_x;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }
                    break;

                case HitTest_Result.HT_RB:
                    {
                        // use LT + RB.
                        pt_right_bottom.X += offset_x;
                        pt_right_bottom.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }
                    break;

                case HitTest_Result.HT_Bottom:
                    {
                        // use LT + RB.
                        pt_right_bottom.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }
                    break;

                case HitTest_Result.HT_LB:
                    {
                        // use RT + LB.
                        pt_left_bottom.X += offset_x;
                        pt_left_bottom.Y += offset_y;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }
                    break;

                case HitTest_Result.HT_Left:
                    {
                        // use RT + LB.
                        pt_left_bottom.X += offset_x;

                        return Normalize(pt_right_top, pt_left_bottom);
                    }
                    break;

                case HitTest_Result.HT_Client:
                    {
                        pt_left_top.X += offset_x;
                        pt_left_top.Y += offset_y;

                        pt_right_bottom.X += offset_x;
                        pt_right_bottom.Y += offset_y;

                        return Normalize(pt_left_top, pt_right_bottom);
                    }
                    break;

                default:
                    break;
            }

            return new Rectangle(0, 0, 0, 0);
        }

        public void Modify_Done()
        {
            m_HitTest_Rresult = HitTest_Result.None;
            m_Editing_LBtn_Down = new Point(0, 0);
        }

        public static void Normalize(ref Rectangle rt)
        {
            Rectangle rt_dest = rt;
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

        public static Rectangle Normalize(Point pt_1, Point pt_2)
        {
            Rectangle rt_dest = new Rectangle( Math.Min( pt_1.X, pt_2.X)
                                            ,  Math.Min( pt_1.Y, pt_2.Y )
                                            ,  Math.Abs( pt_1.X - pt_2.X)
                                            ,  Math.Abs( pt_1.Y - pt_2.Y) );
            return rt_dest;
        }


        public HitTest_Result HitTest_Rresult { get => m_HitTest_Rresult; set => m_HitTest_Rresult = value; }

        private HitTest_Result m_HitTest_Rresult;

        private Rectangle m_Editing_Rect_Before_Modify;
        private Point m_Editing_LBtn_Down;

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
