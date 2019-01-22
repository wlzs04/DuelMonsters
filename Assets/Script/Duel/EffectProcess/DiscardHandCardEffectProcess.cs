using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Events;

namespace Assets.Script.Duel.EffectProcess
{
    class DiscardHandCardEffectProcess : EffectProcessBase
    {
        int needDiscardHandCardNumber = 1;//需要丢弃的卡牌数量

        public DiscardHandCardEffectProcess(int needDiscardHandCardNumber, Player ownerPlayer,UnityAction finishAction) :base(ownerPlayer, finishAction)
        {
            this.needDiscardHandCardNumber = needDiscardHandCardNumber;
            effectProcessType = EffectProcessType.RemoveAfterFinish;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void ProcessFunction()
        {
            haveProcess = true;
            GameManager.ShowMessage("请丢弃卡牌！");
            ownerPlayer.ThinkAction();
        }

        /// <summary>
        /// 由外部调用，代表丢弃了一张手卡
        /// </summary>
        public void DiscardOneHandCard()
        {
            needDiscardHandCardNumber -= 1;
            if(needDiscardHandCardNumber==0)
            {
                AfterFinishProcessFunction();
            }
            else
            {
                GameManager.ShowMessage("请丢弃卡牌！");
                ownerPlayer.ThinkAction();
            }
        }
    }
}
