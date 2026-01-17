namespace API.Helpers
{
    public class MessageParams : PagingParams
    {
        //this is for getting the messages of an individual user, we need to know the memberId of that user and whether they are interested in seeing the inbox or outbox

        public string? MemberId { get; set; }
        public string Container { get; set; } = "Inbox";
    }
}
