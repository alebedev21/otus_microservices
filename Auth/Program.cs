using API.Services;
using Auth.ConfigOptions;
using Auth.Contexts;
using Auth.DTO.Income;
using Auth.Repositories;
using Auth.Services;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5086); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IAuthService, AuthService>();
builder.Services.AddSingleton<IAuthRepository, AuthRepository>();

builder.Services.AddOptions<PrivateKeyOptions>().BindConfiguration("PrivateKeyOptions");
builder.Services.AddSingleton<IPrivateKeyRepository, PrivateKeyRepository>();

builder.Services.AddOptions<JwtServiceOptions>().BindConfiguration("JwtServiceOptions");
builder.Services.AddSingleton<IJwtService, JwtService>();

builder.Services.AddOptions<PostgresOptions>().BindConfiguration("PostgresOptions");
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql();
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();

app.MapPost("/register", async ([Required]RegistrationRequest? request, IAuthService service) =>
{
    try
    {
        var userId = await service.RegisterUser(request!);
        return Results.Ok(new { UserId = userId });
    }
    catch (ArgumentOutOfRangeException)
    {
        return Results.Conflict();
    }
});

app.MapPost("/login", async ([Required] RegistrationRequest? request, IAuthService service) =>
{
    try
    {
        var tokens = await service.LoginUser(request!);
        return Results.Ok(tokens);
    }
    catch (UnauthorizedAccessException)
    {
        return Results.Unauthorized();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
});

app.Run();