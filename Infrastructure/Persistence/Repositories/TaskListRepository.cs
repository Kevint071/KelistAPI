using Application.Data.Repositories;
using Application.Tasks.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class TaskListRepository : ITaskListRepository
    {
        private readonly ApplicationDbContext _context;

        public TaskListRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(TaskListDTO taskListDTO) => _context.TaskLists.Add(taskListDTO);
        public async Task<TaskListDTO?> GetByIdAsync(Guid id) => await _context.TaskLists.Include(t => t.TaskItems).SingleOrDefaultAsync(t => t.Id == id);
    }
}