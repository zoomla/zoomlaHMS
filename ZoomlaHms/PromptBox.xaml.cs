using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ZoomlaHms
{
    /// <summary>
    /// PromptBox.xaml 的交互逻辑
    /// </summary>
    public partial class PromptBox : Window
    {
        private PromptBox(PromptBoxSettings settings)
        {
            DataContext = settings;
            InitializeComponent();

            var text = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd);
            text.Text = settings.Message;

            foreach (var item in Content.Document.Blocks)
            {
                item.TextAlignment = TextAlignment.Center;
            }

            if (settings.Ok && !settings.Cancel)
            {
                NoButton.Visibility = Visibility.Collapsed;
                OkButton.Margin = new Thickness(0, 0, 0, 0);
            }
            else if (!settings.Ok && settings.Cancel)
            {
                NoButton.Margin = new Thickness(0, 0, 0, 0);
                OkButton.Visibility = Visibility.Collapsed;
            }
        }

        #region 窗体拖动
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
        #endregion


        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_Closing(object sender, EventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                System.Threading.Thread.Sleep(800);
                Dispatcher.Invoke(() =>
                {
                    OkButton.Focus();
                });
            });
        }


        public static bool? Show(Window owner, string message, bool ok = true, bool cancel = true)
        {
            return Show(owner, null, message, ok, cancel);
        }

        public static bool? Show(Window owner, string title, string message, bool ok = true, bool cancel = true)
        {
            var box = new PromptBox(new PromptBoxSettings
            {
                Title = string.IsNullOrEmpty(title) ? "提示" : title,
                Message = string.IsNullOrEmpty(message) ? "确定操作吗" : message,
                Ok = ok,
                Cancel = cancel,
            });
            box.Owner = owner;
            return box.ShowDialog();
        }


        public class PromptBoxSettings
        {
            public string Title { get; set; }
            public string Message { get; set; }
            public bool Ok { get; set; }
            public bool Cancel { get; set; }
        }
    }
}
