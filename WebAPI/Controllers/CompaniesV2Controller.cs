using AutoMapper;
using Contracts;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public CompaniesV2Controller(IRepositoryManager repositoryManager, IMapper mapper, ILoggerManager logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetCompanies")]
        public async Task<IActionResult> GetCompanies([FromQuery] CompanyParameters companyParameters)
        {
            var companies = await _repositoryManager.Company.GetAllCompanies(companyParameters, trackChanges: false);
            return Ok(companies);
        }
    }
}
