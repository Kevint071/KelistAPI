using Domain.ValueObjects.User;

namespace Kelist.Tests.Unit.Domain.ValueObjects.User
{
    public class PersonNameTests
    {
        [Theory]
        [InlineData("John", "John")]
        [InlineData("mary jane ", "Mary Jane")]
        [InlineData("        ANA         MARIA         ", "Ana Maria")]
        public void Create_ValidName_ReturnsSuccessWithNormalizedValue(string input, string expected)
        {
            var result = PersonName.Create(input);

            Assert.False(result.IsError);
            Assert.Equal(expected, result.Value.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullName_ReturnsValidationError(string? invalidInput)
        {
            var result = PersonName.Create(invalidInput!);

            Assert.True(result.IsError);
            Assert.Equal("User.Name", result.FirstError.Code);
            Assert.Equal("El nombre no puede estar vacío.", result.FirstError.Description);
        }

        [Fact]
        public void Create_NameTooLong_ReturnsValidationError()
        {
            string longName = new('a', 51);
            var result = PersonName.Create(longName);

            Assert.True(result.IsError);
            Assert.Equal("User.Name", result.FirstError.Code);
            Assert.Equal("El nombre debe tener menos de 50 caracteres.", result.FirstError.Description);
        }

        [Theory]
        [InlineData("John123")]
        [InlineData("Mary!")]
        [InlineData("Jane@Doe")]
        public void Create_NameWithNonLetters_ReturnsValidationError(string invalidInput)
        {
            var result = PersonName.Create(invalidInput);

            Assert.True(result.IsError);
            Assert.Equal("User.Name", result.FirstError.Code);
            Assert.Equal("El nombre solo puede contener letras.", result.FirstError.Description);
        }
    }
}