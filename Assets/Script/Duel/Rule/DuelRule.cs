using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.Rule
{
    enum DuelProcess
    {
        Unknown,
        Draw,
        Prepare,
        Main,
        Battle,
        Second,
        End
    }

    class DuelRule
    {
        public static readonly int drawCardNumberOnFirstDraw = 5;
        public static readonly int drawCardNumberEveryTurn = 1;
        public static readonly int callMonsterWithoutSacrificeMaxLevel = 4;
        public static readonly int callMonsterWithOneSacrificeMaxLevel = 6;

        public static readonly int monsterAreaNumber = 5;
        public static readonly int monsterATKNumberEveryTurn = 1;

        public static readonly int startLife = 4000;
        public static readonly int lostLife = 0;


    }
}
