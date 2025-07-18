using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;

namespace API.Services;

public class TokenService(IConfiguration config) : ITokenService
{
    public string CreateToken(AppUser user)
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
            new (ClaimTypes.Email, user.Email), // thats a modern way of creating the object new Claim(ClaimTypes.Email,..)
            new (ClaimTypes.NameIdentifier, user.Id),
            // new ("CustomWhatevet", "CustomThing") //this is also fine for custom claim
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature); // here we are signing the token

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            //Specify the properties that our token needs
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = creds
        };

        var tokenHandler = new JwtSecurityTokenHandler();

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}

/**
typically we create a service that does not need to access our database, or it is the third party serviec that we are using, the token service don't need to access our database we are going to pass the user object to the method for create token 
**/
