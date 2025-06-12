using System;
using System.Collections.Generic;
using System.Linq;
using Core.Model;
using Core.Services;
using Microsoft.EntityFrameworkCore;
using ProgrammingWithPalermo.ChurchBulletin.DataAccess.Mappings;

namespace ProgrammingWithPalermo.ChurchBulletin.DataAccess.Handlers
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly DataContext _context;

        public EmployeeRepository(DataContext context)
        {
            _context = context;
        }

        public Employee GetByUserName(string userName)
        {
            return _context.Set<Employee>()
                .Include("Roles")
                .SingleOrDefault(emp => emp.UserName == userName);
        }

        public Employee[] GetEmployees(EmployeeSpecification spec)
        {
            IQueryable<Employee> query = _context.Set<Employee>()
                .Include("Roles");
            
            // Get employees with their roles
            var employees = query.ToList();
            
            // If we need to filter by CanFulfill capability, do it in memory
            if (spec.CanFulfill)
            {
                employees = employees.Where(e => e.CanFulfilWorkOrder()).ToList();
            }
            
            // Sort the employees
            return employees.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToArray();
        }
    }
}