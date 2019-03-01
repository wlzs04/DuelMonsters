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

        public AddEquipEffectProcess(CardBase monsterCard ,CardBase equipCard, Player ownerPlayer) : base(ownerPlayer, "添加装备")
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.monsterCard = monsterCard;
            this.equipCard = equipCard;
            launchEffectCard = equipCard;//有问题，添加装备效果不一定是由装备卡发动的，暂时这样写
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
