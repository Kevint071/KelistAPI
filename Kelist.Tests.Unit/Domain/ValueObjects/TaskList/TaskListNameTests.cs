using Domain.ValueObjects.TaskList;

namespace Kelist.Tests.Unit.Domain.ValueObjects.TaskList
{
    public class TaskListNameTests
    {
        [Theory]
        [InlineData(" My list ", "My list")]
        [InlineData("  Work   Tasks    ", "Work Tasks")]
        [InlineData("      Home ", "Home")]
        public void Create_ValidName_ReturnsSuccessWithNormalizedValue(string input, string expected)
        {
            var result = TaskListName.Create(input);

            Assert.False(result.IsError);
            Assert.Equal(expected, result.Value.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullName_ReturnsValidationError(string? invalidInput)
        {
            var result = TaskListName.Create(invalidInput!);

            Assert.True(result.IsError);
            Assert.Equal("TaskList.Name", result.FirstError.Code);
            Assert.Equal("El nombre no puede estar vacío.", result.FirstError.Description);
        }

        [Fact]
        public void Create_NameTooLong_ReturnsValidationError()
        {
            string longName = new('a', 101);

            var result = TaskListName.Create(longName);

            Assert.True(result.IsError);
            Assert.Equal("TaskList.Name", result.FirstError.Code);
            Assert.Equal("El nombre debe tener menos de 100 caracteres.", result.FirstError.Description);
        }
    }
}
