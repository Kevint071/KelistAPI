using Application.Tasks.Commands.CreateTaskItem;
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
    }
}
