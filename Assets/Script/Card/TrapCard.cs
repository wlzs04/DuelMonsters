using Assets.Script.Config;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Card
{
    /// <summary>
    /// 陷阱卡种类
    /// </summary>
    public enum TrapType
    {
        Unknown,//未知
        Normal,//通常
        Forever,//永续
        BeatBack//反击
    }

    /// <summary>
    /// 陷阱卡部分
    /// </summary>
    public partial class CardBase
    {
        TrapType trapType = TrapType.Normal;

        public void SetTrapType(TrapType trapType)
        {
            this.trapType = trapType;
        }

        public void SetTrapTypeByString(string value)
        {
            trapType = GetTrapTypeByString(value);
        }

        public TrapType GetTrapType()
        {
            return trapType;
        }

        public string GetTrapTypeString()
        {
            return GetStringByTrapType(trapType);
        }

        /// <summary>
        /// 陷阱卡发动效果后的回调
        /// </summary>
        public void TrapLaunchEffectCallback()
        {
            switch (trapType)
            {
                case TrapType.Normal:
                    GetDuelCardScript().GetOwner().MoveCardToTomb(this);
                    break;
                case TrapType.Forever:
                    break;
                case TrapType.BeatBack:
                    break;
                default:
                    Debug.LogError("未知TrapType：" + trapType);
                    break;
            }
        }

        /// <summary>
        /// 将汉字转换为陷阱种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TrapType GetTrapTypeByString(string value)
        {
            TrapTypeConfig config = ConfigManager.GetConfigByName("TrapType") as TrapTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if (config.GetRecordById(i).value == value)
                {
                    return (TrapType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将陷阱种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByTrapType(TrapType trapType)
        {
            TrapTypeConfig config = ConfigManager.GetConfigByName("TrapType") as TrapTypeConfig;
            return config.GetRecordById((int)trapType).value;
        }
    }
}
