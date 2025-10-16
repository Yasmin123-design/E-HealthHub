using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IChatService
    {
        Task<MessageThread> StartConversationAsync(string userId, int pharmacistId);
        Task<ChatMessage> SendMessageAsync(int threadId, string senderId, string text);
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(int threadId);
        Task<IEnumerable<MessageThread>> GetUserThreadsAsync(string userId);
    }
}
