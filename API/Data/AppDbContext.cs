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

    //the databases except the postgresql don't support the UTC datetime natively so we need to configure it manually, and will need to override the method that is inside the DbContext class
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //we can use this when we need to configure or override the entity framework conventions
        base.OnModelCreating(modelBuilder);

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
