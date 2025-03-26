using BrainThrust.src.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using BrainThrust.src.Mappers;
using BrainThrust.src.Dtos.UserDtos;
using BrainThrust.src.Services.Interfaces;
using BrainThrust.src.Services.Classes;

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
        [HttpGet()]
        public async Task<IActionResult> GetUsers()
        {
            _logger.LogInformation("Fetching all users from the database.");

            // Fetch all users from the database, excluding passwords
            var users = await _context.Users
                .Select(s => s.ToUserDto())
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
        [HttpGet("me")]
        public async Task<IActionResult> GetUser()
        {
            // Extract user email from token
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(userEmail))
            {
                _logger.LogWarning("Get user failed: Could not extract email from token.");
                return Unauthorized("User not authenticated.");
            }

            // Find the user in the database by email
            var user = await _context.Users
                .Where(s => s.Email == userEmail) // Filter before projecting
                .Select(s => s.ToUserDto()) // Convert to DTO after filtering
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning("Get user failed: User with email {Email} not found.", userEmail);
                return NotFound("User not found.");
            }

            // Return user details excluding the password
            return Ok(user);
        }

        // POST: api/users (Register new user)
        [HttpPost]
        public async Task<IActionResult> RegisterUser(CreateUserDto createUserDto)
        {
            var email = createUserDto.Email.ToLower();
            _logger.LogInformation("Sign-up attempt for email: {Email}", email);

            if (await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower()))
            {
                _logger.LogWarning("Registration failed: Email {Email} already exists.", email);
                return Conflict("User already exists.");
            }

            if (!PasswordValidator.IsValid(createUserDto.Password))
            {
                _logger.LogWarning("Registration failed: Invalid password format for email {Email}.", email);
                return BadRequest("Password does not meet complexity requirements.");
            }

            PasswordHelper.CreatePasswordHash(createUserDto.Password, out byte[] passwordHash, out byte[] passwordSalt);

            bool isAdmin = await _context.Users.AnyAsync(u => u.Role == "Admin");

            var user = createUserDto.ToUser(passwordHash, passwordSalt, isAdmin);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("{Role} registered successfully with email: {Email}", user.Role, email);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new { message = $"{user.Role} registered successfully!" });
        }

        // POST: api/users/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserLoginDto userLoginDto)
        {
            var email = userLoginDto?.Email?.ToLower();
            _logger.LogInformation("Login attempt for email: {Email}", email);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
            if (user == null)
            {
                _logger.LogWarning("Login failed: User {Email} not found.", email);
                return NotFound("User not found.");
            }

            if (!PasswordHelper.VerifyPasswordHash(userLoginDto.Password, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning("Login failed: Incorrect password for user {Email}.", email);
                return Unauthorized("Incorrect password.");
            }

            var token = JwtTokenService.GenerateJwtToken(user);
            _logger.LogInformation("User {Email} successfully logged in.", email);

            var userDto = user.ToUserDto();

            return Ok(new { message = "Login successful!", token = token, user = userDto });
        }

        // PUT: api/users/update-profile
        [Authorize]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto updateUserDto)
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

            user.UpdateUserFromDto(updateUserDto);


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
        public async Task<IActionResult> ChangePassword(ChangePasswordDto changePasswordDto)
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
            if (!PasswordHelper.VerifyPasswordHash(changePasswordDto.CurrentPassword, user.PasswordHash, user.PasswordSalt))
            {
                _logger.LogWarning("Password change failed: Incorrect current password for user {Email}.", email);
                return BadRequest("Current password is incorrect.");
            }

            // Hash the new password
            PasswordHelper.CreatePasswordHash(changePasswordDto.NewPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);

            user.ChangePasswordFromDto(newPasswordHash, newPasswordSalt);

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
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            _logger.LogInformation("Password reset attempt with token: {Token}", resetPasswordDto.Token);

            // Find the user by the reset token
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == resetPasswordDto.Token);
            if (user == null || user.TokenExpiry < DateTime.UtcNow)
            {
                _logger.LogWarning("Password reset failed: Invalid or expired token.");
                return BadRequest("Invalid or expired token.");
            }

            // Hash the new password
            PasswordHelper.CreatePasswordHash(resetPasswordDto.NewPassword, out byte[] newPasswordHash, out byte[] newPasswordSalt);

            user.ResetPasswordFromDto(newPasswordHash, newPasswordSalt);


            await _context.SaveChangesAsync();
            _logger.LogInformation("Password reset successful for user ID: {Id}", user.Id);

            return Ok("Password reset successful!");
        }
    }

}