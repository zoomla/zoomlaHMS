using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    public static class Logging
    {
        private static readonly Queue<string> _writeBuffer;
        private static bool _syncing = false;
        static Logging()
        {
            _writeBuffer = new Queue<string>(50);
            Task.Run(() =>
            {
                while (true)
                {
                    SyncLogToFile();
                    Thread.Sleep(3000);
                }
            });
        }

        #region 调试
        public static void Debug(Type raiseType, string method, string message)
        {
            WriteLog("Debug", $"{raiseType?.FullName ?? "<anonymous>"}({method})", message);
        }
        public static void Debug(Type raiseType, string method, string message, Exception exception)
        {
            Debug(raiseType, method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        public static void Debug(this object instance, string method, string message)
        {
            Debug(instance?.GetType(), method, message);
        }
        public static void Debug(this object instance, string method, string message, Exception exception)
        {
            instance.Debug(method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        #endregion

        #region 消息
        public static void Info(Type raiseType, string method, string message)
        {
            WriteLog("Info", $"{raiseType?.FullName ?? "<anonymous>"}({method})", message);
        }
        public static void Info(Type raiseType, string method, string message, Exception exception)
        {
            Info(raiseType, method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        public static void Info(this object instance, string method, string message)
        {
            Info(instance?.GetType(), method, message);
        }
        public static void Info(this object instance, string method, string message, Exception exception)
        {
            instance.Info(method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        #endregion

        #region 警告
        public static void Warning(Type raiseType, string method, string message)
        {
            WriteLog("Warning", $"{raiseType?.FullName ?? "<anonymous>"}({method})", message);
        }
        public static void Warning(Type raiseType, string method, string message, Exception exception)
        {
            Warning(raiseType, method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        public static void Warning(this object instance, string method, string message)
        {
            Warning(instance?.GetType(), method, message);
        }
        public static void Warning(this object instance, string method, string message, Exception exception)
        {
            instance.Warning(method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        #endregion

        #region 错误
        public static void Error(Type raiseType, string method, string message)
        {
            WriteLog("Error", $"{raiseType?.FullName ?? "<anonymous>"}({method})", message);
        }
        public static void Error(Type raiseType, string method, string message, Exception exception)
        {
            Error(raiseType, method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        public static void Error(this object instance, string method, string message)
        {
            Error(instance?.GetType(), method, message);
        }
        public static void Error(this object instance, string method, string message, Exception exception)
        {
            instance.Error(method, $"{message}\n{exception.Message}\n{exception.StackTrace}");
        }
        #endregion


        private static void WriteLog(string level, string module, string message)
        {
            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] [Thread#{Thread.CurrentThread.Name}] {module}: {message}";
            Monitor.Enter(_writeBuffer);
            _writeBuffer.Enqueue(log);
            Monitor.Exit(_writeBuffer);
        }

        public static void SyncLogToFile()
        {
            if (_syncing)
            { return; }
            _syncing = true;
            try
            {
                if (!Directory.Exists(SystemPath.AppLogsDirectory))
                { return; }

                if (Monitor.TryEnter(_writeBuffer, 500))
                {
                    string[] arr = new string[_writeBuffer.Count];
                    for (int i = 0; i < arr.Length; i++)
                    {
                        arr[i] = _writeBuffer.Dequeue();
                    }
                    Monitor.Exit(_writeBuffer);

                    if (arr.Length > 0)
                    {
                        string logFile = Path.Combine(SystemPath.AppLogsDirectory, $"{DateTime.Now:yyyy-MM-dd}.log");
                        if (File.Exists(logFile))
                        { File.AppendAllText(logFile, string.Join('\n', arr) + '\n'); }
                        else
                        { File.WriteAllText(logFile, string.Join('\n', arr) + '\n'); }
                    }
                }
            }
            catch (Exception) { }
            _syncing = false;
        }
    }
}
