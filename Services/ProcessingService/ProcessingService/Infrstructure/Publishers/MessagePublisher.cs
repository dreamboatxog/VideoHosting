using System;
using MassTransit;
using Shared.Interfaces;
using Shared.Messages;

namespace ProcessingService.Infrstructure.Publishers;

public class MessagePublisher(IPublishEndpoint _publishEndpoint) : IMessagePublisher
{
    public async Task Publish<T>(T message) where T: class
    {
        var json = System.Text.Json.JsonSerializer.Serialize(message);
        Console.WriteLine($"Serialized JSON: {json}");
        await _publishEndpoint.Publish<T>(message);
        Console.WriteLine("Message published successfully");
    }
}
