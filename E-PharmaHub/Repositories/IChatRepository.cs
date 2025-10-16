using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IChatRepository
    {
        Task AddAsync(ChatMessage entity);
        Task<IEnumerable<ChatMessage>> GetMessagesByThreadIdAsync(int threadId);
        Task<IEnumerable<MessageThread>> GetUserThreadsAsync(string userId);
        Task<MessageThread?> GetThreadBetweenUsersAsync(string user1Id, string user2Id);
    }
}
