using MovieReservationSystem.Models;

namespace MovieReservationSystem.Dtos
{
    public class HallDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "";
        public int numberOfSeats { get; set; }
        public bool IsVip { get; set; } = false;
        public List<Seat>? seats { get; set; }
    }
}