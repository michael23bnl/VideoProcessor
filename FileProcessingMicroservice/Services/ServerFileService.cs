using FileProcessingMicroservice.DTO;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Net.Http.Headers;


namespace FileProcessingMicroservice.Services;

public class ServerFileService : IServerFileService
{
    private const string UploadDirectory = "FilesUploaded";
    
    private readonly List<string> _allowedExtensions = [".mp4", ".webm", ".mov", ".avi", ".wmv", ".mkv", ".flv"];
    
    public async Task<FileUploadSummary> UploadFileAsync(Stream fileStream, string contentType)
    {
        var fileCount = 0;
        long totalSizeInBytes = 0;
        
        var boundary = GetBoundary(MediaTypeHeaderValue.Parse(contentType));
        
        var multipartReader = new MultipartReader(boundary, fileStream);
        var section = await multipartReader.ReadNextSectionAsync();

        var filePaths = new List<string>();
        var notUploadedFiles = new List<string>();

        while (section is not null)
        {
            var fileSection = section.AsFileSection();
            
            if (fileSection is not null)
            {
                var result = await SaveFileAsync(fileSection, filePaths, notUploadedFiles);

                if (result > 0)
                {
                    totalSizeInBytes += result;
                    fileCount++;
                }
            }
            
            section = await multipartReader.ReadNextSectionAsync();
        }

        return new FileUploadSummary
        {
            TotalFilesUploaded = fileCount,
            TotalSizeUploaded = ConvertSizeToString(totalSizeInBytes),
            FilePaths = filePaths,
            NotUploadedFiles = notUploadedFiles
        };
    }

    private async Task<long> SaveFileAsync(FileMultipartSection fileSection, 
        List<string> filePaths, List<string> notUploadedFiles)
    {
        var extension = Path.GetExtension(fileSection.FileName);
        if (!_allowedExtensions.Contains(extension))
        {
            notUploadedFiles.Add(fileSection.FileName);

            return 0;
        }
        
        Directory.CreateDirectory(UploadDirectory);
        
        var filePath = Path.Combine(UploadDirectory, fileSection.FileName);
        
        await using var stream = new FileStream(filePath, FileMode.Create, 
            FileAccess.Write, FileShare.None, 1024);
        await fileSection.FileStream.CopyToAsync(stream);
        
        filePaths.Add(GetFullFilePath(fileSection));

        return fileSection.FileStream.Length;
    }

    private string GetBoundary(MediaTypeHeaderValue mediaTypeHeaderValue)
    {
        var boundary = HeaderUtilities.RemoveQuotes(mediaTypeHeaderValue.Boundary).Value;
        
        if (string.IsNullOrWhiteSpace(boundary))
            throw new InvalidDataException("Missing content-type boundary.");
        
        return boundary;
    }

    private string GetFullFilePath(FileMultipartSection fileSection)
    {
        return !string.IsNullOrEmpty(fileSection.FileName)
            ? Path.Combine(Directory.GetCurrentDirectory(), UploadDirectory, fileSection.FileName)
            : string.Empty;
    }

    private string ConvertSizeToString(long bytes)
    {
        var fileSize = new decimal(bytes);
        var kilobyte = new decimal(1024);
        var megabyte = new decimal(1024 * 1024);
        var gigabyte = new decimal(1024 * 1024 * 1024);

        return fileSize switch
        {
            _ when fileSize < kilobyte => "Less than 1KB",
            _ when fileSize < megabyte => $"{
                Math.Round(
                    fileSize / kilobyte, 
                    fileSize < 10 * kilobyte ? 2 : 1, 
                    MidpointRounding.AwayFromZero)
                :##,###.##}KB",
            _ when fileSize < gigabyte => $"{
                Math.Round(
                    fileSize / megabyte, 
                    fileSize < 10 * megabyte ? 2 : 1, 
                    MidpointRounding.AwayFromZero)
                :##,###.##}MB",
            _ when fileSize >= gigabyte => $"{
                Math.Round(
                    fileSize / gigabyte, 
                    fileSize < 10 * gigabyte ? 2 : 1, 
                    MidpointRounding.AwayFromZero)
                :##,###.##}GB",
            _ => "n/a"
        };
    }
}