
namespace MovieReservationSystem.Models
{
    public class ShowTime : BaseEntity
    {
        public Guid movieId { get; set; }
        public Guid hallId { get; set; }
        public DateTime date { get; set; }
        public decimal ticketCost { get; set; }
    }
}