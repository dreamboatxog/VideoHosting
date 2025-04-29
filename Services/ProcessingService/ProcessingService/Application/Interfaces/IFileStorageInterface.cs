using System;
using System.Drawing;

namespace ProcessingService.Application.Interfaces;

public interface IFileStorageInterface
{
    Task<string> GetThumbnailSavePath(Guid id);
    Task<string> GetHLSSavePath(Guid id);
}
