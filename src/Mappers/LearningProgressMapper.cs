using BrainThrust.src.Dtos.LearningProgressDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class LearningProgressMapper
    {
        public static LearningProgressDto ToDTO(LessonProgress lessonProgress)
        {
            if (lessonProgress == null) return null;

            return new LearningProgressDto
            {
                LessonId = lessonProgress.LessonId,
                LessonTitle = lessonProgress.Lesson?.Title ?? "Unknown Lesson",
                IsCompleted = lessonProgress.IsCompleted,
                DateCompleted = lessonProgress.DateCompleted
            };
        }

        public static List<LearningProgressDto> ToDTOList(List<LessonProgress> lessonProgresses)
        {
            return lessonProgresses.Select(ToDTO).ToList();
        }
    }
}
