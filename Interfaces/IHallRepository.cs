using MovieReservationSystem.Models;

namespace MovieReservationSystem.Interfaces
{
    public interface IHallRepository
    {
        public Task<List<Hall>> GetAllHallsAsync();
        public Task<Hall?> GetHallByIdAsync(Guid hallId);
        public Task AddHall(Hall hall);
        public Task DeleteHall(Hall hall);
    }
}