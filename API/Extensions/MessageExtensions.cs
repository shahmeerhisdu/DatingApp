using API.DTOs;
using API.Entities;

namespace API.Extensions
{
    public static class MessageExtensions
    {
        //gonna do something to allow us to extend the message entity in the future, as this is going to contain the extension methods for the message entity so we are going to make the class static

        public static MessageDto ToDto(this Message message)
        {
            return new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderDisplayName = message.Sender.DisplayName,
                SenderImageUrl = message.Sender.ImageUrl,
                RecipientId = message.RecipientId,
                RecipientDisplayName = message.Recipient.DisplayName,
                RecipientImageUrl = message.Recipient.ImageUrl,
                Content = message.Content,
                DateRead = message.DateRead,
                MessageSent = message.MessageSent,
            };
        }

        // we also need another object so that we can receive as parameter of what we need to create the message in our database.
    }
}
