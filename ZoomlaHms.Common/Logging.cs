using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    public static class Logging
    {
        private readonly static Dictionary<string, Logger> loggers;
        private readonly static Logger globalLogger;

        static Logging()
        {
            globalLogger = new Logger(null);
            loggers = new Dictionary<string, Logger>();
            loggers.Add("_global", globalLogger);
        }


        #region 调试
        public static void Debug(string message)
        {
            globalLogger.Debug(message);
        }
        public static void Debug(string message, Exception exception)
        {
            globalLogger.Debug(message, exception);
        }
        public static void Debug(string module, string message)
        {
            GetOrRegisterLogger(module).Debug(message);
        }
        public static void Debug(string module, string message, Exception exception)
        {
            GetOrRegisterLogger(module).Debug(message, exception);
        }
        #endregion

        #region 消息
        public static void Info(string message)
        {
            globalLogger.Info(message);
        }
        public static void Info(string message, Exception exception)
        {
            globalLogger.Info(message, exception);
        }
        public static void Info(string module, string message)
        {
            GetOrRegisterLogger(module).Info(message);
        }
        public static void Info(string module, string message, Exception exception)
        {
            GetOrRegisterLogger(module).Info(message, exception);
        }
        #endregion

        #region 警告
        public static void Warning(string message)
        {
            globalLogger.Warning(message);
        }
        public static void Warning(string message, Exception exception)
        {
            globalLogger.Warning(message, exception);
        }
        public static void Warning(string module, string message)
        {
            GetOrRegisterLogger(module).Warning(message);
        }
        public static void Warning(string module, string message, Exception exception)
        {
            GetOrRegisterLogger(module).Warning(message, exception);
        }
        #endregion

        #region 错误
        public static void Error(string message)
        {
            globalLogger.Error(message);
        }
        public static void Error(string message, Exception exception)
        {
            globalLogger.Error(message, exception);
        }
        public static void Error(string module, string message)
        {
            GetOrRegisterLogger(module).Error(message);
        }
        public static void Error(string module, string message, Exception exception)
        {
            GetOrRegisterLogger(module).Error(message, exception);
        }
        #endregion

        public static void SyncLogToFile(string module = null)
        {
            if (!string.IsNullOrEmpty(module) && loggers.ContainsKey(module))
            {
                loggers[module].SyncLogToFile();
                return;
            }

            foreach (var item in loggers.Values)
            {
                item.SyncLogToFile();
            }
        }

        public static string[] GetLogFileList(string module = null)
        {
            if (module == null)
            { module = string.Empty; }

            string baseDir = Path.Combine(SystemPath.AppLogsDirectory, module);
            if (!Directory.Exists(baseDir))
            { return Array.Empty<string>(); }

            return Directory.GetFiles(baseDir, "*.log");
        }

        public static string[] FetchLogFromFile(string module = null, DateTime? date = null, int count = 10, bool tail = true)
        {
            if (module == null)
            { module = string.Empty; }
            if (date == null)
            { date = DateTime.Now; }
            if (count == 0 || count < -1)
            { return Array.Empty<string>(); }

            string file = Path.Combine(SystemPath.AppLogsDirectory, module, $"{date.Value:yyyy-MM-dd}.log");
            if (!File.Exists(file))
            { return Array.Empty<string>(); }

            if (count == -1)
            { return File.ReadAllLines(file); }

            Stack<string> frames = new Stack<string>();
            using var read = File.OpenRead(file);
            if (tail)
            {
                List<byte> bytes = new List<byte>();

                long posi = read.Length;
                byte[] buffer;
                while (posi >= 0)
                {
                    read.Position = posi;
                    buffer = new byte[1];
                    read.Read(buffer);

                    if (buffer[0] == 10)
                    {
                        bytes.Reverse();
                        frames.Push(Encoding.UTF8.GetString(bytes.ToArray()));
                        bytes.Clear();

                        if (frames.Count >= count)
                        { break; }
                    }
                    else
                    {
                        bytes.Add(buffer[0]);
                    }

                    posi--;
                }

                if (frames.Count < count && bytes.Count > 0)
                {
                    bytes.Reverse();
                    frames.Push(Encoding.UTF8.GetString(bytes.ToArray()));
                }
            }
            else
            {
                List<byte> bytes = new List<byte>();

                byte[] buffer;
                for (long i = 0; i <= read.Length; i++)
                {
                    buffer = new byte[1];
                    read.Read(buffer);

                    if (buffer[0] == 10)
                    {
                        frames.Push(Encoding.UTF8.GetString(bytes.ToArray()));
                        bytes.Clear();

                        if (frames.Count >= count)
                        { break; }
                    }
                    else
                    {
                        bytes.Add(buffer[0]);
                    }
                }

                if (frames.Count < count && bytes.Count > 0)
                { frames.Push(Encoding.UTF8.GetString(bytes.ToArray())); }
            }

            return frames.ToArray();
        }

        public static void RegisterLogger(string module, string format)
        {
            if (loggers.ContainsKey(module))
            {
                globalLogger.Warning($"Cannot register repeatedly logger '{module}'.");
                return;
            }

            loggers.Add(module, new Logger(module, format));
        }

        private static Logger GetOrRegisterLogger(string module)
        {
            Logger logger;
            if (!loggers.ContainsKey(module))
            {
                logger = new Logger(module);
                loggers.Add(module, logger);
            }
            else
            {
                logger = loggers[module];
            }

            return logger;
        }


        private class Logger
        {
            private readonly Queue<string> writeBuffer = new Queue<string>(50);
            private bool syncing = false;

            public string Module { get; }
            public string Format { get; }

            public Logger(string module) : this(module, null) { }

            public Logger(string module, string format)
            {
                Module = module ?? string.Empty;
                Format = format ?? "[{time}] [{level}] {Thread#{threadId}(threadName)} <{class}>{method}: {message}";

                Task.Run(() =>
                {
                    Thread.Sleep(1000);
                    var now = int.Parse(DateTime.Now.ToString("yyyyMMdd"));
                    string dir = Path.Combine(SystemPath.AppLogsDirectory, Module);
                    if (Directory.Exists(dir))
                    {
                        foreach (var item in Directory.GetFiles(dir))
                        {
                            var time = int.Parse(Path.GetFileNameWithoutExtension(item).Replace("-", ""));
                            if (time - now > 30)
                            { File.Delete(item); }
                        }
                    }

                    while (true)
                    {
                        SyncLogToFile();
                        Thread.Sleep(3000);
                    }
                });
            }

            #region 调试
            public void Debug(string message)
            {
                WriteLog("Debug", message);
            }
            public void Debug(string message, Exception exception)
            {
                Debug($"{message}\n{exception.Message}\n{exception.StackTrace}");
            }
            #endregion

            #region 消息
            public void Info(string message)
            {
                WriteLog("Info", message);
            }
            public void Info(string message, Exception exception)
            {
                Info($"{message}\n{exception.Message}\n{exception.StackTrace}");
            }
            #endregion

            #region 警告
            public void Warning(string message)
            {
                WriteLog("Warning", message);
            }
            public void Warning(string message, Exception exception)
            {
                Warning($"{message}\n{exception.Message}\n{exception.StackTrace}");
            }
            #endregion

            #region 错误
            public void Error(string message)
            {
                WriteLog("Error", message);
            }
            public void Error(string message, Exception exception)
            {
                Error($"{message}\n{exception.Message}\n{exception.StackTrace}");
            }
            #endregion

            public void SyncLogToFile()
            {
                if (syncing)
                { return; }
                syncing = true;

                try
                {
                    string baseDir = Path.Combine(SystemPath.AppLogsDirectory, Module);
                    if (!Directory.Exists(baseDir))
                    {
                        if (string.IsNullOrEmpty(Module))
                        { return; }
                        Directory.CreateDirectory(baseDir);
                    }

                    if (Monitor.TryEnter(writeBuffer, 500))
                    {
                        string[] arr = new string[writeBuffer.Count];
                        for (int i = 0; i < arr.Length; i++)
                        {
                            arr[i] = writeBuffer.Dequeue();
                        }
                        Monitor.Exit(writeBuffer);

                        if (arr.Length > 0)
                        {
                            string logFile = Path.Combine(SystemPath.AppLogsDirectory, Module, $"{DateTime.Now:yyyy-MM-dd}.log");
                            if (File.Exists(logFile))
                            { File.AppendAllText(logFile, string.Join('\n', arr) + '\n'); }
                            else
                            { File.WriteAllText(logFile, string.Join('\n', arr) + '\n'); }
                        }
                    }
                }
                catch (Exception) { }
                syncing = false;
            }

            private void WriteLog(string level, string message)
            {
                MethodBase methodInfo = null;

                var frames = new System.Diagnostics.StackTrace().GetFrames();
                string prefix = typeof(Logging).FullName;
                for (int i = frames.Length - 1; i >= 0; i--)
                {
                    var item = frames[i];
                    var method = item.GetMethod();
                    if ((method.DeclaringType.FullName ?? method.DeclaringType.Name).StartsWith(prefix))
                    { continue; }

                    methodInfo = method;
                    break;
                }
                var currentThread = Thread.CurrentThread;

                string log = Format
                    .Replace("{time}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"))
                    .Replace("{level}", level)
                    .Replace("{threadId}", currentThread.ManagedThreadId.ToString())
                    .Replace("{threadName}", string.IsNullOrEmpty(currentThread.Name) ? "anonymous" : currentThread.Name)
                    .Replace("{class}", methodInfo.DeclaringType.FullName ?? methodInfo.DeclaringType.Name)
                    .Replace("{method}", methodInfo.Name)
                    .Replace("{message}", message)
                    ;

                Monitor.Enter(writeBuffer);
                writeBuffer.Enqueue(log);
                Monitor.Exit(writeBuffer);
            }
        }
    }
}
