using BrainThrust.src.Dtos.EnrollmentDtos;
using BrainThrust.src.Models.Entities;

namespace BrainThrust.src.Mappers
{
    public static class EnrollmentMapper
    {
        public static GetEnrollmentDto ToGetEnrollmentDto(Enrollment enrollment)
        {
            return new GetEnrollmentDto
            {
                UserId = enrollment.UserId,
                SubjectId = enrollment.SubjectId,
                SubjectTitle = enrollment.Subject?.Title ?? "Unknown",
                UserFirstName = $"{enrollment.User?.FirstName ?? ""} {enrollment.User?.LastName ?? ""}".Trim()
            };
        }

        public static Enrollment ToEnrollment(CreateEnrollmentDto createEnrollmentDto, int userId)
        {
            return new Enrollment
            {
                UserId = userId,
                SubjectId = createEnrollmentDto.SubjectId
            };
        }
    }
}
