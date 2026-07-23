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
            var movie = await context.Movies.FirstOrDefaultAsync(m => m.Id == movieId);
            if(movie is null)
            {
                return null;
            }

            return movie;
        }

        public Task<List<Movie>> GetMoviesByGenreAsync(string genre)
        {
            throw new NotImplementedException();
        }
    }
}