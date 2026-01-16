using Shared.Result;

namespace VideoDataMicroservice.Core.Models.Errors;

public static class VideoDataError
{
    public static readonly Error NotFound = new(
        ErrorTypeConstant.NotFound,
        "Видео с указанным идентификатором не найдено.");

    public static readonly Error InvalidTitle = new(
        ErrorTypeConstant.ValidationError,
        "Заголовок видео не может быть пустым.");

    /*public static readonly Error InvalidUrl = new(
        "VideoData.InvalidUrl",
        "Видео должно иметь URL");*/
    
    public static readonly Error InvalidThumbnailUrl = new(
        ErrorTypeConstant.ValidationError,
        "Видео должно иметь изображение предварительного просмотра");
}