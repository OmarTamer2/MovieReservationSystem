using MovieReservationSystem.Models;

namespace MovieReservationSystem.Interfaces
{
    public interface IHallRepository
    {
        public Task<List<Hall>> GetAllHallsAsync();
        public Task<Hall?> GetHallByIdAsync(Guid hallId);
        public Task AddHall(Hall hall);
        public Task DeleteHall(Hall hall);
        Task<Reservation?> GetReservationByShowTimeIdAndSeatIdsAsync(Guid showTimeId, List<Guid> seatIds);
        Task<Reservation?> GetReservationByIdempotencyKey(Guid IdempotencyKey);
        Task<Reservation?> GetReservationById(Guid reservationId);
        Task<Seat?> GetSeatByIdAsync(Guid seatId);
        Task<List<Seat>> GetSeatsByIdsAsync(List<Guid> SeatIds);
        Task<List<Reservation>> GetReservationsByUserId(Guid UserId);
        Task AddReservations(List<Reservation> reservations);
        Task<List<Guid>> GetReservedSeatsByShowTimeId(Guid showTimeId);
        Task<List<Reservation>> GetAllReservations();
    }
}