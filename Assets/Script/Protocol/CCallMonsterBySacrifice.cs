using Assets.Script.Card;
using Assets.Script.Duel;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 使用祭品召唤怪兽
    /// key：cardID callType fromCardGameState toCardGameState flag sacrificeNumber sacrificeInfo(格式为"ID:ID")
    /// value：int CallType CardGameState CardGameState int string
    /// </summary>
    class CCallMonsterBySacrifice : ClientProtocol
    {
        public override void Process()
        {
            int cardID = int.Parse(GetContent("cardID"));
            CallType callType = (CallType)Enum.Parse(typeof(CallType), GetContent("callType"));
            CardGameState fromCardGameState = (CardGameState)Enum.Parse(typeof(CardGameState), GetContent("fromCardGameState"));
            CardGameState toCardGameState = (CardGameState)Enum.Parse(typeof(CardGameState), GetContent("toCardGameState"));
            int flag = int.Parse(GetContent("flag"));

            string sacrificeInfo = GetContent("sacrificeInfo");

            GameManager.GetDuelScene().OpponentCallMonsterBySacrifice(cardID, callType, fromCardGameState, toCardGameState, flag, sacrificeInfo);
        }
    }
}
