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

    [HttpPost("metadata")]
    public async Task<ActionResult<Guid>> CreateVideoMetadata(CreateVideoMetadataRequest request, CancellationToken cancellationToken)
    {
        var response = await _videoDataService.CreateVideoMetadataAsync(
            request.Id, 
            request.FileName, 
            null, 
            request.InitialStatus,
            null, 
            cancellationToken);

        if (response.IsFailure)
        {
            return BadRequest(response.Error);
        }

        return Ok(response.Value);
    }
    
    [HttpPost("manifest")]
    public async Task<ActionResult<Guid>> CreateVideoManifest(CreateVideoManifestRequest request, 
        CancellationToken cancellationToken)
    {
        var response = await _videoDataService
            .CreateVideoManifestAsync(request.Id, request.Protocol, request.S3Key, cancellationToken);
        
        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpGet]
    public async Task<ActionResult<VideoMetadata>> GetVideoData(Guid id, CancellationToken cancellationToken)
    {
        var response = await _videoDataService.GetVideoDataAsync(id, cancellationToken);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    // [HttpGet("get/all")]
    // public async Task<ActionResult<List<VideoMetadata>>> GetAllVideoData(CancellationToken cancellationToken)
    // {
    //     var response = await _videoDataService.GetAllVideoDataAsync(cancellationToken);
    //
    //     return Ok(response.Value);
    // }

    [HttpGet("uploads")]
    public async Task<ActionResult<List<UploadedVideoData>>> GetUploadedVideoData(CancellationToken cancellationToken)
    {
        var response = await _videoDataService.GetUploadedVideoDataAsync(cancellationToken);
        
        return Ok(response.Value);
    }

    [HttpPut("metadata")]
    public async Task<ActionResult<Guid>> UpdateVideoMetadata(Guid id, string title, string? description,
        string thumbnailUrl, CancellationToken cancellationToken)
    {
        var response = await _videoDataService
            .UpdateVideoMetadataAsync(id, title, description, thumbnailUrl, cancellationToken);

        if (response.IsFailure)
        {
            return NotFound(response.Error);
        }
        
        return Ok(response.Value);
    }

    [HttpPut("metadata/status")]
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

    [HttpDelete]
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