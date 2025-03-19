using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Tasks.Commands.CreateTaskItem;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.Tasks.Commands
{
    public class CreateTaskItemCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateTaskItemCommandHandler _handler;

        public CreateTaskItemCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateTaskItemCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_UserAndTaskListExist_CreatesTaskItemAndReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var command = new CreateTaskItemCommand(userId, taskListId, "Buy groceries");
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = new List<TaskListDTO> { new TaskListDTO { Id = taskListId, TaskListName = "Shopping" } }
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<TaskItemDTO>();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.Description.Should().Be("Buy groceries");
            result.Value.IsCompleted.Should().BeFalse();

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskItemToTaskList(userId, taskListId, It.Is<TaskItemDTO>(ti =>
                ti.Description == "Buy groceries" && ti.IsCompleted == false)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var command = new CreateTaskItemCommand(userId, taskListId, "Buy groceries");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("User.NotFound");
            result.FirstError.Description.Should().Be("The user with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskItemToTaskList(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TaskItemDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_TaskListDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var command = new CreateTaskItemCommand(userId, taskListId, "Buy groceries");
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = new List<TaskListDTO> { new TaskListDTO { Id = Guid.NewGuid(), TaskListName = "Other" } }
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("TaskList.NotFound");
            result.FirstError.Description.Should().Be("The task list with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskItemToTaskList(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TaskItemDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
