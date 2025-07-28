using System;
using Core.Model;
using NUnit.Framework;
using Shouldly;

namespace UnitTests.Core.Model
{
    [TestFixture]
    public class AuditEntryTester
    {
        private Employee _employee1;
        private Employee _employee2;
        private DateTime _date1;
        private DateTime _date2;

        [SetUp]
        public void Setup()
        {
            _employee1 = new Employee("user1", "John", "Doe", "john@example.com")
            {
                Id = Guid.NewGuid()
            };
            _employee2 = new Employee("user2", "Jane", "Smith", "jane@example.com")
            {
                Id = Guid.NewGuid()
            };
            _date1 = new DateTime(2023, 10, 15, 14, 30, 45, 123);
            _date2 = new DateTime(2023, 10, 16, 15, 31, 46, 456);
        }

        [Test]
        public void ShouldBeEqualWhenAllPropertiesMatch()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.ShouldBe(entry2);
            entry2.ShouldBe(entry1);
            (entry1 == entry2).ShouldBeTrue();
            (entry1 != entry2).ShouldBeFalse();
        }

        [Test]
        public void ShouldBeEqualWhenDatesOnlyDifferByMilliseconds()
        {
            var dateWithMilliseconds = new DateTime(2023, 10, 15, 14, 30, 45, 123);
            var sameDateNoMilliseconds = new DateTime(2023, 10, 15, 14, 30, 45, 0);
            
            var entry1 = new AuditEntry(_employee1, dateWithMilliseconds, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, sameDateNoMilliseconds, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.ShouldBe(entry2);
            entry2.ShouldBe(entry1);
        }

        [Test]
        public void ShouldNotBeEqualWhenEmployeesDiffer()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee2, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.ShouldNotBe(entry2);
            entry2.ShouldNotBe(entry1);
            (entry1 == entry2).ShouldBeFalse();
            (entry1 != entry2).ShouldBeTrue();
        }

        [Test]
        public void ShouldNotBeEqualWhenDatesDifferByMoreThanMilliseconds()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date2, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.ShouldNotBe(entry2);
            entry2.ShouldNotBe(entry1);
        }

        [Test]
        public void ShouldNotBeEqualWhenArchivedEmployeeNamesDiffer()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            entry2.ArchivedEmployeeName = "Different Name";

            entry1.ShouldNotBe(entry2);
            entry2.ShouldNotBe(entry1);
        }

        [Test]
        public void ShouldNotBeEqualWhenBeginStatusesDiffer()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date1, WorkOrderStatus.InProgress, WorkOrderStatus.Assigned);

            entry1.ShouldNotBe(entry2);
            entry2.ShouldNotBe(entry1);
        }

        [Test]
        public void ShouldNotBeEqualWhenEndStatusesDiffer()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.InProgress);

            entry1.ShouldNotBe(entry2);
            entry2.ShouldNotBe(entry1);
        }

        [Test]
        public void ShouldNotBeEqualToNull()
        {
            var entry = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry.ShouldNotBe(null);
            entry.Equals(null).ShouldBeFalse();
            (entry == null).ShouldBeFalse();
            (entry != null).ShouldBeTrue();
        }

        [Test]
        public void ShouldBeEqualToSameReference()
        {
            var entry = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry.ShouldBe(entry);
            entry.Equals(entry).ShouldBeTrue();
#pragma warning disable CS1718 // Comparison made to same variable; intentional for testing equality operators
            (entry == entry).ShouldBeTrue();
            (entry != entry).ShouldBeFalse();
#pragma warning restore CS1718
        }

        [Test]
        public void ShouldNotBeEqualToDifferentType()
        {
            var entry = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var otherObject = "not an audit entry";

            entry.Equals(otherObject).ShouldBeFalse();
        }

        [Test]
        public void ShouldHandleNullOperatorEquality()
        {
            AuditEntry? entry1 = null;
            AuditEntry? entry2 = null;
            var entry3 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            (entry1 == entry2).ShouldBeTrue();
            (entry1 != entry2).ShouldBeFalse();
            (entry1 == entry3).ShouldBeFalse();
            (entry1 != entry3).ShouldBeTrue();
            (entry3 == entry1).ShouldBeFalse();
            (entry3 != entry1).ShouldBeTrue();
        }

        [Test]
        public void ShouldGenerateConsistentHashCode()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.GetHashCode().ShouldBe(entry2.GetHashCode());
        }

        [Test]
        public void ShouldGenerateDifferentHashCodesForDifferentObjects()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee2, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.GetHashCode().ShouldNotBe(entry2.GetHashCode());
        }

        [Test]
        public void ShouldUseTypedEqualsMethod()
        {
            var entry1 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, _date1, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.Equals(entry2).ShouldBeTrue();
        }

        [Test]
        public void ShouldHandleDefaultConstructorEquality()
        {
            var entry1 = new AuditEntry();
            var entry2 = new AuditEntry();

            entry1.ShouldBe(entry2);
            entry2.ShouldBe(entry1);
        }

        [Test]
        public void ShouldTruncateDateToSecondsInComparison()
        {
            var dateWithNanoseconds = new DateTime(2023, 10, 15, 14, 30, 45, 123, DateTimeKind.Utc).AddTicks(5000);
            var sameDateTruncated = new DateTime(2023, 10, 15, 14, 30, 45, 0, DateTimeKind.Utc);
            
            var entry1 = new AuditEntry(_employee1, dateWithNanoseconds, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);
            var entry2 = new AuditEntry(_employee1, sameDateTruncated, WorkOrderStatus.Draft, WorkOrderStatus.Assigned);

            entry1.ShouldBe(entry2);
        }
    }
}