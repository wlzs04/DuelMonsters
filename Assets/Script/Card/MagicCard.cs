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

        CardBase equidMonster = null;//装备此卡的怪兽

        public void SetMagicType(MagicType magicType)
        {
            this.magicType = magicType;
        }

        public void SetMagicTypeByString(string value)
        {
            magicType = GetMagicTypeByString(value);
        }

        public MagicType GetMagicType()
        {
            return magicType;
        }

        public string GetMagicTypeString()
        {
            return GetStringByMagicType(magicType);
        }

        /// <summary>
        /// 魔法卡判断是否可以发动效果
        /// </summary>
        /// <returns></returns>
        public bool MagicCanLaunchEffect()
        {
            if (GetCardType() == CardType.Magic &&
                GetOwner().IsMyTurn() &&
                (duelScene.GetCurrentPhaseType() == PhaseType.Main ||
                duelScene.GetCurrentPhaseType() == PhaseType.Second) &&
                GetOwner().GetCurrentEffectProcess() == null
                )
            {
                if (GetCardGameState() == CardGameState.Hand)
                {
                    return !GetOwner().MagicTrapAreaIsFull() && canLaunchEffectAction != null ? canLaunchEffectAction(this) : true;
                }
                else if (IsInArea(cardGameState))
                {
                    return (duelScene.GetCurrentTurnNumber() != GetBePlacedAreaTurnNumber()) && (canLaunchEffectAction != null ? canLaunchEffectAction(this) : true);
                }
            }
            return false;
        }

        /// <summary>
        /// 魔法卡发动效果后的回调
        /// </summary>
        public void MagicLaunchEffectCallback()
        {
            switch (magicType)
            {
                case MagicType.Normal:
                    GetDuelCardScript().GetOwner().MoveCardToTomb(this);
                    break;
                case MagicType.Equipment:
                    break;
                case MagicType.Terrain:
                    break;
                case MagicType.Forever:
                    break;
                case MagicType.Ceremony:
                    break;
                case MagicType.Quick:
                    GetDuelCardScript().GetOwner().MoveCardToTomb(this);
                    break;
                default:
                    Debug.LogError("未知MagicType：" + magicType);
                    break;
            }
        }

        /// <summary>
        /// 魔法卡离场后的回调
        /// </summary>
        public void MagicExitAreaCallBack()
        {
            if (GetEquidMonster()==null)
            {
                return;
            }
            GetEquidMonster().RemoveEquip(this);
            SetEquidMonster(null);
        }


        public void SetEquidMonster(CardBase equidMonster)
        {
            if(this.equidMonster!=null && equidMonster!=null)
            {
                Debug.LogError("装备卡：" + GetName() + "已经装备到怪兽：" + this.equidMonster.GetName() + "上了！将重新装备到怪兽：" + equidMonster.GetName() + "上！");
            }
            this.equidMonster = equidMonster;
        }

        public CardBase GetEquidMonster()
        {
            return equidMonster;
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
    }
}
