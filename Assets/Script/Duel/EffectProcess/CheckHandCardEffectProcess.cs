using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 在回合结束后检查手牌是否超过规定的最大数量，超过的话丢弃多余数量
    /// </summary>
    class CheckHandCardEffectProcess : EffectProcessBase
    {
        public CheckHandCardEffectProcess(Player ownerPlayer):base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.Forever;
            finishAction = duelScene.ChangeCurrentPlayer;
        }

        public override bool CheckCanTrigger()
        {
            if (ownerPlayer == duelScene.currentPlayer &&
                duelScene.GetCurrentDuelProcess() == DuelProcess.End &&
                !haveProcess)
            {
                return true;
            }
            return false;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;
            if (ownerPlayer.GetHandCards().Count > DuelRuleManager.GetHandCardNumberUpperLimit())
            {
                GameManager.ShowMessage("当前手牌数量大于规定数量！");
                int number = ownerPlayer.GetHandCards().Count - DuelRuleManager.GetHandCardNumberUpperLimit();
                DiscardHandCardEffectProcess discardHandCardEffectProcess = new DiscardHandCardEffectProcess(null,number, (launchCard,discardCard)=> { BeforeProcessFunction(); } , ownerPlayer);
                ownerPlayer.AddEffectProcess(discardHandCardEffectProcess);
            }
            else
            {
                AfterFinishProcessFunction();
            }
        }
    }
}
