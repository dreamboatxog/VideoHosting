using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using VideoService.Application.Interfaces;

namespace VideoService.Infrastructure.Services;

public class LocalStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public LocalStorageService(IConfiguration configuration)
    {
        _storagePath = configuration["Storage:Path"] ?? "uploads";
        Directory.CreateDirectory(_storagePath);
    }
    public async Task<string> SaveAsync(IFormFile file)
    {
        if (file == null)
            throw new ArgumentNullException(nameof(file));

        var fileName = $"{Guid.NewGuid()}_{file.FileName}";
        var filePath = Path.Combine(_storagePath, fileName);

        var fullFilePath = Path.GetFullPath(filePath);
        using (var stream = new FileStream(fullFilePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fullFilePath;
    }

    public async Task DeleteAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        await Task.CompletedTask;
    }
}
