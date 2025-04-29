using Shared.Interfaces;
using Shared.Messages;
using ProcessingService.Application.Interfaces;

namespace ProcessingService.Application.Services;

public class VideoWorkflowOrchestrator(IMessagePublisher _messagePublisher, IEnumerable<IVideoProcessingStep> _processingSteps) : IVideoWorkflowOrchestrator
{
    public async Task ProcessVideoAsync(string videoFilePath, Guid videoId)
    {
        var context = new VideoProcessingContext();

        try
        {
            foreach (var step in _processingSteps)
            {
                await step.ProcessAsync(videoId, videoFilePath, context);

                if (!context.Success)
                {
                    System.Console.WriteLine("Processing failed at step: " + step.GetType().Name+" for videoId: " + videoId+" with error: "+ context.ErrorMessage );
                    break;
                }
            }

            var message = new VideoProcessed(
                videoId,
                context.ThumbnailPath,
                context.Resolutions,
                context.HlsPath,
                context.Success
            );

            await _messagePublisher.Publish(message);
        }
        catch (Exception ex)
        {
            var errorMessage = new VideoProcessed(videoId, null, null, null, false);
            await _messagePublisher.Publish(errorMessage);
            throw;
        }
    }


}
