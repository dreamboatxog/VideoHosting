using MassTransit;
using Shared.Messages;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Consumers;

public class VideoProcessedConsumer(IVideoService _videoService) : IConsumer<VideoProcessed> 
{
    public async Task Consume(ConsumeContext<VideoProcessed> context)
    {
        await _videoService.ProcessVideoAsync(context.Message);
    }
}
