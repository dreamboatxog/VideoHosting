using StreamingService.Application.Interfaces;

namespace StreamingService.Infrastructure.Repositories;

public class StreamRepository(StreamDbContext _dbContext) : IStreamRepository
{
    public async Task<bool> AddAsync(Domain.Entities.Stream stream)
    {
        await _dbContext.Streams.AddAsync(stream);
        return await _dbContext.SaveChangesAsync()>0;
    }

    public async Task<bool> DeleteAsync(Guid Id)
    {
        var stream= await _dbContext.Streams.FindAsync(Id);
        if(stream is null){
            throw new Exception($"Stream with id {Id} not found.");
        }
        _dbContext.Streams.Remove(stream);
        return await _dbContext.SaveChangesAsync()>0;
    }



    public async Task<Domain.Entities.Stream> GetByIdAsync(Guid streamId)
    {
        var stream= await _dbContext.Streams.FindAsync(streamId);
        if(stream is null){
            throw new Exception($"Stream with id {streamId} not found.");
        }
        return stream;
    }

}
