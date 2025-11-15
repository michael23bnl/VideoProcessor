using Microsoft.AspNetCore.Mvc;
using VideoDataMicroservice.VideoDataMicroservice.Core.Abstractions;
using VideoDataMicroservice.VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.VideoDataMicroservice.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoDataController : ControllerBase
{
    private readonly IVideoDataService _videoDataService;

    public VideoDataController(IVideoDataService videoDataService)
    {
        _videoDataService = videoDataService;
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateVideoData(string title, string description, string url, string thumbnailUrl)
    {
        var response = await _videoDataService.CreateVideoDataAsync(title, description, url, thumbnailUrl);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return Ok(response.Value);
    }

    [HttpGet("get")]
    public async Task<ActionResult<VideoData>> GetVideoData(Guid id)
    {
        var response = await _videoDataService.GetVideoDataAsync(id);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpGet("get/all")]
    public async Task<ActionResult<List<VideoData>>> GetAllVideoData()
    {
        var response = await _videoDataService.GetAllVideoDataAsync();

        return Ok(response.Value);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Guid>> UpdateVideoData(Guid id, string title, string description,
        string thumbnailUrl)
    {
        var response = await _videoDataService
            .UpdateVideoDataAsync(id, title, description, thumbnailUrl);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<Guid>> DeleteVideoData(Guid id)
    {
        var response = await _videoDataService.DeleteVideoDataAsync(id);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }
    
}