using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using API.Middleware;
using API.Helpers;
using API.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.Any Services that we add here are made avaiable for dependency injection to other classes that we use

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(opt =>
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddCors();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IMemberRepository, MemberRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<ILikesRepository, LikesRepository>();
builder.Services.AddScoped<LogUserActivity>(); // we need to add this as the scoped service so that we can use it in the action filter and we are going to use it in our BaseApiController so that it will be applied to all the controllers that are inheriting from BaseApiController.
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));

//Configuring the ASP.NET Identity
builder.Services.AddIdentityCore<AppUser>(opt =>
{
    opt.Password.RequireNonAlphanumeric = false;
    opt.User.RequireUniqueEmail = true;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var tokenKey = builder.Configuration["TokenKey"] ?? throw new Exception("Token Key Not Found --program.cs");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            //options how we want to validate this token
            ValidateIssuerSigningKey = true, //make sure the token is valid when our API server receives it
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey)),
            ValidateIssuer = false, // we only accept token issued by certain issuer
            ValidateAudience = false // specify who the audiance is.
        };
    });
/** we have three life times for the services.
    builder.Services.AddSingleton() 
        here singleton means that when our application will start up, this will create an instance of our service and will keep it running for as long as the application is alive, so for TokenService we don't need the life time scope, we only need this when the user loging in and when the user do log in we issue the user with the token and we don't need that service again until somebody else logs in, so we don't need that as singleton 
    builder.Services.AddTransient()
        AddTransient will create new instance of our token service for every single request, realistically this is considered to be very short for our purposes.
    builder.Services.AddScoped()
        This is the one that we typically use for our service. This Scoped means that it will be scoped to the level of the HTTP request. So our request comes into our account controller we gonna inject TokenService into our account controller and will be created when the request comes in and will be disposed off when the HTTP request is finised.
    Now in order to use the Interface class (ITokenService) with the implementation class (TokenService) we need to specify ITokenService as the first type parameter in the scope and the TokenService as the second type parameter.

    We gonna inject not the implementation class but the interface class when we use it in the account controller and because we have registered in this way builder.Services.AddScoped<ITokenService, TokenService>(); then the framework knows that when we are using this ITokenService then its implementation class is going to be this (TokenService) one so create the instance of TokenService when we need to create a Token.
**/

var app = builder.Build();
//Middleware comes on the top of the request pipeline
app.UseMiddleware<ExceptionMiddleware>();


// Configure the HTTP request pipeline, section under this is the middleware section, ordering in this section is important if we put the cors command after the run command that it will not be executed
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200","https://localhost:4200"));

app.UseAuthentication(); //ordering is important
app.UseAuthorization();
app.MapControllers();

// we can not have the dependency injection here, but we can hold of our AppDbContext using the pattern and is referred to as a service locator pattern

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<AppDbContext>();
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    await context.Database.MigrateAsync();
    await Seed.SeedUsers(userManager);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occured during the migrations");
}

app.Run();
