//Entity represents the table in the data base
// we will use the object relational mapper to map the entities to table in the database.
using Microsoft.AspNetCore.Identity;

namespace API.Entities;
//name space is the logical representation of where the class is located its not the physical file or typically where the class is

//you can create a new name space and create the AppUser class in that

public class AppUser : IdentityUser
// this has to be public because entity framework is outside of our class
{
    //each appuser will have an associated member with it, and in entity framework we call this relations, one app user is going to be related with one member

    public required string DisplayName { get; set; }
    //string gives the null waring because it is the reference type and it is a common type of exception when somebody try to use the property while the compiler don't know its null or not that exception is called nulled reference exception.
    public string? ImageUrl { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
    // Navigation Property
    public Member Member { get; set; } = null!;
}
