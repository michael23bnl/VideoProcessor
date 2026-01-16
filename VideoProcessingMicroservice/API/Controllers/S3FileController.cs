using Microsoft.AspNetCore.Mvc;
using VideoProcessingMicroservice.Application.DTO.PresignedURL;
using VideoProcessingMicroservice.Core.Abstractions;

namespace VideoProcessingMicroservice.API.Controllers;

[ApiController]
[Route("[controller]")]
public class S3FileController : ControllerBase
{
    private readonly IVideoUploadService _videoUploadService;

    public S3FileController(IVideoUploadService videoUploadService)
    {
        _videoUploadService = videoUploadService;
    }

    [HttpPost("initial-upload")]
    public async Task<IActionResult> InitialUpload([FromBody] UploadRequest request, CancellationToken cancellationToken)
    {
        var response = await _videoUploadService.InitiateUploadAsync(request, cancellationToken);
        
        return Ok(response);
    }

    [HttpPost("complete-upload")]
    public async Task<IActionResult> CompleteUpload([FromBody] CompleteUploadRequest request, CancellationToken cancellationToken)
    {
        var response = await _videoUploadService.CompleteUploadAsync(request, cancellationToken);
        
        return Ok(response);
    }

    [HttpGet("download-url")]
    public async Task<IActionResult> GetDownloadUrl([FromQuery] string key)
    {
        var url = await _videoUploadService.GetDownloadUrlAsync(key);
        
        return Ok(url);
    }
}