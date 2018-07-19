using Assets.Script.Card;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 召唤怪兽
    /// key：cardID callType fromCardGameState toCardGameState flag
    /// value：int CallType CardGameState CardGameState int
    /// </summary>
    class CCallMonster : ClientProtocol
    {
        public CCallMonster() : base("CCallMonster") { }

        public override ClientProtocol GetInstance()
        {
            return new CCallMonster();
        }

        public override void Process()
        {
            int cardID = int.Parse(GetContent("cardID"));
            CallType callType = (CallType)Enum.Parse(typeof(CallType),GetContent("callType"));
            CardGameState fromCardGameState = (CardGameState)Enum.Parse(typeof(CardGameState), GetContent("fromCardGameState"));
            CardGameState toCardGameState = (CardGameState)Enum.Parse(typeof(CardGameState), GetContent("toCardGameState"));
            int flag = int.Parse(GetContent("flag"));

            GameManager.GetSingleInstance().GetDuelScene().OpponentCallMonster(cardID, callType, fromCardGameState, toCardGameState, flag);
        }
    }
}
