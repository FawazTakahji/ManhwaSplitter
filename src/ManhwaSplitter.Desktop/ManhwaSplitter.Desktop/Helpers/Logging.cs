using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ManhwaSplitter.Desktop.Helpers;

public static class Logging
{
    public static void ConfigureNLog()
    {
        LoggingConfiguration config = new();
        FileTarget fileTarget = new()
        {
            FileName = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "logs.txt"),
            DeleteOldFileOnStartup = true
        };
        config.AddTarget("file", fileTarget);
        config.AddRule(new LoggingRule("*", LogLevel.Trace, fileTarget));
        LogManager.Configuration = config;
        LogManager.GetCurrentClassLogger().Info("NLog configured.");
    }
}