using Application.Users.Dtos;
using Application.Users.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IAuthService authService) : ApiController
    {
        private readonly IAuthService _authService = authService ?? throw new ArgumentNullException(nameof(authService));

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDto request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Match<IActionResult>(
                user => Created($"/users/{user.Id}", user),
                errors => Problem(errors)
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDto request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Match<IActionResult>(
                token => Ok(token),
                errors => Problem(errors)
            );
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await _authService.RefreshTokensAsync(request);
            return result.Match<IActionResult>(
                token => Ok(token),
                errors => Problem(errors)
            );
        }

        [Authorize]
        [HttpGet("protected")]
        public IActionResult ProtectedEndpoint()
        {
            return Ok("This is a protected endpoint.");
        }
    }
}
