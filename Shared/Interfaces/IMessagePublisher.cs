using System;

namespace Shared.Interfaces;

public interface IMessagePublisher
{
    Task Publish<T>(T message) where T : class;
}
