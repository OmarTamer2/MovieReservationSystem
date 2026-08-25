using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Dtos;
using MovieReservationSystem.Interfaces;
using MovieReservationSystem.Models;
using MovieReservationSystem.Models.Database;
using MovieReservationSystem.Requests;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HallController : ControllerBase
    {
        private readonly IHallRepository _hallRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMoviesRepository _movieRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUser _currentUser;

        public HallController(IHallRepository hallRepository, IMoviesRepository movieRepository, ICurrentUser currentUser, IUnitOfWork unitOfWork, UserManager<AppUser> userManager)
        {
            _hallRepository = hallRepository;
            _movieRepository = movieRepository;
            _unitOfWork = unitOfWork;
            _currentUser = currentUser;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
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

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllHalls()
        {
            var halls = await _hallRepository.GetAllHallsAsync();
            var hallDtos = halls.Select(h => new HallDto
            {
                Id = h.Id,
                Name = h.Name,
                numberOfSeats = h.numberOfSeats,
                IsVip = h.IsVip,
                seats = h.seats
            }).ToList();

            return Ok(hallDtos);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetHallById([FromRoute] Guid id)
        {
            var hall = await _hallRepository.GetHallByIdAsync(id);
            if(hall is null) return NotFound();

            var hallDto = new HallDto
            {
                Id = hall.Id,
                Name = hall.Name,
                numberOfSeats = hall.numberOfSeats,
                IsVip = hall.IsVip,
                seats = hall.seats
            };

            return Ok(hallDto);
        }

        [Authorize]
        [HttpPost("{hallId:guid}")]
        public async Task<IActionResult> MakeReservation([FromBody] MakeReservationRequest request)
        {
            
            if(_currentUser.Id is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var user = await _userManager.FindByIdAsync(_currentUser.Id);
            if (user is null)
            {
                return Unauthorized("User not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if(_currentUser.Id is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var SeatIds = request.SeatIds?.Split(',').Select(Guid.Parse).ToList();
            if (SeatIds is null || !SeatIds.Any())
            {
                return BadRequest("No seat IDs provided.");
            }

            var dublicateReservation = await _hallRepository.GetReservationByIdempotencyKey(request.IdempotencyKey);
            if (dublicateReservation is not null)
            {
                return Conflict("A reservation with the same idempotency key already exists.");
            }

            var showTimeExists = await _movieRepository.GetShowTimeById(request.ShowTimeId);
            if (showTimeExists is null)
            {
                return NotFound("Showtime not found.");
            }

            if(DateTime.UtcNow.CompareTo(showTimeExists.startTime) >= 0)
            {
                return BadRequest("Cannot make a reservation for a showtime that has already started.");
            }

            var seats = await _hallRepository.GetSeatsByIdsAsync(SeatIds);
            if(seats.Count != SeatIds.Count)
            {
                return NotFound("One or more seats not found.");
            }

            var ConflictingReservation = await _hallRepository.GetReservationByShowTimeIdAndSeatIdsAsync(request.ShowTimeId, SeatIds);
            if (ConflictingReservation is not null)
            {
                return Conflict("one or more seats is already reserved for the specified showtime.");
            }

            if(user.Balance < showTimeExists.ticketCost * SeatIds.Count)
            {
                return BadRequest("Insufficient balance to make the reservation.");
            }
            
            List<Reservation> reservations = new List<Reservation>();
            foreach(var seat in seats)
            {
                var reservation = new Reservation
                {
                    seatId = seat.Id,
                    ShowTimeId = request.ShowTimeId,
                    IdempotencyKey = request.IdempotencyKey,
                    userId = Guid.Parse(_currentUser.Id),
                    status = Enums.ReservationStatus.Confirmed,
                    totalAmount = showTimeExists.ticketCost
                };

                reservations.Add(reservation);
            }
            
            user.Balance -= showTimeExists.ticketCost * SeatIds.Count;
            await _hallRepository.AddReservations(reservations);
            await _unitOfWork.SaveChangesAsync();
            
            return Created();
        }

        [Authorize]
        [HttpGet("/api/reservations")]
        public async Task<IActionResult> GetAllUserReservations()
        {
            if(_currentUser.Id is null)
            {
                return Unauthorized("User is not authenticated.");
            }
            
            var reservations = await _hallRepository.GetReservationsByUserId(Guid.Parse(_currentUser.Id));

            var showTimes = await _movieRepository.GetShowTimesByIds(reservations.Select(r => r.ShowTimeId).ToList());

            List<ReservationDto> reservationDtos = new List<ReservationDto>();
            var showTimesDict = showTimes.ToDictionary(st => st.Id, st => st);
            for( int i = 0; i < reservations.Count; i++)
            {
                var reservationDto = new ReservationDto
                {
                    Id = reservations[i].Id,
                    seatId = reservations[i].seatId,
                    Time = showTimesDict[reservations[i].ShowTimeId].startTime,
                    status = reservations[i].status,
                    totalAmount = reservations[i].totalAmount
                };
                reservationDtos.Add(reservationDto);
            }
            return Ok(reservationDtos);
        }

        [Authorize]
        [HttpPut("/api/reservations/{reservationId:guid}/cancel")]
        public async Task<IActionResult> CancelReservation([FromRoute] Guid reservationId)
        {
            if(_currentUser.Id is null)
            {
                return Unauthorized("User is not authenticated.");
            }

            var reservation = await _hallRepository.GetReservationById(reservationId);
            if(reservation is null)
            {
                return NotFound("Reservation not found.");
            }

            var showtime = await _movieRepository.GetShowTimeById(reservation.ShowTimeId);
            if(showtime is null)
            {
                return NotFound("Showtime not found.");
            }

            if( DateTime.UtcNow.CompareTo(showtime.startTime) >= 0)
            {
                return BadRequest("Cannot cancel reservation for a showtime that has already started.");
            }

            if(reservation.userId != Guid.Parse(_currentUser.Id))
            {
                return Forbid("You are not authorized to cancel this reservation.");
            }

            if(reservation.status == Enums.ReservationStatus.Cancelled)
            {
                return BadRequest("Reservation is already cancelled.");
            }

            reservation.status = Enums.ReservationStatus.Cancelled;

            var user = await _userManager.FindByIdAsync(_currentUser.Id);
            if (user is null)
            {
                return Unauthorized("User not found.");
            }

            user.Balance += reservation.totalAmount;
            await _unitOfWork.SaveChangesAsync();

            return Ok("Reservation cancelled successfully.");
        }

        [HttpGet("/api/admin/reservations")]
        public async Task<IActionResult> GetAllReservations()
        {
            var reservations = await _hallRepository.GetAllReservations();
            var showTimes = await _movieRepository.GetShowTimesByIds(reservations.Select(r => r.ShowTimeId).ToList());

            List<ReservationDto> reservationDtos = new List<ReservationDto>();
            for( int i = 0; i < reservations.Count; i++)
            {
                var reservationDto = new ReservationDto
                {
                    Id = reservations[i].Id,
                    seatId = reservations[i].seatId,
                    Time = showTimes[i].startTime,
                    status = reservations[i].status,
                    totalAmount = reservations[i].totalAmount
                };
                reservationDtos.Add(reservationDto);
            }
            return Ok(reservationDtos);
        }
    }
}