using Gateway.ConfigOptions;
using Gateway.DTO.Income;
using Gateway.Services;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5168); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddOptions<ApiPointsOptions>().BindConfiguration("ApiPointsOptions");

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDeveloperExceptionPage();

app.MapPost("/register", async ([Required] RegistrationRequest? request, IAuthService service) =>
{
    try
    {
        await service.RegisterUser(request!);
        return Results.Ok();
    }
    catch (ArgumentOutOfRangeException)
    {
        return Results.Conflict();
    }
});

app.MapPost("/user", async (UserAddRequest request, IUserService service) =>
{
    var response = await service.Add(request);

    return Results.Created();
});

app.Run();
