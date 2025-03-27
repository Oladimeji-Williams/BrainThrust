using System.Security.Claims;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Services.Interfaces
{
    public interface IUserService
    {
        Task<User?> GetAuthenticatedUserAsync(ClaimsPrincipal user);
        int? GetLoggedInUserId(ClaimsPrincipal user);
        Task<bool> IsUserAdmin(ClaimsPrincipal user);
    }
}
