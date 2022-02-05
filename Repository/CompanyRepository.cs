using Contracts;
using Entities;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class CompanyRepository : RepositoryBase<Company>,ICompanyRepository
    {
        public CompanyRepository(RepositoryContext repositoryContext) :
            base(repositoryContext)
        {

        }

        public void AddCompany(Company company)
        {
            Create(company);
        }

        public void DeleteCompany(Company company)
        {
            Delete(company);
        }

        public async Task<IEnumerable<Company>> GetAllCompanies(bool trackChanges)
        => await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();
        public async Task<Company> GetCompany( Guid id, bool trackChanges) =>
           await FindByCondition(c => c.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

        public async Task<IEnumerable<Company>> GetCompanyByIds(IEnumerable<Guid> ids, bool trackChnages)
        {
           return await  FindByCondition(c=>ids.Contains(c.Id),trackChnages).ToListAsync();
        }
    }
}
