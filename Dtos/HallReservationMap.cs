
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Dtos
{
    public class HallReservationMap
    {
        public List<SeatDto> Booked { get; set; } = new List<SeatDto>();
        public List<SeatDto> Available { get; set; } = new List<SeatDto>();
    }
}