using FileProcessingMicroservice.Services;
using FileProcessingMicroservice.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessingMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public class ServerFileController : ControllerBase
{
    private readonly IServerFileService _serverFileService;

    public ServerFileController(IServerFileService serverFileService)
    {
        _serverFileService = serverFileService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [MultipartFormData]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> Upload()
    {
        var fileUploadSummary = await _serverFileService.UploadFileAsync(Request.Body, Request.ContentType!);
        
        return CreatedAtAction(nameof(Upload), fileUploadSummary);
    }
}