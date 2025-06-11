using System;
using System.IO;
using System.Linq;

public static class AppLogger
{
    private static readonly object _lock = new();
    private static readonly string LogDirectory = "logs";
    private static readonly int LogRetentionDays = 7;

    static AppLogger()
    {
        Directory.CreateDirectory(LogDirectory);
        CleanupOldLogs();
    }

    private static string Timestamp => DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

    private static string GetLogFilePath(string logType)
    {
        string fileName = $"{logType}-{DateTime.Now:yyyy-MM-dd}.log";
        return Path.Combine(LogDirectory, fileName);
    }

    private static void WriteToFile(string logType, string message)
    {
        lock (_lock)
        {
            string path = GetLogFilePath(logType);
            File.AppendAllText(path, message + Environment.NewLine);
        }
    }

    private static void PrintColoredLog(string levelTag, ConsoleColor color, string message)
    {
        Console.Write($"[{Timestamp}] ");
        Console.ForegroundColor = color;
        Console.Write(levelTag);
        Console.ResetColor();
        Console.WriteLine($" {message}");
    }

    public static void LogSuccess(string message)
    {
        string log = $"[{Timestamp}] [SUCCESS] {message}";
        PrintColoredLog("[SUCCESS]", ConsoleColor.Green, message);
        WriteToFile("success", log);
    }

    public static void LogError(string message, Exception? ex = null)
    {
        string log = $"[{Timestamp}] [ERROR] {message}";
        PrintColoredLog("[ERROR]", ConsoleColor.Red, message);
        WriteToFile("error", log);

        if (ex != null)
        {
            string exLog = $"[{Timestamp}] [EXCEPTION] {ex.Message}\n{ex.StackTrace}";
            Console.WriteLine(exLog);
            WriteToFile("error", exLog);
        }
    }

    public static void LogDbError(string operation, Exception ex)
    {
        string log = $"[{Timestamp}] [DB ERROR] Thao tác: {operation}";
        PrintColoredLog("[DB ERROR]", ConsoleColor.DarkRed, $"Thao tác: {operation}");
        WriteToFile("db-error", log);

        string exLog = $"[{Timestamp}] [EXCEPTION] {ex.Message}\n{ex.StackTrace}";
        Console.WriteLine(exLog);
        WriteToFile("db-error", exLog);
    }

    public static void LogWarning(string message)
    {
        string log = $"[{Timestamp}] [WARN] {message}";
        PrintColoredLog("[WARN]", ConsoleColor.Yellow, message);
        WriteToFile("warn", log);
    }

    public static void LogInfo(string message)
    {
        string log = $"[{Timestamp}] [INFO] {message}";
        PrintColoredLog("[INFO]", ConsoleColor.Cyan, message);
        WriteToFile("info", log);
    }

    private static void CleanupOldLogs()
    {
        try
        {
            var files = Directory.GetFiles(LogDirectory, "*.log");
            foreach (var file in files)
            {
                var creationDate = File.GetCreationTime(file);
                if ((DateTime.Now - creationDate).TotalDays > LogRetentionDays)
                {
                    File.Delete(file);
                }
            }
        }
        catch (Exception ex)
        {
            Console.Write($"[{Timestamp}] ");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.Write("[CLEANUP ERROR]");
            Console.ResetColor();
            Console.WriteLine($" {ex.Message}");
        }
    }
}
