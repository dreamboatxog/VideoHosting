using Shared.Interfaces;
using Shared.Messages;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
using VideoService.Domain.Enums;


namespace VideoService.Application.Services;

public class VideoService(IVideoRepository _repository, IFileStorageService _storageService, IMessagePublisher _messagePublisher) : IVideoService
{
    public async Task<bool> DeleteVideoAsync(Guid id)
    {
        var video = await _repository.GetByIdAsync(id);
        if (video == null)
            return false;
        if (!string.IsNullOrEmpty(video.FilePath))
            await _storageService.DeleteAsync(video.FilePath);
        await _messagePublisher.Publish(new VideoDeleted(video.Id));
        return await _repository.DeleteAsync(video);
    }



    public async Task<Video?> GetVideoByIdAsync(Guid id)
    {
        var video = await _repository.GetByIdAsync(id);
        if (video == null)
            return null;
        return video;
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
        if (processedVideo.ThumbnailPath == null || processedVideo.ProcessedVideoPath == null)
        {
           return false;
        }
        video.Status = VideoStatus.Completed;
        video.ThumbnailPath = processedVideo.ThumbnailPath!;
        video.FilePath = processedVideo.ProcessedVideoPath!.FirstOrDefault() ?? string.Empty;
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
