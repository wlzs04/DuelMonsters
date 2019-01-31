using Assets.Script.Config;
using Assets.Script.Duel.Rule;
using Assets.Script.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Card
{
    /// <summary>
    /// 怪兽卡属性种类
    /// </summary>
    public enum PropertyType
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
    public enum MonsterType
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
    public enum CallType
    {
        Unknown,//未知
        Normal,//通常召唤
        Sacrifice,//祭品召唤
        Special,//特殊召唤
    }

    /// <summary>
    /// 怪兽卡部分
    /// </summary>
    public partial class CardBase
    {
        int level = 4;//等级
        PropertyType propertyType = PropertyType.Unknown;//属性
        MonsterType monsterType = MonsterType.Unknown;//种族

        int attackNumber = 1500;//攻击力
        int defenseNumber = 500;//防御力
        int canBeSacrificedNumber = 1;//可被当成祭品的个数

        bool canDirectAttack = true;//是否可以直接攻击
        bool canBeAttacked = true;//是否可以被攻击
        bool canPenetrateDefense = false;//是否可以贯穿守备怪兽

        List<CardBase> equipCards = new List<CardBase>();//怪兽的装备卡列表

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

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanAttack()
        {
            return  GetCardGameState() == CardGameState.FrontAttack;
        }

        public bool GetCanPenetrateDefense()
        {
            return canPenetrateDefense;
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
            if(GetLevel()>DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
            {
                if(GetLevel() > DuelRuleManager.GetCallMonsterWithOneSacrificeLevelUpperLimit())
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

        public void SetLevel(int level)
        {
            this.level = level;
        }

        public int GetLevel()
        {
            return level;
        }

        public void SetAttackNumber(int attackNumber)
        {
            attackNumber = attackNumber > 0 ? attackNumber : 0;
            this.attackNumber = attackNumber;
            if(GetDuelCardScript()!=null)
            {
                GetDuelCardScript().ResetAttackAndDefenseText();
            }
        }

        public int GetAttackNumber()
        {
            return attackNumber;
        }

        public void SetDefenseNumber(int defenseNumber)
        {
            defenseNumber = defenseNumber > 0 ? defenseNumber : 0;
            this.defenseNumber = defenseNumber;
            if (GetDuelCardScript()!= null)
            {
                GetDuelCardScript().ResetAttackAndDefenseText();
            }
        }

        public int GetDefenseNumber()
        {
            return defenseNumber;
        }

        public void SetPropertyType(PropertyType propertyType)
        {
            this.propertyType = propertyType;
        }

        public void SetPropertyTypeByString(string value)
        {
            propertyType = GetPropertyTypeByString(value);
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

        public void SetMonsterType(MonsterType monsterType)
        {
            this.monsterType = monsterType;
        }

        public void SetMonsterTypeByString(string value)
        {
            monsterType = GetMonsterTypeByString(value);
        }

        public MonsterType GetMonsterType()
        {
            return monsterType;
        }

        /// <summary>
        /// 获得种族类型的文字描述
        /// </summary>
        /// <returns></returns>
        public string GetMonsterTypeString()
        {
            return GetStringByMonsterType(monsterType);
        }

        /// <summary>
        /// 怪兽卡发动效果后的回调
        /// </summary>
        public void MonsterLaunchEffectCalback()
        {

        }

        /// <summary>
        /// 为怪兽添加装备卡
        /// </summary>
        public void AddEquip(CardBase equipCard)
        {
            if(equipCards.Contains(equipCard))
            {
                Debug.LogError("当前怪兽已经装备此卡牌！");
                return;
            }
            equipCards.Add(equipCard);
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
    }
}
