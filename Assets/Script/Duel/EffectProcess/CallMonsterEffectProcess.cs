using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 召唤怪兽的方式
    /// </summary>
    public enum CallMonsterType
    {
        Unknown,//未知
        Normal,//通常
        Flip,//反转
        Sacrifice,//祭品
        Special,//特殊
    }

    /// <summary>
    /// 召唤怪兽
    /// </summary>
    public class CallMonsterEffectProcess : EffectProcessBase
    {
        CardBase calledMonster = null;//被召唤的怪兽
        CardGameState cardGameState;//召唤后卡牌状态
        CallMonsterType callMonsterType;//召唤类型

        List<CardBase> sacrificeCards = new List<CardBase>();//祭品列表

        public CallMonsterEffectProcess(CardBase calledMonster, CardGameState cardGameState, Player ownerPlayer) : base(ownerPlayer, "召唤怪兽")
        {
            canBeChained = true;
            this.calledMonster = calledMonster;
            this.cardGameState = cardGameState;

            finishAction += () => { };
        }

        public CallMonsterEffectProcess(CardBase calledMonster, CardGameState cardGameState, List<CardBase> sacrificeCards, Player ownerPlayer) : base(ownerPlayer, "召唤怪兽")
        {
            canBeChained = true;
            this.calledMonster = calledMonster;
            this.cardGameState = cardGameState;
            this.sacrificeCards = sacrificeCards;

            finishAction += () => { };
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
        {
            if(cardGameState== CardGameState.Unknown)
            {
                duelScene.ShowAttackOrDefensePanel(calledMonster, (cardGameState) =>
                {
                    this.cardGameState= cardGameState;
                    RealFunction();
                });
                return;
            }
            RealFunction();
        }

        /// <summary>
        /// 当所有信息齐后进行
        /// </summary>
        void RealFunction()
        {
            //如果当前怪兽的状态是里侧表示，则进行反转召唤
            if (calledMonster.GetCardGameState() == CardGameState.Back)
            {
                callMonsterType = CallMonsterType.Flip;
                calledMonster.SetCardGameState(cardGameState);
                calledMonster.SetChangeAttackOrDefenseNumber(0);
                calledMonster.GetDuelCardScript().SetOwner(ownerPlayer);
                CheckCardCanChainLaunch();
                return;
            }
            int monsterLevel = calledMonster.GetLevel();
            //先判断是否可以直接进行召唤
            if (monsterLevel <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
            {
                int index = CallMonster();
                calledMonster.GetDuelCardScript().SetOwner(ownerPlayer);
                ownerPlayer.GetOpponentPlayer().CallMonsterNotify(calledMonster.GetID(), CallType.Normal, CardGameState.Hand, cardGameState, index);
                CheckCardCanChainLaunch();
            }
            else//使用祭品召唤
            {
                if (ownerPlayer.GetCanBeSacrificeMonsterNumber() >= calledMonster.NeedSacrificeMonsterNumer())
                {
                    callMonsterType = CallMonsterType.Sacrifice;
                    TrySacrificeCall();
                }
            }
        }

        /// <summary>
        /// 召唤怪兽
        /// </summary>
        int CallMonster()
        {
            int index = 0;
            for (; index < DuelRuleManager.GetMonsterAreaNumber(); index++)
            {
                if (ownerPlayer.GetMonsterCardArea()[index] == null)
                {
                    ownerPlayer.GetMonsterCardArea()[index] = calledMonster;
                    break;
                }
            }
            callMonsterType = CallMonsterType.Normal;
            calledMonster.AddContent("monsterCardAreaIndex", index);

            if(calledMonster.GetCardGameState()==CardGameState.Tomb)
            {
                calledMonster.GetDuelCardScript().GetOwner().GetTombCards().Remove(calledMonster);
            }
            else if(calledMonster.GetCardGameState() == CardGameState.Hand)
            {
                calledMonster.GetDuelCardScript().GetOwner().GetHandCards().Remove(calledMonster);
            }

            calledMonster.SetCardGameState(cardGameState, index);
            
            ownerPlayer.SetNormalCallNumber(ownerPlayer.GetNormalCallNumber() - 1);
            return index;
        }

        /// <summary>
        /// 判断是否可以选择作为祭品
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool CanChooseMonsterAsSacrifice(CardBase card)
        {
            if (!sacrificeCards.Contains(card))
            {
                sacrificeCards.Add(card);
                return true;
            }
            else
            {
                Debug.LogError("ChooseMonsterAsSacrifice 已经选中了此怪兽:" + card.GetName());
                return false;
            }
        }

        /// <summary>
        /// 获得当前列表中可被祭品的个数
        /// </summary>
        /// <returns></returns>
        int GetCurrentSacrificeCount()
        {
            int count = 0;
            foreach (var item in sacrificeCards)
            {
                count+= item.GetCanBeSacrificedNumber();
            }
            return count;
        }

        public CallMonsterType GetCallMonsterType()
        {
            return callMonsterType;
        }

        /// <summary>
        /// 检测当前祭品数量是否足够,足够则直接进行祭品召唤
        /// </summary>
        /// <returns></returns>
        public void TrySacrificeCall()
        {
            if(callMonsterType== CallMonsterType.Sacrifice)
            {
                if (GetCurrentSacrificeCount() >= calledMonster.NeedSacrificeMonsterNumer())
                {
                    CallMonsterWithSacrifice();
                }
            }
        }

        /// <summary>
        /// 使用祭品进行召唤
        /// </summary>
        public void CallMonsterWithSacrifice()
        {
            foreach (var item in sacrificeCards)
            {
                item.GetDuelCardScript().ClearCurrentState();
                ownerPlayer.MoveCardToTomb(item);
            }

            int index = CallMonster();

            StringBuilder sacrificeInfo = new StringBuilder();
            for (int i = 0; i < sacrificeCards.Count; i++)
            {
                if (i == sacrificeCards.Count - 1)
                {
                    sacrificeInfo.Append(sacrificeCards[i].GetID());
                }
                else
                {
                    sacrificeInfo.Append(sacrificeCards[i].GetID() + ":");
                }
            }
            calledMonster.GetDuelCardScript().SetOwner(ownerPlayer);
            ownerPlayer.GetOpponentPlayer().CallMonsterWithSacrificeNotify(calledMonster.GetID(), CallType.Normal, CardGameState.Hand, cardGameState, index, sacrificeInfo.ToString());

            sacrificeCards.Clear();

            CheckCardCanChainLaunch();
        }

        public CardBase GetCalledMonster()
        {
            return calledMonster;
        }

        protected override void RealProcessFunction()
        {
            AfterFinishProcessFunction();
        }
    }
}
