using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class UnitOfWork(AppDbContext context) : IUnitOfWork
    {
        //the crutial aspect of the UnitOfWork is that our repositories should share the same instance of the context, so we need to inject the context into the constructor of the UnitOfWork and then we will pass this context to the repositories when we create them, so they will share the same instance of the context and then when we call the Complete method it will save all the changes that have been made in all the repositories that are using this context.

        //Now our unit of work will be responsible for initializing the repositories that we are using inside our controllers, so it is responsible for coordinating the repositories operations with in the single trasaction then we have a single complete method inside here and this ensures automacity, so either all of the changes are commited or non of them are.

        // we also share the same dbContext so this ensures consistency across operations and it prevents the kind of issues where two different repositories might save conflicting data independently

        //this also centralizes our repositories in one place so we don't need to inject, if we did need for instance in controller class we need access to all these repositories then we don't need to inject all three of them we just need to inject the unit of work
        private IMemberRepository? _memberRepository;
        private IMessageRepository? _messageRepository;
        private ILikesRepository? _likesRepository;

        public IMemberRepository MemberRepository => _memberRepository ??= new MemberRepository(context);

        public IMessageRepository MessageRepository => _messageRepository ??= new MessageRepository(context);

        public ILikesRepository LikesRepository => _likesRepository ??= new LikesRepository(context);

        public async Task<bool> Complete()
        {
            try
            {
                return await context.SaveChangesAsync() > 0;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while saving changes to the database.", ex);
            }
        }

        public bool HasChanges()
        {
            return context.ChangeTracker.HasChanges();
            //if ef has some changes it has tracked then we can make a decision whether or not we have to call savechangesasync
        }
    }
}