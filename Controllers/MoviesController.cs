using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Constants;
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
        private readonly IUnitOfWork _unitOfWork;

        public MoviesController(IStorageClient storageClient, IMoviesRepository moviesRepository, IUnitOfWork unitOfWork)
        {
            _storageClient = storageClient;
            _moviesRepository = moviesRepository;
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
        
    }
}