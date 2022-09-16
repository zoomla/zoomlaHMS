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
        private DateTime lastPush;
        private static readonly string _loggerName = "PushPakToMobile";

        public string ChoosePakFile()
        {
            App.CreateOpenFileDialogInvoke(dialog =>
            {
                if (!string.IsNullOrEmpty(config.ThemePackFile))
                {
                    string initDir = Path.GetDirectoryName(config.ThemePackFile);
                    if (Directory.Exists(initDir))
                    { dialog.InitialDirectory = initDir; }
                }
            }).Invoke(dialog =>
            {
                config.ThemePackFile = dialog.FileName;
                config.SaveConfigToFile();
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

        public string GetLastPushTime() => lastPush.ToString("yyyy-MM-dd HH:mm:ss");
        public void SetLastPushTime(DateTime time) => lastPush = time;

        public List<Tuple<string, int>> GetPushLogFileList()
        {
            var files = Logging.GetLogFileList(_loggerName);
            List<Tuple<string, int>> fileList = new List<Tuple<string, int>>(files.Length);

            foreach (var item in files)
            {
                fileList.Add(new Tuple<string, int>(Path.GetFileNameWithoutExtension(item), File.ReadAllLines(item).Length));
            }

            return fileList;
        }

        public string[] GetPushLog(string date)
        {
            if (string.IsNullOrEmpty(date) || date == "now")
            { return Logging.FetchLogFromFile(module: _loggerName, count: -1); }

            var vs = date.Split('-', StringSplitOptions.RemoveEmptyEntries);
            if (vs.Length != 3)
            { return Array.Empty<string>(); }

            int year, month, day;
            if (!int.TryParse(vs[0], out year) || year < 0)
            { return Array.Empty<string>(); }
            if (!int.TryParse(vs[1], out month) || month < 1 || month > 12)
            { return Array.Empty<string>(); }
            if (!int.TryParse(vs[2], out day) || day < 1 || day > 31)
            { return Array.Empty<string>(); }

            return Logging.FetchLogFromFile(_loggerName, new DateTime(year, month, day), -1);
        }

        public void AddPushLog(int status, string message)
        {
            if (status == 1)
            {
                Logging.Error(_loggerName, $"推送成功。,source={config.ThemePackFile},target={config.MobileDirectory}");
            }
            else
            {
                Logging.Error(_loggerName, $"推送失败。（原因：{message}）,source={config.ThemePackFile},target={config.MobileDirectory}");
            }
        }
    }


    public class PushPakToMobileConfig : ConfigBase
    {
        public override string ConfigName => "PushPakToMobile";

        public string ThemePackFile { get; set; }
        public string MobileDirectory { get; set; } = "/Huawei/Themes/";
    }
}
