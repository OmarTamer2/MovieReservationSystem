using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MovieReservationSystem.Requests
{
    public class MakeReservationRequest
    {
        [Required]
        public Guid MovieId { get; set; }
        [Required]
        public string? SeatIds { get; set; }
        [Required]
        public Guid IdempotencyKey { get; set; }
    }
}