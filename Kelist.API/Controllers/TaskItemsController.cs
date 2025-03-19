using Application.Tasks.Commands.CreateTaskItem;
using Application.Tasks.Commands.DeleteTaskItem;
using Application.Tasks.Commands.UpdateTaskItem;
using Application.Tasks.Dtos;
using Application.Tasks.Queries.GetAllByTaskList;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("users/{userId}/tasklists/{taskListId}/taskitems")]
    [Produces("application/json")]
    public class TaskItemsController(ISender mediator) : ApiController
    {
        private readonly ISender _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));

        /// <summary>
        /// Obtiene todos los ítems de una lista de tareas.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="taskListId">ID de la lista de tareas.</param>
        /// <returns>Una lista de ítems de tareas.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<TaskItemDTO>), StatusCodes.Status200OK)] // Ajusta TaskItemDto según tu DTO real
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAll(Guid userId, Guid taskListId)
        {
            var taskItemsResult = await _mediator.Send(new GetAllTaskItemsByTaskListQuery(userId, taskListId));

            return taskItemsResult.Match(
                taskItems => Ok(taskItems),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Crea un nuevo ítem en una lista de tareas.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="taskListId">ID de la lista de tareas.</param>
        /// <param name="request">Datos del ítem de tarea a crear.</param>
        /// <returns>El ítem de tarea creado.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status201Created)] // Ajusta TaskItemDto según tu DTO real
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(Guid userId, Guid taskListId, [FromBody] CreateTaskItemRequest request)
        {
            var command = new CreateTaskItemCommand(userId, taskListId, request.Description);
            var createResult = await _mediator.Send(command);

            return createResult.Match(
                taskItem => Created($"/users/{userId}/tasklists/{taskListId}/taskitems/{taskItem.Id}", taskItem),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Actualiza un ítem de tarea existente.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="taskListId">ID de la lista de tareas.</param>
        /// <param name="taskItemId">ID del ítem de tarea a actualizar.</param>
        /// <param name="request">Datos actualizados del ítem de tarea.</param>
        [HttpPut("{taskItemId}")]
        [ProducesResponseType(typeof(TaskItemDTO), StatusCodes.Status200OK)] // Ajusta TaskItemDto según tu DTO real
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid userId, Guid taskListId, Guid taskItemId, [FromBody] UpdateTaskItemRequest request)
        {
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, request.Description, request.IsCompleted);
            var updateResult = await _mediator.Send(command);
            return updateResult.Match(
                taskItem => Ok(taskItem),
                errors => Problem(errors)
            );
        }

        /// <summary>
        /// Elimina un ítem de tarea.
        /// </summary>
        /// <param name="userId">ID del usuario.</param>
        /// <param name="taskListId">ID de la lista de tareas.</param>
        /// <param name="taskItemId">ID del ítem de tarea a eliminar.</param>
        [HttpDelete("{taskItemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid userId, Guid taskListId, Guid taskItemId)
        {
            var command = new DeleteTaskItemCommand(userId, taskListId, taskItemId);
            var deleteResult = await _mediator.Send(command);
            return deleteResult.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }
    }
    public record CreateTaskItemRequest(string Description);
    public record UpdateTaskItemRequest(string Description, bool IsCompleted);
}
