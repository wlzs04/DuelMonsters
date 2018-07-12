using Assets.Script.Duel.Rule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Duel
{
    class DuelScene
    {
        Player myPlayer;//我方玩家
        Player opponentPlayer;//敌方玩家

        Player currentPlayer;//当前玩家
        Player startPlayer;//开始玩家
        int currentTurnNumber = 1;//当前回合数
        DuelProcess currentDuelProcess= DuelProcess.First;//当前流程

        void StartDuel()
        {

        }

        void EndDuel()
        {

        }
    }
}
