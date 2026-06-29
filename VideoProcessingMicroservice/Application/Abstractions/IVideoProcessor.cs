using VideoProcessingMicroservice.Core;

namespace VideoProcessingMicroservice.Application.Abstractions;

public interface IVideoProcessor
{
    Task ProcessAsync(Stream input, string inputPath, string outputDir, AdaptiveFormat format,
        CancellationToken cancellationToken);
    //Task ProcessToHlsAsync(Stream input, string inputPath, string outputDir, CancellationToken cancellationToken);
    //Task ProcessToDashAsync(Stream input, string inputPath, string outputDir, CancellationToken cancellationToken);

    Task ConvertToSpecificResolutionAsync(Stream input, Stream output, string resolution,
        CancellationToken cancellationToken);
}