using Application.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Users.Dtos;
using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;

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
        public async Task<UserDTO?> GetByIdAsync(Guid id) => await _context.Users.Include(x => x.TaskLists).SingleOrDefaultAsync(c => c.Id == id);

        public void AddTaskListToUser(Guid userId, TaskListDTO taskListDto)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);

            if (userDto != null)
            {
                userDto.TaskLists.Add(taskListDto);
                // Asegurarnos de que EF Core la trate como una entidad nueva
                _context.Entry(taskListDto).State = EntityState.Added;
            }
        }

        public void UpdateTaskList(Guid userId, TaskListDTO taskListDto)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);

            if (userDto != null)
            {
                var existingTaskList = userDto.TaskLists.FirstOrDefault(tl => tl.Id == taskListDto.Id);
                if (existingTaskList != null)
                {
                    existingTaskList.TaskListName = taskListDto.TaskListName;
                    // Si hay más propiedades como TaskItems, actualizarlas aquí
                    _context.Entry(existingTaskList).State = EntityState.Modified;
                }
            }
        }
        public void DeleteTaskList(Guid userId, Guid taskListId)
        {
            var userDto = _context.Users
                .Include(u => u.TaskLists)
                .SingleOrDefault(u => u.Id == userId);

            if (userDto != null)
            {
                var taskListToRemove = userDto.TaskLists.FirstOrDefault(tl => tl.Id == taskListId);
                if (taskListToRemove != null)
                {
                    userDto.TaskLists.Remove(taskListToRemove);
                    // Si TaskListDTO es una entidad rastreada por EF, podemos marcarla como eliminada
                    _context.Entry(taskListToRemove).State = EntityState.Deleted;
                }
            }
        }

        public void AddTaskItemToTaskList(Guid userId, Guid taskListId, TaskItemDTO taskItemDTO)
        {
            var userDto = _context.Users.Include(u => u.TaskLists).SingleOrDefault(u => u.Id == userId);
            if (userDto != null)
            {
                var existingTaskList = userDto.TaskLists.FirstOrDefault(t => t.Id == taskListId);
                if (existingTaskList != null)
                {
                    existingTaskList.TaskItems.Add(taskItemDTO);
                    // Asegurarnos de que EF Core la trate como una entidad nueva
                    _context.Entry(taskItemDTO).State = EntityState.Added;
                }
            }
        }
    }
}