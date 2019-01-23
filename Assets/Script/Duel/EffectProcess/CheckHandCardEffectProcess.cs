using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    class CheckHandCardEffectProcess : EffectProcessBase
    {
        public CheckHandCardEffectProcess(Player ownerPlayer):base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.Forever;
            finishAction = duelScene.ChangeCurrentPlayer;
        }

        /// <summary>
        /// 在回合结束后检查手牌
        /// </summary>
        /// <returns></returns>
        public override bool CheckCanTrigger()
        {
            if (ownerPlayer == duelScene.currentPlayer &&
                duelScene.currentDuelProcess==DuelProcess.End &&
                !haveProcess)
            {
                return true;
            }
            return false;
        }

        protected override void ProcessFunction()
        {
            haveProcess = true;
            if (ownerPlayer.GetHandCards().Count > DuelRuleManager.GetHandCardNumberUpperLimit())
            {
                GameManager.ShowMessage("当前手牌数量大于规定数量！");
                int number = ownerPlayer.GetHandCards().Count - DuelRuleManager.GetHandCardNumberUpperLimit();
                DiscardHandCardEffectProcess discardHandCardEffectProcess = new DiscardHandCardEffectProcess(number, ownerPlayer, ProcessFunction);
                ownerPlayer.AddEffectProcess(discardHandCardEffectProcess);
            }
            else
            {
                AfterFinishProcessFunction();
            }
        }
    }
}
