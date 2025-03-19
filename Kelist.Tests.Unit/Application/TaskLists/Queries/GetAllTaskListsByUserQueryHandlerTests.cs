using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.TaskLists.Queries.GetAllByUser;
using Application.Users.Dtos;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.TaskLists.Queries
{
    public class GetAllTaskListsByUserQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetAllTaskListsByUserQueryHandler _handler;

        public GetAllTaskListsByUserQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllTaskListsByUserQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ValidQuery_UserExistsWithTaskLists_ReturnsTaskLists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetAllTaskListsByUserQuery(userId);
            var taskLists = new List<TaskListDTO>
            {
                new() { Id = Guid.NewGuid(), TaskListName = "Daily Tasks", TaskItems = [] },
                new() { Id = Guid.NewGuid(), TaskListName = "Work Tasks", TaskItems = [] }
            };
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = taskLists
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<List<TaskListDTO>>();
            result.Value.Should().BeEquivalentTo(taskLists);
            result.Value.Count.Should().Be(2);
            result.Value[0].TaskListName.Should().Be("Daily Tasks");
            result.Value[1].TaskListName.Should().Be("Work Tasks");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_ValidQuery_UserExistsWithNoTaskLists_ReturnsEmptyList()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetAllTaskListsByUserQuery(userId);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = []
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<List<TaskListDTO>>();
            result.Value.Should().BeEmpty();

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetAllTaskListsByUserQuery(userId);

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
    }
}
