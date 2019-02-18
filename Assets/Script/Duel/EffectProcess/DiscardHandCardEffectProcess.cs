using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 丢弃手卡
    /// </summary>
    class DiscardHandCardEffectProcess : EffectProcessBase
    {
        CardBase launchEffectCard;
        int needDiscardHandCardNumber = 1;//需要丢弃的卡牌数量

        Action<CardBase, CardBase> discardCardFinishAction;

        public DiscardHandCardEffectProcess(CardBase launchEffectCard,int needDiscardHandCardNumber, Action<CardBase, CardBase> discardCardFinishAction, Player ownerPlayer) :base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.launchEffectCard = launchEffectCard;
            this.needDiscardHandCardNumber = needDiscardHandCardNumber;
            this.discardCardFinishAction = discardCardFinishAction;
            finishAction += DiscardCardFinish;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void ProcessFunction()
        {
            haveProcess = true;
            GameManager.ShowMessage("请丢弃卡牌！");
            foreach (var item in ownerPlayer.GetHandCards())
            {
                item.GetDuelCardScript().SetClickCallback(launchEffectCard, DiscardOneHandCard);
            }
            ownerPlayer.ThinkAction();
        }

        /// <summary>
        /// 由外部调用，代表丢弃了一张手卡
        /// </summary>
        public void DiscardOneHandCard(CardBase launchEffectCard, CardBase discardCard)
        {
            discardCard.GetDuelCardScript().RemoveClickCallback();
            ownerPlayer.MoveCardToTomb(discardCard);
            needDiscardHandCardNumber -= 1;
            if(needDiscardHandCardNumber==0)
            {
                foreach (var item in ownerPlayer.GetHandCards())
                {
                    item.GetDuelCardScript().RemoveClickCallback();
                }
                AfterFinishProcessFunction();
            }
            else
            {
                GameManager.ShowMessage("请丢弃卡牌！");
                ownerPlayer.ThinkAction();
            }
        }

        /// <summary>
        /// 丢弃卡牌结束后
        /// </summary>
        void DiscardCardFinish()
        {
            discardCardFinishAction(launchEffectCard, null);
        }
    }
}
