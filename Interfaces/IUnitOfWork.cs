
namespace MovieReservationSystem.Interfaces
{
    public interface IUnitOfWork
    {
        public Task SaveChangesAsync();
    }
}