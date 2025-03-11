using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("users")]
    public class UsersController : ApiController
    {
        private readonly ISender _mediator;

        public UsersController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userResult = await _mediator.Send(new GetUserByIdQuery(id));
            return userResult.Match(
                user => Ok(user),
                errors => Problem(errors));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var createUserResult = await _mediator.Send(command);
            return createUserResult.Match(
                user => Ok(),
                errors => Problem(errors));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            var updatedCommand = new UpdateUserCommand(id, request.Name, request.LastName, request.Email);
            var updateUserResult = await _mediator.Send(updatedCommand);

            return updateUserResult.Match(
                userId => NoContent(),
                errors => Problem(errors));
        }
        public record UpdateUserRequest(string Name, string LastName, string Email);


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteUserResult = await _mediator.Send(new DeleteUserCommand(id));
            return deleteUserResult.Match(
                customerId => NoContent(),
                errors => Problem(errors));
        }
    }
}