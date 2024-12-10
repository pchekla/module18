using System;
using YouTubeDownloader.Commands;
using YouTubeDownloader.Interfaces;
using YouTubeDownloader.Services;

namespace YouTubeDownloader;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Введите путь для сохранения видео (оставьте пустым для сохранения на рабочий стол):");
        
        var downloadPath = Console.ReadLine();
        downloadPath = string.IsNullOrWhiteSpace(downloadPath) ? null : downloadPath;

        Console.WriteLine("Введите URL видео:");
        var videoUrl = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(videoUrl))
        {
            Console.WriteLine("URL видео не может быть пустым.");
            return;
        }

        var youtubeService = new YoutubeService();
        var youtubeClient = youtubeService.GetClient();

        Console.WriteLine("Введите команду (info/download):");
        var command = Console.ReadLine()?.ToLower();

        ICommand? executedCommand = command switch
        {
            "info" => new GetVideoInfoCommand(youtubeClient, videoUrl),
            "download" => new DownloadVideoCommand(youtubeClient, videoUrl, downloadPath),
            _ => null
        };

        if (executedCommand != null)
        {
            await executedCommand.ExecuteAsync();
        }
        else
        {
            Console.WriteLine("Неизвестная команда.");
        }
    }
}