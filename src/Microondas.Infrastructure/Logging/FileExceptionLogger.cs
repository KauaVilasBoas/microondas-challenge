using Microsoft.Extensions.Logging;

namespace Microondas.Infrastructure.Logging;

public sealed class FileExceptionLogger
{
    private readonly string _logFilePath;
    private readonly ILogger<FileExceptionLogger> _logger;

    public FileExceptionLogger(ILogger<FileExceptionLogger> logger)
    {
        _logger = logger;
        _logFilePath = Path.Combine(AppContext.BaseDirectory, "logs", "exceptions.log");
        Directory.CreateDirectory(Path.GetDirectoryName(_logFilePath)!);
    }

    public async Task LogAsync(Exception exception, string? context = null)
    {
        var entry = BuildLogEntry(exception, context);
        _logger.LogError(exception, "Unhandled exception: {Context}", context);

        await File.AppendAllTextAsync(_logFilePath, entry);
    }

    private static string BuildLogEntry(Exception exception, string? context)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"[{DateTimeOffset.UtcNow:O}]");
        sb.AppendLine($"Context: {context ?? "N/A"}");
        sb.AppendLine($"Exception: {exception.GetType().FullName}");
        sb.AppendLine($"Message: {exception.Message}");
        sb.AppendLine($"StackTrace: {exception.StackTrace}");

        if (exception.InnerException is not null)
        {
            sb.AppendLine($"Inner Exception: {exception.InnerException.GetType().FullName}");
            sb.AppendLine($"Inner Message: {exception.InnerException.Message}");
            sb.AppendLine($"Inner StackTrace: {exception.InnerException.StackTrace}");
        }

        sb.AppendLine(new string('-', 80));
        return sb.ToString();
    }
}
