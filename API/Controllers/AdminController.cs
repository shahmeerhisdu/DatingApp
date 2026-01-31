using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class AdminController : BaseApiController
    {
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public ActionResult GetUsersWithRoles()
        {
            return Ok("Only Admins can see this");
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photos-to-moderate")]
        public ActionResult GetPhotosForModeration()
        {
            //this will be a type of another admin or moderator type task so that when the user uploads a photo it needs to be approved by an admin or moderator before it is visible to other users
            // but for that we need to configure these policies in the program.cs file
            return Ok("Admins or Moderators can see this");
        }
    }
}
