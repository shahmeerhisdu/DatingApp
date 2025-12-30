using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

//register this class in program.cs as service
public class AppDbContext(DbContextOptions options) : DbContext(options)
{
    // we need to pass the options to DbContext forexample the connections strings
    // public AppDbContext(DbContextOptions options) : base(options)
    // {
    //old way to wrting the base constructor
    // }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }

    //the databases except the postgresql don't support the UTC datetime natively so we need to configure it manually, and will need to override the method that is inside the DbContext class
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //we can use this when we need to configure or override the entity framework conventions
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MemberLike>()
            .HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

        //we are saying here that one source member can have many liked members, and the foreign key is the sourcememberid, and should we delete the SouceMember then that gonna cascade into the MemberLike entity and delete all the related data.
        modelBuilder.Entity<MemberLike>()
            .HasOne(x => x.SouceMember)
            .WithMany(x => x.LikedMembers)
            .HasForeignKey(x => x.SourceMemberId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<MemberLike>()
            .HasOne(x => x.TargetMember)
            .WithMany(x => x.LikedByMembers)
            .HasForeignKey(x => x.TargetMemberId)
            .OnDelete(DeleteBehavior.NoAction);

        var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc) //when we are reading from the database we need to specify that this is in UTC format
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }
            }
        }
    }
}
