using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Script.Card
{
    enum CardType
    {
        Monster,
        Magic,
        trap
    }

    enum CardGameState
    {
        Group,//在卡组中
        Hand,//手牌中
        Front,//表侧表示
        Back,//覆盖表示
        Tomb,//在墓地中
        Exclusion//被排除在游戏外
    }

    class CardBase
    {
        CardType cardType = CardType.Monster;//卡片类型
        CardGameState cardGameState = CardGameState.Group;
        Sprite image;
        string name = "未命名";//名称
        int cardNO = 0;//唯一编号，0代表为假卡。
        string effect = "没有效果。";

        internal void SetImage(Sprite image)
        {
            this.image = image;
        }

        internal Sprite GetImage()
        {
            return image;
        }
    }
}
