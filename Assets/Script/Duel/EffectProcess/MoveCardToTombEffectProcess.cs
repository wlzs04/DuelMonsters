using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.EffectProcess
{
    /// <summary>
    /// 移动卡牌到墓地的方式
    /// </summary>
    public enum MoveCardToTombType
    {
        Battle,//攻击
        Effect,//效果
    }

    /// <summary>
    /// 移动卡牌到墓地，一般是由攻击或效果
    /// </summary>
    class MoveCardToTombEffectProcess : EffectProcessBase
    {
        CardBase card;
        MoveCardToTombType moveCardToTombType;

        public MoveCardToTombEffectProcess(CardBase card, MoveCardToTombType moveCardToTombType, Player ownerPlayer) : base(ownerPlayer, "移动卡牌到墓地")
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.card = card;
            this.moveCardToTombType = moveCardToTombType;
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
        {
            ownerPlayer.MoveCardToTomb(card);
            AfterFinishProcessFunction();
        }

        public MoveCardToTombType GetMoveCardToTombType()
        {
            return moveCardToTombType;
        }
    }
}
