using BrainThrust.src.Data;
using BrainThrust.src.Models.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace BrainThrust.src.Services
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the authenticated user based on their email claim.
        /// </summary>
        public async Task<User> GetAuthenticatedUserAsync(ClaimsPrincipal user)
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <summary>
        /// Get the authenticated user's ID.
        /// </summary>
        public int? GetLoggedInUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : (int?)null;
        }
    }
}
