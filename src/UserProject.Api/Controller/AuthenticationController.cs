
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
 
        public async Task<string> Login(LoginRequestDto loginRequest)
        {
            var user = await _authService.ValidateUser(loginRequest.UserEmail, loginRequest.Password);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid username or password");
            }

            var claims = await  _authService.GenerateClaims(user);
            var token = _authService.GenerateToken(claims);
            return token;
        }

    }
}