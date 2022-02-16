using AutoMapper;
using Contracts;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [ApiVersion("2.0")]
    [Route("api/{v:apiversion}/[controller]")]
    [ApiController]
    public class CompaniesV2Controller : ControllerBase
    {
        private IRepositoryManager _repositoryManager;
        private IMapper _mapper;
        private ILoggerManager _logger;
        private IDistributedCache _distributedCache;
        public CompaniesV2Controller(IRepositoryManager repositoryManager, IMapper mapper, ILoggerManager logger,IDistributedCache distributedCache)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        [Route("GetCompanies")]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            bool IsCached = false;
            IEnumerable<Company> companies = new List<Company>();
            var cachedCompanies = await _distributedCache.GetStringAsync("companies");
            if (cachedCompanies != null)
            {
                companies = JsonConvert.DeserializeObject<List<Company>>(cachedCompanies);
                IsCached = true;
            }
            else
            {
                companies = await _repositoryManager.Company.GetAllCompanies(companyParameters, trackChanges: false);
                cachedCompanies = JsonConvert.SerializeObject(companies);
                var expirationOptions = new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
                    SlidingExpiration = TimeSpan.FromMinutes(3)
                };
                await _distributedCache.SetStringAsync("companies", cachedCompanies,expirationOptions);
            }             
            return Ok(new { IsCached ,companies});
        }
    }
}
