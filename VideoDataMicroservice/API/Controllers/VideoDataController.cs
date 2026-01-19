using Microsoft.AspNetCore.Mvc;
using Shared.DTO;
using VideoDataMicroservice.Application.DTO;
using VideoDataMicroservice.Core.Abstractions;
using VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.API.Controllers;

[ApiController]
[Route("[controller]")]
public class VideoDataController : ControllerBase
{
    private readonly IVideoDataService _videoDataService;

    public VideoDataController(IVideoDataService videoDataService)
    {
        _videoDataService = videoDataService;
    }

    [HttpPost("create")]
    public async Task<ActionResult<Guid>> CreateVideoData(CreateVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        var response = await _videoDataService.CreateVideoDataAsync(
            request.Id,
                request.FileName, 
                null, 
                request.InitialStatus,
                null, 
                null, 
                cancellationToken);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return Ok(response.Value);
    }

    [HttpGet("get")]
    public async Task<ActionResult<VideoData>> GetVideoData(Guid id, CancellationToken cancellationToken)
    {
        var response = await _videoDataService.GetVideoDataAsync(id, cancellationToken);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpGet("get/all")]
    public async Task<ActionResult<List<VideoData>>> GetAllVideoData(CancellationToken cancellationToken)
    {
        var response = await _videoDataService.GetAllVideoDataAsync(cancellationToken);

        return Ok(response.Value);
    }

    [HttpGet("get/uploads")]
    public async Task<ActionResult<List<UploadedVideoData>>> GetUploadedVideoData(CancellationToken cancellationToken)
    {
        var response = await _videoDataService.GetUploadedVideoDataAsync(cancellationToken);
        
        return Ok(response.Value);
    }

    [HttpPut("update")]
    public async Task<ActionResult<Guid>> UpdateVideoData(Guid id, string title, string? description,
        string thumbnailUrl, CancellationToken cancellationToken)
    {
        var response = await _videoDataService
            .UpdateVideoDataAsync(id, title, description, thumbnailUrl, cancellationToken);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpPut("update-status")]
    public async Task<ActionResult<Guid>> UpdateVideoStatus(UpdateVideoStatusRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _videoDataService.UpdateVideoStatusAsync(request.Id, request.Status, cancellationToken);
        
        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpPut("set-key")]
    public async Task<ActionResult<Guid>> SetVideoKey(SetVideoKeyRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _videoDataService.SetVideoKeyAsync(request.Id, request.Key, cancellationToken);
        
        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpDelete("delete")]
    public async Task<ActionResult<Guid>> DeleteVideoData(Guid id, CancellationToken cancellationToken)
    {
        var response = await _videoDataService.DeleteVideoDataAsync(id, cancellationToken);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }
    
}