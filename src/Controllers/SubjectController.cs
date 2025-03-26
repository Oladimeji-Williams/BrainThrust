using BrainThrust.src.Dtos.SubjectDtos;
using BrainThrust.src.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BrainThrust.src.Controllers
{
    [ApiController]
    [Route("api/subjects")]
    public class SubjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SubjectController> _logger;

        public SubjectController(ApplicationDbContext context, ILogger<SubjectController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ✅ Get all subjects (GET)
        [HttpGet]
        public async Task<IActionResult> GetSubjects()
        {
            _logger.LogInformation("Fetching all subjects.");

            var subjects = await _context.Subjects
                .Where(s => !s.IsDeleted)
                .Include(s => s.Topics)
                .ToListAsync();

            var subjectsDto = subjects.Select(s => s.ToSubjectDto()).ToList();

            _logger.LogInformation("Retrieved {Count} subjects.", subjectsDto.Count);
            return Ok(subjectsDto);
        }

        // ✅ Get a single subject by Id (GET)
        [HttpGet("{subjectId}")]
        public async Task<ActionResult<GetSubjectDto>> GetSubject(int subjectId)
        {
            _logger.LogInformation("Fetching subject with Id {Id}.", subjectId);

            var subject = await _context.Subjects
                .Include(s => s.Topics)
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
            {
                _logger.LogWarning("Subject with Id {Id} not found.", subjectId);
                return NotFound("Subject not found.");
            }

            return Ok(subject.ToSubjectDto());
        }

        // ✅ Create a new subject (POST)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateSubject([FromBody] CreateSubjectDto createSubjectDto)
        {
            _logger.LogInformation("Received request to create a new subject.");

            if (createSubjectDto == null || string.IsNullOrWhiteSpace(createSubjectDto.Title))
            {
                _logger.LogWarning("Invalid subject data received.");
                return BadRequest("Subject title is required.");
            }

            var subject = createSubjectDto.ToSubject();

            try
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Subject '{Title}' created successfully with Id {Id}.", subject.Title, subject.Id);

                return CreatedAtAction(nameof(GetSubject), new { subjectId = subject.Id }, subject.ToSubjectDto());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating subject '{Title}'.", subject.Title);
                return StatusCode(500, "An error occurred while creating the subject.");
            }
        }

        // ✅ Update an existing subject (PATCH) - Now using Mapper
        [HttpPatch("{subjectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(int subjectId, [FromBody] UpdateSubjectDto dto)
        {
            _logger.LogInformation("Attempting to update subject with Id {Id}.", subjectId);

            var subject = await _context.Subjects.IgnoreQueryFilters().FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null)
            {
                _logger.LogWarning("Subject with Id {Id} not found.", subjectId);
                return NotFound("Subject not found.");
            }

            try
            {
                // Restore soft-deleted subject if needed
                if (subject.IsDeleted)
                {
                    _logger.LogInformation("Restoring soft-deleted subject with Id {Id}.", subjectId);
                    subject.IsDeleted = false;
                }

                // ✅ Use Mapper to update properties
                subject.UpdateFromDto(dto);

                await _context.SaveChangesAsync();
                _logger.LogInformation("Subject with Id {Id} updated successfully.", subjectId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating subject with Id {Id}.", subjectId);
                return StatusCode(500, "An error occurred while updating the subject.");
            }
        }

        [HttpDelete("{subjectId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int subjectId)
        {
            _logger.LogInformation("Attempting to permanently delete subject with Id {Id}.", subjectId);

            var subject = await _context.Subjects
                .Include(s => s.Topics) // Include related topics if necessary
                .FirstOrDefaultAsync(s => s.Id == subjectId);

            if (subject == null)
            {
                _logger.LogWarning("Subject with Id {Id} not found.", subjectId);
                return NotFound("Subject not found.");
            }

            try
            {
                // ✅ Hard delete instead of soft delete
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Subject '{Title}' with Id {Id} permanently deleted.", subject.Title, subjectId);
                return Ok($"Subject '{subject.Title}' has been deleted successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting subject with Id {Id}.", subjectId);
                return StatusCode(500, "An error occurred while deleting the subject.");
            }
        }

    }
}
