using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace API.Entities;

public class Member
{
    public string Id { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string? ImageUrl { get; set; }
    public required string DisplayName { get; set; }
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime LastActive { get; set; } = DateTime.UtcNow;
    public required string Gender { get; set; }
    public string? Description { get; set; }
    public required string City { get; set; }
    public required string Country { get; set; }

    //Navigation Property
    [JsonIgnore]
    public List<Photo> Photos { get; set; } = [];

    //adding these two navigation properties for the MemberLike entity, each member will have two lists of MemberLike. The users they are liked by and the users that they have liked.
    [JsonIgnore] // we use JsonIgnore because we don't want to return the entire list of likes when we return a member object.
    public List<MemberLike> LikedByMembers { get; set; } = [];
    public List<MemberLike> LikedMembers { get; set; } = [];

    [JsonIgnore]
    [ForeignKey(nameof(Id))]
    public AppUser User { get; set; } = null!;

}
