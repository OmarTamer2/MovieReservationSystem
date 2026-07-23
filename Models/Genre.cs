using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieReservationSystem.Models
{
    public class Genre : BaseEntity
    {
        public string Name { get; set; } = "";
    }

    public class GenreConfiguration : IEntityTypeConfiguration<Genre>
    {
        public void Configure(EntityTypeBuilder<Genre> builder)
        {
            builder.Property(g => g.Name).IsRequired().HasMaxLength(50);
        }
    }
}