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
        CardBase callMonster = null;//被召唤的怪兽
        CardGameState cardGameState;//召唤后卡牌状态
        CallMonsterType callMonsterType;//召唤类型

        List<CardBase> sacrificeCards = new List<CardBase>();//祭品列表

        public CallMonsterEffectProcess(CardBase callMonster, CardGameState cardGameState, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.callMonster = callMonster;
            this.cardGameState = cardGameState;

            finishAction += () => { };
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void ProcessFunction()
        {
            if(cardGameState== CardGameState.Unknown)
            {
                duelScene.ShowAttackOrDefensePanel(callMonster, (cardGameState) =>
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
            if (callMonster.GetCardGameState() == CardGameState.Back)
            {
                callMonsterType = CallMonsterType.Flip;
                callMonster.SetCardGameState(cardGameState);
                callMonster.SetChangeAttackOrDefenseNumber(0);
                callMonster.GetDuelCardScript().SetOwner(ownerPlayer);
                AfterFinishProcessFunction();
                return;
            }
            int monsterLevel = callMonster.GetLevel();
            //先判断是否可以直接进行召唤
            if (monsterLevel <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
            {
                int index = CallMonster();
                callMonster.GetDuelCardScript().SetOwner(ownerPlayer);
                ownerPlayer.GetOpponentPlayer().CallMonsterNotify(callMonster.GetID(), CallType.Normal, CardGameState.Hand, cardGameState, index);
                AfterFinishProcessFunction();
            }
            else//使用祭品召唤
            {
                if (ownerPlayer.GetCanBeSacrificeMonsterNumber() >= callMonster.NeedSacrificeMonsterNumer())
                {
                    callMonsterType = CallMonsterType.Sacrifice;
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
                    ownerPlayer.GetMonsterCardArea()[index] = callMonster;
                    break;
                }
            }
            callMonsterType = CallMonsterType.Normal;
            callMonster.AddContent("monsterCardAreaIndex", index);

            if(callMonster.GetCardGameState()==CardGameState.Tomb)
            {
                callMonster.GetDuelCardScript().GetOwner().GetTombCards().Remove(callMonster);
            }
            else if(callMonster.GetCardGameState() == CardGameState.Hand)
            {
                callMonster.GetDuelCardScript().GetOwner().GetHandCards().Remove(callMonster);
            }

            callMonster.SetCardGameState(cardGameState);
            callMonster.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
            callMonster.GetDuelCardScript().SetLocalPosition(new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1));
            
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
                if (sacrificeCards.Count >= callMonster.NeedSacrificeMonsterNumer())
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
            callMonster.GetDuelCardScript().SetOwner(ownerPlayer);
            ownerPlayer.GetOpponentPlayer().CallMonsterWithSacrificeNotify(callMonster.GetID(), CallType.Normal, CardGameState.Hand, cardGameState, index, sacrificeInfo.ToString());

            sacrificeCards.Clear();
            AfterFinishProcessFunction();
        }
    }
}
