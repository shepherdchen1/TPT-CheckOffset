using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckOffset.ProjectInspInfo
{
    public class DS_Insp_Param_Chip
    {
        private float _polysimplify_tolerance = 2.5f;

        public float Polysimplify_tolerance { get => _polysimplify_tolerance; set => _polysimplify_tolerance = value; }
    }
}
