using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserProject.Api.DTOs;
using UserProject.Business.Models;
using UserProject.Business.Repository;

namespace UserProject.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles ="Admin")]
    public class UserController : ControllerBase
    {
         private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;

        public UserController(IUserRepo userRepo, IMapper mapper)
        {
            _userRepo = userRepo;
            _mapper = mapper;

        }

        [HttpPost("register")]
        public async Task<ActionResult<UserRegistrationDto>> Register([FromBody] UserRegistrationDto registrationDto)
        {

            var existingUser = await _userRepo.GetUserByEmail(registrationDto.UserEmail);
            if (existingUser == null)
            {
                return BadRequest("User with this email already exists.");
            }

            var newUser = _mapper.Map<Users>(registrationDto);
            var createdUser = await _userRepo.AddUser(newUser);
            var mappedUser = _mapper.Map<UserRegistrationDto>(createdUser);
            return Ok(mappedUser);
        }


    }
}