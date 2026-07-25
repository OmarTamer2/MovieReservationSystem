
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieReservationSystem.Enums;
using MovieReservationSystem.Models.Database;

namespace MovieReservationSystem.Models
{
    public class Reservation : BaseEntity
    {
        public Guid userId { get; set; }
        public Guid seatId { get; set; }
        public Guid ShowTimeId { get; set; }
        public ReservationStatus status { get; set; }
        public decimal totalAmount { get; set; }
    }

    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> builder)
        {
            builder
            .HasOne<AppUser>()
            .WithMany()
            .HasForeignKey(r => r.userId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

            builder
            .HasOne<Seat>()
            .WithMany()
            .HasForeignKey(r => r.seatId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);

            builder
            .HasOne<ShowTime>()
            .WithMany()
            .HasForeignKey(r => r.ShowTimeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.SetNull);

            builder.Property(r => r.status).HasConversion<string>();

            builder.HasIndex(["ShowTimeId", "seatId"]);

        }
    }
}