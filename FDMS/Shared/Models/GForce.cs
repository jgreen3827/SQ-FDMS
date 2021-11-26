using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.Shared
{
    // Used to hold the GForce data for an aircraft log
    public class GForce
    {
        public int GForceId { get; set; }

        public double AccelX { get; set; }

        public double AccelY { get; set; }

        public double AccelZ { get; set; }

        public double Weight { get; set; }
    }
}