using Application.TaskLists.Dtos;
using Application.Users.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class TaskListConfiguration : IEntityTypeConfiguration<TaskListDTO>
    {
        public void Configure(EntityTypeBuilder<TaskListDTO> builder)
        {
            builder.ToTable("TaskLists");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(100).IsRequired();
            builder.HasMany(x => x.TaskItems)
                   .WithOne()
                   .HasForeignKey("TaskListId");
            builder.HasOne<UserDTO>().WithMany().HasForeignKey(x => x.UserId);
            builder.Metadata.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
