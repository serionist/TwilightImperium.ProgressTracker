using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TwilightImperium.ProgressTracker
{
    public class Logger:ILogger
    {
        public Logger(string path)
        {
            FilePath = path;
            string dir = Path.GetDirectoryName(FilePath);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);

            LogCleanupLoop();
        }

        private string FilePath { get; }

        public void Info(string log, Exception ex = null)
        {
            WriteLog("INFO", log, ex);
        }

        public void Warn(string log, Exception ex = null)
        {
            WriteLog("WARN", log, ex);
        }

        public void Error(string log, Exception ex = null)
        {
            WriteLog("ERROR", log, ex);
        }

        public void Debug(string log, Exception ex = null)
        {
            WriteLog("DEBUG", log, ex);
        }
        private SemaphoreSlim _fileLock = new SemaphoreSlim(1);
        private void WriteLog(string level, string log, Exception ex)
        {
            string line = $"{DateTime.Now:G} {level} - {log}";
            if (ex != null)
                line += $" | Exception: {ex}";
            Console.WriteLine(line);
            Task.Run(async () =>
            {
                await _fileLock.WaitAsync();
                try
                {
                    using (var fs = new StreamWriter(new FileStream(FilePath, FileMode.Append, FileAccess.Write)))
                        await fs.WriteLineAsync(line);
                }
                finally
                {
                    _fileLock.Release();
                }
            });
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            switch (logLevel)
            {
                case LogLevel.Debug:
                    Debug(formatter(state, exception), exception);
                    break;
                case LogLevel.Information:
                    Info(formatter(state, exception), exception);
                    break;
                case LogLevel.Warning:
                    Warn(formatter(state, exception), exception);
                    break;
                case LogLevel.Error:
                    Error(formatter(state, exception), exception);
                    break;
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Critical:
                case LogLevel.None:
                case LogLevel.Trace:
                    return false;
                    break;
                case LogLevel.Debug:
                case LogLevel.Information:
                case LogLevel.Warning:
                case LogLevel.Error:
                    return true;
                    break;

            }

            return false;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotImplementedException();
        }


        private static readonly int MaxLogSize = 50 * 1024 * 1024;

        private async void LogCleanupLoop()
        {
            await Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    _fileLock.Wait();
                    try
                    {
                        var size = new FileInfo(FilePath).Length;
                        if (size > MaxLogSize)
                        {
                            var content = File.ReadAllText(FilePath);
                            content = content.Substring(content.Length / 4);
                            File.WriteAllText(FilePath, content);
                        }
                            
                    }
                    catch
                    {
                    }
                    finally
                    {
                        _fileLock.Release();
                    }

                    await Task.Delay(TimeSpan.FromHours(1));
                }
            }, TaskCreationOptions.LongRunning);  
          
        }
    }
}
