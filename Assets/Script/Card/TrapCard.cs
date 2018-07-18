using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    enum TrapType
    {
        Unknown,//未知
        Normal,//通常
        Forever,//永续
        BeatBack//反击
    }

    class TrapCard : CardBase
    {
        TrapType trapType = TrapType.Normal;

        public TrapCard()
        {
            cardType = CardType.Trap;
        }

        public string GetTrapTypeString()
        {
            return GetStringByTrapType(trapType);
        }

        public override void LoadInfo(string info)
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
                    case "TrapType":
                        trapType = GetTrapTypeByString(value);
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
        /// 将汉字转换为陷阱种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static TrapType GetTrapTypeByString(string value)
        {
            switch (value)
            {
                case "通常":
                    return TrapType.Normal;
                case "永续":
                    return TrapType.Forever;
                case "反击":
                    return TrapType.BeatBack;
                default:
                    return TrapType.Unknown;
            }
        }

        /// <summary>
        /// 将陷阱种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByTrapType(TrapType trapType)
        {
            switch (trapType)
            {
                case TrapType.Normal:
                    return "通常";
                case TrapType.Forever:
                    return "永续";
                case TrapType.BeatBack:
                    return "反击";
                default:
                    return "未知";
            }
        }

        public override CardBase GetInstance()
        {
            Random random = new Random();

            TrapCard trapCard = new TrapCard();
            trapCard.name = name;
            trapCard.SetImage(GetImage());
            trapCard.cardNo = cardNo;
            trapCard.cardID = random.Next();
            trapCard.cardType = cardType;
            trapCard.trapType = trapType;
            trapCard.effect = effect;
            return trapCard;
        }
    }
}
