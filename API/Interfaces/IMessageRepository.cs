using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IMessageRepository
    {
        void AddMessage(Message message);
        void DeleteMessage(Message message);
        Task<Message?> GetMessage(string messageId);
        Task<PaginatedResult<MessageDto>> GetMessagesForMember(MessageParams messageParams);
        Task<IReadOnlyList<MessageDto>> GetMessageThread(string currentMemberId, string recipientId);
        void AddGroup(Group group);
        Task RemoveConnection(string connectionId);
        Task<Connection?> GetConnection(string connectionId);
        Task<Group?> GetMessageGroup(string groupName);
        Task<Group?> GetGroupForConnection(string connectionId);
        //Bit of work to track the members of Group inside a signalR hub
        //So each user just as the same as our presence they can make a connection from multiple devices or multiple browser windows and each of them will have their unique connection. But as long as at least one connection for that user is inside that group then we are going to consider them online for message read purposes.
    }
}
