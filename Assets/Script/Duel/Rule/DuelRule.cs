using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel.Rule
{
    /// <summary>
    /// 回合中的流程阶段
    /// </summary>
    enum DuelProcess
    {
        Unknown,//未知
        Draw,//抽卡
        Prepare,//准备
        Main,//主要
        Battle,//战斗
        Second,//第二主要
        End//结束
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
