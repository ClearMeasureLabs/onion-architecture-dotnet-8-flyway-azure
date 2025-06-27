using Core.Model;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Handlers;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.DataAccess;
using System.Threading.Tasks;

namespace IntegrationTests.DataAccess
{
    [TestFixture]
    public class EmployeeRepositoryTester
    {
        [Test]
        public async Task ShouldFindEmployeeByUsername()
        {
            new DatabaseTester().Clean();

            var one = new Employee("1", "first1", "last1", "email1");
            var two = new Employee("2", "first2", "last2", "email2");
            var three = new Employee("3", "first3", "last3", "email3");
            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(one);
                context.Add(two);
                context.Add(three);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            IEmployeeRepository repository = new EmployeeRepository(dataContext);
            Employee employee = await repository.GetByUserNameAsync("1");
            Assert.That(employee.Id, Is.EqualTo(one.Id));
        }

        [Test]
        public async Task ShouldGetAllEmployees()
        {
            new DatabaseTester().Clean();

            var one = new Employee("1", "first1", "last1", "email1");
            var two = new Employee("2", "first2", "last2", "email2");
            var three = new Employee("3", "first3", "last3", "email3");
            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(two);
                context.Add(three);
                context.Add(one);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            IEmployeeRepository repository = new EmployeeRepository(dataContext);
            Employee[] employees = await repository.GetEmployeesAsync(EmployeeSpecification.All);

            Assert.That(employees.Length, Is.EqualTo(3));
            Assert.That(employees[0].UserName, Is.EqualTo("1"));
            Assert.That(employees[0].FirstName, Is.EqualTo("first1"));
            Assert.That(employees[0].LastName, Is.EqualTo("last1"));
            Assert.That(employees[0].EmailAddress, Is.EqualTo("email1"));
        }

        [Test]
        public async Task ShouldGetAllEmployeesForFulfillment()
        {
            new DatabaseTester().Clean();

            var role = new Role("foo", false, true);
            var one = new Employee("1", "first1", "last1", "email1");
            one.AddRole(role);
            var two = new Employee("2", "first2", "last2", "email2");
            two.AddRole(role);
            var three = new Employee("3", "first3", "last3", "email3");
            
            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(role);
                context.Add(one);
                context.Add(two);
                context.Add(three);
                context.SaveChanges();
            }

            var dataContext = TestHost.GetRequiredService<DataContext>();
            IEmployeeRepository repository = new EmployeeRepository(dataContext);
            Employee[] employees = await repository.GetEmployeesAsync(new EmployeeSpecification(true));

            Assert.That(employees.Length, Is.EqualTo(2));
        }

        [Test]
        public void ShouldSaveRolesWithEmployee()
        {
            new DatabaseTester().Clean();
            
            var role1 = new Role("foo", false, false);
            var role2 = new Role("bar", true, true);
            var emp1 = new Employee("1", "first1", "last1", "email1");
            emp1.AddRole(role1);
            emp1.AddRole(role2);

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                context.Add(role1);
                context.Add(role2);
                context.Add(emp1);
                context.SaveChanges();
            }

            using (var context = TestHost.GetRequiredService<DbContext>())
            {
                var rehydratedEmployee = context.Set<Employee>()
                    .Include("Roles")
                    .Single(e => e.Id == emp1.Id);
                
                Assert.That(rehydratedEmployee.Roles.Count, Is.EqualTo(2));
            }
        }
    }
}