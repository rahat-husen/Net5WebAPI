using Entities.Models;
using Repository.Extensions.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
namespace Repository.Extensions
{
    public static class RepositoryCompanyExtensions
    {
        public static IQueryable<Company> FilterCompanies(this IQueryable<Company> companies)
        {
            return companies;
        }

        public static IQueryable<Company> Sort(this IQueryable<Company> companies,string orderByQueryString)
        {
            if (orderByQueryString is null)
                return companies.OrderBy(x => x.Name);
            var orderByQuery = OrderQueryBuilder.CreateOrderQuery<Company>(orderByQueryString);
            if (orderByQuery is null)
                return companies.OrderBy(x => x.Name);
            return companies.OrderBy(orderByQuery);
        }
    }
}
