using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Prometheus.Users.Api.Data;
using Prometheus.Users.Api.Security;
using Prometheus.Users.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Prometheus Users API",
        Version = "v1",
        Description = "Implementação referencial do contexto Users/Identidade."
    });

    var bearerScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Informe o JWT no formato: Bearer {token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };

    options.AddSecurityDefinition("Bearer", bearerScheme);
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        [bearerScheme] = Array.Empty<string>()
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("PortalWeb", policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

builder.Services.AddDbContext<AppDbContext>((services, options) =>
{
    var configuration = services.GetRequiredService<IConfiguration>();
    var connectionString = configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=prometheus-auth.db";
    options.UseSqlite(connectionString);
});

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();

builder.Services
    .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
    .Configure<Microsoft.Extensions.Options.IOptions<JwtOptions>>((options, jwtOptionsAccessor) =>
    {
        var jwtOptions = jwtOptionsAccessor.Value;
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtOptions.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = "role"
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddSingleton<JwtService>();

var app = builder.Build();

app.UseExceptionHandler();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors("PortalWeb");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

var seedDemonstrationAdmin =
    app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Testing");
await DatabaseInitializer.InitializeAsync(app.Services, seedDemonstrationAdmin);

app.Run();

public partial class Program;
