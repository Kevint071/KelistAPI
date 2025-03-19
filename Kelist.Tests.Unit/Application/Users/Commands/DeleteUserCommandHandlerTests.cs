using Application.Data.Repositories;
using Application.Users.Commands.DeleteUser;
using Application.Users.Dtos;
using Application.Users.Services;
using ErrorOr;
using FluentAssertions;
using Moq;

namespace Kelist.Tests.Unit.Application.Users.Commands
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUserService> _userServiceMock;
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _userServiceMock = new Mock<IUserService>();
            _handler = new DeleteUserCommandHandler(_userRepositoryMock.Object, _userServiceMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_UserExists_DeletesUserAndReturnsUnit()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand(userId);
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com");

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);
            _userServiceMock.Setup(s => s.DeleteAsync(userDto, It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Should().Be(MediatR.Unit.Value);

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userServiceMock.Verify(s => s.DeleteAsync(userDto, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_UserDoesNotExist_ReturnsNotFoundError()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand(userId);

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserDTO?)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);
            result.FirstError.Code.Should().Be("User.NotFound");
            result.FirstError.Description.Should().Be("The user with the provided Id was not found.");

            _userRepositoryMock.Verify(r => r.GetByIdAsync(userId), Times.Once);
            _userServiceMock.Verify(s => s.DeleteAsync(It.IsAny<UserDTO>(), It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
