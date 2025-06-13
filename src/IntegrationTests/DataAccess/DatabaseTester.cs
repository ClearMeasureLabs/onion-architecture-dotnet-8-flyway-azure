using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using ProgrammingWithPalermo.ChurchBulletin.IntegrationTests;

namespace ProgrammingWithPalermo.ChurchBulletin.IntegrationTests.DataAccess
{
    [TestFixture]
    public class DatabaseTester
    {
        [Test, Explicit, Category("DataSchema")]
        public void CreateDatabaseSchema()
        {
            var context = TestHost.GetRequiredService<DbContext>();
            context.Database.EnsureCreated();
        }

        public void Clean()
        {
            new DatabaseEmptier(TestHost.GetRequiredService<DbContext>().Database).DeleteAllData();
        }
    }
}