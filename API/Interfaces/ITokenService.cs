using API.Entities;

namespace API.Interfaces;

public interface ITokenService
{
    string CreateToken(AppUser user);
}

/**
You can think of the interface as the contract between the interface class and the class that is implementing this interface.

Now what the interface system provides for us, it provides an abstraction away from what the service actually does, so out token service we can say this by just looking at the code that it is gonna create the token and return a string but we can't tell what logic is inside the implementation class to create the token nor do we can, the idea of the interface is we have abstracted away from the implementation, so how this token is created is not important. What we know when we use the ITokenService if we use the create token method we just need to pass it the user and it will returns us a string and based on the name of the method we can assume that it will be a token.

We have a TokenService class that uses this interface but I can create the other TokenService or other services to create the token using the ITokenService that have different implementation logic but they use the same method take the same parameter and return a string, and when we use this ITokenService because it acts like a contract it ensures that all the implementation class that use this interface will offer the same operation. So we get consistency and reliablity using this interface system.

But probably using this interface system make our code more testable, when testing token service you don't need the logic of create token you can simply mock the ITokenService

**/
