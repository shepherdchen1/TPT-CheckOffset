using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using CheckOffset.Insp;

namespace CheckOffset.ProjectInspInfo
{

    public class DS_Single_Insp_Info
    {
        public string _tobe_insp_file = "";
        public Mat<byte> _tobe_insp_buffer = new Mat<byte>();

        /// <summary>
        /// 
        /// </summary>
        public bool _inspected = false;

        public InspGlueOverflow _insp_inst = null;


        public DS_Single_Insp_Info()
        {
            _tobe_insp_file = "";
            _tobe_insp_buffer = new Mat<byte>();

            _inspected = false;
            _insp_inst = null;
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
