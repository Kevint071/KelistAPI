using Application.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Users.Dtos;
using Application.TaskLists.Dtos;

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
    }
}