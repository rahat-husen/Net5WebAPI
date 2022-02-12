using AutoMapper;
using Contracts;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebAPI.ActionFilters;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private ILoggerManager _logger;
        private IMapper _mapper;
        private UserManager<User> _userManager;
        private readonly IAuthenticationManager _authenticationManager;
        public AuthenticationController(ILoggerManager logger,IMapper mapper,UserManager<User> userManager,IAuthenticationManager authenticationManager)
        {
            _logger = logger;
            _mapper = mapper;
            _userManager = userManager;
            _authenticationManager = authenticationManager;
        }
        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> RegisterUser([FromBody] UserForRegistrationDto userForRegistration)
        {
            var user = _mapper.Map<User>(userForRegistration);
            var result = await _userManager.CreateAsync(user,userForRegistration.Password);
            if (!result.Succeeded)
            {
                foreach(var error in result.Errors)
                {
                    ModelState.TryAddModelError(error.Code, error.Description);
                }
                return BadRequest(ModelState);
            }
            await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> UserLogin([FromBody] UserForAuthenticationDto userForAuthentication)
        {
            var isValidUser = await _authenticationManager.ValidateUser(userForAuthentication);
            if (!isValidUser)
            {
                _logger.LogError("User Credentials are Incorrect");
                return BadRequest("User Credentials are incorrect");
            }

            return Ok(new {Token=await _authenticationManager.CreateToken()});
        }
    }
}
