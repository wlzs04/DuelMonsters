using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    enum TrapType
    {
        Normal,//普通
        Forever,//永续
        BeatBack//反击
    }

    class TrapCard : CardBase
    {
        TrapType trapType = TrapType.Normal;
    }
}
