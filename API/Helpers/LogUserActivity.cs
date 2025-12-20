using API.Data;
using API.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace API.Helpers
{
    public class LogUserActivity: IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            //so the idea is we will do something after we have executed this ActionExecutionDelegate next, which means we gonna do something after the action inside our controller has executed the request.

            var resultContext = await next();

            //any thing that is going to happen after this next is gonna happen after the request has been execucted in our controller. And ofcourse if we want to make things happen before this then we are going to put that code before the await next(); line.

            //so we need to establish if this is the authenticated request, there is no point in continuing if the user is not authenticated.

            if (context.HttpContext.User.Identity?.IsAuthenticated !=  true) return;

            var memberId = resultContext.HttpContext.User.GetMemberId();

            // as we have to update the database and we need the dbContext, but we can not inject anything into this method so we will be using the service locator pattern to get the dbContext from the httpContext request services.

            var dbContext = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

            await dbContext.Members
                .Where(x => x.Id == memberId)
                .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastActive, DateTime.UtcNow));
            //Because ExecuteUpdateAsync is related to HTTPContect so we need to add this as the scoped service in Program.cs            
        }
    }
}
