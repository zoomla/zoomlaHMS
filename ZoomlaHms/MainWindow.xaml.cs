using CefSharp;
using CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZoomlaHms.Common;
using ZoomlaHms.JsEvent;

namespace ZoomlaHms
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            Logging.Info("MianWindow init.");
            InitializeComponent();
            Title += SystemConstant.Version;
            AppTitle.Content = Title;

            InitBrowser();

            if (!System.IO.File.Exists(System.IO.Path.Combine(SystemPath.StartupLocation, ".agreement")))
            {
                Hide();
                new Policy(this).Show();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //var content = this.Content as UIElement;
            //var layer = AdornerLayer.GetAdornerLayer(content);
            //layer.Add(new WindowResizeAdorner(content));

            Logging.Info("MianWindow show.");
            TopPane.DataContext = this;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Logging.Info("MianWindow close.");
        }

        #region 窗体事件
        //窗体拖动事件
        private void TopDragGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            WindowMoving(e.GetPosition(this));
        }
        private void TopDragGrid_PreviewTouchDown(object sender, TouchEventArgs e)
        {
            WindowMoving(e.GetTouchPoint(this).Position);
        }
        private void WindowMoving(Point curposi)
        {
            if (WindowState != WindowState.Maximized)
            {
                return;
            }

            double curOffLeft = curposi.X / ActualWidth;
            double curOffTop = curposi.Y;

            var mousePosi = PointToScreen(curposi);
            Top = mousePosi.Y - curOffTop;
            WindowState = WindowState.Normal;
            Left = mousePosi.X - curOffLeft * ActualWidth;
        }

        private void TopDragGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        private void TopDragGrid_TouchDown(object sender, TouchEventArgs e)
        {
            DragMove();
        }


        //跳转开源仓库
        private void OpenSource_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://gitee.com/zoomla/zoomlaHMS")
                {
                    UseShellExecute = true,
                });
            }
            catch (Exception) { }
        }
        private void OpenSource_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("https://gitee.com/zoomla/zoomlaHMS")
                {
                    UseShellExecute = true,
                });
            }
            catch (Exception) { }
        }

        //帮助按钮
        private void HelpButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Browser.ExecuteScriptAsync("app.switchTo('help')");
        }
        private void HelpButton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            Browser.ExecuteScriptAsync("app.switchTo('help')");
        }

        //最小化按钮
        private void MinimalButton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            WindowMinimize();
        }
        private void MinimalButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowMinimize();
        }
        private void WindowMinimize()
        {
            WindowState = WindowState.Minimized;
        }

        //最大化按钮
        private void MaximalButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowMaximize();
        }
        private void MaximalButton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            WindowMaximize();
        }
        private void WindowMaximize()
        {
            if (WindowState == WindowState.Maximized)
            {
                TopPane.Margin = new Thickness(10);
                WindowState = WindowState.Normal;
            }
            else
            {
                TopPane.Margin = new Thickness(8);
                WindowState = WindowState.Maximized;
            }
        }

        //关闭按钮
        private void CloseButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowClose();
        }
        private void CloseButton_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            WindowClose();
        }
        private void WindowClose()
        {
            Logging.Info("Shutdown application.");
            Application.Current.Shutdown();
        }
        #endregion

        private void InitBrowser()
        {
            string indexFile = $"{SystemPath.WebRoot}\\index.html";
            if (!System.IO.File.Exists(indexFile))
            {
                Logging.Warning("WebRoot directory is corrupt.");
                MessageBox.Show(this, "文件受损，请重新安装");
                return;
            }

            Browser.Address = indexFile;

            Browser.JavascriptObjectRepository.Settings.LegacyBindingEnabled = true;
            Browser.JavascriptObjectRepository.NameConverter = new CefSharp.JavascriptBinding.LegacyCamelCaseJavascriptNameConverter();
            Browser.JavascriptObjectRepository.Register("CSharp", EventBridge.Insatance);
        }

        private async void Browser_Drop(object sender, DragEventArgs e)
        {
            var data = e.Data as DataObject;
            if (data == null)
            { return; }

            if (!data.ContainsFileDropList())
            { return; }

            var dropList = data.GetFileDropList();
            if (dropList.Count == 0)
            { return; }

#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
            Browser.EvaluateScriptAsync($"csc('SetFilePath', '{dropList[0].Replace("\\", "\\\\")}')").ContinueWith(state =>
            {
                Browser.EvaluateScriptAsync("csc('OpenFile')");
            });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
        }
    }
}
