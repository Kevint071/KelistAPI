using Application.Data.Repositories;
using Application.Users.Dtos;
using Application.Users.Queries.GetAll;
using Moq;

namespace Kelist.Tests.Unit.Application.Users.Queries
{
    public class GetAllUsersQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly GetAllUsersQueryHandler _handler;

        public GetAllUsersQueryHandlerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _handler = new GetAllUsersQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_ReturnsListOfUserResponses_WhenUsersExist()
        {
            // Arrange
            var userDtos = new List<UserDTO>
            {
                new(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", []),
                new(Guid.NewGuid(), "Jane", "Smith", "jane.smith@example.com", [])
            };
            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(userDtos);
            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            var userResponses = result.Value;
            Assert.Equal(2, userResponses.Count);
            Assert.Contains(userResponses, ur => ur.FullName == "John Doe" && ur.Email == "john.doe@example.com");
            Assert.Contains(userResponses, ur => ur.FullName == "Jane Smith" && ur.Email == "jane.smith@example.com");
        }

        [Fact]
        public async Task Handle_ReturnsEmptyList_WhenNoUsersExist()
        {
            // Arrange
            _userRepositoryMock.Setup(r => r.GetAll()).ReturnsAsync(new List<UserDTO>());
            var query = new GetAllUsersQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsError);
            var userResponses = result.Value;
            Assert.Empty(userResponses);
        }
    }
}
