using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Users.Queries.GetById;
using ErrorOr;
using Moq;

namespace Kelist.Tests.Unit.Application.Users.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsUserResponse_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDto = new UserDTO(userId, "John", "Doe", "john.doe@example.com", []);
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(userDto);
            var query = new GetUserByIdQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            var userResponse = result.Value;
            Assert.Equal(userId, userResponse.Id);
            Assert.Equal("John Doe", userResponse.FullName);
            Assert.Equal("john.doe@example.com", userResponse.Email);
        }

        [Fact]
        public async Task Handle_ReturnsNotFoundError_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((UserDTO?)null);
            var query = new GetUserByIdQuery(userId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal(Error.NotFound("User.NotFound", "The user with the provided Id was not found."), result.FirstError);
        }
    }
}
