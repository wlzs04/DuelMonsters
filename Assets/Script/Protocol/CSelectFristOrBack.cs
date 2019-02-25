using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 选择先攻或后手
    /// </summary>
    class CSelectFristOrBack : ClientProtocol
    {
        public override void Process()
        {
            bool opponentSelectFrist = Convert.ToBoolean( GetContent("opponentSelectFrist"));
            GameManager.GetDuelScene().OpponentSelectFirstOrBack(opponentSelectFrist);
        }
    }
}
