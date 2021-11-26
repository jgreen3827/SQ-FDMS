using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.Shared
{
    // Used to hold the GForce and Attitude Data for an
    // aircraft as well as the time it was sent
    public class Telemetry
    {
        public string AircraftTailNumber { get; set; }
        public GForce GForceData { get; set; }
        public Attitude AttitudeData { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
