using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZoomlaHms.JsEvent.Implements
{
    public class HwtViewerJsEvent : IJsEvent
    {
        private string filePath;

        public string ChooseFile()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommonFileDialog dialog = new CommonOpenFileDialog();
                dialog.Filters.Add(new CommonFileDialogFilter("未加密的主题包", ".hwt"));

                string initPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemeStudio\\workspace");
                if (!Directory.Exists(initPath))
                { initPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                dialog.InitialDirectory = initPath;

                if (!string.IsNullOrEmpty(filePath))
                {
                    string path = filePath
                        .Replace('\\', Path.DirectorySeparatorChar)
                        .Replace('/', Path.DirectorySeparatorChar);
                    string dirPath = Path.GetDirectoryName(path);

                    if (Directory.Exists(dirPath))
                    { dialog.InitialDirectory = dirPath; }
                }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    SetFilePath(dialog.FileName);
                }
            });

            return filePath;
        }

        public string GetFilePath()
        {
            return filePath;
        }

        public int SetFilePath(string path)
        {
            if (!File.Exists(path) || !path.EndsWith(".hwt"))
            { return -1; }

            filePath = path;
            return 1;
        }

        public void OpenFile()
        {
            if (string.IsNullOrEmpty(filePath))
            {
                EventBridge.Insatance.Prompt("请选择.hwt文件");
                return;
            }
            if (!File.Exists(filePath))
            {
                EventBridge.Insatance.Prompt("文件已被删除或移动");
                return;
            }

            Application.Current.Dispatcher.Invoke(() =>
            {
                new ZipViewer(filePath).Show();
            });
        }
    }
}
