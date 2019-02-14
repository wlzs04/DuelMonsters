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
    /// 发动效果
    /// </summary>
    class LaunchEffectEffectProcess : EffectProcessBase
    {
        CardBase launchEffectCard;

        public LaunchEffectEffectProcess(CardBase launchEffectCard, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.launchEffectCard = launchEffectCard;
            finishAction += () => { ownerPlayer.ThinkAction(); };
        }

        public override void Update()
        {
            base.Update();
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void ProcessFunction()
        {
            int index = 0;
            for (; index < DuelRuleManager.GetMagicTrapAreaNumber(); index++)
            {
                if (ownerPlayer.GetMagicTrapCardArea()[index] == null)
                {
                    ownerPlayer.GetMagicTrapCardArea()[index] = launchEffectCard;
                    break;
                }
            }
            launchEffectCard.AddContent("magicTrapCardAreaIndex", index);
            if(launchEffectCard.GetCardGameState()==CardGameState.Hand)
            {
                ownerPlayer.GetHandCards().Remove(launchEffectCard);
            }
            launchEffectCard.SetCardGameState(CardGameState.Front);
            launchEffectCard.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
            launchEffectCard.GetDuelCardScript().SetLocalPosition(new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMagicTrapCardFarLeftPositionY, 1));
            
            launchEffectCard.LaunchEffect();

            AfterFinishProcessFunction();
        }
    }
}
