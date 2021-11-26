using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FDMS.Shared
{
    // Main Aircraft object. Used to hold a list of the aircraft's telemetry data
    public class Aircraft
    {
        // The tail number of the aircraft (also ID)
        public string AircraftTailNumber { get; set; }

        // The list of all the telemetries for the aircraft
        public List<Telemetry> AircraftTelemetries { get; set; }
    }
}
