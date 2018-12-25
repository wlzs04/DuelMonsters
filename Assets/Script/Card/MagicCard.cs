using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    /// <summary>
    /// 魔法卡种类
    /// </summary>
    enum MagicType
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
    /// 魔法卡
    /// </summary>
    class MagicCard : CardBase
    {
        public MagicCard()
        {
            cardType = CardType.Magic;
        }

        MagicType magicType = MagicType.Normal;

        public string GetMagicTypeString()
        {
            return GetStringByMagicType(magicType);
        }

        protected override void LoadInfo(string info)
        {
            string[] keyValues = info.Split('\n');
            foreach (var item in keyValues)
            {
                string key = item.Substring(0, item.IndexOf(':'));
                string value = item.Substring(item.IndexOf(':') + 1);
                switch (key)
                {
                    case "Name":
                        name = value;
                        break;
                    case "MagicType":
                        magicType = GetMagicTypeByString(value);
                        break;
                    case "Effect":
                        effect = value;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 将汉字转换为魔法种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MagicType GetMagicTypeByString(string value)
        {
            switch (value)
            {
                case "通常":
                    return MagicType.Normal;
                case "速攻":
                    return MagicType.Quick;
                case "环境":
                    return MagicType.Environment;
                case "装备":
                    return MagicType.Equipment;
                case "永续":
                    return MagicType.Forever;
                default:
                    return MagicType.Unknown;
            }
        }

        /// <summary>
        /// 将魔法种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByMagicType(MagicType magicType)
        {
            switch (magicType)
            {
                case MagicType.Normal:
                    return "通常";
                case MagicType.Quick:
                    return "速攻";
                case MagicType.Environment:
                    return "环境";
                case MagicType.Equipment:
                    return "装备";
                case MagicType.Forever:
                    return "永续";
                default:
                    return "未知";
            }
        }

        public override CardBase GetInstance()
        {
            MagicCard magicCard = new MagicCard();
            magicCard.SetImage(GetImage());
            magicCard.cardNo = cardNo;
            magicCard.cardID = RandomHelper.random.Next();
            magicCard.cardType = cardType;
            magicCard.name = name;
            magicCard.magicType = magicType;
            magicCard.effect = effect;
            return magicCard;
        }
    }
}
