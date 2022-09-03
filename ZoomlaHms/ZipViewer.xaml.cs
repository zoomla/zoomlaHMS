using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
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
    /// ZipViewer.xaml 的交互逻辑
    /// </summary>
    public partial class ZipViewer : Window
    {
        private readonly string zipFilePath;
        private ZipArchive zip;
        private MemoryStream zipStream;
        private readonly ObservableCollection<ZipFile> zipFiles = new ObservableCollection<ZipFile>();
        private Stack<string> directory = new Stack<string>();

        public ZipViewer(string zipPath)
        {
            this.Info("ctor", $"ZipViewer init with: {zipPath}");
            InitializeComponent();

            if (!File.Exists(zipPath))
            {
                this.Warning("ctor", "The file has been moved or deleted.");
                MessageBox.Show("文件已被删除或移动");
                WindowState = WindowState.Minimized;
                ShowInTaskbar = false;

                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(200);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Close();
                    });
                });
                return;
            }
            Title = System.IO.Path.GetFileName(zipPath);
            zipFilePath = zipPath;
            Init();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.Info(nameof(Window_Loaded), "ZipViewer show.");
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Info(nameof(Window_Loaded), "ZipViewer close.");
            zip.Dispose();
            zipStream.Dispose();

            if (zipFilePath.StartsWith(SystemPath.TempFileDirectory))
            {
                this.Info(nameof(Window_Loaded), "Clear temporary file.");
                File.Delete(zipFilePath);
            }
        }


        private void Init()
        {
            this.Info(nameof(Init), "Load (or reload) zip file.");
            if (zip != null)
            { zip.Dispose(); }    
            if (zipStream != null)
            { zipStream.Dispose(); }

            using (var fs = File.OpenRead(zipFilePath))
            {
                zipStream = new MemoryStream();
                fs.CopyTo(zipStream);
            }

            zip = new ZipArchive(zipStream, ZipArchiveMode.Read, true);
            FileList.ItemsSource = zipFiles;
            BuildFileList();
        }

        private static readonly string[] _zipFileInZip =
        {
            "com.android.contacts",
            "com.android.incallui",
            "com.android.mms",
            "com.android.phone",
            "com.android.server.telecom",
            "com.android.systemui",
            "com.huawei.android.launcher",
            "com.huawei.mediacontroller",
            "com.huawei.ohos.famanager",
            "com.huawei.ohos.search",
            "com.huawei.phone.recorder",
            "icons",
        };

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var control = sender as ListViewItem;
            if (control == null)
            { return; }

            var data = control.DataContext as ZipFile;
            if (data == null)
            { return; }

            if (data.IsDirectory)
            {
                if (data.Path == "..")
                { directory.Pop(); }
                else
                { directory.Push(data.Name); }

                BuildFileList();
                return;
            }

            this.Info(nameof(ListViewItem_MouseDoubleClick), "Preview file: " + data.Path);
            var tempFilePath = System.IO.Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{data.Name}");
            using (var zipStream = zip.GetEntry(data.Path).Open())
            {
                using var tempFileStream = File.OpenWrite(tempFilePath);
                zipStream.CopyTo(tempFileStream);
                tempFileStream.Flush();
            }
            if (_zipFileInZip.Contains(data.Name))
            {
                new ZipViewer(tempFilePath).Show();
                return;
            }


            try
            {
                var proc = System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo("explorer")
                {
                    Arguments = tempFilePath,
                });
            }
            catch (Exception)
            { }
        }

        private void FileList_KeyDown(object sender, KeyEventArgs e)
        {
            if (FileList.SelectedItems.Count == 0)
            { return;  }

            if (e.Key != Key.Delete)
            { return; }

            if (!File.Exists(zipFilePath))
            { return; }

            using (var fs = File.Open(zipFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Update, true))
                {
                    foreach (ZipFile item in FileList.SelectedItems)
                    {
                        this.Info(nameof(FileList_KeyDown), "Delete file: " + item.Path);
                        if (!item.IsDirectory)
                        {
                            zipArchive.GetEntry(item.Path)?.Delete();
                            continue;
                        }

                        foreach (var entry in zipArchive.Entries.Where(w => w.FullName.StartsWith(item.Path)).ToList())
                        {
                            entry.Delete();
                        }
                    }
                }

                fs.Flush();
            }
            Init();
        }

        private void FileList_Drop(object sender, DragEventArgs e)
        {
            if (!File.Exists(zipFilePath))
            { return; }

            var data = e.Data as DataObject;
            if (data == null)
            { return; }

            if (!data.ContainsFileDropList())
            { return; }

            zipFileAddOverrideAll = false;
            zipFileAddSkipAll = false;
            zipFileAddCancel = false;
            var dropList = data.GetFileDropList();
            using (var fs = File.Open(zipFilePath, FileMode.Open, FileAccess.ReadWrite))
            {
                using (ZipArchive zipArchive = new ZipArchive(fs, ZipArchiveMode.Update, true))
                {
                    string baseZipPath = directory.Count == 0 ? string.Empty : string.Join("/", directory) + "/";
                    this.Info(nameof(FileList_Drop), $"Add '{string.Join("','", dropList)}' to '{(string.IsNullOrEmpty(baseZipPath) ? "/" : baseZipPath)}'.");
                    foreach (var dropItem in dropList)
                    {
                        if (File.Exists(dropItem))
                        {
                            AddFileToZip(dropItem, baseZipPath + System.IO.Path.GetFileName(dropItem), zipArchive);
                        }
                        else if (Directory.Exists(dropItem))
                        {
                            AddDirToZip(dropItem, baseZipPath + System.IO.Path.GetFileName(dropItem) + "/", zipArchive);
                        }
                    }
                }

                fs.Flush();
            }
            Init();
        }

        private bool zipFileAddOverrideAll = false;
        private bool zipFileAddSkipAll = false;
        private bool zipFileAddCancel = false;
        private void AddFileToZip(string diskPath, string zipPath, ZipArchive zip)
        {
            if (zipFileAddCancel)
            { return; }

            var entry = zip.GetEntry(zipPath);
            if (entry != null)
            {
                if (zipFileAddSkipAll)
                { return; }
                if (zipFileAddOverrideAll)
                { entry.Delete(); }

                if (!zipFileAddSkipAll && !zipFileAddOverrideAll)
                {
                    switch (MessageBox.Show("文件重复，是否覆盖？\n是 - 覆盖\n否 - 跳过\n取消 - 中止打包", "提示", MessageBoxButton.YesNoCancel))
                    {
                        case MessageBoxResult.Yes:
                            if (!zipFileAddOverrideAll && MessageBox.Show("是否为之后的所有冲突都执行此操作？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            { zipFileAddOverrideAll = true; }
                            entry.Delete();
                            break;
                        case MessageBoxResult.No:
                            if (!zipFileAddSkipAll && MessageBox.Show("是否为之后的所有冲突都执行此操作？", "提示", MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                            { zipFileAddSkipAll = true; }
                            return;
                        case MessageBoxResult.Cancel:
                        default:
                            zipFileAddCancel = true;
                            return;
                    }
                }
            }
            entry = zip.CreateEntry(zipPath);

            using Stream entryStream = entry.Open();
            using var fileStream = File.OpenRead(diskPath);
            fileStream.CopyTo(entryStream);
            entryStream.Flush();
        }
        private void AddDirToZip(string diskPath, string zipPath, ZipArchive zip)
        {
            var dirs = Directory.GetDirectories(diskPath);
            foreach (var item in dirs)
            { AddDirToZip(item, zipPath + System.IO.Path.GetFileName(item) + "/", zip); }

            var files = Directory.GetFiles(diskPath);
            foreach (var item in files)
            { AddFileToZip(item, zipPath + System.IO.Path.GetFileName(item), zip); }
        }

        private void BuildFileList()
        {
            string dir = directory.Count == 0 ? string.Empty : string.Join("/", directory.Reverse()) + "/";
            this.Info(nameof(BuildFileList), $"List of files under '{(string.IsNullOrEmpty(dir) ? "/" : dir)}'.");

            var dirList = new List<ZipFile>();
            var fileList = new List<ZipFile>();
            foreach (var item in zip.Entries)
            {
                if (string.IsNullOrEmpty(dir))
                {
                    if (item.FullName.IndexOf('/') == item.FullName.Length - 1)
                    {
                        var add = new ZipFile(null)
                        {
                            IsDirectory = true,
                            Name = item.FullName.TrimEnd('/'),
                            Length = 0,
                            Path = item.FullName,
                            UpdateTime = new DateTime(item.LastWriteTime.Ticks),
                        };
                        if (!dirList.Where(w => w.Path == add.Path).Any())
                        { dirList.Add(add); }
                    }
                    else if (item.FullName.IndexOf('/') == -1)
                    {
                        fileList.Add(new ZipFile(item)
                        {
                            IsDirectory = false,
                            Name = item.Name,
                            Length = item.Length,
                            Path = item.FullName,
                            UpdateTime = new DateTime(item.LastWriteTime.Ticks),
                        });
                    }
                    
                    if (item.FullName.Contains('/'))
                    {
                        string dirPath = item.FullName.EndsWith("/") ? item.FullName : item.FullName.Substring(0, item.FullName.LastIndexOf('/') + 1);
                        string[] levels = dirPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                        string firstLv = levels[0];
                        if (!dirList.Where(w => w.Name == firstLv).Any())
                        {
                            dirList.Add(new ZipFile(null)
                            {
                                IsDirectory = true,
                                Name = firstLv,
                                Length = 0,
                                Path = firstLv + "/",
                                UpdateTime = new DateTime(item.LastWriteTime.Ticks),
                            });
                        }
                    }

                    continue;
                }

                if (!item.FullName.StartsWith(dir) || item.FullName == dir)
                { continue; }

                string fileName = item.FullName.Substring(dir.Length);
                if (fileName.IndexOf('/') == fileName.Length - 1)
                {
                    var add = new ZipFile(null)
                    {
                        IsDirectory = true,
                        Name = fileName.TrimEnd('/'),
                        Length = 0,
                        Path = item.FullName,
                        UpdateTime = new DateTime(item.LastWriteTime.Ticks),
                    };
                    if (!dirList.Where(w => w.Name == add.Name).Any())
                    { dirList.Add(add); }
                }
                else if (fileName.IndexOf('/') == -1)
                {
                    fileList.Add(new ZipFile(item)
                    {
                        IsDirectory = false,
                        Name = item.Name,
                        Length = item.Length,
                        Path = item.FullName,
                        UpdateTime = new DateTime(item.LastWriteTime.Ticks),
                    });
                }

                if (fileName.Contains('/'))
                {
                    string dirPath = fileName.EndsWith("/") ? fileName : fileName.Substring(0, fileName.LastIndexOf('/') + 1);
                    string[] levels = dirPath.Split('/', StringSplitOptions.RemoveEmptyEntries);

                    string firstLv = levels[0];
                    if (!dirList.Where(w => w.Name == firstLv).Any())
                    {
                        dirList.Add(new ZipFile(null)
                        {
                            IsDirectory = true,
                            Name = firstLv,
                            Length = 0,
                            Path = dir + firstLv + "/",
                            UpdateTime = new DateTime(item.LastWriteTime.Ticks),
                        });
                    }
                }
            }

            zipFiles.Clear();
            if (directory.Count > 0)
            {
                zipFiles.Add(new ZipFile(null)
                {
                    IsDirectory = true,
                    Name = ".. (返回上级)",
                    Length = 0,
                    Path = "..",
                    UpdateTime = DateTime.Now,
                });
            }

            foreach (var item in dirList.OrderBy(f => f.Name))
            { zipFiles.Add(item); }
            foreach (var item in fileList.OrderBy(f => f.Name))
            { zipFiles.Add(item); }
        }


        private class ZipFile
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public bool IsDirectory { get; set; }
            public long Length { get; set; }
            public DateTime UpdateTime { get; set; }

            private ZipArchiveEntry refer;

            public ZipFile(ZipArchiveEntry refEntry)
            {
                refer = refEntry;
            }

            public string ViewName
            {
                get
                {
                    if (Name.Length > 15)
                    { return Name.Substring(0, 15) + "..."; }
                    return Name;
                }
            }
            public string ViewLength
            {
                get
                {
                    if (Length < 1024L)
                    { return Length + " B"; }
                    else if (Length >= 1024L && Length < 1024L * 1024)
                    { return (Length / 1024D).ToString("F2") + " KB"; }
                    else if (Length >= 1024L * 1024 && Length < 1024L * 1024 * 1024)
                    { return (Length / 1024D / 1024).ToString("F2") + " MB"; }
                    else if (Length >= 1024L * 1024 * 1024 && Length < 1024L * 1024 * 1024 * 1024)
                    { return (Length / 1024D / 1024 / 1024).ToString("F2") + " GB"; }
                    else if (Length >= 1024L * 1024 * 1024  * 1024 && Length < 1024L * 1024 * 1024 * 1024 * 1024)
                    { return (Length / 1024D / 1024 / 1024 / 1024).ToString("F2") + " TB"; }

                    return "Infinite";
                }
            }
            public string ViewUpdateTime => UpdateTime.ToString("yyyy-MM-dd HH:mm:ss");
            public ImageSource PreviewImage
            {
                get
                {
                    const string
                        _FOLDER = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAFFmlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDYwLCAyMDIwLzA1LzEyLTE2OjA0OjE3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMiAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIyLTA4LTIwVDE1OjQ0OjEzKzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMi0wOC0yMFQxNTo0NjowNCswODowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMi0wOC0yMFQxNTo0NjowNCswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDpiZDdkYmJmMS1lMGMwLTA4NDktODUwNi0wMTE0MzkzMmY3ZDQiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6YmQ3ZGJiZjEtZTBjMC0wODQ5LTg1MDYtMDExNDM5MzJmN2Q0IiB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InhtcC5kaWQ6YmQ3ZGJiZjEtZTBjMC0wODQ5LTg1MDYtMDExNDM5MzJmN2Q0Ij4gPHhtcE1NOkhpc3Rvcnk+IDxyZGY6U2VxPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0iY3JlYXRlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDpiZDdkYmJmMS1lMGMwLTA4NDktODUwNi0wMTE0MzkzMmY3ZDQiIHN0RXZ0OndoZW49IjIwMjItMDgtMjBUMTU6NDQ6MTMrMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4yIChXaW5kb3dzKSIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz77jVbXAAABSUlEQVR42u3ZUQ2DMBSFYSRUQiUgYRImAQlIwAESJgEJSJiESUACo0mbLUuXjSy7p8Df5DwthPTj9rbNqnmeqyOnAgAAAAAAAAAAAAAAAAAAAAAAAAAAAIAPDzyGW3L+MnVlNCwAwmSuS6bwvhXp9wJwWznx5wQ4v3WA6QeAlCZW0jdxewRYmzH2ksMCpPijA5wAKBwg/HZZ0i1p/5Cil0C/tmPvZRuc4td5Hemk2BrFqwCazORbQa8I7/PWALnjbSNsko0lwDWz5r1wlzCtgOlNRx6FX7+17AHnN+teNfnRchfoMpOvxaVfWwK4zHYnL31LgNfRlVD6KoBTKaWvAHBxKyyi9BUAvXDyg/IukEp/Lqn0rQEG5XFXfRtUln9fwnU4NcAhNkGLDHHZOTkA/w0CAAAAAAAAAAAAAAAAAAAAAAAAAACw7dwB1+ZcclayWl8AAAAASUVORK5CYII=",
                        _XML = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAFFmlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDYwLCAyMDIwLzA1LzEyLTE2OjA0OjE3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMiAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIyLTA4LTIwVDE1OjMyOjA3KzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMi0wOC0yMFQxNTo0Mjo1NyswODowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMi0wOC0yMFQxNTo0Mjo1NyswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDowZDBiZDE4YS1lY2Q3LWM1NGMtODEyZi0yNTE4MzhmNjM4YWEiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6MGQwYmQxOGEtZWNkNy1jNTRjLTgxMmYtMjUxODM4ZjYzOGFhIiB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InhtcC5kaWQ6MGQwYmQxOGEtZWNkNy1jNTRjLTgxMmYtMjUxODM4ZjYzOGFhIj4gPHhtcE1NOkhpc3Rvcnk+IDxyZGY6U2VxPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0iY3JlYXRlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDowZDBiZDE4YS1lY2Q3LWM1NGMtODEyZi0yNTE4MzhmNjM4YWEiIHN0RXZ0OndoZW49IjIwMjItMDgtMjBUMTU6MzI6MDcrMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4yIChXaW5kb3dzKSIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz4v7WlgAAABwElEQVR42u3a0ZGDIBAG4L8ES6AESrAES0gJlmAHlmAJlkAJlmAJlMA9HM5we6smZnIn8DOzDwkTJ3zgLpgghICaAwQgAAEIQAACEIAABCAAAa5fBGgADACmN8Liu1kAIwCDk3YngBlAeDO6OK42vvYJyu0BwgcAThFqADhEqAVgF6EmABWhNoBfCDUC/ECoFWBDMDUDBAB9qQAbwlnY0gBGvNhKA9gQ2ifDlAjwSkwEKARgiQlwALDWCDAkea0rFcAfzG6bAEzxPVcSgI8z28TlLvvStkYEAOhLAXgkAzSib076bARolBWRLUAvZtjGWdd2f0Psa8Rn5lwBxpPBB/Hg0yVVQbYlN4BJzKRRkmC6/BvR5wSAlj9uC7Aqy3i5UP5mBSGbFdCKL69ldHNwn3uRH7Rr3BpAe4g5id2fLH+vDD6LJOiVX3cmJUF2B2Vz6/e5lsFVyeheKX97yc/uDD6rjZATS1nW+nUnf5iTw1FWZwEXl7a2+9NunXanchRxGnycJLfij8PmiW1u0QAuzrw5SHB8JEYAAhCAADkBrP8AMNwJYNu5+T8KB6DhHyUJQAACEIAABCAAAQhAAAJcjS+N2HTy2wNf+wAAAABJRU5ErkJggg==",
                        _ZIP = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAFFmlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDYwLCAyMDIwLzA1LzEyLTE2OjA0OjE3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMiAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIyLTA4LTIwVDE1OjQyOjU3KzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMi0wOC0yMFQxNTo0NDoxMyswODowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMi0wOC0yMFQxNTo0NDoxMyswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDpkOTA2OWIwMS0wYmMwLWNkNGEtYmYwYi1hMWMwNTFiMTllYWMiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6ZDkwNjliMDEtMGJjMC1jZDRhLWJmMGItYTFjMDUxYjE5ZWFjIiB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InhtcC5kaWQ6ZDkwNjliMDEtMGJjMC1jZDRhLWJmMGItYTFjMDUxYjE5ZWFjIj4gPHhtcE1NOkhpc3Rvcnk+IDxyZGY6U2VxPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0iY3JlYXRlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDpkOTA2OWIwMS0wYmMwLWNkNGEtYmYwYi1hMWMwNTFiMTllYWMiIHN0RXZ0OndoZW49IjIwMjItMDgtMjBUMTU6NDI6NTcrMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4yIChXaW5kb3dzKSIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6KJ00BAAABmUlEQVR42u3a0ZGDIBAG4L8ESqAES7gSKMESLMEOLMESLMESLMESKIG8rHPMHaIxuYuwPzP7kMHJyBdZFgxCCNAcIAABCEAAAhCAAAQgAAEIcP1LAAOgBzBmYsB3axP9jfQ1cq3BQbsTwAQgHMQa3fuY6HfS9yWffYRyewCfGbiXwa8ArMQZgEOEOwHkfvlO7tceXJcCyCKUAjAJQidze5Cn4SzALkIpAFdzwCFCKQCDDKoFMEusFwB+IWjJAbsI2qbATwRbCsAiiXCSQbqduuEZgACgq3UKmAgqF00pAHNU7rZRMkwly6dabTnARyvGmbAlTQEjmdtHEV6MsbYcQAACEIAABKgRwEXrfyNr+KIJwCaKOKcFwO9UsUYLwJwp5b0GgNwGZ9YA4DIAgwYAG2X/PtoYvSMRFgGwDdQnEqOtHcBnHnWnCcAmDka284Hqp0AfJb1OjsNM5lSo+lL4XStAUQDb+8HtveCibTP0V0EAAhCAALcBWD8A0N8JYDvm8v8UMwDDP0oSgAAEIAABCEAAAhCAAAS4Gg/mz5Dn0qEPIwAAAABJRU5ErkJggg==",
                        _FILE = "iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAACXBIWXMAAAsTAAALEwEAmpwYAAAFFmlUWHRYTUw6Y29tLmFkb2JlLnhtcAAAAAAAPD94cGFja2V0IGJlZ2luPSLvu78iIGlkPSJXNU0wTXBDZWhpSHpyZVN6TlRjemtjOWQiPz4gPHg6eG1wbWV0YSB4bWxuczp4PSJhZG9iZTpuczptZXRhLyIgeDp4bXB0az0iQWRvYmUgWE1QIENvcmUgNi4wLWMwMDIgNzkuMTY0NDYwLCAyMDIwLzA1LzEyLTE2OjA0OjE3ICAgICAgICAiPiA8cmRmOlJERiB4bWxuczpyZGY9Imh0dHA6Ly93d3cudzMub3JnLzE5OTkvMDIvMjItcmRmLXN5bnRheC1ucyMiPiA8cmRmOkRlc2NyaXB0aW9uIHJkZjphYm91dD0iIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1sbnM6cGhvdG9zaG9wPSJodHRwOi8vbnMuYWRvYmUuY29tL3Bob3Rvc2hvcC8xLjAvIiB4bWxuczp4bXBNTT0iaHR0cDovL25zLmFkb2JlLmNvbS94YXAvMS4wL21tLyIgeG1sbnM6c3RFdnQ9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9zVHlwZS9SZXNvdXJjZUV2ZW50IyIgeG1wOkNyZWF0b3JUb29sPSJBZG9iZSBQaG90b3Nob3AgMjEuMiAoV2luZG93cykiIHhtcDpDcmVhdGVEYXRlPSIyMDIyLTA4LTIwVDE1OjExOjIzKzA4OjAwIiB4bXA6TW9kaWZ5RGF0ZT0iMjAyMi0wOC0yMFQxNTozMjowNyswODowMCIgeG1wOk1ldGFkYXRhRGF0ZT0iMjAyMi0wOC0yMFQxNTozMjowNyswODowMCIgZGM6Zm9ybWF0PSJpbWFnZS9wbmciIHBob3Rvc2hvcDpDb2xvck1vZGU9IjMiIHBob3Rvc2hvcDpJQ0NQcm9maWxlPSJzUkdCIElFQzYxOTY2LTIuMSIgeG1wTU06SW5zdGFuY2VJRD0ieG1wLmlpZDpiZjM2NzE4Yy02ZjYzLTY5NDEtYjgwNi0yY2ZhODhkYzg1NjkiIHhtcE1NOkRvY3VtZW50SUQ9InhtcC5kaWQ6YmYzNjcxOGMtNmY2My02OTQxLWI4MDYtMmNmYTg4ZGM4NTY5IiB4bXBNTTpPcmlnaW5hbERvY3VtZW50SUQ9InhtcC5kaWQ6YmYzNjcxOGMtNmY2My02OTQxLWI4MDYtMmNmYTg4ZGM4NTY5Ij4gPHhtcE1NOkhpc3Rvcnk+IDxyZGY6U2VxPiA8cmRmOmxpIHN0RXZ0OmFjdGlvbj0iY3JlYXRlZCIgc3RFdnQ6aW5zdGFuY2VJRD0ieG1wLmlpZDpiZjM2NzE4Yy02ZjYzLTY5NDEtYjgwNi0yY2ZhODhkYzg1NjkiIHN0RXZ0OndoZW49IjIwMjItMDgtMjBUMTU6MTE6MjMrMDg6MDAiIHN0RXZ0OnNvZnR3YXJlQWdlbnQ9IkFkb2JlIFBob3Rvc2hvcCAyMS4yIChXaW5kb3dzKSIvPiA8L3JkZjpTZXE+IDwveG1wTU06SGlzdG9yeT4gPC9yZGY6RGVzY3JpcHRpb24+IDwvcmRmOlJERj4gPC94OnhtcG1ldGE+IDw/eHBhY2tldCBlbmQ9InIiPz6gdmSfAAAB7ElEQVR42u3a7VGEMBAGYEqwA50RJPgH6cASLMESLOE6oARLsARKuBKuBEqIm5DEICC5MzrJ3stMxq8bZ/Yh2XxsCillcc2tAAAAAAAAAAAAAAAAAAAAcPk/MU97Xz93VTN0pTjSV3lh64vAJymAthRvXSXGXwTutyErgOnNRws+GCEZAOryH3GCXiAOmQA0p1hvXeWAUIR0ACJ3+1AEtgChCKwBQhDYA+whMAQQ49pY30Lg2AMkrSkOQQhl884SwFsPDCtt9hnGAGENAAAAAAAAAAAAAAAAAABAqgB6U9OrU+O2ql+3D0/FqHaB+mj9jJpC4gAU1EPzomsG1eOT29bSNna+rRXHH/+eLQC97a+Cidnr1/Vde9veuONv+oz6WQFYLDYAXkDuyFxhzA43KFg9NBQIfc8LQL1tam44UKBeDfGw7DEcAahr6zFuur7tAdNb5w5AAetgzRjX2Z3a97zAF8BkdjfGTca/niFgE54399sT3+UBJ0MAe8avE6HK9gSih8N0icIGfTIzQm+TpJo97O/2Su45LIVnlZ4pF3iVZLNW2Hr2VoX5lMdNAoxdO2B4QeK8OmJiV2T+F0DlirQuSa1NbX/X+uRuibkktzbFRawX2j1GUgC4KQoAAAAAAAAAAAAAAAAAZNc+AfUR3WMFpZLfAAAAAElFTkSuQmCC";

                    string[] imgExt = new[] { ".jpg", ".png" };

                    MemoryStream stream;
                    if (IsDirectory)
                    {
                        stream = new MemoryStream(Convert.FromBase64String(_FOLDER));
                    }
                    else if (_zipFileInZip.Contains(Name) || System.IO.Path.GetExtension(Name)?.ToLower() == ".zip")
                    {
                        stream = new MemoryStream(Convert.FromBase64String(_ZIP));
                    }
                    else if (System.IO.Path.GetExtension(Name)?.ToLower() == ".xml")
                    {
                        stream = new MemoryStream(Convert.FromBase64String(_XML));
                    }
                    else if (imgExt.Contains(System.IO.Path.GetExtension(Name)?.ToLower()))
                    {
                        using var source = refer.Open();
                        stream = new MemoryStream();
                        source.CopyTo(stream);
                        stream.Seek(0, SeekOrigin.Begin);
                    }
                    else
                    {
                        stream = new MemoryStream(Convert.FromBase64String(_FILE));
                    }

                    return new ImageSourceConverter().ConvertFrom(stream) as ImageSource;
                }
            }
        }
    }
}
