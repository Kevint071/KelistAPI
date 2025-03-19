using Domain.Tasks;
using Domain.ValueObjects.TaskItem;
using FluentAssertions;
using DomainTasks = Domain.Tasks;

namespace Kelist.Tests.Unit.Domain.TaskItem
{
    public class TaskItemTests
    {
        private readonly TaskItemId _taskItemId = new(Guid.NewGuid());

        [Fact]
        public void Constructor_SetsPropertiesCorrectly_WithDefaultIsCompleted()
        {
            // Arrange
            var descriptionResult = Description.Create("Buy groceries");
            var description = descriptionResult.Value;

            // Act
            var taskItem = new DomainTasks.TaskItem(_taskItemId, description);

            // Assert
            taskItem.Id.Should().Be(_taskItemId);
            taskItem.Description.Should().Be(description);
            taskItem.IsCompleted.Should().BeFalse();
        }

        [Fact]
        public void Constructor_SetsPropertiesCorrectly_WithExplicitIsCompleted()
        {
            // Arrange
            var descriptionResult = Description.Create("Finish report");
            var description = descriptionResult.Value;
            bool isCompleted = true;

            // Act
            var taskItem = new DomainTasks.TaskItem(_taskItemId, description, isCompleted);

            // Assert
            taskItem.Id.Should().Be(_taskItemId);
            taskItem.Description.Should().Be(description);
            taskItem.IsCompleted.Should().BeTrue();
        }

        [Fact]
        public void Constructor_WithNullDescription_ThrowsArgumentNullException()
        {
            // Act & Assert
            Action act = () =>
            {
                DomainTasks.TaskItem taskItem = new(_taskItemId, null!);
            };
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("description");
        }

        [Fact]
        public void Constructor_WithNullId_ThrowsArgumentNullException()
        {
            // Arrange
            var descriptionResult = Description.Create("Valid task");
            var description = descriptionResult.Value;

            // Act & Assert
            Action act = () =>
            {
                DomainTasks.TaskItem taskItem = new(null!, description);
            };
            act.Should().Throw<ArgumentNullException>()
               .WithParameterName("id");
        }
    }
}
