using TaskFlow.Application.Services.Interfaces;

namespace TaskFlow.Infrastructure.Services;

/// <summary>
/// Local filesystem implementation of IFileStorageService.
/// Stores files under the platform-appropriate application data directory.
/// </summary>
public class FileStorageService : IFileStorageService
{
    private readonly string _baseDir;

    /// <summary>
    /// Initializes the service and ensures data directories exist.
    /// </summary>
    public FileStorageService()
    {
        _baseDir = GetPlatformDataDirectory();
        Directory.CreateDirectory(Path.Combine(_baseDir, "Uploads"));
        Directory.CreateDirectory(Path.Combine(_baseDir, "VisualPdfs"));
    }

    /// <inheritdoc/>
    public async Task<string> SaveFileAsync(string subfolder, string fileName, Stream fileStream)
    {
        var dir = Path.Combine(_baseDir, subfolder);
        Directory.CreateDirectory(dir);

        var filePath = Path.Combine(dir, fileName);
        await using var fs = new FileStream(filePath, FileMode.Create);
        await fileStream.CopyToAsync(fs);
        return filePath;
    }

    /// <inheritdoc/>
    public Task DeleteFileAsync(string filePath)
    {
        if (File.Exists(filePath))
            File.Delete(filePath);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public string GetDataDirectory() => _baseDir;

    /// <inheritdoc/>
    public string GetUploadsDirectory() => Path.Combine(_baseDir, "Uploads");

    /// <inheritdoc/>
    public string GetVisualPdfsDirectory() => Path.Combine(_baseDir, "VisualPdfs");

    /// <inheritdoc/>
    public async Task<string> CopyToVisualFolderAsync(string sourcePath, string fileName)
    {
        var destPath = Path.Combine(GetVisualPdfsDirectory(), fileName);
        using var sourceStream = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);
        await using var destStream = new FileStream(destPath, FileMode.Create);
        await sourceStream.CopyToAsync(destStream);
        return destPath;
    }

    /// <summary>
    /// Resolves the platform-appropriate data directory.
    /// Windows: %APPDATA%/TaskFlow/Data
    /// Linux: ~/.local/share/TaskFlow/Data
    /// </summary>
    private static string GetPlatformDataDirectory()
    {
        if (OperatingSystem.IsWindows())
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            return Path.Combine(appData, "TaskFlow", "Data");
        }
        else
        {
            var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            return Path.Combine(home, ".local", "share", "TaskFlow", "Data");
        }
    }
}
