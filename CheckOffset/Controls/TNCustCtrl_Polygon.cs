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
    public partial class TNCustCtrl_Polygon : Control
    {
        public TNCustCtrl_Polygon()
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

        //public DS_Pos_Info Pos_Info { get => m_pos_info; set => m_pos_info = value; }

        /// <summary>
        /// Image 座標系位置 
        /// </summary>

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
                Pen pen_ctrl = new Pen(Display_Color, 1);

                Point[] draw_pts = new Point[Pos_Info.Points.Length]; 
                for(int pt_id = 0; pt_id < draw_pts.Length; pt_id++)
                {
                    draw_pts[pt_id] = pb.GetPBPointFromImage(Pos_Info.Points[pt_id]);
                }

                graphics_show.DrawPolygon(pen_ctrl, draw_pts);
            }
            else
            {

            }
        }
    }
}
