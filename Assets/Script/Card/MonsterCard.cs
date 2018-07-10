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
        int level = 4;
        MonsterType monsterType = MonsterType.Dark;
        int attackNumber = 1500;
        int defenseNumber = 500;
    }
}
