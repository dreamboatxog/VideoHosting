namespace Shared.Messages;

public record VideoProcessed(Guid Id,string ThumbnailPath, List<string> ProcessedVideoPath, string HLSPath,  bool Status);
