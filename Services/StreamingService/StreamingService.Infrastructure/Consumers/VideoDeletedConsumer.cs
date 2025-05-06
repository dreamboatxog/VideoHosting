using System;
using MassTransit;
using Shared.Messages;
using StreamingService.Application.Interfaces;

namespace StreamingService.Infrastructure.Consumers;

public class VideoDeletedConsumer(IStreamingProvider _streamingProvider) : IConsumer<VideoDeleted>
{
    public async Task Consume(ConsumeContext<VideoDeleted> context)
    {
        await _streamingProvider.RemoveAsync(context.Message.Id);
    }

}
