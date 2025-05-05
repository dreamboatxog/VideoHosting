using System;
using Shared.Messages;

namespace StreamingService.Application.Interfaces;

public interface IStreamingProvider
{
    Task<(string Path, string ContentType)?> GetPlaylistPathAsync(Guid videoId);
    Task AddAsync(HlsGenerated hlsMessage);
    Task<bool> RemoveAsync(Guid videoId);
    Task<(string Path, string ContentType)?> GetSegmentPathAsync(Guid videoId, string segment);
}
