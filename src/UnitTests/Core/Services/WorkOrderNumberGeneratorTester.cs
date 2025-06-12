using Core.Services.Impl;
using NUnit.Framework;

namespace UnitTests.Core.Services
{
    [TestFixture]
    public class WorkOrderNumberGeneratorTester
    {
        [Test]
        public void ShouldBeFiveInLength()
        {
            var generator = new WorkOrderNumberGenerator();
            string number = generator.GenerateNumber();

            Assert.That(number.Length, Is.EqualTo(5));
        }
    }
}