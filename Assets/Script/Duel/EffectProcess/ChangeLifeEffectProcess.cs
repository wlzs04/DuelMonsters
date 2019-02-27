using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 修改生命值的方式
    /// </summary>
    public enum ChangeLifeType
    {
        Battle,//战斗
        Effect,//效果
    }

    /// <summary>
    /// 修改生命值
    /// </summary>
    public class ChangeLifeEffectProcess : EffectProcessBase
    {
        int changeLifeValue = 0;
        ChangeLifeType changeLifeType;

        public ChangeLifeEffectProcess(int changeLifeValue, ChangeLifeType changeLifeType, Player ownerPlayer) : base(ownerPlayer, "减少生命值")
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.changeLifeValue = changeLifeValue;
            this.changeLifeType = changeLifeType;
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
        {
            ownerPlayer.ReduceLife(changeLifeValue);
            AfterFinishProcessFunction();
        }

        public ChangeLifeType GetChangeLifeType()
        {
            return changeLifeType;
        }
    }
}
