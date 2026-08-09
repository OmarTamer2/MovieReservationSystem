using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models;
using MovieReservationSystem.Requests;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallController : ControllerBase
    {
        private readonly IHallRepository _hallRepository;
        private readonly IUnitOfWork _unitOfWork;

        public HallController(IHallRepository hallRepository, IUnitOfWork unitOfWork)
        {
            _hallRepository = hallRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> CreateHall([FromBody] CreateHallRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (request.columnCount <= 0 || request.rowCount <= 0)
            {
                return BadRequest("Column and row counts must be greater than zero.");
            }
            
            var SeatsCount = request.columnCount * request.rowCount;
            var hall = new Hall
            {
                Name = request.name,
                numberOfSeats = SeatsCount,
                IsVip = request.isVip,
                seats = new List<Seat>()
            };

            for(int column = 1; column <= request.columnCount; column++)
            {
                for(int row = 1; row <= request.rowCount; row++)
                {
                    var seat = new Seat
                    {
                        column = column,
                        row = row
                    };

                    hall.seats?.Add(seat);
                }
            }

            await _hallRepository.AddHall(hall);
            await _unitOfWork.SaveChangesAsync();

            return Created();
        }
    }
}