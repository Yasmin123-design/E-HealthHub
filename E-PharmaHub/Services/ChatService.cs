using E_PharmaHub.Hubs;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.SignalR;

namespace E_PharmaHub.Services
{
    public class ChatService : IChatService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext;


        public ChatService(IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
        }

        public async Task<MessageThread> StartConversationAsync(string userId, string pharmacistId)
        {
            var existingThread = await _unitOfWork.Chat.GetThreadBetweenUsersAsync(userId, pharmacistId);
            if (existingThread != null)
                return existingThread;

            var thread = new MessageThread
            {
                Title = $"Chat between {userId} and {pharmacistId}",
                Participants = new List<MessageThreadParticipant>
            {
                new MessageThreadParticipant { UserId = userId },
                new MessageThreadParticipant { UserId = pharmacistId }
            }
            };

            await _unitOfWork.MessageThread.AddAsync(thread);
            await _unitOfWork.CompleteAsync();
            return thread;
        }

        public async Task<ChatMessage> SendMessageAsync(int threadId, string senderId, string text)
        {
            var message = new ChatMessage
            {
                ThreadId = threadId,
                SenderId = senderId,
                Text = text
            };

            await _unitOfWork.Chat.AddAsync(message);
            await _unitOfWork.CompleteAsync();

            await _hubContext.Clients.Group(threadId.ToString())
     .SendAsync("ReceiveMessage", senderId, text, message.SentAt);

            return message;
        }

        public async Task<IEnumerable<ChatMessage>> GetMessagesAsync(int threadId)
        {
            return await _unitOfWork.Chat.GetMessagesByThreadIdAsync(threadId);
        }

        public async Task<IEnumerable<MessageThread>> GetUserThreadsAsync(string userId)
        {
            return await _unitOfWork.Chat.GetUserThreadsAsync(userId);
        }
    }

}
