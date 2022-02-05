using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.ModelBinders;

namespace Net5WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private IRepositoryManager _repositoryManager;
        private IMapper _mapper;
        private ILoggerManager _logger;
        public CompaniesController(IRepositoryManager repositoryManager,IMapper mapper,ILoggerManager logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("GetCompanies")]
        public async Task<IActionResult> GetCompanies()
        {
            var companies =await _repositoryManager.Company.GetAllCompanies(trackChanges: false);
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }

        [HttpGet]
        [Route("GetCompany/{id}")]
        public async Task<IActionResult> GetCompany([FromRoute] Guid id)
        {
            var company =await _repositoryManager.Company.GetCompany(id, false);
            if (company is null)
            {
                _logger.LogWarn($"Company with {id} does not exist");
                return NotFound();
            }
            var companyDTO = _mapper.Map<CompanyDto>(company);
            return Ok(companyDTO);
        }

        [HttpGet]
        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async  Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType =
typeof(ArrayModelBinder))]IEnumerable<Guid> ids)
        {
            if(ids is null)
            {
                _logger.LogError("parameter ids is null");
                return BadRequest("Parameter ids is null");
            }
            var companies =await _repositoryManager.Company.GetCompanyByIds(ids,trackChnages:false);
            if(companies is null)
            {
                _logger.LogError("Companies with ids not found");
                return NotFound("Companies with ids not found");
            }
            var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
            return Ok(companiesDto);
        }
        [HttpPost]
        [Route("AddCompany")]
        public IActionResult AddCompany([FromBody] CompanyForCreationDto companyDTO)
        {
            if(companyDTO is null)
            {
                _logger.LogError($"CompanyDTO sent by client is null");
                return BadRequest("CompanyCreationDTO is null");
            }
            var companyEntity = _mapper.Map<Company>(companyDTO);
            _repositoryManager.Company.AddCompany(companyEntity);
            _repositoryManager.SaveAsync();
            var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);
            return CreatedAtAction(nameof(GetCompany),new { id=companyToReturn.Id},companyToReturn);
        }

        [HttpPost]
        [Route("collection")]

        public async Task<IActionResult> AddCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            if (companyCollection == null)
            {
                _logger.LogError("Company collection sent from client is null.");
                return BadRequest("Company collection is null");
            }
            var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
            foreach (var company in companyEntities)
            {
                _repositoryManager.Company.AddCompany(company);
            }
            await _repositoryManager.SaveAsync();
            var companyCollectionToReturn =
           _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
            var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
            return CreatedAtRoute("CompanyCollection", new {ids},
           companyCollectionToReturn);
        }

        [HttpDelete]
        [Route("DeleteCompany/{id}")]
        public async  Task<IActionResult> DeleteCompany(Guid id)
        {
            var company =await _repositoryManager.Company.GetCompany(id, trackChanges: false);
            if(company is null)
            {
                _logger.LogError($"Company with {id} does not exists");
                return NotFound();
            }
            _repositoryManager.Company.DeleteCompany(company);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }

        [HttpPut]
        [Route("UpdateCompany/{id}")]
        public async Task<IActionResult> UpdateCompany(Guid id,[FromBody]CompanyForUpdateDto company)
        {
            if (company == null)
            {
                _logger.LogError("CompanyForUpdateDto object sent from client is null.");
                return BadRequest("CompanyForUpdateDto object is null");
            }
            var companyEntity =await _repositoryManager.Company.GetCompany(id, trackChanges: true);
            if(companyEntity is null)
            {
                _logger.LogError($"Company with {id} does not exist");
                return NotFound();
            }
            _mapper.Map(company,companyEntity);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }
    }
}
