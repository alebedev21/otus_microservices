using Gateway.Authentication;
using Gateway.ConfigOptions;
using Gateway.DTO.Income;
using Gateway.Helpers;
using Gateway.Repositories;
using Gateway.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.ConfigureKestrel(options => { options.ListenAnyIP(5168); });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc(
    AssemblyInfo.AssemblyName,
    new OpenApiInfo
    {
        Title = $"{AssemblyInfo.ProgramNameVersion} manual",
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{AssemblyInfo.AssemblyName}.xml"), true);

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Authorization: Bearer JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer",
                },
            },
            Array.Empty<string>()
        },
    });

    options.SupportNonNullableReferenceTypes();
});

builder.Services.AddOptions<ApiPointsOptions>().BindConfiguration("ApiPointsOptions");

builder.Services.AddOptions<PublicKeyOptions>().BindConfiguration("PublicKeyOptions");
builder.Services.AddSingleton<IPublicKeyRepository, PublicKeyRepository>();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

builder.Services.AddSingleton<IPostConfigureOptions<JwtBearerOptions>, TokenValidatorPostConfigure>();
builder.Services.AddScoped<JwtBearerEventsHandler>();

builder.Services.AddAuthentication().AddJwtBearer(o =>
{
    o.IncludeErrorDetails = true;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = false,
        ValidateLifetime = true,
        ValidAudience = "otus_microservices_aud",
        ClockSkew = TimeSpan.Zero,
    };

    o.EventsType = typeof(JwtBearerEventsHandler);
});

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint($"./{AssemblyInfo.AssemblyName}/swagger.json", AssemblyInfo.AssemblyName);
    options.DocumentTitle = $"{AssemblyInfo.ProgramNameVersion} manual";
});

app.UseDeveloperExceptionPage();

app.UseAuthentication();
app.UseAuthorization();

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

app.MapPost("/user", async (UserAddRequest request, IUserService service) =>
{
    var response = await service.Add(request);

    return Results.Created();
}).RequireAuthorization();

app.Run();
