using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace YouTubeDownloader.Services;

/// <summary>
/// Сервис для скачивания видео с YouTube.
/// </summary>
public class VideoDownloaderService
{
    private readonly YoutubeClient _youtubeClient;

    public VideoDownloaderService()
    {
        _youtubeClient = new YoutubeClient();
    }

    /// <summary>
    /// Скачивает видео с указанного URL.
    /// </summary>
    /// <param name="videoUrl">URL видео.</param>
    public async Task DownloadVideoAsync(string videoUrl)
    {
        var video = await _youtubeClient.Videos.GetAsync(videoUrl);
        Console.WriteLine($"Название: {video.Title}");
        Console.WriteLine($"Описание: {video.Description}");

        var streamManifest = await _youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);
        var videoStream = SelectVideoStream(streamManifest);
        var audioStream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        if (videoStream == null || audioStream == null)
        {
            Console.WriteLine("Не удалось найти подходящие потоки для скачивания.");
            return;
        }

        var sanitizedTitle = PathService.SanitizeFileName(video.Title);
        var videoFilePath = Path.Combine(Path.GetTempPath(), $"{sanitizedTitle}_video.{videoStream.Container.Name}");
        var audioFilePath = Path.Combine(Path.GetTempPath(), $"{sanitizedTitle}_audio.{audioStream.Container.Name}");
        var outputFilePath = GetOutputFilePath(sanitizedTitle);

        await DownloadStreamAsync(videoStream, videoFilePath, "видео");
        await DownloadStreamAsync(audioStream, audioFilePath, "аудио");

        var ffmpeg = new FFmpegService();
        Console.WriteLine("Объединяем видео и аудио...");
        ffmpeg.MuxStreams(videoFilePath, audioFilePath, outputFilePath);

        File.Delete(videoFilePath);
        File.Delete(audioFilePath);

        Console.WriteLine($"Скачивание завершено: {outputFilePath}");
    }

    /// <summary>
    /// Позволяет пользователю выбрать поток для скачивания.
    /// </summary>
    /// <param name="streamManifest">Манифест потоков.</param>
    /// <returns>Выбранный поток.</returns>
    private IStreamInfo SelectVideoStream(StreamManifest streamManifest)
    {
        var videoStreams = streamManifest.GetVideoOnlyStreams()
            .OrderByDescending(s => s.VideoQuality.Label)
            .ToList();

        if (!videoStreams.Any())
        {
            Console.WriteLine("Нет доступных видео потоков.");
            return null!;
        }

        Console.WriteLine("Доступные качества:");
        for (int i = 0; i < videoStreams.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {videoStreams[i].VideoQuality.Label} ({videoStreams[i].Container.Name})");
        }

        Console.WriteLine("Введите номер качества для скачивания (или 0 для отмены):");
        if (int.TryParse(Console.ReadLine(), out int selectedIndex) &&
            selectedIndex >= 1 &&
            selectedIndex <= videoStreams.Count)
        {
            return videoStreams[selectedIndex - 1];
        }

        Console.WriteLine("Скачивание отменено.");
        return null!;
    }

    /// <summary>
    /// Получить путь для сохранения итогового файла.
    /// </summary>
    /// <param name="sanitizedTitle">Очищенное название видео.</param>
    /// <returns>Путь для сохранения.</returns>
    private string GetOutputFilePath(string sanitizedTitle)
    {
        Console.WriteLine("Введите путь для сохранения видео (оставьте пустым для сохранения на рабочий стол):");
        var downloadPath = Console.ReadLine();
        return string.IsNullOrWhiteSpace(downloadPath)
            ? PathService.GetDefaultDownloadPath($"{sanitizedTitle}.mp4")
            : Path.Combine(downloadPath, $"{sanitizedTitle}.mp4");
    }

    /// <summary>
    /// Скачивает поток.
    /// </summary>
    /// <param name="stream">Информация о потоке.</param>
    /// <param name="filePath">Путь для сохранения.</param>
    /// <param name="streamType">Тип потока (видео/аудио).</param>
    private async Task DownloadStreamAsync(IStreamInfo stream, string filePath, string streamType)
    {
        Console.WriteLine($"Загружается {streamType}...");
        await _youtubeClient.Videos.Streams.DownloadAsync(stream, filePath);
    }
}