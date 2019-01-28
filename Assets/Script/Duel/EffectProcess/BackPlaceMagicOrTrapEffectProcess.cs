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
    /// 放置一张魔法或陷阱卡
    /// </summary>
    class BackPlaceMagicOrTrapEffectProcess : EffectProcessBase
    {
        CardBase backPlaceCard;

        public BackPlaceMagicOrTrapEffectProcess(CardBase card, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            backPlaceCard = card;
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
                    ownerPlayer.GetMagicTrapCardArea()[index] = backPlaceCard;
                    break;
                }
            }
            backPlaceCard.AddContent("magicTrapCardAreaIndex", index);
            backPlaceCard.SetCardGameState(CardGameState.Back);
            backPlaceCard.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
            backPlaceCard.GetDuelCardScript().SetLocalPosition(new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMagicTrapCardFarLeftPositionY, 1));
            ownerPlayer.GetHandCards().Remove(backPlaceCard);

            AfterFinishProcessFunction();
        }
    }
}
