using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MovieReservationSystem.Enums;
using MovieReservationSystem.Models;
using MovieReservationSystem.Models.Database;
using MovieReservationSystem.Repositories;
using MovieReservationSystem.Requests;

namespace MovieReservationSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenProvider _tokenProvider;

        public AuthenticationController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ITokenProvider tokenProvider)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenProvider = tokenProvider;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = new AppUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                AccountType = AccountType.User
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return BadRequest(ModelState);
            }

            return Ok(new { message = "User registered successfully." });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.password, false, false);
            
            if(!result.Succeeded)
            {
                return Unauthorized(new { message = "The Email or password is wrong." });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if(user is null) return NotFound("User not Found");

            var role = user.AccountType.ToString();
            var token = await _tokenProvider.GenerateToken(user.Id, user.Email!, role);
            return Ok(new { token });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetCurrentUser([FromServices] ICurrentUser currentUser)
        {
            return Ok(new { 
                Id = currentUser.Id,
                Role = currentUser.role
             });
        }
 
    }
}