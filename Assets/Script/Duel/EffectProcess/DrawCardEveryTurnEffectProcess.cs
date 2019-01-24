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
        public DrawCardEveryTurnEffectProcess(Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.Forever;
        }

        public override bool CheckCanTrigger()
        {
            if (ownerPlayer == duelScene.currentPlayer &&
                duelScene.currentDuelProcess == DuelProcess.Draw &&
                !haveProcess)
            {
                return true;
            }
            return false;
        }

        protected override void ProcessFunction()
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
