using Microsoft.AspNetCore.Mvc;
using StreamingService.Application.Interfaces;

namespace StreamingService.API.Controllers
{
    [Route("stream-api/[controller]")]
    [ApiController]
    public class StreamController(IStreamingProvider _streamingProvider) : ControllerBase
    {
      
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var playlist = await _streamingProvider.GetPlaylistPathAsync(id);
            
            return playlist != null ? PhysicalFile(playlist.Value.Path, playlist.Value.ContentType) : NotFound();
        }


        [HttpGet("{videoId:guid}/{**relativePath}")]
        public async Task<IActionResult> Get(Guid videoId, string? relativePath)
        {
            var segment = await _streamingProvider.GetSegmentPathAsync(videoId, relativePath);
  //var stream = new FileStream(segment.Value.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return PhysicalFile(segment.Value.Path, segment.Value.ContentType);
        }
    }
}
