using Assets.Script.Card;
using Assets.Script.Duel.EffectProcess;
using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using Assets.Script.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
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

        #region 卡牌

        protected DuelCardGroup duelCardGroup;
        protected List<CardBase> handCards = new List<CardBase>();
        protected List<CardBase> tombCards = new List<CardBase>();
        protected List<CardBase> exceptCards = new List<CardBase>();

        protected List<MonsterCard> sacrificeCards = new List<MonsterCard>();

        protected CardBase[] monsterCardArea = new CardBase[DuelRuleManager.GetMonsterAreaNumber()];
        protected CardBase[] magicTrapCardArea = new CardBase[DuelRuleManager.GetMonsterAreaNumber()];

        #endregion

        protected DuelScene duelScene = null;
        protected string cardGroupName;//我方玩家卡组名称
        Player opponentPlayer = null;

        GameObject handPanel = null;
        protected Scrollbar lifeScrollBar = null;
        protected Text lifeNumberText = null;
        protected Vector3 heartPosition;
        protected int life = 4000;
        
        bool canBeDirectAttacked = true;
        bool canDirectAttack = true;

        PlayGameState playGameState;

        //为祭品召唤准备的，卡片召唤状态
        CardGameState cardGameStateForSacrifice = CardGameState.Unknown;
        
        MonsterCard needSacrificeCallMonster = null;

        GuessEnum guessEnum = GuessEnum.Unknown;

        bool iamReady = false;//判断玩家是否准备完成

        int normalCallNumber = DuelRuleManager.GetDrawNumberEveryTurn();

        List<EffectProcessBase> effectProcessList = new List<EffectProcessBase>();

        DuelEffectProcess duelEffectProcess;
        protected EffectProcessBase currentEffectProcess;

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

        /// <summary>
        /// 获得手牌列表
        /// </summary>
        /// <returns></returns>
        public List<CardBase> GetHandCards()
        {
            return handCards;
        }

        /// <summary>
        /// 获得墓地卡牌牌列表
        /// </summary>
        /// <returns></returns>
        public List<CardBase> GetTombCards()
        {
            return tombCards;
        }

        /// <summary>
        /// 获得除外卡牌列表
        /// </summary>
        /// <returns></returns>
        public List<CardBase> GetExceptCards()
        {
            return exceptCards;
        }

        /// <summary>
        /// 判断怪兽区是否已满
        /// </summary>
        /// <returns></returns>
        public bool MonsterAreaIsFull()
        {
            foreach (var item in monsterCardArea)
            {
                if (item == null)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 在决斗前的初始化
        /// </summary>
        public virtual void InitBeforDuel()
        {
            heartPosition = new Vector3(DuelCommonValue.myHeartPositionX, DuelCommonValue.myHeartPositionY);

            lifeScrollBar = GameObject.Find("myLifeScrollbar").GetComponent<Scrollbar>();
            lifeNumberText = GameObject.Find("myLifeNumberText").GetComponent<Text>();

            life = DuelRuleManager.GetPlayerStartLife();

            List<CardBase> myCards = duelCardGroup.GetCards();
            for (int i = 0; i < myCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(myCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                myCards[i].SetCardObject(go);
                go.transform.SetParent(duelScene.duelBackImage.transform);
                go.transform.localPosition = new Vector3(DuelCommonValue.myCardGroupPositionX, DuelCommonValue.myCardGroupPositionY, 0);
            }
        }

        public bool IAmReady()
        {
            return iamReady;
        }

        /// <summary>
        /// 设置猜先内容
        /// </summary>
        /// <param name="guessEnum"></param>
        /// <returns></returns>
        public bool SetGuessEnum(GuessEnum guessEnum)
        {
            if(this.guessEnum == GuessEnum.Unknown || guessEnum == GuessEnum.Unknown)
            {
                this.guessEnum = guessEnum;
                opponentPlayer.GuessFirstNotify(guessEnum);
                return true;
            }
            return false;
        }

        public GuessEnum GetGuessEnum()
        {
            return guessEnum;
        }
        
        /// <summary>
        /// 选择先攻还是后手
        /// </summary>
        public virtual void SelectFristOrBack()
        {

        }

        public virtual void SelectFristOrBackNotify(bool opponentSelectFrist)
        {

        }

        public void SetCardGroupName(string cardGroupName)
        {
            this.cardGroupName = cardGroupName;
        }

        /// <summary>
        /// 设置卡组
        /// </summary>
        /// <param name="selectCardGroup"></param>
        public virtual void SetCardGroup()
        {
            if(duelCardGroup!=null)
            {
                return;
            }
            duelCardGroup = new DuelCardGroup();
            UserCardGroup selectCardGroup = GameManager.GetSingleInstance().GetUserData().GetCardGroupByName(cardGroupName);

            foreach (var item in selectCardGroup.mainCardList)
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

        public void SetCanDirectAttack(bool canDirectAttack)
        {
            this.canDirectAttack = canDirectAttack;
        }

        public bool GetCanDirectAttack()
        {
            return canDirectAttack;
        }

        /// <summary>
        /// 添加效果处理
        /// </summary>
        public void AddEffectProcess(EffectProcessBase effectProcess)
        {
            effectProcessList.Add(effectProcess);
            effectProcess.Process();
        }

        /// <summary>
        /// 移除效果处理
        /// </summary>
        /// <param name="effectProcess"></param>
        public void RemoveEffectProcess(EffectProcessBase effectProcess)
        {
            if(currentEffectProcess==effectProcess)
            {
                currentEffectProcess = null;
            }
            effectProcessList.Remove(effectProcess);
        }

        public void SetDuelEffectProcess(DuelEffectProcess duelEffectProcess)
        {
            this.duelEffectProcess = duelEffectProcess;
        }

        public DuelEffectProcess GetDuelEffectProcess()
        {
            return duelEffectProcess;
        }

        /// <summary>
        /// 获得当前对玩家产生影响的效果处理类
        /// </summary>
        /// <returns></returns>
        public EffectProcessBase GetCurrentEffectProcess()
        {
            return currentEffectProcess;
        }

        /// <summary>
        /// 一般由效果处理类调用，设置当前对玩家产生影响效果处理类
        /// </summary>
        /// <param name="currentEffectProcess"></param>
        public void SetCurrentEffectProcess(EffectProcessBase currentEffectProcess)
        {
            this.currentEffectProcess = currentEffectProcess;
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
        /// 判断玩家是否可以战斗
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
                    if (item.GetDuelCardScript().CanAttack())
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
                if(item.GetDuelCardScript().CanCall())
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
            duelScene.SetWinner(opponentPlayer,DuelEndReason.Surrender);
        }

        /// <summary>
        /// 决斗开始时抽卡
        /// </summary>
        public void DrawAtFirst()
        {
            for (int i = 0; i < DuelRuleManager.GetDrawNumberOnStartDuel(); i++)
            {
                Draw();
            }
        }

        /// <summary>
        /// 抽卡
        /// </summary>
        public virtual bool Draw()
        {
            if(GetDuelCardGroup().GetCards().Count<=0)
            {
                return false;
            }
            CardBase card = duelCardGroup.GetCards()[0];
            card.GetDuelCardScript().SetParent(handPanel.transform);
            duelCardGroup.GetCards().RemoveAt(0);
            handCards.Add(card);
            card.SetCardGameState(CardGameState.Hand);
            
            if (this == duelScene.myPlayer)
            {
                card.GetDuelCardScript().ShowFront();
                card.GetDuelCardScript().SetCanShowInfo(true);
            }
            else if(this == duelScene.opponentPlayer)
            {
                if(GameManager.GetSingleInstance().GetUserData().showOpponentHandCard)
                {
                    card.GetDuelCardScript().SetCanShowInfo(true);
                }
            }

            duelScene.ShowHelpInfoPanel();

            opponentPlayer.DrawNotify();
            return true;
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
        /// 检查所有注册到玩家的效果处理，判断是否可以触发并触发
        /// </summary>
        /// <returns></returns>
        public bool CheckAllEffectProcess()
        {
            for (int i = effectProcessList.Count - 1; i >= 0; i--)
            {
                if(effectProcessList[i].CheckCanTrigger())
                {
                    effectProcessList[i].Process();
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 开始回合
        /// </summary>
        public void StartTurn()
        {
            playGameState = PlayGameState.Normal;
            duelScene.EnterDuelProcess(DuelProcess.Draw);

            TimerFunction timerFunction = new TimerFunction();

            timerFunction.SetFunction(0.5f, () => 
            {
                normalCallNumber = DuelRuleManager.GetNormalCallMonsterNumber();
                if (GetDuelCardGroup().GetCards().Count <= 0)
                {
                    duelScene.SetWinner(opponentPlayer, DuelEndReason.Draw);
                    return;
                }
                Draw();
            });

            GameManager.AddTimerFunction(timerFunction);
        }

        public void Update()
        {
            if(duelScene.currentDuelProcess==DuelProcess.End)
            {

            }
        }

        /// <summary>
        /// 为场上怪兽添加攻守转换次数
        /// </summary>
        public void CheckAndSetAllMonsterChangeAttackOrDefenseNumber()
        {
            foreach (var item in monsterCardArea)
            {
                if (item != null)
                {
                    item.GetDuelCardScript().SetChangeAttackOrDefenseNumber();
                }
            }
        }

        /// <summary>
        /// 思考行动
        /// </summary>
        public virtual void ThinkAction()
        {
            if(currentEffectProcess!=null)
            {
                return;
            }
            if (duelScene.currentDuelProcess == DuelProcess.Draw)
            {
                duelScene.EnterDuelProcess(DuelProcess.Prepare);
            }
            else if (duelScene.currentDuelProcess == DuelProcess.Prepare)
            {
                duelScene.EnterDuelProcess(DuelProcess.Main);
            }
            else if (duelScene.currentDuelProcess == DuelProcess.End)
            {
            }
        }
        
        /// <summary>
        /// 结束回合
        /// </summary>
        public void EndTurn()
        {
            if (!IsMyTurn())
            {
                GameManager.ShowMessage("不是你的回合！");
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
                GameManager.ShowMessage("不是你的回合！");
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
                GameManager.ShowMessage("不是你的回合！");
                return;
            }
            if (duelScene.GetCurrentTurnNumber()==1&&duelScene.startPlayer==this)
            {
                GameManager.ShowMessage("第一回合先攻者不能攻击！");
                return;
            }
            if(duelScene.currentDuelProcess != DuelProcess.Main)
            {
                GameManager.ShowMessage("只有主要流程才可以进入战斗！当前流程为："+ duelScene.currentDuelProcess.ToString());
                return;
            }
            foreach (var item in monsterCardArea)
            {
                if(item!=null)
                {
                    item.GetDuelCardScript().SetAttackNumber();
                }
            }
            duelScene.Battle();
        }

        /// <summary>
        /// 检查所有怪兽是否可以攻击，并显示可攻击的图标
        /// </summary>
        public void CheckAndShowAllMonsterCanAttack()
        {
            foreach (var item in monsterCardArea)
            {
                if(item!=null && item.GetDuelCardScript().CanAttack())
                {
                    item.GetDuelCardScript().SetAttackState(true);
                }
            }
        }

        /// <summary>
        /// 清空所有怪兽攻击状态，并隐藏可攻击图标
        /// </summary>
        public void ClearAllMonsterCanAttack()
        {
            foreach (var item in monsterCardArea)
            {
                if (item != null)
                {
                    item.GetDuelCardScript().SetAttackState(false);
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
            this.life = this.life > 0 ? this.life : 0;
            lifeScrollBar.size =(float)this.life / DuelRuleManager.GetPlayerStartLife();
            lifeNumberText.text = this.life + "/" + DuelRuleManager.GetPlayerStartLife();
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
        /// 召唤怪兽到场上，未指定表侧形式，所以显示攻守选择面板
        /// </summary>
        /// <param name="monsterCard"></param>
        public void CallMonster(MonsterCard monsterCard)
        {
            duelScene.ShowAttackOrDefensePanel(monsterCard,(cardGameState)=> 
            {
                CallMonster(monsterCard, cardGameState);
            });
        }

        /// <summary>
        /// 召唤怪兽到场上
        /// </summary>
        /// <param name="monsterCard"></param>
        public void CallMonster(MonsterCard monsterCard, CardGameState cardGameState)
        {
            if (IsMyPlayer())
            {
                //检测召唤条件是否满足
                if (normalCallNumber > 0)
                {
                    //先判断是否可以直接进行召唤
                    if (monsterCard.GetLevel() <= DuelRuleManager.GetCallMonsterWithoutSacrificeLevelUpperLimit())
                    {
                        int index = 0;
                        for (; index < DuelRuleManager.GetMonsterAreaNumber(); index++)
                        {
                            if (monsterCardArea[index] == null)
                            {
                                monsterCardArea[index] = monsterCard;
                                break;
                            }
                        }
                        monsterCard.AddContent("monsterCardAreaIndex", index);
                        monsterCard.SetCardGameState(cardGameState);
                        monsterCard.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
                        monsterCard.GetDuelCardScript().SetLocalPosition(new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1));
                        handCards.Remove(monsterCard);
                        normalCallNumber--;

                        CallMonsterNotify(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, cardGameState, index);
                        playGameState = PlayGameState.Normal;
                    }
                    else//使用祭品召唤
                    {
                        int monsterLevel = monsterCard.GetLevel();
                        if (GetCanBeSacrificeMonsterNumber() >= 1)
                        {
                            playGameState = PlayGameState.ChooseSacrifice;
                            cardGameStateForSacrifice = cardGameState;
                            needSacrificeCallMonster = monsterCard;
                        }
                    }
                }
            }
        }

        public virtual void CallMonsterNotify(int id,CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState,int flag)
        {

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
                if(cardGameStateForSacrifice==CardGameState.Unknown)
                {
                    Debug.LogError("祭品召唤时卡片状态出现问题！");
                    return;
                }
                foreach (var item in sacrificeCards)
                {
                    item.GetDuelCardScript().ClearCurrentState();
                    MoveCardToTomb(item);
                }
                int index = 0;
                for (; index < DuelRuleManager.GetMonsterAreaNumber(); index++)
                {
                    if (monsterCardArea[index] == null)
                    {
                        monsterCardArea[index] = monsterCard;
                        break;
                    }
                }
                monsterCard.AddContent("monsterCardAreaIndex", index);
                monsterCard.SetCardGameState(cardGameStateForSacrifice);
                monsterCard.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
                monsterCard.GetDuelCardScript().SetLocalPosition( new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.myMonsterCardPositionY, 1));
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
                opponentPlayer.CallMonsterWithSacrificeNotify(monsterCard.GetID(), CallType.Normal, CardGameState.Hand, CardGameState.FrontAttack, index, sacrificeInfo.ToString());
                
                sacrificeCards.Clear();
                cardGameStateForSacrifice = CardGameState.Unknown;
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
        /// 判断此玩家是否为我方玩家 
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

            int index = DuelRuleManager.GetMonsterAreaNumber() - flag - 1;

            monsterCardArea[flag] = monsterCard;
            monsterCard.AddContent("monsterCardAreaIndex", flag);
            monsterCard.SetCardGameState(toCardGameState);
            monsterCard.GetDuelCardScript().SetParent(duelScene.duelBackImage.transform);
            monsterCard.GetDuelCardScript().SetLocalPosition(new Vector3(DuelCommonValue.cardOnBackFarLeftPositionX + index * DuelCommonValue.cardGap, DuelCommonValue.opponentMonsterCardPositionY, 1));
            
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

        public void SetLife(int newLife)
        {
            life = newLife;
            life = life > 0 ? life : 0;
            lifeScrollBar.size = (float)life / DuelRuleManager.GetPlayerStartLife();
            lifeNumberText.text = life + "/" + DuelRuleManager.GetPlayerStartLife();
            duelScene.CheckWinByLife();
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
                    handCards.Remove(card);
                    tombCards.Add(card);
                    if(GetCurrentEffectProcess() is DiscardHandCardEffectProcess)
                    {
                        (GetCurrentEffectProcess() as DiscardHandCardEffectProcess).DiscardOneHandCard();
                    }
                    break;
                case CardGameState.FrontAttack:
                case CardGameState.FrontDefense:
                case CardGameState.Back:
                    monsterCardArea[int.Parse(card.GetContent("monsterCardAreaIndex").ToString())]=null;
                    card.ClearAllContent();
                    
                    tombCards.Add(card);
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
        /// 判断是否可以选择作为祭品
        /// </summary>
        /// <param name="card"></param>
        /// <returns></returns>
        public bool CanChooseMonsterAsSacrifice(MonsterCard card)
        {
            if (!sacrificeCards.Contains(card))
            {
                sacrificeCards.Add(card);
                return true;
            }
            else
            {
                Debug.LogError("ChooseMonsterAsSacrifice 已经选中了此怪兽:" + card.GetName());
                return false;
            }
        }

        /// <summary>
        /// 检测当前祭品数量是否足够,足够则直接进行祭品召唤
        /// </summary>
        /// <returns></returns>
        public void TrySacrificeCall()
        {
            if (sacrificeCards.Count>= needSacrificeCallMonster.NeedSacrificeMonsterNumer())
            {
                CallMonsterWithSacrifice(needSacrificeCallMonster);
            }
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

        /// <summary>
        /// 玩家猜先通知
        /// </summary>
        public virtual void GuessFirstNotify(GuessEnum guessEnum)
        {

        }
    }
}
