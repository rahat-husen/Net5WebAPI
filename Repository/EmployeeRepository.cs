using Contracts;
using Entities;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext): base(repositoryContext)
        {

        }

        public void CreateEmployeeForCompany(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            Create(employee);
        }

        public void DeleteEmployee(Employee employee)
        {
            Delete(employee);
        }

        public async Task<Employee> GetEmployee(Guid companyId, Guid employeeId, bool trackChanges)
        {
            return await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(employeeId), trackChanges).SingleOrDefaultAsync();
        }

        public async Task<PagedList<Employee>> GetEmployees(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
             var employees=await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges).OrderBy(e=>e.Name).Skip((employeeParameters.PageNumber-1)*employeeParameters.PageSize).Take(employeeParameters.PageSize).ToListAsync();
            int count = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges).CountAsync();
            return new PagedList<Employee>(employees, employeeParameters.PageNumber, employeeParameters.PageSize,count);
        }
    }
}
