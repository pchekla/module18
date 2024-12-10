using System;
using System.IO;

namespace YouTubeDownloader.Services;

/// <summary>
/// Сервис для работы с путями сохранения файлов.
/// </summary>
public static class PathService
{
    /// <summary>
    /// Получить путь сохранения по умолчанию.
    /// </summary>
    /// <param name="fileName">Название файла.</param>
    /// <returns>Путь сохранения по умолчанию.</returns>
    public static string GetDefaultDownloadPath(string fileName)
    {
        string desktopPath;
        if (OperatingSystem.IsWindows())
        {
            desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), fileName);
        }
        else if (OperatingSystem.IsMacOS())
        {
            desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), fileName);
        }
        else if (OperatingSystem.IsLinux())
        {
            desktopPath = Path.Combine(Environment.GetEnvironmentVariable("HOME") ?? "/tmp", "Рабочий стол", fileName);
        }
        else
        {
            throw new PlatformNotSupportedException("Операционная система не поддерживается.");
        }

        return desktopPath;
    }

    /// <summary>
    /// Удалить недопустимые символы из имени файла.
    /// </summary>
    /// <param name="name">Имя файла.</param>
    /// <returns>Очищенное имя файла.</returns>
    public static string SanitizeFileName(string name)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
        {
            name = name.Replace(invalidChar, '_');
        }
        return name;
    }
}