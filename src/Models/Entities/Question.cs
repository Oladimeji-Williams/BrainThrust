using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BrainThrust.src.Models.Entities
{
    public class Question : BaseEntity
    {
        [ForeignKey("Quiz")]
        public int QuizId { get; set; }

        [Required]
        public string? QuestionText { get; set; }

        public virtual Quiz? Quiz { get; set; }

        public virtual ICollection<Option> Options { get; set; } = new List<Option>();

        public int CorrectOptionId { get; set; }
        public int Score { get; set; }

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

