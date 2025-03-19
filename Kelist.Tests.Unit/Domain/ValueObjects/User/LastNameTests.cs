using Domain.ValueObjects.User;

namespace Kelist.Tests.Unit.Domain.ValueObjects.User
{
    public class LastNameTests
    {
        [Theory]
        [InlineData("Doe ", "Doe")]
        [InlineData(" smith jones", "Smith Jones")]
        [InlineData(" DE     LA    CRUZ ", "De La Cruz")]
        public void Create_ValidLastName_ReturnsSuccessWithNormalizedValue(string input, string expected)
        {
            var result = LastName.Create(input);

            Assert.False(result.IsError);
            Assert.Equal(expected, result.Value.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullLastName_ReturnsValidationError(string? invalidInput)
        {
            var result = LastName.Create(invalidInput!);

            Assert.True(result.IsError);
            Assert.Equal("User.LastName", result.FirstError.Code);
            Assert.Equal("El apellido no puede estar vacío.", result.FirstError.Description);
        }

        [Fact]
        public void Create_LastNameTooLong_ReturnsValidationError()
        {
            string longLastName = new('a', 51);
            var result = LastName.Create(longLastName);

            Assert.True(result.IsError);
            Assert.Equal("User.LastName", result.FirstError.Code);
            Assert.Equal("El apellido debe tener menos de 50 caracteres.", result.FirstError.Description);
        }

        [Theory]
        [InlineData("Doe123")]
        [InlineData("Smith!")]
        [InlineData("Jones@Doe")]
        public void Create_LastNameWithNonLetters_ReturnsValidationError(string invalidInput)
        {
            var result = LastName.Create(invalidInput);

            Assert.True(result.IsError);
            Assert.Equal("User.LastName", result.FirstError.Code);
            Assert.Equal("El apellido solo puede contener letras.", result.FirstError.Description);
        }
    }
}
