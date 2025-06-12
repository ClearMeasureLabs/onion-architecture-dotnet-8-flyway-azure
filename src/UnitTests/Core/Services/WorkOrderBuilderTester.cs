using System;
using Core.Model;
using Core.Services;
using Core.Services.Impl;
using NUnit.Framework;
using Rhino.Mocks;

namespace UnitTests.Core.Services
{
    [TestFixture]
    public class WorkOrderBuilderTester
    {
        [Test]
        public void ShouldCorrectlyBuildAWorkOrder()
        {
            var mocks = new MockRepository();
            var generator = mocks.StrictMock<IWorkOrderNumberGenerator>();
            ICalendar calendar = new StubbedCalendar(new DateTime(2000, 1, 1));
            Expect.Call(generator.GenerateNumber()).Return("124");
            mocks.ReplayAll();

            var builder = new WorkOrderBuilder(generator, calendar);
            var creator = new Employee();
            WorkOrder workOrder = builder.CreateNewWorkOrder(creator);

            mocks.VerifyAll();
            Assert.That(workOrder.Creator, Is.EqualTo(creator));
            Assert.That(workOrder.Number, Is.EqualTo("124"));
            Assert.That(workOrder.Assignee, Is.Null);
            Assert.That(workOrder.Title, Is.Empty);
            Assert.That(workOrder.Description, Is.Empty);
            Assert.That(workOrder.Status, Is.EqualTo(WorkOrderStatus.Draft));
            Assert.That(workOrder.RoomNumber, Is.Null);
        }
    }
}