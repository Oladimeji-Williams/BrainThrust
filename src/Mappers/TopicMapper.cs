using BrainThrust.src.Dtos.TopicDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class TopicMapper
    {
        // Maps Topic entity to GetTopicDto
        public static GetTopicDto ToTopicDto(this Topic topic)
        {
            return new GetTopicDto
            {
                Id = topic.Id,
                Title = topic.Title,
                Description = topic.Description,
                SubjectId = topic.SubjectId,
                LessonIds = topic.Lessons?.Select(l => l.Id).ToList() ?? new List<int>()
            };
        }


        // Maps CreateTopicDto to Topic entity
        public static Topic ToTopic(this CreateTopicDto createTopicDto, int subjectId)
        {
            return new Topic
            {
                Title = createTopicDto.Title,
                Description = createTopicDto.Description,
                SubjectId = subjectId
            };
        }

        // Updates an existing Topic entity from UpdateTopicDto
        public static Topic UpdateTopicFromDto(this Topic topic, UpdateTopicDto updateTopicDto)
        {
            if(!string.IsNullOrEmpty(updateTopicDto.Title))
            {
                topic.Title = updateTopicDto.Title;
            }
            if(!string.IsNullOrEmpty(updateTopicDto.Description))
            {
                topic.Description = updateTopicDto.Description;
            }

            return topic;
        }

    }

}