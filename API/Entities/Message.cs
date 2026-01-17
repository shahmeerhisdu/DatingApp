namespace API.Entities
{

    // we will be creating another many to many kind of relationship between members for messaging, beacuse each user can send many messages and each user can receive many messages.
    public class Message
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public required string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; } = DateTime.UtcNow;
        public bool SenderDeleted { get; set; } // to track if the sender has deleted the message, but that does not mean that we are going to delete it from the receipient side as well.
        public bool RecipientDeleted { get; set; }

        //Navigation properties
        public required string SenderId { get; set; }
        public Member Sender { get; set; } = null!;
        public required string RecipientId { get; set; }
        public Member Recipient { get; set; } = null!;
    }
}
