using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MovieReservationSystem.Models.Database
{
    public class appDbContext(DbContextOptions<appDbContext> options) : IdentityDbContext<AppUser>(options)
    {
        public DbSet<Movie> Movies => Set<Movie>();
        public DbSet<Genre> Genres => Set<Genre>();
        public DbSet<Hall> Halls => Set<Hall>();
        public DbSet<Reservation> Reservations => Set<Reservation>();
        public DbSet<Seat> Seats => Set<Seat>();
        public DbSet<ShowTime> ShowTimes => Set<ShowTime>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}