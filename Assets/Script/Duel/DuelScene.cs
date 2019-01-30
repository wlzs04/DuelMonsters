using Assets.Script.Card;
using Assets.Script.Config;
using Assets.Script.Duel.EffectProcess;
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

        public Player myPlayer = null;//我方玩家
        public Player opponentPlayer = null;//敌方玩家

        public Player currentPlayer;//当前玩家
        public Player startPlayer;//开始玩家
        int currentTurnNumber = 1;//当前回合数
        public DuelProcess currentDuelProcess = DuelProcess.Unknown;//当前流程

        bool inSpecialState = false;//在特殊状态，例如进入流程选择界面

        public Image duelBackImage = null;
        public AttackAnimationScript attackAnimationScript = null;
        public Image environmentImage = null;
        public Image cardImage = null;
        public GameObject myHandCardPanel = null;
        public GameObject opponentHandCardPanel = null;

        DuelSceneScript duelSceneScript = null;
        
        bool startDuel = false;
        
        bool inChain = false;//是否在连锁中
        Stack<UnityAction> chainStack=new Stack<UnityAction>();

        public DuelScene(DuelMode duelMode)
        {
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

        /// <summary>
        /// 从场景中初始化控件
        /// </summary>
        void InitControlFromScene()
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

        /// <summary>
        /// 设置我方玩家是否先攻
        /// </summary>
        /// <param name="myFirst"></param>
        public void SetFirst(bool myFirst)
        {
            if(myFirst)
            {
                startPlayer = myPlayer;
                GameManager.ShowMessage("您先手！");
            }
            else
            {
                startPlayer = opponentPlayer;
                GameManager.ShowMessage("您后手！");
            }
        }

        /// <summary>
        /// 右键点击
        /// </summary>
        public void MouseRightButtonDown()
        {
            //当我方玩家在进行攻击时，取消攻击
            if (myPlayer.GetCurrentEffectProcess() is AttackEffectProcess)
            {
                myPlayer.GetCurrentEffectProcess().Stop();
            }
            else if (duelSceneScript.AttackOrDefensePanelIsShowing())
            {
                //如果攻击防御选择面板正在显示，则不做处理。
            }
            else if (duelSceneScript.CardListPanelIsShowing())
            {
                //如果卡牌列表正在显示，则隐藏。
                duelSceneScript.HideCardListPanel();
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
        public void ShowAttackOrDefensePanel(CardBase monsterCard,UnityAction<CardGameState> finishAction)
        {
            duelSceneScript.ShowAttackOrDefensePanel(monsterCard, finishAction);
        }
        
        /// <summary>
        /// 显示指定玩家当前指定类型的卡牌列表
        /// </summary>
        /// <param name="ownerPlayer"></param>
        /// <param name="cardGameState"></param>
        public void ShowCardList(Player ownerPlayer, CardGameState cardGameState)
        {
            if(duelSceneScript.CardListPanelIsShowing())
            {
                return;
            }
            StringResConfig stringResConfig = ConfigManager.GetConfigByName("StringRes") as StringResConfig;
            string titleText = ownerPlayer==myPlayer? stringResConfig.GetRecordById(9).value: stringResConfig.GetRecordById(10).value;
            switch (cardGameState)
            {
                case CardGameState.Tomb:
                    duelSceneScript.ShowCardListPanel(ownerPlayer.GetTombCards(), titleText+ stringResConfig.GetRecordById(11).value);
                    break;
                case CardGameState.Exclusion:
                    duelSceneScript.ShowCardListPanel(ownerPlayer.GetExceptCards(),titleText + stringResConfig.GetRecordById(12).value);
                    break;
                default:
                    Debug.LogError("无法显示当前卡牌状态的列表:" + cardGameState);
                    break;
            }
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
            bool ilost = myPlayer.GetLife() <= 0;
            bool iWin = opponentPlayer.GetLife() <= 0;
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
            duelSceneScript.ShowDuelResultPanel(player, duelEndReason);

            TimerFunction timerFunction = new TimerFunction();

            timerFunction.SetFunction(2, () =>
            {
                Thread.Sleep(2000);
                GameManager.GetSingleInstance().CleanDuelScene();
                GameManager.GetSingleInstance().EnterMainScene();
            });

            GameManager.AddTimerFunction(timerFunction);
            startDuel = false;
        }
        
        /// <summary>
        /// 初始化决斗场景
        /// </summary>
        public void Init()
        {
            InitControlFromScene();
            myPlayer.SetCardGroup();
            opponentPlayer.SetCardGroup();
        }

        public void Update()
        {
            if(startDuel)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    MouseRightButtonDown();
                }
                if(!inChain)
                {
                    if (chainStack.Count>0)
                    {
                        inChain = true;
                        UnityAction action = chainStack.Pop();
                        action += () => 
                        {
                            inChain = false;
                        };
                        action();
                    }
                }
                startPlayer.Update();
                startPlayer.GetOpponentPlayer().Update();
            }
        }

        public bool IsInChain()
        {
            return inChain;
        }

        /// <summary>
        /// 检查是否决斗双方都已经初始化完成
        /// </summary>
        public void CheckPlayInit()
        {
            if(myPlayer.IAmReady()&& opponentPlayer.IAmReady())
            {
                currentPlayer = startPlayer;

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
            myPlayer.InitBeforDuel();
            opponentPlayer.InitBeforDuel();

            GameManager.ShowMessage("决斗开始！");

            startDuel = true;

            EffectProcessBase myCheckHandCardEffectProcess = new CheckHandCardEffectProcess(myPlayer);
            myPlayer.AddEffectProcess(myCheckHandCardEffectProcess);
            EffectProcessBase opponentCheckHandCardEffectProcess = new CheckHandCardEffectProcess(opponentPlayer);
            opponentPlayer.AddEffectProcess(opponentCheckHandCardEffectProcess);

            EffectProcessBase myDrawCardEveryTurnEffectProcess = new DrawCardEveryTurnEffectProcess(myPlayer);
            myPlayer.AddEffectProcess(myDrawCardEveryTurnEffectProcess);
            EffectProcessBase opponentDrawCardEveryTurnEffectProcess = new DrawCardEveryTurnEffectProcess(opponentPlayer);
            opponentPlayer.AddEffectProcess(opponentDrawCardEveryTurnEffectProcess);

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
            duelSceneScript.ShowHelpInfoPanel(myPlayer.GetDuelCardGroup().GetCards().Count, opponentPlayer.GetDuelCardGroup().GetCards().Count);
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
        }

        /// <summary>
        /// 检查是否存在可以触发的效果，将其全部触发完毕，然后执行传入的方法
        /// </summary>
        /// <param name=""></param>
        public void CheckAllEffectProcess(UnityAction nextAction = null)
        {
            if(nextAction!=null)
            {
                chainStack.Push(nextAction);
            }
            if(!currentPlayer.CheckAllEffectProcess() && !currentPlayer.GetOpponentPlayer().CheckAllEffectProcess())
            {
            }
        }

        /// <summary>
        /// 切换当前玩家一般在结束回合后调用
        /// </summary>
        /// <param name="player"></param>
        public void ChangeCurrentPlayer()
        {
            currentTurnNumber++;
            currentDuelProcess = DuelProcess.Unknown;
            currentPlayer = currentPlayer == myPlayer ? opponentPlayer : myPlayer;
            currentPlayer.StartTurn();
        }

        /// <summary>
        /// 进入战斗流程
        /// </summary>
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

        /// <summary>
        /// 显示指定卡牌的详细信息
        /// </summary>
        /// <param name="card"></param>
        public void ShowCardInfo(CardBase card)
        {
            duelSceneScript.SetInfoContent(card);
        }
        
        /// <summary>
        /// 进入指定流程
        /// </summary>
        /// <param name="duelProcess"></param>
        public void EnterDuelProcess(DuelProcess duelProcess)
        {
            if(currentDuelProcess == duelProcess)
            {
                Debug.LogError("当前流程已经是："+ currentDuelProcess + "无法重复进入！");
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
            duelSceneScript.ResetDuelProcessPanelInfo();
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
                    BeginMainDuelProcessEvent();
                    break;
                case DuelProcess.Battle:
                    GameManager.ShowMessage(ex + "战斗流程！");
                    BeginBattleDuelProcessEvent();
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
            currentPlayer.GetOpponentPlayer().EnterDuelNotify(currentDuelProcess);
            TimerFunction timerFunction = new TimerFunction();
            timerFunction.SetFunction(1, () =>
            {
                CheckAllEffectProcess(currentPlayer.ThinkAction);
            });
            GameManager.AddTimerFunction(timerFunction);
        }

        /// <summary>
        /// 开始战斗流程事件
        /// </summary>
        void BeginBattleDuelProcessEvent()
        {
             currentPlayer.CheckAndShowAllMonsterCanAttack();
        }

        /// <summary>
        /// 开始主要流程事件
        /// </summary>
        void BeginMainDuelProcessEvent()
        {
            currentPlayer.CheckAndSetAllMonsterChangeAttackOrDefenseNumber();
        }

        /// <summary>
        /// 结束战斗流程事件
        /// </summary>
        void EndBattleDuelProcessEvent()
        {
            currentPlayer.ClearAllMonsterCanAttack();
        }
        
        //带有Opponent前缀的方法

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
            //AttackMonster((MonsterCard)card1, (MonsterCard)card2);
        }

        /// <summary>
        /// 对方怪兽直接攻击
        /// </summary>
        /// <param name="cardID"></param>
        public void OpponentAttack(int cardID)
        {
            CardBase card1 = opponentPlayer.GetCardByID(cardID);
            //AttackDirect((MonsterCard)card1);
        }

        public void OpponentSelectFirstOrBack(bool selectFirstOrBack)
        {
            GuessFirstSceneScript guessFirstSceneScript = GameObject.Find("Main Camera").GetComponent<GuessFirstSceneScript>();

            SetFirst(!selectFirstOrBack);
            TimerFunction timeFunction = new TimerFunction();
            timeFunction.SetFunction(1, () =>
            {
                GameManager.GetSingleInstance().EnterDuelScene();
            });

            GameManager.AddTimerFunction(timeFunction);
            guessFirstSceneScript.HideSelectFirstPanel();
        }
    }
}
