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
        public LaunchEffectEffectProcess(CardBase launchEffectCard, Player ownerPlayer) : base(ownerPlayer, "发动效果")
        {
            canBeChained = true;
            this.launchEffectCard = launchEffectCard;
            finishAction += () => 
            {
                launchEffectCard.LaunchEffectFinishCallBack();
                ownerPlayer.ThinkAction();
            };
        }

        public override void Update()
        {
            base.Update();
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
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
            launchEffectCard.BeforeLaunchEffect();
            CheckCardCanChainLaunch();
        }

        protected override void RealProcessFunction()
        {
            if(!beDisabled)
            {
                launchEffectCard.LaunchEffect();
            }
            else
            {
                Debug.Log($"{launchEffectCard.GetName()}此次发动被无效！");


            }
            AfterFinishProcessFunction();
        }

        public CardBase GetLaunchCard()
        {
            return launchEffectCard;
        }
    }
}
