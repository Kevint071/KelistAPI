using Moq;
using Application.Users.Services;
using Application.Data.Repositories;
using Application.Data.Interfaces;
using Application.Common;
using Application.Users.Dtos;
using DomainUsers = Domain.Users;
using Domain.ValueObjects.User;

namespace Kelist.Tests.Unit.Application.Users.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<IDomainEventPublisher> _domainEventPublisherMock;
        private readonly UserService _userService;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _domainEventPublisherMock = new Mock<IDomainEventPublisher>();
            _userService = new UserService(_userRepositoryMock.Object, _unitOfWorkMock.Object, _domainEventPublisherMock.Object);
        }

        [Fact]
        public async Task AddAsync_AddsUserToRepository()
        {
            // Arrange
            var user = new DomainUsers.User(
                new DomainUsers.UserId(Guid.NewGuid()),
                PersonName.Create("John").Value,
                LastName.Create("Doe").Value,
                Email.Create("john.doe@example.com").Value
            );
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userService.AddAsync(user, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(r => r.Add(It.Is<UserDTO>(dto =>
                dto.Id == user.Id.Value &&
                dto.PersonName == "John" &&
                dto.LastName == "Doe" &&
                dto.Email == "john.doe@example.com")), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesUserInRepository()
        {
            // Arrange
            var user = new DomainUsers.User(
                new DomainUsers.UserId(Guid.NewGuid()),
                PersonName.Create("John").Value,
                LastName.Create("Doe").Value,
                Email.Create("john.doe@example.com").Value
            );
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userService.UpdateAsync(user, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(r => r.Update(It.Is<UserDTO>(dto =>
                dto.Id == user.Id.Value &&
                dto.PersonName == "John" &&
                dto.LastName == "Doe" &&
                dto.Email == "john.doe@example.com")), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_DeletesUserFromRepository()
        {
            // Arrange
            var userDto = new UserDTO(Guid.NewGuid(), "John", "Doe", "john.doe@example.com", []);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

            // Act
            await _userService.DeleteAsync(userDto, CancellationToken.None);

            // Assert
            _userRepositoryMock.Verify(r => r.Delete(userDto), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(CancellationToken.None), Times.Once);
        }
    }
}