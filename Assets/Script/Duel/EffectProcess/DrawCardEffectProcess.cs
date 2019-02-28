using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 抽卡类型
    /// </summary>
    public enum DrawCardType
    {
        EveryTurn,//每到自己回合的抽卡阶段的抽卡
        Effect,//特殊效果
    }

    /// <summary>
    /// 抽卡，每回合在抽卡流程进行抽卡，当卡组中卡牌数量为0时，玩家败北
    /// 特殊效果抽卡时，当卡组中卡牌数量为0时，玩家不进行抽卡
    /// </summary>
    class DrawCardEffectProcess : EffectProcessBase
    {
        int drawCardNumber = 0;
        DrawCardType drawCardType = DrawCardType.EveryTurn;

        public DrawCardEffectProcess(int drawCardNumber, DrawCardType drawCardType,Player ownerPlayer) : base(ownerPlayer, "抽卡")
        {
            this.drawCardNumber = drawCardNumber;
            this.drawCardType = drawCardType;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;
            //当卡组中卡牌数量为0时，玩家败北
            if (drawCardType == DrawCardType.EveryTurn)
            {
                if (ownerPlayer.GetDuelCardGroup().GetCards().Count <= 0)
                {
                    duelScene.SetWinner(ownerPlayer.GetOpponentPlayer(), DuelEndReason.Draw);
                    return;
                }
            }
            for (int i = 0; i < drawCardNumber; i++)
            {
                if (ownerPlayer.GetDuelCardGroup().GetCards().Count > 0)
                {
                    ownerPlayer.Draw();
                }
                else
                {
                    break;
                }
            }
            AfterFinishProcessFunction();
        }
    }
}
