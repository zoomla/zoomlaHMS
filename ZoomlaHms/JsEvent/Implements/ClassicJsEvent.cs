using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent.Implements
{
    /// <summary>
    /// 响应JavaScript请求处理程序的一般实现
    /// </summary>
    /// <typeparam name="TConfig">配置信息</typeparam>
    public abstract class ClassicJsEvent<TConfig> : IJsEvent where TConfig : ConfigBase, new()
    {
        protected TConfig config;

        /// <summary>
        /// 初始化标准JavaScript请求处理程序的新实例
        /// </summary>
        public ClassicJsEvent()
        {
            config = new TConfig();
            config.LoadConfigFromFile();
        }


        /// <summary>
        /// 获取当前实例的配置，Json格式
        /// </summary>
        /// <returns>Json格式的配置信息</returns>
        public virtual string GetConfig()
        {
            return config.SaveConfigToJson();
        }

        /// <summary>
        /// 使用Json设置当前实例的配置
        /// </summary>
        /// <param name="json">Json格式的配置信息</param>
        /// <returns>修改后的配置信息，Json格式</returns>
        public virtual string SetConfig(string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                config.LoadConfigFromJson(json);
                config.SaveConfigToFile();
            }

            return GetConfig();
        }
    }
}
