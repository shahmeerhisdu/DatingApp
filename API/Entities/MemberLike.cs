namespace API.Entities
{
    public class MemberLike
    {
        //each member will have two lists of MemberLike. The users they liked by and the users that they have liked.

        public required string SourceMemberId { get; set; }
        public Member SouceMember { get; set; } = null!; //The source member likes the target member
        public required string TargetMemberId { get; set; }
        public Member TargetMember { get; set; } = null!;
    }
}
