using System;
using Shared.Messages;

namespace StreamingService.Application.Interfaces;

public interface IStreamingProvider
{
    Task<(string Path, string ContentType)?> GetStreamAsync(Guid videoId, string? relativePath);
    Task AddAsync(HlsGenerated hlsMessage);
    Task<bool> RemoveAsync(Guid videoId);
}
