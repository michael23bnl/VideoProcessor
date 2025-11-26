using Amazon.S3;
using Amazon.S3.Model;
using FileProcessingMicroservice.DTO.PresignedURL;

namespace FileProcessingMicroservice.Services;

public class S3FileService : IS3FileService
{
    private const string BucketName = "video-uploads";
    private const int ChunkSizeInMb = 10;
    private readonly IAmazonS3 _s3Client;

    public S3FileService(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    private async Task EnsureBucketExistsAsync(string bucketName)
    {
        try
        {
            await _s3Client.HeadBucketAsync(new HeadBucketRequest
            {
                BucketName = bucketName
            });
        }
        catch (AmazonS3Exception ex)
        {
            if (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                await _s3Client.PutBucketAsync(new PutBucketRequest
                {
                    BucketName = bucketName
                });
            }
            else
            {
                throw;
            }
        }
    }

    public async Task<InitiateResponse> InitiateUploadAsync(UploadRequest request)
    {
        await EnsureBucketExistsAsync(BucketName);

        var key = $"uploads/{Guid.NewGuid()}_{request.FileName}";

        var initRequest = new InitiateMultipartUploadRequest
        {
            BucketName = BucketName,
            Key = key
        };

        var initResponse = await _s3Client.InitiateMultipartUploadAsync(initRequest);
        var uploadId = initResponse.UploadId;
        var totalParts = (int)Math.Ceiling(request.FileSize / (ChunkSizeInMb * 1024.0 * 1024.0));
        if (totalParts <= 0) totalParts = 1;

        var presignedUrls = new List<PresignedPartUrl>(totalParts);

        // генерируем presigned URL для каждой части
        for (int i = 1; i <= totalParts; i++)
        {
            var urlRequest = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = key,
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(10),
                UploadId = initResponse.UploadId,
                PartNumber = i
            };

            var url = await _s3Client.GetPreSignedURLAsync(urlRequest);

            presignedUrls.Add(new PresignedPartUrl(i, url));
        }

        return new InitiateResponse(uploadId, key, presignedUrls);
    }

    public async Task<UploadCompletedResponse> CompleteUploadAsync(CompleteUploadRequest request)
    {
        var completeRequest = new CompleteMultipartUploadRequest
        {
            BucketName = BucketName,
            Key = request.Key,
            UploadId = request.UploadId,
            PartETags = request.Parts.Select(p => new PartETag(p.PartNumber, p.ETag)).ToList()
        };
        
        var response = await _s3Client.CompleteMultipartUploadAsync(completeRequest);

        return new UploadCompletedResponse(response.Key);
    }
    
    public async Task<string> GetDownloadUrlAsync(string key)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = BucketName,
            Key = key,
            Verb = HttpVerb.GET,
            Expires = DateTime.UtcNow.AddMinutes(30)
        };

        return await _s3Client.GetPreSignedURLAsync(request);
    }


    /*public async Task AbortUploadAsync(AbortUploadRequest request)
    {
        var abortRequest = new AbortMultipartUploadRequest
        {
            BucketName = BucketName,
            Key = request.ObjectName,
            UploadId = request.UploadId
        };

        await _s3Client.AbortMultipartUploadAsync(abortRequest);
    }*/
}