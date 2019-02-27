using Assets.Script.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XLua;

namespace Assets.Script.Duel.EffectProcess
{
    [CSharpCallLua]
    public delegate bool ChooseCardJudgeAction(CardBase launchCardBase, CardBase chooseCard);

    /// <summary>
    /// 选取卡牌对象
    /// </summary>
    class ChooseCardEffectProcess : EffectProcessBase
    {
        CardBase launchEffectCard;
        ChooseCardJudgeAction chooseCardJudgeAction;
        Action<CardBase, CardBase> chooseCardCallBack;
        List<CardBase> canChooseCardBases=new List<CardBase>();

        public ChooseCardEffectProcess(CardBase launchEffectCard, ChooseCardJudgeAction chooseCardJudgeAction, Action<CardBase, CardBase> chooseCardCallBack, Player ownerPlayer) : base(ownerPlayer, "选取卡牌对象")
        {
            effectProcessType = EffectProcessType.RemoveAfterFinish;
            this.launchEffectCard = launchEffectCard;
            this.chooseCardJudgeAction = chooseCardJudgeAction;
            this.chooseCardCallBack = chooseCardCallBack;

            finishAction += duelScene.UnlockScene;
            finishAction += () =>
            {
                 RemoveChooseCardCallBackFromCardList();
                 duelScene.SetTitle("");
            };
        }

        public override bool CheckCanTrigger()
        {
            return true;
        }

        protected override void BeforeProcessFunction()
        {
            duelScene.LockScene();
            //从我方魔法陷阱区进行选择
            CardBase[] myMagicTrapCardArea = ownerPlayer.GetMagicTrapCardArea();
            AddCanChooseCardFromList(myMagicTrapCardArea);

            //从对方魔法陷阱区进行选择
            CardBase[] opponentMagicTrapCardArea = ownerPlayer.GetOpponentPlayer().GetMagicTrapCardArea();
            AddCanChooseCardFromList(opponentMagicTrapCardArea);

            //从我方怪兽区进行选择
            CardBase[] myMonsterCardArea = ownerPlayer.GetMonsterCardArea();
            AddCanChooseCardFromList(myMonsterCardArea);

            //从对方怪兽区进行选择
            CardBase[] opponentMonsterCardArea = ownerPlayer.GetOpponentPlayer().GetMonsterCardArea();
            AddCanChooseCardFromList(opponentMonsterCardArea);

            AddChooseCardCallBackToCardList();
        }

        /// <summary>
        /// 从传入的卡牌列表中选出可以进行选择的卡牌
        /// </summary>
        /// <param name="cardBases"></param>
        void AddCanChooseCardFromList(CardBase[] cardBases)
        {
            foreach (var item in cardBases)
            {
                if (item != null)
                {
                    if (chooseCardJudgeAction(launchEffectCard, item))
                    {
                        canChooseCardBases.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 向可选卡片添加点击回调事件
        /// </summary>
        public void AddChooseCardCallBackToCardList()
        {
            foreach (var item in canChooseCardBases)
            {
                
                item.GetDuelCardScript().SetClickCallback(launchEffectCard, ChooseCardCallBack);
            }
        }

        /// <summary>
        /// 选择卡牌的回调
        /// </summary>
        /// <param name="launchEffectCard"></param>
        /// <param name="chooseCard"></param>
        void ChooseCardCallBack(CardBase launchEffectCard, CardBase chooseCard)
        {
            AfterFinishProcessFunction();
            chooseCardCallBack(launchEffectCard, chooseCard);
        }

        /// <summary>
        /// 移除可选卡片的点击回调事件
        /// </summary>
        public void RemoveChooseCardCallBackFromCardList()
        {
            foreach (var item in canChooseCardBases)
            {
                item.GetDuelCardScript().RemoveClickCallback();
            }
        }

        /// <summary>
        /// 设置在选择卡牌时显示的标题
        /// </summary>
        /// <param name="titleText"></param>
        public void SetTitle(string titleText)
        {
            duelScene.SetTitle(titleText);
        }
    }
}