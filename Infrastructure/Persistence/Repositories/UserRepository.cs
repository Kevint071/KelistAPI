using Application.Data.Repositories;
using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void Add(UserDTO userDto) => _context.Users.Add(userDto);
        public void Update(UserDTO useDto) => _context.Users.Update(useDto);
        public void Delete(UserDTO userDto) => _context.Users.Remove(userDto);
        public async Task<List<UserDTO>> GetAll() => await _context.Users.ToListAsync();
        public async Task<bool> ExistsAsync(Guid id) => await _context.Users.AnyAsync(c => c.Id == id);
        public async Task<UserDTO?> GetByIdAsync(Guid id) => await _context.Users.Include(x => x.TaskLists).ThenInclude(tl => tl.TaskItems).SingleOrDefaultAsync(c => c.Id == id);

        public void AddTaskListToUser(Guid userId, TaskListDTO taskListDto)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);

            if (userDto == null) return;
            userDto.TaskLists.Add(taskListDto);

            _context.Entry(taskListDto).State = EntityState.Added;
        }

        public void UpdateTaskList(Guid userId, TaskListDTO taskListDto)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);
            if (userDto == null) return;

            var existingTaskList = userDto.TaskLists.FirstOrDefault(tl => tl.Id == taskListDto.Id);
            if (existingTaskList == null) return;

            existingTaskList.TaskListName = taskListDto.TaskListName;
            _context.Entry(existingTaskList).State = EntityState.Modified;
        }
        public void DeleteTaskList(Guid userId, Guid taskListId)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);
            if (userDto == null) return;

            var taskListToRemove = userDto.TaskLists.FirstOrDefault(tl => tl.Id == taskListId);
            if (taskListToRemove == null) return;

            userDto.TaskLists.Remove(taskListToRemove);
            _context.Entry(taskListToRemove).State = EntityState.Deleted;
        }

        public void AddTaskItemToTaskList(Guid userId, Guid taskListId, TaskItemDTO taskItemDTO)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);
            if (userDto == null) return;

            var existingTaskList = userDto.TaskLists.FirstOrDefault(t => t.Id == taskListId);
            if (existingTaskList == null) return;

            existingTaskList.TaskItems.Add(taskItemDTO);
            _context.Entry(taskItemDTO).State = EntityState.Added;
        }

        public void UpdateTaskItem(Guid userId, Guid taskListId, TaskItemDTO taskItemDTO)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .ThenInclude(u => u.TaskItems)
                .SingleOrDefault(u => u.Id == userId);
            if (userDto == null) return;

            var existingTaskList = userDto.TaskLists.FirstOrDefault(t => t.Id == taskListId);
            if (existingTaskList == null) return;

            var existingTaskItem = existingTaskList.TaskItems.FirstOrDefault(t => t.Id == taskItemDTO.Id);
            if (existingTaskItem == null) return;

            existingTaskItem.Description = taskItemDTO.Description;
            existingTaskItem.IsCompleted = taskItemDTO.IsCompleted;
            _context.Entry(existingTaskItem).State = EntityState.Modified;
        }

        public void DeleteTaskItem(Guid userId, Guid taskListId, Guid taskItemId)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .ThenInclude(u => u.TaskItems)
                .SingleOrDefault(u => u.Id == userId);
            if (userDto == null) return;

            var existingTaskList = userDto.TaskLists.FirstOrDefault(t => t.Id == taskListId);
            if (existingTaskList == null) return;

            var taskItemToRemove = existingTaskList.TaskItems.FirstOrDefault(t => t.Id == taskItemId);
            if (taskItemToRemove == null) return;

            existingTaskList.TaskItems.Remove(taskItemToRemove);
            _context.Entry(taskItemToRemove).State = EntityState.Deleted;
        }
    }
}