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

        public async Task<Reservation?> GetReservationByShowTimeIdAndSeatIdsAsync(Guid showTimeId, List<Guid> seatIds)
        {
            return await context.Reservations.FirstOrDefaultAsync(r => r.ShowTimeId == showTimeId && seatIds.Contains(r.seatId));
        }

        public async Task<Reservation?> GetReservationByIdempotencyKey(Guid IdempotencyKey)
        {
            return await context.Reservations.FirstOrDefaultAsync(r => r.IdempotencyKey == IdempotencyKey);
        }

        public async Task<List<Reservation>> GetReservationsByUserId(Guid UserId)
        {
            return await context.Reservations.Where(s => s.userId == UserId).ToListAsync();
        }

        public async Task<Seat?> GetSeatByIdAsync(Guid seatId)
        {
            return await context.Seats.FirstOrDefaultAsync(s => s.Id == seatId);
        }

        public async Task<List<Seat>> GetSeatsByIdsAsync(List<Guid> SeatIds)
        {
            return await context.Seats.Where(s => SeatIds.Contains(s.Id)).ToListAsync();
        }

        public async Task AddReservations(List<Reservation> reservations)
        {
            await context.Reservations.AddRangeAsync(reservations);
        }

        public Task<Reservation?> GetReservationById(Guid reservationId)
        {
            return context.Reservations.FirstOrDefaultAsync(r => r.Id == reservationId);
        }

        public async Task<List<Guid>> GetReservedSeatsByShowTimeId(Guid showTimeId)
        {
            return await context.Reservations
                .Where(r => r.ShowTimeId == showTimeId && r.status == Enums.ReservationStatus.Confirmed)
                .Select(r => r.seatId)
                .ToListAsync();
        }

        public Task<List<Reservation>> GetAllReservations()
        {
            return context.Reservations.ToListAsync();
        }
    }
}