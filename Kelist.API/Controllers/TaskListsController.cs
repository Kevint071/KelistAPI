using Application.TaskLists.Commands.CreateTaskList;
using Application.TaskLists.Commands.DeleteTaskList;
using Application.TaskLists.Commands.UpdateTaskList;
using Application.TaskLists.Queries.GetAllByUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Kelist.API.Controllers
{
    [Route("users/{userId}/tasklists")]
    public class TaskListsController : ApiController
    {
        private readonly ISender _mediator;

        public TaskListsController(ISender mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(Guid userId)
        {
            var taskListsResult = await _mediator.Send(new GetAllTaskListsByUserQuery(userId));

            return taskListsResult.Match(
                taskLists => Ok(taskLists),
                errors => Problem(errors)
            );
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid userId, [FromBody] CreateTaskListRequest request)
        {
            var command = new CreateTaskListCommand(userId, request.Name);
            var createResult = await _mediator.Send(command);

            return createResult.Match(
                _ => Created(),
                errors => Problem(errors)
            );
        }

        [HttpPut("{taskListId}")]
        public async Task<IActionResult> Update(Guid userId, Guid taskListId, [FromBody] UpdateTaskListRequest request)
        {
            var command = new UpdateTaskListCommand(userId, taskListId, request.Name);
            var updateResult = await _mediator.Send(command);
            return updateResult.Match(
                _ => NoContent(),
                errors => Problem(errors)
            );
        }

        [HttpDelete("{taskListId}")]
        public async Task<IActionResult> Delete(Guid userId, Guid taskListId)
        {
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
