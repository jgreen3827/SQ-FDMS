using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace FDMS.UnitTest
{
    public class AircraftServiceUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UnitTestInsertAircraft()
        {
            string AircraftTailNumber = "C-FGAX";
            Server.Services.AircraftService.CreateAircraft(AircraftTailNumber);
            Assert.Pass();
        }

        [Test] //Test is currently only checking for 3 aircraft if more were added it wont work
        public void UnitTestGetAllAircraft()
        {
            
            List<String> actual = new List<String>();
            List<String> expected = new List<String>();
            expected.Add("C-FGAX    ");
            expected.Add("C-GEFC    ");
            expected.Add("C-QWWT    ");
            actual = Server.Services.AircraftService.GetAllAircraftTailNumbers();
            
            Assert.AreEqual(expected.Count, actual.Count);
            Assert.AreEqual(actual[0], expected[0]);
            Assert.AreEqual(actual[1], expected[1]);
            Assert.AreEqual(actual[2], expected[2]);
        }

        [Test] //Test is currently only checking for 3 aircraft if more were added it wont work
        public void UnitTestGetAircraft()
        {

            Shared.Aircraft actual = new Shared.Aircraft();
            String expected = "C-FGAX    ";

            actual = Server.Services.AircraftService.GetAircraft("C-FGAX    ");

            Assert.AreEqual(expected, actual.AircraftTailNumber);
        }
    }
}