using MovieReservationSystem.Enums;

namespace MovieReservationSystem.Dtos
{
    public class ReservationDto
    {
        public Guid Id { get; set; }
        public Guid seatId { get; set; }
        public DateTime Time { get; set; }
        public ReservationStatus status { get; set; }
        public decimal totalAmount { get; set; }
    }
}