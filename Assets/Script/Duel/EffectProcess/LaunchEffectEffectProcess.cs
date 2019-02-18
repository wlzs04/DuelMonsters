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
            if(!launchEffectCard.IsInArea())
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
                if (launchEffectCard.GetCardGameState() == CardGameState.Hand)
                {
                    ownerPlayer.GetHandCards().Remove(launchEffectCard);
                }
                launchEffectCard.SetCardGameState(CardGameState.Front, index);
            }
            else
            {
                launchEffectCard.SetCardGameState(CardGameState.Front);
            }
            
            launchEffectCard.LaunchEffect();

            AfterFinishProcessFunction();
        }
    }
}
