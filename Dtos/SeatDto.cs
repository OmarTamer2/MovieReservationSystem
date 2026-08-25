
namespace MovieReservationSystem.Dtos
{
    public class SeatDto
    {
        public Guid Id { get; set; }
        public int row { get; set; }
        public int column { get; set; }
        public Guid hallId { get; set; }
    }
}