using BrainThrust.src.Data;
using BrainThrust.src.Models.Dtos;
using BrainThrust.src.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCubject([FromBody] CreateSubjectDto dto)
        {
            _logger.LogInformation("CreateCubject: Received request to create a new cubject.");

            if (dto == null || string.IsNullOrWhiteSpace(dto.Title))
            {
                _logger.LogWarning("CreateCubject: InvalId cubject data.");
                return BadRequest("Cubject title is required.");
            }

            var subject = new Subject
            {
                Title = dto.Title,
                Description = dto.Description
            };

            try
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                _logger.LogInformation("CreateCubject: Cubject '{Title}' created successfully with Id {Id}.", subject.Title, subject.Id);

                return CreatedAtAction(nameof(GetSubject), new { Id = subject.Id }, new SubjectDto
                {
                    Id = subject.Id,
                    Title = subject.Title,
                    Description = subject.Description,
                    IsDeleted = subject.IsDeleted
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateCubject: Error while creating cubject '{Title}'.", subject.Title);
                return StatusCode(500, "An error occurred while creating the cubject.");
            }
        }

        
        // ✅ Get a single cubject by Id (GET)
        [HttpGet("{Id}")]
        public async Task<ActionResult<SubjectDto>> GetSubject(int Id)
        {
            _logger.LogInformation("GetSubject: Fetching subject with Id {Id}.", Id);

            var subject = await _context.Subjects
                .Include(c => c.Topics)
                .Where(c => c.Id == Id)
                .Select(c => new SubjectDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,
                    Topics = c.Topics.Select(m => new TopicDto { Title = m.Title, Description = m.Description }).ToList()
                })
                .FirstOrDefaultAsync();

            if (subject == null)
            {
                _logger.LogWarning("GetSubject: Subject with Id {Id} not found.", Id);
                return NotFound("Subject not found.");
            }

            return Ok(subject);
        }

        // Get all subjects (GET)
        [HttpGet]
        public async Task<ActionResult<List<SubjectDto>>> GetAllSubjects([FromQuery] bool includeDeleted = false)
        {
            _logger.LogInformation("GetAllSubjects: Fetching all subjects. IncludeDeleted: {IncludeDeleted}", includeDeleted);
            var subjects = _context.Subjects
                .Where(c => !c.IsDeleted)  // Exclude deleted cubjects
                .Include(c => c.Topics)   // Ensure topics are included
                .Select(c => new SubjectDto
                {
                    Id = c.Id,
                    Title = c.Title,
                    Description = c.Description,
                    IsDeleted = c.IsDeleted,
                    Topics = c.Topics.Select(m => new TopicDto
                    {
                        Title = m.Title,
                        SubjectId = m.SubjectId,  // Ensure SubjectId is mapped
                        Description = m.Description
                    }).ToList()
                })
                .ToList();
                
            _logger.LogInformation("GetAllSubjects: Retrieved {Count} subjects.", subjects.Count);
            return Ok(subjects);
        }

        [HttpPatch("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateSubject(int Id, [FromBody] UpdateSubjectDto dto)
        {
            _logger.LogInformation("UpdateCubject: Attempting to update cubject with Id {Id}.", Id);

            var subject = await _context.Subjects.IgnoreQueryFilters().FirstOrDefaultAsync(c => c.Id == Id);
            if (subject == null)
            {
                _logger.LogWarning("UpdateSubject: Subject with Id {Id} not found.", Id);
                return NotFound("Subject not found.");
            }

            // Restore cubject if it's soft-deleted
            if (subject.IsDeleted)
            {
                _logger.LogInformation("UpdateSubject: Restoring soft-deleted subject with Id {Id}.", Id);
                subject.IsDeleted = false;
            }

            // Apply updates only if values are provIded
            if (!string.IsNullOrWhiteSpace(dto.Title)) subject.Title = dto.Title;
            if (!string.IsNullOrWhiteSpace(dto.Description)) subject.Description = dto.Description;
            if (!string.IsNullOrWhiteSpace(dto.ThumbnailUrl)) subject.ThumbnailUrl = dto.ThumbnailUrl;

            // Allow updating IsDeleted manually only if explicitly provIded
            if (dto.IsDeleted.HasValue)
            {
                subject.IsDeleted = dto.IsDeleted.Value;
            }

            try
            {
                await _context.SaveChangesAsync();
                _logger.LogInformation("UpdateCubject: Cubject with Id {Id} updated successfully.", Id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateSubject: Error updating subject with Id {Id}.", Id);
                return StatusCode(500, "An error occurred while updating the cubject.");
            }
        }

        // Soft delete a subject (DELETE)
        [HttpDelete("{Id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteSubject(int Id)
        {
            var subject = await _context.Subjects.FindAsync(Id);
            if (subject == null) return NotFound("Subject not found.");

            _context.Subjects.Remove(subject);
            await _context.SaveChangesAsync();
            
            return NoContent(); // 204 No Content indicates successful deletion
        }

    }

}