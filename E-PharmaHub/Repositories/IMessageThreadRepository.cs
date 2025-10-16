using E_PharmaHub.Models;

namespace E_PharmaHub.Repositories
{
    public interface IMessageThreadRepository
    {
        Task<MessageThread?> GetByIdWithParticipantsAsync(int threadId);
        Task AddAsync(MessageThread entity);

    }
}
