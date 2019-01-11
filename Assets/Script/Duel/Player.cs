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
    public enum PlayGameState
    {
        Unknown,
        Normal,
        AttackPrepare,//攻击准备
        ChooseSacrifice,//选择祭品
    }

    public class Player
    {
        string name = "玩家";
        protected DuelCardGroup duelCardGroup;
        protected List<CardBase> handCards = new List<CardBase>();
        protected List<CardBase> tombCards = new List<CardBase>();
        protected List<CardBase> exceptCards = new List<CardBase>();
        
        List<MonsterCard> sacrificeCards = new List<MonsterCard>();

        public CardBase[] monsterCardArea = new CardBase[DuelRule.monsterAreaNumber];
        protected CardBase[] magicTrapCardArea = new CardBase[DuelRule.monsterAreaNumber];

        protected DuelScene duelScene = null;
        GameObject handPanel = null;
        Scrollbar lifeScrollBar = null;
        int life = 4000;

        Player opponentPlayer = null;

        bool canBeDirectAttacked = true;
        bool canDirectAttack = true;

        Vector3 heartPosition;

        PlayGameState playGameState;

        MonsterCard needSacrificeCallMonster = null;

        GuessEnum guessEnum = GuessEnum.Unknown;

        bool iamReady = false;//判断玩家是否准备完成

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

        public Player(string name, DuelScene duelScene)
        {
            if(this.name == "")
            {
                this.name = "玩家";
            }
            else
            {
                this.name = name;
            }

            this.duelScene = duelScene;
        }

        public virtual void InitCardGroup()
        {
            List<CardBase> myCards = duelCardGroup.GetCards();
            for (int i = 0; i < myCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(myCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                myCards[i].cardObject = go;
                go.transform.SetParent(duelScene.duelBackImage.transform);
                go.transform.localPosition = new Vector3(DuelCommonValue.myCardGroupPositionX, DuelCommonValue.myCardGroupPositionY, 0);
            }
        }

        public bool IAmReady()
        {
            return iamReady;
        }

        public GuessEnum GetGuessEnum()
        {
            return guessEnum;
        }
        
        public bool SetGuessEnum(GuessEnum guessEnum)
        {
            if(this.guessEnum == GuessEnum.Unknown || guessEnum == GuessEnum.Unknown)
            {
                this.guessEnum = guessEnum;
                return true;
            }
            return false;
        }
        
        public virtual void SetCardGroup()
        {
            if(duelCardGroup!=null)
            {
                return;
            }
            duelCardGroup = new DuelCardGroup();

            UserCardGroup firstCardGroup = GameManager.GetSingleInstance().GetUserData().userCardGroupList[0];
            Debug.LogWarning("我方暂时使用第一个卡组。");

            foreach (var item in firstCardGroup.mainCardList)
            {
                for (int i = 0; i < item.number; i++)
                {
                    duelCardGroup.AddCard(item.cardNo);
                }
            }
            ShuffleCardGroup();

            opponentPlayer.SetCardGroupNotify(duelCardGroup);
            iamReady = true;
            duelScene.CheckPlayInit();
        }

        /// <summary>
        /// 一般由协议调用，为对手设置卡组
        /// </summary>
        /// <param name="cardGroupInfo"></param>
        public void SetCardGroup(string cardGroupInfo)
        {
            duelCardGroup = new DuelCardGroup();
            string[] cardNos = cardGroupInfo.Split(':');
            foreach (var item in cardNos)
            {
                duelCardGroup.AddCard(int.Parse(item.Substring(0, item.IndexOf('-'))), int.Parse(item.Substring(item.IndexOf('-') + 1)));
            }

            opponentPlayer.SetCardGroupNotify(duelCardGroup);
            iamReady = true;
            duelScene.CheckPlayInit();
        }

        /// <summary>
        /// 设置卡组时的提醒
        /// </summary>
        /// <param name="duelCardGroup"></param>
        public virtual void SetCardGroupNotify(DuelCardGroup duelCardGroup)
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
        /// 判断玩家是否可以进入战斗流程
        /// </summary>
        /// <returns></returns>
        public bool CanBattle()
        {
            if(duelScene.currentDuelProcess!=DuelProcess.Battle)
            {
                return false;
            }
            foreach (var item in monsterCardArea)
            {
                if(item!=null)
                {
                    DuelCardScript duelCardScript = item.cardObject.GetComponent<DuelCardScript>();
                    if (duelCardScript.CanAttack())
                    {
                        return true;
                    }
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
            bool duelProcessCheck = IsMyTurn() && duelScene.currentDuelProcess == DuelProcess.Main ||
                duelScene.currentDuelProcess == DuelProcess.Second;

            bool callNumberCheck = normalCallNumber > 0;

            bool canCall = false;
            foreach (var item in handCards)
            {
                if(item.cardObject.GetComponent<DuelCardScript>().CanCall())
                {
                    canCall = true;
                    break;
                }
            }

            return duelProcessCheck&& callNumberCheck&&(canCall);
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
            opponentPlayer.SurrenderNotify();
            duelScene.SetWinner(opponentPlayer);
        }

        /// <summary>
        /// 决斗开始时抽卡
        /// </summary>
        public void DrawAtFirst()
        {
            for (int i = 0; i < DuelRule.drawCardNumberOnFirstDraw; i++)
            {
                Draw();
            }
        }

        /// <summary>
        /// 抽卡
        /// </summary>
        public virtual void Draw()
        {
            CardBase card = GetDuelCardGroup().GetCards()[0];
            card.cardObject.transform.SetParent(handPanel.transform);
            duelCardGroup.GetCards().RemoveAt(0);
            handCards.Add(card);
            card.SetCardGameState(CardGameState.Hand);
            

            if (this==duelScene.myPlayer)
            {
                card.cardObject.gameObject.GetComponent<DuelCardScript>().ShowFront();
                card.cardObject.gameObject.GetComponent<DuelCardScript>().SetCanShowInfo(true);
            }

            //opponentPlayer.DrawNotify();
        }

        /// <summary>
        /// 抽卡通知，用于让对方知道，同步网络信息和抽卡事件。
        /// </summary>
        public virtual void DrawNotify()
        {

        }

        /// <summary>
        /// 进入流程通知，
        /// </summary>
        public virtual void EnterDuelNotify(DuelProcess duelProcess)
        {

        }

        /// <summary>
        /// 进入流程通知，
        /// </summary>
        public virtual void EndTurnNotify()
        {

        }

        /// <summary>
        /// 怪兽受到怪兽攻击通知
        /// </summary>
        public virtual void BeAttackedMonsterNotify(int attackCardId,int beAttackedCardId)
        {

        }

        /// <summary>
        /// 受到怪兽直接攻击通知
        /// </summary>
        public virtual void BeDirectAttackedNotify(int attackCardId)
        {

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
            playGameState = PlayGameState.Normal;
            duelScene.EnterNextDuelProcess();
            normalCallNumber = DuelRule.drawCardNumberEveryTurn;
            Draw();
            duelScene.EnterNextDuelProcess();
            duelScene.EnterNextDuelProcess();
        }

        /// <summary>
        /// 思考行动
        /// </summary>
        public virtual void ThinkAction()
        {

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
        /// 进入第二主要流程
        /// </summary>
        public void Second()
        {
            if (!IsMyTurn())
            {
                duelScene.ShowMessage("不是你的回合！");
                return;
            }
            duelScene.EnterDuelProcess(DuelProcess.Second);
        }

        /// <summary>
        /// 进入战斗
        /// </summary>
        public void Battle()
        {
            if (!IsMyTurn())
            {
                duelScene.ShowMessage("不是你的回合！");
                return;
            }
            if (duelScene.GetCurrentTurnNumber()==1&&duelScene.startPlayer==this)
            {
                duelScene.ShowMessage("第一回合先攻者不能攻击！");
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
                    item.cardObject.GetComponent<DuelCardScript>().SetAttackNumber();
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
            duelScene.CheckWinByLife();
        }

        /// <summary>
        /// 获得可以进行召唤的次数，一般指通常召唤和祭品召唤
        /// </summary>
        /// <returns></returns>
        public int GetCanCallNumber()
        {
            return normalCallNumber;
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

                        CallMonsterNotify(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, CardGameState.FrontATK, index);
                        playGameState = PlayGameState.Normal;
                    }
                    else//使用祭品召唤
                    {
                        int monsterLevel = monsterCard.GetLevel();
                        if(GetCanBeSacrificeMonsterNumber()>=1)
                        {
                            playGameState = PlayGameState.ChooseSacrifice;
                            needSacrificeCallMonster = monsterCard;
                        }
                    }
                }
            }
        }

        public virtual void CallMonsterNotify(int id,CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState,int flag)
        {

        }

        /// <summary>
        /// 背面放置到场上
        /// </summary>
        public void BackPlaceMonster(MonsterCard monsterCard)
        {
            if (IsMyPlayer())
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
                        monsterCard.SetCardGameState(CardGameState.Back);
                        monsterCard.cardObject.GetComponent<DuelCardScript>().ShowBack();
                        monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
                        monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1);
                        handCards.Remove(monsterCard);
                        normalCallNumber--;

                        CallMonsterNotify(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, CardGameState.Back, index);
                        playGameState = PlayGameState.Normal;
                    }
                    else//使用祭品召唤
                    {
                        int monsterLevel = monsterCard.GetLevel();
                        if (GetCanBeSacrificeMonsterNumber() >= 1)
                        {
                            playGameState = PlayGameState.ChooseSacrifice;
                            needSacrificeCallMonster = monsterCard;
                        }
                    }
                }
            }
        }

        public virtual void CallMonsterWithSacrificeNotify(int id, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag, string sacrificeInfo)
        {

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

                StringBuilder sacrificeInfo = new StringBuilder();
                for (int i = 0; i < sacrificeCards.Count; i++)
                {
                    if(i== sacrificeCards.Count-1)
                    {
                        sacrificeInfo.Append(sacrificeCards[i].GetID());
                    }
                    else
                    {
                        sacrificeInfo.Append(sacrificeCards[i].GetID() + ":");
                    }
                }
                opponentPlayer.CallMonsterWithSacrificeNotify(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, CardGameState.FrontATK, index, sacrificeInfo.ToString());
                
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
        public void CallMonsterByProtocol(int cardID, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag, string sacrificeinfo = null)
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

            monsterCardArea[flag] = monsterCard;
            monsterCard.AddContent("monsterCardAreaIndex", flag);
            monsterCard.cardObject.gameObject.GetComponent<DuelCardScript>().SetCanShowInfo(true);
            monsterCard.SetCardGameState(toCardGameState);
            monsterCard.cardObject.transform.SetParent(duelScene.duelBackImage.transform);
            monsterCard.cardObject.transform.localPosition = new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.opponentMonsterCardPositionY, 1);
            monsterCard.cardObject.transform.localRotation = Quaternion.Euler(0,0,180);
            monsterCard.cardObject.GetComponent<Image>().sprite = monsterCard.GetImage();
            handCards.Remove(monsterCard);
            normalCallNumber--;

            ThinkAction();
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

            if (sacrificeCards.Count>= needSacrificeCallMonster.NeedSacrificeMonsterNumer())
            {
                CallMonsterWithSacrifice(needSacrificeCallMonster);
            }
            return true;
        }

        /// <summary>
        /// 玩家主动停止决斗
        /// </summary>
        public void StopDuel()
        {
            opponentPlayer.StopDuelNotify();
            GameManager.GetSingleInstance().StopDuel();
        }

        /// <summary>
        /// 停止决斗通知
        /// </summary>
        public virtual void StopDuelNotify()
        {

        }

        /// <summary>
        /// 玩家投降通知
        /// </summary>
        public virtual void SurrenderNotify()
        {

        }
    }
}
