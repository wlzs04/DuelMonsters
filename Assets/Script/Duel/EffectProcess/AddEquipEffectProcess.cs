using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 添加装备
    /// </summary>
    class AddEquipEffectProcess : EffectProcessBase
    {
        CardBase equipCard;
        CardBase monsterCard;

        public AddEquipEffectProcess(CardBase monsterCard ,CardBase equipCard, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.monsterCard = monsterCard;
            this.equipCard = equipCard;
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
        {
            monsterCard.AddEquip(equipCard);
            AfterFinishProcessFunction();
        }
    }
}
