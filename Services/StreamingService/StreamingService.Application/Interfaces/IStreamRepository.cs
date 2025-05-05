using StreamingService.Domain.Entities;
using Stream = StreamingService.Domain.Entities.Stream;
namespace StreamingService.Application.Interfaces;

public interface IStreamRepository
{
    Task<Stream> GetByIdAsync(Guid streamId);
    Task<bool> AddAsync(Stream stream);
    Task<bool> DeleteAsync(Stream stream);
}
