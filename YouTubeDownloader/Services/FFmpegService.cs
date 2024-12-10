using System.Diagnostics;

namespace YouTubeDownloader.Services;

public class FFmpegService
{
    public void MuxStreams(string videoPath, string audioPath, string outputPath)
    {
        var ffmpegPath = "ffmpeg"; // Убедитесь, что ffmpeg установлен и доступен в PATH
        var arguments = $"-i \"{videoPath}\" -i \"{audioPath}\" -c copy \"{outputPath}\"";

        var process = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = arguments,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new Exception($"FFmpeg завершился с ошибкой. Код выхода: {process.ExitCode}");
        }
    }
}