using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

namespace Assets.Script.Duel.EffectProcess
{
    [CSharpCallLua]
    public delegate void ActionTossCoin(CardBase cardBase, CoinType selectCoinType, CoinType resultCoinType);

    /// <summary>
    /// 硬币类型
    /// </summary>
    public enum CoinType
    {
        Unknown,//未知
        Front,//正面
        Back,//反面
    }

    /// <summary>
    /// 抛硬币
    /// </summary>
    class TossCoinEffectProcess : EffectProcessBase
    {
        CardBase launchEffectCard;
        bool showSelectCoinPanel;
        ActionTossCoin tossCoinCallBack;
        CoinType selectCoinType;

        public TossCoinEffectProcess(CardBase launchEffectCard,bool showSelectCoinPanel, ActionTossCoin tossCoinCallBack,CoinType selectCoinType, Player ownerPlayer) : base(ownerPlayer)
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;

            this.launchEffectCard = launchEffectCard;
            this.showSelectCoinPanel = showSelectCoinPanel;
            this.tossCoinCallBack = tossCoinCallBack;
            this.selectCoinType = selectCoinType;
        }

        public override bool CheckCanTrigger()
        {
            return !haveProcess;
        }

        protected override void ProcessFunction()
        {
            launchEffectCard.GetDuelCardScript().GetDuelScene().ShowTossCoinPanel(showSelectCoinPanel,(coinType, resultCoinType) => { tossCoinCallBack(launchEffectCard, coinType,resultCoinType); }, selectCoinType);
        }
    }
}
