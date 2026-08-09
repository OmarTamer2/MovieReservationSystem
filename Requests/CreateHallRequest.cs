using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Requests
{
    public class CreateHallRequest
    {
        [Required]
        public string name { get; set; } = "";
        [Required]
        public int columnCount { get; set; }
        [Required]
        public int rowCount { get; set; }
        [Required]
        public bool isVip { get; set; }
    }
}