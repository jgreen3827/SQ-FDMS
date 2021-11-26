/* 
* FILE : DatabaseController.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is holds the controller for the database, it has the ability to take a telemetry item
* and add it to the database.
*/
using FDMS.Server.Services;

namespace FDMS.Server.Controllers
{
    /*
    * NAME : DatabaseController
    * PURPOSE : The DatabaseController class has the ability to take the telemetry and call the function which
    * will add the telemetry to the database.
    */
    public class DatabaseController
    {
        /* 
        * FUNCTION : CreateTelemetry
        * DESCRIPTION :
        *   This function will take the incoming telemtry info and call the function to add it to the database.
        * PARAMETERS :
        *   Shared.Telemetry telemetry : this is the telemetry info the server received and that needs to be added
        *   to the database
        * RETURNS :
        *   void : none
        */
        public void CreateTelemetry(Shared.Telemetry telemetry)
        {
            try
            {
                //Check if aircraft exist
                telemetry.AircraftTailNumber.Replace(" ", "");
                Shared.Aircraft temp = AircraftService.GetAircraft(telemetry.AircraftTailNumber);
                if (temp.AircraftTailNumber == null)
                {
                    AircraftService.CreateAircraft(telemetry.AircraftTailNumber);
                }

                //Create GForce
                int gForceId = GForceService.CreateGForce(telemetry.GForceData);
                if (gForceId != 0 || gForceId != null)
                {
                    telemetry.GForceData.GForceId = gForceId;
                }
                //Create Attitude
                int attitudeId = AttitudeService.CreateAttitude(telemetry.AttitudeData);
                if (attitudeId != 0 || attitudeId != null)
                {
                    telemetry.AttitudeData.AttitudeId = attitudeId;
                }
                //Create Telemetry
                TelemetryService.CreateTelemetry(telemetry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

        }
    }
}
