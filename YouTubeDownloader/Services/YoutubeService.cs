using YoutubeExplode;

namespace YouTubeDownloader.Services;

/// <summary>
/// Сервис для создания экземпляра YoutubeClient.
/// </summary>
public class YoutubeService
{
    /// <summary>
    /// Создает и возвращает экземпляр YoutubeClient.
    /// </summary>
    /// <returns>Экземпляр YoutubeClient.</returns>
    public YoutubeClient GetClient()
    {
        return new YoutubeClient();
    }
}