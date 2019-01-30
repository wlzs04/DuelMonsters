using Assets.Script.Config;
using Assets.Script.Duel.EffectProcess;
using Assets.Script.Duel.Rule;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using XLua;

namespace Assets.Script.Card
{
    /// <summary>
    /// 魔法卡种类
    /// </summary>
    public enum MagicType
    {
        Unknown,//未知
        Normal,//通常
        Equipment,//装备
        Terrain,//地形
        Forever,//永续
        Ceremony,//仪式
        Quick,//速攻
    }

    /// <summary>
    /// 魔法卡部分
    /// </summary>
    public partial class CardBase
    {
        MagicType magicType = MagicType.Normal;

        public void SetMagicType(MagicType magicType)
        {
            this.magicType = magicType;
        }

        public void SetMagicTypeByString(string value)
        {
            magicType = GetMagicTypeByString(value);
        }

        public string GetMagicTypeString()
        {
            return GetStringByMagicType(magicType);
        }

        /// <summary>
        /// 将汉字转换为魔法种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MagicType GetMagicTypeByString(string value)
        {
            MagicTypeConfig config = ConfigManager.GetConfigByName("MagicType") as MagicTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if (config.GetRecordById(i).value == value)
                {
                    return (MagicType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将魔法种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByMagicType(MagicType magicType)
        {
            MagicTypeConfig config = ConfigManager.GetConfigByName("MagicType") as MagicTypeConfig;
            return config.GetRecordById((int)magicType).value;
        }

        //public override CardBase GetInstance()
        //{
        //    MagicCard magicCard = new MagicCard(cardNo);
        //    magicCard.SetImage(GetImage());
        //    magicCard.cardNo = cardNo;
        //    magicCard.cardID = RandomHelper.random.Next();
        //    magicCard.cardType = cardType;
        //    magicCard.name = name;
        //    magicCard.magicType = magicType;
        //    magicCard.effectInfo = effectInfo;
        //    return magicCard;
        //}
    }
}
