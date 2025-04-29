using Shared.Messages;

namespace ProcessingService.Application.Interfaces;

public interface IVideoWorkflowOrchestrator
{
    Task ProcessVideoAsync(string videoFilePath, Guid videoId);
}
