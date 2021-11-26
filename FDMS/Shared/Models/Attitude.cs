using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.Shared
{
    // Used to hold the attitude data for an aircraft log
    public class Attitude
    {
        public int AttitudeId { get; set; }
        
        public double Altitude { get; set; }

        public double Pitch { get; set; }

        public double Bank { get; set; }
    }
}
