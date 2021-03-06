using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 转换攻防
    /// </summary>
    class ChangeAttackDefenseEffectProcess : EffectProcessBase
    {
        CardBase monsterCard;
        CardGameState cardGameState;

        public ChangeAttackDefenseEffectProcess(CardBase monsterCard,CardGameState cardGameState,Player ownerPlayer) : base(ownerPlayer, "转换攻防")
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.monsterCard = monsterCard;
            launchEffectCard = monsterCard;
            this.cardGameState = cardGameState;
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
        {
            switch (cardGameState)
            {
                case CardGameState.FrontAttack:
                case CardGameState.FrontDefense:
                    monsterCard.SetChangeAttackOrDefenseNumber(monsterCard.GetChangeAttackOrDefenseNumber()-1);
                    monsterCard.SetCardGameState(cardGameState);
                    break;
                default:
                    Debug.LogError("转换攻防时状态不对：" + cardGameState);
                    break;
            }
            monsterCard.SetCardGameState(cardGameState);
            AfterFinishProcessFunction();
        }
    }
}
