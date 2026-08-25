using Microsoft.AspNetCore.Authorization;
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

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddMovie([FromForm] CreateMovieRequest request, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            using var fileStream = request.Photo.OpenReadStream();
            var fileContentType = request.Photo.ContentType;
            var name = Guid.NewGuid().ToString();

            var result = await _storageClient.uploadFile(fileStream, name, fileContentType, Buckets.posters, cancellationToken);

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

        [Authorize]
        [HttpPost("/api/showtime")]
        public async Task<IActionResult> AddShowTime([FromBody] CreateShowTimeRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var startTime = new DateTime(DateTime.UtcNow.Year, request.month, request.day, request.hour, request.minutes, 0);
            
            if(startTime.CompareTo(DateTime.UtcNow) < 0)
            {
                return BadRequest("Invalid start time. Only future dates are allowed.");
            }

            var movie = await _moviesRepository.GetMovieByIdAsync(request.movieId);
            if(movie is null) return NotFound("This movie doesn't exist");

            var movieDuration = movie.Duration;
            
            var conflictShowTime = await _moviesRepository.CheckConflictingShowTimes(request.hallId, startTime, movieDuration);

            if(conflictShowTime is not null)
            {
                return BadRequest("Showtime conflict detected for the specified hall and date.");
            }

            var hall = await _hallRepository.GetHallByIdAsync(request.hallId);

            if(hall is null)
            {
                return NotFound("Specified hall does not exist.");
            }

            var endTime = startTime.AddMinutes(movieDuration + 20);
            var showTime = new ShowTime
            {
                hallId = request.hallId,
                movieId = request.movieId,
                startTime = startTime,
                endTime = endTime,
                ticketCost = request.TicketCost
            };

            await _moviesRepository.AddShowTime(showTime);
            await _unitOfWork.SaveChangesAsync();

            return Created();
        }
        
        [Authorize]
        [HttpGet("/api/showtime")]
        public async Task<IActionResult> GetShowTimesByDate([FromQuery] int year, [FromQuery] int month, [FromQuery] int day)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            DateOnly date;
            try
            {
                date = new DateOnly(year, month, day);
            }
            catch (ArgumentOutOfRangeException)
            {
                return BadRequest("Invalid date parameters.");
            }
            
            var currentDate = new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day);
            if (date.CompareTo(currentDate) < 0)
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

        [Authorize]
        [HttpGet("/api/showtime/{showTimeId:guid}")]
        public async Task<IActionResult> GetShowTimeReservationById([FromRoute] Guid showTimeId)
        {
            var showTime = await _moviesRepository.GetShowTimeById(showTimeId);
            if (showTime is null)
            {
                return NotFound("Showtime not found.");
            }

            var hall = await _hallRepository.GetHallByIdAsync(showTime.hallId);
            if (hall is null)
            {
                return NotFound("Hall not found.");
            }

            var BookedSeatsIds = await _hallRepository.GetReservedSeatsByShowTimeId(showTimeId);
            var bookedSeats = await _hallRepository.GetSeatsByIdsAsync(BookedSeatsIds);
            
            var availableSeats = hall.seats!.Where(s => !BookedSeatsIds.Contains(s.Id)).ToList();
            var bookedSeatsDto = bookedSeats.Select(s => new SeatDto
            {
                Id = s.Id,
                row = s.row,
                column = s.column,
                hallId = s.hallId
            }).ToList();

            var availableSeatsDto = availableSeats.Select(s => new SeatDto
            {
                Id = s.Id,
                row = s.row,
                column = s.column,
                hallId = s.hallId
            }).ToList();
            
            return Ok(new HallReservationMap
            {
                Booked = bookedSeatsDto,
                Available = availableSeatsDto
            });
        }
    }
}