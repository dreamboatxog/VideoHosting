using System;
using Shared.Messages;
using VideoService.Application.DTOs;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IVideoService
{
    Task<bool> UploadVideoAsync(UploadVideoDTO dto);
    Task<Video?> GetVideoByIdAsync(Guid id);
    Task<bool> UpdateVideoAsync(Guid id, UpdateVideoDTO dto);
    Task<bool> DeleteVideoAsync(Guid id);
    Task<bool> ProcessVideoAsync(VideoProcessed processedVideo);
}
