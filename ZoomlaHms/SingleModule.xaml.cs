using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ZoomlaHms.Common;
using ZoomlaHms.JsEvent;

namespace ZoomlaHms
{
    /// <summary>
    /// SingleModule.xaml 的交互逻辑
    /// </summary>
    public partial class SingleModule : Window
    {
        private static readonly Dictionary<string, SingleModule> opened = new Dictionary<string, SingleModule>();

        private string page;

        public SingleModule(string url)
        {
            lock (opened)
            {
                if (opened.ContainsKey(url))
                {
                    WindowState = WindowState.Minimized;
                    ShowInTaskbar = false;

                    Task.Run(() =>
                    {
                        Thread.Sleep(200);
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Close();
                            Win32Api.SetWindowToForegroundWithAttachThreadInput(opened[url]);
                        });
                    });
                    return;
                }
                else
                {
                    opened.Add(url, this);
                }
            }

            page = url;
            this.Info("ctor", $"SingleModule init with '{url}'.");
            InitializeComponent();

            string indexFile = $"{SystemPath.WebRoot}\\empty.html";
            if (!System.IO.File.Exists(indexFile))
            {
                this.Warning("ctor", "WebRoot directory is corrupt.");
                MessageBox.Show(Application.Current.MainWindow, "文件受损，请重新安装");
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;

                Task.Run(() =>
                {
                    Thread.Sleep(200);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Close();
                    });
                });
                return;
            }
            Browser.Address = indexFile + "?page=" + url;

            Browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            Browser.JavascriptObjectRepository.NameConverter = new CefSharp.JavascriptBinding.LegacyCamelCaseJavascriptNameConverter();
            Browser.JavascriptObjectRepository.Register("CSharp", EventBridge.Insatance);

            Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Info(nameof(Window_Loaded), "SingleModule show.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Info(nameof(Window_Closed), "SingleModule close.");

            lock (opened)
            {
                opened.Remove(page);
            }
        }

        private void Browser_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data == null)
            { return; }

            if (!data.ContainsFileDropList())
            { return; }

            var dropList = data.GetFileDropList();
            if (dropList.Count == 0)
            { return; }

            Browser.EvaluateScriptAsync($"csc('SetFilePath', '{dropList[0].Replace("\\", "\\\\")}')").ContinueWith(state =>
            {
                Browser.EvaluateScriptAsync("csc('OpenFile')");
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Browser.EvaluateScriptAsync("csc('StopTake')");
        }
    }
}
