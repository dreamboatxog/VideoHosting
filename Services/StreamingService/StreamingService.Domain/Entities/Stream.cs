namespace StreamingService.Domain.Entities;

public class Stream
{
    public Guid Id { get; set; }
    public string HlsPath { get; set; } = string.Empty;
}
