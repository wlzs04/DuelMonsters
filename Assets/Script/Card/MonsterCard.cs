using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Card
{
    enum MonsterType
    {
        Unknown,
        Water,
        Fire,
        Wind,
        Soil,
        Light,
        Dark
    }

    class MonsterCard : CardBase
    {
        int level = 4;//等级
        MonsterType monsterType = MonsterType.Dark;//属性
        bool normal = true;//是否为通常怪
        int attackNumber = 1500;//攻击力
        int defenseNumber = 500;//防御力
    }
}
