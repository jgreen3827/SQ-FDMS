 using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FDMS.UnitTest
{
    public class TelemtryServiceUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UnitTestInsertTelemetry()
        {
            Shared.Telemetry telemetry = new Shared.Telemetry();
            telemetry.AircraftTailNumber = "C-WGGF";
            telemetry.GForceData = new Shared.GForce()
            {
                AccelX = (float)1.001336,
                AccelY = (float)3.001336,
                AccelZ = (float)0.001336,
                Weight = (float)1241.001336
            };
            telemetry.AttitudeData = new Shared.Attitude()
            {
                Bank = (float)1.001336,
                Pitch = (float)3.001336,
                Altitude = (float)0.001336
            };
            telemetry.TimeStamp = System.DateTime.Today;
            Server.Services.TelemetryService.CreateTelemetry(telemetry);
            Assert.Pass();

        }


        [Test]
        public void UnitTestGetAllTelemetry()
        {
            Assert.NotNull(Server.Services.TelemetryService.GetAllTelemetry());
        }

        [Test]
        public void UnitTestGetFilteredTelemetry()
        {
            DateTime start = new DateTime(2018, 01, 01, 3, 5, 5, DateTimeKind.Utc);
            DateTime end = new DateTime(2019, 01, 01, 3, 5, 5, DateTimeKind.Utc);
            Assert.NotNull(Server.Services.TelemetryService.GetTelemetry("C-FGAX", start, end));
        }
    }
}