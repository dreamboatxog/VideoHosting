using System;
using VideoService.Domain.Entities;

namespace VideoService.Application.Interfaces;

public interface IVideoRepository
{
    Task<bool> AddAsync(Video video);

    Task<bool> UpdateAsync(Video video);

    Task<Video?> GetByIdAsync(Guid id);

    Task<bool> DeleteAsync(Video video); 
}
