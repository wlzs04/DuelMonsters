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

        Player myPlayer = null;//我方玩家
        Player opponentPlayer = null;//敌方玩家
        Player currentPlayer;//当前玩家
        Player startPlayer;//开始玩家

        int currentTurnNumber = 1;//当前回合数
        PhaseType currentPhaseType = PhaseType.Unknown;//当前流程

        bool inSpecialState = false;//在特殊状态，例如进入流程选择界面
        bool lockScene = false;//锁定场景，例如选择卡牌做效果对象，防止右键点击或卡牌操作等进行干扰

        public Image duelBackImage = null;
        public AttackAnimationScript attackAnimationScript = null;
        public Image environmentImage = null;
        public Image cardImage = null;
        public GameObject myHandCardPanel = null;
        public GameObject opponentHandCardPanel = null;

        DuelSceneScript duelSceneScript = null;
        
        bool startDuel = false;
        
        bool inChain = false;//是否在连锁中
        Stack<EffectProcessBase> effectProcessChainStack = new Stack<EffectProcessBase>();

        List<Vector2> myMonsterCardPositionAnchorList = new List<Vector2>();
        List<Vector2> myMagicTrapCardPositionAnchorList = new List<Vector2>();
        List<Vector2> opponentMonsterCardPositionAnchorList = new List<Vector2>();
        List<Vector2> opponentMagicTrapCardPositionAnchorList = new List<Vector2>();

        CardBase currentPointCard = null;//当前鼠标所在位置的卡牌
        CardBase currentInfoCard = null;//当前信息面板显示的卡牌

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

            myMonsterCardPositionAnchorList.Add(DuelCommonValue.myMonsterCardPosition0Anchor);
            myMonsterCardPositionAnchorList.Add(DuelCommonValue.myMonsterCardPosition1Anchor);
            myMonsterCardPositionAnchorList.Add(DuelCommonValue.myMonsterCardPosition2Anchor);
            myMonsterCardPositionAnchorList.Add(DuelCommonValue.myMonsterCardPosition3Anchor);
            myMonsterCardPositionAnchorList.Add(DuelCommonValue.myMonsterCardPosition4Anchor);

            myMagicTrapCardPositionAnchorList.Add(DuelCommonValue.myMagicTrapCardPosition0Anchor);
            myMagicTrapCardPositionAnchorList.Add(DuelCommonValue.myMagicTrapCardPosition1Anchor);
            myMagicTrapCardPositionAnchorList.Add(DuelCommonValue.myMagicTrapCardPosition2Anchor);
            myMagicTrapCardPositionAnchorList.Add(DuelCommonValue.myMagicTrapCardPosition3Anchor);
            myMagicTrapCardPositionAnchorList.Add(DuelCommonValue.myMagicTrapCardPosition4Anchor);

            opponentMonsterCardPositionAnchorList.Add(DuelCommonValue.opponentMonsterCardPosition0Anchor);
            opponentMonsterCardPositionAnchorList.Add(DuelCommonValue.opponentMonsterCardPosition1Anchor);
            opponentMonsterCardPositionAnchorList.Add(DuelCommonValue.opponentMonsterCardPosition2Anchor);
            opponentMonsterCardPositionAnchorList.Add(DuelCommonValue.opponentMonsterCardPosition3Anchor);
            opponentMonsterCardPositionAnchorList.Add(DuelCommonValue.opponentMonsterCardPosition4Anchor);

            opponentMagicTrapCardPositionAnchorList.Add(DuelCommonValue.opponentMagicTrapCardPosition0Anchor);
            opponentMagicTrapCardPositionAnchorList.Add(DuelCommonValue.opponentMagicTrapCardPosition1Anchor);
            opponentMagicTrapCardPositionAnchorList.Add(DuelCommonValue.opponentMagicTrapCardPosition2Anchor);
            opponentMagicTrapCardPositionAnchorList.Add(DuelCommonValue.opponentMagicTrapCardPosition3Anchor);
            opponentMagicTrapCardPositionAnchorList.Add(DuelCommonValue.opponentMagicTrapCardPosition4Anchor);

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

        /// <summary>
        /// 获得决斗场景的宽度
        /// </summary>
        public float GetDuelWidth()
        {
            return duelBackImage.rectTransform.rect.width;
        }

        /// <summary>
        /// 获得决斗场景的高度
        /// </summary>
        public float GetDuelHeight()
        {
            return duelBackImage.rectTransform.rect.height;
        }

        public Player GetMyPlayer()
        {
            return myPlayer;
        }

        public Player GetOpponentPlayer()
        {
            return opponentPlayer;
        }

        public Player GetCurrentPlayer()
        {
            return currentPlayer;
        }

        public Player GetStartPlayer()
        {
            return startPlayer;
        }
        
        /// <summary>
        /// 获得指定玩家怪兽区指定位置的锚点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector2 GetMonsterPositionAnchorByIndex(Player player,int index)
        {
            if(player==myPlayer)
            {
                return myMonsterCardPositionAnchorList[index];
            }
            else if(player == opponentPlayer)
            {
                return opponentMonsterCardPositionAnchorList[index];
            }
            return Vector2.zero;
        }

        /// <summary>
        /// 获得指定玩家魔法陷阱区指定位置的锚点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector2 GetMagicTrapPositionAnchorByIndex(Player player, int index)
        {
            if (player == myPlayer)
            {
                return myMagicTrapCardPositionAnchorList[index];
            }
            else if (player == opponentPlayer)
            {
                return opponentMagicTrapCardPositionAnchorList[index];
            }
            return Vector2.zero;
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
        /// 锁定场景
        /// </summary>
        public void LockScene()
        {
            lockScene = true;
        }

        /// <summary>
        /// 解锁场景
        /// </summary>
        public void UnlockScene()
        {
            lockScene = false;
        }

        /// <summary>
        /// 获得当前场景是否被锁定
        /// </summary>
        /// <returns></returns>
        public bool GetLockScene()
        {
            return lockScene;
        }

        /// <summary>
        /// 右键点击
        /// </summary>
        public void MouseRightButtonDown()
        {
            if(lockScene)
            {
                return;
            }
            //当我方玩家在进行攻击时，取消攻击
            if (myPlayer.GetCurrentEffectProcess() is AttackEffectProcess)
            {
                myPlayer.GetCurrentEffectProcess().Stop();
            }
            else if (duelSceneScript.AttackOrDefensePanelIsShowing())
            {
                //如果攻击防御选择面板正在显示，则不做处理。
            }
            else if (duelSceneScript.EffectSelectPanelIsShowing())
            {
                //如果效果选择面板正在显示，则不做处理。
            }
            else if (duelSceneScript.CardListPanelIsShowing())
            {
                //如果卡牌列表正在显示，则隐藏。
                HideCardList(true);
            }
            else
            {
                //显示或隐藏决斗流程面板
                if (!inSpecialState)
                {
                    duelSceneScript.SetPhaseTypePanel(true);
                    inSpecialState = true;
                }
                else
                {
                    duelSceneScript.SetPhaseTypePanel(false);
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
        /// 显示效果选择面板
        /// </summary>
        /// <param name="effectList"></param>
        /// <param name="finishAction"></param>
        public void ShowEffectSelectPanel(CardBase card, List<string> effectList, ActionIndex finishAction)
        {
            duelSceneScript.ShowEffectSelectPanel(card,effectList, finishAction);
        }

        /// <summary>
        /// 显示抛硬币面板
        /// </summary>
        public void ShowTossCoinPanel(bool showSelectCoinPanel, Action<CoinType, CoinType> actionTossCoin, CoinType coinType = CoinType.Unknown)
        {
            duelSceneScript.ShowTossCoinPanel(showSelectCoinPanel, actionTossCoin,coinType);
        }

        /// <summary>
        /// 显示掷骰子面板
        /// </summary>
        public void ShowThrowDicePanel(Action<int> actionIndex)
        {
            duelSceneScript.ShowThrowDicePanel(actionIndex);
        }

        /// <summary>
        /// 显示选择项面板
        /// </summary>
        public void ShowSelectItemPanel(Type type, UnityAction<int> actionIndex)
        {
            duelSceneScript.ShowSelectItemPanel(type, actionIndex);
        }

        /// <summary>
        /// 显示指定玩家当前指定类型的卡牌列表
        /// </summary>
        /// <param name="ownerPlayer"></param>
        /// <param name="cardGameState"></param>
        public void ShowCardList(Player ownerPlayer, CardGameState cardGameState,bool canHideByPlayer, CardBase launchEffectCard, Action<CardBase, CardBase> clickCallback)
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
                    duelSceneScript.ShowCardListPanel(ownerPlayer.GetTombCards(), titleText + stringResConfig.GetRecordById(11).value, canHideByPlayer, launchEffectCard, clickCallback);
                    break;
                case CardGameState.Exclusion:
                    duelSceneScript.ShowCardListPanel(ownerPlayer.GetExceptCards(),titleText + stringResConfig.GetRecordById(12).value, canHideByPlayer, launchEffectCard, clickCallback);
                    break;
                default:
                    Debug.LogError("无法显示当前卡牌状态的列表:" + cardGameState);
                    break;
            }
        }

        /// <summary>
        /// 显示卡牌列表
        /// </summary>
        /// <param name="cardList"></param>
        /// <param name="title"></param>
        /// <param name="canHideByPlayer"></param>
        /// <param name="clickCalback"></param>
        public void ShowCardList(List<CardBase> cardList, string title, bool canHideByPlayer, CardBase launchEffectCard, Action<CardBase, CardBase> clickCalback)
        {
            duelSceneScript.ShowCardListPanel(cardList, title, canHideByPlayer, launchEffectCard, clickCalback);
        }

        public void HideCardList(bool fromPlayer)
        {

            duelSceneScript.HideCardListPanel(fromPlayer);
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
        /// 获得当前阶段
        /// </summary>
        /// <returns></returns>
        public PhaseType GetCurrentPhaseType()
        {
            return currentPhaseType;
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
                //判断连锁
                if(effectProcessChainStack.Count>0)
                {
                    EffectProcessBase effectProcess = effectProcessChainStack.Peek();
                    if(effectProcess.GetHaveFinish())
                    {
                        effectProcessChainStack.Pop();
                    }
                    else
                    {
                        if(effectProcess.GetBeInterrupted())
                        {
                            if(effectProcess != effectProcess.GetOwnerPlayer().GetCurrentEffectProcess())
                            {
                                effectProcess.GetOwnerPlayer().SetCurrentEffectProcess(effectProcess);
                            }
                            effectProcess.ContinueProcess();
                        }
                    }

                }
                else
                {
                    currentPlayer.ThinkAction();
                }
                startPlayer.Update();
                startPlayer.GetOpponentPlayer().Update();
                if(currentInfoCard!=null)
                {
                    currentInfoCard.Update();
                }
            }
        }

        public bool IsInChain()
        {
            return inChain;
        }

        /// <summary>
        /// 添加效果连锁
        /// </summary>
        /// <param name="effectProcess"></param>
        public void AddEffectProcessChain(EffectProcessBase effectProcess)
        {
            effectProcessChainStack.Push(effectProcess);
        }

        public Stack<EffectProcessBase> GetEffectProcessChain()
        {
            return effectProcessChainStack;
        }

        /// <summary>
        /// 获得当前进行连锁的效果
        /// </summary>
        /// <returns></returns>
        public EffectProcessBase GetCurrentChainEffectProcess()
        {
            foreach (var item in effectProcessChainStack)
            {
                if(item.GetCanChain())
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 获得当前进行连锁中不是由指定卡牌发动的效果，一般用来过滤LaunchEffectEffectProcess
        /// </summary>
        /// <returns></returns>
        public EffectProcessBase GetCurrentChainEffectProcess(CardBase cardBase)
        {
            foreach (var item in effectProcessChainStack)
            {
                if (item.GetCanChain() && item.GetLaunchEffectCard()!= cardBase)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 检查是否决斗双方都已经初始化完成
        /// </summary>
        public void CheckPlayInit()
        {
            if (myPlayer.IAmReady()&& opponentPlayer.IAmReady())
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

        /// <summary>
        /// 设置场景标题
        /// </summary>
        /// <param name="titleText"></param>
        public void SetTitle(string titleText)
        {
            duelSceneScript.ShowTitle(titleText);
        }

        /// <summary>
        /// 显示确认面板
        /// </summary>
        /// <param name="titleText"></param>
        /// <param name="chooseCanChainCard"></param>
        /// <param name="realProcessFunction"></param>
        public void ShowMakeSurePanel(string titleText, UnityAction chooseCanChainCard, UnityAction realProcessFunction)
        {
            duelSceneScript.ShowMakeSurePanel(titleText, chooseCanChainCard, realProcessFunction);
        }

        public void SetCurrentPointCard(CardBase currentPointCard)
        {
            this.currentPointCard = currentPointCard;
        }

        public CardBase GetCurrentPointCard()
        {
            return currentPointCard;
        }

        void EndDuel()
        {

        }

        /// <summary>
        /// 结束当前玩家的回合
        /// </summary>
        public void EndTurn()
        {
            EnterPhaseType(PhaseType.End);
        }

        /// <summary>
        /// 重新检查当前鼠标所在卡牌允许的操作
        /// </summary>
        public void ResetCurrentPointCardAllowedOperation()
        {
            if (currentPointCard != null)
            {
                currentPointCard.GetDuelCardScript().RecheckAllowedOperation();
            }
        }

        /// <summary>
        /// 检查是否存在可以触发的效果，将其全部触发完毕，然后执行传入的方法
        /// </summary>
        /// <param name=""></param>
        public void CheckAllEffectProcess(UnityAction nextAction = null)
        {
            currentPlayer.CheckAllEffectProcess();
            //nextAction();
        }

        /// <summary>
        /// 切换当前玩家一般在结束回合后调用
        /// </summary>
        /// <param name="player"></param>
        public void ChangeCurrentPlayer()
        {
            currentTurnNumber++;
            currentPhaseType = PhaseType.Unknown;
            currentPlayer = currentPlayer == myPlayer ? opponentPlayer : myPlayer;
            currentPlayer.StartTurn();
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
        public void SetCurrentInfoCard(CardBase currentInfoCard)
        {
            this.currentInfoCard = currentInfoCard;
            duelSceneScript.SetInfoContent(currentInfoCard);
        }

        public CardBase GetCurrentInfoCard()
        {
            return currentInfoCard;
        }
        
        /// <summary>
        /// 进入指定流程
        /// </summary>
        /// <param name="phaseType"></param>
        public void EnterPhaseType(PhaseType phaseType)
        {
            EnterPhaseEffectProcess enterPhaseEffectProcess = new EnterPhaseEffectProcess(phaseType, currentPlayer);
            currentPlayer.AddEffectProcess(enterPhaseEffectProcess);
        }

        public void SetCurrentPhaseType(PhaseType phaseType)
        {
            currentPhaseType = phaseType;
        }

        /// <summary>
        /// 重新设置决斗流程面板信息面板
        /// </summary>
        public void ResetPhaseTypePanelInfo()
        {
            duelSceneScript.ResetPhaseTypePanelInfo();
        }
        
        /// <summary>
        /// 停止战斗流程，一般是由效果中断
        /// </summary>
        public void StopBattlePhaseType()
        {
            if(currentPhaseType==PhaseType.Battle)
            {
                EnterPhaseType(PhaseType.Second);
            }
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
            //opponentPlayer.CallMonsterByProtocol(cardID, callType,fromCardGameState,toCardGameState,flag);
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
            //opponentPlayer.CallMonsterByProtocol(cardID, callType, fromCardGameState, toCardGameState, flag, sacrificeInfo);
        }

        /// <summary>
        /// 对方进入某个流程
        /// </summary>
        /// <param name="opponentPhaseType"></param>
        public void OpponentEnterPhaseType(PhaseType opponentPhaseType)
        {
            EnterPhaseType(opponentPhaseType);
        }

        /// <summary>
        /// 对方怪兽攻击
        /// </summary>
        /// <param name="cardID"></param>
        /// <param name="anotherCardID"></param>
        public void OpponentAttack(int cardID, int anotherCardID)
        {
            //CardBase card1 = opponentPlayer.GetCardByID(cardID);
            //CardBase card2 = myPlayer.GetCardByID(anotherCardID);
            //AttackMonster((MonsterCard)card1, (MonsterCard)card2);
        }

        /// <summary>
        /// 对方怪兽直接攻击
        /// </summary>
        /// <param name="cardID"></param>
        public void OpponentAttack(int cardID)
        {
            //CardBase card1 = opponentPlayer.GetCardByID(cardID);
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

        /// <summary>
        /// 获得场上的卡牌数量
        /// </summary>
        /// <returns></returns>
        public int GetCardNumberInArea()
        {
            return myPlayer.GetCardNumberInArea() + opponentPlayer.GetCardNumberInArea();
        }

        /// <summary>
        /// 检测玩家是否存在可以连锁发动效果的卡牌并返回玩家
        /// </summary>
        /// <returns></returns>
        public Player CheckCardCanChainLaunch()
        {
            if(startPlayer.CheckCardCanChainLaunch())
            {
                return startPlayer;
            }
            else if(startPlayer.GetOpponentPlayer().CheckCardCanChainLaunch())
            {
                return startPlayer.GetOpponentPlayer();
            }
            return null;
        }
    }
}
