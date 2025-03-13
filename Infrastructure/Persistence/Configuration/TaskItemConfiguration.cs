using Application.TaskLists.Dtos;
using Application.Tasks.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class TaskItemConfiguration : IEntityTypeConfiguration<TaskItemDTO>
    {
        public void Configure(EntityTypeBuilder<TaskItemDTO> builder)
        {
            builder.ToTable("TaskItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);
            builder.Property(x => x.Description).HasMaxLength(200).IsRequired();
            builder.Property(x => x.IsCompleted).IsRequired();
            builder.HasOne<TaskListDTO>()
               .WithMany()
               .HasForeignKey(x => x.TaskListId);
        }
    }
}
