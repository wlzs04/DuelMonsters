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
    enum PlayGameState
    {
        Unknown,
        Normal,
        AttackPrepare,//攻击准备
        ChooseSacrifice,//选择祭品
    }

    class Player
    {
        string name = "玩家";
        DuelCardGroup duelCardGroup;
        List<CardBase> handCards = new List<CardBase>();
        List<CardBase> tombCards = new List<CardBase>();
        List<CardBase> exceptCards = new List<CardBase>();
        
        List<MonsterCard> sacrificeCards = new List<MonsterCard>();

        CardBase[] monsterCardArea = new CardBase[DuelRule.monsterAreaNumber];
        CardBase[] magicTrapCardArea = new CardBase[DuelRule.monsterAreaNumber];

        DuelScene duelScene = null;
        GameObject handPanel = null;
        Scrollbar lifeScrollBar = null;
        int life = 4000;

        Player opponentPlayer = null;

        bool canBeDirectAttacked = true;
        bool canDirectAttack = true;

        Vector3 heartPosition;

        PlayGameState playGameState;

        MonsterCard needSacrificeMonster = null;

        public bool CanDirectAttack
        {
            get
            {
                return canDirectAttack;
            }

            set
            {
                canDirectAttack = value;
            }
        }

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

        public void SetHeartPosition(Vector3 heartPosition)
        {
            this.heartPosition = heartPosition;
        }

        public Vector3 GetHeartPosition()
        {
            return heartPosition;
        }

        public void SetOpponentPlayer(Player opponentPlayer)
        {
            this.opponentPlayer = opponentPlayer;
        }

        public Player GetOpponentPlayer()
        {
            return opponentPlayer;
        }

        /// <summary>
        /// 判断玩家是否有可以被攻击的怪兽
        /// </summary>
        /// <returns></returns>
        public bool HaveBeAttackedMonster()
        {
            foreach (var item in monsterCardArea)
            {
                if(item!=null&& ((MonsterCard)item).CanBeAttacked)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断玩家是否可以被直接攻击
        /// </summary>
        /// <returns></returns>
        public bool CanBeDirectAttacked()
        {
            return canBeDirectAttacked;
        }

        /// <summary>
        /// 设置玩家是否可以被直接攻击
        /// </summary>
        /// <param name="canBeDirectAttacked"></param>
        public void SetCanBeDirectAttacked(bool canBeDirectAttacked)
        {
            this.canBeDirectAttacked = canBeDirectAttacked;
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
        /// 获得我方场上可以被祭献的怪兽数量。
        /// </summary>
        /// <returns></returns>
        public int GetCanBeSacrificeMonsterNumber()
        {
            int canBeSacrificeMonsterNumber = 0;
            foreach (var item in monsterCardArea)
            {
                if(item!=null)
                {
                    canBeSacrificeMonsterNumber+=((MonsterCard)item).GetCanBeSacrificedNumber();
                }
            }

            return canBeSacrificeMonsterNumber;
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
            playGameState = PlayGameState.Normal;
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
            if (duelScene.GetCurrentTurnNumber()==1&&duelScene.startPlayer==this)
            {
                duelScene.ShowMessage("第一回合先攻者不能攻击！");
                return;
            }
            if (!IsMyTurn())
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
            lifeScrollBar.size =(float)this.life / DuelRule.startLife;
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
                    //先判断是否可以直接进行召唤
                    if (monsterCard.GetLevel() <= DuelRule.callMonsterWithoutSacrificeMaxLevel)
                    {
                        int index = 0;
                        for (; index < DuelRule.monsterAreaNumber; index++)
                        {
                            if (monsterCardArea[index] == null)
                            {
                                monsterCardArea[index] = monsterCard;
                                break;
                            }
                        }
                        monsterCard.AddContent("monsterCardAreaIndex", index);
                        monsterCard.SetCardGameState(CardGameState.FrontATK);
                        monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
                        monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1);
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
                    else//使用祭品召唤
                    {
                        int monsterLevel = monsterCard.GetLevel();
                        if(monsterLevel<=DuelRule.callMonsterWithoutOneSacrificeMaxLevel&&GetCanBeSacrificeMonsterNumber()>=1)
                        {
                            playGameState = PlayGameState.ChooseSacrifice;
                            needSacrificeMonster = monsterCard;
                        }
                        else if(GetCanBeSacrificeMonsterNumber() >= 2)
                        {
                            Debug.LogError("暂时不允许召唤需要使用超过两只祭品的怪兽！");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 使用祭品召唤怪兽
        /// </summary>
        /// <param name="monsterCard"></param>
        public void CallMonsterWithSacrifice(MonsterCard monsterCard)
        {
            if (IsMyPlayer())
            {
                foreach (var item in sacrificeCards)
                {
                    MoveCardToTomb(item);
                }
                int index = 0;
                for (; index < DuelRule.monsterAreaNumber; index++)
                {
                    if (monsterCardArea[index] == null)
                    {
                        monsterCardArea[index] = monsterCard;
                        break;
                    }
                }
                monsterCard.AddContent("monsterCardAreaIndex", index);
                monsterCard.SetCardGameState(CardGameState.FrontATK);
                monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
                monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1);
                handCards.Remove(monsterCard);
                normalCallNumber--;

                playGameState = PlayGameState.Normal;

                CCallMonsterBySacrifice cCallMonsterBySacrifice = new CCallMonsterBySacrifice();
                cCallMonsterBySacrifice.AddContent("cardID", monsterCard.GetID());
                cCallMonsterBySacrifice.AddContent("callType", CallType.Normal);
                cCallMonsterBySacrifice.AddContent("fromCardGameState", CardGameState.Hand);
                cCallMonsterBySacrifice.AddContent("toCardGameState", CardGameState.FrontATK);
                cCallMonsterBySacrifice.AddContent("flag", index);

                StringBuilder sacrificeInfo = new StringBuilder();

                foreach (var item in sacrificeCards)
                {
                    sacrificeInfo.Append(item.GetID());
                }

                cCallMonsterBySacrifice.AddContent("sacrificeInfo", sacrificeInfo.ToString());
                ClientManager.GetSingleInstance().SendProtocol(cCallMonsterBySacrifice);

                sacrificeCards.Clear();
            }
        }

        /// <summary>
        /// 获得玩家在游戏中的状态
        /// </summary>
        /// <returns></returns>
        public PlayGameState GetPlayGameState()
        {
            return playGameState;
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
        /// 由协议调用召唤怪兽,最后一项祭品列表为空代表直接召唤，否则为祭品召唤
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="callType"></param>
        /// <param name="fromCardGameState"></param>
        /// <param name="toCardGameState"></param>
        /// <param name="flag"></param>
        /// <param name="sacrificeList"></param>
        public void CallMonster(int cardID, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag, string sacrificeinfo = null)
        {
            if(fromCardGameState!=CardGameState.Hand)
            {
                Debug.LogError("非手卡召唤！");
                return;
            }

            if(sacrificeinfo != null)
            {
                string[] sacrificeIDs = sacrificeinfo.Split(':');

                foreach (var item in sacrificeIDs)
                {
                    MoveCardToTomb(GetCardByID(int.Parse(item)));
                }
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
            monsterCard.AddContent("monsterCardAreaIndex", index);
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
                    monsterCardArea[int.Parse(card.GetContent("monsterCardAreaIndex").ToString())]=null;
                    card.ClearAllContent();
                    if (IsMyPlayer())
                    {
                        card.cardObject.transform.localPosition = new Vector3(DuelCommonValue.myTombPositionX, DuelCommonValue.myTombPositionY, 0);
                    }
                    else
                    {
                        card.cardObject.transform.localPosition = new Vector3(DuelCommonValue.opponentTombPositionX, DuelCommonValue.opponentTombPositionY, 0);
                    }
                    tombCards.Add(card);
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
        
        /// <summary>
        /// 选择怪兽作为祭品
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool ChooseMonsterAsSacrifice(MonsterCard card)
        {
            if(!sacrificeCards.Contains(card))
            {
                sacrificeCards.Add(card);
            }
            else
            {
                Debug.LogError("ChooseMonsterAsSacrifice 已经选中了此怪兽:"+card.GetName());
            }
            if(sacrificeCards.Count>=needSacrificeMonster.GetCanBeSacrificedNumber())
            {
                CallMonsterWithSacrifice(needSacrificeMonster);
            }
            return true;
        }
    }
}
