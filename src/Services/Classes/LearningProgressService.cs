using BrainThrust.src.Dtos.LearningProgressDtos;
using BrainThrust.src.Mappers;
using BrainThrust.src.Repositories.Interfaces;
using BrainThrust.src.Services.Interfaces;

namespace BrainThrust.src.Services.Classes
{
    public class LearningProgressService : ILearningProgressService
    {
        private readonly ILearningProgressRepository _progressRepository;

        public LearningProgressService(ILearningProgressRepository progressRepository)
        {
            _progressRepository = progressRepository;
        }

        public async Task<bool> MarkLessonCompleted(int userId, int lessonId)
        {
            return await _progressRepository.MarkLessonCompleted(userId, lessonId);
        }

        public async Task<List<LearningProgressDto>> GetUserProgress(int userId, int subjectId)
        {
            var progress = await _progressRepository.GetUserProgress(userId, subjectId);
            return LearningProgressMapper.ToDTOList(progress);
        }

        public async Task<LearningProgressDto?> GetLastVisitedLesson(int userId)
        {
            var lastLesson = await _progressRepository.GetLastVisitedLesson(userId);
            return LearningProgressMapper.ToDTO(lastLesson);
        }
    }
}
