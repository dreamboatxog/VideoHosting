using Shared.Messages;
using StreamingService.Application.Interfaces;
using StreamingService.Domain.Entities;
using Stream = StreamingService.Domain.Entities.Stream;

namespace StreamingService.Application.Services;

public class HlsStreamingService(IStreamRepository _repository) : IStreamingProvider
{
    const string PLAYLIST_CONTENT_TYPE = "application/vnd.apple.mpegurl";
    const string SEGMENT_CONTENT_TYPE = "video/MP2T";
    public async Task AddAsync(HlsGenerated hlsMessage)
    {
        Stream stream = new Stream
        {
            Id = hlsMessage.VideoId,
            HlsPath = hlsMessage.HlsPath
        };
        var streamAdded = await _repository.AddAsync(stream);
        if (!streamAdded)
        {
            throw new Exception("Failed to add stream to the repository.");
        }
    }

    public async Task<(string Path, string ContentType)?> GetPlaylistPathAsync(Guid videoId)
    {  
        var stream = await _repository.GetByIdAsync(videoId);
        if (stream == null || string.IsNullOrWhiteSpace(stream.HlsPath))
            return null;
        string playlistPath = Path.Combine(stream.HlsPath, stream.Id.ToString() + ".m3u8");
        return (playlistPath, PLAYLIST_CONTENT_TYPE);
    }

    public async Task<(string Path, string ContentType)?> GetSegmentPathAsync(Guid videoId, string segment)
    {
        var stream = await _repository.GetByIdAsync(videoId);
        if (stream == null || string.IsNullOrWhiteSpace(stream.HlsPath))
            return null;
        string segmentPath = Path.Combine(stream.HlsPath, stream.Id.ToString() +segment);
        return (segmentPath, SEGMENT_CONTENT_TYPE);
    }

    public async Task<bool> RemoveAsync(Guid videoId)
    {
        throw new NotImplementedException();
    }

}
