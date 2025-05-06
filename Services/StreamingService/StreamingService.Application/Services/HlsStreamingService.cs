using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Messages;
using StreamingService.Application.Interfaces;
using Stream = StreamingService.Domain.Entities.Stream;

namespace StreamingService.Application.Services;

public class HlsStreamingService(IStreamRepository _repository,  IDistributedCache _cache) : IStreamingProvider
{
    private record StreamCache(string FullPath, string ContentType);
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

    public async Task<(string Path, string ContentType)?> GetStreamAsync(Guid videoId, string? relativePath)
    {
         var cachedValue = await _cache.GetStringAsync(videoId.ToString() + relativePath ?? "");
        if (cachedValue != null)
        {
            var value = JsonSerializer.Deserialize<StreamCache>(cachedValue);
            System.Console.WriteLine("Cache hit for videoId: " + videoId.ToString() + relativePath ?? "");
            return (value!.FullPath, value.ContentType);
        }
        var stream = await _repository.GetByIdAsync(videoId);
        if (stream == null || string.IsNullOrWhiteSpace(stream.HlsPath))
            return null;

        string fullPath;

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            fullPath = stream.HlsPath;
        }
        else
        {
            var videoFolder = Path.GetDirectoryName(stream.HlsPath);
            if (string.IsNullOrWhiteSpace(videoFolder))
            {
                return null;
            }

            fullPath = Path.Combine(videoFolder, relativePath);
        }

        if (!File.Exists(fullPath))
            return null;

        var ext = Path.GetExtension(fullPath).ToLowerInvariant();
        var contentType = ext switch
        {
            ".m3u8" => PLAYLIST_CONTENT_TYPE,
            ".ts" => SEGMENT_CONTENT_TYPE,
            _ => "unknown"
        };

        var cachedData = new StreamCache(fullPath,contentType);
        var cachedJson = JsonSerializer.Serialize(cachedData);
        await _cache.SetStringAsync(videoId.ToString() + relativePath ?? "", cachedJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });
        return (fullPath, contentType);
    }

    public async Task<bool> RemoveAsync(Guid videoId)
    {
        return await _repository.DeleteAsync(videoId);
    }

}
