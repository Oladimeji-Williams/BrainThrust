using BrainThrust.src.Dtos.EnrollmentDtos;
using BrainThrust.src.Mappers;
using BrainThrust.src.Services.Classes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainThrust.src.Controllers
{

    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(ApplicationDbContext context, UserService userService, ILogger<EnrollmentController> logger)
        {
            _context = context;
            _userService = userService;
            _logger = logger;
        }

        /// <summary>
        /// Get all active enrollments (Admin only).
        /// </summary>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<ActionResult<List<GetEnrollmentDto>>> GetEnrollments()
        {
            _logger.LogInformation("Admin fetching all active enrollments.");

            var enrollments = await _context.Enrollments
                .Where(e => !e.IsDeleted)
                .Include(e => e.User)
                .Include(e => e.Subject)
                .Select(e => EnrollmentMapper.ToGetEnrollmentDto(e))
                .ToListAsync();
                

            return Ok(enrollments);
        }

        /// <summary>
        /// Get the logged-in user's active enrollments.
        /// </summary>
        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<List<GetEnrollmentDto>>> GetUserEnrollments()
        {
            var user = await _userService.GetAuthenticatedUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to user enrollments.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            _logger.LogInformation("Fetching enrollments for user Id {UserId}", user.Id);

            var enrollments = await _context.Enrollments
                .Where(e => e.UserId == user.Id && !e.IsDeleted)
                .Include(e => e.Subject)
                .Select(e => EnrollmentMapper.ToGetEnrollmentDto(e))
                .ToListAsync();

            if (!enrollments.Any())
            {
                _logger.LogWarning("No active enrollments found for user Id {UserId}", user.Id);
                return NotFound(new { message = "No active enrollments found." });
            }

            return Ok(enrollments);
        }

        /// <summary>
        /// Check if the logged-in user is enrolled in a specific subject.
        /// </summary>
        [Authorize]
        [HttpGet("user/subject/{subjectId:int}")]
        public async Task<ActionResult> IsUserEnrolledInCubject(int subjectId)
        {
            var user = await _userService.GetAuthenticatedUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Unauthorized access attempt to check subject enrollment.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            _logger.LogInformation("Checking if user {UserId} is enrolled in cubject {CubjectId}", user.Id, subjectId);

            var isEnrolled = await _context.Enrollments
                .AnyAsync(e => e.UserId == user.Id && e.SubjectId == subjectId && !e.IsDeleted);

            return isEnrolled
                ? Ok(new { message = "User is enrolled in this subject." })
                : NotFound(new { message = "User is not enrolled in this subject." });
        }

        /// <summary>
        /// Enroll a user in a subject.
        /// </summary>
        [Authorize]
        [HttpPost("enroll")]
        public async Task<IActionResult> EnrollUser([FromBody] CreateEnrollmentDto createEnrollmentDto)
        {
            var user = await _userService.GetAuthenticatedUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Enrollment failed: User not authenticated.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            _logger.LogInformation("User {UserId} attempting to enroll in Subject {SubjectId}", user.Id, createEnrollmentDto.SubjectId);

            var subject = await _context.Subjects.FindAsync(createEnrollmentDto.SubjectId);
            if (subject == null)
            {
                _logger.LogWarning("Subject {SubjectId} not found.", createEnrollmentDto.SubjectId);
                return NotFound(new { message = "Subject not found." });
            }

            var existingEnrollment = await _context.Enrollments
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.SubjectId == createEnrollmentDto.SubjectId);

            if (existingEnrollment != null)
            {
                if (existingEnrollment.IsDeleted)
                {
                    existingEnrollment.IsDeleted = false;
                    existingEnrollment.DateDeleted = null;
                    await _context.SaveChangesAsync();
                    return Ok(new { message = "User successfully re-enrolled in the subject." });
                }
                return BadRequest(new { message = "User is already enrolled in this subject." });
            }

            var enrollment = EnrollmentMapper.ToEnrollment(createEnrollmentDto, user.Id);
            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully enrolled in Subject {SubjectId}", user.Id, createEnrollmentDto.SubjectId);

            return Ok(new { message = "User successfully enrolled in the subject." });
        }

        /// <summary>
        /// Soft delete an enrollment (Unenroll logged-in user from a subject).
        /// </summary>
        [Authorize]
        [HttpDelete("user/subject/{subjectId:int}")]
        public async Task<IActionResult> DeleteEnrollment(int subjectId)
        {
            var user = await _userService.GetAuthenticatedUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("Unenrollment failed: User not authenticated.");
                return Unauthorized(new { message = "User not authenticated." });
            }

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == user.Id && e.SubjectId == subjectId);

            if (enrollment == null || enrollment.IsDeleted)
            {
                return NotFound(new { message = "Enrollment not found or already removed." });
            }

            enrollment.IsDeleted = true;
            enrollment.DateDeleted = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} successfully unenrolled from Subject {SubjectId}", user.Id, subjectId);

            return NoContent(); // 204 No Content instead of 200 OK
        }
    }

}