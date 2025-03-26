using BrainThrust.src.Dtos.LessonDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class LessonMappers
    {
        public static GetLessonDto ToLessonDto(this Lesson lesson)
        {
            return new GetLessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Content = lesson.Content,
                VideoUrl = lesson.VideoUrl,
                TopicId = lesson.TopicId
            };
        }

        public static Lesson ToLesson(this CreateLessonDto createLessonDto, int topicId)
        {
            return new Lesson
            {
                Title = createLessonDto.Title,
                Content = createLessonDto.Content,
                VideoUrl = createLessonDto.VideoUrl,
                TopicId = topicId
            };
        }

        public static void UpdateLessonFromDto(this Lesson lesson, UpdateLessonDto updateLessonDto)
        {
            if (!string.IsNullOrEmpty(updateLessonDto.Title))
                lesson.Title = updateLessonDto.Title;
            
            if (!string.IsNullOrEmpty(updateLessonDto.Content))
                lesson.Content = updateLessonDto.Content;
            
            if (!string.IsNullOrEmpty(updateLessonDto.VideoUrl))
                lesson.VideoUrl = updateLessonDto.VideoUrl;
        }
    }
}
