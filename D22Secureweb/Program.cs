using D22Secureweb;
using D22Secureweb.Model;
using D22Secureweb.Repositories;
using D22Secureweb.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

#region add Authentication and Authorization services
const String AuthScheme = "token";

builder.Services.AddAuthentication(AuthScheme)
    .AddCookie(AuthScheme, Options =>
    {
        Options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        Options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(builder =>
{
    builder.AddPolicy("user", pb =>
    {
        pb.RequireAuthenticatedUser()
        .AddAuthenticationSchemes(AuthScheme)
        .AddRequirements()
        .RequireClaim("user_type", "standard");
    });
});

//Test overwritting default authorization behaviour in the Middleware
//builder.Services.AddSingleton<
//    IAuthorizationMiddlewareResultHandler, AuthorizationMiddlewareResultHandlerImp>();

#endregion

#region add DI services
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<D22RestDatabase>(
    builder.Configuration.GetSection("D22RestDatabaseSettings"));

builder.Services.AddSingleton<IClassifiedRepository, ClassifiedRepository>();
builder.Services.AddSingleton<IUserRepository, UserRepository>();
#endregion

#region Add app middleware
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
#endregion

#region login endpoints
app.MapPost("/login", async (User user, IUserRepository iur, HttpContext ctx) =>
{
    if (await iur.Authenticate(user))
    {
        var claims = new List<Claim>();
        claims.Add(new Claim("user_type", "standard"));
        var identity = new ClaimsIdentity(claims, AuthScheme);
        var userIdentity = new ClaimsPrincipal(identity);
        

        await ctx.SignInAsync(AuthScheme, userIdentity);
        return Results.Ok("Login success");
    }
    return Results.Unauthorized();

}).AllowAnonymous();

app.MapGet("/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync();
    return Results.Ok("Logout success");

}).RequireAuthorization("user");

#endregion login

#region define domain endpoints
app.MapPost("/classified", async (Classified cls, IClassifiedRepository icr) =>
{
    await icr.Add(cls);
    return Results.Created($"/classified/{cls.Id}", cls);
}).RequireAuthorization("user"); 

app.MapGet("/classified/{classifiedID}", async (int classifiedID, IClassifiedRepository icr) =>
{
    var res = await icr.Get(classifiedID);
    return res!=null?Results.Ok(res):Results.NotFound();
});//defaults to AllowAnonymous

app.MapGet("/classified", async (IClassifiedRepository icr) =>
{
    return await icr.GetAll();

}).RequireAuthorization("user");


app.MapDelete("/classified/{classifiedID}", async (int classifiedID, IClassifiedRepository icr) =>
{
    var res = await icr.Delete(classifiedID);
    return res?Results.Ok():Results.NotFound();

}).RequireAuthorization("admin");
#endregion


app.Run();

