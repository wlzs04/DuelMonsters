using Assets.Script.Duel;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 卡组信息,包括卡牌的No和ID
    /// key：cardGroupList(格式为"No-ID:No-ID")
    /// value：string("int-int:int-int")
    /// </summary>
    class CCardGroup : ClientProtocol
    {
        public override void Process()
        {
            GameManager.GetDuelScene().GetOpponentPlayer().SetCardGroup(GetContent("cardGroupList"));
        }
    }
}
