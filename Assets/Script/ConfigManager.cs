using Assets.Script.Config;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Assets.Script
{
    /// <summary>
    /// 配置管理器
    /// </summary>
    class ConfigManager
    {
        //单例
        static ConfigManager configManagerInstance = new ConfigManager();

        static Dictionary<string, ConfigBase> allConfigMap = new Dictionary<string, ConfigBase>();

        private ConfigManager()
        {
        }

        //public static ConfigManager GetSingleInstance()
        //{
        //    return configManagerInstance;
        //}

        /// <summary>
        /// 获得配置根路径
        /// </summary>
        /// <returns></returns>
        public static string GetConfigRootPath()
        {
            return Application.dataPath + "/Artres/Config/";
        }

        /// <summary>
        /// 加载所有配置文件
        /// </summary>
        void LoadAllConfig()
        {
            Debug.LogError("加载所有配置文件的功能暂时不开启！");
        }

        /// <summary>
        /// 通过名称获得配置
        /// </summary>
        /// <param name="configName"></param>
        /// <returns></returns>
        public static ConfigBase GetConfigByName(string configName)
        {
            if (allConfigMap.ContainsKey(configName))
            {
                return allConfigMap[configName];
            }
            else
            {
                LoadConfigByName(configName);
                if(allConfigMap.ContainsKey(configName))
                {
                    return allConfigMap[configName];
                }
                else
                {
                    
                    return null;
                }
            }
        }

        /// <summary>
        /// 加载指定配置
        /// </summary>
        public static void LoadConfigByName(string configName)
        {
            string configPath = GetConfigRootPath() + configName + ".xml";
            if (File.Exists(configPath))
            {
                Type type = Type.GetType("Assets.Script.Config." + configName+"Config");
                ConfigBase config = XMLHelper.LoadDataFromXML(type, configPath) as ConfigBase;
                allConfigMap[configName] = config;
            }
            else
            {
                Debug.LogError("配置：" + configName + "读取失败！");
            }
        }
    }
}
