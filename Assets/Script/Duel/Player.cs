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
        Scrollbar lifeScrollBar = null;
        int life = 4000;


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

        public void SetLife(Scrollbar lifeScrollBar)
        {
            life = DuelRule.startLife;
            this.lifeScrollBar = lifeScrollBar;
        }

        public void SetHandPanel(GameObject handPanel)
        {
            this.handPanel = handPanel;
        }

        /// <summary>
        /// 获得牌组
        /// </summary>
        /// <returns></returns>
        public DuelCardGroup GetDuelCardGroup()
        {
            return duelCardGroup;
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        public void ShuffleCardGroup()
        {
            duelCardGroup.ShuffleCardGroup();
        }

        /// <summary>
        /// 判断可否召唤怪兽
        /// </summary>
        /// <returns></returns>
        public bool CanCallMonster()
        {
            return (duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second)&&normalCallNumber>0;
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
            //((RectTransform)(card.cardObject.transform)).sizeDelta=new Vector2(DuelCommonValue.cardOnHandWidth, DuelCommonValue.cardOnHandHeight);
            duelCardGroup.GetCards().RemoveAt(0);
            handCards.Add(card);
            card.SetCardGameState(CardGameState.Hand);

            if (this==duelScene.myPlayer)
            {
                card.cardObject.gameObject.GetComponent<Image>().sprite = card.GetImage();
                card.cardObject.gameObject.GetComponent<DuelCardScript>().SetCanShowInfo(true);
                CDrawCard cDrawCard = new CDrawCard();
                ClientManager.GetSingleInstance().SendProtocol(cDrawCard);
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
            duelScene.EnterNextDuelProcess();
            normalCallNumber = DuelRule.drawCardNumberEveryTurn;
            Draw();
            duelScene.EnterNextDuelProcess();
            duelScene.EnterNextDuelProcess();
        }
        
        /// <summary>
        /// 结束回合
        /// </summary>
        public void EndTurn()
        {
            if (!IsMyTurn())
            {
                duelScene.ShowMessage("不是你的回合！");
                return;
            }
            duelScene.EndTurn();
        }

        /// <summary>
        /// 战斗
        /// </summary>
        public void Battle()
        {
            if(!IsMyTurn())
            {
                duelScene.ShowMessage("不是你的回合！");
                return;
            }
            if(duelScene.currentDuelProcess != DuelProcess.Main)
            {
                duelScene.ShowMessage("只有主要流程才可以进入战斗！当前流程为："+ duelScene.currentDuelProcess.ToString());
                return;
            }
            duelScene.Battle();
            foreach (var item in monsterCardArea)
            {
                if(item!=null)
                {
                    item.cardObject.GetComponent<DuelCardScript>().SetATKNumber();
                }
            }
        }

        /// <summary>
        /// 减血
        /// </summary>
        /// <param name="life"></param>
        public void ReduceLife(int life)
        {
            this.life -= life;
            lifeScrollBar.value = this.life / DuelRule.startLife;
            duelScene.CheckWin();
        }

        /// <summary>
        /// 召唤怪兽到场上
        /// </summary>
        /// <param name="monsterCard"></param>
        public void CallMonster(MonsterCard monsterCard)
        {
            if(IsMyPlayer())
            {
                //检测召唤条件是否满足
                if (normalCallNumber > 0)
                {
                    int index = 0;
                    if (monsterCard.GetLevel() <= DuelRule.callMonsterWithoutSacrificeMaxLevel)
                    {
                        for (; index < DuelRule.monsterAreaNumber; index++)
                        {
                            if (monsterCardArea[index] == null)
                            {
                                monsterCardArea[index] = monsterCard;
                                break;
                            }
                        }
                        monsterCard.SetCardGameState(CardGameState.FrontATK);
                        monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
                        monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1);
                        //((RectTransform)(monsterCard.cardObject.transform)).sizeDelta = new Vector2(DuelCommonValue.cardOnBackWidth, DuelCommonValue.cardOnBackHeight);
                        handCards.Remove(monsterCard);
                        normalCallNumber--;

                        CCallMonster cCallMonster = new CCallMonster();
                        cCallMonster.AddContent("cardID", monsterCard.GetID());
                        cCallMonster.AddContent("callType", CallType.Normal);
                        cCallMonster.AddContent("fromCardGameState", CardGameState.Hand);
                        cCallMonster.AddContent("toCardGameState", CardGameState.FrontATK);
                        cCallMonster.AddContent("flag", index);
                        ClientManager.GetSingleInstance().SendProtocol(cCallMonster);
                    }
                }
            }
        }

        /// <summary>
        /// 判断是否为我方回合
        /// </summary>
        /// <returns></returns>
        public bool IsMyTurn()
        {
            return this == duelScene.currentPlayer;
        }

        /// <summary>
        /// 判断此玩家是否为方玩家 
        /// </summary>
        /// <returns></returns>
        public bool IsMyPlayer()
        {
            return this == duelScene.myPlayer;
        }

        /// <summary>
        /// 由协议调用召唤怪兽
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="callType"></param>
        /// <param name="fromCardGameState"></param>
        /// <param name="toCardGameState"></param>
        /// <param name="flag"></param>
        public void CallMonster(int cardID, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag)
        {
            if(fromCardGameState!=CardGameState.Hand)
            {
                Debug.LogError("非手卡召唤！");
                return;
            }
            MonsterCard monsterCard = null;
            foreach (var item in handCards)
            {
                if(item.GetID()==cardID)
                {
                    monsterCard = (MonsterCard)item;
                    break;
                }
            }

            int index = DuelRule.monsterAreaNumber - flag - 1;

            monsterCardArea[index] = monsterCard;

            monsterCard.cardObject.gameObject.GetComponent<DuelCardScript>().SetCanShowInfo(true);
            monsterCard.SetCardGameState(toCardGameState);
            monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
            monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.opponentMonsterCardPositionY, 1);
            monsterCard.cardObject.transform.localRotation = Quaternion.Euler(0,0,180);
            monsterCard.cardObject.GetComponent<Image>().sprite = monsterCard.GetImage();
            //((RectTransform)(monsterCard.cardObject.transform)).sizeDelta = new Vector2(DuelCommonValue.cardOnBackWidth, DuelCommonValue.cardOnBackHeight);
            handCards.Remove(monsterCard);
            normalCallNumber--;
        }

        /// <summary>
        /// 通过ID获得卡牌
        /// </summary>
        /// <param name="cardID"></param>
        /// <returns></returns>
        public CardBase GetCardByID(int cardID)
        {
            foreach (var item in monsterCardArea)
            {
                if(item!=null&& item.GetID()==cardID)
                {
                    return item;
                }
            }
            foreach (var item in magicTrapCardArea)
            {
                if (item != null && item.GetID() == cardID)
                {
                    return item;
                }
            }
            foreach (var item in handCards)
            {
                if (item != null && item.GetID() == cardID)
                {
                    return item;
                }
            }
            foreach (var item in tombCards)
            {
                if (item != null && item.GetID() == cardID)
                {
                    return item;
                }
            }
            foreach (var item in exceptCards)
            {
                if (item != null && item.GetID() == cardID)
                {
                    return item;
                }
            }
            foreach (var item in duelCardGroup.GetCards())
            {
                if (item != null && item.GetID() == cardID)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得生命值
        /// </summary>
        /// <returns></returns>
        public int GetLife()
        {
            return life;
        }

        /// <summary>
        /// 将卡牌送入墓地
        /// </summary>
        /// <param name="card"></param>
        public void MoveCardToTomb(CardBase card)
        {
            switch (card.GetCardGameState())
            {
                case CardGameState.Group:
                    break;
                case CardGameState.Hand:
                    break;
                case CardGameState.FrontATK:
                    if(IsMyPlayer())
                    {
                        card.cardObject.transform.localPosition = new Vector3(DuelCommonValue.myTombPositionX, DuelCommonValue.myTombPositionY, 0);
                    }
                    else
                    {
                        card.cardObject.transform.localPosition = new Vector3(DuelCommonValue.opponentTombPositionX, DuelCommonValue.opponentTombPositionY, 0);
                    }
                    break;
                case CardGameState.FrontDEF:
                    break;
                case CardGameState.Back:
                    break;
                case CardGameState.Tomb:
                    break;
                case CardGameState.Exclusion:
                    break;
                default:
                    break;
            }

            card.SetCardGameState(CardGameState.Tomb);
        }
    }
}
