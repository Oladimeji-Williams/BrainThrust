using BrainThrust.src.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BrainThrust.src.Repositories.Classes
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> TopicExists(int topicId)
        {
            return await _context.Topics.AnyAsync(t => t.Id == topicId);
        }
    }

}