using System;

namespace ProcessingService.Application;

public class VideoProcessingContext
{
    public string ThumbnailPath { get; set; }
    public string HlsPath { get; set; }
    public List<string> Resolutions { get; set; } = new List<string>();
    public bool Success { get; set; } = true;
    public string ErrorMessage { get; set; }
}
