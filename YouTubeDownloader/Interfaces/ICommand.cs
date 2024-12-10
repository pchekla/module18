namespace YouTubeDownloader.Interfaces;

/// <summary>
/// Интерфейс команды.
/// </summary>
public interface ICommand
{
    Task ExecuteAsync();
}