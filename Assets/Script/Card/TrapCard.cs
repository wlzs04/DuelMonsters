using Assets.Script.Config;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    /// <summary>
    /// 陷阱卡种类
    /// </summary>
    enum TrapType
    {
        Unknown,//未知
        Normal,//通常
        Forever,//永续
        BeatBack//反击
    }

    /// <summary>
    /// 陷阱卡
    /// </summary>
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
            TrapTypeConfig config = ConfigManager.GetSingleInstance().GetConfigByName("TrapType") as TrapTypeConfig;
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
            TrapTypeConfig config = ConfigManager.GetSingleInstance().GetConfigByName("TrapType") as TrapTypeConfig;
            return config.GetRecordById((int)trapType).value;
        }

        public override CardBase GetInstance()
        {
            TrapCard trapCard = new TrapCard();
            trapCard.name = name;
            trapCard.SetImage(GetImage());
            trapCard.cardNo = cardNo;
            trapCard.cardID = RandomHelper.random.Next();
            trapCard.cardType = cardType;
            trapCard.trapType = trapType;
            trapCard.effect = effect;
            return trapCard;
        }
    }
}
