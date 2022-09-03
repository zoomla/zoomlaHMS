using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Xml;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent.Implements
{
    public class ThemePackJsEvent : ClassicJsEvent<ThemePackConfig>
    {
        public string ChooseThemeFolder()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
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

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    config.ThemeFolder = dialog.FileName;
                    config.SaveConfigToFile();
                }
            });

            return config.ThemeFolder;
        }

        public string ChooseThemeExportFolder()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                CommonOpenFileDialog dialog = new CommonOpenFileDialog();
                dialog.IsFolderPicker = true;
                if (string.IsNullOrEmpty(config.ThemeExportFolder) || !Directory.Exists(config.ThemeExportFolder))
                { dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments); }
                else
                { dialog.InitialDirectory = config.ThemeExportFolder; }

                if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    config.ThemeExportFolder = dialog.FileName;
                    config.SaveConfigToFile();
                }
            });

            return config.ThemeExportFolder;
        }

        public void PackTheme()
        {
            if (string.IsNullOrEmpty(config.ThemeFolder))
            {
                EventBridge.Insatance.Prompt("请选择主题项目文件夹");
                return;
            }
            if (!Directory.Exists(config.ThemeFolder))
            {
                EventBridge.Insatance.Prompt("主题项目已被删除或移动");
                return;
            }

            if (string.IsNullOrEmpty(config.ThemeExportFolder))
            {
                EventBridge.Insatance.Prompt("请选择打包输出文件夹");
                return;
            }
            if (!Directory.Exists(config.ThemeExportFolder))
            {
                EventBridge.Insatance.Prompt("打包输出文件夹已被删除或移动");
                return;
            }

            foreach (var item in config.ApplyDevices)
            {
                switch (item)
                {
                    case "phone":
                        switch (config.ThemeType)
                        {
                            case "largeScope":
                                ThemePackFactory.Phone_largeScope(config);
                                break;
                            case "smallScope":
                                ThemePackFactory.Phone_smallScope(config);
                                break;
                            case "icon":
                                ThemePackFactory.Phone_icon(config);
                                break;
                            case "lockScreen":
                                ThemePackFactory.Phone_lockScreen(config);
                                break;
                            case "aod":
                            default:
                                break;
                        }
                        break;
                    case "tablet":
                        //switch (config.ThemeType)
                        //{
                        //    case "largeScope":
                        //        ThemePackFactory.Tablet_largeScope(config);
                        //        break;
                        //    case "smallScope":
                        //        ThemePackFactory.Tablet_smallScope(config);
                        //        break;
                        //    case "icon":
                        //        ThemePackFactory.Tablet_icon(config);
                        //        break;
                        //    case "lockScreen":
                        //        ThemePackFactory.Tablet_lockScreen(config);
                        //        break;
                        //    case "aod":
                        //    default:
                        //        break;
                        //}
                        break;
                    case "foldable":
                        //switch (config.ThemeType)
                        //{
                        //    case "largeScope":
                        //        ThemePackFactory.Foldable_largeScope(config);
                        //        break;
                        //    case "smallScope":
                        //        ThemePackFactory.Foldable_smallScope(config);
                        //        break;
                        //    case "icon":
                        //        ThemePackFactory.Foldable_icon(config);
                        //        break;
                        //    case "lockScreen":
                        //        ThemePackFactory.Foldable_lockScreen(config);
                        //        break;
                        //    case "aod":
                        //    default:
                        //        break;
                        //}
                        break;
                    default:
                        break;
                }
            }
            //var zip = new ZipDocument(Path.Combine(config.ThemeExportFolder, "test.zip"));
            //zip.Add(config.ThemeFolder);
            //zip.SaveToFile();
        }


        private static class ThemePackFactory
        {
            private static void Pack_describePhone(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "description.xml";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!File.Exists(moduleName))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_describePhone), $"'{moduleName}' is missing.");
                    return;
                }

                try
                {
                    string tempFile = Path.Combine(SystemPath.TempFileDirectory, moduleName);
                    XmlDocument xml = new XmlDocument();
                    xml.Load(modulePath);

                    xml.SelectSingleNode("/HwTheme/screen").InnerText = "FHD";
                    xml.SelectSingleNode("/HwTheme/version").InnerText = pakcfg.ApplySystem;

                    xml.Save(tempFile);
                    zipfile.Add(tempFile, moduleName);
                }
                catch (Exception ex)
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_describePhone), $"Cannot change '{moduleName}' file content.", ex);
                    zipfile.Add(modulePath, moduleName);
                }
            }

            private static void Pack_describeTablet(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "description.xml";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!File.Exists(moduleName))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_describeTablet), $"'{moduleName}' is missing.");
                    return;
                }

                try
                {
                    string tempFile = Path.Combine(SystemPath.TempFileDirectory, moduleName);
                    XmlDocument xml = new XmlDocument();
                    xml.Load(modulePath);

                    xml.SelectSingleNode("/HwTheme/screen").InnerText = "WUXGA";
                    xml.SelectSingleNode("/HwTheme/version").InnerText = pakcfg.ApplySystem;

                    xml.Save(tempFile);
                    zipfile.Add(tempFile, moduleName);
                }
                catch (Exception ex)
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_describeTablet), $"Cannot change '{moduleName}' file content.", ex);
                    zipfile.Add(modulePath, moduleName);
                }
            }

            private static void Pack_describeFoldable(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "description.xml";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!File.Exists(moduleName))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_describeFoldable), $"'{moduleName}' is missing.");
                    return;
                }

                try
                {
                    string tempFile = Path.Combine(SystemPath.TempFileDirectory, moduleName);
                    XmlDocument xml = new XmlDocument();
                    xml.Load(modulePath);

                    xml.SelectSingleNode("/HwTheme/screen").InnerText = "WQHD";
                    xml.SelectSingleNode("/HwTheme/version").InnerText = pakcfg.ApplySystem;

                    xml.Save(tempFile);
                    zipfile.Add(tempFile, moduleName);
                }
                catch (Exception ex)
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_describeFoldable), $"Cannot change '{moduleName}' file content.", ex);
                    zipfile.Add(modulePath, moduleName);
                }
            }

            private static void Pack_unlockDynamic(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "unlock-dynamic";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_unlockDynamic), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "unlock");
            }

            private static void Pack_unlockMagazine(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "unlock-magazine";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_unlockMagazine), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "unlock");
            }

            private static void Pack_unlockSlide(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "unlock-slide";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_unlockSlide), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "unlock");
            }

            private static void Pack_unlockVideo(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "unlock-video";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_unlockVideo), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "unlock");
            }

            private static void Pack_unlockVr(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "unlock-vr";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_unlockVr), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "unlock");
            }

            private static void Pack_wallpaperPhone(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "wallpaper";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_wallpaperPhone), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "wallpaper");
            }

            private static void Pack_wallpaperTablet(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "wallpaper-tablet";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_wallpaperTablet), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "wallpaper");
            }

            private static void Pack_wallpaperFoldable(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "wallpaper-foldable";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_wallpaperFoldable), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "wallpaper");
            }

            private static void Pack_contacts(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.android.contacts";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_contacts), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            private static void Pack_incallui(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.android.incallui";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_incallui), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            private static void Pack_mms(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.android.mms";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_mms), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            private static void Pack_phone(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.android.phone";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_phone), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_telecom(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.android.server.telecom";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_telecom), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_systemui(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.android.systemui";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_systemui), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_launcher(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.huawei.android.launcher";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_launcher), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    string comres = "framework-res-hwext";
                    string comresDir = Path.Combine(pakcfg.ThemeFolder, comres);

                    tempZip.Add(modulePath);
                    if (Directory.Exists(comresDir))
                    {
                        tempZip.Delete(comres + "/");
                        tempZip.Add(comresDir, comres);
                    }
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_recorder(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.huawei.phone.recorder";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_recorder), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    tempZip.Add(modulePath);
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_famanager(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "com.huawei.ohos.famanager";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_famanager), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    tempZip.Add(modulePath);
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_icons(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "icons";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_icons), $"'{moduleName}' is missing.");
                    return;
                }

                string tempZipPath = Path.Combine(SystemPath.TempFileDirectory, $"{Guid.NewGuid():N}@{moduleName}");
                using (ZipDocument tempZip = new ZipDocument(tempZipPath))
                {
                    tempZip.Add(modulePath);
                    tempZip.SaveChangesToFile();
                }

                zipfile.Add(tempZipPath, moduleName);
            }

            public static void Pack_widget(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "theme-widget";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_widget), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, moduleName);
            }

            public static void Pack_previewPhone(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "preview";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_previewPhone), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "preview");
            }

            public static void Pack_previewTablet(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "preview-tablet";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_previewTablet), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "preview");
            }

            public static void Pack_previewFoldable(ThemePackConfig pakcfg, ZipDocument zipfile)
            {
                string moduleName = "preview-foldable";
                string modulePath = Path.Combine(pakcfg.ThemeFolder, moduleName);
                if (!Directory.Exists(modulePath))
                {
                    Logging.Warning(typeof(ThemePackFactory), nameof(Pack_previewFoldable), $"'{moduleName}' is missing.");
                    return;
                }

                zipfile.Add(modulePath, "preview");
            }






            public static void Phone_icon(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_Icon_Phone.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewPhone(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describePhone(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        break;
                    case "12.0":
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Phone_largeScope(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_largeScope_Phone.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewPhone(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperPhone(pakcfg, zip);
                Pack_contacts(pakcfg, zip);
                Pack_mms(pakcfg, zip);
                Pack_phone(pakcfg, zip);
                Pack_recorder(pakcfg, zip);
                Pack_telecom(pakcfg, zip);
                Pack_systemui(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describePhone(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        //com.huawei.hwvoipservice
                        break;
                    case "11.0":
                        Pack_incallui(pakcfg, zip);
                        //com.huawei.aod
                        break;
                    case "12.0":
                        Pack_incallui(pakcfg, zip);
                        //com.huawei.aod
                        //com.huawei.mediacontroller
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Phone_lockScreen(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_lockScreen_Phone.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewPhone(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperPhone(pakcfg, zip);
                Pack_describePhone(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        break;
                    case "12.0":
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Phone_smallScope(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_smallScope_Phone.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewPhone(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperPhone(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describePhone(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        //com.huawei.aod
                        break;
                    case "12.0":
                        //com.huawei.aod
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }



            public static void Foldable_icon(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_Icon_Foldable.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewFoldable(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describeFoldable(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        break;
                    case "12.0":
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Foldable_largeScope(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_largeScope_Foldable.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewFoldable(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperFoldable(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describeFoldable(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        //com.huawei.aod
                        break;
                    case "12.0":
                        //com.huawei.aod
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Foldable_lockScreen(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_lockScreen_Foldable.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewFoldable(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperFoldable(pakcfg, zip);
                Pack_describeFoldable(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        break;
                    case "12.0":
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Foldable_smallScope(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_smallScope_Foldable.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewFoldable(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperFoldable(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describeFoldable(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        //com.huawei.aod
                        break;
                    case "12.0":
                        //com.huawei.aod
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }



            public static void Tablet_icon(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_Icon_Tablet.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewTablet(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describeTablet(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        break;
                    case "12.0":
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Tablet_largeScope(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_largeScope_Tablet.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewTablet(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperTablet(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describeTablet(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        //com.huawei.aod
                        break;
                    case "12.0":
                        //com.huawei.aod
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Tablet_lockScreen(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_lockScreen_Tablet.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewTablet(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperTablet(pakcfg, zip);
                Pack_describeTablet(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        break;
                    case "12.0":
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }

            public static void Tablet_smallScope(ThemePackConfig pakcfg)
            {
                string zipPath = Path.Combine(pakcfg.ThemeExportFolder, $"{GetThemeName(pakcfg)}_{pakcfg.ApplySystem}_smallScope_Tablet.hwt");
                using ZipDocument zip = new ZipDocument(zipPath);

                Pack_previewTablet(pakcfg, zip);
                switch (pakcfg.ThemeLockscreen)
                {
                    case "dynamic":
                        Pack_unlockDynamic(pakcfg, zip);
                        break;
                    case "magazine":
                        Pack_unlockMagazine(pakcfg, zip);
                        break;
                    case "slide":
                        Pack_unlockSlide(pakcfg, zip);
                        break;
                    case "video":
                        Pack_unlockVideo(pakcfg, zip);
                        break;
                    case "vr":
                        Pack_unlockVr(pakcfg, zip);
                        break;
                    default:
                        break;
                }
                Pack_wallpaperTablet(pakcfg, zip);
                Pack_launcher(pakcfg, zip);
                Pack_describeTablet(pakcfg, zip);
                Pack_icons(pakcfg, zip);
                switch (pakcfg.ApplySystem)
                {
                    case "10.0.100":
                        break;
                    case "11.0":
                        //com.huawei.aod
                        break;
                    case "12.0":
                        //com.huawei.aod
                        Pack_famanager(pakcfg, zip);
                        //com.huawei.ohos.search
                        break;
                    default:
                        break;
                }

                zip.SaveChangesToFile();
            }



            private static string GetThemeName(ThemePackConfig pakcfg)
            {
                string dir = pakcfg.ThemeFolder.Replace('\\', Path.DirectorySeparatorChar).Replace('/', Path.DirectorySeparatorChar);
                string dirName = dir.LastIndexOf(Path.DirectorySeparatorChar) > -1 ? dir.Substring(dir.LastIndexOf(Path.DirectorySeparatorChar) + 1) : Path.GetRandomFileName();

                string xmlPath = Path.Combine(pakcfg.ThemeFolder, "description.xml");
                if (!File.Exists(xmlPath))
                { return dirName; }

                try
                {
                    XmlDocument xml = new XmlDocument();
                    xml.Load(xmlPath);
                    return xml.SelectSingleNode("/HwTheme/title").InnerText;
                }
                catch (Exception)
                {
                    return dirName;
                }
            }
        }
    }


    public class ThemePackConfig : ConfigBase
    {
        public override string ConfigName => "ThemePack";

        public string ThemeFolder { get; set; }
        public string ThemeExportFolder { get; set; }
        public string[] ApplyDevices { get; set; } = { "phone" };
        public string ApplySystem { get; set; } = "11.0";
        public string ThemeType { get; set; } = "largeScope";
        public string ThemeLockscreen { get; set; } = "slide";
    }
}
