using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOffset.ProjectInspInfo
{
    public class DS_Defect_Pin_Info
    {
        public Point Pt_start { get; set; }
        public Point Pt_end { get; set; }
        public int Pin_width { get; set; }

        public DS_Defect_Pin_Info()
        {
            Pt_start = new Point(0, 0);
            Pt_end = new Point(0, 0);

            Pin_width = 0;
        }
    }
}
