using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Parcial2.Data;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT key (≥32 bytes) y una sola instancia
var keyStr = builder.Configuration["Jwt:Key"] ?? throw new Exception("Falta Jwt:Key");
if (Encoding.UTF8.GetByteCount(keyStr) < 32) throw new Exception("Jwt:Key ≥ 32 bytes");
var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr));
builder.Services.AddSingleton<SecurityKey>(signingKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });

builder.Services.AddScoped<TokenService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Parcial2 API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement {
        { new OpenApiSecurityScheme{ Reference = new OpenApiReference{ Type = ReferenceType.SecurityScheme, Id="Bearer"} }, new string[]{} }
    });
});

builder.Services.AddCors(o => o.AddPolicy("AllowAll",
    p => p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
