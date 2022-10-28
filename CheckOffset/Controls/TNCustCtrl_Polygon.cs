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

        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (null == Pos_Info.Points)
                return;

            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                graphics_show.DrawPolygon(pen_ctrl, Pos_Info.Points);
            }
            else
            {

            }
        }
    }
}
