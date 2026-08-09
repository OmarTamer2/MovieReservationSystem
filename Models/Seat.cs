
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MovieReservationSystem.Models
{
    public class Seat : BaseEntity
    {
        public int row { get; set; }
        public int column { get; set; }
        public Guid hallId { get; set; }
        public Hall? hall { get; set; }
    }


    public class SeatConfiguration : IEntityTypeConfiguration<Seat>
    {
        public void Configure(EntityTypeBuilder<Seat> builder)
        {
            builder.HasKey(s => s.Id);

            builder
            .HasOne<Hall>()
            .WithMany()
            .HasForeignKey(s => s.hallId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
 
        }
    }
}