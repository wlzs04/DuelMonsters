using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Script.Duel
{
    /// <summary>
    /// 在决斗中一些固定值，如卡组在背景图片的位置等。
    /// </summary>
    class DuelCommonValue
    {
        public static readonly int myCardGroupPositionX = 237;
        public static readonly int myCardGroupPositionY = -241;

        public static readonly int myTombPositionX = 237;
        public static readonly int myTombPositionY = -50;

        public static readonly int opponentCardGroupPositionX = -247;
        public static readonly int opponentCardGroupPositionY = 239;

        public static readonly int opponentTombPositionX = -247;
        public static readonly int opponentTombPositionY = 48;

        public static readonly float cardGap = 77.5f;//卡片之间的间距

        public static readonly int cardOnBackFarLeftPositionX = -160;
        public static readonly int myMonsterCardPositionY = -100;
        public static readonly int myMagicTrapCardFarLeftPositionY = -190;
        public static readonly int opponentMonsterCardPositionY = 100;
        public static readonly int opponentMagicTrapCardFarLeftPositionY = 190;

        public static readonly int cardOnBackWidth = 60;
        public static readonly int cardOnBackHeight = 85;
        public static readonly int cardOnHandWidth = 80;
        public static readonly int cardOnHandHeight = 115;


    }
}
