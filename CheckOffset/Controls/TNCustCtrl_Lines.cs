using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TNControls
{
    public partial class TNCustCtrl_Lines : Control
    {
        public TNCustCtrl_Lines()
        {
            InitializeComponent();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);
        }

        public class DS_Pos_Info
        {
            public Point[]? Points;

            public DS_Pos_Info()
            {
                Points = null;
            }
        }

        public DS_Pos_Info Pos_Info = new DS_Pos_Info();

        public bool Editing { get; set; }

        public Color Display_Color { get; set; }


        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (null == Pos_Info.Points)
                return;

            if (Pos_Info.Points.Length <= 1)
                return;

            if (!Editing)
            {
                Point[] draw_points = new Point[Pos_Info.Points.Length];   
                for (int pt_id = 0; pt_id < Pos_Info.Points.Length; pt_id++)
                {
                    draw_points[ pt_id ] = pb.GetPBPointFromImage(Pos_Info.Points[pt_id]);
                }

                Pen pen_ctrl = new Pen(Display_Color, 1);
                graphics_show.DrawPolygon(pen_ctrl, draw_points );
            }
            else
            {
      
            }

            //Font draw_font = new Font("Arial", 16);
            //string draw_string = _insp_param.Draw_String() + "\r\n" + _insp_result.Draw_String();
            //graphics_show.DrawString(_insp_param.Draw_String(), draw_font, new SolidBrush(Color.Green)
            //                    , pb.GetPBRectFromImage(Editing_Rect).X
            //                    , pb.GetPBRectFromImage(Editing_Rect).Y);
        }

    }


}
