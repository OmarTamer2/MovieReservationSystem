
namespace MovieReservationSystem.Models
{
    public class Seat : BaseEntity
    {
        public int row { get; set; }
        public int column { get; set; }
        public Guid hallId { get; set; }
    }
}