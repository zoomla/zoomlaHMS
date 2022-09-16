using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    /// <summary>
    /// 常用路径
    /// </summary>
    public static class SystemPath
    {
        /// <summary>
        /// 基准路径（程序目录）
        /// </summary>
        public static string PathBase
        {
            get
            {
#if DEBUG
                string configDicParent = Path.GetDirectoryName(Environment.ProcessPath);
                configDicParent = configDicParent.Substring(0, configDicParent.IndexOf("ZoomlaHms\\") + 9);
                return configDicParent;
#else
                return Path.GetDirectoryName(Environment.ProcessPath);
#endif
            }
        }

        /// <summary>
        /// exe启动目录
        /// </summary>
        public static string StartupLocation => Path.GetDirectoryName(Environment.ProcessPath);
        /// <summary>
        /// 网页根目录 - CefSharp用
        /// </summary>
        public static string WebRoot => Path.Combine(PathBase, "wwwroot");

        private static string configFileDirectory = null;
        /// <summary>
        /// 配置文件目录
        /// </summary>
        public static string ConfigFileDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(configFileDirectory))
                {
                    string path = Path.Combine(PathBase, "Config");
                    if (!Directory.Exists(path))
                    {
                        Logging.Info("Missing config file directory, creating...");
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error("Unable to create config file directory.", ex);
                            throw;
                        }
                    }

                    configFileDirectory = path;
                }

                return configFileDirectory;
            }
        }

        private static string tempFileDirectory;
        /// <summary>
        /// 临时文件目录
        /// </summary>
        public static string TempFileDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(tempFileDirectory))
                {
                    string path = Path.Combine(PathBase, "Temp");
                    if (!Directory.Exists(path))
                    {
                        Logging.Info("Missing temporary file directory, creating...");
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error("Unable to create temporary file directory.", ex);
                            throw;
                        }
                    }

                    tempFileDirectory = path;
                }

                return tempFileDirectory;
            }
        }

        private static string externalToolsDirectory = null;
        /// <summary>
        /// 扩展工具目录
        /// </summary>
        public static string ExternalToolsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(externalToolsDirectory))
                {
                    string path = Path.Combine(PathBase, "Tools");
                    if (!Directory.Exists(path))
                    {
                        Logging.Info("Missing external tools directory, creating...");
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error("Unable to create external tools directory.", ex);
                            throw;
                        }
                    }

                    externalToolsDirectory = path;
                }

                return externalToolsDirectory;
            }
        }

        private static string appLogsDirectory = null;
        /// <summary>
        /// 日志文件目录
        /// </summary>
        public static string AppLogsDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(appLogsDirectory))
                {
                    string path = Path.Combine(PathBase, "Logs");
                    if (!Directory.Exists(path))
                    {
                        Logging.Info("Missing log file directory, creating...");
                        try
                        {
                            Directory.CreateDirectory(path);
                        }
                        catch (Exception ex)
                        {
                            Logging.Error("Unable to create log file directory.", ex);
                            throw;
                        }
                    }

                    appLogsDirectory = path;
                }

                return appLogsDirectory;
            }
        }
    }
}
