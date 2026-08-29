
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieReservationSystem.Models
{
    public class Movie : BaseEntity
    {
        public string Title { get; set; } = "";
        public string PhotoUrl { get; set; } = "";
        public string Description { get; set; } = "";
        public Guid? genreId {get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int Duration { get; set; }

        public Genre? genre{get; set;} = null;
    }


    public class MovieConfiguration : IEntityTypeConfiguration<Movie>
    {
        public void Configure(EntityTypeBuilder<Movie> builder)
        {
            builder.Property(m => m.Title).IsRequired().HasMaxLength(100);
            builder.Property(m => m.PhotoUrl).IsRequired();
            builder.Property(m => m.Description).IsRequired().HasMaxLength(500);
            builder.Property(m => m.ReleaseDate).IsRequired();
            builder.Property(m => m.Duration).IsRequired();
            builder.HasOne<Genre>().WithMany().HasForeignKey(m => m.genreId);
        }
    }
}