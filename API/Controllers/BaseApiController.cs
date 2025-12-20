using API.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ServiceFilter(typeof(LogUserActivity))] // now we can notice that when user does something it will update the last active property of that user.
    [Route("api/[controller]")]
    [ApiController]
    public class BaseApiController : ControllerBase
    {
        //Action Filter allows us to take an action before or after the request that comes into our API controller.
        //So for instance a user comes in and they request for the list of members then it will hit our action filter first, and like I say we can do something before or after that request hits our controller action. And when the request is finished and we have sent back our list of members than we can use the action filters to update the last active property of that user.
    }
}
