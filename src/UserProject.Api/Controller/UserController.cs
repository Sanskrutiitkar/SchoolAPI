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
        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <param name="registrationDto">The user registration data.</param>
        /// <returns>A UserRegistrationDto object representing the created user.</returns>
        /// <response code="200">Returns the created user</response>
        /// <response code="400">If the registration data is invalid</response>
        /// <response code="409">If a user with the same email already exists</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRegistrationDto))] 
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
        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of UserRequestDto objects representing all users.</returns>
        /// <response code="200">Returns a list of users</response>
        /// <response code="404">If no users are found</response>
        [HttpGet]  
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserRequestDto>))] 
       
        public async Task<ActionResult<UserRequestDto>> Get(){
            var users = await _userRepo.GetAllUser();
            var dtoResponse = _mapper.Map<IEnumerable<UserRequestDto>>(users);
            if(dtoResponse.Count()==0){
                return NotFound(dtoResponse);
            }
                    
            return Ok(dtoResponse);
        }

        /// <summary>
        /// Deletes a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to delete.</param>
        /// <returns>No content if the deletion is successful.</returns>
        /// <response code="200">User successfully deleted</response>
        /// <response code="404">If the user is not found or already inactive</response>
        [HttpDelete("{id}")]   
        [ProducesResponseType(StatusCodes.Status200OK)]
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
        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>A UserRequestDto object representing the user.</returns>
        /// <response code="200">Returns the user details</response>
        /// <response code="404">If the user is not found</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserRequestDto))] 
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