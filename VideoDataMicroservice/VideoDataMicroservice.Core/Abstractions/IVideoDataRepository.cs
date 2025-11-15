using VideoDataMicroservice.VideoDataMicroservice.Core.Models;

namespace VideoDataMicroservice.VideoDataMicroservice.Core.Abstractions;

public interface IVideoDataRepository
{
    Task<Guid> CreateAsync(string title, string description, 
        string url, string thumbnailUrl);

    Task<VideoData?> GetAsync(Guid id);
    Task<List<VideoData>> GetAllAsync();

    Task<int> UpdateAsync(Guid id, string title, string description,
        string thumbnailUrl);

    Task<int> DeleteAsync(Guid id);
}