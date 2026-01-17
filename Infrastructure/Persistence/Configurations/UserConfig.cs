using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Configurations
{
    public sealed class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> b)
        {
            b.ToTable("Users");
            b.HasKey(x => x.Id);
            b.Property(x => x.Email).HasMaxLength(320).IsRequired();
            b.HasIndex(x => x.Email).IsUnique();
            b.Property(x => x.DisplayName).HasMaxLength(200).IsRequired();
            b.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();

            b.OwnsMany(x => x.Roles, rb =>
            {
                rb.ToTable("UserRoles");
                rb.WithOwner().HasForeignKey(x => x.UserId);
                rb.HasKey("UserId", "Role");
                rb.Property(x => x.Role).HasConversion<int>();
            });
        }
    }
}
