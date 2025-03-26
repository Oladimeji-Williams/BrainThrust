namespace BrainThrust.src.Repositories.Interfaces
{
    public interface ITopicRepository
    {
        Task<bool> TopicExists(int topicId);
    }

}