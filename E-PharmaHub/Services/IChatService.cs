using E_PharmaHub.Dtos;
using E_PharmaHub.Models;

namespace E_PharmaHub.Services
{
    public interface IChatService
    {
        Task<MessageThreadDto> StartConversationWithPharmacistAsync(string userId, int pharmacistId);
        Task<ChatMessage> SendMessageAsync(int threadId, string senderId, string text);
        Task<IEnumerable<ChatMessage>> GetMessagesAsync(int threadId);
        Task<IEnumerable<ThreadDto>> GetUserThreadsAsync(string userId);
        Task<MessageThreadDto> StartConversationWithDoctorAsync(string userId, int doctorId);
    }
}
