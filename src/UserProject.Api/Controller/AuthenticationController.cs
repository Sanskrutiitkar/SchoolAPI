
using Microsoft.AspNetCore.Mvc;
using UserProject.Api.Constants;
using UserProject.Api.DTOs;
using UserProject.Business.Services;

namespace UserProject.Api.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthenticationController(IAuthService authService)
        {
            _authService = authService;
        }


        /// <summary>
        /// Authenticates a user and returns a JWT token.
        /// </summary>
        /// <param name="loginRequest">The login request data containing user credentials.</param>
        /// <returns>A JWT token if authentication is successful.</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="401">If the credentials are invalid</response>
        [HttpPost]  
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))] 
 
        // public async Task<IActionResult> Login(LoginRequestDto loginRequest)
        // {
        //     try
        //     {
        //         var token = await _authService.Login(loginRequest.UserEmail, loginRequest.Password);
        //         return Ok(new { Token = token });
        //     }
        //     catch (UnauthorizedAccessException)
        //     {
        //         return Unauthorized(new { message = ExceptionMessages.InvalidCredentials});
        //     }  
        // }
        public async Task<string> Login(string userEmail, string password)
        {
            var user = await _authService.ValidateUser(userEmail, password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            // Validate the password by comparing the entered password's hash with the stored hash
            // if (!_authService.VerifyPassword(password, user.UserPassword, user.PasswordSalt))
            // {
            //     throw new UnauthorizedAccessException("Invalid username or password");
            // }

            var claims = await  _authService.GenerateClaims(user);
            var token = _authService.GenerateToken(claims);
            return token;
        }

    }
}