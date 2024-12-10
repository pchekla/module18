using YouTubeDownloader.Interfaces;
using YoutubeExplode;

namespace YouTubeDownloader.Commands;

/// <summary>
/// Команда для получения информации о видео.
/// </summary>
public class GetVideoInfoCommand : ICommand
{
    private readonly YoutubeClient _client;
    private readonly string _videoUrl;

    public GetVideoInfoCommand(YoutubeClient client, string videoUrl)
    {
        _client = client;
        _videoUrl = videoUrl;
    }

    public async Task ExecuteAsync()
    {
        var video = await _client.Videos.GetAsync(_videoUrl);
        Console.WriteLine($"Название: {video.Title}");
        Console.WriteLine($"Описание: {video.Description}");
    }
}