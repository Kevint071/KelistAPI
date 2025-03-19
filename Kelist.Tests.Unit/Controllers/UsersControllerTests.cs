using Application.Users.Commands.CreateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UpdateUser;
using Application.Users.Common;
using Application.Users.Dtos;
using Application.Users.Queries.GetAll;
using Application.Users.Queries.GetById;
using ErrorOr;
using FluentAssertions;
using Kelist.API.Controllers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Kelist.Tests.Unit.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<ISender> _mediatorMock;
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _mediatorMock = new Mock<ISender>();
            _controller = new UsersController(_mediatorMock.Object)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };
        }

        [Fact]
        public async Task GetAll_ReturnsOk_WhenUsersExist()
        {
            // Arrange
            var users = new List<UserResponse> { new(Guid.NewGuid(), "John Doe", "john.doe@example.com") };
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetAllUsersQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From((IReadOnlyList<UserResponse>)users));

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task GetById_ReturnsOk_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = new UserResponse(userId, "John Doe", "john.doe@example.com");
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From(user));

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            okResult.Value.Should().BeEquivalentTo(user);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(new List<Error> { Error.NotFound("User.NotFound", "User not found") });

            // Act
            var result = await _controller.GetById(userId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Create_ReturnsCreated_WhenCommandSucceeds()
        {
            // Arrange
            var command = new CreateUserCommand("John", "Doe", "john.doe@example.com");
            var userDto = new UserDTO(Guid.NewGuid(), "John", "Doe", "john.doe@example.com");
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(ErrorOrFactory.From(userDto));

            // Act
            var result = await _controller.Create(command);

            // Assert
            result.Should().BeOfType<CreatedResult>();
        }

        [Fact]
        public async Task Create_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var command = new CreateUserCommand("", "Doe", "john.doe@example.com");
            var errors = new List<Error> { Error.Validation("User.Name", "El nombre no puede estar vacío.") };
            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                         .ReturnsAsync(errors);

            // Act
            var result = await _controller.Create(command);

            // Assert
            result.Should().BeOfType<ObjectResult>("El resultado debería ser un ObjectResult indicando un error");

            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ValidationProblemDetails>("El valor debería contener detalles de validación");

            var problemDetails = (ValidationProblemDetails)objectResult.Value;
            problemDetails.Errors.Should().ContainKey("User.Name", "Debería contener el error de validación para User.Name");
            problemDetails.Errors["User.Name"].Should().Contain("El nombre no puede estar vacío.", "El mensaje de error debería coincidir");
        }

        [Fact]
        public async Task Update_ReturnsNoContent_WhenUpdateSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UsersController.UpdateUserRequest("John", "Doe", "john.doe@example.com");
            var command = new UpdateUserCommand(userId, request.Name, request.LastName, request.Email);
            var userDto = new UserDTO(Guid.NewGuid(), "John", "Doe", "john.doe@example.com");

            _mediatorMock.Setup(m => m.Send(It.Is<UpdateUserCommand>(c => c.Id == userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(ErrorOrFactory.From(userDto));

            // Act
            var result = await _controller.Update(userId, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
        }

        [Fact]
        public async Task Update_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UsersController.UpdateUserRequest("John", "Doe", "john.doe@example.com");
            var errors = new List<Error> { Error.NotFound("User.NotFound", "User not found") };

            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(errors);

            // Act
            var result = await _controller.Update(userId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Update_ReturnsBadRequest_WhenValidationFails()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var request = new UsersController.UpdateUserRequest("", "Doe", "invalid-email");
            var errors = new List<Error>
            {
                Error.Validation("User.Name", "El nombre no puede estar vacío"),
                Error.Validation("User.Email", "El email debe ser válido")
            };
            _mediatorMock.Setup(m => m.Send(It.IsAny<UpdateUserCommand>(), It.IsAny<CancellationToken>()))
                         .ReturnsAsync(errors);

            // Act
            var result = await _controller.Update(userId, request);

            // Assert
            result.Should().BeOfType<ObjectResult>("El resultado debería ser un ObjectResult indicando un error");

            var objectResult = (ObjectResult)result;
            objectResult.Value.Should().BeOfType<ValidationProblemDetails>("El valor debería contener detalles de validación");

            var problemDetails = (ValidationProblemDetails)objectResult.Value;
            problemDetails.Errors.Should().ContainKey("User.Name", "Debería contener el error de validación para User.Name");
            problemDetails.Errors["User.Name"].Should().Contain("El nombre no puede estar vacío", "El mensaje de error para Name debería coincidir");

            problemDetails.Errors.Should().ContainKey("User.Email", "Debería contener el error de validación para User.Email");
            problemDetails.Errors["User.Email"].Should().Contain("El email debe ser válido", "El mensaje de error para Email debería coincidir");
        }

        [Fact]
        public async Task Delete_ReturnsNoContent_WhenDeleteSucceeds()
        {
            // Arrange
            var userId = Guid.NewGuid();
            _mediatorMock.Setup(m => m.Send(It.Is<DeleteUserCommand>(c => c.Id == userId), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(ErrorOrFactory.From(MediatR.Unit.Value));

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var errors = new List<Error> { Error.NotFound("User.NotFound", "User not found") };

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                        .ReturnsAsync(errors);

            // Act
            var result = await _controller.Delete(userId);

            // Assert
            result.Should().BeOfType<ObjectResult>()
                  .Which.StatusCode.Should().Be(404);
        }
    }
}
