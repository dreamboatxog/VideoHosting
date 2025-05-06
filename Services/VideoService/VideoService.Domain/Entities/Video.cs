using System;
using VideoService.Domain.Enums;

namespace VideoService.Domain.Entities;

public class Video
{
    public Guid Id { get; set; }
    public required string Title { get; set; }   
    public required string Description { get; set; } 
    public string FilePath { get; set; } = string.Empty;
    public string ThumbnailPath { get; set; } = string.Empty;
    public required DateTime CreatedAt { get; set; }
    public required VideoStatus Status { get; set; }
}
