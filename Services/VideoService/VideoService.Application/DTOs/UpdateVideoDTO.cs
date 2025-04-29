namespace VideoService.Application.DTOs;

public record class UpdateVideoDTO
{
    public required string Title { get; set; }
    public required string Description { get; set; }
}
