using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
// .AddJwtBearer(options =>
// {
//     options.TokenValidationParameters = new TokenValidationParameters
//     {
//         ValidateIssuer = true,
//         ValidateAudience = true,
//         ValidateLifetime = true,
//         ValidateIssuerSigningKey = true,
//         ValidIssuer = "https://auth.snowse.duckdns.org/realms/advanced-frontend",
//         ValidAudience = "garion-auth-class",
//         // IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_secret_key"))
//         IssuerSigningKeyResolver = (token, securityToken, identifier, parameters) =>
//         {
//             var httpClient = new HttpClient();
//             var json = httpClient.GetStringAsync("https://auth.snowse.duckdns.org/realms/advanced-frontend/.well-known/openid-configuration").Result;

//             var jwksUri = JsonConvert.DeserializeObject<dynamic>(json)!.jwks_uri ?? "https://auth.snowse.duckdns.org/realms/advanced-frontend/protocol/openid-connect/certs";
//             var jwksJson = httpClient.GetStringAsync(jwksUri).Result;

//             var jwks = JsonConvert.DeserializeObject<JsonWebKeySet>(jwksJson);
//             return jwks.Keys;
//         }
//     };
// });
.AddJwtBearer(options =>
{
    options.Audience = "garion-auth-class";
    options.Authority = "https://auth.snowse.duckdns.org/realms/advanced-frontend/protocol/openid-connect/certs";
});

var app = builder.Build();

app.MapGet("/authOnly", (HttpRequest request, ClaimsPrincipal user) =>
{
    var id_token = request.Headers["Authorization"].ToString().Replace("Bearer ", "").Trim();

    Console.WriteLine("In Auth Endpoint, using key " + id_token);

    if (user.Identity?.IsAuthenticated == true)
    {
        Console.WriteLine($"Authenticated user: {user.Identity.Name}");
        return $"Authenticated user: {user.Identity.Name}";
    }

    Console.WriteLine("User not authenticated");
    return "User not authenticated";
});



app.MapGet("/public", () =>
{
    // Console.WriteLine("In Public Endpoint");
    return "healthy";
}).AllowAnonymous();

app.UseCors(
    p => p
     .AllowAnyHeader()
     .AllowAnyMethod()
     .AllowAnyOrigin());

app.Run();
