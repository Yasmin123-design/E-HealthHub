using E_PharmaHub.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace E_PharmaHub.Repositories.MessageThreadRepo
{
    public class MessageThreadRepository : IMessageThreadRepository
    {
        private readonly EHealthDbContext _context;

        public MessageThreadRepository(EHealthDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(MessageThread entity)
        {
            await _context.AddAsync(entity);
        }

        public async Task<MessageThread?> GetByIdWithParticipantsAsync(int threadId)
        {
            return await _context.MessageThreads
                .Include(t => t.Participants)
                .Include(t => t.Messages)
                .FirstOrDefaultAsync(t => t.Id == threadId);
        }
    }
}
