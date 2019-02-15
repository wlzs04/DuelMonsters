using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Duel
{
    /// <summary>
    /// 在决斗中一些固定值和相对值，如卡组在背景图片的位置等。
    /// </summary>
    class DuelCommonValue
    {
        public static readonly Vector2 myCardGroupPositionAnchor = new Vector2(0.83f, 0.08f);

        public static readonly Vector2 myTombPositionAnchor = new Vector2(0.83f, 0.41f);
        
        public static readonly Vector2 opponentCardGroupPositionAnchor = new Vector2(0.16f, 0.91f);

        public static readonly Vector2 opponentTombPositionAnchor = new Vector2(0.16f, 0.59f);

        public static readonly int cardOnBackFarLeftPositionX = -160;
        
        public static readonly Vector2 myMonsterCardPosition0Anchor = new Vector2(0.275f, 0.325f);
        public static readonly Vector2 myMonsterCardPosition1Anchor = new Vector2(0.385f, 0.325f);
        public static readonly Vector2 myMonsterCardPosition2Anchor = new Vector2(0.495f, 0.325f);
        public static readonly Vector2 myMonsterCardPosition3Anchor = new Vector2(0.6f, 0.325f);
        public static readonly Vector2 myMonsterCardPosition4Anchor = new Vector2(0.71f, 0.325f);

        public static readonly Vector2 myMagicTrapCardPosition0Anchor = new Vector2(0.275f, 0.18f);
        public static readonly Vector2 myMagicTrapCardPosition1Anchor = new Vector2(0.385f, 0.18f);
        public static readonly Vector2 myMagicTrapCardPosition2Anchor = new Vector2(0.495f, 0.18f);
        public static readonly Vector2 myMagicTrapCardPosition3Anchor = new Vector2(0.6f, 0.18f);
        public static readonly Vector2 myMagicTrapCardPosition4Anchor = new Vector2(0.71f, 0.18f);
        
        public static readonly Vector2 opponentMonsterCardPosition0Anchor = new Vector2(0.71f, 0.67f);
        public static readonly Vector2 opponentMonsterCardPosition1Anchor = new Vector2(0.6f, 0.67f);
        public static readonly Vector2 opponentMonsterCardPosition2Anchor = new Vector2(0.495f, 0.67f);
        public static readonly Vector2 opponentMonsterCardPosition3Anchor = new Vector2(0.385f, 0.67f);
        public static readonly Vector2 opponentMonsterCardPosition4Anchor = new Vector2(0.275f, 0.67f);

        public static readonly Vector2 opponentMagicTrapCardPosition0Anchor = new Vector2(0.71f, 0.82f);
        public static readonly Vector2 opponentMagicTrapCardPosition1Anchor = new Vector2(0.6f, 0.82f);
        public static readonly Vector2 opponentMagicTrapCardPosition2Anchor = new Vector2(0.495f, 0.82f);
        public static readonly Vector2 opponentMagicTrapCardPosition3Anchor = new Vector2(0.385f, 0.82f);
        public static readonly Vector2 opponentMagicTrapCardPosition4Anchor = new Vector2(0.275f, 0.82f);

        public static readonly int cardOnBackWidth = 60;
        public static readonly int cardOnBackHeight = 90;
        public static readonly int cardOnHandWidth = 80;
        public static readonly int cardOnHandHeight = 120;
    }
}
