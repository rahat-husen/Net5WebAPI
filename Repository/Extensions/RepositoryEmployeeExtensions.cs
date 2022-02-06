using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.Extensions
{
    public static class RepositoryEmployeeExtensions
    {
        public static IQueryable<Employee> FilterEmployees(this IQueryable<Employee> employees,uint minAge,uint maxAge)
        {
            return employees.Where(e => e.Age >= minAge && e.Age <= maxAge);
        }

        public static IQueryable<Employee> Search(this IQueryable<Employee> employees,string searchTerm)
        {
            if (searchTerm is null)
                return employees;
            return employees.Where(x => x.Name.ToLower().Contains(searchTerm.Trim().ToLower()));
        }
    }
}
