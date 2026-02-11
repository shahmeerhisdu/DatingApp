using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

// we need to add the roles to the token as well so that when the user presents the token to access a resource we can check the roles from the token itself instead of going to the database to get the roles for the user

public class TokenService(IConfiguration config, UserManager<AppUser> userManager) : ITokenService
{
    public async Task<string> CreateToken(AppUser user)
    {
        /** we need this key as our token is going to be signed by our API so that when our token is presented by the client browser to our API service then this TokenKey needs to be validated that its a genuine toked thats been issued by our server 
        The way we do that is we sign the token, and we need to install packages for these security related stuff.

        Now as we are using HMACSHA512 our TokenKey needs to be in certain length 

        ?? this is null correlessing operator
        **/
        var tokenKey = config["TokenKey"] ?? throw new Exception("Can not get token key");
        if (tokenKey.Length < 64) throw new Exception("Your Token Key Needs to be grater than or equal to 64");

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        //now this is the key we are going to use to sing the token and symmetric security means same key is the one used to both encrypt and decrypt information

        //There is another system that is commonly used with the certificates such as SSL certificate they use AsymmetricSecurity System with uses the concept of public and private key, but because our token key doesn' have to leave our server then the simplest approach is to use the SymmetricSecurityKey

        var claims = new List<Claim>
        {
            new (ClaimTypes.Email, user.Email!), // thats a modern way of creating the object new Claim(ClaimTypes.Email,..)
            new (ClaimTypes.NameIdentifier, user.Id),
            // new ("CustomWhatevet", "CustomThing") //this is also fine for custom claim
        };

        var roles = await userManager.GetRolesAsync(user); // this is going to give us a list of roles for the user

        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role))); // we are adding the roles to the claims

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // here we are signing the token

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Specify the properties that our token needs
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        //we are gonna store this long live refresh token along with the user object in the database and we gonna return to the client browser as the cookie and HTTP only secure cookie and we will do this inside the account controller.
        var randomBytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(randomBytes);
    }
}

/**
typically we create a service that does not need to access our database, or it is the third party serviec that we are using, the token service don't need to access our database we are going to pass the user object to the method for create token 
**/
