using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 转换攻防
    /// </summary>
    class ChangeAttackDefenseEffectProcess : EffectProcessBase
    {
        MonsterCard monsterCard;
        CardGameState cardGameState;

        public ChangeAttackDefenseEffectProcess(MonsterCard monsterCard,CardGameState cardGameState,Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.monsterCard = monsterCard;
            this.cardGameState = cardGameState;
        }

        public override bool CheckCanTrigger()
        {
            throw new NotImplementedException();
        }

        protected override void ProcessFunction()
        {
            monsterCard.SetCardGameState(cardGameState);
            AfterFinishProcessFunction();
        }
    }
}
