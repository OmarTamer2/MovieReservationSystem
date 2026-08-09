
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
        [Range(1, int.MaxValue, ErrorMessage = "Year must be a positive integer.")]
        public int year { get; set; }
        [Required]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int month { get; set; }
        [Required]
        [Range(1, 31, ErrorMessage = "Day must be between 1 and 31.")]
        public int day { get; set; }
        [Required]
        [Range(0, 23, ErrorMessage = "Hour must be between 0 and 23.")]
        public int hour { get; set; }
        [Required]
        [Range(0, 59, ErrorMessage = "Minutes must be between 0 and 59.")]
        public int minutes { get; set; }
        [Required]
        public decimal TicketCost { get; set; }
    }
}