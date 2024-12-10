using YoutubeExplode;

namespace YouTubeDownloader.Services;

/// <summary>
/// Сервис для работы с YouTube.
/// </summary>
public class YoutubeService
{
    private readonly YoutubeClient _youtubeClient;

    public YoutubeService()
    {
        _youtubeClient = new YoutubeClient();
    }

    public YoutubeClient GetClient()
    {
        return _youtubeClient;
    }
}