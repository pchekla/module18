namespace YouTubeDownloader.Interfaces;

/// <summary>
/// Интерфейс для команд программы.
/// </summary>
public interface ICommand
{
    /// <summary>
    /// Выполняет команду.
    /// </summary>
    Task ExecuteAsync();
}