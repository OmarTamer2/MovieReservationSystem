
using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Requests
{
    public class CreateShowTimeRequest
    {
        [Required]
        public Guid movieId { get; set; }
        [Required]
        public Guid hallId { get; set; }
        [Required]
        public DateTime startTime { get; set; }
        [Required]
        public decimal TicketCost { get; set; }
    }
}