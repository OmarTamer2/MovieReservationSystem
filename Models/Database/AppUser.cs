using Microsoft.AspNetCore.Identity;
using MovieReservationSystem.Enums;

namespace MovieReservationSystem.Models.Database
{
    public class AppUser : IdentityUser
    {
        public AccountType AccountType { get; set; }
        public decimal Balance { get; set; }
    }
}