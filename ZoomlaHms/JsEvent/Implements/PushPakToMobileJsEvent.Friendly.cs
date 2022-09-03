using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent.Implements
{
    public partial class PushPakToMobileJsEvent : ClassicJsEvent<PushPakToMobileConfig>
    {
        public string ChoosePakFile()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                if (!string.IsNullOrEmpty(config.ThemePackFile))
                {
                    string initDir = Path.GetDirectoryName(config.ThemePackFile);
                    if (Directory.Exists(initDir))
                    { dialog.InitialDirectory = initDir; }
                }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    config.ThemePackFile = dialog.FileName;
                    config.SaveConfigToFile();
                }
            });

            return config.ThemePackFile;
        }

        public string SetPakFile(string path)
        {
            config.ThemePackFile = path;
            config.SaveConfigToFile();
            return config.ThemePackFile;
        }

        public string GetPakFile()
        {
            return config.ThemePackFile;
        }

        //public string ChooseMobileDirectory()
        //{
        //    return mobileDirectory;
        //}

        public string SetMobileDirectory(string path)
        {
            config.MobileDirectory = path;
            config.SaveConfigToFile();
            return config.MobileDirectory;
        }

        public string GetMobileDirectory()
        {
            return config.MobileDirectory;
        }

        public int CheckPath()
        {
            string themePackFile = config.ThemePackFile
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
            string mobileDirectory = config.MobileDirectory.Replace('\\', '/');

            if (string.IsNullOrEmpty(themePackFile))
            {
                return -10;
            }
            if (!File.Exists(themePackFile))
            {
                return -11;
            }

            if (!mobileDirectory.StartsWith("/") || !mobileDirectory.EndsWith("/"))
            {
                return -1000;
            }
            if (mobileDirectory.Contains("//"))
            {
                return -1001;
            }

            return 1;
        }
    }


    public class PushPakToMobileConfig : ConfigBase
    {
        public override string ConfigName => "PushPakToMobile";

        public string ThemePackFile { get; set; }
        public string MobileDirectory { get; set; }
    }
}
