using Application.Users.Commands.CreateUser;
using Application.Users.Dtos;
using Application.Users.Services;
using Domain.Users;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.Users.Commands
{
    public class CreateUserCommandHandlerTests
    {
        private readonly Mock<IUserService> _userServiceMock; // Cambiado a IUserService
        private readonly CreateUserCommandHandler _handler;

        public CreateUserCommandHandlerTests()
        {
            _userServiceMock = new Mock<IUserService>();
            _handler = new CreateUserCommandHandler(_userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CreatesUserAndReturnsSuccess()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");
            var userDto = new UserDTO(Guid.NewGuid(), "John", "Doe", "john.doe@example.com");
            _userServiceMock.Setup(s => s.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
                           .ReturnsAsync(userDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().BeOfType<UserDTO>();
            _userServiceMock.Verify(s => s.AddAsync(It.Is<User>(u =>
                u.PersonName.Value == "John" &&
                u.LastName.Value == "Doe" &&
                u.Email.Value == "john.doe@example.com"), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_InvalidEmail_ReturnsValidationError()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "invalid-email");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("User.Email");
            result.FirstError.Description.Should().Be("Estructura de Email inválida");
            _userServiceMock.Verify(s => s.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyName_ReturnsValidationError()
        {
            // Arrange
            var command = new CreateUserCommand("", "Doe", "john.doe@example.com");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("User.Name");
            result.FirstError.Description.Should().Be("El nombre no puede estar vacío.");
            _userServiceMock.Verify(s => s.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_EmptyLastName_ReturnsValidationError()
        {
            // Arrange
            var command = new CreateUserCommand("John", "", "john.doe@example.com");

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("User.LastName");
            result.FirstError.Description.Should().Be("El apellido no puede estar vacío.");
            _userServiceMock.Verify(s => s.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
