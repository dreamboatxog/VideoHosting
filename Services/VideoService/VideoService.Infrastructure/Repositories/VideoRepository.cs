using System;
using Microsoft.EntityFrameworkCore;
using VideoService.Application.Interfaces;
using VideoService.Domain.Entities;
namespace VideoService.Infrastructure.Repositories;

public class VideoRepository(VideoDbContext context) : IVideoRepository
{
    public async Task<bool> AddAsync(Video video)
    {
        await context.AddAsync(video);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(Video video)
    {
        context.Videos.Update(video);
        return await context.SaveChangesAsync() > 0;
    }

    public async Task<Video?> GetByIdAsync(Guid id)
    {
        var video = await context.Videos.FindAsync(id);
        return video;
    }

    public async Task<bool> DeleteAsync(Video video)
    {
        context.Videos.Remove(video);
        return await context.SaveChangesAsync()>0;
    }
}
