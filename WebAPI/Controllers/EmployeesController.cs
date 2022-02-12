using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.ActionFilters;

namespace WebAPI.Controllers
{
    [ApiVersion("1.0")]
    [Route("api/{v:apiversion}/companies/{companyId}/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private IRepositoryManager _repositoryManager;
        private IMapper _mapper;
        private ILoggerManager _logger;
        public EmployeesController(IRepositoryManager repositoryManager, IMapper mapper, ILoggerManager logger)
        {
            _repositoryManager = repositoryManager;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [HttpHead]
        [Route("GetEmployees")]
        public async Task<IActionResult> GetEmployees(Guid companyId,[FromQuery] EmployeeParameters employeeParameters)
        {
            if (!employeeParameters.HasValidRange)
                return BadRequest("max age cannot be less than min age");
            var company = await _repositoryManager.Company.GetCompany(companyId, trackChanges: false);
            if(company is null)
            {
                _logger.LogError($"Company with {companyId} is not there");
                return NotFound();
            }
            var employees =await _repositoryManager.Employee.GetEmployees(companyId, employeeParameters, trackChanges: false);
            Response.Headers.Add("X-Pagination",JsonConvert.SerializeObject(employees.MetaData));
            var employeesDTO = _mapper.Map<IEnumerable<EmployeeDto>>(employees);
            return Ok(employeesDTO);
        }

        [HttpGet]
        [Route("GetEmployee/{employeeId}")]
        public async Task<IActionResult> GetEmployee(Guid companyId,Guid employeeId)
        {
            var company = await _repositoryManager.Company.GetCompany(companyId, trackChanges: false);
            if(company is null)
            {
                _logger.LogError($"Company with {companyId} not found");
                return NotFound();
            }
            var employee =await _repositoryManager.Employee.GetEmployee(companyId, employeeId, trackChanges: false);
            if(employee is null)
            {
                _logger.LogError($"Employee with {employeeId} not found");
                return NotFound();
            }
            var employeedto = _mapper.Map<EmployeeDto>(employee);
            return Ok(employeedto);
        }

        [HttpPost]
        [Route("AddEmployee")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public IActionResult AddEmployee(Guid companyId,[FromBody] EmployeeForCreationDto employeeDTO)
        {            
            var employeeEntity = _mapper.Map<Employee>(employeeDTO);
            _repositoryManager.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
            _repositoryManager.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return CreatedAtAction(nameof(GetEmployee), new { companyId = companyId, employeeId = employeeToReturn.Id }, employeeToReturn);           
        }

        [HttpDelete]
        [Route("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(Guid companyId,Guid id)
        {
            var company =await _repositoryManager.Company.GetCompany(companyId, trackChanges: false);
            if(company is null)
            {
                _logger.LogError($"Company with {companyId} does not exists");
                return NotFound("Company with companyid does not exists");
            }
            var employee = await _repositoryManager.Employee.GetEmployee(company.Id, id, trackChanges: false);
            if(employee is null)
            {
                _logger.LogError($"Employee with {id} does not exists");
                return NotFound();
            }
            _repositoryManager.Employee.DeleteEmployee(employee);
            await _repositoryManager.SaveAsync();
            return NoContent();
        }

        [HttpPut]
        [Route("UpdateEmployee/{id}")]
        public async Task<IActionResult> UpdateEmployee(Guid companyId,Guid id,[FromBody] EmployeeForUpdateDto employeeForUpdate)
        {
            if (employeeForUpdate == null)
            {
                _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var company =await _repositoryManager.Company.GetCompany(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogError($"Company with {companyId} does not exists");
                return NotFound("Company with companyid does not exists");
            }
            var employee =await _repositoryManager.Employee.GetEmployee(company.Id, id, trackChanges: true);
            if (employee is null)
            {
                _logger.LogError($"Employee with {id} does not exists");
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                _logger.LogError("employeeupdate dto is not valid");
                UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeForUpdate, employee);
           await _repositoryManager.SaveAsync();
            return NoContent();

        }
        [HttpPatch]
        [Route("UpdateEmployeePartially/{id}")]
        public async Task<IActionResult> UpdateEmployeePartially(Guid companyId,Guid id,[FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc == null)
            {
                _logger.LogError("EmployeeForUpdateDto object sent from client is null.");
                return BadRequest("EmployeeForUpdateDto object is null");
            }
            var company =await _repositoryManager.Company.GetCompany(companyId, trackChanges: false);
            if (company is null)
            {
                _logger.LogError($"Company with {companyId} does not exists");
                return NotFound("Company with companyid does not exists");
            }
            var employee =await _repositoryManager.Employee.GetEmployee(company.Id, id, trackChanges: true);
            if (employee is null)
            {
                _logger.LogError($"Employee with {id} does not exists");
                return NotFound();
            }
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employee);           
            patchDoc.ApplyTo(employeeToPatch);
            if (!ModelState.IsValid)
            {
                _logger.LogError("pathdto is not valid");
                UnprocessableEntity(ModelState);
            }
            _mapper.Map(employeeToPatch, employee);
           await _repositoryManager.SaveAsync();
            return NoContent();
        }
    }
}
