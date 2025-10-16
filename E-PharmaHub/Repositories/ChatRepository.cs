using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly EHealthDbContext _context;

        public ChatRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(ChatMessage entity)
        {
            await _context.ChatMessages.AddAsync(entity);
        }
        public async Task<IEnumerable<ChatMessage>> GetMessagesByThreadIdAsync(int threadId)
        {
            return await _context.ChatMessages
                .Where(m => m.ThreadId == threadId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<MessageThread>> GetUserThreadsAsync(string userId)
        {
            return await _context.MessageThreads
                .Include(m => m.Messages)
                .Include(t => t.Participants)
                .ThenInclude(a => a.User)
                .Where(t => t.Participants.Any(p => p.UserId == userId))
                .ToListAsync();
        }

        public async Task<MessageThread?> GetThreadBetweenUsersAsync(string user1Id, string user2Id)
        {
            return await _context.MessageThreads
                .Include(t => t.Participants)
                .FirstOrDefaultAsync(t =>
                    t.Participants.Any(p => p.UserId == user1Id) &&
                    t.Participants.Any(p => p.UserId == user2Id));
        }
    }
}
