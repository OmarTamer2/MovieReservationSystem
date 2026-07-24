
namespace MovieReservationSystem.Models
{
    public class Reservation : BaseEntity
    {
        public Guid userId {get; set;}
        public Guid seatId {get; set;}
        public Guid ShowTimeId { get; set; }
        public decimal totalAmount { get; set; }
    }
}