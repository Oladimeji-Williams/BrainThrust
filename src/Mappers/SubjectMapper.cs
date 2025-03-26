using BrainThrust.src.Dtos.SubjectDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class SubjectMapper
    {
        public static GetSubjectDto ToSubjectDto(this Subject subject)
        {
            return new GetSubjectDto
            {
                Id = subject.Id,
                Title = subject.Title,
                Description = subject.Description
            };
        }

        public static Subject ToSubject(this CreateSubjectDto createSubjectDto)
        {
            return new Subject
            {
                Title = createSubjectDto.Title,
                Description = createSubjectDto.Description
            };
        }

        // ✅ New method to update existing Subject from UpdateSubjectDto
        public static void UpdateFromDto(this Subject subject, UpdateSubjectDto updateSubjectDto)
        {
            if (!string.IsNullOrWhiteSpace(updateSubjectDto.Title)) subject.Title = updateSubjectDto.Title;
            if (!string.IsNullOrWhiteSpace(updateSubjectDto.Description)) subject.Description = updateSubjectDto.Description;
            if (!string.IsNullOrWhiteSpace(updateSubjectDto.ThumbnailUrl)) subject.ThumbnailUrl = updateSubjectDto.ThumbnailUrl;

            if (updateSubjectDto.IsDeleted.HasValue)
            {
                subject.IsDeleted = updateSubjectDto.IsDeleted.Value;
            }
        }
    }
}
