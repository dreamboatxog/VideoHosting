using System;
using System.Drawing;
using ProcessingService.Application.Interfaces;

namespace ProcessingService.Infrstructure.Storage;

public class LocalFileStorage : IFileStorageInterface
{

    private readonly string _basePath = @"D:\Temp";
    public async Task<string> GetHLSSavePath(Guid id)
    {
        return await CreateAndReturnPath("HLS", id);
    }

    public async Task<string> GetThumbnailSavePath(Guid id)
    {
        return await CreateAndReturnPath("Thumbnail", id);
    }

    public async Task<string> GetVideoSavePath(Guid id, Size resolution)
    {
        return await CreateAndReturnPath("Video", id, resolution);
    }


    private Task<string> CreateAndReturnPath(string subfolder, Guid id, Size? resolution = null)
    {
        string path = Path.Combine(_basePath, subfolder, id.ToString());

        if (resolution.HasValue)
        {
            string resFolder = $"{resolution.Value.Width}x{resolution.Value.Height}";
            path = Path.Combine(path, resFolder);
        }

        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        return Task.FromResult(path);
    }
}
