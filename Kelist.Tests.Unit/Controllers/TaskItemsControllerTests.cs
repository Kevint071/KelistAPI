using Application.Tasks.Commands.CreateTaskItem;
using Application.Tasks.Commands.DeleteTaskItem;
using Application.Tasks.Commands.UpdateTaskItem;
using Application.Tasks.Dtos;
using Application.Tasks.Queries.GetAllByTaskList;
using ErrorOr;
using FluentAssertions;
using Kelist.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kelist.Tests.Unit.Controllers
{
    public class TaskItemsControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly TaskItemsController _controller;

        public TaskItemsControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            _controller = new TaskItemsController(_mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenTaskItemsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItems = new List<TaskItemDTO>
            {
                new() { Id = Guid.NewGuid(), Description = "Task 1", IsCompleted = false }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllTaskItemsByTaskListQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From((IReadOnlyList<TaskItemDTO>)taskItems));

            // Act
            var result = await _controller.GetAll(userId, taskListId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(taskItems);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenCommandSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var request = new CreateTaskItemRequest("New Task");
            var command = new CreateTaskItemCommand(userId, taskListId, request.Description);
            var taskItemDto = new TaskItemDTO
            {
                Id = Guid.NewGuid(),
                Description = "New Task",
                IsCompleted = false
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From(taskItemDto));

            // Act
            var result = await _controller.Create(userId, taskListId, request);

            // Assert
            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var request = new CreateTaskItemRequest("");
            var command = new CreateTaskItemCommand(userId, taskListId, request.Description);
            var errors = new List<Error> { Error.Validation("TaskItem.Description", "La descripción no puede estar vacía.") };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(errors);

            // Act
            var result = await _controller.Create(userId, taskListId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>("El resultado debería ser un ObjectResult indicando un error");

            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ValidationProblemDetails>("El valor debería contener detalles de validación");

            var problemDetails = (ValidationProblemDetails)objectResult.Value;
            problemDetails.Errors.Should().ContainKey("TaskItem.Description", "Debería contener el error de validación para TaskItem.Description");
            problemDetails.Errors["TaskItem.Description"].Should().Contain("La descripción no puede estar vacía.", "El mensaje de error debería coincidir");
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var request = new UpdateTaskItemRequest("Updated Task", true);
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, request.Description, request.IsCompleted);
            var taskItemDto = new TaskItemDTO
            {
                Id = taskItemId,
                Description = "Updated Task",
                IsCompleted = true
            };

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateTaskItemCommand>(c => c.TaskItemId == taskItemId && c.TaskListId == taskListId && c.UserId == userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(ErrorOrFactory.From(taskItemDto));

            // Act
            var result = await _controller.Update(userId, taskListId, taskItemId, request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(taskItemDto);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTaskItemDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var request = new UpdateTaskItemRequest("Updated Task", true);
            var errors = new List<Error> { Error.NotFound("TaskItem.NotFound", "Task item not found") };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTaskItemCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(errors);

            // Act
            var result = await _controller.Update(userId, taskListId, taskItemId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var request = new UpdateTaskItemRequest("", false);
            var errors = new List<Error> { Error.Validation("TaskItem.Description", "La descripción no puede estar vacía") };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTaskItemCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(errors);

            // Act
            var result = await _controller.Update(userId, taskListId, taskItemId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>("El resultado debería ser un ObjectResult indicando un error");

            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ValidationProblemDetails>("El valor debería contener detalles de validación");

            var problemDetails = (ValidationProblemDetails)objectResult.Value;
            problemDetails.Errors.Should().ContainKey("TaskItem.Description", "Debería contener el error de validación para TaskItem.Description");
            problemDetails.Errors["TaskItem.Description"].Should().Contain("La descripción no puede estar vacía", "El mensaje de error debería coincidir");
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteTaskItemCommand>(c => c.UserId == userId && c.TaskListId == taskListId && c.TaskItemId == taskItemId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(ErrorOrFactory.From(MediatR.Unit.Value));

            // Act
            var result = await _controller.Delete(userId, taskListId, taskItemId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTaskItemDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var errors = new List<Error> { Error.NotFound("TaskItem.NotFound", "Task item not found") };

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteTaskItemCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(errors);

            // Act
            var result = await _controller.Delete(userId, taskListId, taskItemId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }
    }
}
