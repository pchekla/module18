using System;
using System.IO;
using System.Linq;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YouTubeDownloader.Services;

namespace YouTubeDownloader;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Введите URL видео:");
        var videoUrl = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            Console.WriteLine("URL видео не может быть пустым.");
            return;
        }

        var youtubeClient = new YoutubeService().GetClient();

        try
        {
            // Получаем информацию о видео
            var video = await youtubeClient.Videos.GetAsync(videoUrl);
            Console.WriteLine($"Название: {video.Title}");
            Console.WriteLine($"Описание: {video.Description}");

            // Получаем список доступных потоков
            var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoUrl);

            // Получаем видео-потоки
            var videoStreams = streamManifest.GetVideoOnlyStreams()
                .OrderByDescending(s => s.VideoQuality.Label)
                .ToList();

            if (!videoStreams.Any())
            {
                Console.WriteLine("Доступных потоков для скачивания не найдено.");
                return;
            }

            Console.WriteLine("Доступные качества:");
            for (int i = 0; i < videoStreams.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {videoStreams[i].VideoQuality.Label} ({videoStreams[i].Container.Name})");
            }

            Console.WriteLine("Введите номер качества для скачивания (или 0 для отмены):");
            if (!int.TryParse(Console.ReadLine(), out int selectedIndex) || selectedIndex < 1 || selectedIndex > videoStreams.Count)
            {
                Console.WriteLine("Скачивание отменено.");
                return;
            }

            // Выбираем поток
            var selectedVideoStream = videoStreams[selectedIndex - 1];
            var audioStream = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            if (audioStream == null)
            {
                Console.WriteLine("Аудиопоток не найден.");
                return;
            }

            // Запрашиваем путь для сохранения
            Console.WriteLine("Введите путь для сохранения видео (оставьте пустым для сохранения на рабочий стол):");
            var downloadPath = Console.ReadLine();
            var sanitizedTitle = PathService.SanitizeFileName(video.Title);
            var outputFilePath = string.IsNullOrWhiteSpace(downloadPath)
                ? PathService.GetDefaultDownloadPath($"{sanitizedTitle}.mp4")
                : Path.Combine(downloadPath, $"{sanitizedTitle}.mp4");

            // Загружаем видео- и аудиопотоки
            var videoFilePath = Path.Combine(Path.GetTempPath(), $"{sanitizedTitle}_video.{selectedVideoStream.Container.Name}");
            var audioFilePath = Path.Combine(Path.GetTempPath(), $"{sanitizedTitle}_audio.{audioStream.Container.Name}");

            Console.WriteLine("Загружается видео...");
            await youtubeClient.Videos.Streams.DownloadAsync(selectedVideoStream, videoFilePath);

            Console.WriteLine("Загружается аудио...");
            await youtubeClient.Videos.Streams.DownloadAsync(audioStream, audioFilePath);

            // Объединяем видео и аудио
            Console.WriteLine("Объединяем видео и аудио...");
            var ffmpeg = new FFmpegService();
            ffmpeg.MuxStreams(videoFilePath, audioFilePath, outputFilePath);

            // Удаляем временные файлы
            File.Delete(videoFilePath);
            File.Delete(audioFilePath);

            Console.WriteLine($"Скачивание завершено: {outputFilePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Произошла ошибка: {ex.Message}");
        }
    }
}