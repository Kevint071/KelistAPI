using Application.Users.Dtos;
using Microsoft.EntityFrameworkCore;


namespace Application.Data
{
    public interface IApplicationDbContext
    {
        DbSet<UserDTO> Users { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
