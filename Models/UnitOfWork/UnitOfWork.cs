
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models.Database;

namespace MovieReservationSystem.Models.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private appDbContext _appDbContext;

        public UnitOfWork(appDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task SaveChangesAsync()
        {
            await _appDbContext.SaveChangesAsync();
        }
    }
}