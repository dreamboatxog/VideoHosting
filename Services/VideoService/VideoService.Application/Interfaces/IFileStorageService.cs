using System;
using Microsoft.AspNetCore.Http;

namespace VideoService.Application.Interfaces;

public interface IFileStorageService
{
        Task<string> SaveAsync(IFormFile file);
        Task DeleteAsync(string filePath);
}
