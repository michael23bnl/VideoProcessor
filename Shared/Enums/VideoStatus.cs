namespace Shared.Enums;

public enum VideoStatus
{
    Pending, // инициализация загрузки
    Processing, // обработка FFmpeg
    Ready, // master.m3u8 доступен
    Failed // ошибки
}