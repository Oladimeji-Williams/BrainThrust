using BrainThrust.src.Models.Dtos;
using BrainThrust.src.Utilities;
using BrainThrust.src.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using BrainThrust.src.Services;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserController> _logger;
        private readonly IEmailService _emailService;
        private readonly UserService _userService;

        public UserController(ApplicationDbContext context, ILogger<UserController> logger, IEmailService emailService, UserService userService)
        {
            _context = context;
            _logger = logger;
            _emailService = emailService;
            _userService = userService;
        }

        // GET: api/users
        [Authorize(Roles ="Admin")]
        [HttpGet("all-users")]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Fetching all users from the database.");

            // Fetch all users from the database, excluding passwords
            var users = await _context.Users
                .Select(user => new
                {
                    user.Id,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    user.PhoneNumber,
                    user.Address,
                    user.Created,
                    user.Modified
                })
                .ToListAsync();

            if (users == null || users.Count == 0)
            {
                _logger.LogWarning("No users found in the database.");
                return NotFound("No users found.");
            }
            _logger.LogInformation("Successfully fetched {Count} users.", users.Count);
            return Ok(users);
        }

        // GET: api/users?email={email}
        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetUser([FromQuery] string email = null)
        {
            // Extract user email from token if not provided
            var userEmail = email ?? User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("Get user failed: Could not extract email from token.");
                return Unauthorized("User not authenticated.");
            }

            // Find the user in the database by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
            if (user == null)
            {
                _logger.LogWarning("Get user failed: User with email {Email} not found.", userEmail);
                return NotFound("User not found.");
            }

            // Return user details excluding the password
            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.PhoneNumber,
                user.Address,
                user.Created,
                user.Modified
            });
        }

        // POST: api/users (Register new user)
        [HttpPost]
        public async Task<IActionResult> RegisterUser(CreateUserDto request)
        {
            var email = request.Email.ToLower();
            _logger.LogInformation("Sign-up attempt for email: {Email}", email);

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", email);
                return Conflict("User already exists.");
            }

            if (!PasswordValidator.IsValid(request.Password))
            {
                _logger.LogWarning("Registration failed: Invalid password format for email {Email}.", email);
                return BadRequest("Password does not meet complexity requirements.");
            }

            PasswordHelper.CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);

            bool adminExists = await _context.Users.AnyAsync(u => u.Role == "Admin");
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = adminExists ? "User" : "Admin"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("{Role} registered successfully with email: {Email}", user.Role, email);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new { message = $"{user.Role} registered successfully!" });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto request)
        {
            var email = request.Email.ToLower();
            _logger.LogInformation("Login attempt for email: {Email}", email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email);
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Email} not found.", email);
                return NotFound("User not found.");
            }

            if (!PasswordHelper.VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning("Login failed: Incorrect password for user {Email}.", email);
                return Unauthorized("Incorrect password.");
            }

            var token = JwtTokenService.GenerateJwtToken(user);
            _logger.LogInformation("User {Email} successfully logged in.", email);

            return Ok(new { message = "Login successful!", token });
        }

        // PUT: api/users/update-profile
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto request)
        {
            // Extract the user ID from the JWT token claims
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                _logger.LogWarning("Update failed: Could not extract user ID from token.");
                return Unauthorized("User not authenticated.");
            }

            int userId = int.Parse(userIdClaim);
            _logger.LogInformation("Updating profile for user ID: {Id}", userId);

            // Find the logged-in user in the database
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Update failed: User with ID {Id} not found.", userId);
                return NotFound("User not found.");
            }

            // Update only the provided fields
            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.PhoneNumber = request.PhoneNumber ?? user.PhoneNumber;
            user.Address = request.Address ?? user.Address;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully updated profile for user ID: {Id}", userId);

            return Ok(new { message = "Profile updated successfully!" });
        }

        // DELETE: api/users/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            _logger.LogInformation("Deleting user with ID: {Id}", id);

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("Delete failed: User with ID: {Id} not found.", id);
                return NotFound("User not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Successfully deleted user with ID: {Id}", id);

            return NoContent();
        }

        // PUT: api/users/change-password
        [Authorize]
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordDto request)
        {
            // Retrieve the logged-in user's email from JWT claims
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                _logger.LogWarning("Unauthorized password change attempt: No email found in claims.");
                return Unauthorized("User identity could not be verified.");
            }

            _logger.LogInformation("Password change attempt for user: {Email}", email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("Password change failed: User {Email} not found.", email);
                return NotFound("User not found.");
            }

            // Verify the current password
            if (!PasswordHelper.VerifyPasswordHash(request.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning("Password change failed: Incorrect current password for user {Email}.", email);
                return BadRequest("Current password is incorrect.");
            }

            // Hash the new password
            PasswordHelper.CreatePasswordHash(request.NewPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);
            user.PasswordHash = newPasswordHash;
            user.PasswordSalt = newPasswordSalt;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Password successfully changed for user: {Email}", email);

            return NoContent();
        }

        // POST: api/users/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto request)
        {
            _logger.LogInformation("Password reset request received for email: {Email}", request.Email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == request.Email.ToLower());
            if (user == null) return NotFound("User not found.");

            user.PasswordResetToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
            user.TokenExpiry = DateTime.UtcNow.AddHours(1);

            await _context.SaveChangesAsync();
            _logger.LogInformation("Password reset token generated for user email: {Email}", request.Email);

            return Ok(new { message = "Password reset instructions sent to your email.", token = user.PasswordResetToken });
        }


        // POST: api/users/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto request)
        {
            _logger.LogInformation("Password reset attempt with token: {Token}", request.Token);

            // Find the user by the reset token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (user == null || user.TokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Password reset failed: Invalid or expired token.");
                return BadRequest("Invalid or expired token.");
            }

            // Hash the new password
            PasswordHelper.CreatePasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            // Clear the reset token and expiration
            user.PasswordResetToken = null;
            user.TokenExpiry = null;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Password reset successful for user ID: {Id}", user.Id);

            return Ok("Password reset successful!");
        }
    }

}