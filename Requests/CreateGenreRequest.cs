using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieReservationSystem.Requests
{
    public class CreateGenreRequest
    {
        public string GenreName { get; set; } = null!;
    }
}