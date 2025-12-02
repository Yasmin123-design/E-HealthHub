using E_PharmaHub.Dtos;
using E_PharmaHub.Hubs;
using E_PharmaHub.Models;
using E_PharmaHub.UnitOfWorkes;
using Microsoft.AspNetCore.SignalR;

namespace E_PharmaHub.Services.ChatServ
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

        public async Task<MessageThreadDto> StartConversationWithPharmacistAsync(string userId, int pharmacistId)
        {
            var pharmacist = await _unitOfWork.PharmasistsProfile.GetByIdAsync(pharmacistId);
            if (pharmacist == null)
                throw new Exception("Pharmacist not found.");

            var pharmacistUserId = pharmacist.AppUserId;

            var existingThread = await _unitOfWork.Chat.GetThreadBetweenUsersAsync(userId, pharmacistUserId);
            if (existingThread != null)
            {
                return new MessageThreadDto
                {
                    Id = existingThread.Id,
                    Title = existingThread.Title,
                    ParticipantIds = existingThread.Participants.Select(p => p.UserId).ToList()
                };
            }

            var thread = new MessageThread
            {
                Title = $"Chat between {userId} and {pharmacistUserId}",
                Participants = new List<MessageThreadParticipant>
        {
            new MessageThreadParticipant { UserId = userId },
            new MessageThreadParticipant { UserId = pharmacistUserId }
        }
            };

            await _unitOfWork.MessageThread.AddAsync(thread);
            await _unitOfWork.CompleteAsync();

            return new MessageThreadDto
            {
                Id = thread.Id,
                Title = thread.Title,
                ParticipantIds = thread.Participants.Select(p => p.UserId).ToList()
            };
        }
        public async Task<MessageThreadDto> StartConversationWithDoctorAsync(string userId, int doctorId)
        {
            var doctor = await _unitOfWork.Doctors.GetByIdAsync(doctorId);
            if (doctor == null)
                throw new Exception("Doctor not found.");

            var doctorUserId = doctor.AppUserId;

            var existingThread = await _unitOfWork.Chat.GetThreadBetweenUsersAsync(userId, doctorUserId);
            if (existingThread != null)
            {
                return new MessageThreadDto
                {
                    Id = existingThread.Id,
                    Title = existingThread.Title,
                    ParticipantIds = existingThread.Participants.Select(p => p.UserId).ToList()
                };
            }

            var thread = new MessageThread
            {
                Title = $"Chat between {userId} and {doctorUserId}",
                Participants = new List<MessageThreadParticipant>
        {
            new MessageThreadParticipant { UserId = userId },
            new MessageThreadParticipant { UserId = doctorUserId }
        }
            };

            await _unitOfWork.MessageThread.AddAsync(thread);
            await _unitOfWork.CompleteAsync();

            return new MessageThreadDto
            {
                Id = thread.Id,
                Title = thread.Title,
                ParticipantIds = thread.Participants.Select(p => p.UserId).ToList()
            };
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

        public async Task<IEnumerable<ThreadDto>> GetUserThreadsAsync(string userId)
        {
            var threads = await _unitOfWork.Chat.GetUserThreadsAsync(userId);

            return threads.Select(t => new ThreadDto
            {
                Id = t.Id,
                Title = t.Title,
                Participants = t.Participants.Select(p => new ParticipantDto
                {
                    UserId = p.UserId,
                    UserName = p.User?.UserName ?? "Unknown"
                }).ToList(),
                LastMessage = t.Messages
                    .OrderByDescending(m => m.SentAt)
                    .Select(m => new LastMessageDto
                    {
                        Text = m.Text,
                        SentAt = m.SentAt
                    })
                    .FirstOrDefault()
            });
        }

    }

}
