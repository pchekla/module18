
# YouTubeDownloader

**YouTubeDownloader** – это консольное приложение для скачивания видео с YouTube. Оно позволяет выбрать качество видео перед загрузкой, объединяет видео- и аудиопотоки с использованием FFmpeg и сохраняет итоговый файл на рабочий стол или в указанное пользователем место.

## Функционал

- **Получение информации о видео**: отображение названия, описания и доступных качеств.
- **Выбор качества**: пользователь может выбрать одно из доступных видео-качеств.
- **Скачивание потоков**: приложение автоматически загружает видео- и аудиопотоки с YouTube.
- **Объединение потоков**: с помощью FFmpeg объединяет аудио- и видеопотоки в единый файл.
- **Кросс-платформенность**: работает на Windows, macOS и Linux.

## Требования

- [.NET 6.0+](https://dotnet.microsoft.com/download)
- [FFmpeg](https://ffmpeg.org/download.html): должен быть установлен и доступен в PATH.
- Интернет-соединение.

## Установка

1. Клонируйте репозиторий:

   ```bash
   git clone https://github.com/your-username/YouTubeDownloader.git
   cd YouTubeDownloader
   ```

2. Установите зависимости:

   ```bash
   dotnet restore
   ```

3. Убедитесь, что FFmpeg установлен и доступен:

   - **Windows**: Добавьте папку с FFmpeg в переменную PATH.
   - **Linux/macOS**: Установите FFmpeg с помощью пакетного менеджера:
     ```bash
     sudo apt install ffmpeg  # Для Ubuntu/Debian
     brew install ffmpeg      # Для macOS
     ```

4. Соберите проект:

   ```bash
   dotnet build
   ```

## Использование

1. Запустите приложение:

   ```bash
   dotnet run
   ```

2. Введите URL видео:

   ```plaintext
   Введите URL видео: https://www.youtube.com/watch?v=example
   ```

3. Выберите качество видео из предложенных вариантов:

   ```plaintext
   Доступные качества:
   1. 1080p
   2. 720p
   3. 480p
   ...
   Введите номер качества для скачивания (или 0 для отмены): 1
   ```

4. Укажите путь для сохранения файла (или оставьте пустым для сохранения на рабочий стол).

5. Дождитесь завершения загрузки. Итоговый файл будет сохранен в указанном месте.

## Структура проекта

- **`Program.cs`**: Точка входа в приложение.
- **`Services`**: Логика для работы с YouTube API, путями и объединением потоков:
  - **`VideoDownloaderService`**: Управляет процессом скачивания и объединения потоков.
  - **`PathService`**: Утилиты для обработки путей.
  - **`FFmpegService`**: Логика для работы с FFmpeg.
  - **`YoutubeService`**: Создает экземпляр YoutubeClient.
- **`Commands`**: Реализация отдельных команд:
  - **`GetVideoInfoCommand`**: Получение информации о видео.
  - **`DownloadVideoCommand`**: Скачивание видео.
- **`Interfaces`**: Интерфейс `ICommand` для реализации паттерна "Команда".

## Пример работы

```plaintext
Введите URL видео:
https://www.youtube.com/watch?v=example
Название: Example Video
Описание: This is an example description.
Доступные качества:
1. 1080p
2. 720p
3. 480p
Введите номер качества для скачивания (или 0 для отмены): 2
Введите путь для сохранения видео (оставьте пустым для сохранения на рабочий стол):
Начинается скачивание...
Загружается видео...
Загружается аудио...
Объединяем видео и аудио...
Скачивание завершено: /путь/к/файлу/Example Video.mp4
```