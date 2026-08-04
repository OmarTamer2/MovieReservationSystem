using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Constants;
using MovieReservationSystem.Dtos;
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models;
using MovieReservationSystem.Requests;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MoviesController : Controller
    {
        private readonly IStorageClient _storageClient;
        private readonly IMoviesRepository _moviesRepository;
        private readonly IHallRepository _hallRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MoviesController(IStorageClient storageClient, IMoviesRepository moviesRepository, IHallRepository hallRepository, IUnitOfWork unitOfWork)
        {
            _storageClient = storageClient;
            _moviesRepository = moviesRepository;
            _hallRepository = hallRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost("AddMovie")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddMovie([FromForm] CreateMovieRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var fileStream = request.Photo.OpenReadStream();
            var fileContentType = request.Photo.ContentType;
            var name = Guid.NewGuid().ToString();

            var result = await _storageClient.uploadFile(fileStream, name, fileContentType, Buckets.posters);

            if(result is null)
            {
                return BadRequest("Poster Upload Failed");
            }

            var movie = new Movie
            {
                Title = request.Title,
                PhotoUrl = name,
                Description = request.Description,
                ReleaseDate = request.ReleaseDate,
                Duration = request.Duration
            };

            var IsSuccess = await _moviesRepository.AddMovieAsync(movie);
            if (IsSuccess)
            {
                await _unitOfWork.SaveChangesAsync();
                return Created();
            }

            return BadRequest("Movie Addition Failed");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetMovieById([FromRoute] Guid id)
        {
            var movie = await _moviesRepository.GetMovieByIdAsync(id);
            if(movie is null) return NotFound();

            var viewUrl = await _storageClient.get_file_url(Buckets.posters, movie.PhotoUrl);

            var movieDto = new MovieDto
            {
                Id = movie.Id,
                Title = movie.Title,
                PhotoUrl = viewUrl,
                ReleaseDate = movie.ReleaseDate,
                Description = movie.Description,
                Duration = movie.Duration
            };

            return Ok(movieDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllMovies()
        {
            var movies = await _moviesRepository.GetAllMoviesAsync();

            var movieDtos = new List<MovieDto>();
            foreach(var movie in movies)
            {
                var viewUrl = await _storageClient.get_file_url(Buckets.posters, movie.PhotoUrl);

                var movieDto = new MovieDto
                {
                    Id = movie.Id,
                    Title = movie.Title,
                    PhotoUrl = viewUrl,
                    ReleaseDate = movie.ReleaseDate,
                    Description = movie.Description,
                    Duration = movie.Duration
                };

                movieDtos.Add(movieDto);
            }
            return Ok(movieDtos);
        }

        [HttpPost("showtime")]
        public async Task<IActionResult> AddShowTime([FromBody] CreateShowTimeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            if(request.startTime.CompareTo(DateTime.UtcNow) < 0)
            {
                return BadRequest("Invalid start time. Only future dates are allowed.");
            }
            
            var conflictShowTime = await _moviesRepository.GetShowTimeByHallIdAndDate(request.hallId, request.startTime);

            if(conflictShowTime is not null)
            {
                return BadRequest("Showtime conflict detected for the specified hall and date.");
            }

            var hall = await _hallRepository.GetHallByIdAsync(request.hallId);

            if(hall is null)
            {
                return NotFound("Specified hall does not exist.");
            }

            var showTime = new ShowTime
            {
                hallId = request.hallId,
                movieId = request.movieId,
                date = request.startTime,
                ticketCost = request.TicketCost
            };

            await _moviesRepository.AddShowTime(showTime);
            await _unitOfWork.SaveChangesAsync();

            return Created();
        }
        
        [HttpGet("showtime")]
        public async Task<IActionResult> GetShowTimesByDate([FromQuery] DateOnly date)
        {
            if (date.CompareTo(DateTime.UtcNow.Date) < 0)
            {
                return BadRequest("Invalid date. Only future dates are allowed.");
            }

            var showTimes = await _moviesRepository.GetShowTimesByDate(date);

            if (showTimes == null || !showTimes.Any())
            {
                return NotFound("No showtimes found for the specified date.");
            }

            return Ok(showTimes);
        }
    }
}