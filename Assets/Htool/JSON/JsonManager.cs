using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using LitJson;
using Newtonsoft.Json;
using System.IO;

namespace Htool
{
    /// <summary>
    /// Json使用的解析器工具
    /// </summary>
    public enum JsonParserTool
    {
        JsonUtility,
        LitJson,
        Newtonsoft_Json
    }

    /// <summary>
    /// Json管理器
    /// </summary>
    public class JsonManager : Singleton<JsonManager>
    {
        /// <summary>
        /// 序列化Json
        /// </summary>
        /// <param name="value">要序列化的数据</param>
        /// <param name="JsonType">使用哪个Json解析工具</param>
        /// <returns>Json文本</returns>
        public string SerializeJson(object value, JsonParserTool JsonType)
        {
            string outJsonText = null;
            switch (JsonType)
            {
                case JsonParserTool.JsonUtility:
                    outJsonText = JsonUtility.ToJson(value);
                    break;
                case JsonParserTool.LitJson:
                    outJsonText = JsonMapper.ToJson(value);
                    break;
                case JsonParserTool.Newtonsoft_Json:
                    outJsonText = JsonConvert.SerializeObject(value);
                    break;
                default:
                    break;
            }
            return outJsonText;
        }

        /// <summary>
        /// 保存数据到Json文件
        /// </summary>
        /// <param name="value">要保存的数据</param>
        /// <param name="outputPath">输出路径，末尾不需要带后缀名</param>
        /// <param name="JsonType">使用哪个Json解析工具</param>
        public void SaveDataToJsonFile(object value, string outputPath, JsonParserTool JsonType)
        {
            string Path = outputPath + ".json";
            string outJsonText = SerializeJson(value, JsonType);
            File.WriteAllText(Path, outJsonText);
        }

        /// <summary>
        /// 反序列化Json
        /// </summary>
        /// <typeparam name="T">Json对应的数据类型</typeparam>
        /// <param name="value">Json文本</param>
        /// <param name="JsonType">使用哪个Json解析工具</param>
        /// <returns>T类型Json数据</returns>
        public T DeserializeJson<T>(string value, JsonParserTool JsonType)
        {
            T data = default(T);
            switch (JsonType)
            {
                case JsonParserTool.JsonUtility:
                    data = JsonUtility.FromJson<T>(value);
                    break;
                case JsonParserTool.LitJson:
                    data = JsonMapper.ToObject<T>(value);
                    break;
                case JsonParserTool.Newtonsoft_Json:
                    data = JsonConvert.DeserializeObject<T>(value);
                    break;
                default:
                    break;
            }
            return data;
        }

        /// <summary>
        /// 读取Json文件到数据
        /// </summary>
        /// <typeparam name="T">Json对应的数据类型</typeparam>
        /// <param name="filePath">Json文件的路径</param>
        /// <param name="JsonType">使用哪个Json解析工具</param>
        /// <returns>T类型Json数据</returns>
        public T ReadJsonFileToData<T>(string filePath, JsonParserTool JsonType)
        {
            T data = default(T);
            if (!File.Exists(filePath))
                return data;
            string JsonText = File.ReadAllText(filePath);
            data = DeserializeJson<T>(JsonText, JsonType);
            return data;
        }
    }
}