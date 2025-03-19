using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Tasks.Commands.UpdateTaskItem;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.Tasks.Commands
{
    public class UpdateTaskItemCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly UpdateTaskItemCommandHandler _handler;

        public UpdateTaskItemCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateTaskItemCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_UserTaskListAndTaskItemExist_UpdatesTaskItemAndReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, "Updated description", true);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists =
                [
                    new TaskListDTO
                    {
                        Id = taskListId,
                        TaskListName = "Shopping",
                        TaskItems =
                        [
                            new TaskItemDTO { Id = taskItemId, Description = "Old description", IsCompleted = false }
                        ]
                    }
                ]
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<TaskItemDTO>();
            result.Value.Id.Should().Be(taskItemId);
            result.Value.Description.Should().Be("Updated description");
            result.Value.IsCompleted.Should().BeTrue();

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateTaskItem(userId, taskListId, It.Is<TaskItemDTO>(ti =>
                ti.Id == taskItemId &&
                ti.Description == "Updated description" &&
                ti.IsCompleted == true)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, "Updated description", true);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("User.NotFound");
            result.FirstError.Description.Should().Be("The user with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateTaskItem(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TaskItemDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_TaskListDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, "Updated description", true);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists =
                [
                    new TaskListDTO { Id = Guid.NewGuid(), TaskListName = "Other" }
                ]
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
            _userRepositoryMock.Verify(r => r.UpdateTaskItem(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TaskItemDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_TaskItemDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var taskItemId = Guid.NewGuid();
            var command = new UpdateTaskItemCommand(userId, taskListId, taskItemId, "Updated description", true);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = new List<TaskListDTO>
                {
                    new TaskListDTO
                    {
                        Id = taskListId,
                        TaskListName = "Shopping",
                        TaskItems =
                        [
                            new TaskItemDTO { Id = Guid.NewGuid(), Description = "Other task", IsCompleted = false }
                        ]
                    }
                }
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("TaskItem.NotFound");
            result.FirstError.Description.Should().Be("The task item with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.UpdateTaskItem(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<TaskItemDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
