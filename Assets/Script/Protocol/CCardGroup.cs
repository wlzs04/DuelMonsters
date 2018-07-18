using Assets.Script.Duel;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 卡组信息
    /// key：cardGroupList
    /// value：List
    /// </summary>
    class CCardGroup : ClientProtocol
    {
        public CCardGroup() : base("CCardGroup") { }

        public override ClientProtocol GetInstance()
        {
            return new CCardGroup();
        }

        public override void Process()
        {
            DuelScene duelScene = GameManager.GetSingleInstance().GetDuelScene();
            duelScene.SetCardGroupFromOpponent(GetContent("cardGroupList"));
        }
    }
}
