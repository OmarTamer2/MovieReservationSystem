
using MovieReservationSystem.Models;

namespace MovieReservationSystem.Dtos
{
    public class HallReservationMap
    {
        public List<Seat> Booked { get; set; } = new List<Seat>();
        public List<Seat> Available { get; set; } = new List<Seat>();
    }
}