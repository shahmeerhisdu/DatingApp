using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        //Define the contract between the interface and the implementing class
        Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId);
        Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likedParams);
        //this is gonna tell what kind of list we want to get, either the list of members that the user has liked or the list of members that have liked the user or mutual likes.
        Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId);
        void DeleteLike(MemberLike like);
        void AddLike(MemberLike like);

    }
}
