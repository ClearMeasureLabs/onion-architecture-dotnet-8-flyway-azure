using Core.Model;
using NUnit.Framework;

namespace UnitTests.Core.Model
{
    [TestFixture]
    public class WorkOrderStatusTester
    {
        [Test]
        public void ShouldListAllStatuses()
        {
            WorkOrderStatus[] statuses = WorkOrderStatus.GetAllItems();

            Assert.That(statuses.Length, Is.EqualTo(5));
            Assert.That(statuses[0], Is.EqualTo(WorkOrderStatus.Draft));
            Assert.That(statuses[1], Is.EqualTo(WorkOrderStatus.Assigned));
            Assert.That(statuses[2], Is.EqualTo(WorkOrderStatus.InProgress));
            Assert.That(statuses[3], Is.EqualTo(WorkOrderStatus.Complete));
            Assert.That(statuses[4], Is.EqualTo(WorkOrderStatus.Cancelled));
        }

        [Test]
        public void CanParseOnKey()
        {
            WorkOrderStatus draft = WorkOrderStatus.Parse("draft");
            Assert.That(draft, Is.EqualTo(WorkOrderStatus.Draft));

            WorkOrderStatus assigned = WorkOrderStatus.Parse("assigned");
            Assert.That(assigned, Is.EqualTo(WorkOrderStatus.Assigned));

            WorkOrderStatus inprogress = WorkOrderStatus.Parse("inprogress");
            Assert.That(inprogress, Is.EqualTo(WorkOrderStatus.InProgress));

            WorkOrderStatus complete = WorkOrderStatus.Parse("complete");
            Assert.That(complete, Is.EqualTo(WorkOrderStatus.Complete));
        }
    }
}