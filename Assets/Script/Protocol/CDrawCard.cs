using Assets.Script.Duel;
using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    class CDrawCard : ClientProtocol
    {
        public override void Process()
        {
            DuelScene duelScene = GameManager.GetSingleInstance().GetDuelScene();
            duelScene.OpponentDraw();
        }
    }
}
