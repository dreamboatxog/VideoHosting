using System;
using Microsoft.AspNetCore.Http;

namespace VideoService.Application.DTOs;

public record UploadVideoDTO
{
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required IFormFile File { get; set; }

}
