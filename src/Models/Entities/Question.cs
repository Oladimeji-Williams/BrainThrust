using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Question : BaseEntity
    {
        [ForeignKey("Quiz")]
        public required int QuizId { get; set; }

        [Required]
        public required string QuestionText { get; set; }

        public virtual Quiz? Quiz { get; set; }

        public virtual ICollection<Option> Options { get; set; } = new List<Option>();

        public int? CorrectOptionId { get; set; }  // Store the correct option ID directly

        /// <summary>
        /// Retrieves the correct option based on the CorrectOptionId.
        /// </summary>
        /// <returns>The correct Option or null if not found.</returns>
        public Option? GetCorrectOption()
        {
            return Options.FirstOrDefault(o => o.Id == CorrectOptionId);
        }
    }

}

