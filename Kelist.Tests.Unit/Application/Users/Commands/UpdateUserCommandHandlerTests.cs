using Application.Data.Repositories;
using Application.Users.Commands.UpdateUser;
using Application.Users.Dtos;
using Application.Users.Services;
using Domain.Users;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.Users.Commands
{
    public class UpdateUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly UpdateUserCommandHandler _handler;

        public UpdateUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userServiceMock = new Mock<IUserService>();
            _handler = new UpdateUserCommandHandler(_userRepositoryMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_UserExists_UpdatesUserAndReturnsSuccess()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "Jane", "Smith", "jane.smith@example.com");
            var userDto = new UserDTO(userId, "Jane", "Smith", "jane.smith@example.com");

            _userRepositoryMock.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);
            _userServiceMock.Setup(s => s.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<UserDTO>();
            result.Value.Id.Should().Be(userId);
            result.Value.PersonName.Should().Be("Jane");
            result.Value.LastName.Should().Be("Smith");
            result.Value.Email.Should().Be("jane.smith@example.com");

            _userServiceMock.Verify(s => s.UpdateAsync(It.Is<User>(u =>
                u.Id.Value == userId &&
                u.PersonName.Value == "Jane" &&
                u.LastName.Value == "Smith" &&
                u.Email.Value == "jane.smith@example.com"), It.IsAny<CancellationToken>()), Times.Once);
            _userRepositoryMock.Verify(r => r.ExistsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "Jane", "Smith", "jane.smith@example.com");

            _userRepositoryMock.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(false);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("User.NotFound");
            result.FirstError.Description.Should().Be("The user with the provided Id was not found.");
            _userServiceMock.Verify(s => s.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _userRepositoryMock.Verify(r => r.ExistsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ReturnsValidationError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "Jane", "Smith", "invalid-email");

            _userRepositoryMock.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("User.Email");
            result.FirstError.Description.Should().Be("Estructura de Email inválida");
            _userServiceMock.Verify(s => s.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _userRepositoryMock.Verify(r => r.ExistsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyName_ReturnsValidationError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "", "Smith", "jane.smith@example.com");

            _userRepositoryMock.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("User.Name");
            result.FirstError.Description.Should().Be("El nombre no puede estar vacío.");
            _userServiceMock.Verify(s => s.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _userRepositoryMock.Verify(r => r.ExistsAsync(userId), Times.Once);
        }

        [Fact]
        public async Task Handle_EmptyLastName_ReturnsValidationError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserCommand(userId, "Jane", "", "jane.smith@example.com");

            _userRepositoryMock.Setup(r => r.ExistsAsync(userId)).ReturnsAsync(true);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("User.LastName");
            result.FirstError.Description.Should().Be("El apellido no puede estar vacío.");
            _userServiceMock.Verify(s => s.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
            _userRepositoryMock.Verify(r => r.ExistsAsync(userId), Times.Once);
        }
    }
}
