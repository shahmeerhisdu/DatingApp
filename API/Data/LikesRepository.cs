using API.Entities;
using API.Helpers;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class LikesRepository(AppDbContext context) : ILikesRepository
    {
        public void AddLike(MemberLike like)
        {
            context.Likes.Add(like);
        }

        public void DeleteLike(MemberLike like)
        {
            context.Likes.Remove(like);
        }

        public async Task<IReadOnlyList<string>> GetCurrentMemberLikeIds(string memberId)
        {
            //for this one we need to use projection, and projection means using a select
            return await context.Likes
                .Where(like => like.SourceMemberId == memberId)
                .Select(like => like.TargetMemberId)
                .ToListAsync();
        }

        public async Task<MemberLike?> GetMemberLike(string sourceMemberId, string targetMemberId)
        {
            return await context.Likes.FindAsync(sourceMemberId, targetMemberId);
        }

        public async Task<PaginatedResult<Member>> GetMemberLikes(LikesParams likedParams)
        {
            var query = context.Likes.AsQueryable();
            IQueryable<Member> result; //because we need to create the paginated result outside of the switch, and we don't need to return from the switch statement

            switch (likedParams.Predicate)
            {
                case "liked":
                    result = query
                        .Where(m => m.SourceMemberId == likedParams.MemberId)
                        .Select(m => m.TargetMember);
                    break;
                case "likedBy":
                    result = query
                        .Where(m => m.TargetMemberId == likedParams.MemberId)
                        .Select(m => m.SourceMember);
                    break;
                default: //mutual, both liked each other
                    var likeIds = await GetCurrentMemberLikeIds(likedParams.MemberId);
                    result = query
                        .Where(m => m.TargetMemberId == likedParams.MemberId && likeIds.Contains(m.SourceMemberId))
                        .Select(m => m.SourceMember);
                    break;
            }

            return await PaginationHelper.CreateAsync(
                result,
                likedParams.PageNumber,
                likedParams.PageSize);
        }

    }
}
