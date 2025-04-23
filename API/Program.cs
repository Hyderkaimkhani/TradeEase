using Domain.Config;
using Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Repositories.Config;
using Repositories.Context;
using Serilog;
using Services.Config;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using Api.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);


string logDirectory = builder.Configuration["LogDirectory"] ?? "logs";
SymmetricSecurityKey signingKey;
TokenProviderOptions userTokenOptions;
TokenValidationParameters tokenValidationParameters;

builder.Host.UseSerilog((ctx, lc) => lc
.ReadFrom.Configuration(ctx.Configuration)
.WriteTo.Console()
    .WriteTo.File($"{logDirectory}/TradeEase-api.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 14,
        buffered: true,
        flushToDiskInterval: TimeSpan.FromSeconds(30))
);


builder.Services.AddDbContext<Context>(
        options =>
        {
            //options.ConfigureWarnings(warnings => warnings.Throw(RelationalEventId.MultipleCollectionIncludeWarning));
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection") ??
                throw new ArgumentException("DefaultConnection not provided"),
                optionsBuilder =>
                {
                    optionsBuilder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
                    optionsBuilder.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
        });


// Add services to the container.
RegisterServices.RegisterComponents(builder.Services, builder.Configuration);
RegisterDALRepositories.RegisterComponents(builder.Services, builder.Configuration);
RegisterDomainServices.RegisterComponents(builder.Services, builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

JsonSerializerOptions opts = new JsonSerializerOptions();
opts.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
opts.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
opts.PreferredObjectCreationHandling = JsonObjectCreationHandling.Populate;
opts.PropertyNameCaseInsensitive = true;

#region Token 
signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["TokenProviderOptions:UserAuthSecretKey"]));

userTokenOptions = new TokenProviderOptions
{
    Audience = builder.Configuration["TokenProviderOptions:TokenAudience"],
    Issuer = builder.Configuration["TokenProviderOptions:TokenIssuer"],
    SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256),
};

IdentityModelEventSource.ShowPII = true;

//All validations are closed for development purposes
tokenValidationParameters = new TokenValidationParameters
{
    // The signing key must match!
    ValidateIssuerSigningKey = true,
    //IssuerSigningKey = signingKey,

    // Validate the JWT Issuer (iss) claim
    ValidateIssuer = true,
    //ValidIssuer = "Silicon",

    // Validate the JWT Audience (aud) claim
    ValidateAudience = true,
    //ValidAudience = "Eticket",

    // Validate the token expiry
    ValidateLifetime = true,

    //Require signed token
    RequireSignedTokens = false,

    //Require Expiraion Time
    RequireExpirationTime = false,

    SignatureValidator = delegate (string token, TokenValidationParameters parameters)
    {
        var jwt = new JwtSecurityToken(token);

        return jwt;
    },

    // If you want to allow a certain amount of clock drift, set that here:
    ClockSkew = TimeSpan.Zero,
    ValidAudience = builder.Configuration["TokenProviderOptions:TokenAudience"],
    ValidIssuer = builder.Configuration["TokenProviderOptions:TokenIssuer"],
    IssuerSigningKey = signingKey,

};

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = tokenValidationParameters;
    var validator = options.SecurityTokenValidators.OfType<JwtSecurityTokenHandler>().SingleOrDefault();
    validator.InboundClaimTypeMap.Clear();
});

#endregion

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ExceptionHandleMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapControllers();

//app.MapHealthChecks("/api-health", new HealthCheckOptions
//{
//    ResponseWriter = HealthReporting.WriteResponse
//});


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
