using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOffset.ProjectInspInfo
{
    public class DS_ProjectInspInfo
    {
        public DS_Defect_Pin_Info[]? Defect_Pins { get; set; }

        public DS_ProjectInspInfo()
        {
            Defect_Pins = null;
        }
    }
}
