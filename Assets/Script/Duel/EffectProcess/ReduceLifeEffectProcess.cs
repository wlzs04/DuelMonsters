using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 减少生命值的方式
    /// </summary>
    public enum ReduceLifeType
    {
        Battle,//攻击
        Effect,//效果
    }

    /// <summary>
    /// 减少生命值
    /// </summary>
    class ReduceLifeEffectProcess : EffectProcessBase
    {
        int reduceLife = 0;
        ReduceLifeType reduceLifeType;

        public ReduceLifeEffectProcess(int reduceLife, ReduceLifeType reduceLifeType, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.reduceLife = reduceLife;
            this.reduceLifeType = reduceLifeType;
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void ProcessFunction()
        {
            ownerPlayer.ReduceLife(reduceLife);
            AfterFinishProcessFunction();
        }
    }
}
