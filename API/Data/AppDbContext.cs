using System;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace API.Data;

//register this class in program.cs as service
public class AppDbContext(DbContextOptions options) : IdentityDbContext<AppUser>(options)
{
    // we need to pass the options to DbContext forexample the connections strings
    // public AppDbContext(DbContextOptions options) : base(options)
    // {
    //old way to wrting the base constructor
    // }
    public DbSet<Member> Members { get; set; }
    public DbSet<Photo> Photos { get; set; }
    public DbSet<MemberLike> Likes { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Connection> Connections { get; set; }

    //the databases except the postgresql don't support the UTC datetime natively so we need to configure it manually, and will need to override the method that is inside the DbContext class
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //we can use this when we need to configure or override the entity framework conventions
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<IdentityRole>()
            .HasData(
                new IdentityRole
                {
                    Id = "Member_id",
                    Name = "Member",
                    NormalizedName = "MEMBER"
                },
                new IdentityRole 
                {
                    Id = "Moderator_id",
                    Name = "Moderator",
                    NormalizedName = "MODERATOR"
                },
                new IdentityRole
                {
                    Id = "Admin_id",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            );

        modelBuilder.Entity<Message>()
            .HasOne(x => x.Recipient)
            .WithMany(x => x.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne(x => x.Sender)
            .WithMany(x => x.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<MemberLike>()
            .HasKey(x => new { x.SourceMemberId, x.TargetMemberId });

        //we are saying here that one source member can have many liked members, and the foreign key is the sourcememberid, and should we delete the SouceMember then that gonna cascade into the MemberLike entity and delete all the related data.
        modelBuilder.Entity<MemberLike>()
            .HasOne(x => x.SourceMember)
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

        var nullableDateTimeConverter = new ValueConverter<DateTime?, DateTime?>(
            v => v.HasValue ? v.Value.ToUniversalTime() : null ,
            v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null //when we are reading from the database we need to specify that this is in UTC format
        );

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(dateTimeConverter);
                }else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(nullableDateTimeConverter);
                }
            }
        }
    }
}
