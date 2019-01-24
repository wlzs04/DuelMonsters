using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 召唤怪兽
    /// </summary>
    class CallMonsterEffectProcess : EffectProcessBase
    {
        public CallMonsterEffectProcess(Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
        }

        public override bool CheckCanTrigger()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessFunction()
        {
            throw new NotImplementedException();
        }
    }
}
