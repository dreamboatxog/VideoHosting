using StreamingService.Infrastructure;
using Microsoft.EntityFrameworkCore;
using StreamingService.Application.Interfaces;
using StreamingService.Infrastructure.Repositories;
using StreamingService.Application.Services;
using MassTransit;
using Shared.Messages;
using StreamingService.Infrastructure.Consumers;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<StreamDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IStreamRepository, StreamRepository>();
builder.Services.AddScoped<IStreamingProvider, HlsStreamingService>();
builder.Services.AddScoped<IConsumer<HlsGenerated>,HlsGeneratedConsumer>();


builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<IConsumer<HlsGenerated>>();

    x.UsingRabbitMq((context, cfg) =>
       {
           cfg.Host(builder.Configuration["RabbitMQ:Host"], "/", h =>
           {
               h.Username(builder.Configuration["RabbitMQ:Username"]);
               h.Password(builder.Configuration["RabbitMQ:Password"]);
           });
           cfg.ReceiveEndpoint("hls-generated-queue", e =>
           {
               e.ConfigureConsumer<IConsumer<HlsGenerated>>(context);
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

/* builder.Services
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
    }); */
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
