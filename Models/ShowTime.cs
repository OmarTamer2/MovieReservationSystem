
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieReservationSystem.Models
{
    public class ShowTime : BaseEntity
    {
        public Guid movieId { get; set; }
        public Guid hallId { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public decimal ticketCost { get; set; }
    }

    public class ShowTimeConfiguration : IEntityTypeConfiguration<ShowTime>
    {
        public void Configure(EntityTypeBuilder<ShowTime> builder)
        {
            builder.HasKey(st => st.Id);
            
            builder
            .HasOne<Movie>()
            .WithMany()
            .HasForeignKey(st => st.movieId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder
            .HasOne<Hall>()
            .WithMany()
            .HasForeignKey(st => st.hallId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder.Property(st => st.ticketCost).HasColumnType("decimal(18,2)").HasPrecision(10);
        }
    }
}