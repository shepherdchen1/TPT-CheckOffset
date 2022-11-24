using CheckOffset.ImageTools;
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
    public partial class TNCustCtrl_Line : Control
    {
        public TNCustCtrl_Line()
        {
            InitializeComponent();

            Init_Data();
        }

        private void Init_Data()
        {
            Pos_Info = new DS_Pos_Info();
            Display_Color = Color.Blue;
            Display_Pen_Width = 1;
            Display_Cross = false;
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public class DS_Pos_Info
        {
            public Point PT_Start;
            public Point PT_End;

            public DS_Pos_Info()
            {
                PT_Start = new Point(0, 0);
                PT_End   = new Point(0, 0);
            }
        }

        public DS_Pos_Info Pos_Info = new DS_Pos_Info();

        public bool Editing { get; set; }

        public Color Display_Color { get; set; }

        public int Display_Pen_Width { get; set; }
        public bool Display_Cross { get; set; }

        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Display_Color, Display_Pen_Width);
                Point pt_1 = pb.GetPBPointFromImage(Pos_Info.PT_Start);
                Point pt_2 = pb.GetPBPointFromImage(Pos_Info.PT_End);
                graphics_show.DrawLine(pen_ctrl, pt_1, pt_2);

                if (Display_Cross)
                {
                    int draw_len = 5;
                    System.Drawing.Point display_pt = pb.GetPBPointFromImage(Pos_Info.PT_Start);
                    System.Drawing.Point pt_left = new System.Drawing.Point( display_pt.X - draw_len, display_pt.Y);
                    System.Drawing.Point pt_right = new System.Drawing.Point(display_pt.X + draw_len, display_pt.Y);
                    graphics_show.DrawLine(pen_ctrl, pt_left, pt_right);
                    System.Drawing.Point pt_top    = new System.Drawing.Point(display_pt.X, display_pt.Y - draw_len);
                    System.Drawing.Point pt_bottom = new System.Drawing.Point(display_pt.X, display_pt.Y + draw_len);
                    graphics_show.DrawLine(pen_ctrl, pt_top, pt_bottom);

                    display_pt = pb.GetPBPointFromImage(Pos_Info.PT_End);
                    pt_left = new System.Drawing.Point(display_pt.X - draw_len, display_pt.Y);
                    pt_right = new System.Drawing.Point(display_pt.X + draw_len, display_pt.Y);
                    graphics_show.DrawLine(pen_ctrl, pt_left, pt_right);
                    pt_top = new System.Drawing.Point(display_pt.X, display_pt.Y - draw_len);
                    pt_bottom = new System.Drawing.Point(display_pt.X, display_pt.Y + draw_len);
                    graphics_show.DrawLine(pen_ctrl, pt_top, pt_bottom);
                }
            }
            else
            {

            }
        }
    }
}
