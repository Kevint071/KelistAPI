using Application.TaskLists.Commands.CreateTaskList;
using Application.TaskLists.Commands.DeleteTaskList;
using Application.TaskLists.Commands.UpdateTaskList;
using Application.TaskLists.Dtos;
using Application.TaskLists.Queries.GetAllByUser;
using ErrorOr;
using FluentAssertions;
using Kelist.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kelist.Tests.Unit.Controllers
{
    public class TaskListsControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly TaskListsController _controller;

        public TaskListsControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            _controller = new TaskListsController(_mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenTaskListsExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskLists = new List<TaskListDTO>
            {
                new() { Id = Guid.NewGuid(), TaskListName = "Task List 1", TaskItems = [] }
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllTaskListsByUserQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From((IReadOnlyList<TaskListDTO>)taskLists));

            // Act
            var result = await _controller.GetAll(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(taskLists);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenCommandSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateTaskListRequest("New Task List");
            var command = new CreateTaskListCommand(userId, request.Name);
            var taskListDto = new TaskListDTO
            {
                Id = Guid.NewGuid(),
                TaskListName = "New Task List",
                TaskItems = []
            };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From(taskListDto));

            // Act
            var result = await _controller.Create(userId, request);

            // Assert
            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new CreateTaskListRequest("");
            var command = new CreateTaskListCommand(userId, request.Name);
            var errors = new List<Error> { Error.Validation("TaskList.Name", "El nombre no puede estar vacío.") };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(errors);

            // Act
            var result = await _controller.Create(userId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>("El resultado debería ser un ObjectResult indicando un error");

            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ValidationProblemDetails>("El valor debería contener detalles de validación");

            var problemDetails = (ValidationProblemDetails)objectResult.Value;
            problemDetails.Errors.Should().ContainKey("TaskList.Name", "Debería contener el error de validación para TaskList.Name");
            problemDetails.Errors["TaskList.Name"].Should().Contain("El nombre no puede estar vacío.", "El mensaje de error debería coincidir");
        }

        [Fact]
        public async Task Update_ReturnsOk_WhenUpdateSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var request = new UpdateTaskListRequest("Updated Task List");
            var command = new UpdateTaskListCommand(userId, taskListId, request.Name);
            var taskListDto = new TaskListDTO
            {
                Id = taskListId,
                TaskListName = "Updated Task List",
                TaskItems = []
            };

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateTaskListCommand>(c => c.TaskListId == taskListId && c.UserId == userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(ErrorOrFactory.From(taskListDto));

            // Act
            var result = await _controller.Update(userId, taskListId, request);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(taskListDto);
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenTaskListDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var request = new UpdateTaskListRequest("Updated Task List");
            var errors = new List<Error> { Error.NotFound("TaskList.NotFound", "Task list not found") };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTaskListCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(errors);

            // Act
            var result = await _controller.Update(userId, taskListId, request);

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
            var request = new UpdateTaskListRequest("");
            var errors = new List<Error> { Error.Validation("TaskList.Name", "El nombre no puede estar vacío") };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateTaskListCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(errors);

            // Act
            var result = await _controller.Update(userId, taskListId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>("El resultado debería ser un ObjectResult indicando un error");

            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ValidationProblemDetails>("El valor debería contener detalles de validación");

            var problemDetails = (ValidationProblemDetails)objectResult.Value;
            problemDetails.Errors.Should().ContainKey("TaskList.Name", "Debería contener el error de validación para TaskList.Name");
            problemDetails.Errors["TaskList.Name"].Should().Contain("El nombre no puede estar vacío", "El mensaje de error debería coincidir");
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteTaskListCommand>(c => c.UserId == userId && c.TaskListId == taskListId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(ErrorOrFactory.From(MediatR.Unit.Value));

            // Act
            var result = await _controller.Delete(userId, taskListId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenTaskListDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var errors = new List<Error> { Error.NotFound("TaskList.NotFound", "Task list not found") };

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteTaskListCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(errors);

            // Act
            var result = await _controller.Delete(userId, taskListId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }
    }
}
