using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeDownloader.Interfaces;

namespace YouTubeDownloader.Commands;

/// <summary>
/// Команда для скачивания видео.
/// </summary>
public class DownloadVideoCommand : ICommand
{
    private readonly YoutubeClient _youtubeClient;
    private readonly string _videoUrl;
    private readonly string _outputFilePath;

    /// <summary>
    /// Инициализирует новую команду для скачивания видео.
    /// </summary>
    /// <param name="youtubeClient">Клиент YoutubeClient.</param>
    /// <param name="videoUrl">URL видео.</param>
    /// <param name="outputFilePath">Путь для сохранения видео.</param>
    public DownloadVideoCommand(YoutubeClient youtubeClient, string videoUrl, string outputFilePath)
    {
        _youtubeClient = youtubeClient;
        _videoUrl = videoUrl;
        _outputFilePath = outputFilePath;
    }

    /// <summary>
    /// Выполняет скачивание видео.
    /// </summary>
    public async Task ExecuteAsync()
    {
        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(_videoUrl);

        // Выбираем muxed поток (с видео и аудио)
        var streamInfo = streamManifest.GetMuxedStreams().GetWithHighestBitrate();

        if (streamInfo == null)
        {
            Console.WriteLine("Не удалось найти подходящий поток для загрузки.");
            return;
        }

        Console.WriteLine($"Начинается скачивание видео: {_videoUrl}");
        await _youtubeClient.Videos.Streams.DownloadAsync(streamInfo, _outputFilePath);
        Console.WriteLine($"Видео сохранено в: {_outputFilePath}");
    }
}