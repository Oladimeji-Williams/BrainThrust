using BrainThrust.src.Models.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using BrainThrust.src.Services.Interfaces;

namespace BrainThrust.src.Services.Classes
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetAuthenticatedUserAsync(ClaimsPrincipal user)
        {
            var email = user.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email)) return null;

            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public int? GetLoggedInUserId(ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : null;
        }
        public async Task<bool> IsUserAdmin(ClaimsPrincipal user)
        {
            var userId = GetLoggedInUserId(user);
            if (userId == null) return false;

            var userEntity = await _context.Users.FindAsync(userId.Value);
            return userEntity != null && userEntity.Role == "Admin";
        }

    }
}
