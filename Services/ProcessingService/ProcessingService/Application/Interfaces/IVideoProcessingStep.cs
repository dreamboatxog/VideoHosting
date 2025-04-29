namespace ProcessingService.Application.Interfaces;

public interface IVideoProcessingStep 
{
    Task ProcessAsync(Guid videoId, string videoFilePath, VideoProcessingContext context);
}
