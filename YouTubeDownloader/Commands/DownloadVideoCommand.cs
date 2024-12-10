using System;
using System.IO;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeDownloader.Interfaces;

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
        _client = client;
        _videoUrl = videoUrl;

        // Устанавливаем путь по умолчанию, если путь не указан
        _outputFilePath = string.IsNullOrWhiteSpace(outputFilePath)
            ? GetDefaultDownloadPath()
            : Path.Combine(outputFilePath, "video.mp4");
    }

    public async Task ExecuteAsync()
    {
        var streamManifest = await _client.Videos.Streams.GetManifestAsync(_videoUrl);

        // Выбираем поток с максимальным качеством
        var streamInfo = streamManifest.GetMuxedStreams()
            .OrderByDescending(s => GetVideoResolution(s.VideoQuality.Label))
            .FirstOrDefault();

        if (streamInfo != null)
        {
            Console.WriteLine("Начинается скачивание...");
            await _client.Videos.Streams.DownloadAsync(streamInfo, _outputFilePath);
            Console.WriteLine($"Скачивание завершено: {_outputFilePath}");
        }
        else
        {
            Console.WriteLine("Не удалось найти подходящий поток.");
        }
    }

    /// <summary>
    /// Получить путь загрузки по умолчанию на основе ОС.
    /// </summary>
    /// <returns>Путь загрузки по умолчанию.</returns>
    private static string GetDefaultDownloadPath()
    {
        string desktopPath;
        if (OperatingSystem.IsWindows())
        {
            desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "video.mp4");
        }
        else if (OperatingSystem.IsMacOS())
        {
            desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), "video.mp4");
        }
        else if (OperatingSystem.IsLinux())
        {
            desktopPath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "/tmp", "Рабочий стол", "video.mp4");
        }
        else
        {
            throw new PlatformNotSupportedException("Операционная система не поддерживается.");
        }

        return desktopPath;
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