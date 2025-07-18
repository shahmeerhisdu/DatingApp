using API.DTOs;
using API.Entities;
using API.Interfaces;

namespace API.Extensions;

public static class AppUserExtensions
{
    // because we are going to extend the functionality of the APP user and we need to make the extension class static.
    // and when we make a class static that means we don't need to create the instance of this class in order to use its functioanlity
    // we can not use the dependency injection in the static classes

    public static UserDto ToDto(this AppUser user, ITokenService tokenService)
    {
        //the first parameter of the class will be the thing that we are extending and we reference using this keyword
        return new UserDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Token = tokenService.CreateToken(user)
        };
    }
}
