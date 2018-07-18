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
    class DuelScene
    {
        string cardPrefabPath = "Prefab/CardPre";
        GameObject cardPre = null;

        public Player myPlayer;//我方玩家
        public Player opponentPlayer;//敌方玩家

        public bool myFirst = false;
        public Player currentPlayer;//当前玩家
        public Player startPlayer;//开始玩家
        public int currentTurnNumber = 1;//当前回合数
        public DuelProcess currentDuelProcess = DuelProcess.Unknown;//当前流程

        public Image duelBackImage = null;
        public Image environmentImage = null;
        public Image cardImage = null;
        public GameObject myHandCardPanel = null;
        public GameObject opponentHandCardPanel = null;

        DuelSceneScript duelSceneScript = null;

        public void AddControlFromScene()
        {
            GameObject go = GameObject.Find("duelBackImage");
            environmentImage = GameObject.Find("environmentImage").GetComponent<Image>();
            duelBackImage = go.GetComponent<Image>();
            cardImage = GameObject.Find("cardImage").GetComponent<Image>();
            myHandCardPanel = GameObject.Find("myHandCardPanel");
            opponentHandCardPanel = GameObject.Find("opponentHandCardPanel");
        }

        public void SetFirst(bool myFirst)
        {
            this.myFirst = myFirst;
        }

        public void ShowMessage(string value)
        {
            GameManager.ShowMessage(value);
        }

        /// <summary>
        /// 初始化决斗场景
        /// </summary>
        /// <param name="myFirst"></param>
        public void Init()
        {
            AddControlFromScene();
            SetMyCardGroup();
            SendCardGroupToOpponent();
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
            List<CardBase> cards=myPlayer.GetDuelCardGroup().GetCards();

            for (int i = 0; i < cards.Count-1; i++)
            {
                stringBuilder.Append(cards[i].GetCardNo() + ":");
            }
            stringBuilder.Append(cards[cards.Count - 1].GetCardNo());

            cCardGroup.AddContent("cardGroupList", stringBuilder.ToString());
            ClientManager.GetInstance().SendProtocol(cCardGroup);
        }

        /// <summary>
        /// 设置对方卡组信息
        /// </summary>
        public void SetCardGroupFromOpponent(string cardGroupInfo)
        {
            DuelCardGroup duelCardGroup = new DuelCardGroup();
            opponentPlayer = new Player("对方",duelCardGroup);
            string[] cardNos = cardGroupInfo.Split(':');
            foreach (var item in cardNos)
            {
                duelCardGroup.AddCard(int.Parse(item));
            }
            CheckPlayInit();
        }

        /// <summary>
        /// 一场决斗中只调用一次，用来设置卡组信息。
        /// </summary>
        void SetMyCardGroup()
        {
            DuelCardGroup duelCardGroup = new DuelCardGroup();
            myPlayer = new Player("我方",duelCardGroup);
            foreach (var item in GameManager.GetSingleInstance().Userdata.userCardGroupList)
            {
                for (int i = 0; i < item.number; i++)
                {
                    duelCardGroup.AddCard(item.cardNo);
                }
            }
            ShuffleCardGroup();
        }

        /// <summary>
        /// 洗牌
        /// </summary>
        void ShuffleCardGroup()
        {
            myPlayer.ShuffleCardGroup();
        }

        /// <summary>
        /// 开始决斗
        /// </summary>
        void StartDuel()
        {
            InitCardGroup();
            ShowMessage("抽牌！");
            for (int i = 0; i < DuelRule.drawCardNumberOnFirstDraw; i++)
            {
                myPlayer.Draw();
            }
            ShowMessage("决斗开始！");
            currentDuelProcess = DuelProcess.Unknown;
            if(myFirst)
            {
                currentPlayer.StartTurn();
            }
        }

        void EndDuel()
        {

        }

        void InitCardGroup()
        {
            cardPre = Resources.Load<GameObject>(cardPrefabPath);
            List<CardBase> myCards = myPlayer.GetDuelCardGroup().GetCards();
            for (int i = 0; i < myCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(cardPre, duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(myCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(myPlayer);
                myCards[i].cardObject = go;
                go.transform.SetParent(duelBackImage.transform);
                go.transform.localPosition = new Vector3(DuelCommonValue.myCardGroupPositionX, DuelCommonValue.myCardGroupPositionY,0);
            }

            List<CardBase> opponentCards = opponentPlayer.GetDuelCardGroup().GetCards();
            for (int i = 0; i < opponentCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(cardPre, duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(opponentCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(opponentPlayer);
                opponentCards[i].cardObject = go;
                go.transform.SetParent(duelBackImage.transform);
                go.transform.localPosition = new Vector3(DuelCommonValue.opponentCardGroupPositionX, DuelCommonValue.opponentCardGroupPositionY, 0);
            }
        }

        /// <summary>
        /// 带有Opponent前缀的方法代表对方的操作同步到我方。
        /// </summary>
        public void OpponentDraw()
        {
            opponentPlayer.Draw();
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
                    currentDuelProcess = DuelProcess.Draw;
                    break;
                case DuelProcess.Draw:
                    currentDuelProcess = DuelProcess.Prepare;
                    break;
                case DuelProcess.Prepare:
                    currentDuelProcess = DuelProcess.Main;
                    break;
                case DuelProcess.Main:
                    currentDuelProcess = DuelProcess.End;
                    Debug.LogError("EnterNextDuelProcess DuelProcess.Main");
                    break;
                case DuelProcess.Battle:
                    currentDuelProcess = DuelProcess.Second;
                    break;
                case DuelProcess.Second:
                    currentDuelProcess = DuelProcess.End;
                    break;
                case DuelProcess.End:
                    currentDuelProcess = DuelProcess.End;
                    Debug.LogError("EnterNextDuelProcess DuelProcess.End");
                    break;
                default:
                    break;
            }
            EnterDuelProcess(currentDuelProcess);
        }

        /// <summary>
        /// 向玩家显示当前流程
        /// </summary>
        /// <param name="duelProcess"></param>
        void EnterDuelProcess(DuelProcess duelProcess)
        {
            switch (duelProcess)
            {
                case DuelProcess.Unknown:
                    ShowMessage("未知流程！");
                    break;
                case DuelProcess.Draw:
                    ShowMessage("抽牌流程！");
                    break;
                case DuelProcess.Prepare:
                    ShowMessage("准备流程！");
                    break;
                case DuelProcess.Main:
                    ShowMessage("主要流程！");
                    break;
                case DuelProcess.Battle:
                    ShowMessage("战斗流程！");
                    break;
                case DuelProcess.Second:
                    ShowMessage("第二主要流程！");
                    break;
                case DuelProcess.End:
                    ShowMessage("结束流程！");
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 将怪兽召唤到怪兽区
        /// </summary>
        public bool AddMonsterCardToDuelArea(MonsterCard monsterCard,CallType callType)
        {
            return false;
        }
    }
}
