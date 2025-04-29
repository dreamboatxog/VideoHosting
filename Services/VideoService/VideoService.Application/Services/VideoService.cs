using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Shared.Interfaces;
using Shared.Messages;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;


namespace VideoService.Application.Services;

public class VideoService(IVideoRepository _repository, IFileStorageService _storageService, IMessagePublisher _messagePublisher, IDistributedCache _cache) : IVideoService
{
    private record StreamCache(string FullPath, string ContentType);

    public async Task<bool> DeleteVideoAsync(Guid id)
    {
        var video = await _repository.GetByIdAsync(id);
        if (video == null)
            return false;
        if (!string.IsNullOrEmpty(video.FilePath))
            await _storageService.DeleteAsync(video.FilePath);
        return await _repository.DeleteAsync(video);
    }



    public async Task<Video?> GetVideoByIdAsync(Guid id)
    {
        var video = await _repository.GetByIdAsync(id);
        if (video == null)
            return null;
        return video;
    }

    public async Task<(string, string)?> GetStreamByIdAsync(Guid id, string? relativePath)
    {
        var cachedValue = await _cache.GetStringAsync(id.ToString() + relativePath ?? "");
        if (cachedValue != null)
        {
            var value = JsonSerializer.Deserialize<StreamCache>(cachedValue);
            return (value.FullPath, value.ContentType);
        }

        var video = await _repository.GetByIdAsync(id);
        if (video == null || string.IsNullOrWhiteSpace(video.HLSPath))
            return null;

        string fullPath;

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            fullPath = video.HLSPath;
        }
        else
        {
            var videoFolder = Path.GetDirectoryName(video.HLSPath);
            if (string.IsNullOrWhiteSpace(videoFolder))
            {
                return null;
            }

            fullPath = Path.Combine(videoFolder, relativePath);
        }

        if (!System.IO.File.Exists(fullPath))
            return null;

        var ext = Path.GetExtension(fullPath).ToLowerInvariant();
        var contentType = ext switch
        {
            ".m3u8" => "application/vnd.apple.mpegurl",
            ".ts" => "video/MP2T",
            _ => "application/octet-stream"
        };

        var cacheData = new StreamCache(fullPath,contentType);
        var cacheJson = JsonSerializer.Serialize(cacheData);
        await _cache.SetStringAsync(id.ToString() + relativePath ?? "", cacheJson, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
        });

        return (fullPath, contentType);
    }

    public async Task<bool> ProcessVideoAsync(VideoProcessed processedVideo)
    {
        var video = await _repository.GetByIdAsync(processedVideo.Id);
        if (video == null)
            return false;
        if (!processedVideo.Status)
        {
            video.Status = VideoStatus.Failed;
            return await _repository.UpdateAsync(video);
        }
        video.Status = VideoStatus.Completed;
        video.HLSPath = processedVideo.HLSPath;
        video.ThumbnailPath = processedVideo.ThumbnailPath;
        video.FilePath = processedVideo.ProcessedVideoPath.FirstOrDefault() ?? string.Empty;
        return await _repository.UpdateAsync(video);
    }

    public async Task<bool> UpdateVideoAsync(Guid id, UpdateVideoDTO dto)
    {
        if (dto == null)
            return false;

        var video = await _repository.GetByIdAsync(id);
        if (video == null)
            return false;

        video.Title = dto.Title;
        video.Description = dto.Description;

        return await _repository.UpdateAsync(video);
    }

    public async Task<bool> UploadVideoAsync(UploadVideoDTO videoDto)
    {
        Video video = new Video
        {
            Title = videoDto.Title,
            Description = videoDto.Description,
            CreatedAt = DateTime.UtcNow,
            Status = VideoStatus.Uploading
        };
        var addResult = await _repository.AddAsync(video);
        if (!addResult)
            return false;
        var filePath = await _storageService.SaveAsync(videoDto.File);
        video.FilePath = filePath;
        video.Status = VideoStatus.Processing;
        var result = await _repository.UpdateAsync(video);
        await _messagePublisher.Publish(new VideoCreated(video.Id, video.FilePath));
        return result;
    }
}
