using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    enum MagicType
    {
        Normal,//普通
        Quick,//速攻
        Environment,//环境
        Equipment,//装备
        Forever//永续
    }

    class MagicCard : CardBase
    {
        MagicType magicType = MagicType.Normal;
    }
}
