using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TN.Insp_Param;
using TNControls;

namespace TNControls
{
    public partial class TNCustCtrl_String : Control
    {
        public TNCustCtrl_String()
        {
            InitializeComponent();

            Init_Data();
        }

        private void Init_Data()
        {
            Pos_Info = new DS_Pos_Info();
            Display_Color = Color.Blue;
        }

        public class DS_Pos_Info
        {
            public Point Point_LT;

            public DS_Pos_Info()
            {
                Point_LT = new Point(0,0);
            }
        }

        public DS_Pos_Info Pos_Info = new DS_Pos_Info();

        public bool Editing { get; set; }

        public Color Display_Color { get; set; }

        public string Display_Str { get; set; }
        public int Display_Font_Size 
        {
            get => _display_font_size;
            set
            {
                if (value <= 4)
                    return;

                _display_font_size = value;
            } 
        }

        private int _display_font_size = 16;


        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (0 == Pos_Info.Point_LT.X || 0 == Pos_Info.Point_LT.Y)
                return;

            Pen pen_ctrl = new Pen(Display_Color, 1);
            //Point pt_lt = pb.GetPBPointFromImage(pt);
            //Point pt_rt_temp = new Point(pt.X + 1, pt.Y + 1);
            //Point pt_rb = pb.GetPBPointFromImage(pt_rt_temp);
            //RectangleF rt = new RectangleF(pt_lt.X, pt_lt.Y, pt_rb.X - pt_lt.X, pt_rb.Y - pt_lt.Y);

            Font draw_font = new Font("Arial", _display_font_size);
            graphics_show.DrawString(Display_Str, draw_font, new SolidBrush(Color.Green)
                                , pb.GetPBPointFromImage(Pos_Info.Point_LT).X
                                , pb.GetPBPointFromImage(Pos_Info.Point_LT).Y);
        }

        public void Paint_Cross(Graphics graphics_show, TNPictureBox pb, RectangleF rt)
        {
            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                PointF pt_left = new PointF(rt.Left, (rt.Top + rt.Bottom) / 2);
                PointF pt_right = new PointF(rt.Right, (rt.Top + rt.Bottom) / 2);
                graphics_show.DrawLine(pen_ctrl, pt_left, pt_right);

                PointF pt_top = new PointF((rt.Left + rt.Right) / 2, rt.Top);
                PointF pt_bottom = new PointF((rt.Left + rt.Right) / 2, rt.Bottom);
                graphics_show.DrawLine(pen_ctrl, pt_top, pt_bottom);
            }
            else
            {

            }
        }
    }
}
