using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClearMeasure.Bootcamp.Core.Model;
using ClearMeasure.Bootcamp.Core.Services;
using ClearMeasure.Bootcamp.DataAccess.Mappings;
using Microsoft.EntityFrameworkCore;

namespace ClearMeasure.Bootcamp.DataAccess.Handlers
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<Employee> GetByUserNameAsync(string? userName)
        {
            return await _context.Set<Employee>()
                .Include("Roles")
                .SingleAsync(emp => emp.UserName == userName);
        }

        public async Task<Employee[]> GetEmployeesAsync(EmployeeSpecification spec)
        {
            IQueryable<Employee> query = _context.Set<Employee>()
                .Include("Roles");
            var employees = await query.ToListAsync();
            if (spec.CanFulfill)
            {
                employees = employees.Where(e => e.CanFulfilWorkOrder()).ToList();
            }
            return employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToArray();
        }
    }
}