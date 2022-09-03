using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using ZoomlaHms.Common;

namespace ZoomlaHms
{
    /// <summary>
    /// LogViewer.xaml 的交互逻辑
    /// </summary>
    public partial class LogViewer : Window
    {
        private ObservableCollection<LogFile> files;

        public LogViewer()
        {
            this.Info("ctor", "LogViewer init.");
            InitializeComponent();

            this.Info("ctor", "Loading log file list...");
            files = new ObservableCollection<LogFile>();
            foreach (var item in Directory.GetFiles(SystemPath.AppLogsDirectory))
            {
                files.Add(new LogFile
                {
                    Name = System.IO.Path.GetFileName(item),
                    Path = item,
                });
            }
            FileList.ItemsSource = files;
            this.Info("ctor", "File list loaded successfully.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Info(nameof(Window_Loaded), "LogViewer show.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Info(nameof(Window_Closed), "LogViewer close.");
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogFile logFile = FileList.SelectedItem as LogFile;
            if (logFile == null)
            { return; }

            this.Info(nameof(ListViewItem_MouseDoubleClick), $"Loading contents of file '{logFile.Name}'...");
            if (!File.Exists(logFile.Path))
            {
                this.Warning(nameof(ListViewItem_MouseDoubleClick), "The file has been moved or deleted.");
                MessageBox.Show(this, "日志文件已被移动或删除");
                files.Remove(logFile);
                return;
            }

            TextRange text = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd);
            text.Text = File.ReadAllText(logFile.Path);
            this.Info(nameof(ListViewItem_MouseDoubleClick), $"Successfully loaded the contents of the file '{logFile.Name}'.");
        }


        private class LogFile
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}
