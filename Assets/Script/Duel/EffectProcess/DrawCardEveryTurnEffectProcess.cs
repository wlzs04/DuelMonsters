using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 每回合在抽卡流程进行抽卡，当卡组中卡牌数量为0时，玩家败北
    /// </summary>
    class DrawCardEveryTurnEffectProcess : EffectProcessBase
    {
        public DrawCardEveryTurnEffectProcess(Player ownerPlayer) : base(ownerPlayer, "抽卡流程抽卡")
        {
            effectProcessType = EffectProcessType.Forever;
        }

        public override bool CheckCanTrigger()
        {
            if (ownerPlayer == duelScene.currentPlayer &&
                duelScene.GetCurrentDuelProcess() == DuelProcess.Draw &&
                !haveProcess)
            {
                return true;
            }
            return false;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;

            //当卡组中卡牌数量为0时，玩家败北
            if (ownerPlayer.GetDuelCardGroup().GetCards().Count <= 0)
            {
                duelScene.SetWinner(ownerPlayer.GetOpponentPlayer(), DuelEndReason.Draw);
                return;
            }

            ownerPlayer.Draw();
            
            AfterFinishProcessFunction();
        }
    }
}
