using MassTransit;
using Shared.Interfaces;
using Shared.Messages;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Publishers;

public class MessagePublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public async Task Publish<T>(T message)  where T:class
    {
        var json = System.Text.Json.JsonSerializer.Serialize(typeof(T).Name);
        Console.WriteLine($"Serialized JSON: {json}");
        await publishEndpoint.Publish<T>(message);
        Console.WriteLine("Message published successfully");
    }
}
