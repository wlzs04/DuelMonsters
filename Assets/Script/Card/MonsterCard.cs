using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    enum MonsterType
    {
        Unknown,
        Water,
        Fire,
        Wind,
        Soil,
        Light,
        Dark
    }
    
    class MonsterCard : CardBase
    {
        int level = 4;//等级
        MonsterType monsterType = MonsterType.Dark;//属性
        bool isNormal = true;//是否为通常怪
        int attackNumber = 1500;//攻击力
        int defenseNumber = 500;//防御力

        public MonsterCard()
        {
            cardType = CardType.Monster;
        }

        public int GetLevel()
        {
            return level;
        }

        public int GetAttackNumber()
        {
            return attackNumber;
        }

        public int GetDefenseNumber()
        {
            return defenseNumber;
        }

        public bool IsNormal()
        {
            return isNormal;
        }

        public MonsterType GetMonsterType()
        {
            return monsterType;
        }

        public string GetMonsterTypeString()
        {
            return GetStringByMonsterType(monsterType);
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
                    case "MonsterType":
                        monsterType = GetMonsterTypeByString(value);
                        break;
                    case "Level":
                        level = int.Parse(value);
                        break;
                    case "ATK":
                        attackNumber = int.Parse(value);
                        break;
                    case "DEF":
                        defenseNumber = int.Parse(value);
                        break;
                    case "Noraml":
                        isNormal = bool.Parse(value);
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
        /// 将汉字转换为怪物种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MonsterType GetMonsterTypeByString(string value)
        {
            switch (value)
            {
                case "水":
                    return MonsterType.Water;
                case "炎":
                    return MonsterType.Fire;
                case "风":
                    return MonsterType.Wind;
                case "地":
                    return MonsterType.Soil;
                case "光":
                    return MonsterType.Light;
                case "暗":
                    return MonsterType.Dark;
                default:
                    return MonsterType.Unknown;
            }
        }

        /// <summary>
        /// 将怪物种类转换为汉字
        /// </summary>
        /// <param name="monsterType"></param>
        /// <returns></returns>
        public static string GetStringByMonsterType(MonsterType monsterType)
        {
            switch (monsterType)
            {
                case MonsterType.Water:
                    return "水";
                case MonsterType.Fire:
                    return "炎";
                case MonsterType.Wind:
                    return "风";
                case MonsterType.Soil:
                    return "地";
                case MonsterType.Light:
                    return "光";
                case MonsterType.Dark:
                    return "暗";
                default:
                    return "未知";
            }
        }

        public override CardBase GetInstance()
        {
            Random random = new Random();

            MonsterCard monsterCard = new MonsterCard();
            monsterCard.name = name;
            monsterCard.SetImage(GetImage());
            monsterCard.cardNo = cardNo;
            monsterCard.cardID = random.Next();
            monsterCard.cardType = cardType;
            monsterCard.monsterType = monsterType;
            monsterCard.level = level;
            monsterCard.attackNumber = attackNumber;
            monsterCard.defenseNumber = defenseNumber;
            monsterCard.isNormal = isNormal;
            monsterCard.effect = effect;
            return monsterCard;
        }
    }
}
