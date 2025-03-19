using Domain.TaskLists;
using Domain.ValueObjects.TaskList;

namespace Kelist.Tests.Unit.Domain.TaskLists
{
    public class TaskListTests
    {
        private readonly TaskListId _taskListId = new(Guid.NewGuid());
        private readonly TaskListName _name = TaskListName.Create("My List").Value;

        [Fact]
        public void Constructor_SetsPropertiesCorrectly()
        {
            var taskList = new TaskList(_taskListId, _name);

            Assert.Equal(_taskListId, taskList.Id);
            Assert.Equal(_name, taskList.TaskListName);
            Assert.Empty(taskList.TaskItems);
        }
    }
}
