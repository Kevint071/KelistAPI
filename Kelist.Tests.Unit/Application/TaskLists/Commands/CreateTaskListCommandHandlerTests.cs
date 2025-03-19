using Application.Data.Interfaces;
using Application.Data.Repositories;
using Application.TaskLists.Commands.CreateTaskList;
using Application.TaskLists.Dtos;
using Application.Users.Dtos;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.TaskLists.Commands
{
    public class CreateTaskListCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly CreateTaskListCommandHandler _handler;

        public CreateTaskListCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new CreateTaskListCommandHandler(_userRepositoryMock.Object, _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_UserExists_CreatesTaskListAndReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTaskListCommand(userId, "Daily Tasks");
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = []
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<TaskListDTO>();
            result.Value.Id.Should().NotBeEmpty();
            result.Value.TaskListName.Should().Be("Daily Tasks");
            result.Value.TaskItems.Should().BeEmpty();

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskListToUser(userId, It.Is<TaskListDTO>(tl =>
                tl.TaskListName == "Daily Tasks" &&
                tl.TaskItems.Count == 0)), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTaskListCommand(userId, "Daily Tasks");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("User.NotFound");
            result.FirstError.Description.Should().Be("The user with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskListToUser(It.IsAny<Guid>(), It.IsAny<TaskListDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyName_ReturnsValidationError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new CreateTaskListCommand(userId, "");
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = []
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
            result.FirstError.Code.Should().Be("TaskList.Name");
            result.FirstError.Description.Should().Contain("El nombre no puede estar vacío.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskListToUser(It.IsAny<Guid>(), It.IsAny<TaskListDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_NameTooLong_ReturnsValidationError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var longName = new string('A', 101); // 101 caracteres, excede el máximo de 100
            var command = new CreateTaskListCommand(userId, longName);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com")
            {
                TaskLists = []
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.Validation);
            result.FirstError.Code.Should().Be("TaskList.Name");
            result.FirstError.Description.Should().Contain("El nombre debe tener menos de 100 caracteres.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userRepositoryMock.Verify(r => r.AddTaskListToUser(It.IsAny<Guid>(), It.IsAny<TaskListDTO>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
