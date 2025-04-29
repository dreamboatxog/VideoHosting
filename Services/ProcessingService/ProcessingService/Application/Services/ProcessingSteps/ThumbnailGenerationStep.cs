using System;
using NReco.VideoConverter;
using ProcessingService.Application.Interfaces;

namespace ProcessingService.Application.Services.ProcessingSteps;

public class ThumbnailGenerationStep(FFMpegConverter _fFMpeg, IFileStorageInterface _fileStorage) : IVideoProcessingStep
{
    public async Task ProcessAsync(Guid videoId, string videoFilePath, VideoProcessingContext context)
    {
        try
        {
            var filePath = await _fileStorage.GetThumbnailSavePath(videoId);
            await Task.Run(() => _fFMpeg.GetVideoThumbnail(videoFilePath, filePath, 1));
            context.ThumbnailPath = filePath;
            context.Success = true;
        }
        catch (Exception ex)
        {
            context.Success = false;
            context.ErrorMessage = ex.Message;
        }
    }
}
