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

        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                graphics_show.DrawLine(pen_ctrl, Pos_Info.PT_Start, Pos_Info.PT_End);
            }
            else
            {

            }
        }
    }
}
