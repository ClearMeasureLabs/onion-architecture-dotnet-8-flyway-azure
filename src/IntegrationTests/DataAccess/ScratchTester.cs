using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Core.Model;
using NUnit.Framework;
using Shouldly;

namespace IntegrationTests.DataAccess
{
    [TestFixture, Explicit]
    public class ScratchTester
    {
        [Test]
        public void Foo1()
        {
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            var query = context.Set<WorkOrder>()
                .Include(w => w.Creator)
                .Where(w => w.Creator!.FirstName == "P-Mo")
                .OrderBy(w => w.Number);

            var list = query.ToList();

            foreach (var order in list)
            {
                Console.WriteLine(order);
            }
        }

        [Test]
        public void Foo3()
        {
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            var list = context.Set<WorkOrder>()
                .Include(w => w.Creator)
                .Where(w => w.Creator!.FirstName == "P-Mo")
                .ToList();

            foreach (var order in list)
            {
                Console.WriteLine(order);
                Console.WriteLine(order.Creator!.EmailAddress);
                order.Creator.EmailAddress = "foo@bar.com";
                Console.WriteLine(order.Creator.EmailAddress);
            }
        }

        [Test]
        public void Foo4()
        {
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            var list = context.Set<WorkOrder>()
                .Include(w => w.AuditEntries)
                .ToList();

            foreach (var order in list)
            {
                Console.WriteLine(order);
            }
        }

        [Test]
        public void Foo5()
        {
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            var count = context.Set<WorkOrder>().Count();

            Console.WriteLine(count);
        }

        [Test]
        public void Foo6()
        {
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            var role = new Role("", true, true);
            var employee = new Employee("1", "1", "1", "1");
            employee.AddRole(role);

            context.Add(role);
            context.Add(employee);
            context.SaveChanges();
        }

        [Test]
        public void Foo8()
        {
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            var list = context.Set<Employee>().ToList();
            
            foreach (var employee in list)
            {
                Console.WriteLine(employee.GetType().Name + " - " + employee);
            }
        }

        [Test]
        public void Foo11()
        {
            EmptyDatabase();
            
            using var context = ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>();
            context.Database.ExecuteSqlRaw("print 'howdy yall'");
            
            var list = context.Set<Employee>().ToList();
            foreach (var employee in list)
            {
                Console.WriteLine(employee.GetType().Name + " - " + employee);
            }
        }

        private void EmptyDatabase()
        {
            new ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.DatabaseEmptier(
                ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.TestHost.GetRequiredService<DbContext>().Database).DeleteAllData();
        }
    }
}