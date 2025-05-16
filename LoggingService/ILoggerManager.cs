namespace LoggingService;

public interface ILoggerManager
{
    void LogDebug(string message);
    void LogInformation(string message);
    void LogWarning(string message);
    void LogError(string message);
}