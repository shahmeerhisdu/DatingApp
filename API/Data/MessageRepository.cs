using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class MessageRepository(AppDbContext context) : IMessageRepository
    {
        public void AddGroup(Group group)
        {
            context.Add<Group>(group);
        }

        public void AddMessage(Message message)
        {
            context.Messages.Add(message);
        }

        public void DeleteMessage(Message message)
        {
            context.Messages.Remove(message);
        }

        public async Task<Connection?> GetConnection(string connectionId)
        {
            return await context.Connections.FindAsync(connectionId);
        }

        public async Task<Group?> GetGroupForConnection(string connectionId)
        {
            return await context.Groups
                .Include(x => x.Connecitons)
                .Where(x => x.Connecitons.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }

        public async Task<Message?> GetMessage(string messageId)
        {
            return await context.Messages.FindAsync(messageId);
        }

        public async Task<Group?> GetMessageGroup(string groupName)
        {
            return await context.Groups
                .Include(x => x.Connecitons)
                .FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams)
        {
            //we need to return the messageDto outside of this repository as well, so we have used the automapper project to map the message to messageDto but automapper is going to be commercial, so to achieve this functionality we will be using the extension methods
            var query = context.Messages
                .OrderByDescending(x => x.MessageSent) //because this is IOrderable but PaginatedResult has different return type
                .AsQueryable();

            query = messageParams.Container switch
            {
                "Outbox" => query.Where(x => x.SenderId == messageParams.MemberId && x.SenderDeleted == false),
                _ => query.Where(x => x.RecipientId == messageParams.MemberId && x.RecipientDeleted == false)
            };

            //create the projection for the MessageDto

            /*var messageQuery = query.Select(message => message.ToDto());*/ // this will not work because select expects an expression and we will need to create the new Extension method for this.

            var messageQuery = query.Select(MessageExtensions.ToDtoProjection());
            //this is going to do effectively inside our database. We are not gonna get the message first and then convert into the DTO which is effectively what we are doing for ToDto method. By this projection we are making the part of the expression down here where we are sending the createasync request with pagination. And what this also means is that even though the sender and recepient are related entities of a message we don't need to eagerly load either of them because the projection is going to select what we need to satisfy our dtoprojection.

            return await PaginationHelper.CreateAsync(messageQuery, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId)
        {
            // when the user get the message thread we want to mark the messages as read, and we will mark as read when the currentmemberId is equal to the recipientId
            await context.Messages
                .Where(m => m.RecipientId == currentMemberId && m.SenderId == recipientId && m.DateRead == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.DateRead, DateTime.UtcNow));

            return await context.Messages
                .Where(x => (x.RecipientId == currentMemberId && x.RecipientDeleted == false && x.SenderId == recipientId) || (x.SenderId == currentMemberId && x.SenderDeleted == false && x.RecipientId == recipientId))
                .OrderBy(x => x.MessageSent)
                .Select(MessageExtensions.ToDtoProjection())
                .ToListAsync();
        }

        public async Task RemoveConnection(string connectionId)
        {
            await context.Connections
                .Where(x => x.ConnectionId == connectionId)
                .ExecuteDeleteAsync(); // Fire and forget, we attempt to remove the connection when the user disconnects from the signalR hub, and lets just remove that row from the database
        }

    }
}
