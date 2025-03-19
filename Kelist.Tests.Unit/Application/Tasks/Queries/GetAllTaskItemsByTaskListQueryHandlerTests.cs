using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Tasks.Queries.GetAllByTaskList;
using Application.Users.Dtos;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.Tasks.Queries
{
    public class GetAllTaskItemsByTaskListQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetAllTaskItemsByTaskListQueryHandler _handler;

        public GetAllTaskItemsByTaskListQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllTaskItemsByTaskListQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_UserAndTaskListExist_ReturnsTaskItems()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var query = new GetAllTaskItemsByTaskListQuery(userId, taskListId);
            var taskItems = new List<TaskItemDTO>
            {
                new() { Id = Guid.NewGuid(), Description = "Buy groceries", IsCompleted = false },
                new() { Id = Guid.NewGuid(), Description = "Call mom", IsCompleted = true }
            };
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists =
                [
                    new TaskListDTO
                    {
                        Id = taskListId,
                        TaskListName = "Daily",
                        TaskItems = taskItems
                    }
                ]
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeAssignableTo<IReadOnlyList<TaskItemDTO>>();
            result.Value.Should().BeEquivalentTo(taskItems);
            result.Value.Count.Should().Be(2);
            result.Value[0].Description.Should().Be("Buy groceries");
            result.Value[1].IsCompleted.Should().BeTrue();

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var query = new GetAllTaskItemsByTaskListQuery(userId, taskListId);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("User.NotFound");
            result.FirstError.Description.Should().Be("The user with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_TaskListDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var query = new GetAllTaskItemsByTaskListQuery(userId, taskListId);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists =
                [
                    new TaskListDTO { Id = Guid.NewGuid(), TaskListName = "Other" }
                ]
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("TaskList.NotFound");
            result.FirstError.Description.Should().Be("The task list with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyTaskList_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var taskListId = Guid.NewGuid();
            var query = new GetAllTaskItemsByTaskListQuery(userId, taskListId);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists =
                [
                    new TaskListDTO
                    {
                        Id = taskListId,
                        TaskListName = "Daily",
                        TaskItems = []
                    }
                ]
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeAssignableTo<IReadOnlyList<TaskItemDTO>>();
            result.Value.Should().BeEmpty();

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }
    }
}
