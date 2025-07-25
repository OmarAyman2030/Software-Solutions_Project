using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Software_Solutions.Data;
using System.Security.Claims;

namespace Software_Solutions.Controllers
{
    [ApiController]
    [Route("api/[Controller]")]
    public class UserCont : ControllerBase
    {
        private readonly AppDbContext _context;

        public UserCont (AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("profile-data")]
        public async Task<ActionResult> GetProfile()
        {
            var userID = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userID == null) return Unauthorized("Token is invalid");

            var user = await _context.Users.FindAsync(int.Parse(userID));

            if (user == null) return Unauthorized("User Not found.");

            var ProfileUser = new
            {
                user.ID,
                user.FullName,
                user.Email,
                user.PhoneNumber
            };

            return Ok(new
            {
                message = $"Hello Eng/{user.FullName.Split(' ')[0]}",
                ProfileUser
            });

        }
    }
}
