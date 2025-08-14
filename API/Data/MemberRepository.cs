using System;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class MemberRepository(AppDbContext context) : IMemberRepository
{
    public async Task<Member?> GetMemberByIdAsync(string id)
    {
        return await context.Members.FindAsync(id);
    }

    public async Task<IReadOnlyList<Member>> GetMembersAsync()
    {
        return await context.Members.ToListAsync();
    }

    public async Task<IReadOnlyList<Photo>> GetPhotosForMemberAsync(string memberId)
    {
        return await context.Members
        .Where(x => x.Id == memberId)
        .SelectMany(x => x.Photos) //select many allows us to do the projection, although we are quering the Members but we will get the list of photos
        .ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await context.SaveChangesAsync() > 0;
    }

    public void Update(Member member)
    {
        context.Entry(member).State = EntityState.Modified;
        //The purpose of this forexample, if we have updated a member with its exact same properties that were already in our database so nothing actually changed but we saved it to database using this method SaveChangesAsync and this method is checking to see if there are any changes if we don't use the update method and save the changes to the database then saveallasync method will return false and we will be checking in our controllers so if we have not successfully saved something for our database we would return a bad request, but if we wanted to avoid that bad request just incase for what ever reason we did allowed sending up an identical member object with the same properties then we avoid getting the error by using the Update method. 
    }
}
