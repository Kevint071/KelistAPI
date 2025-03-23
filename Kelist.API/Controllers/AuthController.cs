using Application.AuthUsers.Command.LoginUser;
using Application.AuthUsers.Command.RefreshTokenUser;
using Application.AuthUsers.Command.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(IMediator mediator) : ApiController
    {
        private readonly IMediator _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Match<IActionResult>(
                user => Created($"/users/{user.Id}", user),
                errors => Problem(errors)
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserCommand command)
        {
            var result = await _mediator.Send(command);
            return result.Match<IActionResult>(
                token => Ok(token),
                errors => Problem(errors)
            );
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenUserCommand command)
        {
            var result = await _mediator.Send(command);
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
