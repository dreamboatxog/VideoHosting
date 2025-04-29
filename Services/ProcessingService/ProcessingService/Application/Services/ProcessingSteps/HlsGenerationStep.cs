using System;
using NReco.VideoConverter;
using ProcessingService.Application.Interfaces;

namespace ProcessingService.Application.Services.ProcessingSteps;

public class HlsGenerationStep(FFMpegConverter _fFMpeg, IFileStorageInterface _fileStorage) : IVideoProcessingStep
{
    public async Task ProcessAsync(Guid videoId, string videoFilePath, VideoProcessingContext context)
    {
        try
        {
            var outputDirectory = await _fileStorage.GetHLSSavePath(videoId);

            var arguments = $"-i \"{videoFilePath}\" " +
                            "-profile:v baseline -level 1.0 -start_number 0 " +
                            "-hls_time 1 -hls_list_size 0 -f hls " +
                            $"\"{outputDirectory}\"";

            await Task.Run(() => _fFMpeg.Invoke(arguments));
            context.HlsPath = outputDirectory;
        }
        catch (Exception ex)
        {
            context.Success = false;
            context.ErrorMessage = ex.Message;
        }
    }
}
