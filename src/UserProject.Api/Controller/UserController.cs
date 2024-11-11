using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using UserProject.Api.DTOs;
using UserProject.Api.Exceptions;
using UserProject.Api.GlobalException;
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

        [HttpPost]
        public async Task<ActionResult<UserRegistrationDto>> Register([FromBody] UserRegistrationDto registrationDto)
        {
            var existingUser = await _userRepo.GetUserByEmail(registrationDto.UserEmail);
            if (existingUser != null)
            {
                throw new DuplicateEntryException(ExceptionMessages.AlreadyExists);
            }

            var newUser = _mapper.Map<Users>(registrationDto);
            var createdUser = await _userRepo.AddUser(newUser);
            var mappedUser = _mapper.Map<UserRegistrationDto>(createdUser);
            return Ok(mappedUser);
        }

        [HttpGet]
        public async Task<ActionResult<UserRequestDto>> Get(){
            var users = await _userRepo.GetAllUser();
            var dtoResponse = _mapper.Map<IEnumerable<UserRequestDto>>(users);
            if(dtoResponse.Count()==0){
                return NotFound(dtoResponse);
            }
                    
            return Ok(dtoResponse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var existingUser = await _userRepo.GetUserById(id);
            if (existingUser == null){
                return NotFound(ExceptionMessages.UserNotFound);
            }
            if(!existingUser.IsActive)
            {
                return NotFound(ExceptionMessages.AlreadyInactive);
            }
         
            await _userRepo.DeleteUser(id);
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserRequestDto>> GetStudentById(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null)
            {
                return NotFound(ExceptionMessages.UserNotFound);
            }

            var dtoResponse = _mapper.Map<UserRequestDto>(user);
            return Ok(dtoResponse);
        }



    }
}