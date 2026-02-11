using System;
using System.Security.Cryptography;
using System.Text;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AccountController(UserManager<AppUser> userManager, ITokenService tokenService) : BaseApiController
{
    [HttpPost("register")] // api/account/register
    public async Task<ActionResult<UserDto>> Register(RegisterDTO registerDTO)
    {
        
        var user = new AppUser
        {
            DisplayName = registerDTO.DisplayName,
            Email = registerDTO.Email,
            UserName = registerDTO.Email,
            Member = new Member
            {
                DisplayName = registerDTO.DisplayName,
                Gender = registerDTO.Gender,
                City = registerDTO.City,
                Country = registerDTO.Country,
                DateOfBirth = registerDTO.DateOfBirth
            }

        };

        var result = await userManager.CreateAsync(user, registerDTO.Password);

        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }
            return ValidationProblem();
        }

        await userManager.AddToRoleAsync(user, "Member");

        await SetRefreshTokenCookie(user);

        //extension method toDto takes two parameters but we are passing only token service because this refers to the user here to which its extending in the todto method.
        return await user.ToDto(tokenService);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
    {
        var user = await userManager.FindByEmailAsync(loginDto.Email);

        if (user == null)
        {
            return Unauthorized("Invalid Email Address");
        }

        var result = await userManager.CheckPasswordAsync(user, loginDto.Password);

        if (!result)
        {
            return Unauthorized("Invalid Password");
        }

        await SetRefreshTokenCookie(user); // when we use cookies like this we need to configure our CORS policies

        return await user.ToDto(tokenService);
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        var refreshToken = Request.Cookies["refreshToken"];

        if (refreshToken == null) return NoContent();

        var user = await userManager.Users
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
                && x.RefreshTokenExpiry > DateTime.UtcNow);

        if (user == null)
        {
            return Unauthorized();
        }

        await SetRefreshTokenCookie(user);

        return await user.ToDto(tokenService);
    }

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true, //this means this cookie is not accessible from any kind of JavaScript incluing our own client application
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }

}

//notes 

/**
1: when we inject something in the class that means its a framework job to create the instance of injected class when it needs to (for now its AppDbContext) and its the frameworks job to dispose that off when our AccountController is out of the scope, as the HTTP request has completed, the account controller is no longer needed, the context is no longer needed then framework will dispose that off and will free up the memory. 

2: When it comes to creating the instance of class using new keyword inside a method of a class then the framework is not going to immediately dispose that off, there is the process in .NET called the garbage collection that will come in the unusual time and will dispose that off, but we can control this, and certain classes implement an interface called IDisposeable (For now interface is just the contract between the class and the interface)

3: if we check the parent of the parent ... of the HMACSHA512 class there will be interface implemented as IDisposeable, and this shows that we can specify the scope of that how long this gonna be used before it is disposed, so to make this out of the scope when the AccountController is out of the scope we need to add the Using keyword before the var in var hmac = new HMACSHA512(); as using var hmac = new HMACSHA512()

4: if we hit this from postman and put this email, username, password in body it will give us error, becuase the strings by convension are read by the query string in .NET, to send in the body we will need to create the object for that

5: so we are using the DTO, data transferable object for sending email, name and password, DTO is the term used often in the coding when we want to transfer data between one layer and other layer, and by passing in DTO we can use the body of the request, but along using the DTO if you want to send as query string as before then use [FromQuery]DTO as method parameter. and similar we can use [FromBody]parameter name with each parameter to get from body.

6: It is often asked that why we don't use the aysnc version of this "context.Add(user);", at this stage we are tracking chnages in the memory, we are not making the call to the database on this line, that's the next line we are making the call to the database, so that is the reason we don't use the async version of this there is the special use case in which we use the async version of this and that is when you have to pre generate a number from the sql server that's the very specific case.

7: context.Users.SingleOrDefault(user => user.Email == loginDto.Email); this might give the error when there can be duplicate email in the database.

8: JSON Web Tokens
    How we authenticate to an API?
    API is not something that we maintain the session state with, we simply make an API request and it returns the data to us and then our relationship is finished with the API until we need to make another request.

    Now tokens are good thing to use with an API, becuase they are small enough to send with every single request.

    JSON web token are inudstry standard web tokens

    Self-Contained and can contain:

    1: Credentials
    2: Claims
    3: Other Information

    They are basically the long string, separated into three parts each part is separated by a period, now in the first part there is the header of the token it contains the algorithm and the type of the token that it is, now the algorithm is what used to encrypt the signature in the third part of the token, the second part in this token is the payload and this is where we contain the information about our claims and our credentials so we can have things like name identifier, user name, roles the users in and any other claims that users have about themselves. When we say claim that means the user is claiming to be something.

    we have three time stamps in the token as well in the second part nbf (can not be used before a certain date and time), exp, iat (issued at)

    and the third part is where the signature is contained, this signature is encrypted by the server it self using the secure key that never ever leaves a server and the only part of the token that is encrypted is the signature, every other thing is eaisly obtained simply by decoding the token which we don't need to do anything clever with it, is very simple to get information get out of the token. What we can not do is to modify the token in any way and expect our API to accept it because that will change the entire structure of the token, and the signature will not be verified.

    Lets see how the token authentication works
        So lets suppose the user logs in and sends the username and the password to the server, the server will validate that request and will return the JSON web token, that the client will store locally on their machine, we often use the browser storage to hold on the token so that we can send the JSON web token with every single request, so every time we need to access anything on the server that is protected by the authentication we send the JSON web token with that request.
        Now what we do with this token we add an authentication header to the request and then the server will take a look at the token and verify the token is valid, now the server that has signed the token has the access to the private key that is stored on the server and the server is able to verify that the token is valid, without needing to make a call to the database, and in the final part server says yes this token is okay and sends back the response.

    Benifits of using the JWT 
        No session to manage. All JWT are self contained tokens. We just send the token with every request and because they are very small and light weight that doesn't add much to the request that we send over.
        
        The are portable. A single token can be used with multiple backends, so as long as the backends shares the same signature, the same secret key then they all can verify that token is valid.

        No cookies are required, that means JWT are mobile friendly

        Performance, once the token is issued, there is no need to make a databse request to verify the user is authenticated. 
**/
