
namespace MovieReservationSystem.Models
{
    public class Hall : BaseEntity
    {
        public string Name { get; set; } = "";
        public int numberOfSeats { get; set; }
        public bool IsVip { get; set; } = false;
        public List<Seat>? seats { get; set; }
    }
}