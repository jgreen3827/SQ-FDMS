using NUnit.Framework;
using System.Collections.Generic;

namespace FDMS.UnitTest
{
    public class GForceServiceUnitTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void UnitTestInsertGForce()
        {
            Shared.GForce gForce = new Shared.GForce();
            gForce.AccelX = (float)4.001336;
            gForce.AccelY = (float)1.033695;
            gForce.AccelZ = (float)1.001336;
            gForce.Weight = (float)1232.001336;
            Assert.Pass();

        }

        [Test]
        public void UnitTestGetGForce()
        {

            Shared.GForce expected = new Shared.GForce();
            expected.GForceId = 30;
            expected.AccelX = 0.25779300928115845;
            expected.AccelY = 1.8596760034561157;
            expected.AccelZ = 2.1054549217224121;
            expected.Weight = 2154.67041015625;
            Shared.GForce actual = Server.Services.GForceService.GetGForce(30);
            Assert.AreEqual(expected.AccelX, actual.AccelX);
            Assert.AreEqual(expected.AccelY, actual.AccelY);
            Assert.AreEqual(expected.AccelZ, actual.AccelZ);
            Assert.AreEqual(expected.Weight, actual.Weight);

        }

        [Test]
        public void UnitTestGetAllGforce()
        {
            Assert.NotNull(Server.Services.GForceService.GetAllGForce());
        }

    }
}