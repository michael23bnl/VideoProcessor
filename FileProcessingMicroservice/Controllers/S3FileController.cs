using FileProcessingMicroservice.DTO.PresignedURL;
using FileProcessingMicroservice.Services;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessingMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public class S3FileController : ControllerBase
{
    private readonly IS3FileService _s3FileService;

    public S3FileController(IS3FileService s3FileService)
    {
        _s3FileService = s3FileService;
    }

    [HttpPost("initial-upload")]
    public async Task<IActionResult> InitialUpload([FromBody] UploadRequest request)
    {
        var response = await _s3FileService.InitiateUploadAsync(request);
        return Ok(response);
    }

    [HttpPost("complete-upload")]
    public async Task<IActionResult> CompleteUpload([FromBody] CompleteUploadRequest request)
    {
        var response = await _s3FileService.CompleteUploadAsync(request);
        return Ok(response);
    }

    [HttpGet("download-url")]
    public async Task<IActionResult> GetDownloadUrl([FromQuery] string key)
    {
        var url = await _s3FileService.GetDownloadUrlAsync(key);
        return Ok(url);
    }
}