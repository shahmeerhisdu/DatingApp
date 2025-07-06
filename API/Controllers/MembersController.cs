using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api/[controller]")] //localhost:5001/api/Members
    [ApiController]
    public class MembersController(AppDbContext context) : ControllerBase
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
        public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
        {
            // so what difference does this make?
            // So instead of request being blocked when it comes in to this API controller once we are awaiting for something to happen and comeback from our database, instead action of going out to another thread is delegated to another thread, that thread can go about the business for querying the databse, if thats the long running database query it is not gonna effect our API controller from servicing our request.
            var members = await context.Users.ToListAsync();
            return members;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetMember(string id)
        {
            // var member = context.Users.Where(u => u.Id == id).FirstOrDefault();
            var member = await context.Users.FindAsync(id);
            if (member != null)
                return member;
            return NotFound();
        }

    }
}
