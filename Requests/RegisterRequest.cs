using System.ComponentModel.DataAnnotations;

namespace MovieReservationSystem.Requests
{
    public class RegisterRequest
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; } ="";
        [Required]
        [MaxLength(50, ErrorMessage = "Username cannot exceed 50 characters.")]
        public string UserName{get; set;} = "";
        [Required]
        public string Password { get; set; } = "";
        [Required]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = "";
        [Required]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = "";
    }
}