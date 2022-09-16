using Microsoft.VisualBasic;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent.Implements
{
    public class ThemeAutoPackJsEvent : ClassicJsEvent<ThemeAutoPackConfig>
    {
        private static readonly string _loggerName = "ThemeAutoPack";

        private AbstractListener listener;
        private bool syncing = false;
        private DateTime lastPack = DateTime.MinValue;

        public ThemeAutoPackJsEvent()
        {
            Logging.RegisterLogger(_loggerName, "{time} => {message}");
        }

        public override string SetConfig(string json)
        {
            base.SetConfig(json);

            bool resav = false;
            if (config.InSecondSleep < 0)
            {
                config.InSecondSleep = 0;
                resav = true;
            }
            if (config.WatchFileSleep < 0)
            {
                config.WatchFileSleep = 0;
                resav = true;
            }
            if (resav)
            {
                config.SaveConfigToFile();
            }

            return config.SaveConfigToJson();
        }

        public string ChooseThemeFolder()
        {
            App.CreateOpenFileDialogInvoke(dialog =>
            {
                dialog.IsFolderPicker = true;
                if (string.IsNullOrEmpty(config.ThemeFolder) || !Directory.Exists(config.ThemeFolder))
                {
                    string initPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemeStudio\\workspace");
                    if (!Directory.Exists(initPath))
                    { initPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }

                    dialog.InitialDirectory = initPath;
                }
                else
                {
                    dialog.InitialDirectory = config.ThemeFolder;
                }
            }).Invoke(dialog =>
            {
                config.ThemeFolder = dialog.FileName;
                config.SaveConfigToFile();
            });

            return config.ThemeFolder;
        }

        public string ChooseThemePackage()
        {
            App.CreateOpenFileDialogInvoke(dialog =>
            {
                string initPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ThemeStudio\\workspace");
                if (!Directory.Exists(initPath))
                { initPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                dialog.InitialDirectory = initPath;

                if (!string.IsNullOrEmpty(config.ThemePackage))
                {
                    string path = config.ThemePackage
                        .Replace('\\', Path.DirectorySeparatorChar)
                        .Replace('/', Path.DirectorySeparatorChar);
                    string dirPath = Path.GetDirectoryName(path);

                    if (Directory.Exists(dirPath))
                    { dialog.InitialDirectory = dirPath; }
                }
            }).Invoke(dialog =>
            {
                config.ThemePackage = dialog.FileName;
                config.SaveConfigToFile();
            });

            return config.ThemePackage;
        }

        public int StartSync()
        {
            if (string.IsNullOrEmpty(config.ThemeFolder))
            {
                EventBridge.Insatance.Prompt("请选择主题项目目录");
                return -1;
            }
            if (!Directory.Exists(config.ThemeFolder))
            {
                EventBridge.Insatance.Prompt("主题项目目录已被删除或移动");
                return -1;
            }

            if (string.IsNullOrEmpty(config.ThemePackage))
            {
                EventBridge.Insatance.Prompt("请选择打包生成的.hwt文件");
                return -1;
            }
            if (!File.Exists(config.ThemePackage))
            {
                EventBridge.Insatance.Prompt("打包的.hwt文件已被删除或移动");
                return -1;
            }

            if (config.PackFiles.Length == 0)
            {
                EventBridge.Insatance.Prompt("未选择要打包的文件");
                return -1;
            }


            if (listener != null)
            {
                listener.Packaged -= Packaged;
                listener.Dispose();
                listener = null;
            }

            switch (config.AutoRun)
            {
                case "inSecond":
                    listener = new TimerListener(
                        config.ThemeFolder,
                        config.ThemePackage,
                        config.InSecondSleep,
                        config.PackFiles
                    );
                    break;
                case "watchFile":
                    listener = new FileWatcherListener(
                        config.ThemeFolder,
                        config.ThemePackage,
                        config.WatchFileSleep,
                        config.PackFiles
                    );
                    break;
                default:
                    break;
            }

            if (listener != null)
            {
                listener.Packaged += Packaged;
                listener.Start();
                syncing = true;
                Logging.Info($"Listener activated. Listener={listener.GetType().Name}; Project={config.ThemeFolder}; Package={config.ThemePackage}; Files={string.Join(',', config.PackFiles)}");
                return 1;
            }
            return -1;
        }

        public int StopSync()
        {
            if (listener == null)
            { return -1; }

            listener.Stop();
            syncing = false;
            return 1;
        }

        public int IsSyncing()
        {
            return syncing ? 1 : 0;
        }

        public int ManualSync()
        {
            if (syncing)
            {
                EventBridge.Insatance.Prompt("已启动自动打包流程，操作取消");
                Logging.Warning("Automatic process detected.");
                return -1;
            }

            if (string.IsNullOrEmpty(config.ThemeFolder))
            {
                EventBridge.Insatance.Prompt("请选择主题项目目录");
                return -1;
            }
            if (!Directory.Exists(config.ThemeFolder))
            {
                EventBridge.Insatance.Prompt("主题项目目录已被删除或移动");
                return -1;
            }

            if (string.IsNullOrEmpty(config.ThemePackage))
            {
                EventBridge.Insatance.Prompt("请选择打包生成的.hwt文件");
                return -1;
            }
            if (!File.Exists(config.ThemePackage))
            {
                EventBridge.Insatance.Prompt("打包的.hwt文件已被删除或移动");
                return -1;
            }

            if (config.PackFiles.Length == 0)
            {
                EventBridge.Insatance.Prompt("未选择要打包的文件");
                return -1;
            }

            Logging.Info($"Packaging. Project={config.ThemeFolder}; Package={config.ThemePackage}; Files={string.Join(',', config.PackFiles)}");
            new ManualListener(
                config.ThemeFolder,
                config.ThemePackage,
                0,
                config.PackFiles
            ).Start();
            lastPack = DateTime.Now;
            return 1;
        }

        public string GetPackagedTime() => lastPack.ToString("yyyy-MM-dd HH:mm:ss");
        public long GetPackageSize() => File.Exists(config.ThemePackage) ? new FileInfo(config.ThemePackage).Length : -1;

        public List<Tuple<string, int>> GetPackLogFileList()
        {
            var files = Logging.GetLogFileList(_loggerName);
            List<Tuple<string, int>> fileList = new List<Tuple<string, int>>(files.Length);

            foreach (var item in files)
            {
                fileList.Add(new Tuple<string, int>(Path.GetFileNameWithoutExtension(item), File.ReadAllLines(item).Length));
            }

            return fileList;
        }

        public string[] GetPackLog(string date)
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

        private void Packaged() => lastPack = DateTime.Now;


        /// <summary>
        /// 打包监听程序通用行为
        /// </summary>
        private abstract class AbstractListener : IDisposable
        {
            protected bool disposedValue;

            public string Source { get; }
            public string Target { get; }
            public int Sleep { get; }
            public string[] FileList { get; }

            public event Action Packaged;

            public AbstractListener(string source, string target, int sleepSecond, string[] fileList)
            {
                Source = source;
                Target = target;
                Sleep = sleepSecond;
                FileList = fileList;
            }

            public abstract void Start();
            public abstract void Stop();

            /// <summary>
            /// 打包
            /// </summary>
            protected void Package()
            {
                if (!Directory.Exists(Source))
                {
                    EventBridge.Insatance.Prompt("主题项目已被删除或移动，请停止同步");
                    Logging.Warning($"Theme project directory has been moved or deleted. Path={Source}");
                    return;
                }
                if (!File.Exists(Target))
                {
                    EventBridge.Insatance.Prompt("打包后的.hwt已被删除或移动，请停止同步");
                    Logging.Warning($"Theme package file has been moved or deleted. Path={Target}");
                    return;
                }

                try
                {
                    Logging.Info($"Packaging. Project={Source}; Package={Target}; Files={string.Join(',', FileList)}");
                    using IZipDocument zip = new ZipDocument(Target);
                    foreach (var item in FileList)
                    {
                        switch (item)
                        {
                            case "icons":
                                Pack_icons(zip);
                                break;
                            case "unlock-dynamic":
                                Pack_unlockDynamic(zip);
                                break;
                            case "unlock-magazine":
                                Pack_unlockMagazine(zip);
                                break;
                            case "unlock-slide":
                                Pack_unlockSlide(zip);
                                break;
                            case "unlock-video":
                                Pack_unlockVideo(zip);
                                break;
                            case "unlock-vr":
                                Pack_unlockVr(zip);
                                break;
                            case "wallpaper":
                                Pack_wallpaper(zip);
                                break;
                            case "wallpaper-tablet":
                                Pack_wallpaperTablet(zip);
                                break;
                            case "wallpaper-foldable":
                                Pack_wallpaperFoldable(zip);
                                break;
                            case "contacts":
                                Pack_contacts(zip);
                                break;
                            case "incallui":
                                Pack_incallui(zip);
                                break;
                            case "mms":
                                Pack_mms(zip);
                                break;
                            case "phone":
                                Pack_phone(zip);
                                break;
                            case "telecom":
                                Pack_telecom(zip);
                                break;
                            case "systemui":
                                Pack_systemui(zip);
                                break;
                            case "launcher":
                                Pack_launcher(zip);
                                break;
                            case "recorder":
                                Pack_recorder(zip);
                                break;
                            case "famanager":
                                Pack_famanager(zip);
                                break;
                            case "widget":
                                Pack_widget(zip);
                                break;
                            default:
                                break;
                        }
                    }

                    var xmlCntBytes = zip.Get("description.xml");
                    if (xmlCntBytes != null && xmlCntBytes.Length > 0)
                    {
                        var xmlCnt = Encoding.UTF8.GetString(xmlCntBytes);
                        if (!xmlCnt.Contains(SystemConstant.XmlAD))
                        {
                            xmlCnt += "\n" + SystemConstant.XmlAD;
                            var xmlTempPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@description.xml");
                            File.WriteAllText(xmlTempPath, xmlCnt);
                            zip.Add(xmlTempPath, "description.xml");
                        }
                    }

                    zip.Save();
                    Logging.Info($"Packaging completed.");
                    Logging.Info(_loggerName, $"打包成功。,proj={Source},pack={Target},size={new FileInfo(Target).Length}");
                }
                catch (Exception ex)
                {
                    Logging.Error("Package cancelled.", ex);
                    Logging.Error(_loggerName, $"打包失败。（原因：{ex.Message}）,proj={Source},pack={Target}");
                }

                if (Packaged != null)
                {
                    Packaged.Invoke();
                }
            }

            #region 各组件打包逻辑
            /// <summary>
            /// 万象小组件
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_widget(IZipDocument zip)
            {
                string modName = "theme-widget";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName + "/");

                zip.Add(modDir, modName);
            }

            /// <summary>
            /// 图标替换
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_icons(IZipDocument zip)
            {
                string modName = "icons";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    tempZip.Add(modDir);
                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// 锁屏样式 - 动态锁屏
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_unlockDynamic(IZipDocument zip)
            {
                string modName = "unlock-dynamic";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete("unlock/");

                zip.Add(modDir, "unlock");
            }

            /// <summary>
            /// 锁屏样式 - 杂志锁屏
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_unlockMagazine(IZipDocument zip)
            {
                string modName = "unlock-magazine";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete("unlock/");

                zip.Add(modDir, "unlock");
            }

            /// <summary>
            /// 锁屏样式 - 滑动锁屏
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_unlockSlide(IZipDocument zip)
            {
                string modName = "unlock-slide";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete("unlock/");

                zip.Add(modDir, "unlock");
            }

            /// <summary>
            /// 锁屏样式 - 视频锁屏
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_unlockVideo(IZipDocument zip)
            {
                string modName = "unlock-video";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete("unlock/");

                zip.Add(modDir, "unlock");
            }

            /// <summary>
            /// 锁屏样式 - VR锁屏
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_unlockVr(IZipDocument zip)
            {
                string modName = "unlock-vr";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete("unlock/");

                zip.Add(modDir, "unlock");
            }

            /// <summary>
            /// 桌面样式 - 手机
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_wallpaper(IZipDocument zip)
            {
                string modName = "wallpaper";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                zip.Add(modDir, modName);
            }

            /// <summary>
            /// 桌面样式 - 平板
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_wallpaperTablet(IZipDocument zip)
            {
                string modName = "wallpaper-tablet";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                zip.Add(modDir, modName);
            }

            /// <summary>
            /// 桌面样式 - 折叠屏
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_wallpaperFoldable(IZipDocument zip)
            {
                string modName = "wallpaper-foldable";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                zip.Add(modDir, modName);
            }

            /// <summary>
            /// 联系人模块样式
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_contacts(IZipDocument zip)
            {
                string modName = "com.android.contacts";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (var tempZip = new ZipDocument2(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(Source, comres);

                    tempZip.Add(modDir);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }

                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// 通话中界面样式
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_incallui(IZipDocument zip)
            {
                string modName = "com.android.incallui";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    tempZip.Add(modDir);
                    tempZip.Delete(comres + "/");
                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// 短信模块样式
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_mms(IZipDocument zip)
            {
                string modName = "com.android.mms";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument2(tempZipPath))
                {
                    tempZip.Add(modDir);
                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// com.android.phone
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_phone(IZipDocument zip)
            {
                string modName = "com.android.phone";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(Source, comres);

                    tempZip.Add(modDir);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }

                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// com.android.server.telecom
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_telecom(IZipDocument zip)
            {
                string modName = "com.android.server.telecom";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(Source, comres);

                    tempZip.Add(modDir);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }

                    string appendPath = "res/drawable-xxhdpi";
                    string cres = Path.Combine(comresDir, appendPath);
                    string pres = Path.Combine(modDir, comres, appendPath);
                    if (Directory.Exists(pres))
                    {
                        foreach (var item in Directory.GetFiles(pres))
                        {
                            string fileName = Path.GetFileName(item);

                            var cfile = new FileInfo(Path.Combine(cres, fileName));
                            if (!cfile.Exists)
                            { continue; }
                            var pfile = new FileInfo(item);
                            if (pfile.Length == cfile.Length)
                            { continue; }

                            string zipFilePath = $"{comres}/{appendPath}/{fileName}";

                            tempZip.Delete(zipFilePath);
                            if (pfile.Length > cfile.Length)
                            { tempZip.Add(item, zipFilePath); }
                            else
                            { tempZip.Add(cfile.FullName, zipFilePath); }
                        }
                    }

                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// 通知模块样式
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_systemui(IZipDocument zip)
            {
                string modName = "com.android.systemui";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(Source, comres);

                    tempZip.Add(modDir);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }

                    string appendPath = "res/drawable-xxhdpi";
                    string cres = Path.Combine(comresDir, appendPath);
                    string pres = Path.Combine(modDir, comres, appendPath);
                    if (Directory.Exists(pres))
                    {
                        foreach (var item in Directory.GetFiles(pres))
                        {
                            string fileName = Path.GetFileName(item);

                            var cfile = new FileInfo(Path.Combine(cres, fileName));
                            if (!cfile.Exists)
                            { continue; }
                            var pfile = new FileInfo(item);
                            if (pfile.Length == cfile.Length)
                            { continue; }

                            string zipFilePath = $"{comres}/{appendPath}/{fileName}";

                            tempZip.Delete(zipFilePath);
                            if (pfile.Length > cfile.Length)
                            { tempZip.Add(item, zipFilePath); }
                            else
                            { tempZip.Add(cfile.FullName, zipFilePath); }
                        }
                    }

                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// 桌面模块样式
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_launcher(IZipDocument zip)
            {
                string modName = "com.huawei.android.launcher";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(Source, comres);

                    tempZip.Add(modDir);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }

                    string appendPath = "res/drawable-xxhdpi";
                    string cres = Path.Combine(comresDir, appendPath);
                    string pres = Path.Combine(modDir, comres, appendPath);
                    if (Directory.Exists(pres))
                    {
                        foreach (var item in Directory.GetFiles(pres))
                        {
                            string fileName = Path.GetFileName(item);

                            var cfile = new FileInfo(Path.Combine(cres, fileName));
                            if (!cfile.Exists)
                            { continue; }
                            var pfile = new FileInfo(item);
                            if (pfile.Length == cfile.Length)
                            { continue; }

                            string zipFilePath = $"{comres}/{appendPath}/{fileName}";

                            tempZip.Delete(zipFilePath);
                            if (pfile.Length > cfile.Length)
                            { tempZip.Add(item, zipFilePath); }
                            else
                            { tempZip.Add(cfile.FullName, zipFilePath); }
                        }
                    }

                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// com.huawei.phone.recorder
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_recorder(IZipDocument zip)
            {
                string modName = "com.huawei.phone.recorder";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    tempZip.Add(modDir);
                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }

            /// <summary>
            /// com.huawei.ohos.famanager
            /// </summary>
            /// <param name="zip"></param>
            private void Pack_famanager(IZipDocument zip)
            {
                string modName = "com.huawei.ohos.famanager";
                string modDir = Path.Combine(Source, modName);
                if (!Directory.Exists(modDir))
                { return; }
                zip.Delete(modName);

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{modName}");
                using (IZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    tempZip.Add(modDir);
                    tempZip.Save();
                }

                zip.Add(tempZipPath, modName);
            }
            #endregion


            /*
            protected virtual void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        // TODO: 释放托管状态(托管对象)
                    }

                    // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                    // TODO: 将大型字段设置为 null
                    disposedValue = true;
                }
            }
            */

            protected abstract void Dispose(bool disposing);

            // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
            // ~AbstractListener()
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

        /// <summary>
        /// 基于定时器实现的打包工具
        /// </summary>
        private class TimerListener : AbstractListener
        {
            private Timer timer;
            private bool executing = false;

            public TimerListener(string source, string target, int sleepSecond, string[] fileList) : base(source, target, sleepSecond, fileList)
            {
                timer = new Timer(Math.Max(sleepSecond, 1) * 1000);
                timer.Enabled = true;
                timer.Elapsed += Timer_Elapsed;
            }

            private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
            {
                if (executing)
                { return;  }

                try
                {
                    executing = true;
                    Package();
                    executing = false;
                }
                catch (Exception ex)
                {
                    executing = false;
                    Logging.Error("AutoPack task failed.", ex);
                }
            }

            public override void Start()
            {
                timer.Start();
            }

            public override void Stop()
            {
                timer.Stop();
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        timer.Dispose();
                    }

                    disposedValue = true;
                }
            }
        }

        /// <summary>
        /// 基于监听文件变化实现的打包工具
        /// </summary>
        private class FileWatcherListener : AbstractListener
        {
            private FileSystemWatcher watcher;
            private bool executing = false;
            private Timer timer;

            public FileWatcherListener(string source, string target, int sleepSecond, string[] fileList) : base(source, target, sleepSecond, fileList)
            {
                timer = new Timer(Math.Max(sleepSecond, 1) * 1000);
                timer.Enabled = true;
                timer.Elapsed += Timer_Elapsed;
            }

            private void Timer_Elapsed(object? sender, ElapsedEventArgs e)
            {
                executing = false;
                timer.Stop();
            }

            public override void Start()
            {
                if (watcher != null)
                { watcher.Dispose(); }

                watcher = new FileSystemWatcher(Source);
                watcher.InternalBufferSize = 65535;
                watcher.IncludeSubdirectories = true;

                watcher.Created += Watcher_Created;
                watcher.Changed += Watcher_Changed;
                watcher.Renamed += Watcher_Renamed;
                watcher.Deleted += Watcher_Deleted;
                watcher.Error += Watcher_Error;

                watcher.EnableRaisingEvents = true;
            }

            private void Watcher_Error(object sender, ErrorEventArgs e)
            {
            }

            private void Watcher_Deleted(object sender, FileSystemEventArgs e)
            {
                StartPack();
            }

            private void Watcher_Renamed(object sender, RenamedEventArgs e)
            {
                StartPack();
            }

            private void Watcher_Changed(object sender, FileSystemEventArgs e)
            {
                StartPack();
            }

            private void Watcher_Created(object sender, FileSystemEventArgs e)
            {
                StartPack();
            }

            private void StartPack()
            {
                if (executing)
                { return; }

                executing = true;
                Task.Run(() =>
                {
                    System.Threading.Thread.Sleep(1000);
                    Package();
                    timer.Start();
                });
            }

            public override void Stop()
            {
                watcher.EnableRaisingEvents = false;
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposedValue)
                {
                    if (disposing)
                    {
                        if (watcher != null)
                        { watcher.Dispose(); }
                        timer.Dispose();
                    }

                    disposedValue = true;
                }
            }
        }

        /// <summary>
        /// 手动打包
        /// </summary>
        private class ManualListener : AbstractListener
        {
            public ManualListener(string source, string target, int sleepSecond, string[] fileList) : base(source, target, sleepSecond, fileList)
            {
            }

            public override void Start()
            {
                Package();
            }

            public override void Stop()
            {
                throw new NotImplementedException();
            }

            protected override void Dispose(bool disposing)
            {
                disposedValue = true;
            }
        }
    }


    public class ThemeAutoPackConfig : ConfigBase
    {
        public override string ConfigName => "ThemeAutoPack";

        public string ThemeFolder { get; set; }
        public string ThemePackage { get; set; }
        public string AutoRun { get; set; } = "inSecond";
        public int InSecondSleep { get; set; } = 5;
        public int WatchFileSleep { get; set; } = 5;
        public string[] PackFiles { get; set; } = { "icons", "unlock-dynamic", "wallpaper", "widget" };
    }
}
