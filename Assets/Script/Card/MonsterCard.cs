using Assets.Script.Config;
using Assets.Script.Duel.Rule;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    /// <summary>
    /// 怪兽卡属性种类
    /// </summary>
    enum PropertyType
    {
        Unknown,
        Light,//光
        Dark,//暗
        Soil,//地
        Wind,//风
        Water,//水
        Fire,//炎
        Gold,//神
    }

    /// <summary>
    /// 怪兽卡种族
    /// </summary>
    enum MonsterType
    {
        Unknown,
        Dragon,//龙
        SeaDragon,//海龙
        MagicDragon,//幻龙
        Immortal,//不死
        Devil,//恶魔
        Angel,//天使
        Machine,//机械
        Magician,//魔法师
        Warrior,//战士
        BeastWarrior,//兽战士
        Water,//水
        Fire,//炎
        Thunder,//雷
        Rock,//岩石
        Reptile,//爬虫
        Beast,//兽
        BirdBeast,//鸟兽
        Fish,//鱼
        Dinosaur,//恐龙
        Insect,//昆虫
        Plant,//植物
        Psychokinesis,//念动力
        Cyberse,//电子界
        MagicGold,//幻神兽
        CreateGold,//创造神
    }

    /// <summary>
    /// 召唤方式
    /// </summary>
    enum CallType
    {
        Unknown,//未知
        Normal,//通常召唤
        Sacrifice,//祭品召唤
        Special,//特殊召唤
    }

    /// <summary>
    /// 怪兽卡
    /// </summary>
    class MonsterCard : CardBase
    {
        int level = 4;//等级
        PropertyType propertyType = PropertyType.Unknown;//属性
        MonsterType monsterType = MonsterType.Unknown;//种族

        bool isNormal = true;//是否为通常怪
        int attackNumber = 1500;//攻击力
        int defenseNumber = 500;//防御力
        int canBeSacrificedNumber = 1;//可被当成祭品的个数

        bool canDirectAttack = true;//是否可以直接攻击
        bool canBeAttacked = true;

        public MonsterCard()
        {
            cardType = CardType.Monster;
        }

        public bool CanDirectAttack
        {
            get
            {
                return canDirectAttack;
            }

            set
            {
                canDirectAttack = value;
            }
        }

        public bool CanBeAttacked
        {
            get
            {
                return canBeAttacked;
            }

            set
            {
                canBeAttacked = value;
            }
        }

        public int GetCanBeSacrificedNumber()
        {
            return canBeSacrificedNumber;
        }

        /// <summary>
        /// 召唤需要的祭品数量
        /// </summary>
        /// <returns></returns>
        public int NeedSacrificeMonsterNumer()
        {
            if(GetLevel()>DuelRule.callMonsterWithoutSacrificeMaxLevel)
            {
                if(GetLevel() > DuelRule.callMonsterWithOneSacrificeMaxLevel)
                {
                    return 2;
                }
                else
                {
                    return 1;
                }
            }
            return 0;
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

        public PropertyType GetPropertyType()
        {
            return propertyType;
        }

        /// <summary>
        /// 获得属性类型的文字描述
        /// </summary>
        /// <returns></returns>
        public string GetPropertyTypeString()
        {
            return GetStringByPropertyType(propertyType);
        }

        /// <summary>
        /// 获得种族类型的文字描述
        /// </summary>
        /// <returns></returns>
        public string GetMonsterTypeString()
        {
            return GetStringByMonsterType(monsterType);
        }

        protected override void LoadInfo(string info)
        {
            string[] keyValues = info.Split(new string[] { "\n", "\r\n" },StringSplitOptions.None);
            foreach (var item in keyValues)
            {
                string key = item.Substring(0, item.IndexOf(':'));
                string value = item.Substring(item.IndexOf(':') + 1);
                switch (key)
                {
                    case "Name":
                        name = value;
                        break;
                    case "PropertyType":
                        propertyType = GetPropertyTypeByString(value);
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
        /// 将汉字转换为属性种类
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static PropertyType GetPropertyTypeByString(string value)
        {
            PropertyTypeConfig config = ConfigManager.GetConfigByName("PropertyType") as PropertyTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if (config.GetRecordById(i).value == value)
                {
                    return (PropertyType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将属性种类转换为汉字
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static string GetStringByPropertyType(PropertyType propertyType)
        {
            PropertyTypeConfig config = ConfigManager.GetConfigByName("PropertyType") as PropertyTypeConfig;
            return config.GetRecordById((int)propertyType).value;
        }

        /// <summary>
        /// 将汉字转换为怪兽种族
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MonsterType GetMonsterTypeByString(string value)
        {
            MonsterTypeConfig config = ConfigManager.GetConfigByName("MonsterType") as MonsterTypeConfig;
            int count = config.GetRecordCount();
            for (int i = 0; i < count; i++)
            {
                if (config.GetRecordById(i).value == value)
                {
                    return (MonsterType)i;
                }
            }
            return 0;
        }

        /// <summary>
        /// 将怪兽种族转换为汉字
        /// </summary>
        /// <param name="propertyType"></param>
        /// <returns></returns>
        public static string GetStringByMonsterType(MonsterType monsterType)
        {
            MonsterTypeConfig config = ConfigManager.GetConfigByName("MonsterType") as MonsterTypeConfig;
            return config.GetRecordById((int)monsterType).value;
        }

        /// <summary>
        /// 获得当前卡牌的一个实例
        /// </summary>
        /// <returns></returns>
        public override CardBase GetInstance()
        {
            MonsterCard monsterCard = new MonsterCard();
            monsterCard.name = name;
            monsterCard.SetImage(GetImage());
            monsterCard.cardNo = cardNo;
            monsterCard.cardID = RandomHelper.random.Next();
            monsterCard.cardType = cardType;
            monsterCard.propertyType = propertyType;
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
