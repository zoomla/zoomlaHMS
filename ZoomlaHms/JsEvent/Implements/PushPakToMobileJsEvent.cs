using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent.Implements
{
    public partial class PushPakToMobileJsEvent : IJsEvent, IDisposable
    {
        private Process proc;
        private const int HISTORY_MAX = 50;
        private Queue<string> historyOutput;
        private const int HISTORY_BUFFER_MAX = 15;
        private Queue<string> historyReadBuffer;
        private bool running = false;
        private bool disposedValue;

        public PushPakToMobileJsEvent()
        {
            historyOutput = new Queue<string>((int)(HISTORY_MAX * 1.5));
            historyReadBuffer = new Queue<string>((int)(HISTORY_BUFFER_MAX * 1.2));

            Task.Run(() =>
            {
                while (true)
                {
                    if (proc == null || !running)
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    if (proc.StandardOutput.EndOfStream)
                    { continue; }

                    List<char> chars = new List<char>();
                    int charInt = 0;
                    while (proc.StandardOutput.Peek() > -1)
                    {
                        charInt = proc.StandardOutput.Read();
                        chars.Add((char)charInt);
                    }

                    string content = string.Concat(chars);
                    string[] conarr = content.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                    if (conarr.Length == 0)
                    { continue; }

                    Monitor.Enter(historyOutput);
                    if (historyOutput.Count > HISTORY_MAX)
                    {
                        for (int i = 0; i < HISTORY_MAX - historyOutput.Count; i++)
                        { historyOutput.Dequeue(); }
                    }
                    foreach (var item in conarr)
                    { historyOutput.Enqueue(item); }
                    Monitor.Exit(historyOutput);

                    Monitor.Enter(historyReadBuffer);
                    if (historyReadBuffer.Count > HISTORY_BUFFER_MAX)
                    {
                        for (int i = 0; i < HISTORY_BUFFER_MAX - historyReadBuffer.Count; i++)
                        { historyReadBuffer.Dequeue(); }
                    }
                    foreach (var item in conarr)
                    { historyReadBuffer.Enqueue(item); }
                    Monitor.Exit(historyReadBuffer);
                }
            });

            Application.Current.Dispatcher.ShutdownStarted += (sender, e) =>
            {
                if (proc != null && !proc.HasExited && !disposedValue)
                {
                    running = false;
                    proc.Kill();
                }
            };
        }

        public List<string> GetHistory()
        {
            return historyOutput.ToList();
        }

        public List<string> GetNewestHistory()
        {
            Monitor.Enter(historyReadBuffer);
            var list = historyReadBuffer.ToList();
            historyReadBuffer.Clear();
            Monitor.Exit(historyReadBuffer);

            return list;
        }

        public void StartProc()
        {
            if (running)
            { return; }

            if (proc != null)
            {
                proc.Start();
                running = true;
                return;
            }

            ProcessStartInfo procInfo = new ProcessStartInfo(Path.Combine(SystemPath.ExternalToolsDirectory, "mtpaccess.exe"))
            {
                CreateNoWindow = true,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
            };

            proc = new Process()
            {
                StartInfo = procInfo,
                EnableRaisingEvents = true,
            };
            proc.Exited += Proc_Exited;
            proc.Start();
            running = true;
        }

        public void StopProc()
        {
            if (proc == null)
            { return; }

            proc.Kill(true);
        }

        public int IsRunning()
        {
            return running ? 1 : 0;
        }

        public void ExecuteCommand(string command)
        {
            if (!running)
            { return; }

            proc.StandardInput.Write(command);
            proc.StandardInput.Write("\n");
            proc.StandardInput.Flush();
        }


        private void Proc_Exited(object? sender, EventArgs e)
        {
            historyOutput.Clear();
            historyReadBuffer.Clear();
            running = false;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    if (!proc.HasExited)
                    { proc.Kill(); }
                    proc.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~PushPakToMobileJsEvent()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
