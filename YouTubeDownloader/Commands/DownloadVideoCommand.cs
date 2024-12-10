using System;
using System.IO;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Services;

namespace YouTubeDownloader.Commands;

/// <summary>
/// Команда для скачивания видео.
/// </summary>
public class DownloadVideoCommand : ICommand
{
    private readonly YoutubeClient _client;
    private readonly string _videoUrl;
    private string _outputFilePath;

    public DownloadVideoCommand(YoutubeClient client, string videoUrl, string? outputFilePath = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _videoUrl = videoUrl ?? throw new ArgumentNullException(nameof(videoUrl));

        // Устанавливаем начальное значение _outputFilePath
        _outputFilePath = outputFilePath ?? string.Empty;
    }

    public async Task ExecuteAsync()
    {
        // Получаем информацию о видео
        var video = await _client.Videos.GetAsync(_videoUrl);

        // Формируем путь с названием видео
        var sanitizedTitle = PathService.SanitizeFileName(video.Title);
        var fileName = $"{sanitizedTitle}.mp4";
        _outputFilePath = string.IsNullOrWhiteSpace(_outputFilePath)
            ? PathService.GetDefaultDownloadPath(fileName)
            : Path.Combine(_outputFilePath, fileName);

        var streamManifest = await _client.Videos.Streams.GetManifestAsync(_videoUrl);

        // Выбираем поток с максимальным качеством
        var streamInfo = streamManifest.GetMuxedStreams()
            .OrderByDescending(s => GetVideoResolution(s.VideoQuality.Label))
            .FirstOrDefault();

        if (streamInfo != null)
        {
            Console.WriteLine($"Начинается скачивание видео: {video.Title}");
            await _client.Videos.Streams.DownloadAsync(streamInfo, _outputFilePath);
            Console.WriteLine($"Скачивание завершено: {_outputFilePath}");
        }
        else
        {
            Console.WriteLine("Не удалось найти подходящий поток.");
        }
    }

    /// <summary>
    /// Получить числовое разрешение из ярлыка качества видео.
    /// </summary>
    /// <param name="qualityLabel">Ярлык качества (например, "1080p").</param>
    /// <returns>Числовое разрешение.</returns>
    private static int GetVideoResolution(string qualityLabel)
    {
        if (int.TryParse(qualityLabel.TrimEnd('p'), out var resolution))
        {
            return resolution;
        }
        return 0;
    }
}