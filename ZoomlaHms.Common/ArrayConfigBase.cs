using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    /// <summary>
    /// 适用于数组配置的功能配置基类
    /// </summary>
    /// <typeparam name="TConfig">配置类型</typeparam>
    public abstract class ArrayConfigBase<TConfig> : ConfigBase, IEnumerable<TConfig>
    {
        /// <summary>
        /// 配置列表
        /// </summary>
        private List<TConfig> configs = new List<TConfig>();

        /// <summary>
        /// 获取或修改列表中的配置
        /// </summary>
        /// <param name="index">配置在列表中的索引</param>
        /// <returns>配置实例</returns>
        public TConfig this[int index]
        {
            get => configs[index];
            set => configs[index] = value;
        }

        /// <summary>
        /// 添加一个配置
        /// </summary>
        /// <param name="config"></param>
        public void Add(TConfig config) => configs.Add(config);
        /// <summary>
        /// 从索引处移除一个配置
        /// </summary>
        /// <param name="index">配置在列表中的索引</param>
        public void RemoveAt(int index) => configs.RemoveAt(index);

        /// <summary>
        /// 从Json中加载配置列表
        /// </summary>
        /// <param name="json"></param>
        public override void LoadConfigFromJson(string json)
        {
            try
            {
                configs = JsonSerializer.Deserialize<List<TConfig>>(json, _jsonSerializerOptions);
            }
            catch (JsonException)
            {
                //do something...
            }
        }

        /// <summary>
        /// 将配置列表保存到Json中
        /// </summary>
        /// <returns></returns>
        public override string SaveConfigToJson()
        {
            return JsonSerializer.Serialize(configs);
        }

        public IEnumerator<TConfig> GetEnumerator()
        {
            return configs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return configs.GetEnumerator();
        }
    }
}
