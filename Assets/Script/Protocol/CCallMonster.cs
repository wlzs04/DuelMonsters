using Assets.Script.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Protocol
{
    /// <summary>
    /// 召唤怪兽
    /// key：cardID 
    /// value：int  
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
            
        }
    }
}
