using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using Assets.Script.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Duel
{
    class Player
    {
        string name="玩家";
        DuelCardGroup duelCardGroup;
        List<CardBase> handCards = new List<CardBase>();
        List<CardBase> tombCards = new List<CardBase>();
        List<CardBase> exceptCards = new List<CardBase>();

        CardBase[] monsterCardArea = new CardBase[DuelRule.monsterAreaNumber];
        CardBase[] magicTrapCardArea = new CardBase[DuelRule.monsterAreaNumber];

        DuelScene duelScene = null;
        GameObject handPanel = null;
        int lift = 4000;
        int normalCallNumber = DuelRule.drawCardNumberEveryTurn;

        public Player(string name,DuelCardGroup duelCardGroup)
        {
            this.name = name;
            this.duelCardGroup = duelCardGroup;
            duelScene = GameManager.GetSingleInstance().GetDuelScene();
        }

        public Player(DuelCardGroup duelCardGroup):this("玩家", duelCardGroup)
        {

        }

        public void SetHandPanel(GameObject handPanel)
        {
            this.handPanel = handPanel;
        }

        public DuelCardGroup GetDuelCardGroup()
        {
            return duelCardGroup;
        }

        public void ShuffleCardGroup()
        {
            duelCardGroup.ShuffleCardGroup();
        }

        /// <summary>
        /// 投降
        /// </summary>
        public void Surrender()
        {
            
        }

        /// <summary>
        /// 抽卡
        /// </summary>
        public void Draw()
        {
            CardBase card = GetDuelCardGroup().GetCards()[0];
            card.cardObject.transform.SetParent(handPanel.transform);
            ((RectTransform)(card.cardObject.transform)).sizeDelta=new Vector2(DuelCommonValue.cardOnHandWidth, DuelCommonValue.cardOnHandHeight);
            duelCardGroup.GetCards().RemoveAt(0);
            handCards.Add(card);
            card.SetCardGameState(CardGameState.Hand);

            if (this==duelScene.myPlayer)
            {
                card.cardObject.gameObject.GetComponent<Image>().sprite = card.GetImage();

                CDrawCard cDrawCard = new CDrawCard();
                ClientManager.GetInstance().SendProtocol(cDrawCard);
            }
        }

        /// <summary>
        /// 选择流程
        /// </summary>
        public void ChooseProcess()
        {

        }

        /// <summary>
        /// 开始回合
        /// </summary>
        public void StartTurn()
        {
            normalCallNumber = DuelRule.drawCardNumberEveryTurn;
            duelScene.EnterNextDuelProcess();
            Draw();
            duelScene.EnterNextDuelProcess();
            duelScene.EnterNextDuelProcess();
        }
        
        /// <summary>
        /// 结束回合
        /// </summary>
        public void EndTurn()
        {

        }

        /// <summary>
        /// 战斗
        /// </summary>
        public void Battle()
        {

        }

        /// <summary>
        /// 召唤怪兽到场上
        /// </summary>
        /// <param name="monsterCard"></param>
        public void CallMonster(MonsterCard monsterCard)
        {
            //检测召唤条件是否满足
            if (normalCallNumber > 0)
            {
                int index = 0;
                if ( monsterCard.GetLevel() <= DuelRule.callMonsterWithoutSacrificeMaxLevel)
                {
                    for (; index < DuelRule.monsterAreaNumber; index++)
                    {
                        if(monsterCardArea[index] ==null)
                        {
                            monsterCardArea[index] = monsterCard;
                            break;
                        }
                    }
                    monsterCard.SetCardGameState(CardGameState.FrontATK);
                    monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
                    monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX+index* DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY,1);
                    ((RectTransform)(monsterCard.cardObject.transform)).sizeDelta = new Vector2(DuelCommonValue.cardOnBackWidth, DuelCommonValue.cardOnBackHeight);
                    handCards.Remove(monsterCard);
                    normalCallNumber--;


                }
            }
        }

        public bool IsMyTurn()
        {
            return this == duelScene.currentPlayer;
        }
    }
}
