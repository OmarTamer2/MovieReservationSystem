using Microsoft.EntityFrameworkCore;
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models.Database;

namespace MovieReservationSystem.Models.Repositories
{
    public class MoviesRepository(appDbContext context) : IMoviesRepository
    {
        public async Task<bool> AddMovieAsync(Movie movie)
        {
            await context.Movies.AddAsync(movie);
            return true;
        }

        public async Task AddShowTime(ShowTime showTime)
        {
            await context.ShowTimes.AddAsync(showTime);
        }

        public async Task<bool> DeleteMovieAsync(Movie movie)
        {
            try
            {
                context.Movies.Remove(movie);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<Movie>> GetAllMoviesAsync()
        {
            var movies = await context.Movies.ToListAsync();
            return movies;
        }

        public async Task<Movie?> GetMovieByIdAsync(Guid movieId)
        {
            var movie = await context.Movies
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == movieId);

            return movie;
        }

        public Task<List<Movie>> GetMoviesByGenreAsync(string genre)
        {
            throw new NotImplementedException();
        }

        public async Task<ShowTime> CheckConflictingShowTimes(Guid hallId, DateTime date, int movieDuration)
        {
            var endTime = date.AddMinutes(movieDuration + 20);
            var showtime = await context.ShowTimes.FirstOrDefaultAsync(s => s.hallId == hallId && s.startTime < endTime && s.endTime > date);
            return showtime!;
        }

        public async Task<List<ShowTime>> GetShowTimesByDate(DateOnly date)
        {
            var startDate = date.ToDateTime(TimeOnly.MinValue);
            var endDate = startDate.AddDays(1);
            return await context.ShowTimes.Where(s => s.startTime >= startDate && s.startTime < endDate).ToListAsync();
        }

        public async Task<ShowTime?> GetShowTimeById(Guid showTimeId)
        {
            return await context.ShowTimes.FindAsync(showTimeId);
        }

        public Task<List<ShowTime>> GetShowTimesByIds(List<Guid> showTimeIds)
        {
            return context.ShowTimes.Where(s => showTimeIds.Contains(s.Id)).ToListAsync();
        }
    }
}