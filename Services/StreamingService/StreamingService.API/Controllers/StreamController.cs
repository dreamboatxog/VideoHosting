using Microsoft.AspNetCore.Mvc;
using StreamingService.Application.Interfaces;

namespace StreamingService.API.Controllers
{
    [Route("stream-api/[controller]")]
    [ApiController]
    public class StreamController(IStreamingProvider _streamingProvider) : ControllerBase
    {
        [HttpGet("{videoId:guid}/{**relativePath}")]
        public async Task<IActionResult> Get(Guid videoId, string? relativePath)
        {
             var fullPath= await _streamingProvider.GetStreamAsync(videoId, relativePath);
        var stream = new FileStream(fullPath.Value.Item1, FileMode.Open, FileAccess.Read, FileShare.Read);
        return File(stream, fullPath.Value.Item2);
        }
    }
}
