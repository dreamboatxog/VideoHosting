using Microsoft.EntityFrameworkCore;
using VideoService.Infrastructure;
using VideoService.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MassTransit;
using VideoService.Infrastructure.Services;
using VideoService.Infrastructure.Repositories;
using VideoService.Infrastructure.Publishers;
using Shared.Messages;
using VideoService.Infrastructure.Consumers;
using Shared.Interfaces;
var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<VideoDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IVideoRepository, VideoRepository>();
builder.Services.AddScoped<IFileStorageService, LocalStorageService>();
builder.Services.AddScoped<IVideoService, VideoService.Application.Services.VideoService>();
builder.Services.AddScoped<IMessagePublisher, MessagePublisher>();
builder.Services.AddScoped<IConsumer<VideoProcessed>,VideoProcessedConsumer>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:Configuration"];
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<IConsumer<VideoProcessed>>();

    x.UsingRabbitMq((context, cfg) =>
       {
           cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
           {
               h.Username(builder.Configuration["RabbitMQ:Username"]);
               h.Password(builder.Configuration["RabbitMQ:Password"]);
           });
           cfg.ReceiveEndpoint("video-processed-queue", e =>
           {
               e.ConfigureConsumer<IConsumer<VideoProcessed>>(context);
           });

           cfg.ConfigureEndpoints(context);
       });
});


builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "VideoService API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

var jwtSecretKey = builder.Configuration["Jwt:SecretKey"];

if(string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(jwtSecretKey))
{
    throw new Exception("JWT configuration is missing in appsettings.json");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,

            ValidateAudience = true,
            ValidAudience =jwtAudience,

            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecretKey)
            )
        };
    });
builder.Services.AddAuthorization();

var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.MapControllers();
app.Run();
