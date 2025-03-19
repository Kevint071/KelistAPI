using Domain.ValueObjects.TaskItem;
using FluentAssertions;

namespace Kelist.Tests.Unit.Domain.ValueObjects.TaskItem
{
    public class DescriptionTests
    {
        [Theory]
        [InlineData("Buy groceries")]
        [InlineData("Finish report")]
        [InlineData("A single word")]
        public void Create_ValidDescription_ReturnsSuccess(string validInput)
        {
            // Act
            var result = Description.Create(validInput);

            // Assert
            result.IsError.Should().BeFalse();
            result.Value.Value.Should().Be(validInput);
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        [InlineData(null)]
        public void Create_EmptyOrNullDescription_ReturnsValidationError(string? invalidInput)
        {
            // Act
            var result = Description.Create(invalidInput!);

            // Assert
            result.IsError.Should().BeTrue();
            result.FirstError.Code.Should().Be("TaskItem.Description");
            result.FirstError.Description.Should().Be("La descripción no puede estar vacía.");
        }
    }
}
