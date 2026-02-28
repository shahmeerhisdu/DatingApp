using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Primitives;

namespace API.SignalR
{
    [Authorize]
    public class MessageHub(IUnitOfWork uow, IHubContext<PresenceHub> presenceHub) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // we are gonna need the HTTP context to get the user information from the token and we can do that through the Context property of the hub class, we will send up the user id of the other user as the part of query string parameter when we connect to the hub from the client and then we will get that user id from the query string and then we will send a message to that user to say that the other user has connected to the hub and is ready to receive messages.

            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new HubException("Other user not found");

            // we will also create the group as well because we need to ensure that messaging is private between the two users and we can do that by creating a group for each pair of users and then we will add both users to that group and then we will send the message to that group and only those two users will receive the message.

            var groupName = GetGroupName(GetUserId(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            await AddToGroup(groupName);

            //get the messages from the database.
            var messages = await uow.MessageRepository.GetMessageThread(GetUserId(), otherUser);    

            //notify the users in this group and pass back the message thread.
            await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var sender = await uow.MemberRepository.GetMemberByIdAsync(GetUserId());

            var recipient = await uow.MemberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId) throw new HubException("Can't send this message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };

            var groupName = GetGroupName(sender.Id, recipient.Id);
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
            var userInGroup = group != null && group.Connecitons.Any(x => x.UserId == message.RecipientId);//then we know the recepient is in the message group

            if (userInGroup)
            {
                message.DateRead = DateTime.UtcNow;
            }
            uow.MessageRepository.AddMessage(message);

            if (await uow.Complete())
            {
                await Clients.Group(groupName).SendAsync("NewMessage", message.ToDto());
                var connections = await PresenceTracker.GetConnectionsForUser(recipient.Id); // To track that user is online means user has the presence but the user is not on the message component so we can show the toast.
                if (connections != null && connections.Count > 0 && !userInGroup) // user has the connection or user has the presence in the application but the user is not in the group that is created when message component is rendered
                {
                    await presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived", message.ToDto()); // we have to notify all of the connections on all devices
                }
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            //When a user disconnect from the signalR hub and they are inside the group, then it automatically going to be removed from the group, but for our database we have to manually remove it.
            await uow.MessageRepository.RemoveConnection(Context.ConnectionId);
            
            await base.OnDisconnectedAsync(exception);

        }

        private async Task<bool> AddToGroup(string groupName)
        {
            var group = await uow.MessageRepository.GetMessageGroup(groupName);
            var connection = new Connection(Context.ConnectionId, GetUserId());

            if (group == null)
            {
                group = new Group(groupName);
                uow.MessageRepository.AddGroup(group);
            }

            group.Connecitons.Add(connection);

            return await uow.Complete();
        }

        private static string GetGroupName(string? caller, string? other)
        {
            //We will create the group name based on the user ids, we will always gonna return the same regardless of the order they connect to the hub, so we will need to sort this to the alphabetical order, so that we always return the same group name.
            var stringCompare = string.CompareOrdinal(caller, other) < 0;

            return stringCompare ? $"{caller}-{other}" : $"{other}-{caller}";
        }

        private string GetUserId()
        {
            return Context.User?.GetMemberId() ?? throw new HubException("Cannot get the member id");
        }
    }
}
