using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("users")]
    [Produces("application/json")]
    public class UsersController(ISender mediator) : ApiController
    {
        private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Obtiene todos los usuarios.
        /// </summary>
        /// <returns>Una lista de usuarios.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<UserDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var userResult = await _mediator.Send(new GetAllUsersQuery());

            return userResult.Match(
                users => Ok(users),
                errors => Problem(errors)
            );
        }
        /// <summary>
        /// Obtiene un usuario por su ID.
        /// </summary>
        /// <param name="id">ID del usuario.</param>
        /// <returns>Información del usuario.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var userResult = await _mediator.Send(new GetUserByIdQuery(id));
            return userResult.Match(
                user => Ok(user),
                errors => Problem(errors));
        }

        /// <summary>
        /// Crea un nuevo usuario.
        /// </summary>
        /// <param name="command">Datos del usuario a crear.</param>
        /// <returns>El usuario creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        {
            var createUserResult = await _mediator.Send(command);
            return createUserResult.Match(
                user => Created($"/users/{user.Id}", user),
                errors => Problem(errors));
        }

        /// <summary>
        /// Actualiza un usuario existente.
        /// </summary>
        /// <param name="id">ID del usuario a actualizar.</param>
        /// <param name="request">Datos actualizados del usuario.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request)
        {
            var updatedCommand = new UpdateUserCommand(id, request.Name, request.LastName, request.Email);
            var updateUserResult = await _mediator.Send(updatedCommand);

            return updateUserResult.Match(
                user => Ok(user),
                errors => Problem(errors));
        }
        public record UpdateUserRequest(string Name, string LastName, string Email);


        /// <summary>
        /// Elimina un usuario.
        /// </summary>
        /// <param name="id">ID del usuario a eliminar.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deleteUserResult = await _mediator.Send(new DeleteUserCommand(id));
            return deleteUserResult.Match(
                customerId => NoContent(),
                errors => Problem(errors));
        }
    }
}
