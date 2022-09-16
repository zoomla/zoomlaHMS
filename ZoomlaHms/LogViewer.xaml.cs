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
        private static LogViewer[] _opend = new LogViewer[] { null };
        private ObservableCollection<LogFile> files = new ObservableCollection<LogFile>();

        public LogViewer()
        {
            lock (_opend)
            {
                if (_opend[0] == null)
                { _opend[0] = this; }
                else
                {
                    _opend[0].Activate();
                    Close();
                    return;
                }
            }

            Logging.Info("LogViewer init.");
            InitializeComponent();

            FileList.ItemsSource = files;
        }

        private void BuildFileList()
        {
            Logging.Info("Loading log file list...");
            files.Clear();
            foreach (var item in Directory.GetFiles(SystemPath.AppLogsDirectory, "*.log"))
            {
                files.Add(new LogFile
                {
                    Name = System.IO.Path.GetFileName(item),
                    Path = item,
                });
            }
            Logging.Info("File list loaded successfully.");
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Logging.Info("LogViewer show.");
            BuildFileList();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            lock (_opend)
            { _opend[0] = null; }

            Logging.Info("LogViewer close.");
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogFile logFile = FileList.SelectedItem as LogFile;
            if (logFile == null)
            { return; }

            Logging.Info($"Loading contents of file '{logFile.Name}'...");
            if (!File.Exists(logFile.Path))
            {
                Logging.Warning("The file has been moved or deleted.");
                MessageBox.Show(this, "日志文件已被移动或删除");
                BuildFileList();
                return;
            }

            TextRange text = new TextRange(Content.Document.ContentStart, Content.Document.ContentEnd);
            text.Text = File.ReadAllText(logFile.Path);
            Logging.Info($"Successfully loaded the contents of the file '{logFile.Name}'.");
        }


        private class LogFile
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }
    }
}
