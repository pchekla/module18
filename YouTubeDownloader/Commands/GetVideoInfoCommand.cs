using YoutubeExplode;
using YouTubeDownloader.Interfaces;

namespace YouTubeDownloader.Commands;

/// <summary>
/// Команда для получения информации о видео.
/// </summary>
public class GetVideoInfoCommand : ICommand
{
    private readonly YoutubeClient _youtubeClient;
    private readonly string _videoUrl;

    /// <summary>
    /// Инициализирует новую команду для получения информации о видео.
    /// </summary>
    /// <param name="youtubeClient">Клиент YoutubeClient.</param>
    /// <param name="videoUrl">URL видео.</param>
    public GetVideoInfoCommand(YoutubeClient youtubeClient, string videoUrl)
    {
        _youtubeClient = youtubeClient;
        _videoUrl = videoUrl;
    }

    /// <summary>
    /// Выполняет получение информации о видео.
    /// </summary>
    public async Task ExecuteAsync()
    {
        var video = await _youtubeClient.Videos.GetAsync(_videoUrl);
        Console.WriteLine($"Название: {video.Title}");
        Console.WriteLine($"Автор: {video.Author.ChannelTitle}");
        Console.WriteLine($"Длительность: {video.Duration}");
    }
}