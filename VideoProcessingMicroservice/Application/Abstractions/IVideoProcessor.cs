namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IVideoProcessor
{
    Task ProcessToHlsAsync(Stream input, string inputPath, string outputDir, CancellationToken cancellationToken);

    Task ConvertToSpecificResolutionAsync(Stream input, Stream output, string resolution,
        CancellationToken cancellationToken);
}