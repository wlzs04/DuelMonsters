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

        int attackValue = 0;//攻击力
        int defenseValue = 0;//防御力

        int originalAttackValue = 0;//原本攻击力
        int originalDefenseValue = 0;//原本防御力

        int canBeSacrificedNumber = 1;//可被当成祭品的个数

        int attackNumber = 0; //攻击次数
        int changeAttackOrDefenseNumber = 0;//攻守转换次数

        bool canDirectAttack = true;//是否可以直接攻击
        bool canAttack = true;//是否进行攻击
        bool canBeAttacked = true;//是否可以被攻击
        bool canPenetrateDefense = false;//是否可以贯穿守备怪兽

        List<CardBase> equipCards = new List<CardBase>();//怪兽的装备卡列表

        public bool CanDirectAttack()
        {
            return canDirectAttack;
        }

        public bool CanBeAttacked()
        {
            return canBeAttacked;
        }

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        public bool CanAttack()
        {
            return  GetCardGameState() == CardGameState.FrontAttack && attackNumber > 0 && canAttack;
        }

        /// <summary>
        /// 进行攻击
        /// </summary>
        public void Attack()
        {
            attackNumber--;
            //攻击过后无法进行攻守转换
            changeAttackOrDefenseNumber--;
            duelCardScript.ClearPrepareAttackState();
            if (attackNumber <= 0)
            {
                duelCardScript.SetAttackState(false);
            }
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

        public void SetOriginalAttackValue(int originalAttackValue)
        {
            this.originalAttackValue = originalAttackValue;
            RecalculationCardByCardEffect();
        }

        public int GetOriginalAttackValue()
        {
            return originalAttackValue;
        }

        /// <summary>
        /// 设置攻击力
        /// </summary>
        /// <param name="attackValue"></param>
        public void SetAttackValue(int attackValue)
        {
            attackValue = attackValue > 0 ? attackValue : 0;
            this.attackValue = attackValue;
            infoDirty = true;
            if (GetDuelCardScript()!=null)
            {
                GetDuelCardScript().ResetAttackAndDefenseText();
            }
        }

        /// <summary>
        /// 获得攻击力
        /// </summary>
        /// <returns></returns>
        public int GetAttackValue()
        {
            return attackValue;
        }

        public void SetOriginalDefenseValue(int originalDefenseValue)
        {
            this.originalDefenseValue = originalDefenseValue;
            RecalculationCardByCardEffect();
        }

        public int GetOriginalDefenseValue()
        {
            return originalDefenseValue;
        }

        /// <summary>
        /// 设置防御力
        /// </summary>
        /// <param name="defenseValue"></param>
        public void SetDefenseValue(int defenseValue)
        {
            defenseValue = defenseValue > 0 ? defenseValue : 0;
            this.defenseValue = defenseValue;
            infoDirty = true;
            if (GetDuelCardScript()!= null)
            {
                GetDuelCardScript().ResetAttackAndDefenseText();
            }
        }

        /// <summary>
        /// 获得防御力
        /// </summary>
        /// <returns></returns>
        public int GetDefenseValue()
        {
            return defenseValue;
        }

        public void SetAttackNumber(int number)
        {
            attackNumber = number;
        }

        public void SetAttackNumber()
        {
            SetAttackNumber(DuelRuleManager.GetMonsterAttackNumberEveryTurn());
        }

        public int GetAttackNumber()
        {
            return attackNumber;
        }

        public void SetChangeAttackOrDefenseNumber(int number)
        {
            changeAttackOrDefenseNumber = number;
        }

        public void SetChangeAttackOrDefenseNumber()
        {
            SetChangeAttackOrDefenseNumber(DuelRuleManager.GetMonsterChangeAttackOrDefenseNumberEveryTurn());
        }

        public int GetChangeAttackOrDefenseNumber()
        {
            return changeAttackOrDefenseNumber;
        }

        /// <summary>
        /// 判断是否可以转换成攻击表示
        /// </summary>
        /// <returns></returns>
        public bool CanChangeToFrontAttack()
        {
            if (GetOwner().IsMyTurn() && 
                GetCardType() == CardType.Monster &&
                (GetCardGameState() == CardGameState.FrontDefense) &&
                GetChangeAttackOrDefenseNumber() > 0 &&
                (duelScene.GetCurrentPhaseType() == PhaseType.Main ||
                duelScene.GetCurrentPhaseType() == PhaseType.Second) &&
                GetOwner().GetCurrentEffectProcess() == null
                )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 判断是否可以转换成防守表示
        /// </summary>
        /// <returns></returns>
        public bool CanChangeToFrontDefense()
        {
            if (GetOwner().IsMyTurn() && 
                GetCardType() == CardType.Monster &&
                (GetCardGameState() == CardGameState.FrontAttack) &&
                GetChangeAttackOrDefenseNumber() > 0 &&
                (duelScene.GetCurrentPhaseType() == PhaseType.Main ||
                duelScene.GetCurrentPhaseType() == PhaseType.Second) &&
                GetOwner().GetCurrentEffectProcess() == null
                )
            {
                return true;
            }
            return false;
        }

        public void SetPropertyType(PropertyType propertyType)
        {
            this.propertyType = propertyType;
            infoDirty = true;
        }

        public void SetPropertyTypeByString(string value)
        {
            SetPropertyType(GetPropertyTypeByString(value));
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
            infoDirty = true;
        }

        public void SetMonsterTypeByString(string value)
        {
            SetMonsterType(GetMonsterTypeByString(value));
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
        public void MonsterLaunchEffectCallback()
        {

        }

        /// <summary>
        /// 怪兽卡离场后的回调
        /// </summary>
        public void MonsterExitAreaCallBack()
        {
            for (int i = equipCards.Count - 1; i >= 0; i--)
            {
                equipCards[i].GetOwner().MoveCardToTomb(equipCards[i]);
            }
        }

        /// <summary>
        /// 为怪兽添加装备卡
        /// </summary>
        public void AddEquip(CardBase equipCard)
        {
            if(equipCards.Contains(equipCard))
            {
                Debug.LogError("当前怪兽:" + GetName() + "已经装备卡牌:" + equipCard.GetName() + "！");
                return;
            }
            equipCards.Add(equipCard);
        }

        /// <summary>
        /// 移除怪兽的装备卡
        /// </summary>
        public void RemoveEquip(CardBase equipCard)
        {
            if (!equipCards.Contains(equipCard))
            {
                Debug.LogError("当前怪兽:" + GetName() + "没有装备卡牌:" + equipCard.GetName() + "！");
                return;
            }
            equipCards.Remove(equipCard);
            RemoveCardEffect(equipCard);
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
