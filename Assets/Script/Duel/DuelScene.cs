using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using Assets.Script.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Assets.Script.Duel
{
    /// <summary>
    /// 决斗模式
    /// </summary>
    public enum DuelMode
    {
        Unknown,//未知
        Single,//单人
        Net//网络
    }

    /// <summary>
    /// 决斗场
    /// </summary>
    public class DuelScene
    {
        string cardPrefabPath = "Prefab/CardPre";
        public GameObject cardPre = null;
        DuelMode duelMode = DuelMode.Unknown;

        public Player myPlayer = null;//我方玩家
        public Player opponentPlayer = null;//敌方玩家

        public bool myFirst = false;
        public Player currentPlayer;//当前玩家
        public Player startPlayer;//开始玩家
        public int currentTurnNumber = 1;//当前回合数
        public DuelProcess currentDuelProcess = DuelProcess.Unknown;//当前流程

        bool inSpecialState = false;//在特殊状态，例如进入流程选择界面

        public Image duelBackImage = null;
        public AttackAnimationScript attackAnimationScript = null;
        public Image environmentImage = null;
        public Image cardImage = null;
        public GameObject myHandCardPanel = null;
        public GameObject opponentHandCardPanel = null;

        DuelSceneScript duelSceneScript = null;

        CardBase currentChooseCard = null;
        bool canChoose = false;

        public DuelScene(DuelMode duelMode)
        {
            this.duelMode = duelMode;
            myPlayer = new Player("玩家", this);
            switch (duelMode)
            {
                case DuelMode.Unknown:
                    Debug.LogError("未选择决斗模式！");
                    return;
                case DuelMode.Single:
                    opponentPlayer = new ComputerPlayer(this);
                    break;
                case DuelMode.Net:
                    opponentPlayer = new NetPlayer(this);
                    break;
                default:
                    break;
            }

            myPlayer.SetOpponentPlayer(opponentPlayer);
            opponentPlayer.SetOpponentPlayer(myPlayer);

            cardPre = Resources.Load<GameObject>(cardPrefabPath);
        }

        void AddControlFromScene()
        {
            duelBackImage = GameObject.Find("duelBackImage").GetComponent<Image>();
            environmentImage = GameObject.Find("environmentImage").GetComponent<Image>();
            attackAnimationScript = GameObject.Find("attackImage").GetComponent<AttackAnimationScript>();
            attackAnimationScript.StopPlay();
            cardImage = GameObject.Find("cardImage").GetComponent<Image>();
            myHandCardPanel = GameObject.Find("MyHandCardPanel");
            opponentHandCardPanel = GameObject.Find("OpponentHandCardPanel");
        }

        public int GetCurrentTurnNumber()
        {
            return currentTurnNumber;
        }

        public DuelMode GetDuelMode()
        {
            return duelMode;
        }

        /// <summary>
        /// 设置我方玩家是否先攻
        /// </summary>
        /// <param name="myFirst"></param>
        public void SetFirst(bool myFirst)
        {
            this.myFirst = myFirst;
        }

        /// <summary>
        /// 右键点击
        /// </summary>
        public void MouseRightButtonDown()
        {
            //取消当前选中卡牌的状态
            if(currentChooseCard!=null)
            {
                currentChooseCard.cardObject.GetComponent<DuelCardScript>().ClearCurrentState();
                currentChooseCard = null;
            }
            else if(duelSceneScript.AttackOrDefensePanelIsShowing())
            {
                //如果攻击防御选择面板正在显示，则不做处理。
            }
            else
            {
                //显示或隐藏决斗流程面板
                if (!inSpecialState)
                {
                    duelSceneScript.SetDuelProcessPanel(true);
                    inSpecialState = true;
                }
                else
                {
                    duelSceneScript.SetDuelProcessPanel(false);
                    inSpecialState = false;
                }
            }
        }

        /// <summary>
        /// 显示攻击防御选择面板
        /// </summary>
        /// <param name="monsterCard"></param>
        /// <param name="finishAction"></param>
        public void ShowAttackOrDefensePanel(MonsterCard monsterCard,UnityAction<CardGameState> finishAction)
        {
            duelSceneScript.ShowAttackOrDefensePanel(monsterCard, finishAction);
        }

        /// <summary>
        /// 设置为可以选择卡牌的状态
        /// </summary>
        /// <param name="canChoose"></param>
        public void SetCanChoose(bool canChoose)
        {
            this.canChoose = canChoose;
        }

        /// <summary>
        /// 选择一张卡
        /// </summary>
        /// <param name="card"></param>
        public void ChooseCard(CardBase card)
        {
            if(!canChoose)
            {
                return;
            }
            if(currentChooseCard == null)
            {
                currentChooseCard = card;
            }
            else
            {
                ChooseAnotherCard(card);
            }
        }

        /// <summary>
        /// 选择另一张卡
        /// </summary>
        /// <param name="card"></param>
        void ChooseAnotherCard(CardBase card)
        {
            if(currentDuelProcess==DuelProcess.Battle)
            {
                if(currentChooseCard.cardObject.GetComponent<DuelCardScript>().GetOwner()!= 
                    card.cardObject.GetComponent<DuelCardScript>().GetOwner()&&
                    ((MonsterCard)card).CanBeAttacked)
                {
                    opponentPlayer.BeAttackedMonsterNotify(currentChooseCard.GetID(), card.GetID());
                    
                    AttackMonster((MonsterCard)currentChooseCard, (MonsterCard)card);
                    currentChooseCard = null;
                }
            }
        }

        /// <summary>
        /// 攻击
        /// </summary>
        /// <param name="card1"></param>
        /// <param name="card2"></param>
        void AttackMonster(MonsterCard card1, MonsterCard card2)
        {
            card1.cardObject.GetComponent<DuelCardScript>().Attack();
            int differenceValue = card1.GetAttackNumber() - card2.GetAttackNumber();

            if (differenceValue==0)
            {
                SendCardToTomb(card1);
                SendCardToTomb(card2);
            }
            else if(differenceValue>0)
            {
                card2.cardObject.GetComponent<DuelCardScript>().GetOwner().ReduceLife(differenceValue);
                SendCardToTomb(card2);
            }
            else
            {
                card1.cardObject.GetComponent<DuelCardScript>().GetOwner().ReduceLife(-differenceValue);
                SendCardToTomb(card1);
            }
            canChoose = false;
        }

        /// <summary>
        /// 直接攻击
        /// </summary>
        /// <param name="card"></param>
        public void AttackDirect(MonsterCard card)
        {
            card.cardObject.GetComponent<DuelCardScript>().GetOwner().GetOpponentPlayer().ReduceLife(card.GetAttackNumber());
        }

        public void StartPlayAttackAnimation(Vector3 fromPosition,Vector3 toPosition)
        {
            attackAnimationScript.StartPlay(fromPosition, toPosition);
        }

        public void SetAttackAnimationFinishEvent(AttackAnimationScript.FinishEvent action)
        {
            attackAnimationScript.AnimationFinishEvent += action;
        }

        /// <summary>
        /// 通过生命值检测胜负
        /// </summary>
        public void CheckWinByLife()
        {
            bool ilost = myPlayer.GetLife() <= DuelRule.lostLife;
            bool iWin = opponentPlayer.GetLife() <= DuelRule.lostLife;
            if(!(iWin|| ilost))
            {
                return;
            }

            if (iWin && ilost)
            {
                SetWinner(null, DuelEndReason.Life);
            }
            else if(iWin)
            {
                SetWinner(myPlayer, DuelEndReason.Life);
            }
            else if (ilost)
            {
                SetWinner(opponentPlayer, DuelEndReason.Life);
            }
        }

        /// <summary>
        /// 设置胜者
        /// </summary>
        /// <param name="player"></param>
        public void SetWinner(Player player,DuelEndReason duelEndReason)
        {
            if(player==null)
            {
                GameManager.ShowMessage("平局！");
            }
            else if(player==myPlayer)
            {
                GameManager.ShowMessage("我赢了!");
            }
            else
            {
                GameManager.ShowMessage("我输了!");
            }

            TimerFunction timerFunction = new TimerFunction();

            timerFunction.SetFunction(2, () =>
            {
                Thread.Sleep(2000);
                GameManager.GetSingleInstance().EnterMainScene();
            });

            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 将卡牌送入墓地
        /// </summary>
        /// <param name="card"></param>
        public void SendCardToTomb(CardBase card)
        {
            card.cardObject.GetComponent<DuelCardScript>().GetOwner().MoveCardToTomb(card);
        }

        /// <summary>
        /// 初始化决斗场景
        /// </summary>
        public void Init()
        {
            AddControlFromScene();
            myPlayer.SetCardGroup();
            opponentPlayer.SetCardGroup();
        }

        /// <summary>
        /// 检查是否决斗双方都已经初始化完成
        /// </summary>
        public void CheckPlayInit()
        {
            if(myPlayer.IAmReady()&& opponentPlayer.IAmReady())
            {
                if (myFirst)
                {
                    currentPlayer = myPlayer;
                    startPlayer = myPlayer;
                }
                else
                {
                    currentPlayer = opponentPlayer;
                    startPlayer = opponentPlayer;
                }

                TimerFunction timerFunction = new TimerFunction();
                timerFunction.SetFunction(1, () =>
                {
                    duelSceneScript = GameObject.Find("Main Camera").GetComponent<DuelSceneScript>();
                    myPlayer.SetHandPanel(myHandCardPanel);
                    opponentPlayer.SetHandPanel(opponentHandCardPanel);
                    StartDuel();
                });

                GameManager.AddTimerFunction(timerFunction);
            }
        }
        
        /// <summary>
        /// 开始决斗
        /// </summary>
        void StartDuel()
        {
            myPlayer.SetLife(GameObject.Find("myLifeScrollbar").GetComponent<Scrollbar>(), GameObject.Find("myLifeNumberText").GetComponent<Text>());
            opponentPlayer.SetLife(GameObject.Find("opponentLifeScrollbar").GetComponent<Scrollbar>(), GameObject.Find("opponentLifeNumberText").GetComponent<Text>());

            myPlayer.SetHeartPosition(new Vector3(DuelCommonValue.myHeartPositionX, DuelCommonValue.myHeartPositionY));
            opponentPlayer.SetHeartPosition(new Vector3(DuelCommonValue.opponentHeartPositionX, DuelCommonValue.opponentHeartPositionY));

            myPlayer.InitCardGroup();
            opponentPlayer.InitCardGroup();

            GameManager.ShowMessage("决斗开始！");

            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, () =>
            {
                myPlayer.DrawAtFirst();
                opponentPlayer.DrawAtFirst();
                
                startPlayer.StartTurn();
            });

            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 显示帮助信息面板
        /// </summary>
        public void ShowHelpInfoPanel()
        {
            duelSceneScript.ShowHelpInfoPanel(opponentPlayer.GetDuelCardGroup().GetCards().Count, myPlayer.GetDuelCardGroup().GetCards().Count);
        }

        void EndDuel()
        {

        }

        /// <summary>
        /// 结束当前玩家的回合
        /// </summary>
        public void EndTurn()
        {
            EnterDuelProcess(DuelProcess.End);
            if (currentPlayer == myPlayer)
            {
                opponentPlayer.EndTurnNotify();
            }
            currentTurnNumber++;
            currentDuelProcess = DuelProcess.Unknown;
            currentPlayer = currentPlayer == myPlayer ? opponentPlayer : myPlayer;
            currentPlayer.StartTurn();
        }

        public void Battle()
        {
            EnterDuelProcess(DuelProcess.Battle);
        }

        /// <summary>
        /// 设置环境
        /// </summary>
        public void SetEnvironment()
        {

        }

        public void ShowCardInfo(CardBase card)
        {
            duelSceneScript.SetInfoContent(card);
        }
        
        /// <summary>
        /// 向玩家显示当前流程
        /// </summary>
        /// <param name="duelProcess"></param>
        public void EnterDuelProcess(DuelProcess duelProcess)
        {
            if(currentDuelProcess == duelProcess)
            {
                return;
            }
            switch (currentDuelProcess)
            {
                case DuelProcess.Unknown:
                    break;
                case DuelProcess.Draw:
                    break;
                case DuelProcess.Prepare:
                    break;
                case DuelProcess.Main:
                    break;
                case DuelProcess.Battle:
                    EndBattleDuelProcessEvent();
                    break;
                case DuelProcess.Second:
                    break;
                case DuelProcess.End:
                    break;
                default:
                    break;
            }
            currentDuelProcess = duelProcess;
            string ex = currentPlayer == myPlayer ? "我方进入" : "对方进入";
            switch (duelProcess)
            {
                case DuelProcess.Unknown:
                    GameManager.ShowMessage(ex + "未知流程！");
                    break;
                case DuelProcess.Draw:
                    GameManager.ShowMessage(ex + "抽牌流程！");
                    break;
                case DuelProcess.Prepare:
                    GameManager.ShowMessage(ex + "准备流程！");
                    break;
                case DuelProcess.Main:
                    GameManager.ShowMessage(ex + "主要流程！");
                    break;
                case DuelProcess.Battle:
                    BeginBattleDuelProcessEvent();
                    GameManager.ShowMessage(ex + "战斗流程！");
                    break;
                case DuelProcess.Second:
                    GameManager.ShowMessage(ex + "第二主要流程！");
                    break;
                case DuelProcess.End:
                    GameManager.ShowMessage(ex + "结束流程！");
                    break;
                default:
                    break;
            }
            if(myPlayer.IsMyTurn())
            {
                opponentPlayer.EnterDuelNotify(currentDuelProcess);
            }
            if(opponentPlayer.IsMyTurn())
            {
                opponentPlayer.ThinkAction();
            }
        }

        /// <summary>
        /// 开始战斗流程事件
        /// </summary>
        void BeginBattleDuelProcessEvent()
        {
             currentPlayer.CheckAndShowAllMonsterCanAttack();
        }

        /// <summary>
        /// 结束战斗流程事件
        /// </summary>
        void EndBattleDuelProcessEvent()
        {
            currentPlayer.ClearAllMonsterCanAttack();
        }

        /// <summary>
        /// 将怪兽召唤到怪兽区
        /// </summary>
        public bool AddMonsterCardToDuelArea(MonsterCard monsterCard,CallType callType)
        {
            return false;
        }
        
        // 带有Opponent前缀的方法

        /// <summary>
        /// 对方抽牌
        /// </summary>
        public void OpponentDraw()
        {
            opponentPlayer.Draw();
        }

        /// <summary>
        /// 对方召唤怪兽
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="callType"></param>
        /// <param name="fromCardGameState"></param>
        /// <param name="toCardGameState"></param>
        /// <param name="flag"></param>
        public void OpponentCallMonster(int cardID, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState,int flag)
        {
            opponentPlayer.CallMonsterByProtocol(cardID, callType,fromCardGameState,toCardGameState,flag);
        }
        
        /// <summary>
        /// 对方祭品召唤怪兽
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="callType"></param>
        /// <param name="fromCardGameState"></param>
        /// <param name="toCardGameState"></param>
        /// <param name="flag"></param>
        /// <param name="sacrificeInfo"></param>
        public void OpponentCallMonsterBySacrifice(int cardID, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag,string sacrificeInfo)
        {
            opponentPlayer.CallMonsterByProtocol(cardID, callType, fromCardGameState, toCardGameState, flag, sacrificeInfo);
        }

        /// <summary>
        /// 对方进入某个流程
        /// </summary>
        /// <param name="opponentDuelProcess"></param>
        public void OpponentEnterDuelProcess(DuelProcess opponentDuelProcess)
        {
            EnterDuelProcess(opponentDuelProcess);
        }

        /// <summary>
        /// 对方怪兽攻击
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="anotherCardID"></param>
        public void OpponentAttack(int cardID, int anotherCardID)
        {
            CardBase card1 = opponentPlayer.GetCardByID(cardID);
            CardBase card2 = myPlayer.GetCardByID(anotherCardID);
            AttackMonster((MonsterCard)card1, (MonsterCard)card2);
        }

        /// <summary>
        /// 对方怪兽直接攻击
        /// </summary>
        /// <param name="cardID"></param>
        public void OpponentAttack(int cardID)
        {
            CardBase card1 = opponentPlayer.GetCardByID(cardID);
            AttackDirect((MonsterCard)card1);
        }
    }
}
