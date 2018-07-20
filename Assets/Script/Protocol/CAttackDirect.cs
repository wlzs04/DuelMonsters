using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 怪兽直接攻击
    /// key：cardID
    /// value：int
    /// </summary>
    class CAttackDirect : ClientProtocol
    {
        public CAttackDirect() : base("CAttackDirect") { }

        public override ClientProtocol GetInstance()
        {
            return new CAttackDirect();
        }

        public override void Process()
        {
            int cardID = int.Parse(GetContent("cardID"));
            GameManager.GetSingleInstance().GetDuelScene().OpponentAttack(cardID);
        }
    }
}
