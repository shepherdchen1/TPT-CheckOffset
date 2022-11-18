using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;

namespace CheckOffset.ProjectInspInfo
{

    public class DS_Single_Insp_Info
    {
        public string _tobe_insp_file = "";
        public Mat<byte> _tobe_insp_buffer = new Mat<byte>();
        public bool _inspected = false;

        public DS_Single_Insp_Info()
        {
            _tobe_insp_file = "";
            _tobe_insp_buffer = new Mat<byte>();
            _inspected = false;
        }
    }

    public class DS_ProjectInspInfo
    {
        public List<DS_Single_Insp_Info> Insp_Pool { get; set; }

        public DS_ProjectInspInfo()
        {
            Insp_Pool = new List<DS_Single_Insp_Info>();
        }
    }
}
