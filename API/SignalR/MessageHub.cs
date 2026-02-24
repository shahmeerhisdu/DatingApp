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
    public class MessageHub(IMessageRepository messageRepository, IMemberRepository memberRepository) : Hub
    {
        public override async Task OnConnectedAsync()
        {
            // we are gonna need the HTTP context to get the user information from the token and we can do that through the Context property of the hub class, we will send up the user id of the other user as the part of query string parameter when we connect to the hub from the client and then we will get that user id from the query string and then we will send a message to that user to say that the other user has connected to the hub and is ready to receive messages.

            var httpContext = Context.GetHttpContext();
            var otherUser = httpContext?.Request.Query["userId"].ToString() ?? throw new HubException("Other user not found");

            // we will also create the group as well because we need to ensure that messaging is private between the two users and we can do that by creating a group for each pair of users and then we will add both users to that group and then we will send the message to that group and only those two users will receive the message.

            var groupName = GetGroupName(GetUserId(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            //get the messages from the database.
            var messages = await messageRepository.GetMessageThread(GetUserId(), otherUser);    

            //notify the users in this group and pass back the message thread.
            await Clients.Group(groupName).SendAsync("ReceivedMessageThread", messages);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var sender = await memberRepository.GetMemberByIdAsync(GetUserId());

            var recipient = await memberRepository.GetMemberByIdAsync(createMessageDto.RecipientId);

            if (recipient == null || sender == null || sender.Id == createMessageDto.RecipientId) throw new HubException("Can't send this message");

            var message = new Message
            {
                SenderId = sender.Id,
                RecipientId = recipient.Id,
                Content = createMessageDto.Content
            };

            messageRepository.AddMessage(message);

            if (await messageRepository.SaveAllAsync())
            {
                var group = GetGroupName(sender.Id, recipient.Id);
                await Clients.Group(group).SendAsync("NewMessage", message.ToDto());
            }
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            //When a user disconnect from the signalR hub and they are inside the group, then it automatically going to be removed from the group
            
            return base.OnDisconnectedAsync(exception);

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
