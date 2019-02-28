using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 选择项
    /// </summary>
    class SelectItemEffectProcess : EffectProcessBase
    {
        CardBase launchEffectCard;
        Type type;
        ActionIndex selectItemCallBack;

        public SelectItemEffectProcess(CardBase launchEffectCard,Type type ,ActionIndex selectItemCallBack, Player ownerPlayer) : base(ownerPlayer, "选择项")
        {
            this.launchEffectCard = launchEffectCard;
            this.type = type;
            this.selectItemCallBack = selectItemCallBack;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void BeforeProcessFunction()
        {
            haveProcess = true;
            launchEffectCard.GetDuelCardScript().GetDuelScene().ShowSelectItemPanel(type,SelectItemCallBack);
        }

        void SelectItemCallBack(int resultNumber)
        {
            AfterFinishProcessFunction();
            selectItemCallBack(launchEffectCard, resultNumber);
        }
    }
}
