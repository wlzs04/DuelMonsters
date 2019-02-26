using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 由于特殊效果抽卡，当卡组中卡牌数量为0时，玩家不进行抽卡
    /// </summary>
    class DrawCardEffectProcess : EffectProcessBase
    {
        int drawCardNumber = 0;

        public DrawCardEffectProcess(int drawCardNumber,Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.drawCardNumber = drawCardNumber;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;
            for (int i = 0; i < drawCardNumber; i++)
            {
                if(ownerPlayer.GetDuelCardGroup().GetCards().Count>0)
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
