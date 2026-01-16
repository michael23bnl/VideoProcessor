using Shared.DTO;
using Shared.Enums;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Application.DTO;
using VideoProcessingMicroservice.Application.DTO.PresignedURL;
using VideoProcessingMicroservice.Core.Abstractions;

namespace VideoProcessingMicroservice.Application.Services;

public class VideoUploadService : IVideoUploadService
{
    private readonly IVideoStorage _videoStorage;
    private readonly IVideoMetadataClient _videoMetadataClient;
    private readonly IMessageProducer _messageProducer;

    public VideoUploadService(IVideoStorage videoStorage, IVideoMetadataClient videoMetadataClient,
        IMessageProducer messageProducer)
    {
        _videoStorage = videoStorage;
        _videoMetadataClient = videoMetadataClient;
        _messageProducer = messageProducer;
    }

    public async Task<InitiateResponse> InitiateUploadAsync(UploadRequest request, CancellationToken cancellationToken)
    {
        var videoId = Guid.NewGuid();
        var storageResponse = await _videoStorage.InitiateUploadAsync(request, videoId, cancellationToken);
        
        await _videoMetadataClient.CreateAsync(new CreateVideoMetadataRequest
        (
            videoId,
            request.FileName,
            VideoStatus.Pending
        ), cancellationToken);
        
        return storageResponse;
    }

    public async Task<UploadCompletedResponse> CompleteUploadAsync(CompleteUploadRequest request,
        CancellationToken cancellationToken)
    {
        var storageResponse = await _videoStorage.CompleteUploadAsync(request, cancellationToken);

        await _videoMetadataClient.UpdateStatusAsync(new UpdateVideoStatusRequest(
            request.VideoId,
            VideoStatus.Processing
        ), cancellationToken);
        
        var videoUploadedEvent = new VideoUploadedEvent(
            storageResponse.Key,
            request.VideoId
        );
        
        await _messageProducer.SendMessageAsync(videoUploadedEvent, cancellationToken);

        return storageResponse;
    }

    public async Task<string> GetDownloadUrlAsync(string key)
    {
        var url = await _videoStorage.GetDownloadUrlAsync(key);

        return url;
    }
}