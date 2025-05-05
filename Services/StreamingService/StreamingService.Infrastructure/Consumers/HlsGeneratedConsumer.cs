using System;
using MassTransit;
using Shared.Messages;
using StreamingService.Application.Interfaces;

namespace StreamingService.Infrastructure.Consumers;

public class HlsGeneratedConsumer(IStreamingProvider _streamingProvider) : IConsumer<HlsGenerated>
{
    public async Task Consume(ConsumeContext<HlsGenerated> context)
    {
        await _streamingProvider.AddAsync(context.Message);
    }
}
