using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models.Database;

namespace MovieReservationSystem.Models.Repositories
{
    public class HallRepository(appDbContext context) : IHallRepository
    {
        public async Task AddHall(Hall hall)
        {
            await context.Halls.AddAsync(hall);
        }

        public async Task DeleteHall(Hall hall)
        {
            context.Halls.Remove(hall);
        }

        public async Task<List<Hall>> GetAllHallsAsync()
        {
            return await context.Halls.ToListAsync();
        }

        public async Task<Hall?> GetHallByIdAsync(Guid hallId)
        {
            return await context.Halls.Include(x => x.seats).FirstOrDefaultAsync(h => h.Id == hallId);
        }
    }
}