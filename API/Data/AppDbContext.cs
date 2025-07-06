using System;
using API.Entities;
using Microsoft.EntityFrameworkCore;

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
}
