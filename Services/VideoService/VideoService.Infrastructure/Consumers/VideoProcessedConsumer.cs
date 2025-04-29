using System;
using MassTransit;
using Shared.Messages;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Consumers;

public class VideoProcessedConsumer(IVideoService _videoService) : IConsumer<VideoProcessed> 
{
    public async Task Consume(ConsumeContext<VideoProcessed> context)
    {
        Console.WriteLine("Message recieved\n"+context.Message.HLSPath);
        await _videoService.ProcessVideoAsync(context.Message);
    }
}
