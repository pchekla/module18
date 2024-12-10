using System;
using YouTubeDownloader.Services;

namespace YouTubeDownloader;

/// <summary>
/// Основной класс программы.
/// </summary>
class Program
{
    /// <summary>
    /// Точка входа в программу.
    /// </summary>
    /// <param name="args">Аргументы командной строки.</param>
    static async Task Main(string[] args)
    {
        Console.WriteLine("Введите URL видео:");
        var videoUrl = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            Console.WriteLine("URL видео не может быть пустым.");
            return;
        }

        try
        {
            var videoDownloader = new VideoDownloaderService();
            await videoDownloader.DownloadVideoAsync(videoUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}