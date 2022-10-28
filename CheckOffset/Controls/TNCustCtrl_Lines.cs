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


        public void Draw2PB(Graphics graphics_show, TNPictureBox pb)
        {
            if (null == Pos_Info.Points)
                return;

            if (!Editing)
            {
                Pen pen_ctrl = new Pen(Color.Blue, 1);
                graphics_show.DrawLines(pen_ctrl, Pos_Info.Points);
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
