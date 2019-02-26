using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 掷骰子
    /// </summary>
    class ThrowDiceEffectProcess : EffectProcessBase
    {
        CardBase launchEffectCard;
        ActionIndex throwDiceCallBack;

        public ThrowDiceEffectProcess(CardBase launchEffectCard, ActionIndex throwDiceCallBack, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;

            this.launchEffectCard = launchEffectCard;
            this.throwDiceCallBack = throwDiceCallBack;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;
            launchEffectCard.GetDuelCardScript().GetDuelScene().ShowThrowDicePanel(ThrowDiceCallBack);
        }

        void ThrowDiceCallBack(int resultNumber)
        {
            AfterFinishProcessFunction();
            throwDiceCallBack(launchEffectCard, resultNumber);
        }
    }
}
