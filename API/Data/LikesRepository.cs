using API.Entities;
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

        public async Task<IReadOnlyList<Member>> GetMemberLikes(string predicate, string memberId)
        {
            var query = context.Likes.AsQueryable();

            switch (predicate)
            {
                case "liked":
                    return await query
                        .Where(m => m.SourceMemberId == memberId)
                        .Select(m => m.TargetMember)
                        .ToListAsync();
                case "likedBy":
                    return await query
                        .Where(m => m.TargetMemberId == memberId)
                        .Select(m => m.SourceMember)
                        .ToListAsync();
                default: //mutual, both liked each other

                    var likeIds = await GetCurrentMemberLikeIds(memberId);
                    return await query
                        .Where(m => m.SourceMemberId == memberId && likeIds.Contains(m.SourceMemberId))
                        .Select(m => m.SourceMember)
                        .ToListAsync();
            }
        }

        public async Task<bool> SaveAllChanges()
        {
            return await context.SaveChangesAsync() > 0;
        }
    }
}
