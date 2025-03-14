using Application.Tasks.Commands.CreateTaskItem;
using Application.Tasks.Commands.DeleteTaskItem;
using Application.Tasks.Commands.UpdateTaskItem;
using Application.Tasks.Queries.GetAllByTaskListId;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("users/{userId}/tasklists/{taskListId}/taskitems")]
    public class TaskItemsController : ApiController
    {
        private readonly ISender _mediator;

        public TaskItemsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid userId, Guid taskListId)
        {
            var taskItemsResult = await _mediator.Send(new GetAllTaskItemsByTaskListQuery(userId, taskListId));

            return taskItemsResult.Match(
                taskItems => Ok(taskItems),
                errors => Problem(errors)
            );
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid userId, Guid taskListId, [FromBody] CreateTaskItemRequest request)
        {
            var command = new CreateTaskItemCommand(userId, taskListId, request.Description, request.IsCompleted);
            var createResult = await _mediator.Send(command);

            return createResult.Match(
                _ => Created(),
                errors => Problem(errors)
            );
        }

        public record CreateTaskItemRequest(string Description, bool IsCompleted);

        [HttpPut("{taskItemId}")]
        public async Task<IActionResult> Update(Guid userId, Guid taskListId, Guid taskItemId, [FromBody] UpdateTaskItemRequest request)
        {
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, request.Description, request.IsCompleted);
            var updateResult = await _mediator.Send(command);
            return updateResult.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }
        public record UpdateTaskItemRequest(string Description, bool IsCompleted);

        [HttpDelete("{taskItemId}")]
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
}
