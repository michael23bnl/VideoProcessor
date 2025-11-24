using FileProcessingMicroservice.Services;
using FileProcessingMicroservice.Utilities;
using Microsoft.AspNetCore.Mvc;

namespace FileProcessingMicroservice.Controllers;

[ApiController]
[Route("[controller]")]
public class FileController : ControllerBase
{
    private readonly IFileService _fileService;

    public FileController(IFileService fileService)
    {
        _fileService = fileService;
    }

    [HttpPost("upload")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status415UnsupportedMediaType)]
    [MultipartFormData]
    [DisableFormValueModelBinding]
    public async Task<IActionResult> Upload()
    {
        var fileUploadSummary = await _fileService.UploadFileAsync(Request.Body, Request.ContentType!);
        
        return CreatedAtAction(nameof(Upload), fileUploadSummary);
    }
}