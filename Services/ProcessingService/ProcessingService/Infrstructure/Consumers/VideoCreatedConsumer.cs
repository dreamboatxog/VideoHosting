using MassTransit;
using Shared.Messages;
using ProcessingService.Application.Interfaces;

namespace ProcessingService.Infrstructure.Consumers;

public class VideoCreatedConsumer(IVideoWorkflowOrchestrator _videoWorkflow) : IConsumer<VideoCreated>
{
    public async Task Consume(ConsumeContext<VideoCreated> context)
    {
        System.Console.WriteLine(context.Message.FilePath);
       await _videoWorkflow.ProcessVideoAsync(context.Message.FilePath, context.Message.Id);
    }
}
