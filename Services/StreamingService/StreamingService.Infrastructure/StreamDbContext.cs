using Microsoft.EntityFrameworkCore;

namespace StreamingService.Infrastructure;

public class StreamDbContext : DbContext
{
    public StreamDbContext(DbContextOptions<StreamDbContext> options) : base(options) { }
    public DbSet<Domain.Entities.Stream> Streams { get; set; }
}
