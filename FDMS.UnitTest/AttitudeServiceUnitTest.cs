using NUnit.Framework;
using System.Collections.Generic;

namespace FDMS.UnitTest
{
    public class AttitudeServiceUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UnitTestInsertAircraft()
        {
            Shared.Attitude attitude = new Shared.Attitude();
            attitude.Altitude = (float)1124.1112344;
            attitude.Pitch = (float)0.033695;
            attitude.Bank = (float)0.001336;
            Assert.Pass();

        }

        [Test]
        public void UnitTestGetAttitude()
        {
            Shared.Attitude expected = new Shared.Attitude();
            expected.AttitudeId = 30;
            expected.Altitude = 1641.050048828125;
            expected.Pitch = 0.031823001801967621;
            expected.Bank = 0.034561000764369965;
            Shared.Attitude actual = Server.Services.AttitudeService.GetAttitude(30);
            Assert.AreEqual(expected.Altitude, actual.Altitude);
            Assert.AreEqual(expected.Pitch, actual.Pitch);
            Assert.AreEqual(expected.Bank, actual.Bank);

        }
        [Test]
        public void UniteTestGetAllAttitude()
        {
            Assert.NotNull(Server.Services.AttitudeService.GetAllAttitude());
        }

    }
}