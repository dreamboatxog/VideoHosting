using System;
using Microsoft.EntityFrameworkCore;
using StreamingService.Application.Interfaces;
using StreamingService.Domain.Entities;

namespace StreamingService.Infrastructure.Repositories;

public class StreamRepository(StreamDbContext _dbContext) : IStreamRepository
{
    public async Task<bool> AddAsync(Domain.Entities.Stream stream)
    {
        await _dbContext.Streams.AddAsync(stream);
        return await _dbContext.SaveChangesAsync()>0;
    }

    public async Task<bool> DeleteAsync(Domain.Entities.Stream stream)
    {
         _dbContext.Streams.Remove(stream);
        return await _dbContext.SaveChangesAsync()>0;
    }



    public async Task<Domain.Entities.Stream> GetByIdAsync(Guid streamId)
    {
        return await _dbContext.Streams.FindAsync(streamId);
    }

}
