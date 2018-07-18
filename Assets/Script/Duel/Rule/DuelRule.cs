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

        public static readonly int monsterAreaNumber = 5;

    }
}
