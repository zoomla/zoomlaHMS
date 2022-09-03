using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;

namespace ZoomlaHms.Common
{
    /// <summary>
    /// 功能配置基类
    /// </summary>
    public abstract class ConfigBase
    {
        /// <summary>
        /// Json格式化配置
        /// </summary>
        protected static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All), //不转义汉字
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase, //首字母小写
            WriteIndented = true, //格式化Json
            IgnoreReadOnlyFields = true, //忽略只读字段
            IgnoreReadOnlyProperties = true, //忽略只读属性
        };
        /// <summary>
        /// 存放配置文件的文件夹
        /// </summary>
        private static readonly string confgBasePath;

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private string ConfigPath => Path.Combine(confgBasePath, ConfigName + ".json");
        /// <summary>
        /// 配置名称
        /// </summary>
        public abstract string ConfigName { get; }

        static ConfigBase()
        {
            confgBasePath = SystemPath.ConfigFileDirectory;
            if (!Directory.Exists(confgBasePath))
            { Directory.CreateDirectory(confgBasePath); }
        }

        /// <summary>
        /// 序列化配置为Json字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return SaveConfigToJson();
        }

        /// <summary>
        /// 从配置文件中加载配置
        /// </summary>
        public void LoadConfigFromFile()
        {
            if (!File.Exists(ConfigPath))
            { return; }

            LoadConfigFromJson(File.ReadAllText(ConfigPath));
        }

        /// <summary>
        /// 从Json字符串中加载配置
        /// </summary>
        /// <param name="json"></param>
        public virtual void LoadConfigFromJson(string json)
        {
            try
            {
                object config = JsonSerializer.Deserialize(json, GetType(), _jsonSerializerOptions);
                var props = GetType().GetProperties();
                foreach (var prop in props)
                {
                    if (!prop.CanWrite)
                    { continue; }
                    prop.SetValue(this, prop.GetValue(config));
                }
            }
            catch (JsonException)
            {
                //do something...
            }
        }

        /// <summary>
        /// 保存配置到配置文件
        /// </summary>
        public void SaveConfigToFile()
        {
            try
            {
                if (!Directory.Exists(confgBasePath))
                { Directory.CreateDirectory(confgBasePath); }

                File.WriteAllText(ConfigPath, SaveConfigToJson());
            }
            catch (Exception)
            {
                //do something...
            }
        }

        /// <summary>
        /// 保存配置到Json字符串
        /// </summary>
        /// <returns></returns>
        public virtual string SaveConfigToJson()
        {
            return JsonSerializer.Serialize(this, GetType(), _jsonSerializerOptions);
        }
    }
}
