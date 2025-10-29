using System;
using System.IO;

namespace LU2_software_testen.Utils
{
    public static class Logger
    {
        public static void Log(UserViewModel user, string message)
        {
            // Get the solution root (3 levels up from output directory)
            var baseDir = AppContext.BaseDirectory;
            var solutionDir = Directory.GetParent(baseDir)!.Parent!.Parent!.FullName;

            // Ensure Logs directory exists
            var logsPath = Path.Combine(baseDir, "Logs");
            if (!Directory.Exists(logsPath))
            {
                Directory.CreateDirectory(logsPath);
            }

            // Log file for the current day
            var logFileName = $"log_{DateTime.Now:yyyyMMdd}.txt";
            var logFilePath = Path.Combine(logsPath, logFileName);

            // Log line format: <current time>: <user name>: <log message>
            var logLine = $"{DateTime.Now:HH:mm:ss}: UserID {user.Id}: {message}";

            // Append the log line to the file
            File.AppendAllText(logFilePath, logLine + Environment.NewLine);
        }
    }
}