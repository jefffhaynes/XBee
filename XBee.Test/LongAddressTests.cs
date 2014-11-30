using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XBee.Test
{
    [TestClass]
    public class LongAddressTests
    {
        [TestMethod]
        public void RoundtripShort()
        {
            var address = new LongAddress(1);
            Assert.AreEqual(1UL, address.Value);
            Assert.AreEqual(1U, address.Low);
            Assert.AreEqual(0U, address.High);
        }

        [TestMethod]
        public void RoundtripLong()
        {
            var address = new LongAddress(0x1000000000000000UL);
            Assert.AreEqual(0x1000000000000000UL, address.Value);
            Assert.AreEqual(0U, address.Low);
            Assert.AreEqual(0x10000000U, address.High);
        }
    }
}
