using Application.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using Application.Users.Dtos;

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
        public async Task<UserDTO?> GetByIdAsync(Guid id) => await _context.Users.SingleOrDefaultAsync(c => c.Id == id);
    }
}