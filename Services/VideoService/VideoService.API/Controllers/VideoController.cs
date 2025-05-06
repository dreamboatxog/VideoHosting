using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VideoService.Application.DTOs;
using VideoService.Application.Interfaces;

namespace VideoService.API.Controllers;

[Route("video-api/[controller]")]
[ApiController]
public class VideoController(IVideoService _videoService) : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> Upload([FromForm] UploadVideoDTO dto)
    {
        var result = await _videoService.UploadVideoAsync(dto);
        if (!result)
            return BadRequest();
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var video = await _videoService.GetVideoByIdAsync(id);
        return video != null ? Ok(video) : NotFound();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVideoDTO dto)
    {
        var success = await _videoService.UpdateVideoAsync(id, dto);
        return success ? Ok() : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _videoService.DeleteVideoAsync(id);
        return success ? NoContent() : NotFound();
    }
}
