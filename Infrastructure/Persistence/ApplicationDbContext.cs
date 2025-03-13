using Application.Data;
using Application.Data.Interfaces;
using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Application.Users.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext, IUnitOfWork
    {
        public ApplicationDbContext(DbContextOptions options) : base(options) { }

        public DbSet<UserDTO> Users { get; set; }
        public DbSet<TaskListDTO> TaskLists { get; set; }
        public DbSet<TaskItemDTO> TaskItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
