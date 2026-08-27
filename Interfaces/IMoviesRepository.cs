using MovieReservationSystem.Models;

namespace MovieReservationSystem.Interfaces
{
    public interface IMoviesRepository
    {
        public Task<bool> AddMovieAsync(Movie movie);
        public Task<bool> DeleteMovieAsync(Movie movie);
        public Task<Movie?> GetMovieByIdAsync(Guid movieId);
        public Task<List<Movie>> GetAllMoviesAsync();
        public Task<List<Movie>> GetMoviesByGenreAsync(string genre);
        public Task<ShowTime> CheckConflictingShowTimes(Guid hallId, DateTime date, int movieDuration);
        public Task<List<ShowTime>> GetShowTimesByDate(DateOnly date);
        public Task AddShowTime(ShowTime showTime);
        public Task<ShowTime?> GetShowTimeById(Guid showTimeId);
        public Task<List<ShowTime>> GetShowTimesByIds(List<Guid> showTimeIds);
        Task AddGenreAsync(Genre genre);
        Task<Genre?> GetGenreByNameAsync(string genreName);

    }
}