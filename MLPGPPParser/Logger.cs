using System.Text;

namespace MLPGPPParser;

public class Logger : IDisposable {
    private static Logger? _instance;
    private readonly string _logFilePath;
    private readonly StreamWriter _fileWriter;
    private readonly object _lockObject = new object();

    private Logger(string logFolderPath) {
        // Create logs folder if it doesn't exist
        Directory.CreateDirectory(logFolderPath);

        // Create log file with timestamp
        var timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        var logFileName = $"{timestamp}.log";
        _logFilePath = Path.Combine(logFolderPath, logFileName);

        // Initialize file writer
        _fileWriter = new StreamWriter(_logFilePath, false, Encoding.UTF8) {
            AutoFlush = true
        };

        WriteToFile($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Logger initialized");
    }

    /// <summary>
    /// Initializes the logger with the specified log folder path
    /// </summary>
    public static void Initialize(string logFolderPath) {
        if (_instance != null) {
            _instance.Dispose();
        }
        _instance = new Logger(logFolderPath);
    }

    /// <summary>
    /// Gets the current instance of the logger. Must be initialized first.
    /// </summary>
    public static Logger Instance {
        get {
            if (_instance == null) {
                throw new InvalidOperationException("Logger not initialized. Call Initialize() first.");
            }
            return _instance;
        }
    }

    /// <summary>
    /// Logs an informational message in green
    /// </summary>
    public static void LogInfo(string message) {
        Instance.WriteToConsoleAndFile(message, ConsoleColor.Green);
    }

    /// <summary>
    /// Logs a warning message in yellow
    /// </summary>
    public static void LogWarning(string message) {
        Instance.WriteToConsoleAndFile($"warning: {message}", ConsoleColor.Yellow);
    }

    /// <summary>
    /// Logs an error message in red
    /// </summary>
    public static void LogError(string message) {
        Instance.WriteToConsoleAndFile($"error: {message}", ConsoleColor.Red);
    }

    /// <summary>
    /// Logs a task execution message and monitors progress
    /// </summary>
    public static void LogTaskStart(string message) {
        Instance.WriteToConsoleAndFile(message, ConsoleColor.White);
    }

    /// <summary>
    /// Logs task progress
    /// </summary>
    public static void LogTaskProgress(TimeSpan elapsed) {
        var message = $"\rExecuting task for {elapsed:g} time";
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(message);
        Console.ResetColor();
        Instance.WriteToFileWithoutTimestamp(message);
    }

    /// <summary>
    /// Logs task completion
    /// </summary>
    public static void LogTaskComplete(TimeSpan elapsed) {
        Instance.WriteToConsoleAndFile($"\rExecuted task for {elapsed:g} time ", ConsoleColor.Green);
    }

    private void WriteToConsoleAndFile(string message, ConsoleColor color) {
        lock (_lockObject) {
            Console.ForegroundColor = color;
            Console.WriteLine(message);
            Console.ResetColor();
            WriteToFile(message);
        }
    }

    private void WriteToFile(string message) {
        try {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            _fileWriter.WriteLine($"[{timestamp}] {message}");
        } catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            Console.ResetColor();
        }
    }

    private void WriteToFileWithoutTimestamp(string message) {
        try {
            _fileWriter.WriteLine(message);
        } catch (Exception ex) {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error writing to log file: {ex.Message}");
            Console.ResetColor();
        }
    }

    public void Dispose() {
        _fileWriter?.Dispose();
    }
}

