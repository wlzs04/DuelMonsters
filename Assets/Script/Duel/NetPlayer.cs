using Assets.Script.Card;
using Assets.Script.Duel.Rule;
using Assets.Script.Net;
using Assets.Script.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Duel
{
    class NetPlayer:Player
    {
        public NetPlayer(DuelScene duelScene) : base("网络对手", duelScene)
        {


        }

        public override void InitCardGroup()
        {
            List<CardBase> opponentCards = duelCardGroup.GetCards();
            for (int i = 0; i < opponentCards.Count; i++)
            {
                GameObject go = GameObject.Instantiate(duelScene.cardPre, duelScene.duelBackImage.transform);
                go.GetComponent<DuelCardScript>().SetCard(opponentCards[i]);
                go.GetComponent<DuelCardScript>().SetOwner(this);
                opponentCards[i].cardObject = go;
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
    }
}
