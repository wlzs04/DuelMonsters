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
using UnityEngine.UI;

namespace Assets.Script.Duel
{
    /// <summary>
    /// 决斗模式
    /// </summary>
    enum DuelMode
    {
        Unknown,//未知
        Single,//单人
        Net//网络
    }

    class DuelScene
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
        }

        void AddControlFromScene()
        {
            duelBackImage = GameObject.Find("duelBackImage").GetComponent<Image>();
            environmentImage = GameObject.Find("environmentImage").GetComponent<Image>();
            attackAnimationScript = GameObject.Find("attackImage").GetComponent<AttackAnimationScript>();
            attackAnimationScript.StopPlay();
            cardImage = GameObject.Find("cardImage").GetComponent<Image>();
            myHandCardPanel = GameObject.Find("myHandCardPanel");
            opponentHandCardPanel = GameObject.Find("opponentHandCardPanel");
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

        public void ShowMessage(string value)
        {
            GameManager.ShowMessage(value);
        }

        public void MouseRightButtonDown()
        {

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

        public void SetAttackAnimationFinishEvent(object v)
        {
            throw new NotImplementedException();
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
        /// 检测胜负
        /// </summary>
        public void CheckWin()
        {
            bool ilost = myPlayer.GetLife() <= DuelRule.lostLife;
            bool iWin = opponentPlayer.GetLife() <= DuelRule.lostLife;
            if(!(iWin|| ilost))
            {
                return;
            }

            if (iWin && ilost)
            {
                WinAndLost();
            }
            else if(iWin)
            {
                IWin();
            }
            else if (ilost)
            {
                ILost();
            }
        }

        /// <summary>
        /// 我赢了
        /// </summary>
        public void IWin()
        {
            ShowMessage("我赢了!");
            Thread.Sleep(2000);
            GameManager.GetSingleInstance().EnterMainScene();
        }

        /// <summary>
        /// 我输了
        /// </summary>
        public void ILost()
        {
            ShowMessage("我输了!");
            Thread.Sleep(2000);
            GameManager.GetSingleInstance().EnterMainScene();
        }

        /// <summary>
        /// 平局
        /// </summary>
        public void WinAndLost()
        {
            ShowMessage("平局！");
            Thread.Sleep(2000);
            GameManager.GetSingleInstance().EnterMainScene();
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
            SetMyCardGroup();
            if(duelMode==DuelMode.Net)
            {
                SendCardGroupToOpponent();
            }
            else
            {
                DuelCardGroup duelCardGroup = new DuelCardGroup();
                opponentPlayer.SetCardGroup(duelCardGroup);
                UserCardGroup firstCardGroup = GameManager.GetSingleInstance().GetUserData().userCardGroupList[0];
                Debug.LogWarning("电脑暂时使用第一个卡组。");

                foreach (var item in firstCardGroup.mainCardList)
                {
                    for (int i = 0; i < item.number; i++)
                    {
                        duelCardGroup.AddCard(item.cardNo);
                    }
                }
                opponentPlayer.ShuffleCardGroup();
            }
            CheckPlayInit();
        }

        void CheckPlayInit()
        {
            if(myPlayer!=null&& opponentPlayer!=null)
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
                myPlayer.SetHandPanel(myHandCardPanel);
                opponentPlayer.SetHandPanel(opponentHandCardPanel);
                duelSceneScript = GameObject.Find("Main Camera").GetComponent<DuelSceneScript>();
                StartDuel();
            }
        }

        /// <summary>
        /// 发送我方卡组信息到对方
        /// </summary>
        void SendCardGroupToOpponent()
        {
            CCardGroup cCardGroup = new CCardGroup();
            StringBuilder stringBuilder = new StringBuilder();
            List<CardBase> cards = myPlayer.GetDuelCardGroup().GetCards();

            for (int i = 0; i < cards.Count - 1; i++)
            {
                stringBuilder.Append(cards[i].GetCardNo() + "-" + cards[i].GetID() + ":");
            }
            stringBuilder.Append(cards[cards.Count - 1].GetCardNo() + "-" + cards[cards.Count - 1].GetID());

            cCardGroup.AddContent("cardGroupList", stringBuilder.ToString());
            ClientManager.GetSingleInstance().SendProtocol(cCardGroup);
        }

        /// <summary>
        /// 设置对方卡组信息
        /// </summary>
        public void SetCardGroupFromOpponent(string cardGroupInfo)
        {
            DuelCardGroup duelCardGroup = new DuelCardGroup();
            opponentPlayer.SetCardGroup(duelCardGroup);
            string[] cardNos = cardGroupInfo.Split(':');
            foreach (var item in cardNos)
            {
                duelCardGroup.AddCard(int.Parse(item.Substring(0,item.IndexOf('-'))), int.Parse(item.Substring(item.IndexOf('-')+1)));
            }
            CheckPlayInit();
        }

        /// <summary>
        /// 一场决斗中只调用一次，用来设置卡组信息。
        /// </summary>
        void SetMyCardGroup()
        {
            DuelCardGroup duelCardGroup = new DuelCardGroup();
            myPlayer.SetCardGroup(duelCardGroup);

            UserCardGroup firstCardGroup = GameManager.GetSingleInstance().GetUserData().userCardGroupList[0];
            Debug.LogWarning("我方暂时使用第一个卡组。");

            foreach (var item in firstCardGroup.mainCardList)
            {
                for (int i = 0; i < item.number; i++)
                {
                    duelCardGroup.AddCard(item.cardNo);
                }
            }
            myPlayer.ShuffleCardGroup();
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        //void ShuffleCardGroup()
        //{
            
        //}

        /// <summary>
        /// 开始决斗
        /// </summary>
        void StartDuel()
        {
            myPlayer.SetLife(GameObject.Find("myLifeScrollbar").GetComponent<Scrollbar>());
            opponentPlayer.SetLife(GameObject.Find("opponentLifeScrollbar").GetComponent<Scrollbar>());

            myPlayer.SetOpponentPlayer(opponentPlayer);
            opponentPlayer.SetOpponentPlayer(myPlayer);

            myPlayer.SetHeartPosition(new Vector3(DuelCommonValue.myHeartPositionX, DuelCommonValue.myHeartPositionY));
            opponentPlayer.SetHeartPosition(new Vector3(DuelCommonValue.opponentHeartPositionX, DuelCommonValue.opponentHeartPositionY));

            InitCardGroup();
            ShowMessage("抽牌！");

            myPlayer.DrawAtFirst();
            opponentPlayer.DrawAtFirst();

            ShowMessage("决斗开始！");
            startPlayer.StartTurn();
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

        void InitCardGroup()
        {
            cardPre = Resources.Load<GameObject>(cardPrefabPath);
            myPlayer.InitCardGroup();
            opponentPlayer.InitCardGroup();
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
        /// 由玩家调用，代表进入下一阶段
        /// </summary>
        public void EnterNextDuelProcess()
        {
            switch (currentDuelProcess)
            {
                case DuelProcess.Unknown:
                    EnterDuelProcess(DuelProcess.Draw);
                    break;
                case DuelProcess.Draw:
                    EnterDuelProcess(DuelProcess.Prepare);
                    break;
                case DuelProcess.Prepare:
                    EnterDuelProcess(DuelProcess.Main);
                    break;
                case DuelProcess.Main:
                    EnterDuelProcess(DuelProcess.End);
                    Debug.LogError("EnterNextDuelProcess DuelProcess.Main");
                    break;
                case DuelProcess.Battle:
                    EnterDuelProcess(DuelProcess.Second);
                    break;
                case DuelProcess.Second:
                    EnterDuelProcess(DuelProcess.End);
                    break;
                case DuelProcess.End:
                    EnterDuelProcess(DuelProcess.Unknown);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 向玩家显示当前流程
        /// </summary>
        /// <param name="duelProcess"></param>
        public void EnterDuelProcess(DuelProcess duelProcess)
        {
            currentDuelProcess = duelProcess;
            string ex = currentPlayer == myPlayer ? "我方进入" : "对方进入";
            switch (duelProcess)
            {
                case DuelProcess.Unknown:
                    ShowMessage(ex + "未知流程！");
                    break;
                case DuelProcess.Draw:
                    ShowMessage(ex + "抽牌流程！");
                    break;
                case DuelProcess.Prepare:
                    ShowMessage(ex + "准备流程！");
                    break;
                case DuelProcess.Main:
                    ShowMessage(ex + "主要流程！");
                    break;
                case DuelProcess.Battle:
                    ShowMessage(ex + "战斗流程！");
                    break;
                case DuelProcess.Second:
                    ShowMessage(ex + "第二主要流程！");
                    break;
                case DuelProcess.End:
                    ShowMessage(ex + "结束流程！");
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
