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

        public Mat _from_file_tobe_insp_buffer = new Mat<byte>();

        /// <summary>
        /// 
        /// </summary>
        public bool _inspected = false;

        public InspGlueOverflow _insp_inst = null;

        public Insp_Adh_Tape _insp_adh_tape = null;

        public DS_CAM_Adh_Tape_Info _insp_adh_tape_res = new DS_CAM_Adh_Tape_Info();


        public DS_Single_Insp_Info()
        {
            _tobe_insp_file = "";
            _tobe_insp_buffer = new Mat<byte>();

            _inspected = false;
            _insp_inst = null;

            _insp_adh_tape = new Insp_Adh_Tape();
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
