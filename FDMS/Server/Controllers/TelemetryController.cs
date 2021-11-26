/* 
* FILE : TelemetryController.cs
* PROJECT : SENG3020 - Flight Data Management System
* PROGRAMMER : (Group 8) Benito Zefferino, Daniel Meyer, Jordan Green, Justin Croezen
* FIRST VERSION : 2021-11-12
* DESCRIPTION :
* This file is the telemetry controller which will get all of the aircraft info and put it
* in a list.
*/
using FDMS.Server.Services;
using FDMS.Shared;
using FDMS.Shared.ControllerModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FDMS.Server.Controllers
{
    /*
    * NAME : TelemetryController : ControllerBase
    * PURPOSE : The TelemetryController class get all the aircraft info from the database
    * and puts it in a list.
    */
    [ApiController]
    [Route("[controller]")]
    public class TelemetryController : ControllerBase
    {
        /* 
        * FUNCTION : GetAircraftData
        * DESCRIPTION :
        *   This function will get all of the telemetry data and put it into a list.
        * PARAMETERS :
        *   none
        * RETURNS :
        *  IEnumerable<Shared.Telemetry> : A list of aircraft info
        */
        [HttpGet]
        public IEnumerable<Telemetry> GetAircraftData()
        {
            try
            {
                List<Telemetry> telemetryList = new List<Telemetry>();
                telemetryList = TelemetryService.GetAllTelemetry();
                foreach (Telemetry telemetry in telemetryList)
                {
                    telemetry.GForceData = GForceService.GetGForce(telemetry.GForceData.GForceId);
                    telemetry.AttitudeData = AttitudeService.GetAttitude(telemetry.AttitudeData.AttitudeId);

                }
                return telemetryList;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

        }

        [HttpGet("GetTailNumbers")]
        public IEnumerable<string> GetTailNumbers()
        {
            try
            {
                return AircraftService.GetAllAircraftTailNumbers();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

        }


        [HttpPost("GetFiltered")]
        public IEnumerable<Telemetry> GetFilteredAircraftData(FilteredAircraftRequest filteredAircraftRequest)
        {
            try
            {
                List<Telemetry> telemetryList = new List<Telemetry>();
                telemetryList = TelemetryService.GetTelemetry(filteredAircraftRequest.AircraftTailNumber, filteredAircraftRequest.StartDate, filteredAircraftRequest.EndDate);
                foreach (Telemetry telemetry in telemetryList)
                {
                    telemetry.GForceData = GForceService.GetGForce(telemetry.GForceData.GForceId);
                    telemetry.AttitudeData = AttitudeService.GetAttitude(telemetry.AttitudeData.AttitudeId);

                }
                return telemetryList;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw ex;
            }

        }
    }
}
