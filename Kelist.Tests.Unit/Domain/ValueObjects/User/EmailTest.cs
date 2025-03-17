using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.ValueObjects.User;

namespace Kelist.Tests.Unit.Domain.ValueObjects.User
{
    public class EmailTest
    {
        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.uk")]
        [InlineData("a@b.cd")]
        public void Create_ValidEmail_ReturnsSuccess(string validEmail)
        {
            // Act
            var result = Email.Create(validEmail);

            // Assert
            Assert.False(result.IsError);
            Assert.Equal(validEmail, result.Value.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullEmail_ReturnsValidationError(string? invalidEmail)
        {
            // Act
            var result = Email.Create(invalidEmail!);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("User.Email", result.FirstError.Code);
            Assert.Equal("Email no puede ser vacio", result.FirstError.Description);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("test@.com")]
        [InlineData("@domain.com")]
        [InlineData("test@domain")]
        public void Create_InvalidEmailFormat_ReturnsValidationError(string invalidEmail)
        {
            // Act
            var result = Email.Create(invalidEmail);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("User.Email", result.FirstError.Code);
            Assert.Equal("Estructura de Email inválida", result.FirstError.Description);
        }

        [Fact]
        public void Create_EmailTooLong_ReturnsValidationError()
        {
            // Arrange
            string longEmail = new string('a', 250) + "@example.com"; // > 255 caracteres

            // Act
            var result = Email.Create(longEmail);

            // Assert
            Assert.True(result.IsError);
            Assert.Equal("User.Email", result.FirstError.Code);
            Assert.Equal("Email no puede ser mayor a 255 caracteres", result.FirstError.Description);
        }
    }
}
