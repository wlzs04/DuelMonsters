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
    class NetPlayer:Player
    {
        public NetPlayer(DuelScene duelScene) : base("网络对手", duelScene)
        {
            
        }

        public override void InitBeforDuel()
        {
            heartPosition = new Vector3(DuelCommonValue.opponentHeartPositionX, DuelCommonValue.opponentHeartPositionY);

            lifeScrollBar = GameObject.Find("opponentLifeScrollbar").GetComponent<Scrollbar>();
            lifeNumberText = GameObject.Find("opponentLifeNumberText").GetComponent<Text>();

            life = DuelRuleManager.GetPlayerStartLife();

            List<CardBase> opponentCards = duelCardGroup.GetCards();
            for (int i = 0; i < opponentCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(opponentCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                opponentCards[i].SetCardObject(go);
                go.transform.SetParent(duelScene.duelBackImage.transform);
                go.transform.localPosition = new Vector3(DuelCommonValue.opponentCardGroupPositionX, DuelCommonValue.opponentCardGroupPositionY, 0);
            }
        }

        public override void DrawNotify()
        {
            CDrawCard cDrawCard = new CDrawCard();
            ClientManager.GetSingleInstance().SendProtocol(cDrawCard);
        }

        public override void EnterDuelNotify(DuelProcess duelProcess)
        {
            CEnterDuelProcess cEnterDuelProcess = new CEnterDuelProcess();
            cEnterDuelProcess.AddContent("duelProcess", duelProcess);
            ClientManager.GetSingleInstance().SendProtocol(cEnterDuelProcess);
        }
        
        public override void CallMonsterNotify(int id, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag)
        {
            CCallMonster cCallMonster = new CCallMonster();
            cCallMonster.AddContent("cardID", id);
            cCallMonster.AddContent("callType", callType);
            cCallMonster.AddContent("fromCardGameState", fromCardGameState);
            cCallMonster.AddContent("toCardGameState", toCardGameState);
            cCallMonster.AddContent("flag", flag);
            ClientManager.GetSingleInstance().SendProtocol(cCallMonster);
        }

        public override void CallMonsterWithSacrificeNotify(int id, CallType callType, CardGameState fromCardGameState, CardGameState toCardGameState, int flag, string sacrificeInfo)
        {
            CCallMonsterBySacrifice cCallMonsterBySacrifice = new CCallMonsterBySacrifice();
            cCallMonsterBySacrifice.AddContent("cardID", id);
            cCallMonsterBySacrifice.AddContent("callType", callType);
            cCallMonsterBySacrifice.AddContent("fromCardGameState", fromCardGameState);
            cCallMonsterBySacrifice.AddContent("toCardGameState", toCardGameState);
            cCallMonsterBySacrifice.AddContent("flag", flag);
            cCallMonsterBySacrifice.AddContent("sacrificeInfo", sacrificeInfo);
            ClientManager.GetSingleInstance().SendProtocol(cCallMonsterBySacrifice);
        }

        public override void EndTurnNotify()
        {
            CEndTurn cEndTurn = new CEndTurn();
            ClientManager.GetSingleInstance().SendProtocol(cEndTurn);
        }

        public override void BeAttackedMonsterNotify(int attackCardId, int beAttackedCardId)
        {
            CAttackMonster cAttackMonster = new CAttackMonster();
            cAttackMonster.AddContent("cardID", attackCardId);
            cAttackMonster.AddContent("anotherCardID", beAttackedCardId);
            ClientManager.GetSingleInstance().SendProtocol(cAttackMonster);
        }

        public override void BeDirectAttackedNotify(int attackCardId)
        {
            CAttackDirect cAttackDirect = new CAttackDirect();
            cAttackDirect.AddContent("cardID", attackCardId);
            ClientManager.GetSingleInstance().SendProtocol(cAttackDirect);
        }

        public override void StopDuelNotify()
        {
            CStopDuel cStopDuel = new CStopDuel();
            ClientManager.GetSingleInstance().SendProtocol(cStopDuel);
        }

        public override void SurrenderNotify()
        {
            CSurrender cSurrender = new CSurrender();
            ClientManager.GetSingleInstance().SendProtocol(cSurrender);
        }

        public override void SetCardGroupNotify(DuelCardGroup duelCardGroup)
        {
            CCardGroup cCardGroup = new CCardGroup();
            StringBuilder stringBuilder = new StringBuilder();
            List<CardBase> cards = duelCardGroup.GetCards();

            for (int i = 0; i < cards.Count - 1; i++)
            {
                stringBuilder.Append(cards[i].GetCardNo() + "-" + cards[i].GetID() + ":");
            }
            stringBuilder.Append(cards[cards.Count - 1].GetCardNo() + "-" + cards[cards.Count - 1].GetID());

            cCardGroup.AddContent("cardGroupList", stringBuilder.ToString());
            ClientManager.GetSingleInstance().SendProtocol(cCardGroup);
        }

        public override void GuessFirstNotify(GuessEnum guessEnum)
        {
            if (guessEnum == GuessEnum.Unknown)
            {
                return;
            }
            CGuessFirst guessFirst = new CGuessFirst();
            guessFirst.AddContent("guess", guessEnum.ToString());
            ClientManager.GetSingleInstance().SendProtocol(guessFirst);
        }

        public override void SelectFristOrBackNotify(bool opponentSelectFrist)
        {
            CSelectFristOrBack selectFristOrBack = new CSelectFristOrBack();
            selectFristOrBack.AddContent("opponentSelectFrist", opponentSelectFrist.ToString());
            ClientManager.GetSingleInstance().SendProtocol(selectFristOrBack);
        }
    }
}
