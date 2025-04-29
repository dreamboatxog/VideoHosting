using MassTransit;
using Shared.Interfaces;
using ProcessingService.Application.Interfaces;
using ProcessingService.Application.Services;
using ProcessingService.Infrstructure.Consumers;
using ProcessingService.Infrstructure.Publishers;
using ProcessingService.Infrstructure.Storage;
using ProcessingService.Application.Services.ProcessingSteps;
using NReco.VideoConverter;

var builder = Host.CreateDefaultBuilder();
builder.ConfigureServices((hostContext, services) =>
{
    services.AddMassTransit(x =>
    {

        x.AddConsumer<VideoCreatedConsumer>();


        x.UsingRabbitMq((context, cfg) =>
        {
            var configuration = context.GetService<IConfiguration>();
            cfg.Host(hostContext.Configuration["RabbitMQ:Host"], "/", h =>
            {
                h.Username(hostContext.Configuration["RabbitMQ:Username"]);
                h.Password(hostContext.Configuration["RabbitMQ:Password"]);
            });


            cfg.ReceiveEndpoint("video-processing-queue", e =>
            {
                e.ConfigureConsumer<VideoCreatedConsumer>(context);
            });
            cfg.ConfigureEndpoints(context);
        });
    });
    services.AddScoped<IVideoWorkflowOrchestrator, VideoWorkflowOrchestrator>();
    services.AddScoped<IVideoProcessingStep, HlsGenerationStep>();
    services.AddScoped<IVideoProcessingStep, ThumbnailGenerationStep>();
    services.AddSingleton<FFMpegConverter>();
    services.AddScoped<IFileStorageInterface, LocalFileStorage>();
    services.AddScoped<IMessagePublisher, MessagePublisher>();
});
var app = builder.Build();
app.Run();