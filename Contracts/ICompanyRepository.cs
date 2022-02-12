using Entities.Models;
using Entities.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Company>> GetAllCompanies(CompanyParameters companyParameters, bool trackChanges);
        Task<Company> GetCompany(Guid id, bool trackChanges);
        Task<IEnumerable<Company>> GetCompanyByIds(IEnumerable<Guid> ids, bool trackChnages);

        void AddCompany(Company company);
        void DeleteCompany(Company company);
    }
}
