using Amazon.S3;
using Amazon.S3.Model;
using VideoProcessingMicroservice.Application.Abstractions;
using VideoProcessingMicroservice.Application.DTO.PresignedURL;

namespace VideoProcessingMicroservice.Infrastructure.S3;

public class S3VideoStorage : IVideoStorage
{
    private const int ChunkSize = 10 * 1024 * 1024;
    
    private readonly string _originalBucketName;
    private readonly string _processedBucketName;
    private readonly List<string> _allowedExtensions = [".mp4", ".webm", ".mov", ".avi", ".wmv", ".mkv", ".flv"];
    private readonly IAmazonS3 _s3Client;

    public S3VideoStorage(IAmazonS3 s3Client, IConfiguration config)
    {
        _s3Client = s3Client;
        _originalBucketName = config["Minio:Buckets:Original"];
        _processedBucketName = config["Minio:Buckets:Processed"];
    }

    private async Task EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken)
    {
        try
        {
            await _s3Client.HeadBucketAsync(new HeadBucketRequest
            {
                BucketName = bucketName
            }, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName
                }, cancellationToken);
            }
            else
            {
                throw;
            }
        }
    }

    // public async Task<InitiateResponse> InitiateUploadAsync(UploadRequest request, Guid videoId, CancellationToken cancellationToken)
    // {
    //     var extension = Path.GetExtension(request.FileName);
    //     if (!_allowedExtensions.Contains(extension))
    //     {
    //         throw new Exception();
    //     }
    //     await EnsureBucketExistsAsync(_originalBucketName, cancellationToken);
    //
    //     var key = $"uploads/{videoId}_{request.FileName}";
    //     var initRequest = new InitiateMultipartUploadRequest
    //     {
    //         BucketName = _originalBucketName,
    //         Key = key
    //     };
    //
    //     var initResponse = await _s3Client.InitiateMultipartUploadAsync(initRequest, cancellationToken);
    //     var uploadId = initResponse.UploadId;
    //     var totalParts = (int)Math.Ceiling(request.FileSize / (double)ChunkSize);
    //     
    //     if (totalParts <= 0) totalParts = 1;
    //
    //     var presignedUrls = new List<PresignedPartUrl>(totalParts);
    //
    //     for (int i = 1; i <= totalParts; i++)
    //     {
    //         var urlRequest = new GetPreSignedUrlRequest
    //         {
    //             BucketName = _originalBucketName,
    //             Key = key,
    //             Verb = HttpVerb.PUT,
    //             Expires = DateTime.UtcNow.AddMinutes(10),
    //             UploadId = initResponse.UploadId,
    //             PartNumber = i
    //         };
    //
    //         var url = await _s3Client.GetPreSignedURLAsync(urlRequest);
    //
    //         url = url.Replace("https://", "http://");
    //         presignedUrls.Add(new PresignedPartUrl(i, url));
    //     }
    //
    //     return new InitiateResponse(uploadId, key, presignedUrls, videoId);
    // }
    
    public async Task<InitiateResponse> InitiateUploadAsync(UploadRequest request, Guid videoId, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(request.FileName);
        if (!_allowedExtensions.Contains(extension))
        {
            throw new Exception();
        }
        await EnsureBucketExistsAsync(_originalBucketName, cancellationToken);
    
        var key = $"uploads/{videoId}_{request.FileName}";
        var initRequest = new InitiateMultipartUploadRequest
        {
            BucketName = _originalBucketName,
            Key = key
        };
    
        var initResponse = await _s3Client.InitiateMultipartUploadAsync(initRequest, cancellationToken);
        var uploadId = initResponse.UploadId;
        var totalParts = (int)Math.Ceiling(request.FileSize / (double)ChunkSize);
    
        if (totalParts <= 0) totalParts = 1;
    
        var presignedUrlsTasks = Enumerable.Range(1, totalParts)
            .Select(async partNumber =>
            {
                var urlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _originalBucketName,
                    Key = key,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddMinutes(10),
                    UploadId = uploadId,
                    PartNumber = partNumber
                };
    
                var url = await _s3Client.GetPreSignedURLAsync(urlRequest);
                url = url.Replace("https://", "http://");
                return new PresignedPartUrl(partNumber, url);
            })
            .ToList();
    
        var presignedUrls = await Task.WhenAll(presignedUrlsTasks);
    
        return new InitiateResponse(uploadId, key, presignedUrls.ToList(), videoId);
    }

    public async Task<UploadCompletedResponse> CompleteUploadAsync(CompleteUploadRequest request,
        CancellationToken cancellationToken)
    {
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = _originalBucketName,
            Key = request.Key,
            UploadId = request.UploadId,
            PartETags = request.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };

        var response = await _s3Client.CompleteMultipartUploadAsync(completeRequest, cancellationToken);
        
        return new UploadCompletedResponse(response.Key);
    }
    
    private async Task AbortUploadAsync(string key, string uploadId, CancellationToken cancellationToken)
    {
        var abortRequest = new AbortMultipartUploadRequest
        {
            BucketName = _processedBucketName,
            Key = key,
            UploadId = uploadId
        };

        await _s3Client.AbortMultipartUploadAsync(abortRequest, cancellationToken);
    }

    public async Task<string> GetDownloadUrlAsync(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = _processedBucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };

        var url = await _s3Client.GetPreSignedURLAsync(request);
        
        url = url.Replace("https://", "http://");

        return url;
    }

    public async Task<Stream> DownloadAsync(string key, CancellationToken cancellationToken)
    {
        var memoryStream = new MemoryStream();

        var request = new GetObjectRequest
        {
            BucketName = _originalBucketName,
            Key = key
        };

        using var response = await _s3Client.GetObjectAsync(request, cancellationToken);
        await response.ResponseStream.CopyToAsync(memoryStream, cancellationToken);

        memoryStream.Position = 0; // сброс позиции в начало
        return memoryStream;
    }
    
    public async Task<long> UploadAsync(Stream fileStream, string key, CancellationToken cancellationToken)
    {
        var initResponse = await _s3Client.InitiateMultipartUploadAsync(new InitiateMultipartUploadRequest
        {
            BucketName = _processedBucketName,
            Key = key
        }, cancellationToken);

        var uploadId = initResponse.UploadId;
        var partETags = new List<PartETag>();
        int partNumber = 1;

        var buffer = new byte[ChunkSize];
        int bytesRead;
        long totalBytes = 0;

        try
        {
            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length, cancellationToken)) > 0)
            {
                using var memStream = new MemoryStream(buffer, 0, bytesRead);
                var uploadPartResponse = await _s3Client.UploadPartAsync(new UploadPartRequest
                {
                    BucketName = _processedBucketName,
                    Key = key,
                    UploadId = uploadId,
                    PartNumber = partNumber,
                    InputStream = memStream
                }, cancellationToken);

                partETags.Add(new PartETag(partNumber, uploadPartResponse.ETag));
                partNumber++;
                totalBytes += bytesRead;
            }

            await _s3Client.CompleteMultipartUploadAsync(new CompleteMultipartUploadRequest
            {
                BucketName = _processedBucketName,
                Key = key,
                UploadId = uploadId,
                PartETags = partETags
            }, cancellationToken);

            return totalBytes;
        }
        catch
        {
            await AbortUploadAsync(key, uploadId, cancellationToken);

            throw;
        }
    }
}
