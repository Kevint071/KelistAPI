using Application.Users.Dtos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<UserDTO>
    {
        public void Configure(EntityTypeBuilder<UserDTO> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id);

            builder.Property(x => x.PersonName).HasMaxLength(50).IsRequired();
            builder.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(x => x.Email).IsRequired();

            builder.Property(x => x.PasswordHash).IsRequired();
            builder.Property(x => x.Role).IsRequired();
            builder.Property(x => x.RefreshToken);
            builder.Property(x => x.RefreshTokenExpiryTime);

            builder.HasMany(x => x.TaskLists).WithOne().HasForeignKey("UserId").OnDelete(DeleteBehavior.Cascade);
            builder.Metadata.SetPropertyAccessMode(PropertyAccessMode.Property);
        }
    }
}
