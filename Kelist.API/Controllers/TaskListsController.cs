using Application.TaskLists.Commands.CreateTaskList;
using Application.TaskLists.Commands.DeleteTaskList;
using Application.TaskLists.Commands.UpdateTaskList;
using Application.TaskLists.Dtos;
using Application.TaskLists.Queries.GetAllByUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("users/{userId}/tasklists")]
    [Produces("application/json")]
    [Authorize]
    public class TaskListsController(ISender mediator) : ApiController
    {
        private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        //// <summary>
        /// Obtiene todas las listas de tareas de un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <returns>Una lista de listas de tareas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TaskListDTO>), StatusCodes.Status200OK)] // Ajusta TaskListDto según tu DTO real
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            if (!IsAuthorizedForUser(userId)) return Forbid();
            var taskListsResult = await _mediator.Send(new GetAllTaskListsByUserQuery(userId));

            return taskListsResult.Match(
                taskLists => Ok(taskLists),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Crea una nueva lista de tareas para un usuario.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="request">Datos de la lista de tareas a crear.</param>
        /// <returns>La lista de tareas creada.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskListDTO), StatusCodes.Status201Created)] // Ajusta TaskListDto según tu DTO real
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(Guid userId, [FromBody] CreateTaskListRequest request)
        {
            if (!IsAuthorizedForUser(userId)) return Forbid();
            var command = new CreateTaskListCommand(userId, request.Name);
            var createResult = await _mediator.Send(command);

            return createResult.Match(
                taskList => Created($"/users/{userId}/tasklists/{taskList.Id}", taskList),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Actualiza una lista de tareas existente.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="taskListId">ID de la lista de tareas a actualizar.</param>
        /// <param name="request">Datos actualizados de la lista de tareas.</param>
        [HttpPut("{taskListId}")]
        [ProducesResponseType(typeof(TaskListDTO), StatusCodes.Status200OK)] // Ajusta TaskListDto según tu DTO real
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid userId, Guid taskListId, [FromBody] UpdateTaskListRequest request)
        {
            if (!IsAuthorizedForUser(userId)) return Forbid();
            var command = new UpdateTaskListCommand(userId, taskListId, request.Name);
            var updateResult = await _mediator.Send(command);
            return updateResult.Match(
                taskList => Ok(taskList),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Elimina una lista de tareas.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="taskListId">ID de la lista de tareas a eliminar.</param>
        [HttpDelete("{taskListId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid userId, Guid taskListId)
        {
            if (!IsAuthorizedForUser(userId)) return Forbid();
            var command = new DeleteTaskListCommand(userId, taskListId);
            var deleteResult = await _mediator.Send(command);
            return deleteResult.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }
    }

    public record CreateTaskListRequest(string Name);
    public record UpdateTaskListRequest(string Name);
}
