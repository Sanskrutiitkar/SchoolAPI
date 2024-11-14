
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolApi.Core.Business.Filter;
using SchoolApi.Core.Business.GlobalException;
using UserProject.Api.Constants;
using UserProject.Api.DTOs;
using UserProject.Business.Models;
using UserProject.Business.Repository;
using UserProject.Business.Services;

namespace UserProject.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles =RoleConstant.Admin)]
    [ModelValidationFilter] 
    
    public class UserController : ControllerBase
    {
         private readonly IUserRepo _userRepo;
        private readonly IMapper _mapper;
        private readonly IAuthService _authService;

        public UserController(IUserRepo userRepo, IMapper mapper,IAuthService authService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _authService=authService;

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
    public async Task<ActionResult<UserRegistrationDto>> Register(UserRegistrationDto registrationDto)
    {
        var existingUser = await _userRepo.GetUserByEmail(registrationDto.UserEmail);
        if (existingUser != null)
        {
            throw new DuplicateEntryException(ExceptionMessages.AlreadyExists);
        }

        // Generate a unique salt and hash the password
        var (hashedPassword, salt) = _authService.HashPassword(registrationDto.UserPassword);

        var newUser = _mapper.Map<Users>(registrationDto);
        newUser.UserPassword = hashedPassword;
        newUser.PasswordSalt = salt; // Store the salt in the database along with the hash

        var createdUser = await _userRepo.AddUser(newUser);
        var mappedUser = _mapper.Map<UserRegistrationDto>(createdUser);
        
        return Ok(mappedUser);
    }

        /// <summary>
        /// Retrieves all users.
        /// </summary>
        /// <returns>A list of UserRequestDto objects representing all users.</returns>
        /// <response code="200">Returns a list of users</response>
  
        [HttpGet]  
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserRequestDto>))] 
        public async Task<ActionResult<IEnumerable<UserRequestDto>>> Get(){
            var users = await _userRepo.GetAllUser();
            var dtoResponse = _mapper.Map<IEnumerable<UserRequestDto>>(users);                
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
                return NotFound(new { message = ExceptionMessages.UserNotFound });
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
        public async Task<ActionResult<UserRequestDto>> GetUserById(int id)
        {
            var user = await _userRepo.GetUserById(id);
            if (user == null)
            {
                  return NotFound(new { message = ExceptionMessages.UserNotFound });
            }

            var dtoResponse = _mapper.Map<UserRequestDto>(user);
            return Ok(dtoResponse);
        }



    }
}