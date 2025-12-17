using System.Security.Claims;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]

    // Always use the interface not the implementation class because it will not work as specified in the program.cs
    public class MembersController(IMemberRepository memberRepository, IPhotoService photoService) : BaseApiController
    {
        // we inject the AppDbContext, what that means in practice is that when we receive a HTTP request is received by a .net application, it checks the route and forwards it to the appropriate controller, like if we receive a get request to this localhost:5001/api/Members it will be passed to this controller

        // .Net framework is then responsible for instanciating the new instance of this controller, it looks at its constructor, and also creates a new instance of AppDbContext, so that we actively create the session with our database.

        // [HttpGet]
        //ActionResult allow us to return the HTTP responses
        // public ActionResult<IReadOnlyList<AppUser>> GetMembers()
        // {

        //     // we got the code working but it is not the best practice because this is the synchronus code that means the thread that comes to this request will get blocked untill the data is returned

        //     // to make this code async we will need to add the Async keyword and the return type will be in the Task<>
        //     var members = context.Users.ToList();
        //     return members;
        // }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<Member>>> GetMembers([FromQuery] MemberParams memberParams) //as pagingparams is an object and we gonna pass this information as the querystring so we need to add the FromQuery.
        {
            // so what difference does this make?
            // So instead of request being blocked when it comes in to this API controller once we are awaiting for something to happen and comeback from our database, instead action of going out to another thread is delegated to another thread, that thread can go about the business for querying the databse, if thats the long running database query it is not gonna effect our API controller from servicing our request.
            // var members = await context.Users.ToListAsync();
            // return members;
            memberParams.CurrentMemberId = User.GetMemberId();

            return Ok(await memberRepository.GetMembersAsync(memberParams));
            // by this Ok(await memberRepository.GetMembersAsync()) we loose the type safety, if we change the return type from member to AppUser it will not complain because we are using the Ok response, so IActionResult was used as a way to get a bit of type safety in controller method, and it works for most things a part from list and a repository.
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Member>> GetMember(string id)
        {
            // var member = context.Users.Where(u => u.Id == id).FirstOrDefault();
            var member = await memberRepository.GetMemberByIdAsync(id);
            if (member == null)
                return NotFound();
            return member;
        }

        [HttpGet("{id}/photos")]
        public async Task<ActionResult<IReadOnlyList<Photo>>> GetMemberPhotos(string id)
        {
            return Ok(await memberRepository.GetPhotosForMemberAsync(id));
        }

        [HttpPut]
        public async Task<ActionResult> UpdateMember(MemberUpdateDto memberUpdateDto)
        {
            // var memberId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // we have to get the member by the Id so claimtypes contain the memberId in the name identifier

            // if (memberId == null)
            // {
            //     return BadRequest("Oops - no Id found in the token");
            // }

            var memberId = User.GetMemberId();

            var member = await memberRepository.GetMemberForUpdate(memberId);
            if (member == null)
            {
                return BadRequest("Could not get the member");
            }

            member.DisplayName = memberUpdateDto.DisplayName ?? member.DisplayName;
            member.Description = memberUpdateDto.Description ?? member.Description;
            member.City = memberUpdateDto.City ?? member.City;
            member.Country = memberUpdateDto.Country ?? member.Country;

            member.User.DisplayName = memberUpdateDto.DisplayName ?? member.User.DisplayName;

            // memberRepository.Update(member); //its just an optional

            if (await memberRepository.SaveAllAsync())
            {
                return NoContent();
            }

            return BadRequest("Failed to Update the Member");

        }

        [HttpPost("add-photo")]

        public async Task<ActionResult<Photo>> AddPhoto([FromForm]IFormFile file)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Can't update the member");

            var result = await photoService.UploadPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                MemberId = User.GetMemberId()
            };

            if (member.ImageUrl == null)
            {
                member.ImageUrl = photo.Url;
                member.User.ImageUrl = photo.Url;
            }

            member.Photos.Add(photo);

            if (await memberRepository.SaveAllAsync())
            {
                return photo;
            }

            return BadRequest("Problem adding photo");

        }

        [HttpPut("set-main-photo/{photoId}")]

        public async Task<ActionResult> SetMainPhoto(int photoId)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Can not get the member from the token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if(member.ImageUrl == photo?.Url || photo == null)// if the image is same as the current image then no need to set the image again
            {
                return BadRequest("Can not set this as the main image");
            }

            member.ImageUrl = photo.Url;
            member.User.ImageUrl = photo.Url;

            if (await memberRepository.SaveAllAsync()) return NoContent();

            return BadRequest("Problem setting the main photo");
        }

        [HttpDelete("delete-photo/{photoId}")]
        public async Task<ActionResult> DeletePhoto(int photoId)
        {
            var member = await memberRepository.GetMemberForUpdate(User.GetMemberId());

            if (member == null) return BadRequest("Can not get the member from the token");

            var photo = member.Photos.SingleOrDefault(x => x.Id == photoId);

            if (photo == null || photo.Url == member.ImageUrl)
            {
                return BadRequest("This photo can't be deleted");
            }

            if (photo.PublicId != null)
            {
                var result = await photoService.DeletePhotoAsync(photo.PublicId);
                if (result.Error != null) return BadRequest(result.Error.Message);
            }

            member.Photos.Remove(photo);

            if (await memberRepository.SaveAllAsync()) return Ok();

            return BadRequest("Problem deleting the photo");
        }

    }
}
