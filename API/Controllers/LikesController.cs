
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class LikesController(ILikesRepository likesRepository) : BaseApiController
    {
        [HttpPost("{targetMemberId}")]
        public async Task<ActionResult> ToggleLike(string targetMemberId)
        // we don't need to return anything from this because the client is gonna have all of the information they need whether or not the user is liking another user or removing a like
        {
            var sourceMemberId = User.GetMemberId();
            if (sourceMemberId == targetMemberId)
                return BadRequest("You cannot like yourself.");
            var existingLike = await likesRepository.GetMemberLike(sourceMemberId, targetMemberId);
            if (existingLike == null)
            {
                var like = new MemberLike
                {
                    SourceMemberId = sourceMemberId,
                    TargetMemberId = targetMemberId
                };
                likesRepository.AddLike(like); //this will add it to entity framework's change tracker
            }
            else
            {
                likesRepository.DeleteLike(existingLike);
            }
            if (await likesRepository.SaveAllChanges())
                return Ok();
            return BadRequest("Failed to update like.");
        }


        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<string>>> GetCuurentMemberLikeIds()
        {
            return Ok(await likesRepository.GetCurrentMemberLikeIds(User.GetMemberId()));
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMemberLikes(string predicate)
        {
            var members = await likesRepository.GetMemberLikes(predicate, User.GetMemberId());
            return Ok(members);
        }
    }
}
