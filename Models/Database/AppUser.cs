using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservationSystem.Enums;

namespace MovieReservationSystem.Models.Database
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }

        public AppUser(){}
    }

    public class appUserConfiguration : IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(50);
            builder.Property(u => u.AccountType).IsRequired().HasConversion<string>();
            builder.Property(u => u.Balance).HasColumnType("decimal(18,2)").HasDefaultValue(0);
        }
    }
}