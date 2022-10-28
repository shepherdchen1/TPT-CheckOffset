using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TNControls;

namespace TNControls
{
    public partial class TNCustCtrl_Points : Control
    {
        public TNCustCtrl_Points()
        {
            InitializeComponent();

            Init_Data();
        }

        private void Init_Data()
        {
            Pos_Info = new Struct_Pos_Info();
        }

        public class Struct_Pos_Info
        {
            public Point[]? Points;

            public Struct_Pos_Info()
            {
                Points = null;
            }
        }

        public Struct_Pos_Info Pos_Info = new Struct_Pos_Info();

        public bool Editing { get; set; }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (null == Pos_Info.Points)
                return;

            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                foreach(Point pt in Pos_Info.Points)
                {
                    Point pt_lt = pb.GetPBPointFromImage(pt);
                    Point pt_rt_temp = new Point(pt.X + 1, pt.Y + 1);
                    Point pt_rb = pb.GetPBPointFromImage(pt_rt_temp);
                    RectangleF rt = new RectangleF(pt_lt.X, pt_lt.Y, pt_rb.X - pt_lt.X, pt_rb.Y - pt_lt.Y);
                    Paint_Cross(graphics_show, pb, rt);
                }
            }
            else
            {

            }
        }

        public void Paint_Cross(Graphics graphics_show, TNPictureBox pb, RectangleF rt)
        {
            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                PointF pt_left  = new PointF(rt.Left, ( rt.Top + rt.Bottom ) / 2 );
                PointF pt_right = new PointF(rt.Right, (rt.Top + rt.Bottom) / 2);
                graphics_show.DrawLine(pen_ctrl, pt_left, pt_right);

                PointF pt_top   = new PointF((rt.Left + rt.Right) / 2, rt.Top);
                PointF pt_bottom = new PointF((rt.Left + rt.Right) / 2, rt.Bottom);
                graphics_show.DrawLine(pen_ctrl, pt_top, pt_bottom);
            }
            else
            {

            }
        }
    }
}
